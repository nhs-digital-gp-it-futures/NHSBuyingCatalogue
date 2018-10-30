const _ = require('lodash')
const api = require('catalogue-api')
const app = require('express').Router()
const csrfProtection = require('csurf')()
const uuidGenerator = require('node-uuid-generator')
const { check, validationResult } = require('express-validator/check')
const { matchedData } = require('express-validator/filter')
const { formatting } = require('catalogue-utils')

const multipartBodyParser = require('express-fileupload')

const {
  enrichContextForProductPage,
  enrichContextForProductPagePreview
} = require('./supplier/product-page')

const primaryContactTypes = [
  'Lead Contact', 'Clinical Safety Officer'
]

const primaryContactHelp = {
  'Lead Contact': 'The person responsible for ensuring the information supplied during onboarding is a true reflection of the solution.',
  'Clinical Safety Officer': 'The person in a Supplier’s organisation responsible for ensuring the safety of a Health IT System in that organisation through the application of clinical risk management.'
}

app.get('/', (req, res) => {
  const context = {
    breadcrumbs: [
      { label: 'My Dashboard' }
    ]
  }

  res.render('supplier/dashboard', context)
})

app.get('/solutions', async (req, res) => {
  delete req.session.solutionEx
  const context = {
    breadcrumbs: [
      { label: 'My Dashboard', url: '/suppliers' },
      { label: 'My Solutions' }
    ],
    created: 'created' in req.query,
    errors: {},
    addSolutionUrl: `${req.baseUrl}/solutions/add`,
  }
  let solutions = []

  try {
    // computing the correct status for each solution requires the full SolutionEx
    // structure, so load all of them
    solutions = await Promise.all(
      (await api.get_solutions_for_user(req.user)).map(
        soln => api.get_solution_by_id(soln.id)
      )
    )

  } catch (err) {
    if (err.status === 404) {
      solutions = []
    } else {
      context.errors.general = err
    }
  }

  function solutionDashboardStatus (solutionEx) {
    // by default the status will be the display representation of the base solution
    // status
    const solnStatus = solutionEx.solution.status
    let computedStatus = api.solutionStatuses[solnStatus] || 'Unknown'

    const hasFailedCapAss = _.some(
      solutionEx.claimedCapability,
      ['status', api.CAPABILITY_STATUS.REJECTED]
    )
    const hasRemediationCapAss = _.some(
      solutionEx.claimedCapability,
      ['status', api.CAPABILITY_STATUS.REMEDIATION]
    )

    const decodedStandards = _.map(
      solutionEx.claimedStandard,
      std => ({
        ...std,
        evidence: std.evidence && JSON.parse(std.evidence)
      })
    )

    const hasSubmittedStd = _.some(decodedStandards, 'evidence.submissions.length')

    const productPageStatus = solutionEx.solution.productPage.status
    switch (solnStatus) {
      case api.SOLUTION_STATUS.DRAFT:
        if (hasFailedCapAss) computedStatus = 'Assessment Failed'
        break
      case api.SOLUTION_STATUS.REGISTERED:
        break
      case api.SOLUTION_STATUS.CAPABILITIES_ASSESSMENT:
        computedStatus = 'Submitted'
        if (hasRemediationCapAss) computedStatus = 'Assessment Remediation'
        if (hasFailedCapAss) computedStatus = 'Assessment Failed'
        break
      case api.SOLUTION_STATUS.STANDARDS_COMPLIANCE:
        computedStatus = 'Assessed Pending Compliance'
        if (hasSubmittedStd) computedStatus = 'Submitted for Compliance'
        break
      case api.SOLUTION_STATUS.SOLUTION_PAGE:
        switch (productPageStatus) {
          case 'SUBMITTED':
            computedStatus = 'Page Submitted For Moderation'
            break
          case 'REMEDIATION':
            computedStatus = 'Page in Remediation'
            break
          default:
            computedStatus = 'Approved'
        }
        break
      case api.SOLUTION_STATUS.APPROVED:
        computedStatus = 'Page Approved'
        if (productPageStatus === 'PUBLISH') computedStatus = 'Approved, Accepting Sales'
        break
    }

    return computedStatus
  }

  function solutionDashboardContext (solutionEx) {
    const solution = solutionEx.solution

    return {
      ...solution,
      computedStatus: solutionDashboardStatus(solutionEx),
      continueUrl: solution.status === api.SOLUTION_STATUS.APPROVED
                   ? `${req.baseUrl}/solutions/${solution.id}/product-page/preview`
                   : `${req.baseUrl}/solutions/${solution.id}`
    }
  }

  function solutionStageMapper(solutionCtx) {
    const stageNumber = solutionCtx.status;
    const stageNumberString = api.stageIndicators[stageNumber];
    const currStage = api.solutionStages[stageNumber]
    return {
      ...solutionCtx,
      stageNumber : stageNumberString,
      currentStage : currStage
    }
  }

  function notificationMapper(solutionCtx) {
    const notificationCount = _.random(5)
    // await api.request_solution_notifications;
    return {
      ...solutionCtx,
      notificationCount : notificationCount
    }
  }

  function contractsMapper(solutionCtx) {
    const contractCount = 0;
    // await api.request_solution_contracts;
    return {
      ...solutionCtx,
      contractCount : contractCount
    }
  }

  const liveGroupName = 'Live Solutions';
  const onboardingGroupName = 'Onboarding in Progress'
  function solutionDashboardGroup (solution) {
    return solution.status === api.SOLUTION_STATUS.APPROVED
           ? liveGroupName
           : onboardingGroupName
  }


  context.groupedSolutions = _(solutions)
    .map(solutionDashboardContext)
    .map(solutionStageMapper)
    .map(notificationMapper)
    .map(contractsMapper)
    .groupBy(solutionDashboardGroup)
    .value()

  context.liveSolutions = context.groupedSolutions[liveGroupName]

  context.onboardingSolutions = context.groupedSolutions[onboardingGroupName]

  res.render('supplier/solutions', context)
})

app.get('/solutions/add', csrfProtection, async (req, res) => {
  res.render('supplier/add-solution', {
    backlink: {
      title: 'Onboarding Solution',
      href: `/suppliers/solutions/new`
    },
    primaryContactTypes,
    primaryContactHelp,
    csrfToken: req.csrfToken(),
    pageHasForm: true
  })
})

function addError (errors, fieldName, message) {
  return _.update(errors, fieldName, errs => (errs || []).concat(message))
}

function validateSolution (req) {
  const errors = {}

  let {
    name,
    version,
    description,
    contacts = {},
    secondaryContacts = {}
  } = req.body

  // validate each field
  name = (name || '').trim()
  if (!name.length) addError(errors, 'name', 'Solution name is required')
  if (name.length > 60) addError(errors, 'name', `Solution name is limited to 60 characters, ${name.length} entered`)

  version = (version || '').trim()
  if (version.length > 10) addError(errors, 'version', `Solution version is limited to 10 characters, ${version.length} entered`)

  description = (description || '').trim()
  if (!description.length) addError(errors, 'description', 'Solution description is required')
  if (description.length > 1000) addError(errors, 'description', `Solution description is limited to 1000 characters, ${description.length} entered`)

  // validate the primary contacts, if any part is provided for one, all parts
  // must be provided
  for (const contactType of primaryContactTypes) {
    let {firstName, lastName, emailAddress, phoneNumber} = contacts[contactType] || {}

    firstName = (firstName || '').trim()
    lastName = (lastName || '').trim()
    emailAddress = (emailAddress || '').trim()
    phoneNumber = (phoneNumber || '').trim()

    if (firstName || lastName || emailAddress || phoneNumber || contactType === 'Lead Contact') {
      if (!firstName.length) addError(errors, ['contacts', contactType, 'firstName'], 'First name is required')
      if (firstName.length > 60) addError(errors, ['contacts', contactType, 'firstName'], `First name is limited to 60 characters, ${firstName.length} entered`)

      if (!lastName.length) addError(errors, ['contacts', contactType, 'lastName'], 'Last name is required')
      if (lastName.length > 60) addError(errors, ['contacts', contactType, 'lastName'], `Last name is limited to 60 characters, ${lastName.length} entered`)

      if (!emailAddress.length) addError(errors, ['contacts', contactType, 'emailAddress'], 'Email is required')
      if (emailAddress.length > 120) addError(errors, ['contacts', contactType, 'emailAddress'], `Email is limited to 60 characters, ${emailAddress.length} entered`)

      if (!phoneNumber.length) addError(errors, ['contacts', contactType, 'phoneNumber'], 'Phone number is required')
      if (phoneNumber.length > 30) addError(errors, ['contacts', contactType, 'phoneNumber'], `Phone number is limited to 30 characters, ${phoneNumber.length} entered`)
    } else {
      delete contacts[contactType]
    }
  }

  // convert incoming secondary contacts from an object of arrays into an array of object
  const keys = ['contactType', 'firstName', 'lastName', 'emailAddress', 'phoneNumber']
  secondaryContacts = _.mapValues(secondaryContacts, _.castArray)

  secondaryContacts = secondaryContacts.contactType && secondaryContacts.contactType.length
    ? _.range(secondaryContacts.contactType.length).map(index => _.zipObject(
        keys, _.map(keys, k => secondaryContacts[k][index])
      ))
    : []

  // filter out completely empty secondary contacts, validate the remainder
  secondaryContacts = _.filter(secondaryContacts, contact => {
    let {contactType, firstName, lastName, emailAddress, phoneNumber} = contact

    contactType = (contactType || '').trim()
    firstName = (firstName || '').trim()
    lastName = (lastName || '').trim()
    emailAddress = (emailAddress || '').trim()
    phoneNumber = (phoneNumber || '').trim()

    return contactType || firstName || lastName || emailAddress || phoneNumber
  })

  _.each(secondaryContacts, (contact, index) => {
    let {contactType, firstName, lastName, emailAddress, phoneNumber} = contact

    contactType = (contactType || '').trim()
    firstName = (firstName || '').trim()
    lastName = (lastName || '').trim()
    emailAddress = (emailAddress || '').trim()
    phoneNumber = (phoneNumber || '').trim()

    if (contactType || firstName || lastName || emailAddress || phoneNumber) {
      if (!contactType.length) addError(errors, ['secondaryContacts', index, 'contactType'], 'Contact type is required')
      if (contactType.length > 60) addError(errors, ['secondaryContacts', index, 'contactType'], `Contact type is limited to 60 characters, ${contactType.length} entered`)

      if (!firstName.length) addError(errors, ['secondaryContacts', index, 'firstName'], 'First name is required')
      if (firstName.length > 60) addError(errors, ['secondaryContacts', index, 'firstName'], `First name is limited to 60 characters, ${firstName.length} entered`)

      if (!lastName.length) addError(errors, ['secondaryContacts', index, 'lastName'], 'Last name is required')
      if (lastName.length > 60) addError(errors, ['secondaryContacts', index, 'lastName'], `Last name is limited to 60 characters, ${lastName.length} entered`)

      if (!emailAddress.length) addError(errors, ['secondaryContacts', index, 'emailAddress'], 'Email is required')
      if (emailAddress.length > 120) addError(errors, ['secondaryContacts', index, 'emailAddress'], `Email is limited to 60 characters, ${emailAddress.length} entered`)

      if (!phoneNumber.length) addError(errors, ['secondaryContacts', index, 'phoneNumber'], 'Phone number is required')
      if (phoneNumber.length > 30) addError(errors, ['secondaryContacts', index, 'phoneNumber'], `Phone number is limited to 30 characters, ${phoneNumber.length} entered`)
    }
  })

  let solution = {
    name,
    version,
    description
  }

  return {errors, solution, contacts, secondaryContacts}
}

app.post('/solutions/add', csrfProtection, (req, res) => {
  const {
    errors, solution, contacts, secondaryContacts
  } = validateSolution(req)

  const saveSolution = Object.keys(errors).length
    ? () => Promise.reject(errors)
    : () => {
      return api.create_solution_for_user(solution, req.user)
        .then(newSolution => {
          const solutionEx = {
            solution: newSolution,
            claimedCapability: [],
            claimedStandard: [],
            technicalContact: _.concat(
              _.zipWith(
                _.keys(contacts),
                _.values(contacts),
                (contactType, contact) => {
                  contact.contactType = contactType
                  return contact
                }
              ),
              secondaryContacts
            )
          }
          return api.update_solution(solutionEx)
            .then(() => solutionEx)
        })
        .then(solutionEx => {
          let redirect = `${req.baseUrl}/solutions?created=${solutionEx.solution.id}`
                    
          if(req.body.action === 'continue') {
            redirect = `${req.baseUrl}/solutions/${solutionEx.solution.id}/capabilities`
          }

          if(req.body.action === 'save') {
            redirect = `${req.baseUrl}/solutions/${solutionEx.solution.id}/edit`
          }

          res.redirect(redirect)
        })
        .catch(err => {
          addError(errors, 'general', err)
          return Promise.reject(errors)
        })
    }

  const rerender = () => {
    const context = {
      ...solution,
      backlink: {
        title: 'Onboarding Solution',
        href: `/suppliers/solutions/new`
      },
      primaryContactTypes,
      primaryContactHelp,
      primaryContacts: contacts,
      secondaryContacts,
      errors,
      csrfToken: req.csrfToken(),
      pageHasForm: true
    }

    res.render('supplier/add-solution', context)
  }

  saveSolution().catch(rerender)
})

app.get('/solutions/:solution_id', async (req, res) => {
  delete req.session.solutionEx

  const defaults = {
    continueUrl: false,
    complete: false,
    stageClass: false
  }

  const context = {
    breadcrumbs: [
      { label: 'My Dashboard', url: '/suppliers' },
      { label: 'My Solutions', url: '/suppliers/solutions' },
      { label: 'Onboarding Solution' }
    ],
    errors: {},
    stages: {
      register: {...defaults},
      assessment: {...defaults},
      compliance: {...defaults},
      solution_page: {...defaults}
    },
    submitted: 'submitted' in req.query
  }

  try {
    const solutionEx = await api.get_solution_by_id(req.params.solution_id)
    const solution = solutionEx.solution

    context.solution = solution

    const status = solution.status
    const solnUrl = `${req.baseUrl}/solutions/${solution.id}`

    // all draft solutions can be submitted for registration
    if (status === api.SOLUTION_STATUS.DRAFT) {
      context.stages.register.continueUrl = `${solnUrl}/edit`
    }

    // all registered solutions, and those already in assessment can access assessment
    if (status === api.SOLUTION_STATUS.REGISTERED ||
        status === api.SOLUTION_STATUS.CAPABILITIES_ASSESSMENT) {
      context.stages.register.complete = true
      context.stages.assessment.active = status === api.SOLUTION_STATUS.CAPABILITIES_ASSESSMENT
      context.stages.assessment.continueUrl = `${solnUrl}/assessment`
      context.stages.compliance.viewUrl = `${solnUrl}/compliance`
      context.stages.compliance.stageClass = 'active'
      context.stages.solution_page.viewUrl = `${solnUrl}/product-page`
      context.stages.solution_page.stageClass = 'active'
    }

    // solutions in standards compliance can access compliance
    if (status === api.SOLUTION_STATUS.STANDARDS_COMPLIANCE) {
      context.stages.register.complete = true
      context.stages.assessment.complete = true
      context.stages.compliance.continueUrl = `${solnUrl}/compliance`
      context.stages.solution_page.viewUrl = `${solnUrl}/product-page`
      context.stages.solution_page.stageClass = 'active'
    }

    // solutions in solution page status can edit solution page
    if (status === api.SOLUTION_STATUS.SOLUTION_PAGE) {
      context.stages.register.complete = true
      context.stages.assessment.complete = true
      context.stages.compliance.complete = true
      context.stages.solution_page.continueUrl = `${solnUrl}/product-page`
    }

    // for completeness, handle approved solutions
    if (status === api.SOLUTION_STATUS.APPROVED) {
      context.stages.register.complete = true
      context.stages.assessment.complete = true
      context.stages.compliance.complete = true
      context.stages.solution_page.complete = true
    }
  } catch (err) {
    context.errors.general = err
    context.stages.register.getStartedUrl = `${req.baseUrl}/solutions/add`
  }

  res.render('supplier/onboarding-status', context)
})

app.get('/solutions/:solution_id/edit', csrfProtection, async (req, res) => {
  try {
    const solutionEx = await api.get_solution_by_id(req.params.solution_id)

    // only draft solutions can be edited
    if (solutionEx.solution.status !== api.SOLUTION_STATUS.DRAFT) {
      return res.redirect('/suppliers/solutions')
    }

    // partition contacts by primary/secondary
    const [primaryContacts, secondaryContacts] = _.partition(
      solutionEx.technicalContact,
      contact => _.includes(primaryContactTypes, contact.contactType)
    )

    res.render('supplier/add-solution', {
      ...solutionEx.solution,
      backlink: {
        title: 'Onboarding Solution',
        href: `/suppliers/solutions/${req.params.solution_id}`
      },
      primaryContactTypes,
      primaryContactHelp,
      primaryContacts: _.keyBy(primaryContacts, 'contactType'),
      secondaryContacts,
      csrfToken: req.csrfToken(),
      pageHasForm: true
    })
  } catch (err) {
    throw err
  }
})

app.post('/solutions/:solution_id/edit', csrfProtection, (req, res) => {
  const {
    errors, solution, contacts, secondaryContacts
  } = validateSolution(req)

  const saveSolution = Object.keys(errors).length
    ? () => Promise.reject(errors)
    : () => {
      return api.get_solution_by_id(req.params.solution_id)
        .then(solutionToUpdate => {
          solutionToUpdate.solution = {
            ...solutionToUpdate.solution,
            ...solution
          }

          // TODO not efficient, re-creates all contacts every save
          solutionToUpdate.technicalContact = _.concat(
            _.zipWith(
              _.keys(contacts),
              _.values(contacts),
              (contactType, contact) => {
                contact.contactType = contactType
                return contact
              }
            ),
            secondaryContacts
          )

          return api.update_solution(solutionToUpdate)
            .then(() => solutionToUpdate)
        })
        .then(({solution}) => {
          let redirect = `${req.baseUrl}/solutions?created=${solution.id}`

          if(req.body.action === 'continue') {
            redirect = `${req.baseUrl}/solutions/${solution.id}/capabilities`
          }

          if(req.body.action === 'save') {
            redirect = `${req.baseUrl}/solutions/${solution.id}/edit`
          }

          res.redirect(redirect)
        })
        .catch(err => {
          addError(errors, 'general', err)
          return Promise.reject(errors)
        })
    }

  const rerender = () => {
    const context = {
      ...solution,
      backlink: {
        title: 'Onboarding Solution',
        href: `/suppliers/solutions/${req.params.solution_id}`
      },
      primaryContactTypes,
      primaryContactHelp,
      primaryContacts: contacts,
      secondaryContacts,
      errors,
      csrfToken: req.csrfToken(),
      pageHasForm: true
    }

    res.render('supplier/add-solution', context)
  }

  saveSolution().catch(rerender)
})

app.get('/solutions/:solution_id/capabilities', csrfProtection, async (req, res) => {
  const [solutionEx, { capabilities, groupedStandards }] = await Promise.all([
    api.get_solution_by_id(req.params.solution_id),
    api.get_all_capabilities()
  ])

  // only draft solutions can be edited
  if (solutionEx.solution.status !== api.SOLUTION_STATUS.DRAFT) {
    return res.redirect('/suppliers/solutions')
  }

  const isCapSpecificStd = std => !['CSS1', 'CSS2', 'CSS3'].includes(std.id)

  const enrichedCapabilities = capabilities.map(cap => {
    const claimedCapability = _.find(solutionEx.claimedCapability, ['capabilityId', cap.id])
    cap.selected = !!claimedCapability
    cap.standardIds = _.map(_.flatMap(cap.standards), 'id')

    // pick out the capability-specific standard from the mandatory standards
    cap.standards.capability = _.find(cap.standards.mandatory, isCapSpecificStd)
    cap.standards.mandatory = _.reject(cap.standards.mandatory, isCapSpecificStd)

    return cap
  })

  const isCore = cap => _.includes(_.words(cap.types), 'core')
  const groupedCapabilities = {
    'Core Capabilities': _.filter(enrichedCapabilities, isCore),
    'Non-core Capabilities': _.reject(enrichedCapabilities, isCore)
  }

  res.render('supplier/solution-capabilities', {
    ...solutionEx.solution,
    backlink: {
      title: 'Solution Details',
      subtitle: '(1 of 4)',
      href: `/suppliers/solutions/${req.params.solution_id}/edit`
    },
    capabilities: enrichedCapabilities,
    groupedCapabilities,
    standards: groupedStandards,
    csrfToken: req.csrfToken(),
    pageHasForm:true,
    solution: {name: solutionEx.solution.name}
  })
})

app.post('/solutions/:solution_id/capabilities', csrfProtection, async (req, res) => {
  const [solutionEx, { capabilities, groupedStandards }] = await Promise.all([
    api.get_solution_by_id(req.params.solution_id),
    api.get_all_capabilities()
  ])

  // only draft solutions can be edited
  if (solutionEx.solution.status !== api.SOLUTION_STATUS.DRAFT) {
    return res.redirect('/suppliers/solutions')
  }

  solutionEx.claimedCapability = _.castArray(req.body.capabilities || []).map(id => ({
    id: uuidGenerator.generate(),
    capabilityId: id
  }))

  // claim any optional standards associated with the claimed capabilities
  // (now only used to track which capabilities the standards were selected against)
  solutionEx.claimedCapabilityStandard = []
  solutionEx.claimedCapability.forEach(cap => {
    const optionalStandardId = _.get(req.body.optionalStandards, cap.capabilityId)
    if (optionalStandardId) {
      solutionEx.claimedCapabilityStandard.push({
        claimedCapabilityId: cap.id,
        standardId: optionalStandardId
      })
    }
  })

  // claim all the non-optional standards associated with all the claimed capabilities
  const hasMobile = _.some(solutionEx.claimedStandard, ['standardId', 'CSS3'])

  solutionEx.claimedStandard = _.uniqBy(
    _.map(
      _.flatten(
        _.map(
          solutionEx.claimedCapability,
          ({capabilityId}) => _.flatMap(
            _.pick(
              _.get(_.find(capabilities, ['id', capabilityId]), 'standards', {}),
              ['interop', 'mandatory', 'overarching']
            )
          )
        )
      ),
      std => ({standardId: std.id})
    ),
    'standardId'
  )

  // optional standards are now also claimed along with the other standards
  if (hasMobile) {
    solutionEx.claimedStandard.push({'standardId': 'CSS3'})
  }

  try {
    let redirectUrl = `${req.baseUrl}/solutions`

    if (req.body.action === 'continue') {
      redirectUrl = `${req.baseUrl}/solutions/${solutionEx.solution.id}/mobile`
    }
    if(req.body.action === 'save') {
      redirectUrl = `${req.baseUrl}/solutions/${solutionEx.solution.id}/capabilities`
    }

    await api.update_solution(solutionEx)

    res.redirect(redirectUrl)
  } catch (err) {
    res.render('supplier/solution-capabilities', {
      ...solutionEx.solution,
      backlink: {
        title: 'Solution Details',
        subtitle: '(1 of 4)',
        href: `/suppliers/solutions/${req.params.solution_id}/edit`
      },
      capabilities: capabilities.map(cap => {
        cap.selected = _.some(solutionEx.claimedCapability, ['capabilityId', cap.id])
        cap.standardIds = _.map(_.flatMap(cap.standards), 'id')
        return cap
      }),
      standards: groupedStandards,
      errors: err,
      csrfToken: req.csrfToken(),
      pageHasForm:true
    })
  }
})

app.get('/solutions/:solution_id/mobile', csrfProtection, async (req, res) => {
  const solutionEx = await api.get_solution_by_id(req.params.solution_id)

  // only draft solutions can be edited
  if (solutionEx.solution.status !== api.SOLUTION_STATUS.DRAFT) {
    return res.redirect('/suppliers/solutions')
  }

  const { standards } = await api.get_all_capabilities()

  const context = {
    backlink: {
      title: 'Select Capabilities',
      subtitle: '(2 of 4)',
      href: `/suppliers/solutions/${req.params.solution_id}/capabilities`
    },
    csrfToken: req.csrfToken(),
    isMobile: _.some(solutionEx.claimedStandard, ['standardId', 'CSS3']),
    standard: _.find(standards, ['id', 'CSS3']),
    pageHasForm: true,
    solution: {name: solutionEx.solution.name}
  }

  res.render('supplier/solution-mobile', context)
})

app.post('/solutions/:solution_id/mobile', csrfProtection, async (req, res) => {
  const solutionEx = await api.get_solution_by_id(req.params.solution_id)

  // only draft solutions can be edited
  if (solutionEx.solution.status !== api.SOLUTION_STATUS.DRAFT) {
    return res.redirect('/suppliers/solutions')
  }

  _.remove(solutionEx.claimedStandard, ['standardId', 'CSS3'])
  if (req.body.isMobile === 'yes') {
    solutionEx.claimedStandard.push({
      'standardId': 'CSS3'
    })
  }

  const context = {
    backlink: {
      title: 'Select Capabilities',
      subtitle: '(2 of 4)',
      href: `/suppliers/solutions/${req.params.solution_id}/capabilities`
    },
    csrfToken: req.csrfToken(),
    isMobile: _.some(solutionEx.claimedStandard, ['standardId', 'CSS3']),
    pageHasForm: true
  }

  try {
    let redirectUrl = `${req.baseUrl}/solutions`

    if (req.body.action === 'continue') {
      redirectUrl = `${req.baseUrl}/solutions/${solutionEx.solution.id}/review`
    }
    if(req.body.action === 'save') {
      redirectUrl = `${req.baseUrl}/solutions/${solutionEx.solution.id}/mobile`
    }

    await api.update_solution(solutionEx)

    res.redirect(redirectUrl)
  } catch (err) {
    context.errors = err
    res.render('supplier/solution-mobile', context)
  }
})

app.get('/solutions/:solution_id/review', csrfProtection, async (req, res) => {
  const context = {
    backlink: {
      title: 'Confirm Mobile',
      subtitle: '(3 of 4)',
      href: `/suppliers/solutions/${req.params.solution_id}/mobile`
    },
    csrfToken: req.csrfToken(),
    errors: {},
    editLinks:{
      contacts:`/suppliers/solutions/${req.params.solution_id}/edit`,
      capabilities:`/suppliers/solutions/${req.params.solution_id}/capabilities`,
      mobile:`/suppliers/solutions/${req.params.solution_id}/mobile`
    },
  }

  try {
    const solutionEx = await api.get_solution_by_id(req.params.solution_id)

    // only draft solutions can be edited
    if (solutionEx.solution.status !== api.SOLUTION_STATUS.DRAFT) {
      return res.redirect('/suppliers/solutions')
    }

    // generate review context
    const { capabilities, groupedStandards } = await api.get_all_capabilities()

    context.solution = solutionEx.solution
    const allCaps = _.keyBy(capabilities, 'id')

    context.capabilityCount = solutionEx.claimedCapability.length
    context.capabilitySingular = context.capabilityCount === 1
    context.capabilityEditUrl = './capabilities'

    context.capabilities = _.map(
      solutionEx.claimedCapability,
      ({capabilityId}) => allCaps[capabilityId].name
    )


    context.contacts = solutionEx.technicalContact;
    context.supportsMobile = _.some(solutionEx.claimedStandard, ['standardId', 'CSS3'])


    // merge optional with interop standards prior to display, so that Mobile Working
    // is considered solution-specific
    groupedStandards.interop = _.concat(groupedStandards.interop, groupedStandards.optional)
    delete groupedStandards.optional

    context.standardCount = solutionEx.claimedStandard.length
    context.standardSingular = context.standardCount === 1
    context.standards = _(groupedStandards)
      .map((stds, type) => [
        type,
        stds.filter(std => _.find(solutionEx.claimedStandard, ['standardId', std.id]))
      ])
      .filter(([, stds]) => stds.length)
      .fromPairs()
      .value()

  } catch (err) {
    context.errors.general = err
  }

  res.render('supplier/solution-review', context)
})

app.post('/solutions/:solution_id/review', csrfProtection, async (req, res) => {
  const solutionEx = await api.get_solution_by_id(req.params.solution_id)

  // only draft solutions can be edited
  if (solutionEx.solution.status !== api.SOLUTION_STATUS.DRAFT) {
    return res.redirect('/suppliers/solutions')
  }

  const context = {
    backlink: {
      title: 'Confirm Mobile',
      subtitle: '(3 of 4)',
      href: `/suppliers/solutions/${req.params.solution_id}/mobile`
    },
    csrfToken: req.csrfToken(),
    errors: {}
  }

  try {
    let redirectUrl = `${req.baseUrl}/solutions`

    if (req.body.action === 'continue') {
      solutionEx.solution.status = api.SOLUTION_STATUS.REGISTERED
      redirectUrl = `${req.baseUrl}/solutions/${solutionEx.solution.id}?submitted`
    }

    await api.update_solution(solutionEx)

    res.redirect(redirectUrl)
  } catch (err) {
    context.errors.general = err
    res.render('supplier/solution-review', context)
  }
})

function renderProductPageEditor (req, res, solutionEx, context) {
  enrichContextForProductPage(context, solutionEx)
  context.breadcrumbs = [
    { label: 'My Dashboard', url: '/suppliers' },
    { label: 'My Solutions', url: '/suppliers/solutions' },
    { label: 'Onboarding Solution', url: `/suppliers/solutions/${req.params.solution_id}` },
    { label: 'Solution Page' }
  ]
  context.csrfToken = req.csrfToken()
  context.productPage = solutionEx.solution.productPage ? JSON.parse(solutionEx.solution.productPage) : {}
  
  const pageEditLinkPrefix = `/suppliers/solutions/${req.params.solution_id}/product-page`;
  context.pageEditLinks = {
    features: `${pageEditLinkPrefix}/features`,
    integrations: `${pageEditLinkPrefix}/integrations`,
    summary: `${pageEditLinkPrefix}/summary`,
    about: `${pageEditLinkPrefix}/about`
  }
  
  res.render('supplier/solution-page-edit', context)
}

app.get('/solutions/:solution_id/product-page', csrfProtection, async (req, res) => {
  const context = {
    errors: {}
  }

  let solutionEx

  try {
    // load from session when coming back from preview, if ID is the same
    solutionEx = req.session.solutionEx && req.session.solutionEx.solution.id === req.params.solution_id
              ? req.session.solutionEx
              : await api.get_solution_by_id(req.params.solution_id)

    context.capabilities = _.get(await api.get_all_capabilities(), 'capabilities')

    if (solutionEx.solution.productPage.message) {
      context.message = await formatting.formatMessagesForDisplay([
        _.merge({}, solutionEx.solution.productPage.message)
      ])
    }

    context.organisationName = _.get(await api.get_org_by_id(solutionEx.solution.organisationId), 'name')

    context.allowSubmit = solutionEx.solution.status === api.SOLUTION_STATUS.SOLUTION_PAGE

    // if the page has not been approved, allow the user to submit for review
    context.allowReview = solutionEx.solution.status === api.SOLUTION_STATUS.SOLUTION_PAGE

    // if the page has been approved, allow the user to publish
    context.allowPublish = solutionEx.solution.status === api.SOLUTION_STATUS.APPROVED &&
                          context.productPage.status === 'APPROVED'

  } catch (err) {
    context.errors.general = err
  }

  renderProductPageEditor(req, res, solutionEx, context)
})

app.post('/solutions/:solution_id/product-page', csrfProtection, async (req, res) => {
  const context = {
    errors: _.mapValues(
      _.groupBy(validationResult(req).array(), 'param'),
      errs => _.map(errs, 'msg')
    )
  }
  let solutionEx

  try {
    solutionEx = await api.get_solution_by_id(req.params.solution_id)
    context.capabilities = _.get(await api.get_all_capabilities(), 'capabilities')
    context.organisationName = _.get(await api.get_org_by_id(solutionEx.solution.organisationId), 'name')
  } catch (err) {
    context.errors.general = err
    return renderProductPageEditor(req, res, solutionEx, context)
  }

  // Sanity Validation should be done at a per-form page basis.
  // This post handler Just needs to ensure that all elements are actually present and have been validated.
  
  // const validated = matchedData(req)
  // solutionEx.solution.name = validated.name || req.body.name
  // solutionEx.solution.version = validated.version || req.body.version
  // solutionEx.solution.description = validated.description || req.body.description

  // const updateForProductPage = {
  //   benefits: _.filter(_.map(_.castArray(req.body.benefits), _.trim)),
  //   interop: _.filter(_.map(_.castArray(req.body.interop), _.trim)),
  //   contact: _.pickBy(_.mapValues(req.body.contact, _.trim)),
  //   capabilities: req.body.capabilities ? _.castArray(req.body.capabilities) : [],
  //   requirements: _.filter(_.map(_.castArray(req.body.requirements), _.trim)),
  //   'case-study': _.filter(_.castArray(req.body['case-study']), cs => cs.title),
  //   optionals: {
  //     'additional-services': _.mapValues(
  //       _.pickBy(req.body['additional-services']),
  //       val => val === 'yes'
  //     )
  //   },
  //   about: validated.about || req.body.about
  // }

  // encode the uploaded logo (if any)
  // if (req.files.logo) {
  //   const logo = req.files.logo
  //   updateForProductPage.logoUrl = `data:${logo.mimetype};base64,${logo.data.toString('base64')}`
  // } else if (req.body.preserveLogo && req.session.solutionEx) {
  //   updateForProductPage.logoUrl = _.get(req.session.solutionEx, 'solution.productPage.logoUrl', '')
  // } else if (!req.body.preserveLogo) {
  //   updateForProductPage.logoUrl = ''
  // }

  // solutionEx.solution.productPage = _.assign(
  //   {},
  //   solutionEx.solution.productPage,
  //   updateForProductPage
  // )

  // if there are validation errors, re-render the editor
  if (!_.isEmpty(context.errors)) {
    return renderProductPageEditor(req, res, solutionEx, context)
  }

  const action = _.head(_.keys(req.body.action))
  let redirect = `${req.baseUrl}/solutions`

  if (action === 'review' && solutionEx.solution.status === api.SOLUTION_STATUS.SOLUTION_PAGE) {
    solutionEx.solution.productPage.status = 'SUBMITTED'
    redirect = `${req.baseUrl}/solutions/${solutionEx.solution.id}?submitted`
  }
  else if (action === 'publish' && solutionEx.solution.status === api.SOLUTION_STATUS.APPROVED) {
    solutionEx.solution.productPage.status = 'PUBLISH'
  }

  req.session.solutionEx = await api.update_solution(solutionEx)
  res.redirect(redirect)
})

app.get('/solutions/:solution_id/product-page/:section_name', csrfProtection, async (req, res) => {
  const context = {
    errors : '',
    csrfToken: req.csrfToken(),
    pageHasForm:true
  }

  context.breadcrumbs = [
    { label: 'My Dashboard', url: '/suppliers' },
    { label: 'My Solutions', url: '/suppliers/solutions' },
    { label: 'Onboarding Solution', url: `/suppliers/solutions/${req.params.solution_id}` },
    { label: 'Solution Page', url: `/suppliers/solutions/${req.params.solution_id}/product-page`},
    { label: req.params.section_name}
  ]

  let solutionEx = req.session.solutionEx
  solutionEx = await api.get_solution_by_id(req.params.solution_id)

  const productPage = solutionEx.solution.productPage ? JSON.parse(solutionEx.solution.productPage) : {}

  enrichContextForProductPage(context, solutionEx)

  context.productPage = productPage;
  context.solution = solutionEx.solution;

  res.render(`supplier/product-page/${req.params.section_name}`, context)
});

const validateSolutionName = (fieldName = 'name') =>
  check(fieldName, 'Solution name must be present and has a maximum length of 60 characters')
  .exists()
  .isLength({min: 1, max: 60})
  .trim()

const validateSolutionVersion = (fieldName = 'version') =>
  check(fieldName, 'Solution version has a maximum length of 10 characters')
  .isLength({max: 10})
  .trim()

const validateSolutionDescription = (fieldName = 'description') =>
  check(fieldName, 'Solution description must be present and has a maximum length of 1000 characters')
  .exists()
  .isLength({min: 1, max: 1000})
  .trim()

const validateAbout = (fieldName = 'about') =>
  check(fieldName, 'Company information has a maximum length of 400 characters')
  .isLength({max: 400})
  .trim()

function validateFormArray(array) {
  const maxLengthCheck = (array) => array.length <= 9;
  const minLengthChcek = (array) => array.length > 0;
  return _.defaults(
    !maxLengthCheck(array) ? {message: 'Please enter 9 or fewer items'} : {},
    !minLengthChcek(array) ? {message: 'Please enter at least one item'} : {}
  );
}
function parseArrayItems(items) {
  if(!items) {
    return [];
  }
  else if(items.filter) {
    return items.filter((item => item != ''));
  }
  else {
    return [items];
  }
}

app.post('/solutions/:solution_id/product-page/:section_name', csrfProtection, async (req,res) => {
  const context = {
    errors : '',
    csrfToken: req.csrfToken(),
    pageHasForm:true
  }

  let solutionEx = req.session.solutionEx
  solutionEx = await api.get_solution_by_id(req.params.solution_id)

  const productPage = solutionEx.solution.productPage ? JSON.parse(solutionEx.solution.productPage) : {};

  const arrayForms = ['features', 'integrations'];
  const tableForms = ['service-scope', 'customer-insights', 'data-import-export', 'user-support', 'migration-switching', 'audit-info'];

  const sectionName = req.params.section_name

  if(arrayForms.indexOf(sectionName) > -1) {
    const sectionElements = parseArrayItems(req.body.items)
    context.errors = validateFormArray(sectionElements);
    productPage[sectionName] = sectionElements;
  }
  else if (sectionName === 'summary') {
    solutionEx.solution.description = req.body.text || '';
  }
  else if (sectionName === 'about') {
    productPage[sectionName] = req.body.text || '';
  }

  let redirectURL = `${req.baseUrl}/solutions/${req.params.solution_id}/product-page`;

  if(context.errors.message) {
    redirectURL = `${req.baseUrl}/solutions/${req.params.solution_id}/product-page/${req.params.section_name}`;
    return res.render(`supplier/product-page/${req.params.section_name}`, context)
  }
  else if(req.body.action === 'save') {
    redirectURL = `${req.baseUrl}/solutions/${req.params.solution_id}/product-page/${req.params.section_name}`;
  }

  solutionEx.solution.productPage = JSON.stringify(productPage);

  req.session.solutionEx = await api.update_solution(solutionEx)

  res.redirect(redirectURL)
})

app.use('/solutions/:solution_id/assessment', require('./supplier/assessment'))
app.use('/solutions/:solution_id/compliance', require('./supplier/compliance'))

module.exports = app
