/* global fixture, test */

//
//  These tests are really to ensure that content wasn't accidentally deleted
//  on the homepage at some point...
//

import axeCheck from 'axe-testcafe'
import { homePage } from './pages'

const HOMEPAGE_TITLE = 'Introducing the NHS Store'
const LOGIN_TEXT = 'Log In'
const NUMBER_OF_CARDS = 3
const NUMBER_OF_STEPS = 4
const NUMBER_OF_CHILDREN_PER_CARD = 1

fixture('Homepage')
  .page(homePage.baseUrl)
  .afterEach(axeCheck)

test('a11y: logged out homepage', async t => {
  await t.expect(homePage.loginLink.innerText).eql(LOGIN_TEXT)
})

test('Home Page has a welcome banner containing the correct elements', async t => {
  const title = await homePage.welcomeBanner.find('h1')
  const subTitle = await homePage.welcomeBanner.find('p').nth(0)
  const information = await homePage.welcomeBanner.find('p').nth(1)

  await t.expect(title.innerText).eql(HOMEPAGE_TITLE)
  await t.expect(subTitle.exists).ok()
  await t.expect(information.exists).ok()
})

test('Home Page has 3 cards displaying information', async t => {
  await t.expect(homePage.cardContainer.childElementCount).eql(NUMBER_OF_CARDS)
})

test('Home Page Cards have the correct titles', async t => {
  const cards = homePage.cardContainer.child()
  const titles = [
    'GP IT Futures Framework',
    'Capabilities & Standards',
    'My Solutions'
  ]

  for (let i = 0; i < await cards.count; i++) {
    const cardTitle = await cards.nth(i).find('h3')
    await t.expect(cardTitle.innerText).eql(titles[i])
  }
})

test('Home Page Cards have have one child that is an anchor', async t => {
  const cards = await homePage.cardContainer.child()
  for (let i = 0; i < await cards.count; i++) {
    const children = await cards.nth(i).child()
    await t.expect(children.count).eql(NUMBER_OF_CHILDREN_PER_CARD)
    await t.expect(children.nth(0).tagName).eql('a')
  }
})

test('Homepage have 4 steps displaying information', async t => {
  await t.expect(homePage.stepsContainer.childElementCount).eql(NUMBER_OF_STEPS)
})

test('Home Page Steps have the correct titles', async t => {
  const steps = await homePage.stepsContainer
  const titles = [
    'Provide basic details and select Capabilities',
    'Provide evidence for your Solution\'s Capabilities',
    'Provide evidence for your Solution\'s Standards',
    'Build your Solution Page (What Buyers will see)'
  ]

  for (let i = 0; i < await steps.count; i++) {
    const stepTitle = await steps.nth(i).find('h3')
    await t.expect(stepTitle.innerText).eql(titles[i])
  }
})
