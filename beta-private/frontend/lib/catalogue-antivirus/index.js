
const ClamAVProvider = require('./providers/ClamAVProvider')
const DisabledAVProvider = require('./providers/DisabledAVProvider')

module.exports = {
  antivirusProvider:
    process.env.ANTIVIRUS_PROVIDER_ENV === 'DISABLED' ? new DisabledAVProvider()
      : process.env.ANTIVIRUS_PROVIDER_ENV === 'CLAMAV' ? new ClamAVProvider()
        : new ClamAVProvider(),
  AntivirusProvider: require('./providers/AntivirusProvider')
}
