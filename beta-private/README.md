# Buying Catalogue Private Beta

This is Private Repository for Buying Catalogue Development during Beta phase of GP IT Futures.

## Environment Variables

over time the number of environment variables used by the system slowly grew. Here is a comprehensive list of the variables this system uses, as well as a table providing a example values and usages for each variable.

### Front-end Server

Variable | Usage | Example Values
---------|-------|--------------
NODE_ENV | Specify what environment is being used for the Node Server | `production`, `development`, `test`
BASE_URL | Base of URLs used as part of Page Requests and Redirects | `http:///localhost:3000`
API_BASE_URL | Base of URL used in backend API requests | `http://localhost:5100`
CACHE_HOST | Hostname of the deployed cache container | `cache`
OIDC_ISSUER_URL | URL of the Open ID Connect Issuer | `http://oidc-provider:9000`
OIDC_CLIENT_ID | ID  of the Open ID Connect Issuer | `open-id-provider`
OIDC_CLIENT_SECRET | Used as part of the authentication of the server and any requests | ABCDEFG1234567
SESSION_SECRET | Used to as part of the authentication of clients. Server generates a value if not set. | ABCDEFG1234567
SHAREPOINT_PROVIDER_ENV | Want to use Real SharePoint or FakePoint?  test = FakePoint, development = SharePoint | `test`, `development`
ANTIVIRUS_PROVIDER_ENV | Specifies which Antivirus provider to use. |`CLAMAV`, `DISABLED`
DEPLOYED_ENV_LABEL | Displays a label in the sticky navbar,  could display build numbers or the environment | `619`, `Local Development`, `Test Environment`

### ClamAV

Variable | Usage | Example Value
---------|-------|--------------
MAX_SCAN_SIZE | Sets the maximum amount of data that the AV will scan  | `1024M`
MAX_FILE_SIZE | Sets the maximum size of file that the AV will try to scan | `1024M`

### Backend API

Variable | Usage | Example Value
---------|-------|--------------
ASPNETCORE_ENVIRONMENT |  |
CACHE_HOST | |
OIDC_ISSUER_URL | |
OIDC_USERINFO_URL | |
OIDC_AUDIENCE | |
DATASTORE_CONNECTIONTYPE | |
DATASTORE_CONNECTIONSTRING | |
LOG_CONNECTIONSTRING | |
SHAREPOINT_BASEURL | |
SHAREPOINT_ORGANISATIONSRELATIVEURL | |
SHAREPOINT_CLIENT_ID | |
SHAREPOINT_CLIENT_SECRET | |
SHAREPOINT_LOGIN | |
SHAREPOINT_PASSWORD | |
CRM_APIURI | |
CRM_ACCESSTOKENURI | |
CRM_CLIENTID | |
CRM_CLIENTSECRET | |
USE_CRM | |
LOG_CRM | |
LOG_SHAREPOINT | |

### CRM GIF Service

Variable | Usage | Example Value
---------|-------|--------------
ASPNETCORE_ENVIRONMENT | |
GIF_AUTHORITY_URI | |
GIF_CRM_AUTHORITY | |
GIF_CRM_URL | |
GIF_AZURE_CLIENT_ID | |
GIF_ENCRYPTED_CLIENT_SECRET | |
CRM_CLIENTID | |
CRM_CLIENTSECRET | |
LOG_CRM | |
LOG_CONNECTIONSTRING | |

### Development Open ID Connect Provider

Variable | Usage | Example Value
---------|-------|--------------
USERS_FILE | A JSON File of Test Users permitted to use the Catalogue | `/config/users.json`
CONFIG_FILE | A JSON File of config settings | `/config/config.json`