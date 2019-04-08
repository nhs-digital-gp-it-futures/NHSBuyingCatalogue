const fetch = require('node-fetch')
const fs = require('fs')
const path = require('path')
const os = require('os')
const uuidGenerator = require('node-uuid-generator')
const INTERMEDIATE_STORAGE = process.env.UPLOAD_TEMP_FILE_STORE || os.tmpdir()

const mmm = require('mmmagic')
const Magic = mmm.Magic

const MIME_WHITELIST = [
  'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
  'application/msword',
  'application/vnd.oasis.opendocument.text',
  'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
  'application/vnd.ms-excel',
  'text/csv',
  'application/vnd.oasis.opendocument.spreadsheet',
  'application/vnd.openxmlformats-officedocument.presentationml.presentation',
  'application/vnd.openxmlformats-officedocument.presentationml.slideshow',
  'application/vnd.openxmlformats-officedocument.presentationml.presentation',
  'application/vnd.oasis.opendocument.presentation',
  'application/pdf',
  'application/vnd.ms-outlook',
  'text/plain',
  'application/rtf',
  'image/jpeg',
  'image/bmp',
  'image/png',
  'image/gif',
  'video/mp4',
  'video/x-ms-wmv',
  'video/x-msvideo',
  'application/json',
  'text/xml',
  'application/zip'
]

const MIME_EXTENSIONS = [
  '.docx',
  '.doc',
  '.odt',
  '.xlsx',
  '.xls',
  '.xlt',
  '.csv',
  '.ods',
  '.pptx',
  '.ppsx',
  '.pptx',
  '.odp',
  '.pdf',
  '.msg',
  '.txt',
  '.rtf',
  '.jpg',
  '.bmp',
  '.png',
  '.gif',
  '.mp4',
  '.wmv',
  '.avi',
  '.json',
  '.xml',
  '.zip'
]

const { antivirusProvider } = require('catalogue-antivirus')

class SharePointProvider {
  constructor (CatalogueApi, intermediateStoragePath) {
    this.CatalogueApi = CatalogueApi
    this.stdBlobStoreApi = new CatalogueApi.StandardsApplicableEvidenceBlobStoreApi()
    this.capBlobStoreApi = new CatalogueApi.CapabilitiesImplementedEvidenceBlobStoreApi()
    this.intermediateStoragePath = intermediateStoragePath || INTERMEDIATE_STORAGE
    this.uuidGenerator = uuidGenerator
    this.av = antivirusProvider

    this.TIMEOUT = 1200000
    this.capBlobStoreApi.timeout = this.TIMEOUT
    this.stdBlobStoreApi.timeout = this.TIMEOUT
  }

  getMimeArray () {
    return MIME_WHITELIST
  }

  getMimeCommaString () {
    return MIME_WHITELIST.join(', ')
  }

  getMimeExtensions () {
    return MIME_EXTENSIONS
  }

  getMimeExtensionsCommaString () {
    return MIME_EXTENSIONS.join(', ')
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

  async enumerateCapFolderFiles (solutionId, pageIndex = 1, pageSize = 9999) {
    const options = {
      pageIndex,
      pageSize
    }
    const enumeration = await this.capBlobStoreApi.apiCapabilitiesImplementedEvidenceBlobStoreEnumerateClaimFolderTreeBySolutionIdGet(
      solutionId,
      options
    )

    const claimMap = {}
    enumeration.items.forEach((item) => {
      claimMap[item.claimId] = item.blobInfos.filter((item) => !item.isFolder)
    })

    return claimMap
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
    const validMimeType = this.capValidMimeType.bind(this)
    return this.uploadEvidence(uploadMethod, validMimeType, claimID, buffer, filename, subFolder)
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

  async enumerateStdFolderFiles (solutionId, pageIndex = 1, pageSize = 9999) {
    const options = {
      pageIndex,
      pageSize
    }

    const enumeration = await this.stdBlobStoreApi.apiStandardsApplicableEvidenceBlobStoreEnumerateClaimFolderTreeBySolutionIdGet(
      solutionId,
      options
    )

    const claimMap = {}
    enumeration.items.forEach((item) => {
      claimMap[item.claimId] = item.blobInfos.filter((item) => !item.isFolder)
    })

    return claimMap
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
    const validMimeType = this.stdValidMimeType.bind(this)
    return this.uploadEvidence(uploadMethod, validMimeType, claimID, buffer, filename, subFolder)
  }

  async stdValidMimeType (fileName) {
    const res = [await this.detectMimeType(fileName)]
    const whiteList = new Set(MIME_WHITELIST) // await this.stdBlobStoreApi.apiStandardsApplicableEvidenceBlobStoreGetAllowedFileTypes()

    return res.every((type) => whiteList.has(type))
  }

  async capValidMimeType (fileName) {
    const res = [await this.detectMimeType(fileName)]
    const whiteList = new Set(MIME_WHITELIST) // await this.stdBlobStoreApi.apiStandardsApplicableEvidenceBlobStoreGetAllowedFileTypes()

    return res.every((type) => whiteList.has(type))
  }

  detectMimeType (fileName) {
    const storagePath = this.createFileStoragePath(fileName)
    const magic = new Magic(mmm.MAGIC_MIME_TYPE)

    return new Promise((resolve, reject) => {
      magic.detectFile(storagePath, (err, result) => {
        if (err) return reject(err)
        console.log('\n\n\n\n', result, '\n\n\n\n')
        return resolve(result)
      })
    })
  }

  async uploadEvidence (uploadMethod, mimeTypeChecker, claimID, buffer, filename, subFolder) {
    const options = {
      subFolder: subFolder
    }
    const fileUUID = `${filename}-${this.uuidGenerator.generate()}`
    await this.saveBuffer(buffer, fileUUID)

    try {
      const isValidMimeType = await mimeTypeChecker(fileUUID)

      if (!isValidMimeType) {
        return { err: 'Invalid File Type', isVirus: false, badMime: true }
      }

      const scanResults = await this.scanFile(fileUUID)

      if (scanResults) {
        await this.deleteFile(fileUUID)
        return { err: scanResults, isVirus: true, badMime: false }
      }
      const readStream = this.createFileReadStream(fileUUID)
      const uploadRes = await uploadMethod(claimID, readStream, filename, options)
      await this.deleteFile(fileUUID)
      return { blobId: uploadRes }
    } catch (err) {
      await this.deleteFile(fileUUID)
      throw err
    }
  }

  scanFile (fileName) {
    const stream = this.createFileReadStream(fileName)
    return this.av.scanFile(stream)
  }

  async saveBuffer (buffer, filename) {
    const storagePath = this.createFileStoragePath(filename)
    return new Promise((resolve, reject) => {
      this.writeFile(storagePath, buffer, (err) => {
        if (err) reject(err)
        resolve()
      })
    })
  }

  createFileReadStream (filename) {
    const storagePath = this.createFileStoragePath(filename)
    return this.createReadStream(storagePath)
  }

  async deleteFile (filename) {
    const storagePath = this.createFileStoragePath(filename)

    return new Promise((resolve, reject) => {
      return this.unlinkFile(storagePath, (fileErr) => {
        if (fileErr) return reject(fileErr)
        return resolve()
      })
    })
  }

  createFileStoragePath (filename, root) {
    const folderPath = root || this.intermediateStoragePath || INTERMEDIATE_STORAGE
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
