const _ = require('lodash')
const router = require('express').Router({ strict: true, mergeParams: true })
const { checkSchema, validationResult } = require('express-validator/check')
const { matchedData } = require('express-validator/filter')
const { dataProvider } = require('catalogue-data')

const registrationPageValidation = checkSchema(require('./registration-validation'))

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
  .post(registrationPageValidation, registrationPagePost)

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
  .post(registrationPageValidation, registrationPagePost)

router
  .route('/:solution_id/capabilities/')
  .get(capabilitiesPageGet)
  .post(capabilitiesPagePost)

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

    res.render('supplier/registration/1-details', context)
  } else {
    // TODO create/update solution

    res.redirect(req.body.action && req.body.action.continue ? '../capabilities/' : '../')
  }
}

function capabilitiesPageContext (req) {
  return {
    ...commonOnboardingContext(req)
  }
}

async function capabilitiesPageGet (req, res) {
  const context = {
    ...capabilitiesPageContext(req),
    ...await dataProvider.capabilityMappings()
  }

  context.capabilities = _(context.capabilities)
    .values()
    .orderBy('name')
    .forEach(cap => {
      cap.selected = _.some(req.solution.capabilities, { capabilityId: cap.id })
    })

  res.render('supplier/registration/2-capabilities', context)
}

function capabilitiesPagePost (req, res) {
  const context = {
    ...capabilitiesPageContext(req)
  }

  res.redirect('../')
}

module.exports = router
