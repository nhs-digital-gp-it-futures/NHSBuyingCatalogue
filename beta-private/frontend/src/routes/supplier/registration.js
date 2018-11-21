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
  .post(registrationPreValidation, registrationPageValidation, registrationPagePost)

router
  .route('/:solution_id/capabilities/')
  .get(capabilitiesPageGet)
  .post(capabilitiesPageValidation, capabilitiesPagePost)

function commonOnboardingContext (req) {
  return {
    solution: req.solution,
    csrfToken: req.csrfToken(),
    activeFormTitle: req.solution && _([req.solution.name, req.solution.version]).filter().join(', ')
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
  const context = {
    ...commonOnboardingContext(req),
    activeFormId: 'registration-form'
  }

  if (!context.solution) {
    context.solution = { id: 'new' }
  }

  return context
}

// Handlebars templates can't do string synthesis and that is needed to lookup
// the name of a field in the errors.controls array. Instead, pass a dictionary for the
// contacts that yields the control names.
function addContactFieldsToContext (context) {
  context.contactFields = _.map(context.solution.contacts,
    (c, i) => _(['contactType', 'firstName', 'lastName', 'emailAddress', 'phoneNumber'])
      .map(f => [f, `solution.contacts[${i}].${f}`])
      .fromPairs()
      .value()
  )
}

function registrationPageGet (req, res) {
  const context = {
    ...registrationPageContext(req)
  }

  addContactFieldsToContext(context)

  res.render('supplier/registration/1-details', context)
}

// before attempting to validate the body for registration,
// remove any contacts that are entirely empty
function registrationPreValidation (req, res, next) {
  if (req.body && req.body.solution && req.body.solution.contacts) {
    req.body.solution.contacts = _.filter(
      req.body.solution.contacts,
      c => `${c.contactType}${c.firstName}${c.lastName}${c.emailAddress}${c.phoneNumber}`.trim().length
    )
  }

  next()
}

async function registrationPagePost (req, res) {
  const sanitisedInput = matchedData(req, {
    locations: 'body',
    includeOptionals: true,
    onlyValidData: false
  })
  const context = _.merge({
    ...registrationPageContext(req)
  }, sanitisedInput)
  context.solution.contacts = sanitisedInput.solution.contacts

  addContactFieldsToContext(context)

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
        'solution.version' in context.errors.controls,
      // FIXME the following opaque monstrosity yields an object keyed by the index
      // of any contact that has validation errors (_.toPath being ideal here)
      'Contacts': _(context.errors.controls)
        .keys().map(_.toPath).filter(p => p[0] === 'solution' && p[1] === 'contacts')
        .map(p => p[2]).uniq().map(k => [k, true]).fromPairs().value()
    }
  } else {
    try {
      if (context.solution.id === 'new') {
        req.solution = await dataProvider.createSolutionForRegistration({
          name: context.solution.name,
          description: context.solution.description,
          version: context.solution.version
        }, req.user)
      } else {
        req.solution.name = context.solution.name
        req.solution.description = context.solution.description
        req.solution.version = context.solution.version
      }

      req.solution.contacts = context.solution.contacts

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
    // redirect based on action chosen and whether a new solution was just created
    let redirectUrl = context.solution.id === 'new'
      ? `../../${req.solution.id}/register/`
      : './'

    if (req.body.action) {
      if (req.body.action.continue) redirectUrl += '../capabilities/'
      if (req.body.action.exit) redirectUrl += '../'
    }

    res.redirect(redirectUrl)
  }
}

async function capabilitiesPageContext (req) {
  const context = {
    ...commonOnboardingContext(req),
    ...await dataProvider.capabilityMappings(),
    activeFormId: 'capability-selector-form'
  }

  context.capabilities = _(context.capabilities)
    .values()
    .orderBy(a => a.name.toLowerCase())
    .map(c => ({
      ...c,
      standardIds: _(c.standards)
        .reject(c => context.standards[c.id].isOverarching)
        .map('id')
        .value()
    }))
    .value()

  context.capabilitiesByGroup = _.zipObject(
    ['core', 'noncore'],
    _.partition(context.capabilities, c => _.startsWith(c.id, 'CAP-C-'))
  )

  context.standardsByGroup = _.zipObject(
    ['overarching', 'associated'],
    _.partition(context.standards, s => _.startsWith(s.id, 'STD-O-'))
  )

  return context
}

async function capabilitiesPageGet (req, res) {
  const context = await capabilitiesPageContext(req)

  context.capabilities.forEach(cap => {
    cap.selected = _.get(_.find(req.solution.capabilities, { capabilityId: cap.id }), 'id')
  })

  res.render('supplier/registration/2-capabilities', context)
}

async function capabilitiesPagePost (req, res) {
  const context = await capabilitiesPageContext(req)

  // the "selected" property holds the current ID for each claimed capability,
  // or a newly generated ID for an added capability
  context.capabilities.forEach(cap => {
    cap.selected = _.has(req.body.capabilities, cap.id) &&
      (req.body.capabilities[cap.id] || require('node-uuid-generator').generate())
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
        id: cap.selected,
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
    // redirect based on action chosen
    const redirectUrl = (req.body.action && req.body.action.exit)
      ? '../'
      : './'

    res.redirect(redirectUrl)
  }
}

module.exports = router
