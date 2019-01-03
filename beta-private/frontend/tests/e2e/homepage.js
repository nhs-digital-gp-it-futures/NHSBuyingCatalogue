/* global fixture, test */

import axeCheck from 'axe-testcafe'
import { homePage } from './pages'

fixture('Homepage')
  .page(page.baseUrl)
  .afterEach(axeCheck)

test('a11y: logged out homepage', async t => {
})
