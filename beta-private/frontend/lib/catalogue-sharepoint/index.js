const fetch = require('node-fetch')
const fs = require('fs')
const path = require('path')

const INTERMEDIATE_STORAGE = 'temp_storage/'

class SharePointProvider {
  constructor (CatalogueApi, intermediateStoragePath) {
    this.CatalogueApi = CatalogueApi
    this.stdBlobStoreApi = new CatalogueApi.StandardsApplicableEvidenceBlobStoreApi()
    this.intermediateStoragePath = intermediateStoragePath || INTERMEDIATE_STORAGE
  }

  async getStdEvidence (claimID, subFolder, pageIndex = 1) {
    const options = {
      pageIndex: pageIndex,
      subFolder
    }

    return this.stdBlobStoreApi.apiStandardsApplicableEvidenceBlobStoreEnumerateFolderByClaimIdGet(
      claimID,
      options
    )
  }

  async getStdEvidenceFolders (claimID, subFolder) {
    const enumeratedBlobs = await this.getStdEvidence(claimID, subFolder)
    const filteredBlobs = enumeratedBlobs.items.filter((blob) => blob.isFolder)
    return {
      ...enumeratedBlobs,
      items: filteredBlobs,
      pageSize: filteredBlobs.length
    }
  }

  async getStdEvidenceFiles (claimID, subFolder, pageIndex) {
    const enumeratedBlobs = await this.getStdEvidence(claimID, subFolder, pageIndex)
    const filteredBlobs = enumeratedBlobs.items.filter((blob) => !blob.isFolder)
    return {
      ...enumeratedBlobs,
      items: filteredBlobs,
      pageSize: filteredBlobs.length
    }
  }

  async uploadStdEvidence (claimID, buffer, filename, subFolder) {
    const uploadMethod = this.stdBlobStoreApi.apiStandardsApplicableEvidenceBlobStoreAddEvidenceForClaimPost.bind(this.stdBlobStoreApi)
    return this.uploadEvidence(uploadMethod, claimID, buffer, filename, subFolder)
  }

  async uploadEvidence (method, claimID, buffer, filename, subFolder) {
    const options = {
      subFolder: subFolder
    }
    try {
      await this.saveBuffer(buffer, filename)
      const readStream = this.createFileReadStream(filename)
      const uploadRes = await method(claimID, readStream, filename, options)
      await this.deleteFile(filename)
      return uploadRes
    } catch(err) {
      throw err
    }
  }
  async saveBuffer (buffer, filename) {
    const storagePath = this.createStoragePath(filename)
    return new Promise((resolve, reject) => {
      this.writeFile(storagePath, buffer, (err) => {
        if (err) reject(err)
        resolve()
      })
    })
  }

  createFileReadStream (filename) {
    const storagePath = this.createStoragePath(filename)
    return this.createReadStream(storagePath)
  }

  async deleteFile (filename) {
    const storagePath = this.createStoragePath(filename)
    return new Promise((resolve, reject) => {
      return this.unlinkFile(storagePath, (err) => {
        if (err) reject(err)
        resolve()
      })
    })
  }

  createStoragePath (filename, root) {
    const folderPath = root || this.intermediateStoragePath || INTERMEDIATE_STORAGE
    if (!this.folderExists(folderPath)) {
      this.createFolder(folderPath)
    }
    return path.join(folderPath, filename)
  }

    /**
   * Download Standard Evidence
   *
   * This method is not using Swagger generated code to interact with the Backend web API.
   * Unfortunately this is a result of our inability to get Swashbuckle annotations to
   * produce a swagger.json schema that lets us download files effectively.
   *
   * For now, the implementation will use node-fetch to manually issue a post request to
   * the API, and pipe the response back to the client.
   *
   * It would be great if we could work out exactly what spells we need to cast so that
   * we could download files in a consistent manner.
   */
  async downloadStdEvidence (claimID, fileURL) {
    const urlRoot = `${this.CatalogueApi.ApiClient.instance.basePath}/api/StandardsApplicableEvidenceBlobStore/Download`
    const fetchUrl = `${urlRoot}/${claimID}?extUrl=${fileURL}`
    const options = {
      method:'post',
      headers: {
          accept: 'application/json',
          authorization: `Bearer ${this.getBearerToken()}`
        }
    }
    return this.fetchFile(fetchUrl, options)
  }

  /**
   * Get Bearer Token
   *
   * A result of not going through the swagger generated methods for consuming the API
   * is that you are required to set the bearer token in the request.
   *
   * In the concrete implementation of this class, the access token is set. however in
   * other instances, it might not actually be there, hence this method.
   *
   * The hope is that if the Swagger schema can be sorted out, that this method will
   * not be needed at all.
   *
   */
  getBearerToken() {
    try { return this.CatalogueApi.ApiClient.instance.authentications.oauth2.accessToken }
    catch (err) { return '' }
  }

  folderExists(fp) { return fs.existsSync(fp) }
  createFolder(fp) { fs.mkdirSync(fp) }
  unlinkFile(fp, cb) { fs.unlink(fp, cb) }
  writeFile(fp, bf, cb) { fs.writeFile(fp, bf, cb) }
  createReadStream(fp) { return fs.createReadStream(fp) }
  async fetchFile(url, options) { return fetch(url, options) }

}

// export a default, pre-configured instance of the data provider
// as well as the constructor to allow for using testing with a mock API class
class RealSharePointProvider extends SharePointProvider {
  constructor () {
    super(require('catalogue-api'))
    this.CatalogueApi.ApiClient.instance.basePath = process.env.API_BASE_URL || 'http://api:5100'
  }

  // support for the authentication layer
  setAuthenticationToken (token) {
    this.CatalogueApi.ApiClient.instance.authentications.oauth2.accessToken = token
  }
}

module.exports = {
  sharePointProvider: new RealSharePointProvider(),
  SharePointProvider
}
