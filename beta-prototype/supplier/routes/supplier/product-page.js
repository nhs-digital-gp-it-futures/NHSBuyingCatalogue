const _ = require('lodash')
const api = require('catalogue-api')

function enrichContextForProductPage (context, solutionEx) {
  context.productPage = _.merge({}, solutionEx.solution.productPage)

  context.solution = _.merge({}, solutionEx.solution)

  // if there is no contact saved on the product page, copy in the solution's lead
  // contact as the default
  const hasNoContactDetails = _.isEmpty(
    _.intersection(
      _.keys(context.productPage.contact),
      ['firstName', 'lastName', 'emailAddress', 'phoneNumber']
    )
  )

  if (hasNoContactDetails) {
    const solutionLeadContact = _.find(
      solutionEx.technicalContact,
      ['contactType', 'Lead Contact']
    )
    context.productPage.contact = _.defaults(
      context.productPage.contact || {},
      solutionLeadContact
    )
  }

  // build list of capabilities with video selection status
  const defaultVideoSelected = !('capabilities' in context.productPage)
  context.productPage.capabilities = _.map(
    solutionEx.claimedCapability,
    cap => ({
      ..._.find(context.capabilities, ['id', cap.capabilityId]),
      videoUrl: cap.evidence,
      selected: defaultVideoSelected || _.includes(context.productPage.capabilities, cap.capabilityId)
    })
  )

  context.benefitOptions = [
    'Export functionality',
    'Customisable reporting suite',
    'GDPR compliant',
    'Responsive design for accessing via all screen sizes - mobile, desktop, etc',
    'Time saving for GPs and staff',
    'User role & permission management'
  ]

  // build lists of options for specs
  context.interopOptions = ['EMIS', 'TPP', 'Vision', 'Microtest']
  context.requirementOptions = [
    'None',
    {group: 'Operating Systems', items: ['Linux Debian 8 or Ubuntu 16.04 OS', 'Windows', 'macOS']},
    {group: 'Processor Speed', items: ['Multi-core processor is recommended']},
    {group: 'Miscellaneous', items: ['Users must have internet connectivity', 'Users must have access to a common browser platform']}
  ]

  // construct the optional sets from the product page data
  context.optionals = {
    'additional-services': _.map({
      'training-onsite': 'Training (on site)',
      'training-online': 'Training (online)',
      'training-virtual': 'Training (virtual classroom)',
      'deployment-support': 'Deployment Support',
      'data-migration': 'Data Migration',
      'designated-service-contact': 'Designated Service Contact',
      'audit-trail-retrieval': 'Audit Trail Retrieval'
    }, (label, name) => {
      const path = ['optionals', 'additional-services', name]
      const chosen = _.has(context.productPage, path)
      const enabled = !!_.get(context.productPage, path)
      return {
        label,
        name: `additional-services[${name}]`,
        enabled: chosen && enabled,
        disabled: chosen && !enabled
      }
    })
  }
}

async function enrichContextForProductPagePreview (context, solutionEx) {
  const allCaps = await api.get_all_capabilities()
  context.capabilities = _.get(allCaps, 'capabilities')
  context.standards = _.get(allCaps, 'groupedStandards')
  context.organisationName = _.get(await api.get_org_by_id(solutionEx.solution.organisationId), 'name')

  enrichContextForProductPage(context, solutionEx)

  // process capabilities so only selected ones are in the carousel
  context.carousel = _(context.productPage.capabilities)
    .filter('selected')
    .map(cap => {
      // for YouTube share URLs, extract the video ID
      const matches = /https:\/\/youtu\.be\/([^?#]+)/.exec(cap.videoUrl)
      if (matches) {
        cap.youtubeUrl = matches[1]
      }
      return cap
    })
    .value()

  // process optionals for display
  context.optionals['additional-services'] = _.map(
    _.filter(context.optionals['additional-services'], opt => opt.enabled || opt.disabled),
    opt => ({
      ...opt,
      value: opt.enabled ? 'yes' : 'no'
    })
  )
}

module.exports = {
  enrichContextForProductPage,
  enrichContextForProductPagePreview
}
