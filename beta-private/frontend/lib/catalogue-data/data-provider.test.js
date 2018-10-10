/* eslint-env jest */

const { DataProvider } = require('./index')
let subject

beforeAll(() => {
  function MockApi () {
  }

  function MockContactsApi () {
    this.apiContactsByEmailByEmailGet = jest.fn()
  }

  function MockOrganisationsApi () {
    this.apiOrganisationsByContactByContactIdGet = jest.fn()
  }

  function MockSolutionsApi () {
    this.apiSolutionsByOrganisationByOrganisationIdGet = jest.fn()
  }

  function MockSolutionsExApi () {
    this.apiPorcelainSolutionsExBySolutionBySolutionIdGet = jest.fn()
  }

  subject = new DataProvider({
    ContactsApi: MockContactsApi,
    OrganisationsApi: MockOrganisationsApi,
    SolutionsApi: MockSolutionsApi,
    SolutionsExApi: MockSolutionsExApi
  })
})

describe('contactByEmail', () => {
  it('throws if no contact exists with the specified email address', async () => {
    await expect(subject.contactByEmail('bogus')).rejects.toThrowError('No contact found')
    expect(subject.contactsApi.apiContactsByEmailByEmailGet.mock.calls.length).toBe(1)
  })

  it('throws if no organisation can be found for the contact', async () => {
    subject.contactsApi.apiContactsByEmailByEmailGet.mockReturnValue({
      id: 'bogus'
    })
    await expect(subject.contactByEmail('supplier@test.com')).rejects.toThrowError('No organisation found')
    expect(subject.orgsApi.apiOrganisationsByContactByContactIdGet.mock.calls.length).toBe(1)
  })

  it('returns the contact and organisation with supplier flag set correctly', async () => {
    const testContact = {
      id: 'testContact',
      email: 'supplier@test'
    }

    const testSupplierOrg = {
      id: 'testSupplier',
      primaryRoleId: 'RO92'
    }

    const testNonSupplierOrg = {
      id: 'testNonSupplier'
    }

    subject.contactsApi.apiContactsByEmailByEmailGet.mockReturnValue(testContact)
    subject.orgsApi.apiOrganisationsByContactByContactIdGet
      .mockReturnValueOnce(testSupplierOrg)
      .mockReturnValue(testNonSupplierOrg)

    await expect(subject.contactByEmail('supplier@test')).resolves.toMatchObject({
      contact: {
        id: 'testContact',
        email: 'supplier@test'
      },
      org: {
        id: 'testSupplier',
        isSupplier: true
      }
    })

    await expect(subject.contactByEmail('non-supplier@test')).resolves.toMatchObject({
      contact: {
        id: 'testContact',
        email: 'supplier@test'
      },
      org: {
        id: 'testNonSupplier',
        isSupplier: false
      }
    })
  })
})

describe('solutionForRegistration', () => {
  // I've elided the unhappy path test here as whatever is thrown is decided
  // by the API layer - generally an Error('Not Found')

  it('transforms the returned SolutionEx correctly', async () => {
    subject.solutionsExApi.apiPorcelainSolutionsExBySolutionBySolutionIdGet.mockReturnValue({
      solution: {
        id: 'testid',
        name: 'testname'
      },
      claimedCapability: [
        { claimedCapabilityId: 'testCCId' }
      ],
      claimedStandard: [
        { claimedStandardId: 'testCSId' }
      ],
      technicalContact: [
        { contactId: 'testCId' }
      ]
    })

    await expect(subject.solutionForRegistration('testId')).resolves.toMatchObject({
      id: 'testid',
      name: 'testname',
      capabilities: [
        { claimedCapabilityId: 'testCCId' }
      ],
      standards: [
        { claimedStandardId: 'testCSId' }
      ],
      contacts: [
        { contactId: 'testCId' }
      ]
    })
  })
})

describe('solutionsForSupplierDashboard', () => {
  function mockResult (resultData) {
    subject.solutionsApi.apiSolutionsByOrganisationByOrganisationIdGet.mockReturnValue(resultData)
  }

  it('returns empty arrays if there are no solutions', async () => {
    mockResult({ items: [] })
    await expect(subject.solutionsForSupplierDashboard(12345)).resolves.toEqual({
      onboarding: [],
      live: []
    })
    expect(subject.solutionsApi.apiSolutionsByOrganisationByOrganisationIdGet.mock.calls.length).toBe(1)
    expect(subject.solutionsApi.apiSolutionsByOrganisationByOrganisationIdGet.mock.calls[0][0]).toBe(12345)
  })

  it('returns empty arrays if there are no applicable solutions', async () => {
    mockResult({
      items: [
        { id: 'failed', status: '-1' }
      ]
    })
    await expect(subject.solutionsForSupplierDashboard(12345)).resolves.toEqual({
      onboarding: [],
      live: []
    })
  })

  const testData = {
    items: [
      { id: 'failed', status: '-1', name: 'It Failed', version: 'f' },
      { id: 'draft', status: '0', name: 'A Draft' },
      { id: 'registered', status: '1', name: 'B Registered', version: '1.0r' },
      { id: 'assessment', status: '2', name: 'C Assessment', version: '2.2a' },
      { id: 'compliance', status: '3', name: 'D Compliance', version: '3c' },
      { id: 'approval', status: '4', name: 'E Approval' },
      { id: 'solution', status: '5', name: 'F Solution Page' },
      { id: 'live', status: '6', name: 'G Live', version: 'LLL' }
    ]
  }

  it('separates live and currently onboarding solutions correctly', async () => {
    mockResult(testData)
    const result = await subject.solutionsForSupplierDashboard(12345)

    expect(result.onboarding.map(_ => _.id)).toEqual([
      'draft', 'registered', 'assessment', 'compliance', 'approval', 'solution'
    ])
    expect(result.live).toHaveLength(1)
    expect(result.live[0].id).toEqual('live')
  })

  it('correctly formats the solution name and version for display on the dashboard', async () => {
    mockResult(testData)
    const result = await subject.solutionsForSupplierDashboard(12345)

    expect(result.onboarding.find(_ => _.id === 'draft').displayName).toEqual('A Draft')
    expect(result.onboarding.find(_ => _.id === 'compliance').displayName).toEqual('D Compliance | 3c')
    expect(result.live[0].displayName).toEqual('G Live | LLL')
  })

  it.skip('correctly describes the properties of onboarding solutions', async () => {
    // skipped as the requirements are not well-defined enough
  })

  it.skip('correctly describes the properties of live solutions', async () => {
    // skipped as the requirements are not well-defined enough
  })

  it('correctly applies a supplier mapping function to the results', async () => {
    const testMapper = (soln) => ({
      ...soln,
      copiedId: soln.id.toUpperCase()
    })
    mockResult(testData)
    const result = await subject.solutionsForSupplierDashboard(12345, testMapper)

    expect(result.onboarding.map(_ => _.copiedId)).toEqual([
      'DRAFT', 'REGISTERED', 'ASSESSMENT', 'COMPLIANCE', 'APPROVAL', 'SOLUTION'
    ])
    expect(result.live).toHaveLength(1)
    expect(result.live[0].copiedId).toEqual('LIVE')
  })
})
