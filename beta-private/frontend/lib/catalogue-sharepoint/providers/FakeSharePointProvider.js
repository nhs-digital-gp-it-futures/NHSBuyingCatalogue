const SharePointProvider = require('./SharePointProvider')

class FakeFileStoreAPI {
  constructor () {
    this.folders = {}
  }

  initialiseClaim (claimID) {
    this.folders[claimID] = {
      pageIndex: 0,
      totalPages: 0,
      pageSize: 0,
      items: [],
      hasPreviousPage: false,
      hasNextPage: false
    }
  }

  async addItemToClaim (claimID, readStream, filename, options) {
    if (!this.folders[claimID]) {
      this.initialiseClaim(claimID)
    }

    const newItem = {
      name: filename,
      isFolder: false,
      length: await readStream.readableLength,
      url: `FakeSharePoint.${claimID}.${filename}`,
      timeLastModified: (new Date()).toISOString(),
      blobId: `FakeSharePoint.${claimID}.${filename}`
    }

    this.folders[claimID].items.push(newItem)
  }

  async enumerateFolder (claimID) {
    if (!this.folders[claimID]) {
      this.initialiseClaim(claimID)
    }
    return this.folders[claimID]
  }
}

// Provides Fake for testing Front end aspects that would typically in
class FakeStandardsApplicableEvidenceBlobStoreApi extends FakeFileStoreAPI {
  async apiStandardsApplicableEvidenceBlobStoreEnumerateFolderByClaimIdGet (claimID, options) {
    return this.enumerateFolder(claimID)
  }

  async apiStandardsApplicableEvidenceBlobStoreAddEvidenceForClaimPost (claimID, readStream, filename, options) {
    return this.addItemToClaim(claimID, readStream, filename, options)
  }
}

class FakeCapabilitiesImplementedEvidenceBlobStoreApi extends FakeFileStoreAPI {
  async apiCapabilitiesImplementedEvidenceBlobStoreEnumerateFolderByClaimIdGet (claimID, options) {
    return this.enumerateFolder(claimID)
  }

  async apiCapabilitiesImplementedEvidenceBlobStoreAddEvidenceForClaimPost (claimID, readStream, filename, options) {
    return this.addItemToClaim(claimID, readStream, filename, options)
  }
}

const FakeCatalogueAPI = {
  StandardsApplicableEvidenceBlobStoreApi: FakeStandardsApplicableEvidenceBlobStoreApi,
  CapabilitiesImplementedEvidenceBlobStoreApi: FakeCapabilitiesImplementedEvidenceBlobStoreApi
}

class FakeSharePointProvider extends SharePointProvider {
  constructor () {
    super(FakeCatalogueAPI)
  }
}

module.exports = FakeSharePointProvider
