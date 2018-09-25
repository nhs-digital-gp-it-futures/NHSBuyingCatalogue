-- drop relationship tables
DROP TABLE IF EXISTS CapabilityFramework;
DROP TABLE IF EXISTS FrameworkSolution;
DROP TABLE IF EXISTS FrameworkStandard;
DROP TABLE IF EXISTS CapabilityStandard;
DROP TABLE IF EXISTS CapabilitiesImplemented;
DROP TABLE IF EXISTS StandardsApplicable;
DROP TABLE IF EXISTS CapabilitiesImplementedEvidence;
DROP TABLE IF EXISTS CapabilitiesImplementedReviews;
DROP TABLE IF EXISTS StandardsApplicableEvidence;
DROP TABLE IF EXISTS StandardsApplicableReviews;

-- drop data tables
DROP TABLE IF EXISTS TechnicalContacts;
DROP TABLE IF EXISTS Contacts;
DROP TABLE IF EXISTS Solutions;
DROP TABLE IF EXISTS Organisations;

DROP TABLE IF EXISTS Capabilities;
DROP TABLE IF EXISTS Frameworks;
DROP TABLE IF EXISTS Standards;

DROP TABLE IF EXISTS CachedUserInfoResponseJson;

-- create data tables

-- Organisations.csv
CREATE TABLE Organisations
(
  Id TEXT NOT NULL UNIQUE,
  Name TEXT NOT NULL UNIQUE,
  OdsCode TEXT NOT NULL,
  PrimaryRoleId TEXT NOT NULL DEFAULT 'RO92', 
  Status TEXT NOT NULL DEFAULT 'Active',
  Description TEXT,
  PRIMARY KEY (Id)
);

-- Contacts.csv
CREATE TABLE Contacts
(
  Id TEXT NOT NULL UNIQUE,
  OrganisationId TEXT NOT NULL,
  FirstName TEXT,
  LastName TEXT,
  EmailAddress1 TEXT NOT NULL UNIQUE COLLATE NOCASE,
  PhoneNumber1 TEXT,
  FOREIGN KEY (OrganisationId) REFERENCES Organisations(Id) ON DELETE CASCADE,
  PRIMARY KEY (Id)
);

-- Solutions.csv
CREATE TABLE Solutions
(
  Id TEXT NOT NULL UNIQUE,
  PreviousId TEXT,
  OrganisationId TEXT NOT NULL,
  Version TEXT NOT NULL DEFAULT '',
  Status INTEGER DEFAULT 0,
  CreatedById TEXT NOT NULL,
  CreatedOn TEXT NOT NULL,
  ModifiedById TEXT NOT NULL,
  ModifiedOn TEXT NOT NULL,
  Name TEXT NOT NULL,
  Description TEXT,
  ProductPage TEXT,
  FOREIGN KEY (PreviousId) REFERENCES Solutions(Id),
  FOREIGN KEY (OrganisationId) REFERENCES Organisations(Id) ON DELETE CASCADE,
  FOREIGN KEY (CreatedById) REFERENCES Contacts(Id) ON DELETE CASCADE,
  FOREIGN KEY (ModifiedById) REFERENCES Contacts(Id) ON DELETE CASCADE,
  PRIMARY KEY (Id)
);

-- TechnicalContacts.csv
CREATE TABLE TechnicalContacts
(
  Id TEXT NOT NULL UNIQUE,
  SolutionId TEXT NOT NULL,
  ContactType TEXT NOT NULL,
  FirstName TEXT,
  LastName TEXT,
  EmailAddress TEXT NOT NULL,
  PhoneNumber TEXT,
  FOREIGN KEY (SolutionId) REFERENCES Solutions(Id) ON DELETE CASCADE,
  PRIMARY KEY (Id)
);

-- Capabilities.csv
CREATE TABLE Capabilities
(
  Id TEXT NOT NULL UNIQUE,
  PreviousId TEXT,
  Name TEXT NOT NULL,
  Description TEXT,
  URL TEXT,
  PRIMARY KEY (Id),
  FOREIGN KEY (PreviousId) REFERENCES Capabilities(Id)
);

-- Frameworks.csv
CREATE TABLE Frameworks
(
  Id TEXT NOT NULL UNIQUE,
  PreviousId TEXT,
  Name TEXT NOT NULL,
  Description TEXT,
  PRIMARY KEY (Id),
  FOREIGN KEY (PreviousId) REFERENCES Frameworks(Id)
);

-- Standards.csv
CREATE TABLE Standards
(
  Id TEXT NOT NULL UNIQUE,
  PreviousId TEXT,
  IsOverarching INTEGER DEFAULT 0,
  Name TEXT NOT NULL,
  Description TEXT,
  URL TEXT,
  PRIMARY KEY (Id),
  FOREIGN KEY (PreviousId) REFERENCES Standards(Id)
);

CREATE TABLE CachedUserInfoResponseJson
(
  Id TEXT NOT NULL UNIQUE,
  BearerToken TEXT NOT NULL,
  Data TEXT,
  PRIMARY KEY (Id)
);


-- create relationship tables

-- CapabilitiesImplemented.csv
CREATE TABLE CapabilitiesImplemented
(
  Id TEXT NOT NULL UNIQUE,
  SolutionId TEXT NOT NULL,
  CapabilityId TEXT NOT NULL,
  Status INTEGER DEFAULT 0,
  FOREIGN KEY (SolutionId) REFERENCES Solutions(Id) ON DELETE CASCADE,
  FOREIGN KEY (CapabilityId) REFERENCES Capabilities(Id) ON DELETE CASCADE,
  PRIMARY KEY (Id)
);

-- StandardsApplicable.csv
CREATE TABLE StandardsApplicable
(
  Id TEXT NOT NULL UNIQUE,
  SolutionId TEXT NOT NULL,
  StandardId TEXT NOT NULL,
  Status INTEGER DEFAULT 0,
  FOREIGN KEY (SolutionId) REFERENCES Solutions(Id) ON DELETE CASCADE,
  FOREIGN KEY (StandardId) REFERENCES Standards(Id) ON DELETE CASCADE,
  PRIMARY KEY (Id)
);

-- CapabilityFramework.csv
CREATE TABLE CapabilityFramework
(
  CapabilityId TEXT NOT NULL,
  FrameworkId TEXT NOT NULL,
  FOREIGN KEY (CapabilityId) REFERENCES Capabilities(Id) ON DELETE NO ACTION,
  FOREIGN KEY (FrameworkId) REFERENCES Frameworks(Id) ON DELETE NO ACTION,
  PRIMARY KEY (CapabilityId, FrameworkId)
);

-- FrameworkSolution.csv
CREATE TABLE FrameworkSolution
(
  FrameworkId TEXT NOT NULL,
  SolutionId TEXT NOT NULL,
  FOREIGN KEY (FrameworkId) REFERENCES Frameworks(Id) ON DELETE CASCADE,
  FOREIGN KEY (SolutionId) REFERENCES Solutions(Id) ON DELETE CASCADE,
  PRIMARY KEY (FrameworkId, SolutionId)
);

-- FrameworkStandard.csv
CREATE TABLE FrameworkStandard
(
  FrameworkId TEXT NOT NULL,
  StandardId TEXT NOT NULL,
  FOREIGN KEY (FrameworkId) REFERENCES Frameworks(Id) ON DELETE NO ACTION,
  FOREIGN KEY (StandardId) REFERENCES Standards(Id) ON DELETE NO ACTION,
  PRIMARY KEY (FrameworkId, StandardId)
);

-- CapabilityStandard.csv
CREATE TABLE CapabilityStandard
(
  CapabilityId TEXT NOT NULL,
  StandardId TEXT NOT NULL,
  IsOptional INTEGER DEFAULT 0,
  FOREIGN KEY (CapabilityId) REFERENCES Capabilities(Id) ON DELETE NO ACTION,
  FOREIGN KEY (StandardId) REFERENCES Standards(Id) ON DELETE NO ACTION,
  PRIMARY KEY (CapabilityId, StandardId)
);

-- CapabilitiesImplementedEvidence.csv
CREATE TABLE CapabilitiesImplementedEvidence
(
  Id TEXT NOT NULL UNIQUE,
  PreviousId TEXT,
  ClaimId TEXT NOT NULL,
  CreatedById TEXT NOT NULL,
  CreatedOn TEXT NOT NULL,
  Evidence TEXT,
  FOREIGN KEY (PreviousId) REFERENCES CapabilitiesImplementedEvidence(Id),
  FOREIGN KEY (ClaimId) REFERENCES CapabilitiesImplemented(Id) ON DELETE CASCADE,
  FOREIGN KEY (CreatedById) REFERENCES Contacts(Id) ON DELETE CASCADE,
  PRIMARY KEY (Id)
);

-- CapabilitiesImplementedReviews.csv
CREATE TABLE CapabilitiesImplementedReviews
(
  Id TEXT NOT NULL UNIQUE,
  PreviousId TEXT,
  EvidenceId TEXT NOT NULL,
  CreatedById TEXT NOT NULL,
  CreatedOn TEXT NOT NULL,
  Message TEXT,
  FOREIGN KEY (PreviousId) REFERENCES CapabilitiesImplementedReviews(Id),
  FOREIGN KEY (EvidenceId) REFERENCES CapabilitiesImplementedEvidence(Id) ON DELETE CASCADE,
  FOREIGN KEY (CreatedById) REFERENCES Contacts(Id) ON DELETE CASCADE,
  PRIMARY KEY (Id)
);

-- StandardsApplicableEvidence.csv
CREATE TABLE StandardsApplicableEvidence
(
  Id TEXT NOT NULL UNIQUE,
  PreviousId TEXT,
  ClaimId TEXT NOT NULL,
  CreatedById TEXT NOT NULL,
  CreatedOn TEXT NOT NULL,
  Evidence TEXT,
  FOREIGN KEY (PreviousId) REFERENCES StandardsApplicableEvidence(Id),
  FOREIGN KEY (ClaimId) REFERENCES StandardsApplicable(Id) ON DELETE CASCADE,
  FOREIGN KEY (CreatedById) REFERENCES Contacts(Id) ON DELETE CASCADE,
  PRIMARY KEY (Id)
);

-- StandardsApplicableReviews.csv
CREATE TABLE StandardsApplicableReviews
(
  Id TEXT NOT NULL UNIQUE,
  PreviousId TEXT,
  EvidenceId TEXT NOT NULL,
  CreatedById TEXT NOT NULL,
  CreatedOn TEXT NOT NULL,
  Message TEXT,
  FOREIGN KEY (PreviousId) REFERENCES StandardsApplicableReviews(Id),
  FOREIGN KEY (EvidenceId) REFERENCES StandardsApplicableEvidence(Id) ON DELETE CASCADE,
  FOREIGN KEY (CreatedById) REFERENCES Contacts(Id) ON DELETE CASCADE,
  PRIMARY KEY (Id)
);

