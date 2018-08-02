const _ = require('lodash')
const api = require('catalogue-api')
const app = require('express').Router()
const csrfProtection = require('csurf')()
const { formatting } = require('catalogue-utils')
const {
  enrichContextForProductPagePreview
} = require('./supplier/product-page')

function assessmentEndpointForSolution (solution) {
  return {
    [api.SOLUTION_STATUS.CAPABILITIES_ASSESSMENT]: 'assess',
    [api.SOLUTION_STATUS.STANDARDS_COMPLIANCE]: 'comply',
    [api.SOLUTION_STATUS.SOLUTION_PAGE]: 'produce'
  }[solution.status]
}

app.get('/', async (req, res) => {
  const solutions = _(await api.get_solutions_for_assessment())
    .map(soln => ({
      ...soln,
      assessUrl: `${req.baseUrl}/${soln.id}/${assessmentEndpointForSolution(soln)}`
    }))
    .groupBy(soln => api.solutionStatuses[soln.status])
    .value()

  res.render('assessment/dashboard', {
    solutions,
    inviteUrl: `${req.baseUrl}/invite`
  })
})

app.get('/invite', csrfProtection, async (req, res) => {
  const context = {
    suppliers: await api.get_supplier_orgs(),
    cancelUrl: req.baseUrl,
    csrfToken: req.csrfToken()
  }

  res.render('assessment/invite', context)
})

app.post('/invite', csrfProtection, async (req, res) => {
  try {
    const organisationId = req.body.supplierId ||
      _.get(await api.create_supplier_org({
        name: req.body.supplier,
        odsCode: req.body.odsCode
      }), 'id')

    if (req.body.emailAddress) {
      try {
        await api.get_contact_for_user({email: req.body.emailAddress})
        throw new Error(`Contact with email address ${req.body.emailAddress} already exists`)
      } catch (err) {
        await api.create_contact({
          organisationId,
          firstName: req.body.firstName,
          lastName: req.body.lastName,
          emailAddress1: req.body.emailAddress
        })
      }
    }

    res.redirect(req.originalUrl)
  } catch (error) {
    res.render('assessment/invite', {
      ...req.body,
      error,
      csrfToken: req.csrfToken()
    })
  }
})

app.param('solution_id', function (req, res, next, id) {
  api.get_solution_by_id(id)
    .then(solutionEx => {
      req.solutionEx = solutionEx
      next()
    })
    .catch(next)
})

const assessRouter = require('express').Router()

assessRouter.route('/assess')
.get(async (req, res) => {
  const solutionEx = req.solutionEx
  const [messages, {capabilities, standards}] = await Promise.all([
    api.get_assessment_messages_for_solution(solutionEx.solution.id),
    api.get_all_capabilities()
  ])

  // load the contact details for the messages and embed

  // temporarily do this by loading all the contacts for the current user's org
  // and the supplier's org - Trevor will build an API for this on Monday
  const contacts = _.keyBy(
    _.flatten(
      await Promise.all([
        api.get_contacts_for_org(req.user.org.id),
        api.get_contacts_for_org(solutionEx.solution.organisationId)
      ])
    ),
    'id'
  )

  _.each(messages, message => {
    message.contact = contacts[message.contactId]
    message.displayTimestamp = formatting.formatTimestampForDisplay(message.timestamp)
  })

  // build the status drop-down
  _.each(solutionEx.claimedCapability, cap => {
    cap.statuses = _.map(api.capabilityStatuses, (label, value) => ({
      label,
      value,
      selected: cap.status == value
    }))
  })

  const questions = _.mapValues(await api.get_capability_assessment_questions(), qs => ({
    lede: _.head(qs),
    points: _.tail(qs)
  }))

  res.render('assessment/assess', {
    solution: solutionEx,
    messages: _.orderBy(messages, 'timestamp', 'desc'),
    capabilities: _.keyBy(capabilities, 'id'),
    standards: _.keyBy(standards, 'id'),
    questions,
    csrfToken: req.csrfToken(),
    assessBaseUrl: `${req.baseUrl}`
  })
})
.post(async (req, res) => {
  const solutionEx = req.solutionEx

  // apply status to each claimed capability
  _.each(req.body.status, (status, claimedCapabilityId) => {
    const claimedCapability = _.find(solutionEx.claimedCapability, ['id', claimedCapabilityId])
    if (claimedCapability) {
      claimedCapability.status = status
    }
  })

  // apply overall status
  if (req.body.overall === 'approve') {
    solutionEx.solution.status = api.SOLUTION_STATUS.STANDARDS_COMPLIANCE
  } else if (req.body.overall === 'reject') {
    solutionEx.solution.status = api.SOLUTION_STATUS.DRAFT
  }

  // post a new message if one was supplied
  const message = (req.body.message || '').trim()
  if (message) {
    await api.post_assessment_message({
      solutionId: solutionEx.solution.id,
      contactId: req.user.contact.id,
      timestamp: new Date().toJSON(),
      message
    })
  }

  // update the solution
  await api.update_solution(solutionEx)

  res.redirect(req.originalUrl)
})

assessRouter.route('/comply')
.get(async (req, res) => {
  const solutionEx = req.solutionEx
  const context = {
    csrfToken: req.csrfToken(),
    solution: solutionEx.solution
  }

  let { capabilities, standards } = await api.get_all_capabilities()
  standards = _.keyBy(standards, 'id')

  context.standards = _(solutionEx.claimedStandard)
    .map(claimedStd => ({
      ...claimedStd,
      name: standards[claimedStd.standardId].name,
      description: standards[claimedStd.standardId].description,
      evidence: (claimedStd.evidence && JSON.parse(claimedStd.evidence)) || {},
      statuses: _.map(api.standardStatuses, (status, index) => ({
        label: status,
        selected: claimedStd.status === index
      }))
    }))
    .value()

  res.render('assessment/comply.html', context)
})
.post(async (req, res) => {
  const solutionEx = req.solutionEx

  if (req.body.save) {
    if (req.body.overall === 'approve') {
      solutionEx.solution.status = api.SOLUTION_STATUS.SOLUTION_PAGE
    } else if (req.body.overall === 'reject') {
      solutionEx.solution.status = api.SOLUTION_STATUS.DRAFT
    }

    const standardIdToSave = _.isObject(req.body.save) && _.head(_.keys(req.body.save))

    if (standardIdToSave) {
      const standardToUpdate = _.find(solutionEx.claimedStandard, ['standardId', standardIdToSave])

      standardToUpdate.status = req.body.standard[standardIdToSave].status

      // post a new message if one was supplied
      const message = (req.body.standard[standardIdToSave].message || '').trim()
      if (message) {
        const submission = [{
          contactId: req.user.contact.id,
          timestamp: new Date().toJSON(),
          message
        }]

        const evidence = (standardToUpdate.evidence && JSON.parse(standardToUpdate.evidence)) || {}
        evidence.submissions = _.concat(submission, evidence.submissions || [])
        standardToUpdate.evidence = JSON.stringify(evidence)
      }
    }

    await api.update_solution(solutionEx)
  }

  res.redirect(req.originalUrl)
})

assessRouter.route('/produce')
.get(async (req, res) => {
  const context = {
    csrfToken: req.csrfToken()
  }
  const solutionEx = req.solutionEx
  await enrichContextForProductPagePreview(context, solutionEx)

  res.render('assessment/produce', context)
})
.post(async (req, res) => {
  const solutionEx = req.solutionEx

  // apply overall status
  if (req.body.overall === 'product_approve') {
    solutionEx.solution.status = api.SOLUTION_STATUS.APPROVED
    solutionEx.solution.productPage.status = 'APPROVED'
  } else if (req.body.overall === 'product_reject') {
    solutionEx.solution.status = api.SOLUTION_STATUS.SOLUTION_PAGE
    solutionEx.solution.productPage.status = 'REMEDIATION'
  } else if (req.body.overall === 'publish') {
    solutionEx.solution.status = api.SOLUTION_STATUS.APPROVED
    solutionEx.solution.productPage.status = 'PUBLISH'
  }

  // post a new message if one was supplied
  const message = (req.body.message || '').trim()
  if (message) {
    solutionEx.solution.productPage.message = {
      contactId: req.user.contact.id,
      timestamp: new Date().toJSON(),
      message
    }
  }

  // update the solution
  await api.update_solution(solutionEx)
  res.redirect(req.originalUrl)
})

app.use('/:solution_id', csrfProtection, assessRouter)

module.exports = app
