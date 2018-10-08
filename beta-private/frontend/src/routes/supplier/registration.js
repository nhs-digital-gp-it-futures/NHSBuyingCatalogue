const router = require('express').Router({ strict: true, mergeParams: true })
const { dataProvider } = require('catalogue-data')

// all routes in this module require CSRF protection
router.use(require('csurf')())

// registering a new solution is the same basic flow as editing an existing one
// except that there is no solution to load
router
  .route('/new/')
  .get(onboardingStatusPage)

router
  .route('/new/register/')
  .get(registrationPageGet)
  .post(registrationPagePost)

// all the remaining routes need to load a specified solution
router.param('solution_id', async (req, res, next, solutionId) => {
  try {
    req.solution = await dataProvider.getSolutionForRegistration(solutionId)
    next()
  } catch (err) {
    next(err)
  }
})

router
  .route('/:solution_id/')
  .get(onboardingStatusPage)

router
  .route('/:solution_id/register/')
  .get(registrationPageGet)
  .post(registrationPagePost)

function commonOnboardingContext (req) {
  return {
    solution: req.solution,
    csrfToken: req.csrfToken()
  }
}

function onboardingStatusPage (req, res) {
  const context = {
    ...commonOnboardingContext(req),
    continueOnboardingUrl: 'register',
    registerNewSolutionUrl: 'register'
  }

  res.render('supplier/registration/index', context)
}

function registrationPageContext (req) {
  return {
    ...commonOnboardingContext(req)
  }
}

function registrationPageGet (req, res) {
  const context = {
    ...registrationPageContext(req)
  }

  res.render('supplier/registration/1-details', context)
}

async function registrationPagePost (req, res) {
  const context = {
    ...registrationPageContext(req)
  }

  res.render('supplier/registration/1-details', context)
}

module.exports = router
