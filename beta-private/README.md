# Buying Catalogue Private Beta

This is Private Repository for Buying Catalogue Development during Beta phase of GP IT Futures.

## Setup

For a brief guide for how to get started developing the Catalogue see the [DevSetup](DevSetup.md)

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
OIDC_CLIENT_SECRET | Used as part of the authentication of the server and any requests | `ABCDEFG1234567`
SESSION_SECRET | Used to as part of the authentication of clients. Server generates a value if not set. | `ABCDEFG1234567`
SHAREPOINT_PROVIDER_ENV | Want to use Real SharePoint or FakePoint?  test = FakePoint, development = SharePoint | `test`, `development`
ANTIVIRUS_PROVIDER_ENV | Specifies which Antivirus provider to use. |`CLAMAV`, `DISABLED`
DEPLOYED_ENV_LABEL | Displays a label in the sticky navbar,  could display build numbers or the environment | `619`, `Local Development`, `Test Environment`

### ClamAV

Variable | Usage | Example Value
---------|-------|--------------
MAX_SCAN_SIZE | Sets the maximum amount of data that the AV will scan | `1024M`
MAX_FILE_SIZE | Sets the maximum size of file that the AV will try to scan | `1024M`

### Backend API

Variable | Usage | Example Value
---------|-------|--------------
ASPNETCORE_ENVIRONMENT | ASP.NET Core runtime environment | `Production`, `Development`, `Test`
CACHE_HOST | Hostname of the deployed cache container | `cache`
OIDC_ISSUER_URL | OIDC issuer URL | `http://BuyingCatalog.Auth0.com`
OIDC_USERINFO_URL | OIDC user info URL | `http://BuyingCatalog.Auth0.com/userinfo`
OIDC_AUDIENCE | Registered OIDC audience | `api.buyingcatalog.co.uk`
DATASTORE_CONNECTION | Database connection for datastore | `SqlServer_Cloud`
DATASTORE_CONNECTIONTYPE | Type of database for datastore | `SQLite` or `SqlServer` or `MySql` or `PostgreSql`
DATASTORE_CONNECTIONSTRING | .NET database connection string for database datastore | `Data Source=docker.for.win.localhost;Initial Catalog=BuyingCatalog;User Id=BuyingCatalog;Password=ABCDEFG1234567;`
SHAREPOINT_BASEURL | URL to SharePoint document store root | `http://SharePoint.com/BuyingCatalog`
SHAREPOINT_ORGANISATIONSRELATIVEURL | SharePoint URL, relative to `SHAREPOINT_BASEURL`, to hold supplier documents | `Shared Documents/Buying Catalog/Supplier`
SHAREPOINT_CLIENT_ID | SharePoint add-in client ID | `ABCDEFG1234567`
SHAREPOINT_CLIENT_SECRET | SharePoint add-in client secret | `ABCDEFG1234567`
SHAREPOINT_LOGIN | SharePoint user account | BuyingCatalog@SharePoint.com
SHAREPOINT_PASSWORD | Unencrypted SharePoint user password | `ABCDEFG1234567`
SHAREPOINT_PROVIDER_FAKE | Boolean value to use dummy SharePoint filestore| `true`, `false`
CRM_APIURI | URL and port to deployed CRM container API | `http://crm:5001/api/`
CRM_ACCESSTOKENURI | URL and port to deployed CRM container bearer authentication endpoint | `http://crm:5001/connect/token`
CRM_CLIENTID | CRM client ID | `ABCDEFG1234567`
CRM_CLIENTSECRET | CRM client secret | `ABCDEFG1234567`
CRM_CACHE_EXPIRY_MINS | Cache expiry in minutes for long lived CRM data eg Standards, Capabilities | `10080` (7 days)
CRM_SHORT_TERM_CACHE_EXPIRY_SECS| Cache expiry in seconds for very short lived CRM data eg Solutions | `10` (10 seconds)
USE_CRM | Boolean value to use CRM datastore | `true`, `false`
LOG_CRM | Boolean value to record calls to GIF.Service | `true`, `false`
LOG_SHAREPOINT | Boolean value to record calls to SharePoint API | `true`, `false`
LOG_BEARERAUTH | Boolean value to record calls to bearer authentication failures | `true`, `false`
LOG_CONNECTIONSTRING | .NET database connection string for nLog database target | `Data Source=docker.for.win.localhost;Initial Catalog=BuyingCatalog;User Id=BuyingCatalog;Password=ABCDEFG1234567;`
USE_AMQP | Boolean value to use AMQP message queue, otherwise use `null` queue | `true`, `false`
AMQP_CONNECTION_STRING | AMQP connection string for message queue | `amqp://admin:admin@localhost:5672`
AMQP_TTL_MINS | How many minutes change notification should remain in queue | `10080` (7 days)

#### Notes
* enable `Development` mode by setting env var:  
&nbsp;&nbsp;&nbsp;&nbsp;  `ASPNETCORE_ENVIRONMENT`=`Development`
* SwaggerUI is only enabled in `Development` mode
* Basic authentication (username/password) is only enabled in `Development` mode
* Basic authentication is `username`=`password` eg `Admin/Admin`
* For basic authentication, `role`=`username`

### CRM GIF Service

Variable | Usage | Example Value
---------|-------|--------------
ASPNETCORE_ENVIRONMENT | ASP.NET Core runtime environment | `Production`, `Development`, `Test`
GIF_AUTHORITY_URI | localhost callback for CRM bearer authentication | `http://localhost:5001`
GIF_CRM_AUTHORITY | URL to Windows login account | `https://login.windows.net/ABCDEFG1234567`
GIF_CRM_URL | URL to top level CRM instance | `http://BuyingCatalog.dynamics.com`
GIF_AZURE_CLIENT_ID | Azure client ID | `ABCDEFG1234567`
GIF_ENCRYPTED_CLIENT_SECRET | encrypted (AES) Azure client secret | `ABCDEFG1234567`
CRM_CLIENTID | CRM client ID | `ABCDEFG1234567`
CRM_CLIENTSECRET | CRM client secret | `ABCDEFG1234567`
LOG_CRM | Boolean value to record calls to CRM API | `true`, `false`
LOG_CONNECTIONSTRING | .NET database connection string for nLog database target | `Data Source=docker.for.win.localhost;Initial Catalog=BuyingCatalog;User Id=BuyingCatalog;Password=ABCDEFG1234567;`

#### Notes
* enable `Development` mode by setting env var:  
&nbsp;&nbsp;&nbsp;&nbsp;  `ASPNETCORE_ENVIRONMENT`=`Development`
* SwaggerUI is only enabled in `Development` mode
* Basic authentication (username/password) is only enabled in `Development` mode
* Basic authentication is `username`=`password` eg `Admin/Admin`
* For basic authentication, `role`=`username`

### Development Open ID Connect Provider

Variable | Usage | Example Value
---------|-------|--------------
USERS_FILE | A JSON File of Test Users permitted to use the Catalogue | `/config/users.json`
CONFIG_FILE | A JSON File of config settings | `/config/config.json`