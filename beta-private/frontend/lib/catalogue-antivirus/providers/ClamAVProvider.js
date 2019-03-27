const AntivirusProvider = require('./AntivirusProvider')
const clamav = require('clamav.js')

class ClamAVProvider extends AntivirusProvider {
  constructor () {
    console.info('ClamAV Provider Active')
    super()
  }

  scanFile (fileStream) {
    return new Promise((resolve, reject) => {
      clamav.createScanner(3310, 'clamav').scan(fileStream, (err, object, malicious) => {
        if (err) return reject(err)
        else if (malicious) return resolve(malicious)
        else return resolve()
      })
    })
  }
}

module.exports = ClamAVProvider
