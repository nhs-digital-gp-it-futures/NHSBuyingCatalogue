const _ = require('lodash')
const router = require('express').Router({ strict: true, mergeParams: true })
const { dataProvider } = require('catalogue-data')

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
  .route('/:solution_id/:claim_id/')
  .get(solutionComplianceStandardPageGet)

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
      ...std
    }))
    .value()

  res.render('supplier/compliance/index', context)
}

async function solutionComplianceStandardPageGet (req, res) {
  const context = {
    ...commonComplianceContext(req)
  }

  res.render('supplier/compliance/standard', context)
}

module.exports = router
