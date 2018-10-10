/* global fixture, test */

const { Selector } = require('testcafe')
const axeCheck = require('axe-testcafe')

fixture `Getting started`
  .page `http://localhost:3000`

test('a11y: logged out homepage', async t => {
  await axeCheck(t)
})

test('Login as supplier', async t => {
  await t
    .click('#account .auth a')
    .typeText('[name=email]', 'drkool@kool.com')
    .typeText('[name=password]', 'kool')
    .click('[name=submit]')

    .expect(Selector('#account .user').innerText).contains('Hello, Dr')
})

test('a11y: supplier homepage', async t => {
  await axeCheck(t)
})

test('Clicking logo returns to supplier homepage', async t => {
  await t
    .click('body > header a[aria-label=Home')

    .expect(Selector('#content > h1').innerText).eql('Supplier Home')
})
