const AntivirusProvider = require('./AntivirusProvider')

class DisabledAVProvider extends AntivirusProvider {
  constructor () {
    console.info('Disabled Antivirus Provider Active')
    super()
  }

  scanFile (fileStream) {
    return new Promise((resolve, reject) => {
      return resolve()
    })
  }
}

module.exports = DisabledAVProvider
