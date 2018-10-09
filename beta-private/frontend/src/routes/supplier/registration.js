const _ = require('lodash')
const router = require('express').Router({ strict: true, mergeParams: true })
const { checkSchema, validationResult } = require('express-validator/check')
const { matchedData } = require('express-validator/filter')
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
  .post(registrationPageValidation(), registrationPagePost)

// all the remaining routes need to load a specified solution
router.param('solution_id', async (req, res, next, solutionId) => {
  try {
    req.solution = await dataProvider.solutionForRegistration(solutionId)
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
  .post(registrationPageValidation(), registrationPagePost)

function commonOnboardingContext (req) {
  return {
    solution: req.solution,
    csrfToken: req.csrfToken()
  }
}

function onboardingStatusPage (req, res) {
  const context = {
    ...commonOnboardingContext(req),
    continueOnboardingUrl: 'register#content',
    registerNewSolutionUrl: 'register#content'
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
  const context = _.merge({
    ...registrationPageContext(req)
  }, matchedData(req, {
    locations: 'body',
    includeOptionals: true,
    onlyValidData: false
  }))

  const valres = validationResult(req)
  if (!valres.isEmpty()) {
    context.errors = {
      items: valres.array({ onlyFirstError: true }),
      controls: valres.mapped()
    }
  }

  res.render('supplier/registration/1-details', context)
}

function registrationPageValidation () {
  return checkSchema({
    'solution.name': {
      in: 'body',
      trim: {},
      isEmpty: {
        negated: true,
        errorMessage: 'Solution name is missing'
      },
      isLength: {
        options: { max: 60 },
        errorMessage: 'Solution name exceeds maximum length of 60 characters'
      }
    },

    'solution.description': {
      in: 'body',
      trim: {},
      isEmpty: {
        negated: true,
        errorMessage: 'Solution description is missing'
      },
      isLength: {
        options: { max: 300 },
        errorMessage: 'Solution description exceeds maximum length of 300 characters'
      }
    },

    'solution.version': {
      in: 'body',
      trim: {},
      isLength: {
        options: { max: 10 },
        errorMessage: 'Solution version exceeds maximum length of 10 characters'
      }
    }
  })
}

module.exports = router
