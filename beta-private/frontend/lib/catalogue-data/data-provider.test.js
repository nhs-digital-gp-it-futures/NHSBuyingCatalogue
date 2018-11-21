/* eslint-env jest */

const { DataProvider } = require('./index')
let subject

beforeEach(() => {
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
    this.apiPorcelainSolutionsExUpdatePut = jest.fn()
  }

  function MockCapabilityMappingsApi () {
    this.apiPorcelainCapabilityMappingsGet = jest.fn()
  }

  subject = new DataProvider({
    ContactsApi: MockContactsApi,
    OrganisationsApi: MockOrganisationsApi,
    SolutionsApi: MockSolutionsApi,
    SolutionsExApi: MockSolutionsExApi,
    CapabilityMappingsApi: MockCapabilityMappingsApi
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
        { contactId: 'testCId3', contactType: '3rd type' },
        { contactId: 'testCId1', contactType: 'Lead Contact' },
        { contactId: 'testCId2', contactType: '2nd type' }
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
        { contactId: 'testCId1', contactType: 'Lead Contact' },
        { contactId: 'testCId2', contactType: '2nd type' },
        { contactId: 'testCId3', contactType: '3rd type' }
      ]
    })
  })
})

describe('updateSolutionForRegistration', () => {
  beforeEach(() => {
    subject.solutionsExApi.apiPorcelainSolutionsExBySolutionBySolutionIdGet.mockReturnValue({
      solution: {},
      claimedCapability: [
        { id: 'testCId1', capabilityId: 'CAP1' },
        { id: 'testCId2', capabilityId: 'CAP2' }
      ],
      claimedStandard: [
        { id: 'testSId1', standardId: 'STD1' },
        { id: 'testSId2', standardId: 'STD2' }
      ],
      technicalContact: [],
      claimedCapabilityEvidence: [
        { id: 'cce1', claimId: 'testCId1' },
        { id: 'cce2', claimId: 'testCId2' }
      ],
      claimedCapabilityReview: [
        { id: 'ccr1', evidenceId: 'cce1' },
        { id: 'ccr2', evidenceId: 'cce2' }
      ],
      claimedStandardEvidence: [
        { id: 'cse1', claimId: 'testSId1' },
        { id: 'cse2', claimId: 'testSId2' }
      ],
      claimedStandardReview: [
        { id: 'csr1', evidenceId: 'cse1' },
        { id: 'csr2', evidenceId: 'cse2' }
      ]
    })
  })

  it('should reformat input into a SolutionEx and send to API', async () => {
    const input = {
      id: 'testid',
      name: 'testname',
      capabilities: [
        { id: 'testCCId', capabilityId: 'C1' }
      ],
      standards: [
        { id: 'testCSId', standardId: 'S1' }
      ],
      contacts: [
        { id: '1234', contactId: 'testCId1', contactType: 'Lead Contact' },
        { id: '4567', contactId: 'testCId2', contactType: '2nd type' },
        { id: '8910', contactId: 'testCId3', contactType: '3rd type' }
      ]
    }

    await subject.updateSolutionForRegistration(input)
    expect(subject.solutionsExApi.apiPorcelainSolutionsExUpdatePut.mock.calls.length).toBe(1)
    expect(subject.solutionsExApi.apiPorcelainSolutionsExUpdatePut.mock.calls[0][0]).toMatchObject({
      solnEx: {
        solution: {
          id: 'testid',
          name: 'testname'
        },
        claimedCapability: [
          { id: 'testCCId', capabilityId: 'C1' }
        ],
        claimedStandard: [
          { id: 'testCSId', standardId: 'S1' }
        ],
        technicalContact: [
          { id: '1234', contactId: 'testCId1', contactType: 'Lead Contact' },
          { id: '4567', contactId: 'testCId2', contactType: '2nd type' },
          { id: '8910', contactId: 'testCId3', contactType: '3rd type' }
        ]
      }
    })
  })

  it('should set correct dummy IDs for new technical contact entitites', async () => {
    const input = {
      id: 'testid',
      name: 'testname',
      contacts: [
        { id: '1234', contactId: 'testCId1', contactType: 'Lead Contact' },
        { contactId: 'testCId2', contactType: '2nd type' },
        { id: '8910', contactId: 'testCId3', contactType: '3rd type' }
      ]
    }

    await subject.updateSolutionForRegistration(input)
    expect(subject.solutionsExApi.apiPorcelainSolutionsExUpdatePut.mock.calls.length).toBe(1)
    expect(subject.solutionsExApi.apiPorcelainSolutionsExUpdatePut.mock.calls[0][0]).toMatchObject({
      solnEx: {
        solution: {
          id: 'testid',
          name: 'testname'
        },
        technicalContact: [
          { id: '1234', contactId: 'testCId1', contactType: 'Lead Contact' },
          { id: expect.stringMatching(/^[0-9A-F]{8}-[0-9A-F]{4}-[4][0-9A-F]{3}-[89AB][0-9A-F]{3}-[0-9A-F]{12}$/i), contactId: 'testCId2', contactType: '2nd type' },
          { id: '8910', contactId: 'testCId3', contactType: '3rd type' }
        ]
      }
    })
  })

  it('should cascade delete evidence and reviews for removed capabilities and standards', async () => {
    const input = {
      id: 'testid',
      name: 'testname',
      capabilities: [
        { id: 'newcap', capabilityId: 'CAP6' },
        { id: 'testCId1', capabilityId: 'CAP1' }
      ],
      standards: [
        { id: 'testSId2', standardId: 'STD2' },
        { id: 'newstd', standardId: 'STD1' }
      ]
    }

    await subject.updateSolutionForRegistration(input)
    expect(subject.solutionsExApi.apiPorcelainSolutionsExUpdatePut.mock.calls.length).toBe(1)

    const result = subject.solutionsExApi.apiPorcelainSolutionsExUpdatePut.mock.calls[0][0]
    expect(result).toMatchObject({
      solnEx: {
        solution: {
          id: 'testid',
          name: 'testname'
        },
        claimedCapability: [
          { id: 'newcap', capabilityId: 'CAP6' },
          { id: 'testCId1', capabilityId: 'CAP1' }
        ],
        claimedStandard: [
          { id: 'testSId2', standardId: 'STD2' },
          { id: 'newstd', standardId: 'STD1' }
        ],
        claimedCapabilityEvidence: [
          { id: 'cce1', claimId: 'testCId1' }
        ],
        claimedCapabilityReview: [
          { id: 'ccr1', evidenceId: 'cce1' }
        ],
        claimedStandardEvidence: [
          { id: 'cse2', claimId: 'testSId2' }
        ],
        claimedStandardReview: [
          { id: 'csr2', evidenceId: 'cse2' }
        ]
      }
    })
    expect(result.solnEx.claimedCapabilityEvidence).toHaveLength(1)
    expect(result.solnEx.claimedCapabilityReview).toHaveLength(1)
    expect(result.solnEx.claimedStandardEvidence).toHaveLength(1)
    expect(result.solnEx.claimedStandardReview).toHaveLength(1)
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

describe('validateSolutionUniqueness', () => {
  beforeEach(() => {
    subject.solutionsApi.apiSolutionsByOrganisationByOrganisationIdGet.mockReturnValue({
      items: [
        { id: 'id1', name: 'This is a duplicate', version: '1' },
        { id: 'id2', name: 'This is not a duplicate', version: '2' }
      ]
    })
  })

  it('correctly detects duplicate name and version for new solutions', async () => {
    let result = await subject.validateSolutionUniqueness({
      name: 'This is a duplicate',
      version: '1',
      organisationId: 'orgId'
    })
    expect(result).toBeFalsy()

    result = await subject.validateSolutionUniqueness({
      name: 'This is not a duplicate',
      version: '1',
      organisationId: 'orgId'
    })
    expect(result).toBeTruthy()

    result = await subject.validateSolutionUniqueness({
      name: 'This is not a duplicate',
      version: '2',
      organisationId: 'orgId'
    })
    expect(result).toBeFalsy()

    result = await subject.validateSolutionUniqueness({
      name: 'This is a duplicate',
      version: '2',
      organisationId: 'orgId'
    })
    expect(result).toBeTruthy()
  })

  it('correctly detects duplicate name and version for existing solutions', async () => {
    let result = await subject.validateSolutionUniqueness({
      id: 'id1',
      name: 'This is a duplicate',
      version: '1',
      organisationId: 'orgId'
    })
    expect(result).toBeTruthy()

    result = await subject.validateSolutionUniqueness({
      id: 'id1',
      name: 'This is not a duplicate',
      version: '1',
      organisationId: 'orgId'
    })
    expect(result).toBeTruthy()

    result = await subject.validateSolutionUniqueness({
      id: 'id1',
      name: 'This is not a duplicate',
      version: '2',
      organisationId: 'orgId'
    })
    expect(result).toBeFalsy()
  })
})

describe('capabilityMappings', () => {
  it('correctly maps the incoming static data', async () => {
    subject.capabilityMappingsApi.apiPorcelainCapabilityMappingsGet.mockReturnValue(
      require('./data-provider.test-data')
    )

    const result = await subject.capabilityMappings()

    expect(Object.keys(result.capabilities)).toHaveLength(18)
    expect(result.capabilities['CAP1'].name).toBe('Appointments Management - Citizen')
    expect(result.capabilities['CAP1'].standards).toHaveLength(12)

    expect(Object.keys(result.standards)).toHaveLength(50)
    expect(result.standards['OS1'].name).toBe('Business Continuity / Disaster Recovery')
    expect(result.standards['OS1'].isOverarching).toBeTruthy()
  })
})
