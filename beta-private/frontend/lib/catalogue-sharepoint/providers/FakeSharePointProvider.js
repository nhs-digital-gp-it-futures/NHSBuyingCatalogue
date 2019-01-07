const SharePointProvider = require('./SharePointProvider')
const stream = require('stream')

class FakeFileStoreAPI {
  constructor () {
    this.folders = {}

    this.hasInitialised = false
    this.initialiseTestData().then(() => {
      this.hasInitialised = true // So you can check if the API has finished initialising
    })
  }

  async initialiseTestData () {
    /**
     * Regression Test Data
     *
     * Since Fakepoint is intended to serve as a tool for use in regression test suites,
     * we are populating it with a little bit of test data for standards compliance.
     * This means that standards can have a traceability matrix present.
     */
    const dataMigrationClaimID = 'f49f91de-64fc-4cca-88bf-3238fb1de69b'
    const commercialClaimID = '3a7735f2-759d-4f49-bca0-0828f32cf86c'
    const clinicalSafetyClaimID = '7b62b29a-62a7-4b4a-bcc8-dfa65fb7e35c'
    const testingClaimID = '99619bdd-6452-4850-9244-a4ce9bec70ca'
    const interoperabilityStandard = '0e55d9ec-43e6-41b3-bcac-d8681384ea68'
    const businessContinuityClaimID = '719722d0-2354-437e-acdc-4625989bbca8'

    const fakeBuffer = Buffer.from('Content of the test "Dummy traceabilityMatrix.xlsx" file', 'utf8')
    const fakeName = 'Dummy TraceabilityMatrix.xlsx'

    await this.addTestItem(dataMigrationClaimID, fakeBuffer, fakeName)
    await this.addTestItem(commercialClaimID, fakeBuffer, fakeName)
    await this.addTestItem(clinicalSafetyClaimID, fakeBuffer, fakeName)
    await this.addTestItem(testingClaimID, fakeBuffer, fakeName)
    await this.addTestItem(interoperabilityStandard, fakeBuffer, fakeName)
    await this.addTestItem(businessContinuityClaimID, fakeBuffer, fakeName)
  }

  addTestItem (claimID, buffer, filename) {
    if (!this.folders[claimID]) {
      this.initialiseClaim(claimID)
    }

    const guid = require('node-uuid-generator').generate()

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
