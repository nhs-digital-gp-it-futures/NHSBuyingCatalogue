/* eslint-env jest */

let subject

beforeEach(() => {
  subject = require('./fixtures').mockApi()
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

  it('returns arrays with a Failed Solution if there is only a failed solution', async () => {
    mockResult({
      items: [
        { id: 'failed', status: '-1' }
      ]
    })
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
          'stageName': 'Failure',
          'status': 'Failed'
        }
      ]
    }
    await expect(subject.solutionsForSupplierDashboard(12345)).resolves.toEqual(expectedWithOneFailedSolution )
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
      'failed', 'draft', 'registered', 'assessment', 'compliance', 'approval', 'solution'
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
      'FAILED', 'DRAFT', 'REGISTERED', 'ASSESSMENT', 'COMPLIANCE', 'APPROVAL', 'SOLUTION'
    ])
    expect(result.live).toHaveLength(1)
    expect(result.live[0].copiedId).toEqual('LIVE')
  })
})
