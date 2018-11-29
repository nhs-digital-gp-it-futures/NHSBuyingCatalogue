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

  res.render('supplier/compliance/index', context)
}

async function evidencePageContext (req) {
  const context = {
    ...commonComplianceContext(req),
    ...await dataProvider.capabilityMappings(),
  }

  context.claim = _.find(context.solution.standards, { id: req.params.claim_id })
  context.claim.standard = context.standards[context.claim.standardId]

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

  if(context.files) {
    latestFile = findLatestFile(context.files.items)
  }

  if(latestFile) {
    latestFile.downloadURL = path.join(req.baseUrl, req.path, latestFile.name)

    context.latestFile = latestFile  
  }

  res.render('supplier/compliance/evidence', context)
}

async function solutionComplianceEvidencePagePost (req, res) {
  const context = {
    ...await evidencePageContext(req)
  }

  if (!req.files.length) {
    req.body.errors = 'No file to upload.'
  }
  const fileToUpload = req.files[0]

  try{
    await uploadFile(context.claim.id, fileToUpload.buffer, fileToUpload.originalname)
  }
  catch (err) {
    req.body.errors = [err]
  }

  solutionComplianceEvidencePageGet(req, res)
}

async function downloadEvidenceGet (req, res) {
  const context = {
    ...await evidencePageContext(req)
  }
  req.body.errors = req.body.erros || []

  const claimID = req.params.claim_id
  const fileName = req.params.file_name

  const selectedFile = _.find(context.files.items, ['name', fileName] )

  downloadFile(claimID, selectedFile.url).then((fileResponse) => {
    res.header('Content-Disposition', `attachment; filename="${fileName}"`);
    fileResponse.body.pipe(res)
  }).catch((err) => {
    req.body.errors.push(err)
    solutionComplianceEvidencePageGet(req, res)
  })
}

async function downloadFile (claimID, url) {
  return sharePointProvider.downloadStdEvidence(claimID, url)
}

async function fetchFiles (claimID) {
  console.log('\n\nRequesting Evidence Listing:', claimID, '\n\n')
  return sharePointProvider.getStdEvidenceFiles(claimID)
}

function findLatestFile(files) {
  return _.maxBy(files, (f) => f.timeLastModified)
}

async function uploadFile (claimID, buffer, fileName) {
  return sharePointProvider.uploadStdEvidence(claimID, buffer, fileName)
}

module.exports = router
