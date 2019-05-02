const AntivirusProvider = require('./AntivirusProvider')

class ClamAVProvider extends AntivirusProvider {
  constructor (clamav) {
    super()
    this.clamav = clamav || require('clamav.js')
    console.info('ClamAV Provider Active')
  }

  scanFile (fileStream) {
    return new Promise((resolve, reject) => {
      this.clamav.createScanner(3310, 'clamav').scan(fileStream, (err, object, malicious) => {
        if (err) return reject(err)
        else if (malicious) return resolve(malicious)
        else return resolve()
      })
    })
  }
}

module.exports = ClamAVProvider
