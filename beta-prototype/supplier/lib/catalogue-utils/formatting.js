const _ = require('lodash')
const api = require('catalogue-api')

function formatTimestampForDisplay (ts) {
  const timestamp = new Date(ts)
  return timestamp.toLocaleTimeString('en-gb', {
    timeZone: 'Europe/London',
    hour: 'numeric',
    minute: '2-digit'
  }) + ' ' + timestamp.toLocaleDateString('en-gb', {
    timeZone: 'Europe/London',
    day: 'numeric',
    month: 'long',
    year: 'numeric'
  })
}

// given an array of message objects, enriches each with contact name,
// display timestamp and a class representing the contact's organisation
// as the message objects are updated in-place, many unrelated messages can
// be processed at once by providing an array of references
async function formatMessagesForDisplay (messages) {
  // load the contact details for the messages and embed
  const contacts = _.keyBy(
    await Promise.all(
      _.map(
        _.uniqBy(messages, 'contactId'),
        message => api.get_contact_by_id(message.contactId)
      )
    ),
    'id'
  )

  _.each(messages, message => {
    message.contact = contacts[message.contactId]
    message.originatorClass = message.contact.organisationId === api.NHS_DIGITAL_ORG_ID
                            ? 'assessment-team' : 'supplier'
    message.displayTimestamp = formatTimestampForDisplay(message.timestamp)
  })

  return messages
}

module.exports = {
  formatTimestampForDisplay,
  formatMessagesForDisplay
}
