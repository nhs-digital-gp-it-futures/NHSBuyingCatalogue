const _ = require('lodash')
const router = require('express').Router({ strict: true, mergeParams: true })
const path = require('path')

const { dataProvider } = require('catalogue-data')
const { sharePointProvider } = require('catalogue-sharepoint')

const { checkSchema, validationResult } = require('express-validator/check')
const capabilitiesPageValidation = checkSchema(require('./capabilities-validation').capabilities)

const multer = require('multer')
const storage = multer.memoryStorage()
const upload = multer({ storage: storage }).any()

const LIVE_DEMO_MESSSAGE_INDICATOR = 'A live demonstration was requested for this Capability.'

// Multer is required to be used before CSRF for multi-part form parsing
router.use(upload)

// all routes in this module require CSRF protection
router.use(require('csurf')())

// all routes need to load a specified solution
router.param('solution_id', async (req, res, next, solutionId) => {
  try {
    req.solution = await dataProvider.solutionForAssessment(solutionId)
    next()
  } catch (err) {
    next(err)
  }
})

router
  .route('/:solution_id/')
  .get(solutionCapabilityPageGet)

router
  .route('/:solution_id/')
  .post(capabilitiesPageValidation, solutionCapabilityPagePost)

router
  .route('/:solution_id/:claim_id/:file_name')
  .get(downloadEvidenceGet)

router
  .route('/:solution_id/confirmation')
  .get(confirmationPageGet)

router
  .route('/:solution_id/confirmation')
  .post(confirmationPagePost)

function commonContext (req) {
  return {
    solution: req.solution,
    csrfToken: req.csrfToken(),
    activeForm: {
      title: req.solution && _([req.solution.name, req.solution.version]).filter().join(', ')
    }
  }
}

async function capabilityPageContext (req) {
  const context = {
    ...commonContext(req),
    ...await dataProvider.capabilityMappings()
  }

  context.activeForm.id = 'capability-assessment-form'

  context.solution.capabilities = await Promise.all(
    context.solution.capabilities.map(async (cap) => {
      const files = await fetchFiles(cap.claimID).catch((err) => {
        context.errors = { items: [{ msg: 'Validation.Capability.Evidence.Retrieval.FailedAction', err: err }] }
      })
      let latestFile

      if (files) {
        latestFile = findLatestFile(files.items)
      }

      if (latestFile) {
        latestFile.downloadURL = path.join(req.baseUrl, req.path, cap.claimID, latestFile.name)
      }

      const latestEvidence = findLatestEvidence(cap.evidence)

      return {
        ...cap,
        latestFile: latestFile,
        latestEvidence: latestEvidence,
        isUploadingEvidence: latestEvidence ? !latestEvidence.hasRequestedLiveDemo : true
      }
    })
  )

  return context
}

async function solutionCapabilityPageGet (req, res) {
  const context = {
    ...await capabilityPageContext(req)
  }

  // page is only editable if the solution is registered, and notyet submitted for assessment
  context.notEditable = context.solution.status !== dataProvider.getRegisteredStatusCode()

  if (context.notEditable) {
    delete context.activeForm.id
  }

  res.render('supplier/capabilities/index', context)
}

async function solutionCapabilityPagePost (req, res) {
  const context = {
    ...await capabilityPageContext(req)
  }

  const valRes = validationResult(req)

  // Check if any validation Erors occured
  if (!valRes.isEmpty()) {
    const validationErrors = {
      items: valRes.array({ onlyFirstError: true }),
      controls: _.mapValues(valRes.mapped(), res => ({
        ...res,
        action: res.msg + 'Action'
      }))
    }
    context.errors = validationErrors
  }

  // With the lack of anything other than a single string for adding additional evidence
  // indicating that a live demo has been requested instead of using video evidence
  // must be indicated using the same string that evidence messages are entered.
  //
  // For that reason, if the supplier wants to do a live demo, then a string is
  // shoved into the evidence message field.
  //
  // TODO: 🦄 previousId
  // The API this program interfaces with requires us to generate our own ID's
  // This is couter intuitive and has a tonne of flaws, the hope is eventually a redesign
  // of the API will result in the endpoint only requiring us to Post the Evidence message
  // and not make up an ID, Date, CreatedById, previousId etc.

  const uploadingVideoEvidence = req.body['uploading-video-evidence']

  const evidenceDescriptions = _.map(uploadingVideoEvidence, (isUploading, claimID) => {
    return {
      id: require('node-uuid-generator').generate(),
      claimId: claimID,
      createdById: req.user.contact.id,
      createdOn: new Date().toISOString(),
      evidence: isUploading === 'yes' ? req.body['evidence-description'][claimID] : LIVE_DEMO_MESSSAGE_INDICATOR,
      hasRequestedLiveDemo: isUploading !== 'yes',
      blobId: null
    }
  })


  let errorUpdating = ''
  // Update solution evidence, communicate the error if there isn't any already.
  await updateSolutionCapabilityEvidence(req.solution.id, evidenceDescriptions).catch((err) => {
    context.errors = context.errors || {
      items: [{ msg: 'Validation.Capability.Evidence.Update.FailedAction' }]
    }
    errorUpdating = err
  })

  let errorUploading = ''
  // upload files to SharePoint, communicate the error if there isn't any already.
  if (req.files) {
    const files = parseFiles(req.files)
    await uploadFiles(files).catch((err) => {
      context.errors = context.errors || {
        items: [{ msg: 'Validation.Capability.Evidence.Upload.FailedAction' }]
      }
      errorUploading = err
    })
  }

  if ((errorUploading || errorUpdating) && req.body.action.continue) {
    res.render('supplier/capabilities/index', context)
  } else if (req.body.action.continue) {
    res.redirect(path.join(req.baseUrl, req.url, 'confirmation'))
  } else if (req.body.action.exit) {
    redirectToOnboardingDashboard(req, res)
  } else {
    res.redirect(path.join(req.baseUrl, req.url))
  }
}

async function downloadEvidenceGet (req, res) {
  req.body.errors = req.body.errors || []

  const claimID = req.params.claim_id
  const fileName = req.params.file_name

  const files = await fetchFiles(claimID)
  const selectedFile = _.find(files.items, ['name', fileName])

  downloadFile(claimID, selectedFile.blobId).then((fileResponse) => {
    res.header('Content-Disposition', `attachment; filename="${fileName}"`)
    fileResponse.body.pipe(res)
  }).catch((err) => {
    req.body.errors.push(err)
    solutionCapabilityPageGet(req, res)
  })
}

async function confirmationPageGet (req, res) {
  const context = {
    ...await capabilityPageContext(req)
  }

  context.solution.standardsByGroup = context.solution.capabilities.reduce((obj, cap) => {
    cap.standardsByGroup.associated.forEach((std) => { obj.associated[std.id] = std })
    cap.standardsByGroup.overarching.forEach((std) => { obj.overarching[std.id] = std })
    return obj
  },
  { associated: {}, overarching: {} })

  context.solution.standardsByGroup.associated = _.map(context.solution.standardsByGroup.associated)
  context.solution.standardsByGroup.overarching = _.map(context.solution.standardsByGroup.overarching)

  context.solution.capabilities = context.solution.capabilities.map((cap) => {
    let isLiveDemo = false
    let missingEvidence = true

    // if there is a latest evidence and it is indicating a live demo.
    if (cap.latestEvidence && cap.latestEvidence.evidence === LIVE_DEMO_MESSSAGE_INDICATOR) {
      isLiveDemo = true
      missingEvidence = false
    } else if (cap.latestEvidence && cap.latestFile) {
      missingEvidence = false
    }

    context.editUrls = {
      registration: `/suppliers/solutions/${req.params.solution_id}/register`,
      capabilities: {
        selection: `/suppliers/solutions/${req.params.solution_id}/capabilities`,
        evidence: `/suppliers/capabilities/${req.params.solution_id}`
      }
    }

    return {
      ...cap,
      isLiveDemo,
      missingEvidence
    }
  })

  // page is only editable if the solution is registered, and notyet submitted for assessment
  context.notEditable = context.solution.status !== dataProvider.getRegisteredStatusCode()

  if (context.notEditable) {
    delete context.activeForm.id
  }

  // every solution needs an evidence file w/ a description, or needs to be a live demo.
  if (!context.solution.capabilities.every((cap) => cap.isLiveDemo || !cap.missingEvidence)) {
    res.redirect(path.join(req.baseUrl, req.url.replace('confirmation', '')))
  } else {
    res.render('supplier/capabilities/confirmation', context)
  }
}

async function confirmationPagePost (req, res) {
  const context = {
    ...await capabilityPageContext(req)
  }

  if (req.body.action.save) {
    return confirmationPageGet(req, res)
  } else if (req.body.action.exit) {
    return redirectToOnboardingDashboard()
  } else if (req.body.action.continue) {
    try {
      await submitCapabilityAssessment(req.solution.id)
      return redirectToOnboardingDashboard(req, res)
    } catch (err) {
      context.errors = {
        items: [{ msg: 'Validation.Capability.Evidence.Submission.FailedAction' }]
      }
      res.redirect(path.join(req.baseUrl, req.url.replace('confirmation', '')))
    }
  }
}

async function submitCapabilityAssessment (solutionID) {
  return dataProvider.submitSolutionForCapabilityAssessment(solutionID)
}

async function redirectToOnboardingDashboard (req, res) {
  const route = path.join('/suppliers/solutions', req.params.solution_id)
  return res.redirect(route)
}

async function updateSolutionCapabilityEvidence (solutionID, evidenceDescriptions) {
  return dataProvider.updateSolutionCapabilityEvidence(solutionID, evidenceDescriptions)
}

async function downloadFile (claimID, blobId) {
  return sharePointProvider.downloadCapEvidence(claimID, blobId)
}

async function fetchFiles (claimID) {
  return sharePointProvider.getCapEvidenceFiles(claimID)
}

function findLatestFile (files) {
  return _.maxBy(files, 'timeLastModified')
}

function findLatestEvidence (evidence) {
  return _.maxBy(evidence, 'createdOn')
}

function parseFiles (files) {
  return files.map((file) => {
    return {
      ...file,
      claimID: extractClaimIDFromUploadFieldName(file.fieldname)
    }
  })
}

function extractClaimIDFromUploadFieldName (fieldName) {
  return fieldName.replace('evidence-file[', '').replace(']', '')
}

async function uploadFiles (files) {
  return Promise.all(
    files.map((file) => uploadFile(file.claimID, file.buffer, file.originalname))
  )
}

async function uploadFile (claimID, buffer, fileName) {
  return sharePointProvider.uploadCapEvidence(claimID, buffer, fileName)
}

module.exports = router
