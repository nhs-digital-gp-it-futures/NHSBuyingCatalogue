const fs = require('fs')
const clamav = require('clamav.js')

async function scanFile (filepath) {
  const stream = fs.createReadStream(filepath)
  return new Promise((resolve, reject) => {
    clamav.createScanner(3310, '127.0.0.1').scan(stream, (err, object, malicious) => {
      if (err) {
        console.log(object.path + ': ' + err)
        return Promise.reject(err)
      } else if (malicious) {
        console.log(object.path + ': ' + malicious + ' FOUND')
        return Promise.resolve(malicious)
      } else {
        console.log(object.path + ': OK')
        return Promise.resolve()
      }
    })
  })
}

const fp = '../../tests/e2e/uploads/eicar.com'

scanFile(fp)
  .then((res) => {
    if (res) {
      console.log('VIRUS:', res)
    } else {
      console.log('NO VIRUS')
    }
  })
  .catch((err) => {
    console.log(err)
  })

