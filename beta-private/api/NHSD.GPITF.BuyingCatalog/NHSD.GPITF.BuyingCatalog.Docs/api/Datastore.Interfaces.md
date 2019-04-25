# Data Storage
Data storage is pluggable and the following implementations are supported:
* relational database
  * Microsoft SQL Server
  * MySQL
  * PostgreSQL
  * SQLite
* Microsoft Dynamics CRM

Binary data (blob) storage is also pluggable and the following implementations are supported:
* Microsoft SharePoint

# Domain Entities
  - [ICapabilitiesDatastore](NHSD.GPITF.BuyingCatalog.Interfaces.ICapabilitiesDatastore.yml)
  - [ICapabilitiesImplementedDatastore](NHSD.GPITF.BuyingCatalog.Interfaces.ICapabilitiesImplementedDatastore.yml)
  - [ICapabilitiesImplementedReviewsDatastore](NHSD.GPITF.BuyingCatalog.Interfaces.ICapabilitiesImplementedReviewsDatastore.yml)
  - [ICapabilityStandardDatastore](NHSD.GPITF.BuyingCatalog.Interfaces.ICapabilityStandardDatastore.yml)
  - [IClaimsInfoProvider](NHSD.GPITF.BuyingCatalog.Interfaces.IClaimsInfoProvider.yml)
  - [IContactsDatastore](NHSD.GPITF.BuyingCatalog.Interfaces.IContactsDatastore.yml)
  - [IFrameworksDatastore](NHSD.GPITF.BuyingCatalog.Interfaces.IFrameworksDatastore.yml)
  - [ILinkManagerDatastore](NHSD.GPITF.BuyingCatalog.Interfaces.ILinkManagerDatastore.yml)
  - [IOrganisationsDatastore](NHSD.GPITF.BuyingCatalog.Interfaces.IOrganisationsDatastore.yml)
  - [ISolutionsDatastore](NHSD.GPITF.BuyingCatalog.Interfaces.ISolutionsDatastore.yml)
  - [IStandardsApplicableDatastore](NHSD.GPITF.BuyingCatalog.Interfaces.IStandardsApplicableDatastore.yml)
  - [IStandardsApplicableReviewsDatastore](NHSD.GPITF.BuyingCatalog.Interfaces.IStandardsApplicableReviewsDatastore.yml)
  - [IStandardsDatastore](NHSD.GPITF.BuyingCatalog.Interfaces.IStandardsDatastore.yml)
  - [ITechnicalContactsDatastore](NHSD.GPITF.BuyingCatalog.Interfaces.ITechnicalContactsDatastore.yml)

# Porcelain
  - [ICapabilityMappingsDatastore](NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain.ICapabilityMappingsDatastore.yml)
  - [ISearchDatastore](NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain.ISearchDatastore.yml)
  - [ISolutionsExDatastore](NHSD.GPITF.BuyingCatalog.Interfaces.Porcelain.ISolutionsExDatastore.yml)

# Blob Storage
  - [IEvidenceBlobStoreDatastore](NHSD.GPITF.BuyingCatalog.Interfaces.IEvidenceBlobStoreDatastore.yml)
  - [ICapabilitiesImplementedEvidenceDatastore](NHSD.GPITF.BuyingCatalog.Interfaces.ICapabilitiesImplementedEvidenceDatastore.yml)
  - [IStandardsApplicableEvidenceDatastore](NHSD.GPITF.BuyingCatalog.Interfaces.IStandardsApplicableEvidenceDatastore.yml)

# Search
  - [IKeywordSearchHistoryDatastore](NHSD.GPITF.BuyingCatalog.Interfaces.IKeywordSearchHistoryDatastore.yml)

# Cache
  - [ICache](NHSD.GPITF.BuyingCatalog.Interfaces.ICache.yml)
  - [ILongTermCache](NHSD.GPITF.BuyingCatalog.Interfaces.ILongTermCache.yml)
  - [IShortTermCache](NHSD.GPITF.BuyingCatalog.Interfaces.IShortTermCache.yml)
  - [IOtherCache](NHSD.GPITF.BuyingCatalog.Interfaces.IOtherCache.yml)

# Miscellaneous
  - [ISyncPolicyFactory](NHSD.GPITF.BuyingCatalog.Interfaces.ISyncPolicyFactory.yml)

