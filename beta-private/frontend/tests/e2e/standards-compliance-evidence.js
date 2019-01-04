/* global fixture, test */

import { Selector, RequestLogger } from 'testcafe'
import axeCheck from 'axe-testcafe'

import { asSupplier } from './roles'
import { supplierDashboardPage, onboardingDashboardPage, standardsComplianceDashboardPage } from './pages'

const nonFunctionalStdID = '2c8c9cc8-141b-4d06-872a-8d5e5efb3641'
const businessContinuityStdID = '719722d0-2354-437e-acdc-4625989bbca8'
const dataMigrationStdID = 'f49f91de-64fc-4cca-88bf-3238fb1de69b'
const testingStdID = '99619bdd-6452-4850-9244-a4ce9bec70ca'

fixture('Standards Compliance - Evidence')
  .page(supplierDashboardPage.baseUrl)
  .beforeEach(navigateToStandardsDashboard)
  .afterEach(axeCheck)

function navigateToStandardsDashboard (t) {
  return asSupplier(t)
    .click(supplierDashboardPage.homeLink)
    .click(supplierDashboardPage.secondSolutionName)
    .click(onboardingDashboardPage.standardsComplianceButton)
    .expect(Selector('#compliance').exists).ok()
}

test('With no tracability Matrix present, the page displays a \'please wait\' message', async t => {
  const messageSelector = await Selector('.message.feedback > p:first-child')
  await t
    .click(`a[href*="${nonFunctionalStdID}"]`)
    .expect(messageSelector.innerText).contains('Please wait')
})

test('a \'Not Started\' With a tracability Matrix present, the page shows a form', async t => {
  const filename = 'Dummy TraceabilityMatrix.xlsx'

  const fileDownloadSelector = await Selector(`a[href*="${testingStdID}"]`)

  await t
    .click(fileDownloadSelector)
    .expect(fileDownloadSelector.innerText).contains(filename)
})

const requestLogger = RequestLogger(
  request => request.url.startsWith(standardsComplianceDashboardPage.baseUrl),
  { logResponseBody: true }
)

test
  .requestHooks(requestLogger)(
    'On download, the previously uploaded file should be identical', async t => {
      const fileDownloadSelector = await Selector(`a[href*="${testingStdID}"]`)

      requestLogger.clear()

      await t
        .click(fileDownloadSelector)

        // Ensure that the response has been received and that its status code is 200.
        .expect(requestLogger.contains(record => record.response.statusCode === 200)).ok()

      // Compute the SHA1 hash of the downloaded data and compare to the known hash
      // of the file that was uploaded
      const crypto = require('crypto')
      const hash = crypto.createHash('sha1')
      hash.update(requestLogger.requests[0].response.body)
      console.log(requestLogger.requests[0].response.body, requestLogger.requests[0].response.body.toString())
      await t
        .expect(hash.digest('hex')).eql('e199d8397d9cb790b338389b6aaa6ac5de2c2b00', 'Download does not match expected')
    }
  )
