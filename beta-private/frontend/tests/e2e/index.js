/* global fixture, test */

const { Selector, Role } = require('testcafe')
const axeCheck = require('axe-testcafe')

const supplierRole = Role('http://localhost:3000/oidc/authenticate', async t => {
  await t
    .typeText('[name=email]', 'drkool@kool.com')
    .typeText('[name=password]', 'kool')
    .click('[name=submit]')
})

const homeLink = Selector('body > header a[aria-label=Home]')
const firstOnboardingSolutionName = Selector(
  '#solutions-onboarding table > tbody > tr:first-child > td:first-child'
)
const continueRegistrationButton = Selector('#content a[href^=register]')

const solutionNameInput = Selector('#content [name="solution\\[name\\]"]')
const solutionDescriptionInput = Selector('#content [name="solution\\[description\\]"]')
const solutionVersionInput = Selector('#content [name="solution\\[version\\]"]')
const continueButton = Selector('[name="action\\[continue\\]"]')

fixture('Getting started')
  .page('http://localhost:3000')

test('a11y: logged out homepage', async t => {
  await axeCheck(t)
})

test('Login as supplier', async t => {
  await t
    .useRole(supplierRole)
    .expect(Selector('#account .user').innerText).contains('Hello, Dr')
})

test('a11y: supplier homepage', async t => {
  await t
    .useRole(supplierRole)
  await axeCheck(t)
})

test('Clicking logo returns to supplier homepage', async t => {
  await t
    .useRole(supplierRole)
    .click('body > header a[href^="/about"]')
    .click(homeLink)

    .expect(Selector('#content > h1').innerText).eql('Supplier Home')
})

test('Solutions that are currently onboarding are listed', async t => {
  await t
    .useRole(supplierRole)
    .expect(firstOnboardingSolutionName.innerText).eql('Really Kool Document Manager | 1')
})

test('Registration page shows correct information accessibly', async t => {
  await t
    .useRole(supplierRole)
    .click(firstOnboardingSolutionName)
    .click(continueRegistrationButton)
    .expect(solutionNameInput.value).eql('Really Kool Document Manager')
    .expect(solutionDescriptionInput.value).eql('"Does Really Kool document management"')
    .expect(solutionVersionInput.value).eql('1')

  await axeCheck(t)
})

test('Registration page validation is correct and accessible', async t => {
  await t
    .useRole(supplierRole)
    .click(firstOnboardingSolutionName)
    .click(continueRegistrationButton)
    .selectText(solutionNameInput).pressKey('backspace')
    .selectText(solutionDescriptionInput).pressKey('backspace')
    .click(continueButton)

    .expect(Selector('#errors #error-solution\\.name').innerText).contains('Solution name is missing')
    .expect(Selector('#errors #error-solution\\.description').innerText).contains('Solution description is missing')

  await axeCheck(t)
})

test('Capabilities page shows correct information accessibly', async t => {
  await t
    .useRole(supplierRole)
    .click(firstOnboardingSolutionName)
    .click(continueRegistrationButton)
    .click(continueButton)

    .expect(Selector('[type=checkbox][name^=capabilities]').count).eql(18)
    .expect(Selector('[type=checkbox][name^=capabilities] ~ .name').nth(0).innerText).eql('Appointments Management - Citizen')
    .expect(Selector('[type=checkbox][name^=capabilities] ~ .name').nth(17).innerText).eql('Workflow')
    .expect(Selector('[name=capabilities\\[CAP10\\]]').checked).ok()

  await axeCheck(t)
})

test('Capabilities page validation is correct and accessible', async t => {
  await t
    .useRole(supplierRole)
    .click(firstOnboardingSolutionName)
    .click(continueRegistrationButton)
    .click(continueButton)

  const checkedCapabilities = Selector('[type=checkbox][name^=capabilities]:checked')
  const checkedCapabilitiesCount = await checkedCapabilities.count
  for (let i = 0; i < checkedCapabilitiesCount; i++) {
    await t.click(checkedCapabilities.nth(i))
  }

  await t
    .click(continueButton)
    .expect(Selector('#errors #error-capabilities').innerText).contains('Select at least one capability to continue')
    .expect(Selector('[type=checkbox][name^=capabilities]:checked').exists).notOk('No capabilities should be selected after reload')

  await axeCheck(t)
})
