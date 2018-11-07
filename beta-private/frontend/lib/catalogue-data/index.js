const _ = require('lodash')

const solutionOnboardingStatusMap = {
  0: { stageName: 'Registration', stageStep: '1 of 4', status: 'Draft' },
  1: { stageName: 'Registration', stageStep: '1 of 4', status: 'Registered' },
  2: { stageName: 'Assessment', stageStep: '2 of 4', status: 'Submitted' },
  3: { stageName: 'Compliance', stageStep: '3 of 4', status: 'In progress' },
  4: { stageName: 'Final Approval', stageStep: '3 of 4', status: 'In progress' },
  5: { stageName: 'Solution Page', stageStep: '4 of 4', status: 'In progress' }
}

class DataProvider {
  constructor (CatalogueApi) {
    this.CatalogueApi = CatalogueApi
    this.contactsApi = new this.CatalogueApi.ContactsApi()
    this.orgsApi = new CatalogueApi.OrganisationsApi()
    this.solutionsApi = new CatalogueApi.SolutionsApi()
    this.solutionsExApi = new CatalogueApi.SolutionsExApi()
    this.capabilityMappingsApi = new CatalogueApi.CapabilityMappingsApi()
  }

  async contactByEmail (email) {
    const contact = await this.contactsApi.apiContactsByEmailByEmailGet(email)
    if (!contact) throw new Error(`No contact found`)

    const org = await this.orgsApi.apiOrganisationsByContactByContactIdGet(contact.id)
    if (!org) throw new Error(`No organisation found for contact`)

    // identify supplier organisations
    org.isSupplier = org.primaryRoleId === 'RO92'

    return { contact, org }
  }

  async solutionsForSupplierDashboard (supplierOrgId, solutionMapper = x => x) {
    const isLive = (soln) => +soln.status === 6 /* Solutions.StatusEnum.Approved */
    const isOnboarding = (soln) => +soln.status !== 6 /* Solutions.StatusEnum.Approved */ &&
      +soln.status !== -1 /* Solutions.StatusEnum.Failed */

    const forDashboard = (soln) => ({
      raw: soln,
      id: soln.id,
      displayName: `${soln.name}${soln.version ? ` | ${soln.version}` : ''}`,
      notifications: []
    })

    const forOnboarding = (soln) => ({
      ...soln,
      ...solutionOnboardingStatusMap[+soln.raw.status]
    })

    const forLive = (soln) => ({
      ...soln,
      status: 'Accepting call-offs',
      contractCount: 0
    })

    const paginatedSolutions = await this.solutionsApi.apiSolutionsByOrganisationByOrganisationIdGet(
      supplierOrgId,
      { pageSize: 9999 }
    )

    return {
      onboarding: paginatedSolutions.items.filter(isOnboarding).map(forDashboard).map(forOnboarding).map(solutionMapper),
      live: paginatedSolutions.items.filter(isLive).map(forDashboard).map(forLive).map(solutionMapper)
    }
  }

  async solutionForRegistration (solutionId) {
    const solutionEx = await this.solutionsExApi.apiPorcelainSolutionsExBySolutionBySolutionIdGet(solutionId)

    // reformat the returned value for ease-of-use
    return {
      ...solutionEx.solution,
      capabilities: solutionEx.claimedCapability,
      standards: solutionEx.claimedStandard,
      contacts: _.orderBy(solutionEx.technicalContact, c => {
        // Lead Contact sorts above all others, then alphabetic by type
        return c.contactType === 'Lead Contact' ? '' : c.contactType
      })
    }
  }

  async updateSolutionForRegistration (solution) {
    const solnEx = await this.solutionsExApi.apiPorcelainSolutionsExBySolutionBySolutionIdGet(solution.id)

    // reformat the input back into a SolutionEx
    _.merge(solnEx.solution, _.omit(solution, ['capabilities', 'standards', 'contacts']))
    solnEx.claimedCapability = solution.capabilities
    solnEx.claimedStandard = solution.standards
    solnEx.technicalContact = solution.contacts

    // contacts can only be for this solution
    _.each(solnEx.technicalContact, c => { c.solutionId = solnEx.solution.id })

    await this.solutionsExApi.apiPorcelainSolutionsExUpdatePut({ solnEx })
    return this.solutionForRegistration(solution.id)
  }

  async capabilityMappings () {
    const {
      capabilityMapping,
      standard
    } = await this.capabilityMappingsApi.apiPorcelainCapabilityMappingsGet()

    return {
      capabilities: _(capabilityMapping)
        .map(({ capability, optionalStandard }) => ({
          ...capability,
          standards: optionalStandard
        }))
        .keyBy('id')
        .value(),
      standards: _.keyBy(standard, 'id')
    }
  }
}

// export a default, pre-configured instance of the data provider
// as well as the constructor to allow for using testing with a mock API class
class RealDataProvider extends DataProvider {
  constructor () {
    super(require('catalogue-api'))
    this.CatalogueApi.ApiClient.instance.basePath = 'http://api:5100'
  }

  // support for the authentication layer
  setAuthenticationToken (token) {
    this.CatalogueApi.ApiClient.instance.authentications.oauth2.accessToken = token
  }
}

module.exports = {
  dataProvider: new RealDataProvider(),
  DataProvider
}
