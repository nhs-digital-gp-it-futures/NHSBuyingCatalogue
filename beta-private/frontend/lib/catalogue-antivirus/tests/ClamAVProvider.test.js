
const ClamAVProvider = require('../providers/ClamAVProvider')

global.console = {
    info: jest.fn(),
    log: jest.fn()
}

// --------
// Mocking the ClamAV Dependencies
// --------
const erroringScan = jest.fn((fs, cb) => {
    cb('ERROR!', null, null)
})

const maliciousScan = jest.fn((fs, cb) => {
    cb(false, null, 'VIRUS!')
})

const cleanScan = jest.fn((fs, cb) => {
    cb(false, null, false)
})

const erroringClamAV = {
    createScanner: jest.fn((port, hostname) => {
        return {scan: erroringScan}
    })
}

const maliciousClamAV = {
    createScanner: jest.fn((port, hostname) => {
        return {scan: maliciousScan}
    })
}

const cleanClamAV = {
    createScanner: jest.fn((port, hostname) => {
        return {scan: cleanScan}
    })
}

describe ('ClamAV Provider', () => {

    it ('Logs that the Clam AV provider is active on construction', () => {
        const cav = new ClamAVProvider()
        expect(global.console.info).toHaveBeenCalledWith('ClamAV Provider Active')
    })

    it ('Should create a ClamAV Scanning Object when scanning a file', async () => {
        const cav = new ClamAVProvider(cleanClamAV)
        await cav.scanFile('filestream')
        expect(cleanClamAV.createScanner).toHaveBeenCalledWith(3310, 'clamav')
    })

    it ('Should return a resolving to be nothing promise when the scan is positive', () => {
        const cav = new ClamAVProvider(cleanClamAV)
        expect(cav.scanFile('filestream')).resolves.toBe(undefined)
    })

    it ('Should return a rejecting promise when the scan is errors', () => {
        const cav = new ClamAVProvider(erroringClamAV)
        expect(cav.scanFile('filestream')).rejects.toBe('ERROR!')
    })

    it ('Should return a rejecting promise when the scan is flags a malicious file', () => {
        const cav = new ClamAVProvider(maliciousClamAV)
        expect(cav.scanFile('filestream')).resolves.toBe('VIRUS!')
    })
})