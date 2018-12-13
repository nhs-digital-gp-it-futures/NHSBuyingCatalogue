const fetch = require('node-fetch')
const fs = require('fs')
const path = require('path')
const os = require('os')

const INTERMEDIATE_STORAGE = process.env.UPLOAD_TEMP_FILE_STORE || os.tmpdir()

class SharePointProvider {
  constructor (CatalogueApi, intermediateStoragePath) {
    this.CatalogueApi = CatalogueApi
    this.stdBlobStoreApi = new CatalogueApi.StandardsApplicableEvidenceBlobStoreApi()
    this.capBlobStoreApi = new CatalogueApi.CapabilitiesImplementedEvidenceBlobStoreApi()
    this.intermediateStoragePath = intermediateStoragePath || INTERMEDIATE_STORAGE
  }

  async getCapEvidence (claimID, subFolder, pageIndex = 1) {
    const options = {
      pageIndex: pageIndex,
      subFolder
    }

    return this.capBlobStoreApi.apiCapabilitiesImplementedEvidenceBlobStoreEnumerateFolderByClaimIdGet(
      claimID,
      options
    )
  }

  async getCapEvidenceFolders (claimID, subFolder) {
    const enumeratedBlobs = await this.getCapEvidence(claimID, subFolder)
    const filteredBlobs = enumeratedBlobs.items.filter((blob) => blob.isFolder)
    return {
      ...enumeratedBlobs,
      items: filteredBlobs,
      pageSize: filteredBlobs.length
    }
  }

  async getCapEvidenceFiles (claimID, subFolder, pageIndex) {
    const enumeratedBlobs = await this.getCapEvidence(claimID, subFolder, pageIndex)
    const filteredBlobs = enumeratedBlobs.items.filter((blob) => !blob.isFolder)
    return {
      ...enumeratedBlobs,
      items: filteredBlobs,
      pageSize: filteredBlobs.length
    }
  }

  async uploadCapEvidence (claimID, buffer, filename, subFolder) {
    const uploadMethod = this.capBlobStoreApi.apiCapabilitiesImplementedEvidenceBlobStoreAddEvidenceForClaimPost.bind(this.capBlobStoreApi)
    return this.uploadEvidence(uploadMethod, claimID, buffer, filename, subFolder)
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
      await this.saveBuffer(buffer, filename, claimID)
      const readStream = this.createFileReadStream(filename, claimID)
      const uploadRes = await method(claimID, readStream, filename, options)
      await this.deleteFile(filename, claimID)
      return uploadRes
    } catch (err) {
      throw err
    }
  }
  async saveBuffer (buffer, filename, claimID) {
    const storagePath = this.createFileStoragePath(filename, claimID)
    return new Promise((resolve, reject) => {
      this.writeFile(storagePath, buffer, (err) => {
        if (err) reject(err)
        resolve()
      })
    })
  }

  createFileReadStream (filename, claimID) {
    const storagePath = this.createFileStoragePath(filename, claimID)
    return this.createReadStream(storagePath)
  }

  async deleteFile (filename, claimID) {
    const claimFolderPath = this.createClaimFolderPath(claimID)
    const storagePath = this.createFileStoragePath(filename, claimID)

    // Unlink the file, then unlink the folder
    return new Promise((resolve, reject) => {
      return this.unlinkFile(storagePath, (fileErr) => {
        if (fileErr) return reject(fileErr)
        return resolve()
      })
    }).then(new Promise((resolve, reject) => {
      this.removeFolder(claimFolderPath, (dirErr) => {
        if (dirErr) return reject(dirErr)
        return resolve()
      })
    }))
  }

  createFileStoragePath (filename, claimID, root) {
    const folderPath = root || this.intermediateStoragePath || INTERMEDIATE_STORAGE
    const claimFolderPath = this.createClaimFolderPath(claimID, folderPath)
    return path.join(claimFolderPath, filename)
  }

  createClaimFolderPath (claimID, root) {
    const folderPath = root || this.intermediateStoragePath || INTERMEDIATE_STORAGE
    if (!this.folderExists(folderPath)) {
      this.createFolder(folderPath)
    }

    const claimFolderPath = path.join(folderPath, claimID)
    if (!this.folderExists(claimFolderPath)) {
      this.createFolder(claimFolderPath)
    }
    return claimFolderPath
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
  async downloadStdEvidence (claimID, blobId) {
    const urlRoot = `${this.CatalogueApi.ApiClient.instance.basePath}/api/StandardsApplicableEvidenceBlobStore/Download`
    const fetchUrl = `${urlRoot}/${claimID}?uniqueId=${blobId}`
    const options = {
      method: 'post',
      headers: {
        accept: 'application/json',
        authorization: `Bearer ${this.getBearerToken()}`
      }
    }
    return this.fetchFile(fetchUrl, options)
  }

  /**
   * Download Capability Evidence
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
  async downloadCapEvidence (claimID, blobId) {
    const urlRoot = `${this.CatalogueApi.ApiClient.instance.basePath}/api/CapabilitiesImplementedEvidenceBlobStore/Download`
    const fetchUrl = `${urlRoot}/${claimID}?uniqueId=${blobId}`
    const options = {
      method: 'post',
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
  getBearerToken () {
    try {
      return this.CatalogueApi.ApiClient.instance.authentications.oauth2.accessToken
    } catch (err) {
      return ''
    }
  }

  folderExists (fp) { return fs.existsSync(fp) }
  createFolder (fp) { fs.mkdirSync(fp) }
  removeFolder (fp) { fs.rmdirSync(fp) }
  unlinkFile (fp, cb) { fs.unlink(fp, cb) }
  writeFile (fp, bf, cb) { fs.writeFile(fp, bf, cb) }
  createReadStream (fp) { return fs.createReadStream(fp) }
  async fetchFile (url, options) { return fetch(url, options) }
}

module.exports = SharePointProvider
