const _ = require('lodash')
const fetch = require('node-fetch')
const https = require('https')

// create a custom HTTPS agent to deal with self-signed certificate at the API
const agent = new https.Agent({
  rejectUnauthorized: false
})

const {
  API_BASE_URL = 'https://localhost:8001/api/'
} = process.env

const defaultHeaders = {
}

function api_endpoint (endpoint) {
  return API_BASE_URL + endpoint
}

function api (endpoint, options = {}) {
  const opts = {
    agent,
    ...options,
    headers: {
      ...defaultHeaders,
      ...options.headers
    }
  }

  return fetch(
    api_endpoint(endpoint),
    opts
  ).then(response => {
    if (response.ok) {
      return response.json()
        .catch(() => undefined)
    }

    return response.text()
      .then(err => {
        const error = new Error(`API ${endpoint} returned error ${response.status}: ${err}`)
        error.status = response.status
        return Promise.reject(error)
      })
  })
}

function get_api (endpoint) {
  return api(endpoint)
}

function put_api (endpoint, payload) {
  return api(endpoint, {
    method: 'PUT',
    body: JSON.stringify(payload),
    headers: {
      'Content-Type': 'application/json'
    }
  })
}

function post_api (endpoint, payload) {
  return api(endpoint, {
    method: 'POST',
    body: JSON.stringify(payload),
    headers: {
      'Content-Type': 'application/json'
    }
  })
}

function delete_api (endpoint, payload) {
  return api(endpoint, {
    method: 'DELETE',
    body: JSON.stringify(payload),
    headers: {
      'Content-Type': 'application/json'
    }
  })
}

function decodeSolutionEx (solutionEx) {
  // decode the product page JSON prior to returning to client
  try {
    solutionEx.solution.productPage = JSON.parse(solutionEx.solution.productPage)
  } catch (err) {
    solutionEx.solution.productPage = {}
  }

  return solutionEx
}

class API {
  set_authorisation (auth_header_value) {
    defaultHeaders.Authorization = auth_header_value
  }

  async get_contact_for_user (user) {
    return get_api(`Contact/ByEmail/${user.email}`)
  }

  async get_org_by_id (orgId) {
    return get_api(`Organisation/ById/${orgId}`)
  }

  async get_org_for_user (user) {
    const contact = await this.get_contact_for_user(user)
    const org = contact
              ? await this.get_org_by_id(contact.organisationId)
              : {}
    org.isSupplier = org.primaryRoleId === 'RO92'
    org.isNHSDigital = org.primaryRoleId === 'RO126'
    return { org, contact }
  }

  async get_solutions_for_user (user) {
    const { org } = await this.get_org_for_user(user)
    const paged = await get_api(`Solution/ByOrganisation/${org.id}?pageSize=100`)
    return paged ? paged.items : []
  }

  async get_solution_by_id (solutionId) {
    return decodeSolutionEx(await get_api(`porcelain/SolutionEx/BySolution/${solutionId}`))
  }

  async update_solution (solutionEx) {
    // fill in id and solutionId fields for embedded entities
    const defaults = {
      id: '00000000-0000-0000-0000-000000000000',
      solutionId: solutionEx.solution.id
    }
    _.each(solutionEx.claimedCapability, e => _.defaults(e, defaults))
    _.each(solutionEx.claimedStandard, e => _.defaults(e, defaults))
    _.each(solutionEx.technicalContact, e => _.defaults(e, defaults))

    // encode product page before update
    solutionEx.solution.productPage = JSON.stringify(solutionEx.solution.productPage)

    await put_api('porcelain/SolutionEx/Update', solutionEx)

    return decodeSolutionEx(solutionEx)
  }

  async create_solution_for_user (solution, user) {
    const contact = await this.get_contact_for_user(user)
    solution.id = '00000000-0000-0000-0000-000000000000'
    solution.organisationId = contact.organisationId
    return post_api('Solution/Create', solution)
  }

  async get_all_capabilities () {
    const mappings = await get_api('porcelain/CapabilityMappings')
    const standards = _.keyBy(mappings.standard, 'id')

    function groupStandards (standards) {
      return _.groupBy(
        _.sortBy(standards, 'name'),
        std => _.startsWith(std.standardId || std.id, 'CSS')
          ? ((std.isOptional || std.id === 'CSS3') ? 'optional' : 'mandatory')
          : _.startsWith(std.standardId || std.id, 'INT')
            ? 'interop' : 'overarching'
      )
    }

    const capabilityTypes = {
      CAP1: 'citizen',
      CAP2: 'core',
      CAP3: 'practice',
      CAP4: 'citizen',
      CAP5: 'practice',
      CAP6: 'core',
      CAP7: 'practice',
      CAP8: 'core',
      CAP9: 'core',
      CAP10: 'core',
      CAP11: 'core',
      CAP12: 'citizen',
      CAP13: 'core',
      CAP14: 'practice',
      CAP15: 'practice',
      CAP16: 'practice',
      CAP17: 'practice',
      CAP18: 'citizen',
      CAP19: 'practice'
    }

    for (const mapping of mappings.capabilityMapping) {
      // for each capability there are potentially three classes of standard
      // associated; mandatory context-specific, interoperability and optional context-specific
      // overarching standards are ignored
      // HACK until the API can return this data, the ID of each standard is inspected
      //      to classify it
      mapping.capability.standards = groupStandards(mapping.optionalStandard)

      // merge the optional standards next to each capability into the capability
      mapping.capability.standards = _.mapValues(
        mapping.capability.standards,
        stds => _.map(stds, std => standards[std.standardId])
      )

      // HACK hard-coded capability types for now
      mapping.capability.types = capabilityTypes[mapping.capability.id]
    }

    return {
      capabilities: _.sortBy(
        _.map(mappings.capabilityMapping, 'capability'),
        cap => cap.name.toLowerCase()
      ),
      standards: mappings.standard,
      groupedStandards: groupStandards(mappings.standard)
    }
  }

  async get_solution_capabilities (solutionId) {
    try {
      const claimed = await get_api(`ClaimedCapability/BySolution/${solutionId}?pageSize=100`)
      return claimed.items || []
    } catch (err) {
      return []
    }
  }

  async set_solution_capabilities (solution, capabilities) {
    const solutionId = solution.id

    // as there's no evidence for now, delete all the current capabilities
    // and replace them
    const current = await this.get_solution_capabilities(solutionId)
    for (let cap of current) {
      await delete_api('ClaimedCapability/Delete', cap).catch(() => true)
    }

    for (let cap of capabilities) {
      cap.id = '00000000-0000-0000-0000-000000000000'
      cap.solutionId = solutionId
      cap.evidence = ''
      await post_api('ClaimedCapability/Create', cap)
    }

    return true
  }

  async get_solution_standards (solutionId) {
    try {
      const claimed = await get_api(`ClaimedStandard/BySolution/${solutionId}?pageSize=100`)
      return claimed.items || []
    } catch (err) {
      return []
    }
  }

  async set_solution_standards (solution, standards) {
    const solutionId = solution.id

    // as there's no evidence for now, delete all the current standards
    // and replace them
    const current = await this.get_solution_standards(solutionId)
    for (let std of current) {
      await delete_api('ClaimedStandard/Delete', std).catch(() => true)
    }

    for (let std of standards) {
      std.id = '00000000-0000-0000-0000-000000000000'
      std.solutionId = solutionId
      std.evidence = ''
      await post_api('ClaimedStandard/Create', std)
    }

    return true
  }

  async get_solutions_for_assessment () {
    // very inefficient but as this will be handled by CRM there's no value in
    // building an API to do this properly
    const allOrgs = await get_api('Organisation?pageSize=100')
    const hasQualifyingStatus = item =>
      item.status === this.SOLUTION_STATUS.CAPABILITIES_ASSESSMENT ||
      item.status === this.SOLUTION_STATUS.STANDARDS_COMPLIANCE ||
      item.status === this.SOLUTION_STATUS.SOLUTION_PAGE

    return Promise.all(
      _.map(allOrgs.items, org =>
        get_api(`Solution/ByOrganisation/${org.id}?pageSize=100`)
          .then(solns => _.filter(solns.items, hasQualifyingStatus))
          .then(solns => _.map(solns, soln => ({
            ...soln,
            orgName: org.name
          })))
          .catch(() => [])
      )
    ).then(allSolns => _.flatten(allSolns))
  }

  async get_assessment_messages_for_solution (solutionId) {
    return get_api(`AssessmentMessage/BySolution/${solutionId}?pageSize=100`)
      .then(result => result.items)
      .catch(() => [])
  }

  async post_assessment_message (message) {
    message.id = '00000000-0000-0000-0000-000000000000'
    return post_api('AssessmentMessage/Create', message)
  }

  async get_contacts_for_org (orgId) {
    return get_api(`Contact/ByOrganisation/${orgId}?pageSize=100`)
      .then(result => result.items)
      .catch(() => [])
  }

  async get_contact_by_id (contactId) {
    return get_api(`Contact/ById/${contactId}`)
  }

  async get_supplier_orgs () {
    return get_api(`Organisation?pageSize=100`)
      .then(result => result.items)
      .then(orgs => orgs.filter(org => org.primaryRoleId === 'RO92'))
  }

  async create_supplier_org (supplier) {
    return post_api(`Organisation/Create`, {
      ...supplier,
      id: '00000000-0000-0000-0000-000000000000',
      primaryRoleId: 'RO92',
      status: 'Active',
      description: ''
    })
  }

  async create_contact (contact) {
    return post_api(`Contact/Create`, {
      ...contact,
      id: '00000000-0000-0000-0000-000000000000'
    })
  }

  async get_capability_assessment_questions () {
    return {
      // Appointments Management - Citizen
      'CAP1': [
        'Share a video demonstrating that the solution:',
        'Lorem ipsum dolor sit amet, consectetur adipisicing elit',
        'sed do eiusmod tempor incididunt ut labore et dolore magna aliqua'
      ],

      // Appointments Management - GP
      'CAP2': [
        'Share a video demonstrating that the solution allows users to:',
        'Lorem ipsum dolor sit amet, consectetur adipisicing elit',
        'sed do eiusmod tempor incididunt ut labore et dolore magna aliqua'
      ],

      // Clinical Decision Support
      'CAP3': [
        'Share a video demonstrating that the solution allows users to:',
        'Lorem ipsum dolor sit amet, consectetur adipisicing elit',
        'sed do eiusmod tempor incididunt ut labore et dolore magna aliqua'
      ],

      // Communicate With Practice - Citizen
      'CAP4': [
        'Share a video demonstrating that the solution:',
        'Lorem ipsum dolor sit amet, consectetur adipisicing elit',
        'sed do eiusmod tempor incididunt ut labore et dolore magna aliqua'
      ],

      // Digital Diagnostics
      'CAP5': [
        'Share a video demonstrating that the solution allows clinicians to:',
        'Lorem ipsum dolor sit amet, consectetur adipisicing elit',
        'sed do eiusmod tempor incididunt ut labore et dolore magna aliqua'
      ],

      // Document Management
      'CAP6': [
        'Share a video demonstrating that the solution allows users to:',
        'Lorem ipsum dolor sit amet, consectetur adipisicing elit',
        'sed do eiusmod tempor incididunt ut labore et dolore magna aliqua'
      ],

      // GP Extracts Verification
      'CAP7': [
        'Share a video demonstrating that the solution:',
        'Lorem ipsum dolor sit amet, consectetur adipisicing elit',
        'sed do eiusmod tempor incididunt ut labore et dolore magna aliqua'
      ],

      // GP Referral Management
      'CAP8': [
        'Share a video demonstrating that the solution:',
        'Lorem ipsum dolor sit amet, consectetur adipisicing elit',
        'sed do eiusmod tempor incididunt ut labore et dolore magna aliqua'
      ],

      // GP Resource Management
      'CAP9': [
        'Share a video demonstrating that the solution allows users to:',
        'Lorem ipsum dolor sit amet, consectetur adipisicing elit',
        'sed do eiusmod tempor incididunt ut labore et dolore magna aliqua'
      ],

      // Patient Information Management
      'CAP10': [
        'Share a video demonstrating that the solution allows users to:',
        'Lorem ipsum dolor sit amet, consectetur adipisicing elit',
        'sed do eiusmod tempor incididunt ut labore et dolore magna aliqua'
      ],

      // Prescribing
      'CAP11': [
        'Share a video demonstrating that the solution:',
        'Lorem ipsum dolor sit amet, consectetur adipisicing elit',
        'sed do eiusmod tempor incididunt ut labore et dolore magna aliqua'
      ],

      // Prescription Ordering - Citizen
      'CAP12': [
        'Share a video demonstrating that the solution allows GPS\'s to view, request, amend and cancel:',
        'Lorem ipsum dolor sit amet, consectetur adipisicing elit',
        'sed do eiusmod tempor incididunt ut labore et dolore magna aliqua'
      ],

      // Recording Consultations
      'CAP13': [
        'Share a video demonstrating that the form:',
        'Lorem ipsum dolor sit amet, consectetur adipisicing elit',
        'sed do eiusmod tempor incididunt ut labore et dolore magna aliqua'
      ],

      // Reporting
      'CAP14': [
        'Share a video demonstrating that the solution:',
        'Lorem ipsum dolor sit amet, consectetur adipisicing elit',
        'sed do eiusmod tempor incididunt ut labore et dolore magna aliqua'
      ],

      // Scanning
      'CAP15': [
        'Share a video demonstrating that the solution allows users to:',
        'Lorem ipsum dolor sit amet, consectetur adipisicing elit',
        'sed do eiusmod tempor incididunt ut labore et dolore magna aliqua'
      ],

      // Telecare
      'CAP16': [
        'Share a video demonstrating that the solution:',
        'Lorem ipsum dolor sit amet, consectetur adipisicing elit',
        'sed do eiusmod tempor incididunt ut labore et dolore magna aliqua'
      ],

      // Unstructured Data Extraction
      'CAP17': [
        'Share a video demonstrating that the solution:',
        'Lorem ipsum dolor sit amet, consectetur adipisicing elit',
        'sed do eiusmod tempor incididunt ut labore et dolore magna aliqua'
      ],

      // View Record - Citizen
      'CAP18': [
        'Share a video demonstrating that the solution allows patients to:',
        'Lorem ipsum dolor sit amet, consectetur adipisicing elit',
        'sed do eiusmod tempor incididunt ut labore et dolore magna aliqua'
      ],

      // Workflow
      'CAP19': [
        'Share a video demonstrating that the solution:',
        'Lorem ipsum dolor sit amet, consectetur adipisicing elit',
        'sed do eiusmod tempor incididunt ut labore et dolore magna aliqua'
      ]
    }
  }
}

API.prototype.solutionStatuses = [
  'Draft', 'Registered', 'Capabilities Assessment', 'Standards Compliance',
  'Product Page', 'Approved'
]

API.prototype.SOLUTION_STATUS = {
  DRAFT: 0,
  REGISTERED: 1,
  CAPABILITIES_ASSESSMENT: 2,
  STANDARDS_COMPLIANCE: 3,
  SOLUTION_PAGE: 4,
  APPROVED: 5
}

API.prototype.capabilityStatuses = [
  'Submitted', 'Remediation', 'Approved', 'Rejected'
]

API.prototype.CAPABILITY_STATUS = {
  SUBMITTED: 0,
  REMEDIATION: 1,
  APPROVED: 2,
  REJECTED: 3
}

API.prototype.stageIndicators = [
  '1 of 4', '2 of 4', '2 of 4', '3 of 4', '4 of 4', 'Complete',
];

API.prototype.solutionStages = [
  'Registration', 'Capabilities Assessment', 'Capabilities Review', 'Standards Compliance',
  'Solution Page', 'Approved'
]


API.prototype.standardStatuses = [
  'Submitted', 'Remediation', 'Approved', 'Rejected', 'Partially Approved'
]

API.prototype.STANDARD_STATUS = {
  SUBMITTED: 0,
  REMEDIATION: 1,
  APPROVED: 2,
  REJECTED: 3,
  PARTIALLY_APPROVED: 4
}

API.prototype.NHS_DIGITAL_ORG_ID = 'D6BC9186-5C8E-4638-A7B8-6F7CE7BC6946'

module.exports = new API()
