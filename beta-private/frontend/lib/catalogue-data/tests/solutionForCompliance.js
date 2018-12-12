/* eslint-env jest */

let subject

beforeEach(() => {
  subject = require('./fixtures').mockApi()
})

describe('solutionForCompliance', () => {
  it('adds the extra properties to the SolutionEx standards correctly', async () => {
    subject.solutionsExApi.apiPorcelainSolutionsExBySolutionBySolutionIdGet.mockReturnValue({
      solution: {
        id: 'testid',
        name: 'testname'
      },
      claimedCapability: [
        { claimedCapabilityId: 'testCCId' }
      ],
      claimedStandard: [
        { status: 0, claimedStandardId: 'testCSId' },
        { status: 1, claimedStandardId: 'testCSId1' },
        { status: 2, claimedStandardId: 'testCSId2' },
        { status: 3, claimedStandardId: 'testCSId3' },
        { status: 4, claimedStandardId: 'testCSId4' },
        { status: 7, claimedStandardId: 'testCSId7' }
      ],
      technicalContact: [
        { contactId: 'testCId3', contactType: '3rd type', firstName: 'Helpma', lastName: 'Boab' },
        { contactId: 'testCId1', contactType: 'Lead Contact', firstName: '🦄', lastName: 'Rainbow' },
        { contactId: 'testCId2', contactType: '2nd type' }
      ]
    })

    await expect(subject.solutionForCompliance('testId')).resolves.toMatchObject({
      id: 'testid',
      name: 'testname',
      capabilities: [
        { claimedCapabilityId: 'testCCId' }
      ],
      standards: [
        {
          status: 0,
          claimedStandardId: 'testCSId',
          statusClass: 'not-started',
          ownerContact: { displayName: '🦄 Rainbow' }
        },
        {
          status: 1,
          claimedStandardId: 'testCSId1',
          statusClass: 'draft',
          ownerContact: { displayName: '🦄 Rainbow' }
        },
        {
          status: 2,
          claimedStandardId: 'testCSId2',
          statusClass: 'submitted',
          ownerContact: { displayName: '🦄 Rainbow' }
        },
        {
          status: 3,
          claimedStandardId: 'testCSId',
          statusClass: 'remediation',
          ownerContact: { displayName: '🦄 Rainbow' }
        },
        {
          status: 4,
          claimedStandardId: 'testCSId4',
          statusClass: 'approved',
          ownerContact: { displayName: '🦄 Rainbow' }
        },
        {
          status: 7,
          claimedStandardId: 'testCSId7',
          statusClass: 'rejected',
          ownerContact: { displayName: '🦄 Rainbow' }
        }
      ],
      contacts: [
        { contactId: 'testCId1', contactType: 'Lead Contact' },
        { contactId: 'testCId2', contactType: '2nd type' },
        { contactId: 'testCId3', contactType: '3rd type' }
      ]
    })
  })
})
