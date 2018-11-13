const _ = require('lodash')
const router = require('express').Router({ strict: true, mergeParams: true })
const { checkSchema, validationResult } = require('express-validator/check')
const { matchedData } = require('express-validator/filter')
const { dataProvider } = require('catalogue-data')

const registrationPageValidation = checkSchema(require('./registration-validation').registration)
const capabilitiesPageValidation = checkSchema(require('./registration-validation').capabilities)

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
  .post(capabilitiesPageValidation, capabilitiesPagePost)

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
    ...commonOnboardingContext(req),
    activeFormId: 'registration-form',
    activeFormTitle: _.join(_.filter([req.solution.name, req.solution.version]), ', ')
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
      controls: _.mapValues(valres.mapped(), res => ({
        ...res,
        action: res.msg + 'Action'
      }))
    }
    context.errors.fieldsets = {
      'NameDescVersion': 'solution.name' in context.errors.controls ||
        'solution.description' in context.errors.controls ||
        'solution.version' in context.errors.controls
    }
  } else {
    // TODO create solution if necessary

    req.solution.name = context.solution.name
    req.solution.description = context.solution.description
    req.solution.version = context.solution.version

    try {
      await dataProvider.updateSolutionForRegistration(req.solution)
    } catch (err) {
      context.errors = {
        items: [{ msg: err.toString() }]
      }
    }
  }

  if (context.errors) {
    res.render('supplier/registration/1-details', context)
  } else {
    let redirectUrl = '.'
    if (req.body.action) {
      if (req.body.action.continue) redirectUrl = '../capabilities/'
      if (req.body.action.exit) redirectUrl = '../'
    }
    res.redirect(redirectUrl)
  }
}

async function capabilitiesPageContext (req) {
  const context = {
    ...commonOnboardingContext(req),
    ...await dataProvider.capabilityMappings()
  }

  context.capabilities = _(context.capabilities)
    .values()
    .orderBy('name')
    .value()

  return context
}

async function capabilitiesPageGet (req, res) {
  const context = await capabilitiesPageContext(req)

  context.capabilities.forEach(cap => {
    cap.selected = _.some(req.solution.capabilities, { capabilityId: cap.id })
  })

  res.render('supplier/registration/2-capabilities', context)
}

async function capabilitiesPagePost (req, res) {
  const context = await capabilitiesPageContext(req)

  context.capabilities.forEach(cap => {
    cap.selected = _.has(req.body.capabilities, cap.id)
  })

  const valres = validationResult(req)
  if (!valres.isEmpty() && req.body.action.continue) {
    context.errors = {
      items: valres.array({ onlyFirstError: true }),
      controls: valres.mapped()
    }
  } else {
    req.solution.capabilities = _(context.capabilities)
      .filter('selected')
      .map(cap => ({
        id: require('node-uuid-generator').generate(),
        capabilityId: cap.id,
        status: '0',
        solutionId: req.solution.id
      }))
      .value()

    try {
      await dataProvider.updateSolutionForRegistration(req.solution)
    } catch (err) {
      context.errors = {
        items: [{ msg: err.toString() }]
      }
    }
  }

  if (context.errors) {
    res.render('supplier/registration/2-capabilities', context)
  } else {
    res.redirect('../')
  }
}

module.exports = router
