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

  if (!context.productPage.optionals) {
    context.productPage.optionals = {}
  }

  context.productPage = solutionEx.solution.productPage ? JSON.parse(solutionEx.solution.productPage) : {}

  const sections = ['service-scope', 'customer-insights', 'user-support', 'import-exports']
  sections.forEach((section) => {
    console.log(section, context.productPage[section]);
    context.productPage[section] = _.map(context.productPage[section], (value, key) => {
      return { key: key, value: value }
    })
  })
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

}

module.exports = {
  enrichContextForProductPage,
  enrichContextForProductPagePreview
}
