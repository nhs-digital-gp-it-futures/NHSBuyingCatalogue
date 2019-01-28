/* global fixture, test */

import { Selector, RequestLogger } from 'testcafe'

import { asSupplier } from './roles'
import { supplierDashboardPage, onboardingDashboardPage, standardsComplianceDashboardPage } from './pages'

const nonFunctionalStdID = '2c8c9cc8-141b-4d06-872a-8d5e5efb3641'
const businessContinuityStdID = '719722d0-2354-437e-acdc-4625989bbca8'
const clinicalSafetyStdID = '7b62b29a-62a7-4b4a-bcc8-dfa65fb7e35c'
const testingStdID = '99619bdd-6452-4850-9244-a4ce9bec70ca'
const commercialStdID = '3a7735f2-759d-4f49-bca0-0828f32cf86c'
const interopStdID = '0e55d9ec-43e6-41b3-bcac-d8681384ea68'
const dataMigrationStdID = '0e55d9ec-43e6-41b3-bcac-d8681384ea68'

const downloadFileName = 'Dummy TraceabilityMatrix.xlsx'

function setFileToUpload (t, filename) {
  return t
    .setFilesToUpload(
      Selector('input[type=file]'),
      `./uploads/${filename}`
    )
}

const requestLogger = RequestLogger(
  request => request.url.startsWith(standardsComplianceDashboardPage.baseUrl),
  { logResponseBody: true }
)

fixture('Standards Compliance - Evidence')
  .page(supplierDashboardPage.baseUrl)
  .beforeEach(navigateToStandardsDashboard)
  .afterEach(supplierDashboardPage.checkAccessibility)

function navigateToStandardsDashboard (t) {
  return asSupplier(t)
    .click(supplierDashboardPage.homeLink)
    .click(supplierDashboardPage.lastOnboardingSolutionName)
    .click(onboardingDashboardPage.standardsComplianceButton)
    .expect(Selector('#compliance').exists).ok()
}

test('With no traceability Matrix present, the page displays a \'please wait\' message', async t => {
  const messageSelector = await Selector('.message.feedback > p:first-child')
  await t
    .click(`a[href*="${nonFunctionalStdID}"]`)
    .expect(messageSelector.innerText).contains('Please wait')
})

test('a \'Not Started\' With a traceability Matrix present, the page shows a form with a file upload', async t => {
  await t
    .click(`a[href*="${testingStdID}"]`)
    .expect(Selector('.file-input')).ok()
})

test('A submitted standard does not allow further uploads but does allow download', async t => {
  await t
    .click(`a[href*="${commercialStdID}"]`)
    .expect(Selector('.has-feedback').innerText).contains('Awaiting Compliance Outcome')
    .expect(Selector('.current-file').exists).ok()
})

// Skipped as the acceptance criteria did not clearly specify this as being required,
// meaning that it was not implemented with this in mind.
// This test should be re-enabled once the tech debt is resolved.
test.skip('A Failed standard should not allow the uploading of any more files', async t => {
  await t
    .click(`a[href*="${interopStdID}"]`)
    .expect(Selector('.file-input').exists).notOk()
})

test
  .requestHooks(requestLogger)(
    'A Standard with a file to download should allow the download of that file', async t => {
      const testingStdSelector = await Selector(`a[href*="${testingStdID}"]`)

      await t.click(testingStdSelector) // go from the dashboard to the download page

      requestLogger.clear()

      await t.click(testingStdSelector) // click the download link.
        .expect(requestLogger.contains(record => record.response.statusCode === 200)).ok()

      await t
        .expect(
          requestLogger.requests[0].response.body.toString()
        ).eql('Content of the test "Dummy traceabilityMatrix.xlsx" file', 'Download does not match expected')
    }
  )

test('A \'Not Started\' standard should change to draft if a file is saved against it.', async t => {
  // navigate to 'business continuity...' standard evidence upload page.
  const businessContinuitySelector = Selector(`a[href*="${businessContinuityStdID}"]`)
  await t.click(businessContinuitySelector)
  await setFileToUpload(t, downloadFileName)
    .click('input[value="Save"]')
    .click('.breadcrumb li:nth-child(2) > a')

    // Selecting the Row that is the parent of the link, so that the sibling cell with containing status can be checked
    .expect(businessContinuitySelector.parent().nth(1).find('.status').innerText).eql('Draft')
    .expect(businessContinuitySelector.parent().nth(1).find('.owner').innerText).eql('With Helpma Boab')
})

test('The owner of a standard can be set correctly and is reflected on the Dashboard when saved', async t => {
  // navigate to 'business continuity...' standard evidence upload page.
  const businessContinuitySelector = Selector(`a[href*="${businessContinuityStdID}"]`)
  const ownerDropdown = Selector('#compliance [name="ownerId"]')
  const candidateOwners = ownerDropdown.child('option')

  await t
    .click(businessContinuitySelector)
    .expect(Selector('#compliance .standard-owner .current-owner').textContent).eql('Helpma Boab')
    .expect(ownerDropdown.visible).notOk()

  await t
    .click('#change-owner-button')
    .expect(candidateOwners.count).eql(4)
    .expect(candidateOwners.nth(0).textContent).contains('Lead Contact (Helpma Boab)')
    .expect(candidateOwners.nth(1).textContent).contains('Dr Kool')
    .expect(candidateOwners.nth(2).textContent).contains('Helpma Boab')
    .expect(candidateOwners.nth(3).textContent).contains('Zyra Featherstonhaugh')

  await setFileToUpload(t, downloadFileName)
    .click(ownerDropdown)
    .click(candidateOwners.nth(3))
    .click('input[value="Save"]')
    .click('.breadcrumb li:nth-child(2) > a')

    // Selecting the Row that is the parent of the link, so that the sibling cell with containing owner can be checked
    .expect(businessContinuitySelector.parent().nth(1).find('.owner').textContent).contains('Zyra Featherstonhaugh')
})

test('A standard should change to Submitted if evidence is submitted.', async t => {
  // navigate to 'business continuity...' standard evidence upload page.
  const clinicalSafetySelector = Selector(`a[href*="${clinicalSafetyStdID}"]`)

  await t.click(clinicalSafetySelector)

  await setFileToUpload(t, downloadFileName)
    .click('#submit-for-compliance')
    .expect(Selector('#compliance-confirmation > h1').innerText).contains('Review and submit')

  await t
    .click('#compliance-submission-confirmation-button')

    // Selecting the Row that is the parent of the link, so that the sibling cell with containing status can be checked
    .expect(clinicalSafetySelector.parent().nth(1).find('.status').innerText).eql('Submitted')
    .expect(clinicalSafetySelector.parent().nth(1).find('.owner').innerText).eql('With NHS Digital')
    .expect(Selector('.standard-submitted h3').innerText).contains('Evidence submitted')
})

test('A standard sent back to the supplier should allow the supplier to resubmit', async t => {
  // navigate to 'business continuity...' standard evidence upload page.
  const dataMigrationSelector = Selector(`a[href*="${dataMigrationStdID}"]`)

  await t.click(dataMigrationSelector)

  await setFileToUpload(t, downloadFileName)
    .click('#submit-for-compliance')
    .expect(Selector('#compliance-confirmation > h1').innerText).contains('Review and submit')

  await t
    .click('#compliance-submission-confirmation-button')

    // Selecting the Row that is the parent of the link, so that the sibling cell with containing status can be checked
    .expect(dataMigrationSelector.parent().nth(1).find('.status').innerText).eql('Submitted')
})
