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
const saveButton = Selector('[name="action\\[save\\]"]')

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
    .click(saveButton)

    .expect(Selector('#errors #error-solution\\.name').innerText).contains('Solution name is missing')
    .expect(Selector('#errors #error-solution\\.description').innerText).contains('Solution description is missing')

  await axeCheck(t)
})
