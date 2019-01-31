/* eslint-env jest */

let subject

beforeEach(() => {
  subject = require('./fixtures').mockApi()
})

describe('solutionsForSupplierDashboard', () => {
  function mockResult (resultData) {
    subject.solutionsExApi.apiPorcelainSolutionsExByOrganisationByOrganisationIdGet.mockReturnValue(resultData)
  }

  it('returns empty arrays if there are no solutions', async () => {
    mockResult([])
    await expect(subject.solutionsForSupplierDashboard(12345)).resolves.toEqual({
      onboarding: [],
      live: []
    })
    expect(subject.solutionsExApi.apiPorcelainSolutionsExByOrganisationByOrganisationIdGet.mock.calls.length).toBe(1)
    expect(subject.solutionsExApi.apiPorcelainSolutionsExByOrganisationByOrganisationIdGet.mock.calls[0][0]).toBe(12345)
  })

  it('returns arrays with a Failed Solution if there is only a failed solution', async () => {
    mockResult([{
      solution: { id: 'failed', status: '-1' },
      standards: []
    }])
    const expectedWithOneFailedSolution = {
      'live': [],
      'onboarding': [
        {
          'displayName': 'undefined',
          'id': 'failed',
          'notifications': [],
          'raw': {
            'id': 'failed',
            'status': '-1'
          },
          'solution': {
            'id': 'failed',
            'status': '-1'
          },
          'stageName': 'Failure',
          'stageStep': 'Assessment Outcome',
          'standards': [],
          'status': 'Failed'
        }
      ]
    }
    await expect(subject.solutionsForSupplierDashboard(12345)).resolves.toEqual(expectedWithOneFailedSolution)
  })

  const testData = [
    { standards: [], solution: { id: 'failed', status: '-1', name: 'It Failed', version: 'f' }, claimedCapabilityEvidence: [] },
    { standards: [], solution: { id: 'draft', status: '0', name: 'A Draft' }, claimedCapabilityEvidence: [] },
    { standards: [], solution: { id: 'registered', status: '1', name: 'B Registered', version: '1.0r' }, claimedCapabilityEvidence: [] },
    { standards: [], solution: { id: 'assessment-draft', status: '1', name: 'C Assessment', version: '2.1a' }, claimedCapabilityEvidence: ['evidence'] },
    { standards: [], solution: { id: 'assessment', status: '2', name: 'D Assessment', version: '2.2a' }, claimedCapabilityEvidence: [] },
    { standards: [], solution: { id: 'compliance', status: '3', name: 'E Compliance', version: '3c' }, claimedCapabilityEvidence: [] },
    { standards: [], solution: { id: 'approval', status: '4', name: 'F Approval' }, claimedCapabilityEvidence: [] },
    { standards: [], solution: { id: 'solution', status: '5', name: 'G Solution Page' }, claimedCapabilityEvidence: [] },
    { standards: [], solution: { id: 'live', status: '6', name: 'H Live', version: 'LLL' }, claimedCapabilityEvidence: [] }
  ]

  it('separates live and currently onboarding solutions correctly', async () => {
    mockResult(testData)
    const result = await subject.solutionsForSupplierDashboard(12345)

    expect(result.onboarding.map(_ => _.id)).toEqual([
      'failed', 'draft', 'registered', 'assessment-draft', 'assessment', 'compliance', 'approval', 'solution'
    ])
    expect(result.live).toHaveLength(1)
    expect(result.live[0].id).toEqual('live')
  })

  it('Correctly maps the status and stage name for a registered solution with CapAss evidence', async () => {
    mockResult(testData)
    const result = await subject.solutionsForSupplierDashboard(12345)

    const solutionInDraftAssessment = result.onboarding.find(_ => _.id === 'assessment-draft')
    expect(solutionInDraftAssessment.status).toBe('Draft')
    expect(solutionInDraftAssessment.stageName).toBe('Assessment')
  })

  it('correctly formats the solution name and version for display on the dashboard', async () => {
    mockResult(testData)
    const result = await subject.solutionsForSupplierDashboard(12345)

    expect(result.onboarding.find(_ => _.id === 'draft').displayName).toEqual('A Draft')
    expect(result.onboarding.find(_ => _.id === 'compliance').displayName).toEqual('E Compliance | 3c')
    expect(result.live[0].displayName).toEqual('H Live | LLL')
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
      'FAILED', 'DRAFT', 'REGISTERED', 'ASSESSMENT-DRAFT', 'ASSESSMENT', 'COMPLIANCE', 'APPROVAL', 'SOLUTION'
    ])
    expect(result.live).toHaveLength(1)
    expect(result.live[0].copiedId).toEqual('LIVE')
  })
})
