/* eslint-env jest */

const { SharePointProvider } = require('./index')

function MockCapBlobStoreApi () {
  this.getCapEvidence = jest.fn()
}

function MockStdBlobStoreApi () {
  this.getStdEvidence = jest.fn()
}

let subject

beforeAll(() => {

  subject = new SharePointProvider({
    CapabilitiesImplementedEvidenceBlobStoreApi: MockCapBlobStoreApi,
    StandardsApplicableEvidenceBlobStoreApi: MockStdBlobStoreApi
  })
})

describe('createClaimFolderPath', () => {
  beforeEach(() => {
    subject.folderExists = jest.fn()
    subject.createFolder = jest.fn()
  })

  it('Should not try and create a directory if it already exists', () => {
    subject.folderExists.mockReturnValue(true)

    subject.createClaimFolderPath('name_of_the_file')

    expect(subject.folderExists).toBeCalled()
    expect(subject.createFolder.mock.calls.length).toBe(0)
  })

  it('Should attempt to create a direectory if it doest already exist', () => {
    subject.folderExists.mockReturnValue(false)

    subject.createClaimFolderPath('name_of_the_file')

    expect(subject.folderExists).toBeCalled()
    expect(subject.createFolder).toBeCalled()
  })

  it('Should provide a folder path during folder creation process', () => {
    subject.folderExists.mockReturnValue(false)
    const fp = subject.intermediateStoragePath

    subject.createClaimFolderPath('name_of_the_file')

    expect(subject.folderExists).toBeCalledWith(fp)
    expect(subject.createFolder).toBeCalledWith(fp)
  })

  it('Should throw if folder doesnt exist and createFolder fails', () => {
    subject.folderExists.mockReturnValue(false)
    subject.folderExists.mockImplementation(() => {
      throw new Error()
    })

    let hasThrown
    try{
      subject.createClaimFolderPath('name_of_the_file')
    }
    catch(err) {
      hasThrown = true
    }

    expect(hasThrown).toBeTruthy()
  })

  it('should return a path if no root is provided', () => {
  
    const root = 'foo'
    subject.intermediateStoragePath = root
    const fn = 'test_file'

    const fp = subject.createClaimFolderPath(fn)

    expect(fp).toBe(`${root}/${fn}`)
  })

  it('should return a path if with the root if provided', () => {
    const root = 'foo'
    const fn = 'test_file'

    const fp = subject.createClaimFolderPath(fn, root)

    expect(fp).toBe(`${root}/${fn}`)
  })
})

describe('deleteFile', () => {
  const fn = 'test_file.txt'
  const claim = '1234'

  beforeEach(() => {
    subject.createStoragePath = jest.fn()
    subject.unlinkFile = jest.fn()
    subject.createFileStoragePath = jest.fn()
    subject.createClaimFolderPath = jest.fn()

    subject.createFileStoragePath.mockImplementation(() => {
      return claim + '/' + fn
    })
    subject.createClaimFolderPath.mockImplementation(() => {
      return claim
    })
  })

  it('should try and create the storage paths with filename and claim.', () => {
    subject.deleteFile(fn, claim)

    expect(subject.createClaimFolderPath).toBeCalledWith(claim)
    expect(subject.createFileStoragePath).toBeCalledWith(fn, claim)
  })

  it('should try and unlink the a file at the specified storage path', () => {
    subject.deleteFile(fn, claim)
    const expectedPath = claim + '/' + fn
    expect(subject.unlinkFile).toBeCalledWith(expectedPath, expect.any(Function))
  })

  it('Should return a promise that resolves if file successfully unlinks', async () => {
    subject.unlinkFile.mockImplementation((fp, cb) => {
      cb(/*No Errors*/)
    })
    await expect(subject.deleteFile(fn, claim)).resolves.toBe(undefined)
  })

  it('Should return a promise that rejects if file failes to unlinks', async () => {
    subject.unlinkFile.mockImplementation((fp, cb) => {
      cb('test_error_message')
    })
    await expect(subject.deleteFile(fn, claim)).rejects.toBe('test_error_message')
  })
})

describe('createFileReadStream', () => {
  const fn = 'file'
  const claim = '1234'
  const claimFilePath = claim + '/' + fn

  beforeEach(() => {
    subject.createReadStream = jest.fn()
    subject.createFileStoragePath = jest.fn()

    subject.createFileStoragePath.mockImplementation(() => {
      return claimFilePath
    })
  })

  it('Should try and create storage path', () => {
    subject.createFileReadStream(fn, claim)
    expect(subject.createFileStoragePath).toBeCalledWith(fn, claim)
  })

  it('Should try and create a fileReadStream', () => {
    subject.createFileReadStream(fn, claim)
    expect(subject.createReadStream).toBeCalledWith(claimFilePath)
  })

  it('Should throw if creation of storage path throws', () => {
    subject.createFileStoragePath.mockImplementation(() => {
      throw new Error()
    })
    let throws
    try {
      subject.createFileReadStream(fn, claim)
    }
    catch(err) {
      throws = true
    }
    expect(throws).toBeTruthy()
  })

  it('Should throw if creation of file read stream throws', () => {
    subject.createFileStoragePath.mockImplementation(() => {
      throw new Error()
    })
    let throws
    try {
      subject.createFileReadStream(fn)
    }
    catch(err) {
      throws = true
    }
    expect(throws).toBeTruthy()
  })
})

describe('saveBuffer', () => {
  const fn = 'test_file.txt'
  const claim = '1234'
  const claimFilePath = claim + '/' + fn
  const buffer = Buffer.from('test string', 'utf-8')

  beforeEach(() => {
    subject.createStoragePath = jest.fn()
    subject.writeFile = jest.fn()

    subject.createFileStoragePath.mockImplementation(() => {
      return claimFilePath
    })
  })

  it('Should try and create storage path before writing to it.', () => {
    subject.saveBuffer(buffer, fn, claim)
    expect(subject.createFileStoragePath).toBeCalledWith(fn, claim)
  })

  it('should try and write a file providing storage path, buffer, and a callback', () => {
    subject.saveBuffer(buffer, fn, claim)
    expect(subject.writeFile).toBeCalledWith(claimFilePath, buffer, expect.any(Function))
  })

  it('Should return a resolvable promise if file writing is successful', async () => {
    subject.writeFile.mockImplementation((sp, bf, cb) => {
      cb(/*No Error*/)
    })

    await expect(subject.saveBuffer(buffer, fn, claim)).resolves.toBe(undefined)
  })

  it('Should return a rejecting promise if file writing is successful', async () => {
    subject.writeFile.mockImplementation((sp, bf, cb) => {
      cb('test_error_message')
    })

    await expect(subject.saveBuffer(buffer, fn, claim)).rejects.toBe('test_error_message')
  })
})

describe('uploadEvidence', () => {
  const mm = jest.fn()
  const cm = 'test_claim'
  const bf = Buffer.from('test_file')
  const fn = 'test_file.txt'
  const sb = '/test_sub/'
  const rs = 'test_read_stream'

  beforeEach(() => {
    subject.saveBuffer = jest.fn()
    subject.deleteFile = jest.fn()
    subject.createFileReadStream = jest.fn()
  })

  it('Should save buffer to file forwarding it on', async () => {
    await subject.uploadEvidence(mm, cm, bf, fn, sb)
    expect(subject.saveBuffer).toBeCalledWith(bf, fn, cm)
  })

  it('Should create a file read stream', async () => {
    await subject.uploadEvidence(mm, cm, bf, fn, sb)
    expect(subject.createFileReadStream).toBeCalledWith(fn, cm)
  })

  it('Should call the proided method with params needed for posting file data', async () => {
    const mockMethod = jest.fn()
    mockMethod.mockImplementation((cm, rs, fn, op) => {
      return fn
    })
    subject.createFileReadStream.mockImplementation(() => {
      return rs
    })
    const options = { subFolder: sb }

    await subject.uploadEvidence(mockMethod, cm, bf, fn, sb)
    expect(mockMethod).toBeCalledWith(cm, rs, fn, options)
  })

  it('should call the file delete method after it has uploaded a response', async () => {
    await subject.uploadEvidence(mm, cm, bf, fn, sb)
    expect(subject.deleteFile).toBeCalledWith(fn, cm)
  })

  it('should return a rejecting promise with error message if saving buffer fails', async () => {
    subject.saveBuffer.mockImplementation(() => {
      throw 'error'
    })
    await expect(subject.uploadEvidence(mm, cm, bf, fn, sb)).rejects.toBe('error')
  })

  it('should return a rejecting promise with error message if creating a readStream fails', async () => {
    subject.createFileReadStream.mockImplementation(() => {
      throw 'error'
    })
    await expect(subject.uploadEvidence(mm, cm, bf, fn, sb)).rejects.toBe('error')
  })

  it('should return a rejecting promise with error message if provided method fails', async () => {
    const mockMethod = jest.fn()
    mockMethod.mockImplementation(() => {
      throw 'error'
    })
    await expect(subject.uploadEvidence(mockMethod, cm, bf, fn, sb)).rejects.toBe('error')
  })

  it('should return a rejecting Promise with error message if deleting file fails', async () => {
    subject.deleteFile.mockImplementation(() => {
      throw 'error'
    })
    await expect(subject.uploadEvidence(mm, cm, bf, fn, sb)).rejects.toBe('error')
  })

  it('Should return a resolving promise with the result from the provided method', async () => {
    const mockMethod = jest.fn()
    mockMethod.mockImplementation(() => {
      return fn
    })
    await expect(subject.uploadEvidence(mockMethod, cm, bf, fn, sb)).resolves.toBe(fn)
  })
})

describe('uploadStdEvidence', () => {
  const cm = 'claim'
  const bf = Buffer.from('test buffer')
  const fn = 'test_file.txt'
  const sb = '/sub_folder/'

  beforeEach(() => {
    subject.stdBlobStoreApi.apiStandardsApplicableEvidenceBlobStoreAddEvidenceForClaimPost = jest.fn()
    subject.uploadEvidence = jest.fn()
  })

  it('Should provide the standards upload method to the upload evidence method', async () => {
    await subject.uploadStdEvidence(cm, bf, fn, sb)
    expect(subject.uploadEvidence).toBeCalledWith(expect.any(Function), cm, bf, fn, sb)

    // stringified for comparison
    const firstParam = JSON.stringify(subject.uploadEvidence.mock.calls[0][0])
    const expectedParam = JSON.stringify(
      subject.stdBlobStoreApi.apiStandardsApplicableEvidenceBlobStoreAddEvidenceForClaimPost.bind(subject.stdBlobStoreApi)
    )
    // First param
    expect(firstParam).toEqual(expectedParam)
  })

  it('Should return a rejecting promise if invoked methods fail', async () => {
    subject.uploadEvidence.mockImplementation(() => {
      return Promise.reject('error')
    })

    await expect(subject.uploadStdEvidence(cm, bf, fn, sb)).rejects.toBe('error')
  })

  it('Should return a resolving promise if no internal invokations fail', async () => {
    subject.uploadEvidence.mockImplementation(() => {
      return Promise.resolve()
    })

    await expect(subject.uploadStdEvidence(cm, bf, fn, sb)).resolves.toBe(undefined)
  })
})

describe('getCapEvidence', () => {
  it('Should recieve evidence listings for a capability claim with evidence against it', async () => {
    subject.capBlobStoreApi.getCapEvidence.mockReturnValue(
      require('./sharepoint-cap.test-data.json')
    )

    const res = await subject.capBlobStoreApi.getCapEvidence()
    expect(res.items).toHaveLength(6)
  })

  it('Evidence items should all have a name, an isFolderFlag, a URL, and a timestamp', async () => {
    subject.capBlobStoreApi.getCapEvidence.mockReturnValue(
      require('./sharepoint-cap.test-data.json')
    )

    const res = await subject.capBlobStoreApi.getCapEvidence()
    expect(res.items.every((item) =>
      item.name && item.isFolder && item.url && item.timeLastModified
    ))
  })
})

describe('uploadCapEvidence', () => {
  const cm = 'claim'
  const bf = Buffer.from('test buffer')
  const fn = 'test_file.txt'
  const sb = '/sub_folder/'

  beforeEach(() => {
    subject.capBlobStoreApi.apiCapabilitiesImplementedEvidenceBlobStoreAddEvidenceForClaimPost = jest.fn()
    subject.uploadEvidence = jest.fn()
  })

  it('Should provide the capability upload method to the upload evidence method', async () => {
    await subject.uploadCapEvidence(cm, bf, fn, sb)
    expect(subject.uploadEvidence).toBeCalledWith(expect.any(Function), cm, bf, fn, sb)

    // stringified for comparison
    const firstParam = JSON.stringify(subject.uploadEvidence.mock.calls[0][0])
    const expectedParam = JSON.stringify(
      subject.capBlobStoreApi.apiCapabilitiesImplementedEvidenceBlobStoreAddEvidenceForClaimPost.bind(subject.capBlobStoreApi)
    )
    // First param
    expect(firstParam).toEqual(expectedParam)
  })

  it('Should return a rejecting promise if invoked methods fail', async () => {
    subject.uploadEvidence.mockImplementation(() => {
      return Promise.reject('error')
    })

    await expect(subject.uploadCapEvidence(cm, bf, fn, sb)).rejects.toBe('error')
  })

  it('Should return a resolving promise if no internal invokations fail', async () => {
    subject.uploadEvidence.mockImplementation(() => {
      return Promise.resolve()
    })

    await expect(subject.uploadCapEvidence(cm, bf, fn, sb)).resolves.toBe(undefined)
  })
})