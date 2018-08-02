const _ = require('lodash')
const api = require('catalogue-api')
const app = require('express').Router({ mergeParams: true })
const csrfProtection = require('csurf')()
const { formatting } = require('catalogue-utils')

const STATUS_CLASS_MAP = {
  [api.STANDARD_STATUS.SUBMITTED]: 'submitted',
  [api.STANDARD_STATUS.REMEDIATION]: 'remediation',
  [api.STANDARD_STATUS.APPROVED]: 'approved',
  [api.STANDARD_STATUS.REJECTED]: 'rejected',
  [api.STANDARD_STATUS.PARTIALLY_APPROVED]: 'approved'
}

app.get('/', async (req, res) => {
  const context = {
    breadcrumbs: [
      { label: 'My Dashboard', url: '/suppliers' },
      { label: 'My Solutions', url: '/suppliers/solutions' },
      { label: 'Onboarding Solution', url: `/suppliers/solutions/${req.params.solution_id}` },
      { label: 'Standards Compliance' }
    ],
    errors: {}
  }

  try {
    const solutionEx = await loadEnrichedSolution(req.params.solution_id, req.baseUrl)
    context.solution = solutionEx.solution
  } catch (err) {
    context.errors.general = err
  }

  res.render('supplier/compliance', context)
})

function stdWithEvidenceParsed (std) {
  try {
    std.evidence = std.evidence ? JSON.parse(std.evidence) : {}
  } catch (err) {
    std.evidence = {}
  }

  return std
}

// This works because 🦄🌈
function isCapabilitySpecificStandard (stdId) {
  return stdId.startsWith('CSS') && stdId !== 'CSS1' && stdId !== 'CSS2' && stdId !== 'CSS3'
}

function standardName (std) {
  return isCapabilitySpecificStandard(std.id)
       ? `${std.name} Standard` 
       : std.name
}

function standardOrdering (stdA, stdB) {
  const stdAisCapabilitySpecific = isCapabilitySpecificStandard(stdA.id)
  const stdBisCapabilitySpecific = isCapabilitySpecificStandard(stdB.id)

  // sort capability-specific standards above all others
  if (stdAisCapabilitySpecific && !stdBisCapabilitySpecific) return -1
  if (stdBisCapabilitySpecific && !stdAisCapabilitySpecific) return 1

  // otherwise sort by name
  return stdA.name.localeCompare(stdB.name)
}

async function loadEnrichedSolution (solutionId, baseUrl) {
  const [solutionEx, { capabilities, groupedStandards }] = await Promise.all([
    api.get_solution_by_id(solutionId),
    api.get_all_capabilities()
  ])
  const owners = await api.get_contacts_for_org(solutionEx.solution.organisationId)

  const allCapabilities = _.keyBy(capabilities, 'id')

  // enrich the claimed capability standards with the capability ID to which they refer
  const allClaimedCapabilityStandards = _.map(
    solutionEx.claimedCapabilityStandard,
    claimedCapStd => {
      const claimedCapability = _.find(
        solutionEx.claimedCapability,
        ['id', claimedCapStd.claimedCapabilityId]
      )

      return {
        ...claimedCapStd,
        capabilityId: claimedCapability.capabilityId
      }
    }
  )

  const allClaimedStandards = _.each(
    _.concat(
      solutionEx.claimedStandard,
      allClaimedCapabilityStandards
    ),
    stdWithEvidenceParsed
  )

  // as computing the context for a standard needs information about the organisation to
  // last submit, parse and format all evidence for standards first
  await formatting.formatMessagesForDisplay(
    _.filter(_.flatMap(allClaimedStandards, 'evidence.submissions'))
  )

  // given a complete set of standards associated with a capability, return an
  // enriched subset of standards based on those claimed by the solution
  function enrichedCapabilityStandards (cap, capStds) {
    const claimedStandardSubset = _.intersectionWith(
      allClaimedStandards, capStds,
      (claimedStd, capStd) =>
        claimedStd.standardId === capStd.id &&
        (claimedStd.capabilityId ? claimedStd.capabilityId === cap.capabilityId : true)
    )

    return _(claimedStandardSubset)
      .map(claimedStd => ({
        ...claimedStd,
        ..._.find(capStds, ['id', claimedStd.standardId]),
        ...standardContext(claimedStd, baseUrl, cap)
      }))
      .map(context => {
        context.owners = ownersContext(context, owners)
        context.name = standardName(context)
        return context
      })
      .value()
      .sort(standardOrdering)
  }

  // set the list of claimed capabilities for the solution
  // this matches the design of the wireframes i.e. standards that are associated with
  // multiple capabilities are cloned under each, with optional context-specific standards
  // associated with the capability under which they were claimed

  solutionEx.solution.capabilities = _(solutionEx.claimedCapability)
    .map(cap => ({
      ...cap,
      ...allCapabilities[cap.capabilityId],
      standards: _.mapValues(
        allCapabilities[cap.capabilityId].standards,
        stds => enrichedCapabilityStandards(cap, stds)
      )
    }))
    .map(cap => ({
      ...cap,
      ...capabilityContext(cap)
    }))
    .value()

  // set the list of overarching claimed standards
  solutionEx.solution.standards = _
    .chain(solutionEx.claimedStandard)
    .reduce((acc, std) => {
      const oarchStd = _.find(groupedStandards.overarching, ['id', std.standardId])
      if (oarchStd) {
        acc.push({
          ...std,
          ...oarchStd,
          ...standardContext(std, baseUrl)
        })
      }
      return acc
    }, [])
    .map(context => {
      context.owners = ownersContext(context, owners)
      context.name = standardName(context)
      return context
    })
    .value()
    .sort(standardOrdering)

  return solutionEx
}

function capabilityContext (cap) {
  return {
    status: _.some(cap.standards, stds => _.some(stds, std => !_.isEmpty(std.evidence)))
            ? 'In Progress'
            : 'Not Started'
  }
}

function standardContext (std, baseUrl, cap = undefined) {
  const urlSuffix = cap
                  ? `capability/${cap.capabilityId}#std-${std.standardId}`
                  : `standard/${std.standardId}`

  const evidence = std.evidence
  const hasSubmissions = !_.isEmpty(evidence.submissions)
  const hasSaved = !!(evidence.savedLink || evidence.savedMessage)
  const latestContact = (_.head(evidence.submissions) || {}).contact || {}

  const isWithAssessmentTeam = hasSubmissions && latestContact.organisationId !== api.NHS_DIGITAL_ORG_ID
  const hasFeedback = hasSubmissions && latestContact.organisationId === api.NHS_DIGITAL_ORG_ID

  const isEditable = std => (std.status === api.STANDARD_STATUS.SUBMITTED ||
    std.status === api.STANDARD_STATUS.REMEDIATION) && !isWithAssessmentTeam

  return std && {
    status: std.status === api.STANDARD_STATUS.SUBMITTED
            ? hasSubmissions
                ? hasFeedback ? 'Feedback' : 'Submitted'
                : hasSaved ? 'Draft' : 'Not started'
            : api.standardStatuses[std.status],
    statusClass: std.status === api.STANDARD_STATUS.SUBMITTED
                 ? hasSubmissions
                   ? hasFeedback ? 'feedback' : 'submitted'
                   : ''
                 : STATUS_CLASS_MAP[std.status] || '',
    evidence,
    isWithAssessmentTeam,
    saved: hasSaved,
    viewUrl: !isEditable(std) && `${baseUrl}/edit/${urlSuffix}`,
    editUrl: isEditable(std) && `${baseUrl}/edit/${urlSuffix}`
  }
}

function ownersContext (context, owners) {
  if (owners && owners.length > 1) {
    return _.map(owners, owner => ({
      ...owner,
      selected: owner.id === context.evidence.owner
    }))
  }
}

async function complianceEditHandler (req, res) {
  const context = {
    breadcrumbs: [
      { label: 'My Dashboard', url: '/suppliers' },
      { label: 'My Solutions', url: '/suppliers/solutions' },
      { label: 'Onboarding Solution', url: `/suppliers/solutions/${req.params.solution_id}` },
      { label: 'Standards Compliance', url: `/suppliers/solutions/${req.params.solution_id}/compliance` }
    ],
    errors: {},
    csrfToken: req.csrfToken()
  }

  try {
    const solutionEx = await loadEnrichedSolution(req.params.solution_id, req.baseUrl)

    // could be editing either all standards for a capability
    // or a single overarching standard
    if (req.params.capability_id) {
      context.capability = _.find(
        solutionEx.solution.capabilities,
        ['capabilityId', req.params.capability_id]
      )
      context.subtitle = context.capability.name
      context.standards = context.capability.standards
    } else {
      context.standard = _.find(
        solutionEx.solution.standards,
        ['standardId', req.params.standard_id]
      )
      context.subtitle = context.standard.name
      context.standards = {
        mandatory: [context.standard]
      }
    }

    context.breadcrumbs.push({ label: context.subtitle })

    context.solution = solutionEx.solution
    context.submittable = solutionEx.solution.status === api.SOLUTION_STATUS.STANDARDS_COMPLIANCE
  } catch (err) {
    context.errors.general = err
  }

  res.render('supplier/compliance-edit', context)
}

function findStandardToUpdate (solutionEx, standardId, capabilityId) {
  // the simple case is a directly claimed standard
  let stdToUpdate = _.find(solutionEx.claimedStandard, {standardId})
  if (stdToUpdate) return stdToUpdate

  const claimedCapability = _.find(solutionEx.claimedCapability, {capabilityId})

  // otherwise it must be a standard claimed against a capability
  return _.find(solutionEx.claimedCapabilityStandard, {
    standardId,
    claimedCapabilityId: claimedCapability.id
  })
}

async function complianceEditPostHandler (req, res) {
  const [solutionEx, { standards }] = await Promise.all([
    api.get_solution_by_id(req.params.solution_id),
    api.get_all_capabilities()
  ])

  const stdIdToUpdate = _.head(_.keys(req.body.save)) || _.head(_.keys(req.body.submit))
  const stdToUpdate = findStandardToUpdate(solutionEx, stdIdToUpdate, req.params.capability_id)
  let evidence

  try {
    evidence = JSON.parse(stdToUpdate.evidence)
  } catch (err) {
    evidence = {}
  }

  let updateSolution = false
  let redirect = req.originalUrl

  // the evidence to be saved is based on which "Save" or "Submit" button was pressed
  if (req.body.save || req.body.submit) {
    const linkToSave = _.get(req.body, ['evidence', stdIdToUpdate], '')
    const messageToSave = _.get(req.body, ['message', stdIdToUpdate], '')

    evidence.owner = _.get(req.body, ['owner', stdIdToUpdate], '')

    if (req.body.save) {
      evidence.savedLink = linkToSave
      evidence.savedMessage = messageToSave
      redirect = require('url').resolve(redirect, `#std-${stdIdToUpdate}`)
    } else {
      // submitting pushes the current link and message into the submission array
      delete evidence.savedLink
      delete evidence.savedMessage

      const submission = [{
        contactId: req.user.contact.id,
        timestamp: new Date().toJSON(),
        link: linkToSave,
        message: messageToSave
      }]

      evidence.submissions = _.concat(submission, evidence.submissions || [])
      redirect = `${req.baseUrl}/../submitted?std=${_.find(standards, ['id', stdIdToUpdate]).name}`
    }

    stdToUpdate.evidence = JSON.stringify(evidence)
    updateSolution = true
  }

  // update the solution if changes were made
  if (updateSolution) {
    await api.update_solution(solutionEx)
  }

  res.redirect(redirect)
}

app.get('/edit/capability/:capability_id', csrfProtection, complianceEditHandler)
app.get('/edit/standard/:standard_id', csrfProtection, complianceEditHandler)

app.post('/edit/capability/:capability_id', csrfProtection, complianceEditPostHandler)
app.post('/edit/standard/:standard_id', csrfProtection, complianceEditPostHandler)

module.exports = app
