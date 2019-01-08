/* global fixture, test */

import { Selector } from 'testcafe'
import axeCheck from 'axe-testcafe'

import { asSupplier } from './roles'
import { supplierDashboardPage, onboardingDashboardPage, standardsComplianceDashboardPage } from './pages'

const expectedStatusStandards = {
  'Failed': new Set([
    '0e55d9ec-43e6-41b3-bcac-d8681384ea68'
  ]),
  'Draft': new Set([
    '7b62b29a-62a7-4b4a-bcc8-dfa65fb7e35c'
  ]),
  'Not started': new Set([
    'ca874860-4593-4541-8586-038201945d17',
    '2c8c9cc8-141b-4d06-872a-8d5e5efb3641',
    '99619bdd-6452-4850-9244-a4ce9bec70ca',
    '9e7780e9-7263-4ee4-a722-b3e1aaff4476',
    '719722d0-2354-437e-acdc-4625989bbca8'
  ]),
  'Submitted': new Set([
    '3a7735f2-759d-4f49-bca0-0828f32cf86c'
  ]),
  'Feedback': new Set([
    'f49f91de-64fc-4cca-88bf-3238fb1de69b'
  ]),
  'Approved': new Set([
    '6cf61bc3-9714-4902-953f-76a238d5ffd5',
    'cdfdebda-edde-4af4-aaf4-9d59fca7cdaa',
    '3d10430f-1748-44ad-8df3-5d5d11544f75'
  ])
}

fixture('Standards Compliance - Dashboard')
  .page(supplierDashboardPage.baseUrl)
  .beforeEach(navigateToStandardsDashboard)
  .afterEach(axeCheck)

function navigateToStandardsDashboard (t) {
  return asSupplier(t)
    .click(supplierDashboardPage.homeLink)
    .click(supplierDashboardPage.lastOnboardingSolutionName)
    .click(onboardingDashboardPage.standardsComplianceButton)
    .expect(Selector('#compliance').exists).ok()
}

test('Applicable standards are present and displayed in the correct sections', async t => {
  const assocStd = await standardsComplianceDashboardPage.associatedStandardsTable.find('.standard')
  const ovrchStd = await standardsComplianceDashboardPage.overarchingStandards.find('.standard')

  await t
    .expect(assocStd.count).eql(1)
    .expect(ovrchStd.count).eql(11)
})

test('Displayed Applicable Standards match expected standard\'s status descriptions', async t => {
  const standards = await Selector('.standard')
  const count = await standards.count

  for (let i = 0; i < count; i++) {
    const currentStandard = await standards.nth(i)
    const currentLinkUUID = (
      await currentStandard.find('a').getAttribute('href')
    ).match('evidence/([^/]+)/')[1] // I know Kung Fu (Thanks Paul!)
    const currentStatus = await currentStandard.find('.status').innerText
    await t.expect(expectedStatusStandards[currentStatus].has(currentLinkUUID)).ok()
  }
})
