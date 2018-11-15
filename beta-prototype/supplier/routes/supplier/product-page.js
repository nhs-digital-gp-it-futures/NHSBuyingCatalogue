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
    const values = context.productPage[section]
    const form = require(`../../forms/${section}`)

    if (values && form) {
      context.productPage[section] = filterBlanks(mapDisplayValues(form.inputs, values))
    } else {
      _.omit(context.productPage, section)
    }
  })

  function mapDisplayValues (inputs, valueMap) {
    let displayMap = []

    inputs.forEach((input) => {
      const value = filterBlanks(valueMap[input.name])
      displayMap.push(mapInputValue(input, value))
      if (dependantsActive(input, value)) {
        displayMap = displayMap.concat(mapDisplayValues(input.dependants, valueMap))
      }
    })

    return displayMap
  }

  function dependantsActive (input, value) {
    // abusing the fact that right now, as only radio elements can have dependants.
    if (!hasDependants(input)) {
      return false
    } else if (hasTriggers(input)) {
      return hasTriggeringValue(input, value)
    } else {
      return hasValue(input)
    }
  }

  function mapInputValue (input, value) {
    let valueLabel = ''

    if (valueTriggersHidden(input, value)) {
      return {}
    }

    if (hasOptions(input)) {
      const option = _.find(input['options'], (o) => o.value === value)
      valueLabel = option.label
    }

    return {
      key: input.label || input.title,
      value: valueLabel || value
    }
  }

  function hasOptions (input) {
    return Array.isArray(input['options'])
  }

  function valueTriggersHidden (input, value) {
    if (hasHideTriggers(input)) {
      return input['hidden-on'].includes(value)
    } else {
      return false
    }
  }

  function filterBlanks (values) {
    if (Array.isArray(values)) {
      return values.filter((val) => {
        return !_.isEmpty(val)
      })
    }
    return values
  }

  function hasDependants (input) {
    return !_.isEmpty(input['dependants'])
  }

  function hasValue (input) {
    return !_.isEmpty(input['value'])
  }

  function hasHideTriggers (input) {
    return Array.isArray(input['hidden-on']) && !_.isEmpty(input['hidden-on'])
  }

  function hasTriggers (input) {
    return Array.isArray(input['dependant-on']) && !_.isEmpty(input['dependant-on'])
  }

  function hasTriggeringValue (input, value) {
    return input['dependant-on'].includes(value)
  }

  // Placeholder pricing information

  const placeholderServicePricing = [
    { label: 'Training (on site)', price: '£1000/day' },
    { label: 'Training (online)', price: '£2000/contract' },
    { label: 'Training (virtual classroom)', price: '£1500/contract' }
  ]
  context.services = {
    'associated-services': placeholderServicePricing,
    'additional-services': placeholderServicePricing
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
}

module.exports = {
  enrichContextForProductPage,
  enrichContextForProductPagePreview
}
