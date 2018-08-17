const _ = require('lodash')
const api = require('catalogue-api')
const app = require('express').Router({ mergeParams: true })
const csrfProtection = require('csurf')()
const { formatting } = require('catalogue-utils')

app.get('/', csrfProtection, async (req, res) => {
  const dashboardUrl = `${req.baseUrl}/solutions`
  const [solutionEx, messages, { capabilities }] = await Promise.all([
    api.get_solution_by_id(req.params.solution_id),
    api.get_assessment_messages_for_solution(req.params.solution_id)
       .then(formatting.formatMessagesForDisplay),
    api.get_all_capabilities()
  ])

  // only registered solutions can participate in assessment
  if (solutionEx.solution.status !== api.SOLUTION_STATUS.REGISTERED &&
      solutionEx.solution.status !== api.SOLUTION_STATUS.CAPABILITIES_ASSESSMENT) {
    return res.redirect(dashboardUrl)
  }

  const allCapabilities = _.keyBy(capabilities, 'id')

  solutionEx.solution.capabilities = _.map(solutionEx.claimedCapability, cap => ({
    ...cap,
    ...allCapabilities[cap.capabilityId],
    evidence: cap.evidence,
    status: solutionEx.solution.status === api.SOLUTION_STATUS.CAPABILITIES_ASSESSMENT
            ? _.get(api.capabilityStatuses, cap.status)
            : cap.evidence ? 'Draft Saved' : 'Not Started',
    statusClass: (
                   solutionEx.solution.status === api.SOLUTION_STATUS.CAPABILITIES_ASSESSMENT
                   ? _.get(api.capabilityStatuses, cap.status, '').toLowerCase()
                   : ''
                 ) +
                 (
                   req.query.saved === cap.capabilityId
                                     ? ' expanded'
                                     : ''
                 ) +
                 (
                   cap.evidence ? '' : ' editing'
                 )
  }))

  const questions = _.mapValues(await api.get_capability_assessment_questions(), qs => ({
    lede: _.head(qs),
    points: _.tail(qs)
  }))

  res.render('supplier/assessment', {
    breadcrumbs: [
      { label: 'My Dashboard', url: '/suppliers' },
      { label: 'My Solutions', url: '/suppliers/solutions' },
      { label: 'Onboarding Solution', url: `/suppliers/solutions/${req.params.solution_id}` },
      { label: 'Capabilities Assessment' }
    ],
    solution: solutionEx.solution,
    messages: _.orderBy(messages, 'timestamp', 'desc'),
    questions,
    csrfToken: req.csrfToken()
  })
})

app.post('/', csrfProtection, async (req, res) => {
  const solutionEx = await api.get_solution_by_id(req.params.solution_id)
  let redirectUrl = `/suppliers/solutions/${solutionEx.solution.id}`
  let updateSolution = false

  // only registered solutions can participate in assessment
  if (solutionEx.solution.status !== api.SOLUTION_STATUS.REGISTERED &&
      solutionEx.solution.status !== api.SOLUTION_STATUS.CAPABILITIES_ASSESSMENT) {
    return res.redirect(redirectUrl)
  }

  // set the evidence for the capability that the user asked to save
  if (req.body.save) {
    redirectUrl = _.head(_.split(req.originalUrl, '?', 1))

    const capabilityIdToSave = _.head(Object.keys(req.body.save))
    const cap = _.find(solutionEx.claimedCapability, ['capabilityId', capabilityIdToSave])
    const evidence = _.get(req.body.evidence, capabilityIdToSave, '').trim()

    if (cap && evidence) {
      cap.evidence = evidence

      // update the solution
      updateSolution = true
    }

    redirectUrl += `?saved=${capabilityIdToSave}#evidence-${capabilityIdToSave}`
  }

  // set the status on submission to send the solution to the capabilities assessment team
  if (req.body.action === 'submit' &&
      solutionEx.solution.status === api.SOLUTION_STATUS.REGISTERED) {
    solutionEx.solution.status = api.SOLUTION_STATUS.CAPABILITIES_ASSESSMENT
    redirectUrl = `/suppliers/solutions/${solutionEx.solution.id}/submitted`
    updateSolution = true
  }

  if (updateSolution) {
    await api.update_solution(solutionEx)
  }

  // post a new message if one was supplied
  const message = (req.body.message || '').trim()
  if (message && (req.body.action === 'save' || req.body.action === 'submit')) {
    await api.post_assessment_message({
      solutionId: solutionEx.solution.id,
      contactId: req.user.contact.id,
      timestamp: new Date().toJSON(),
      message
    })
  }

  res.redirect(redirectUrl)
})

module.exports = app
