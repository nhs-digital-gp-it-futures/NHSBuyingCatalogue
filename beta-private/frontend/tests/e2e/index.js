const path = require('path')
const testfiles = [
  'homepage.js',
  'supplier-homepage.js',
  'supplier-registration-details.js',
  'supplier-registration-add-new-solution.js',
  'supplier-registration-capabilities.js'
].map(
  f => path.join(__dirname, f)
)

const createTestcafe = require('testcafe')

let testcafe

createTestcafe('localhost')
  .then(tc => {
    testcafe = tc

    return testcafe.createRunner()
      .src(testfiles)
      .browsers('chrome')
      .run()
  })
  .then(failCount => {
    testcafe.close()
  })
