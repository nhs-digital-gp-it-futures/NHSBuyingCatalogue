
const FakeSharePointProvider = require('./FakeSharePointProvider')
const RealSharePointProvider = require('./RealSharePointProvider')

module.exports = {
  sharePointProvider: process.env.SHAREPOINT_PROVIDER_ENV === 'test'
    ? new FakeSharePointProvider()
    : new RealSharePointProvider(),
  SharePointProvider: require('./SharePointProvider')
}
