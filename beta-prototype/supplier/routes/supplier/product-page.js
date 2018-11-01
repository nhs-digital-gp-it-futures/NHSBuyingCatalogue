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

  // build lists of options for specs
  context.requirementOptions = [
    {group: 'Software add-on or Extension', items:['Yes']},
    {group: 'Cloud deployment model', items:['Private Cloud']},
    {group: 'Operating Systems', items: ['Linux Debian 8 or Ubuntu 16.04 OS', 'Windows', 'macOS']},
    {group: 'Processor Speed', items: ['Multi-core processor is recommended']},
    {group: 'Miscellaneous', items: ['Users must have internet connectivity', 'Users must have access to a common browser platform']}
  ]

    // placeholder data
    context.pricing = '£ / max per patient'

    // placeholder data
    context.commercialArrangements = 'NHS 1.0';

  // construct the optional sets from the product page data
  const placeholderServiceMap = {
    'training-onsite': 'Training (on site)',
    'training-online': 'Training (online)',
    'training-virtual': 'Training (virtual classroom)',
    'deployment-support': 'Deployment Support',
    'data-migration': 'Data Migration',
    'designated-service-contact': 'Designated Service Contact',
    'audit-trail-retrieval': 'Audit Trail Retrieval'
  };

  const placeholderCustomerInsights = [
    {title:'Customer Ref 1',items:[ {text:'Dr. Ranj Singh, GP'}, {text:'Leeds North'}, {text:'Email Dr. Ranj', link:'a@a.com'} ]},
    {title:'User Group',    items:[ {text:'Group Page', link:'#'} ]},
    {title:'Case Study',    items:[ {text:'Case Study Document', link:'#'} ]}
  ];

  const placeholderUserSupport = [
    {title:'Availability',      items:[ {text:'99.99%'} ]},
    {title:'Email Ticketing',   items:[ {text:'Yes'} ]},
    {title:'Online Ticketing',  items:[ {text:'Yes'} ]},
    {title:'Phone Support',     items:[ {text:'Yes'} ]},
    {title:'Phone Availability',items:[ {text:'09:00-17:00 (GMT)'} ]},
    {title:'Web Chat Support',  items:[ {text:'No'} ]}
  ];

  const placeholderDataImportExport = [
    {title:'Export Approach',items:[{text:'The population reporting module enables users to build reports on patients and other criteria, which can be exported to Microsoft Excel. Data extraction services allow users to analyse more complex criteria, and a data analytics service can be purchased separately.'}]},
    {title:'Export Format',items:[{text:'CSV'},{text:'TSV'}]},
    {title:'Import Format',items:[{text:'CSV'},{text:'TSV'}]}
  ];

  const placeholderMigrationSwitching = [
    {title:'Getting Started', items:[{text:'Training provided at additional cost. All users provided with access to online support centre, which provides extensive system guidance and training videos. Regular online live webinars are also available.'}]},
    {title:'Service documentation', items:[{text:'Yes'}]},
    {title:'Documentation Format',items:[{text:'PDF'}, {text:'Markdown'}]},
    {title:'End-of-contract data extraction',items:[{text:'Data extraction provided to cutomers when contract period ends and the customer intends to move to another provider. '}]}
  ];

  const placeholderAuditInfo = [
    {title:'Access to user activity audit information', items:[{text:'Users have access to real-time audit information'}]},
    {title:'How long user audit data is stored for', items:[{text:'At least 12 months'}]},
    {title:'Access to supplier activity audit information', items:[{text:'Users have access to real-time audit information'}]}
  ];

  if(!context.productPage.optionals) {
    context.productPage.optionals = {};
  }

  (function TEMP_addAdditionalAndAssociated(placeholderServiceMap, productPage) {
    const keys = _.shuffle(Object.keys(placeholderServiceMap));
    const assoc = keys.pop()
    const addit = keys.pop()
    productPage.optionals['associated-services'] = {[assoc] : placeholderServiceMap[assoc]};
    productPage.optionals['additional-services'] = {[addit] : placeholderServiceMap[addit]};
  })(placeholderServiceMap, context.productPage)

  /**
   * is this map really needed? presumably the associated and additional services that
   * a solution provides will be in the DB, which will then just be retrieved no?
   *
   * Can then just render what is in there?
   */
  function mapServices(serviceType, serviceMap) {
    return _.map(serviceMap, (label, name) => {
      const path = ['optionals', serviceType, name]
      const chosen = _.has(context.productPage, path)
      const enabled = !!_.get(context.productPage, path)
      return {
        label,
        name: name,
        enabled: chosen && enabled,
        disabled: chosen && !enabled,
        price: `£${Math.floor(Math.random()*999).toFixed(2)}/day`
      }
    })
  }

  context.migrationSwitching = placeholderMigrationSwitching;
  context.userSupport = placeholderUserSupport;
  context.dataImportExport = placeholderDataImportExport;
  context.customerInsights = placeholderCustomerInsights;
  context.auditInfo = placeholderAuditInfo;
  

  context.services = {
    'associated-services': mapServices('associated-services', placeholderServiceMap),
    'additional-services': mapServices('additional-services', placeholderServiceMap)
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
