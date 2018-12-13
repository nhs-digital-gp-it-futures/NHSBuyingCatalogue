const SharePointProvider = require('./SharePointProvider')

class FakeStandardsApplicableEvidenceBlobStoreApi {

}

class FakeCapabilitiesImplementedEvidenceBlobStoreApi {

}

const FakeCatalogueAPI = {
  stdBlobStoreApi: FakeStandardsApplicableEvidenceBlobStoreApi,
  capBlobStoreApi: FakeCapabilitiesImplementedEvidenceBlobStoreApi
}

class FakeSharePointProvider extends SharePointProvider {
  constructor () {
    super(FakeCatalogueAPI)
  }
}

module.exports = {
  FakeSharePointProvider
}
