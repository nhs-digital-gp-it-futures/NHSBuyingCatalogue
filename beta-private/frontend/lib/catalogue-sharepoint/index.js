
const FakeSharePointProvider = require('./FakeSharePointProvider')
const RealSharePointProvider = require('./RealSharePointProvider')

module.exports = {
  sharePointProvider: process.env.NODE_ENV === 'TEST'
    ? new FakeSharePointProvider()
    : new RealSharePointProvider(),
  SharePointProvider: require('./SharePointProvider')
}
