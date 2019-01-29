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
  .post(capabilitiesPageValidation, solutionCapabilityPagePost)

router
  .route('/:solution_id/:claim_id/:file_name')
  .get(downloadEvidenceGet)

router
  .route('/:solution_id/confirmation')
  .get(confirmationPageGet)
  .post(confirmationPagePost)

router
  .route('/:solution_id/confirmation')

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
    ...await dataProvider.capabilityMappings(),
    errors: {
      items: []
    }
  }

  context.activeForm.id = 'capability-assessment-form'

  const enumeration = await sharePointProvider.enumerateCapFolderFiles(req.solution.id).catch((err) => {
    context.errors.items.push(
      { msg: 'Validation.Capability.Evidence.Retrieval.FailedAction', err: err }
    )
  })
  console.log('ENUMERATION\n\n', enumeration, '\n\n', Object.keys(enumeration), '\n\n')

  context.solution.capabilities = context.solution.capabilities.map((cap) => {
    const files = enumeration[cap.claimID]

    console.log('CLAIM ID\n', cap.claimID, 'CAP ID', cap.id)
    console.log('FILES\n', files)

    let latestFile

    if (files) {
      latestFile = findLatestFile(files)
    }

    if (latestFile) {
      latestFile.downloadURL = path.join(req.baseUrl, req.path.replace('/confirmation', ''), cap.claimID, latestFile.name)
    }

    const latestEvidence = findLatestEvidence(cap.evidence)

    return {
      ...cap,
      latestFile,
      latestEvidence,
      isUploadingEvidence: latestEvidence ? !latestEvidence.hasRequestedLiveDemo : true
    }
  })

  context.solution.capabilities = _.sortBy(context.solution.capabilities, 'name')

  return context
}

async function solutionCapabilityPageGet (req, res) {
  const context = {
    ...await capabilityPageContext(req),
    breadcrumbs: [
      { label: 'Onboarding.Title', url: `../../solutions/${req.solution.id}` },
      { label: 'CapAssPages.Breadcrumb' }
    ]
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
    errors: {
      items: []
    },
    breadcrumbs: [
      { label: 'Onboarding.Title', url: `../../solutions/${req.solution.id}` },
      { label: 'CapAssPages.Breadcrumb' }
    ]
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

  // parse anyfiles to be uploaded to SharePoint
  const files = parseFiles(req.files)

  // With the lack of anything other than a single string for adding additional evidence
  // indicating that a live demo has been requested instead of using video evidence
  // must be indicated using the same string that evidence messages are entered.
  //
  // For that reason, if the supplier wants to do a live demo, then a string is
  // shoved into the evidence message field.
  //
  // The API this program interfaces with requires us to generate our own ID's
  // This is couter intuitive and has a tonne of flaws, the hope is eventually a redesign
  // of the API will result in the endpoint only requiring us to Post the Evidence message
  // and not make up an ID, Date, CreatedById, previousId etc.
  const uploadingVideoEvidence = req.body['uploading-video-evidence']
  let systemError

  const evidenceDescriptions = await Promise.all(_.map(uploadingVideoEvidence, async (isUploading, claimID) => {
    let fileToUpload = files[claimID]
    let blobId = null

    if (fileToUpload) {
      blobId = await uploadFile(claimID, fileToUpload.buffer, fileToUpload.originalname).catch((err) => {
        context.errors.items.push({ msg: 'Validation.Capability.Evidence.Upload.FailedAction' })
        systemError = err
      })
    }

    const currentEvidence = _.filter(req.solution.evidence, { claimId: claimID })
    const latestEvidence = findLatestEvidence(currentEvidence)

    return {
      id: require('node-uuid-generator').generate(),
      previousId: latestEvidence ? latestEvidence.id : null,
      claimId: claimID,
      createdById: req.user.contact.id,
      createdOn: new Date().toISOString(),
      evidence: isUploading === 'yes' ? req.body['evidence-description'][claimID] : LIVE_DEMO_MESSSAGE_INDICATOR,
      hasRequestedLiveDemo: isUploading !== 'yes',
      blobId: blobId
    }
  }))

  // Update solution evidence, communicate the error if there isn't any already.
  await updateSolutionCapabilityEvidence(req.solution.id, evidenceDescriptions).catch((err) => {
    context.errors.items.push({ msg: 'Validation.Capability.Evidence.Update.FailedAction' })
    systemError = err
  })

  // only show validation errors if the user elected to continue, not just save
  if (systemError || (context.errors.items.length && req.body.action.continue)) {
    // regenerate the full context (preserving errors)
    req.solution = await dataProvider.solutionForAssessment(req.params.solution_id)
    const fullContext = await capabilityPageContext(req)
    fullContext.errors = context.errors
    res.render('supplier/capabilities/index', fullContext)
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

    if (cap.latestEvidence && cap.latestEvidence.hasRequestedLiveDemo) {
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
    return redirectToOnboardingDashboard(req, res)
  } else if (req.body.action.continue) {
    try {
      await submitCapabilityAssessment(req.solution.id)
      return redirectToOnboardingDashboard(req, res, 'capabilityAssessmentSubmitted')
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

async function redirectToOnboardingDashboard (req, res, param) {
  const route = `${path.join('/suppliers/solutions', req.params.solution_id)}${param ? `?${param}` : ''}`
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
  // follow the trail of previousIds until there are no more
  let previousId = null
  let currentEvidence
  let latestEvidence

  while ((currentEvidence = _.find(evidence, { previousId }))) {
    latestEvidence = currentEvidence
    previousId = currentEvidence.id
  }

  return latestEvidence
}

function parseFiles (files) {
  let fileMap = {}
  files.map((file) => {
    return {
      ...file,
      claimID: extractClaimIDFromUploadFieldName(file.fieldname)
    }
  }).forEach((file) => {
    fileMap[file.claimID] = file
  })
  return fileMap
}

function extractClaimIDFromUploadFieldName (fieldName) {
  return fieldName.replace('evidence-file[', '').replace(']', '')
}

async function uploadFile (claimID, buffer, fileName) {
  let res = await sharePointProvider.uploadCapEvidence(claimID, buffer, fileName)
  return res
}

module.exports = router
