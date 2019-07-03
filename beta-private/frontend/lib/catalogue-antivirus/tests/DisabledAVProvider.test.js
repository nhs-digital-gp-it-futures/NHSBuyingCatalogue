
const DisabledAV = require('../providers/DisabledAVProvider')

global.console = {
    info: jest.fn(),
    log: jest.fn()
}

describe ('Disabled Antivirus Provider', () => {

    it ('Logs that the disabled AV provider is active on construction', () => {
        const dav = new DisabledAV()
        expect(global.console.info).toHaveBeenCalledWith('Disabled Antivirus Provider Active')
    })

    it ('Always returns a resolving promise.', async () => {
        const dav = new DisabledAV()
        const scanRes = dav.scanFile()
        expect(scanRes).resolves
    })
})