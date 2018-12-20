const SharePointProvider = require('./SharePointProvider')
const stream = require('stream')

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

    const guid = require('node-uuid-generator').generate()
    const buffer = await this.readStream(readStream)

    const newItem = {
      name: filename,
      isFolder: false,
      length: buffer.length,
      url: `FakeSharePoint.${guid}`,
      timeLastModified: (new Date()).toISOString(),
      blobId: `FakeSharePoint.${guid}`,
      buffer: buffer
    }

    this.folders[claimID].items.push(newItem)
  }

  async enumerateFolder (claimID) {
    if (!this.folders[claimID]) {
      this.initialiseClaim(claimID)
    }
    return this.folders[claimID]
  }

  async readStream (readStream) {
    return new Promise((resolve, reject) => {
      var buffers = []
      readStream.on('data', (data) => buffers.push(data))
      readStream.on('end', () => resolve(Buffer.concat(buffers)))
    })
  }

  async downloadFile (claimID, opts) {
    if (!this.folders[claimID]) {
      return Promise.reject(new Error('Claim Not Found'))
    }

    const item = this.folders[claimID].items.find((item) => item.blobId === opts.uniqueId)

    if (!item) {
      return Promise.reject(new Error('File Not Found'))
    }

    const bufferStream = new stream.PassThrough()
    bufferStream.end(item.buffer)
    return { body: bufferStream }
  }
}

class FakeStandardsApplicableEvidenceBlobStoreApi extends FakeFileStoreAPI {
  async apiStandardsApplicableEvidenceBlobStoreEnumerateFolderByClaimIdGet (claimID, options) {
    return this.enumerateFolder(claimID)
  }

  async apiStandardsApplicableEvidenceBlobStoreAddEvidenceForClaimPost (claimID, readStream, filename, options) {
    return this.addItemToClaim(claimID, readStream, filename, options)
  }

  async apiStandardsApplicableEvidenceBlobStoreDownloadByClaimIdPost (claimID, opts) {
    return this.downloadFile(claimID, opts)
  }
}

class FakeCapabilitiesImplementedEvidenceBlobStoreApi extends FakeFileStoreAPI {
  async apiCapabilitiesImplementedEvidenceBlobStoreEnumerateFolderByClaimIdGet (claimID, options) {
    return this.enumerateFolder(claimID)
  }

  async apiCapabilitiesImplementedEvidenceBlobStoreAddEvidenceForClaimPost (claimID, readStream, filename, options) {
    return this.addItemToClaim(claimID, readStream, filename, options)
  }

  async apiCapabilitiesImplementedEvidenceBlobStoreDownloadByClaimIdPost (claimID, opts) {
    return this.downloadFile(claimID, opts)
  }
}

const FakeCatalogueAPI = {
  StandardsApplicableEvidenceBlobStoreApi: FakeStandardsApplicableEvidenceBlobStoreApi,
  CapabilitiesImplementedEvidenceBlobStoreApi: FakeCapabilitiesImplementedEvidenceBlobStoreApi
}

class FakeSharePointProvider extends SharePointProvider {
  constructor () {
    console.info('Mock Sharepoint provider active')
    super(FakeCatalogueAPI)
  }

  // This overrides the SharePointProvider implementation of DownloadCapEvidence.
  // When the problems with Swashbuckle and the autogenerated Swagger schema and
  // Code are resolved, this method would not need to exist, and it can be faked
  // in the same way as the other methods
  async downloadCapEvidence (claimID, uniqueId) {
    return this.capBlobStoreApi.apiCapabilitiesImplementedEvidenceBlobStoreDownloadByClaimIdPost(claimID, { uniqueId })
  }

  // This overrides the SharePointProvider implementation of DownloadCapEvidence.
  // When the problems with Swashbuckle and the autogenerated Swagger schema and
  // Code are resolved, this method would not need to exist, and it can be faked
  // in the same way as the other methods
  async downloadStdEvidence (claimID, uniqueId) {
    return this.stdBlobStoreApi.apiStandardsApplicableEvidenceBlobStoreDownloadByClaimIdPost(claimID, { uniqueId })
  }

  removeFolder () {}
}

module.exports = FakeSharePointProvider