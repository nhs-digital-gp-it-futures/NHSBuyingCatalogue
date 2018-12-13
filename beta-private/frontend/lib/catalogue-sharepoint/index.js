
module.exports = {
  sharePointProvider: process.env.NODE_ENV === 'TEST'
    ? new (require('./FakeSharePointProvider'))()
    : new (require('./RealSharePointProvider'))(),
  SharePointProvider: require('./SharePointProvider')
}
