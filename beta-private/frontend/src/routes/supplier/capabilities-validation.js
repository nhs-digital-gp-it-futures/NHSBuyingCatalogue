const _ = require('lodash')

module.exports = {
  'capabilities': {
    'uploading-video-evidence': {
      'in':'body',
      'custom': {
        'options' : (value, { req }) => {
          // Want to ensure that if there has been any indication that a capability is
          // to be assessed with a file upload, that there has been a file uploaded
          // either in this submission, or in a previous submision.
          const files = {}

          req.files.forEach((file) => {
            // Parsing the file fieldnames not parsed by multer
            const claimID = file.fieldname.replace('evidence-file[', '').replace(']', '')
            files[claimID] = {
              ...file,
              fieldname: claimID
            }
          })

          // filter all claims that don't require files, and don't have a file uploaded
          const previousUploads = req.body['evidence-file']
          const claimsRequiringFiles = _.keys(_.omitBy(value, (val) => val !== 'yes'))

          const claimsWithFiles = _.filter(claimsRequiringFiles, (claim) => {
            return !_.isEmpty(files[claim]) || !_.isEmpty(previousUploads[claim])
          })

          const numberOfClaimsRequiringFiles = claimsRequiringFiles.length
          const numberOfClaimsWithFiles = claimsWithFiles.length

          // Check that all claims with files have a none false description
          const evidenceDescriptions = req.body['evidence-description']
          const allFilesHaveDescriptions = _.every(claimsWithFiles, (claim) => !_.isEmpty(evidenceDescriptions[claim]))

          if (numberOfClaimsRequiringFiles !== numberOfClaimsWithFiles) {
            return Promise.reject('Validation.Capability.Evidence.File.Missing')
          } else if (!allFilesHaveDescriptions) {
            return Promise.reject('Validation.Capability.Evidence.Description.Missing')
          } else {
            return Promise.resolve()
          }
        }
      }
    }
  }
}