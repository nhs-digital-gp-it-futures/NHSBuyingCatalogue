/* eslint-env jest */

const { DataProvider } = require('../index')

module.exports = {
  mockApi: function () {
    function MockApi () {
    }

    function MockContactsApi () {
      this.apiContactsByEmailByEmailGet = jest.fn()
      this.apiContactsByOrganisationByOrganisationIdGet = jest.fn()
      this.apiClient = {}
    }

    function MockOrganisationsApi () {
      this.apiOrganisationsByContactByContactIdGet = jest.fn()
      this.apiClient = {}
    }

    function MockSolutionsApi () {
      this.apiSolutionsByOrganisationByOrganisationIdGet = jest.fn()
      this.apiClient = {}
    }

    function MockSolutionsExApi () {
      this.apiPorcelainSolutionsExBySolutionBySolutionIdGet = jest.fn()
      this.apiPorcelainSolutionsExUpdatePut = jest.fn()
      this.apiPorcelainSolutionsExByOrganisationByOrganisationIdGet = jest.fn()
      this.apiClient = {}
    }

    function MockCapabilityMappingsApi () {
      this.apiPorcelainCapabilityMappingsGet = jest.fn()
      this.apiClient = {}
    }

    function MockCapabilitiesImplementedEvidenceApi () {
      this.apiCapabilitiesImplementedEvidenceByClaimByClaimIdGet = jest.fn()
      this.apiClient = {}
    }

    return new DataProvider({
      ContactsApi: MockContactsApi,
      OrganisationsApi: MockOrganisationsApi,
      SolutionsApi: MockSolutionsApi,
      SolutionsExApi: MockSolutionsExApi,
      CapabilityMappingsApi: MockCapabilityMappingsApi,
      CapabilitiesImplementedEvidenceApi: MockCapabilitiesImplementedEvidenceApi
    })
  }
}
