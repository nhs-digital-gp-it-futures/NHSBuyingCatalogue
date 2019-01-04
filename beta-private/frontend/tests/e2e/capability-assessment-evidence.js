/* global fixture, test */

import { Selector, RequestLogger } from 'testcafe'
import axeCheck from 'axe-testcafe'

import { asSupplier } from './roles'
import { supplierDashboardPage, onboardingDashboardPage, capabilityEvidencePage } from './pages'

fixture('Capability Assessment - Evidence')
  .page(supplierDashboardPage.baseUrl)
  .beforeEach(navigateToCapabilityAssessment)
  .afterEach(axeCheck)

function navigateToCapabilityAssessment (t) {
  return asSupplier(t)
    .click(supplierDashboardPage.homeLink)
    .click(supplierDashboardPage.lastOnboardingSolutionName)
    .click(onboardingDashboardPage.capabilityAssessmentButton)
    .expect(Selector('#capability-assessment-page').exists).ok()
}

test('All capabilities registered against the solution are present in the alphabetic order', async t => {
  await t
    .expect(Selector('fieldset.collapsible#CAP-C-001 > legend').textContent).eql('Appointments Management - GP')
    .expect(Selector('fieldset.collapsible#CAP-C-002 > legend').textContent).eql('GP Referral Management')
    .expect(Selector('fieldset.collapsible#CAP-C-003 > legend').textContent).eql('GP Resource Management')

    .expect(Selector('form#capability-assessment-form > fieldset.collapsible:nth-of-type(1)').id).eql('CAP-C-001')
    .expect(Selector('form#capability-assessment-form > fieldset.collapsible:nth-of-type(2)').id).eql('CAP-C-002')
    .expect(Selector('form#capability-assessment-form > fieldset.collapsible:nth-of-type(3)').id).eql('CAP-C-003')
})

test('Collapsible fieldsets behave as an accordion', async t => {
  const allTheFieldsets = Selector('form#capability-assessment-form > fieldset.collapsible')

  await t
    .expect(allTheFieldsets.nth(0).hasClass('collapsed')).notOk()
    .expect(allTheFieldsets.nth(1).hasClass('collapsed')).ok()
    .expect(allTheFieldsets.nth(2).hasClass('collapsed')).ok()

    .click(allTheFieldsets.nth(2).find('legend'))

    .expect(allTheFieldsets.nth(0).hasClass('collapsed')).ok()
    .expect(allTheFieldsets.nth(1).hasClass('collapsed')).ok()
    .expect(allTheFieldsets.nth(2).hasClass('collapsed')).notOk()

    .click(allTheFieldsets.nth(0).find('legend'))
    .click(allTheFieldsets.nth(1).find('legend'))

    .expect(allTheFieldsets.nth(0).hasClass('collapsed')).ok()
    .expect(allTheFieldsets.nth(1).hasClass('collapsed')).notOk()
    .expect(allTheFieldsets.nth(2).hasClass('collapsed')).ok()
})

function uploadFile (t, sel, filename) {
  return t
    .click(sel.child('legend'))
    .click(sel.find('input[type=radio][value=yes]'))
    .setFilesToUpload(
      sel.find('input[type=file]'),
      `./uploads/${filename}`
    )
}

function uploadFileWithMessage (t, sel, filename, message) {
  return uploadFile(t, sel, filename)
    .typeText(
      sel.find('textarea'),
      message,
      { replace: true }
    )
}

function verifyFileUploaded (t, sel, filename) {
  return t
    .click(sel.child('legend'))
    .expect(sel.find('.latest-upload').textContent).contains('last file you uploaded was')
    .expect(sel.find('.latest-upload a').textContent).eql(filename)
}

test('After uploading a file, the supplier dashboard and onboarding dashboard should show updated status', async t => {
  // we'll add evidence for "GP Resource Management"
  const capSection = Selector('fieldset.collapsible#CAP-C-003')

  await uploadFileWithMessage(t, capSection, 'Dummy TraceabilityMatrix.xlsx', 'Automation testing message sent with uploaded file')
    .click(capabilityEvidencePage.globalSaveAndExitButton)

    .expect(onboardingDashboardPage.capabilityAssessmentButton.textContent).eql('Edit')
    .expect(onboardingDashboardPage.capabilityAssessmentStatus.textContent).contains('In progress')
})

test('After uploading a file, the name of the file should show against the capability', async t => {
  const capSection = Selector('fieldset.collapsible#CAP-C-003')

  await verifyFileUploaded(t, capSection, 'Dummy TraceabilityMatrix.xlsx')
})

const requestLogger = RequestLogger(
  request => request.url.startsWith(capabilityEvidencePage.baseUrl),
  { logResponseBody: true }
)

test
  .requestHooks(requestLogger)(
    'On download, the previously uploaded file should be identical', async t => {
      const capSection = Selector('fieldset.collapsible#CAP-C-003')

      requestLogger.clear()

      await t
        .click(capSection.child('legend'))
        .click(capSection.find('.latest-upload a'))

        // Ensure that the response has been received and that its status code is 200.
        .expect(requestLogger.contains(record => record.response.statusCode === 200)).ok()

      // Compute the SHA1 hash of the downloaded data and compare to the known hash
      // of the file that was uploaded
      const crypto = require('crypto')
      const hash = crypto.createHash('sha1')
      hash.update(requestLogger.requests[0].response.body)

      await t
        .expect(hash.digest('hex')).eql('21642af57eb7fd206395cdf745672bdafb8b96ae', 'Download does not match upload')
    }
  )

test('Messages sent along with the uploaded file should be preserved', async t => {
  const capSection = Selector('fieldset.collapsible#CAP-C-003')

  await verifyFileUploaded(t, capSection, 'Dummy TraceabilityMatrix.xlsx')
    .expect(capSection.find('textarea').value).eql('Automation testing message sent with uploaded file')
})

// Test skipped due to Testcafé bug #?
// Which we discovered with this very test 🙂
test.skip('Files and messages can be saved against multiple capabilities at once', async t => {
  // note that the second capability is done first so that the accordion click doesn't
  // close the first capability's fieldset
  const capSection1 = Selector('fieldset.collapsible#CAP-C-002')
  await uploadFileWithMessage(t, capSection1, 'Dummy TraceabilityMatrix 2.xlsx', 'Automated test message for CAP-C-002')

  const capSection2 = Selector('fieldset.collapsible#CAP-C-001')
  await uploadFileWithMessage(t, capSection2, 'Dummy TraceabilityMatrix 3.xlsx', 'Automated test message for CAP-C-001')

  await t
    .click(capabilityEvidencePage.globalSaveButton)

  await verifyFileUploaded(t, capSection1, 'Dummy TraceabilityMatrix 2.xlsx')
    .expect(capSection1.find('textarea').value).eql('Automated test message for CAP-C-002')
  await verifyFileUploaded(t, capSection2, 'Dummy TraceabilityMatrix 3.xlsx')
    .expect(capSection2.find('textarea').value).eql('Automated test message for CAP-C-001')
})

test('Clicking Continue with incomplete evidence should trigger a validation message', async t => {
  // note that the second capability is done first so that the accordion click doesn't
  // close the first capability's fieldset
  const capSection1 = Selector('fieldset.collapsible#CAP-C-002')
  await uploadFileWithMessage(t, capSection1, 'Dummy TraceabilityMatrix 2.xlsx', 'Automated test message for CAP-C-002')

  await t
    .click(capabilityEvidencePage.continueButton)
    .expect(Selector('#errors').exists).ok()
    .expect(Selector('#error-uploading-video-evidence').textContent).contains('requiring evidence is missing a video')

  // as the first capability has no file or message, the missing file validation error should trigger
  // even with a validation message, the submitted file and message should hold
  await verifyFileUploaded(t, capSection1, 'Dummy TraceabilityMatrix 2.xlsx')
    .expect(capSection1.find('textarea').value).eql('Automated test message for CAP-C-002')

  // uploading a file for the first capability without setting a message should
  // trigger a missing description validation error
  const capSection2 = Selector('fieldset.collapsible#CAP-C-001')
  await uploadFile(t, capSection2, 'Dummy TraceabilityMatrix 3.xlsx')

  await t
    .click(capabilityEvidencePage.continueButton)
    .expect(Selector('#errors').exists).ok()
    .expect(Selector('#error-uploading-video-evidence').textContent).contains('an uploaded video is missing a description')

  // even with a validation message, the submitted file and message should hold
  await t.click(capSection2.child('legend')) // close the section because the next call opens it
  await verifyFileUploaded(t, capSection2, 'Dummy TraceabilityMatrix 3.xlsx')
})

test.skip('Clicking Continue with complete evidence should lead to summary page with correct details', async t => {
})

test.skip('Submitting from the summary page should update the status on the dashboard and onboarding page', async t => {
})
