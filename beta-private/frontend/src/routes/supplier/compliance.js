const _ = require('lodash')
const router = require('express').Router({ strict: true, mergeParams: true })
const path = require('path')

const { dataProvider } = require('catalogue-data')
const { sharePointProvider } = require('catalogue-sharepoint')

const multer = require('multer')
const storage = multer.memoryStorage()
const upload = multer({ storage: storage }).any()

// Multer is required to be used before CSRF for multi-part form parsing
router.use(upload)

// all routes in this module require CSRF protection
router.use(require('csurf')())

// all routes need to load a specified solution
router.param('solution_id', async (req, res, next, solutionId) => {
  try {
    req.solution = await dataProvider.solutionForCompliance(solutionId)
    next()
  } catch (err) {
    next(err)
  }
})

router
  .route('/:solution_id/')
  .get(solutionComplianceDashboard)

router
  .route('/:solution_id/evidence/:claim_id/')
  .get(solutionComplianceEvidencePageGet)

router
  .route('/:solution_id/evidence/:claim_id/')
  .post(solutionComplianceEvidencePagePost)

router
  .route('/:solution_id/evidence/:claim_id/:file_name')
  .get(downloadEvidenceGet)

router
  .route('/:solution_id/evidence/:claim_id/confirmation')
  .get(solutionComplianceEvidenceConfirmationGet)

router
  .route('/:solution_id/evidence/:claim_id/confirmation')
  .post(solutionComplianceEvidenceConfirmationPost)

function commonComplianceContext (req) {
  return {
    solution: req.solution,
    csrfToken: req.csrfToken(),
    activeForm: {
      title: req.solution && _([req.solution.name, req.solution.version]).filter().join(', ')
    }
  }
}

async function dashboardContext (req) {
  return {
    ...commonComplianceContext(req),
    ...await dataProvider.capabilityMappings()
  }
}

async function solutionComplianceDashboard (req, res) {
  const context = {
    ...await dashboardContext(req)
  }

  context.solution.standards = _(context.solution.standards)
    .map(std => ({
      ...context.standards[std.standardId],
      ...std,
      continueUrl: `evidence/${std.id}/`
    }))
    .value()

  if ('submitted' in req.query) {
    const submittedStandard = context.standards[req.query.submitted]
    if (submittedStandard) {
      context.submittedStandard = submittedStandard.name
    }
  }

  res.render('supplier/compliance/index', context)
}

function formatTimestampForDisplay (ts) {
  const timestamp = new Date(ts)
  return timestamp.toLocaleDateString('en-gb', {
    timeZone: 'Europe/London',
    day: 'numeric',
    month: 'long',
    year: 'numeric'
  }) + ' ' + timestamp.toLocaleTimeString('en-gb', {
    timeZone: 'Europe/London',
    hour: 'numeric',
    minute: '2-digit',
    hour12: false
  })
}

async function evidencePageContext (req) {
  const context = {
    ...commonComplianceContext(req),
    ...await dataProvider.capabilityMappings()
  }

  context.claim = _.find(context.solution.standards, { id: req.params.claim_id })
  context.claim.standard = context.standards[context.claim.standardId]

  context.claim.capabilities = _.filter(context.capabilities,
    cap => _.some(context.solution.capabilities, { capabilityId: cap.id }) &&
           _.some(cap.standards, { id: context.claim.standardId })
  )

  // compute the message history from the relevant evidence and reviews
  context.claim.submissionHistory = _(context.solution.evidence)
    .filter({ claimId: context.claim.id })
    .flatMap(ev => [ev, ..._.filter(context.solution.reviews, { evidenceId: ev.id })])
    .map(({ id, createdOn, createdById, message, evidence }) => ({
      id,
      createdOn,
      createdById,
      message: message || evidence,
      isFeedback: !!message
    }))
    .orderBy('createdOn', 'asc')
    .each(msg => {
      msg.createdOn = formatTimestampForDisplay(msg.createdOn)
    })

  // set flags based on who sent the last message
  if (context.claim.submissionHistory.length) {
    const latestEntry = _.last(context.claim.submissionHistory)
    context.collapseHistory = context.claim.submissionHistory.length > 1
    context.hasFeedback = latestEntry.isFeedback
    context.feedbackId = context.hasFeedback && latestEntry.id
  }

  context.isSubmitted = +context.claim.status === 2

  // load the contacts associated with the message history
  context.claim.historyContacts = _.keyBy(await Promise.all(
    _(context.claim.submissionHistory)
      .uniqBy('createdById')
      .map('createdById')
      .map(id => dataProvider.contactById(id))
  ), 'id')

  context.errors = req.body.errors || []

  try {
    context.files = await fetchFiles(context.claim.id)
  } catch (err) {
    context.errors.push(err)
  }

  return context
}

async function solutionComplianceEvidencePageGet (req, res) {
  const context = {
    ...await evidencePageContext(req),
    errors: req.body.errors || []
  }

  let latestFile

  if (context.files) {
    latestFile = findLatestFile(context.files.items)
  }

  if (latestFile) {
    latestFile.downloadURL = path.join(req.baseUrl, req.path, latestFile.name)

    context.latestFile = latestFile

    // only allow a submission if a file exists and;
    // a) evidence has not already been submitted, or
    // b) the latest submission is feedback from NHS Digital
    context.allowSubmission = !context.isSubmitted || context.hasFeedback
    if (context.allowSubmission) {
      context.activeForm.id = 'compliance-evidence-upload'
    }
  }

  res.render('supplier/compliance/evidence', context)
}

async function solutionComplianceEvidencePagePost (req, res) {
  const action = req.body.action || {}

  let redirectUrl = action.save
    ? './'
    : '../../'

  if (!req.files.length) {
    if (action.submit) {
      req.body.errors = { items: [{ msg: 'No file to upload.' }] }
    } else {
      res.redirect(redirectUrl)
      return
    }
  } else {
    const fileToUpload = req.files[0]

    try {
      await uploadFile(req.params.claim_id, fileToUpload.buffer, fileToUpload.originalname)

      // update the status of the claim based on the file uploading successfully (not started -> draft)
      // and if the user requested submission to NHS Digital (* -> submitted)
      const claim = _.find(req.solution.standards, { id: req.params.claim_id })

      if (action.submit) {
        redirectUrl = path.join(req.baseUrl, req.url, 'confirmation')
      }

      claim.status = '1' /* draft */

      // always write an evidence record
      // TODO: link previousId when appropriate
      req.solution.evidence.push({
        id: require('node-uuid-generator').generate(),
        claimId: req.params.claim_id,
        createdOn: new Date(),
        createdById: req.user.contact.id,
        evidence: req.body.message
      })

      await dataProvider.updateSolutionForCompliance(req.solution)

      // redirect accourdingly
      res.redirect(redirectUrl)
      return
    } catch (err) {
      console.log(err)
      req.body.errors = { items: [{ msg: String(err) }] }
    }
  }

  // re-render if an error occurred
  solutionComplianceEvidencePageGet(req, res)
}

async function downloadEvidenceGet (req, res) {
  const context = {
    ...await evidencePageContext(req)
  }
  req.body.errors = req.body.errors || []

  const claimID = req.params.claim_id
  const fileName = req.params.file_name

  const selectedFile = _.find(context.files.items, ['name', fileName])

  downloadFile(claimID, selectedFile.blobId).then((fileResponse) => {
    res.header('Content-Disposition', `attachment; filename="${fileName}"`)
    fileResponse.body.pipe(res)
  }).catch((err) => {
    req.body.errors.push(err)
    solutionComplianceEvidencePageGet(req, res)
  })
}

async function solutionComplianceEvidenceConfirmationGet (req, res) {
  const context = {
    ...await evidencePageContext(req)
  }

  const claim = _.find(req.solution.standards, { id: req.params.claim_id })

  await dataProvider.updateSolutionForCompliance(req.solution)

  res.render('supplier/compliance/confirmation', context)
}

async function solutionComplianceEvidenceConfirmationPost (req, res) {
  const context = {
    ...await evidencePageContext(req)
  }
  const action = req.body.action || {}

  let redirectUrl = action.save
    ? './'
    : '../../'

  const claim = _.find(req.solution.standards, { id: req.params.claim_id })

  if (action.submit) {
    claim.status = '2' /* submitted */
    redirectUrl += `?submitted=${claim.standardId}`
  }

  res.render('supplier/compliance/confirmation', context)
}

async function downloadFile (claimID, blobId) {
  return sharePointProvider.downloadStdEvidence(claimID, blobId)
}

async function fetchFiles (claimID) {
  return sharePointProvider.getStdEvidenceFiles(claimID)
}

function findLatestFile (files) {
  return _.maxBy(files, (f) => f.timeLastModified)
}

async function uploadFile (claimID, buffer, fileName) {
  return sharePointProvider.uploadStdEvidence(claimID, buffer, fileName)
}

module.exports = router
