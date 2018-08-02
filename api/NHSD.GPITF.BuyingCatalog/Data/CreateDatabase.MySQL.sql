-- assumes database collation is:
--    utf8_unicode_ci

-- drop relationship tables
DROP TABLE IF EXISTS CapabilityFramework;
DROP TABLE IF EXISTS FrameworkSolution;
DROP TABLE IF EXISTS FrameworkStandard;
DROP TABLE IF EXISTS ClaimedCapabilityStandard;
DROP TABLE IF EXISTS CapabilityStandard;
DROP TABLE IF EXISTS ClaimedCapability;
DROP TABLE IF EXISTS ClaimedStandard;
DROP TABLE IF EXISTS AssessmentMessageContact;

-- drop data tables
DROP TABLE IF EXISTS AssessmentMessage;
DROP TABLE IF EXISTS TechnicalContact;
DROP TABLE IF EXISTS Contact;
DROP TABLE IF EXISTS Solution;
DROP TABLE IF EXISTS Organisation;

DROP TABLE IF EXISTS Capability;
DROP TABLE IF EXISTS Framework;
DROP TABLE IF EXISTS Standard;

-- create data tables

-- Organisation.csv
CREATE TABLE Organisation
(
  Id CHAR(38) NOT NULL UNIQUE,
  Name VARCHAR(512) NOT NULL UNIQUE,
  OdsCode TEXT NOT NULL,
  PrimaryRoleId TEXT NOT NULL DEFAULT 'RO92', 
  Status TEXT NOT NULL DEFAULT 'Active',
  Description TEXT,
  PRIMARY KEY (Id)
);

-- Contact.csv
CREATE TABLE Contact
(
  Id CHAR(38) NOT NULL UNIQUE,
  OrganisationId CHAR(38) NOT NULL,
  FirstName TEXT,
  LastName TEXT,
  EmailAddress1 TEXT NOT NULL,
  PhoneNumber1 TEXT,
  FOREIGN KEY (OrganisationId) REFERENCES Organisation(Id) ON DELETE CASCADE,
  PRIMARY KEY (Id)
);

-- Solution.csv
CREATE TABLE Solution
(
  Id CHAR(38) NOT NULL UNIQUE,
  OrganisationId CHAR(38) NOT NULL,
  Version TEXT NOT NULL DEFAULT '',
  Status INTEGER DEFAULT 0,
  Name TEXT NOT NULL,
  Description TEXT,
  ProductPage TEXT,
  FOREIGN KEY (OrganisationId) REFERENCES Organisation(Id) ON DELETE CASCADE,
  PRIMARY KEY (Id)
);

-- TechnicalContact.csv
CREATE TABLE TechnicalContact
(
  Id CHAR(38) NOT NULL UNIQUE,
  SolutionId CHAR(38) NOT NULL,
  ContactType TEXT NOT NULL,
  FirstName TEXT,
  LastName TEXT,
  EmailAddress TEXT NOT NULL,
  PhoneNumber TEXT,
  FOREIGN KEY (SolutionId) REFERENCES Solution(Id) ON DELETE CASCADE,
  PRIMARY KEY (Id)
);

-- Capability.csv
CREATE TABLE Capability
(
  Id CHAR(38) NOT NULL UNIQUE,
  Name TEXT NOT NULL,
  Description TEXT,
  URL TEXT,
  PRIMARY KEY (Id)
);

-- Framework.csv
CREATE TABLE Framework
(
  Id CHAR(38) NOT NULL UNIQUE,
  Name TEXT NOT NULL,
  Description TEXT,
  PRIMARY KEY (Id)
);

-- Standard.csv
CREATE TABLE Standard
(
  Id CHAR(38) NOT NULL UNIQUE,
  IsOverarching INTEGER DEFAULT 0,
  Name TEXT NOT NULL,
  Description TEXT,
  URL TEXT,
  PRIMARY KEY (Id)
);

-- AssessmentMessage.csv
CREATE TABLE AssessmentMessage
(
  Id CHAR(38) NOT NULL UNIQUE,
  SolutionId CHAR(38) NOT NULL,
  ContactId CHAR(38) NOT NULL,
  Timestamp TEXT NOT NULL,
  Message TEXT,
  PRIMARY KEY (Id),
  FOREIGN KEY (SolutionId) REFERENCES Solution(Id) ON DELETE CASCADE,
  FOREIGN KEY (ContactId) REFERENCES Contact(Id) ON DELETE CASCADE
);


-- create relationship tables

-- ClaimedCapability.csv
CREATE TABLE ClaimedCapability
(
  Id CHAR(38) NOT NULL UNIQUE,
  SolutionId CHAR(38) NOT NULL,
  CapabilityId CHAR(38) NOT NULL,
  Evidence TEXT,
  Status INTEGER DEFAULT 0,
  FOREIGN KEY (SolutionId) REFERENCES Solution(Id) ON DELETE CASCADE,
  FOREIGN KEY (CapabilityId) REFERENCES Capability(Id) ON DELETE CASCADE,
  PRIMARY KEY (Id)
);

-- ClaimedStandard.csv
CREATE TABLE ClaimedStandard
(
  Id CHAR(38) NOT NULL UNIQUE,
  SolutionId CHAR(38) NOT NULL,
  StandardId CHAR(38) NOT NULL,
  Evidence TEXT,
  Status INTEGER DEFAULT 0,
  FOREIGN KEY (SolutionId) REFERENCES Solution(Id) ON DELETE CASCADE,
  FOREIGN KEY (StandardId) REFERENCES Standard(Id) ON DELETE CASCADE,
  PRIMARY KEY (Id)
);

-- CapabilityFramework.csv
CREATE TABLE CapabilityFramework
(
  CapabilityId CHAR(38) NOT NULL,
  FrameworkId CHAR(38) NOT NULL,
  FOREIGN KEY (CapabilityId) REFERENCES Capability(Id) ON DELETE NO ACTION,
  FOREIGN KEY (FrameworkId) REFERENCES Framework(Id) ON DELETE NO ACTION,
  PRIMARY KEY (CapabilityId, FrameworkId)
);

-- FrameworkSolution.csv
CREATE TABLE FrameworkSolution
(
  FrameworkId CHAR(38) NOT NULL,
  SolutionId CHAR(38) NOT NULL,
  FOREIGN KEY (FrameworkId) REFERENCES Framework(Id) ON DELETE CASCADE,
  FOREIGN KEY (SolutionId) REFERENCES Solution(Id) ON DELETE CASCADE,
  PRIMARY KEY (FrameworkId, SolutionId)
);

-- FrameworkStandard.csv
CREATE TABLE FrameworkStandard
(
  FrameworkId CHAR(38) NOT NULL,
  StandardId CHAR(38) NOT NULL,
  FOREIGN KEY (FrameworkId) REFERENCES Framework(Id) ON DELETE NO ACTION,
  FOREIGN KEY (StandardId) REFERENCES Standard(Id) ON DELETE NO ACTION,
  PRIMARY KEY (FrameworkId, StandardId)
);

-- CapabilityStandard.csv
CREATE TABLE CapabilityStandard
(
  CapabilityId CHAR(38) NOT NULL,
  StandardId CHAR(38) NOT NULL,
  IsOptional INTEGER DEFAULT 0,
  FOREIGN KEY (CapabilityId) REFERENCES Capability(Id) ON DELETE NO ACTION,
  FOREIGN KEY (StandardId) REFERENCES Standard(Id) ON DELETE NO ACTION,
  PRIMARY KEY (CapabilityId, StandardId)
);

-- ClaimedCapabilityStandard.csv
CREATE TABLE ClaimedCapabilityStandard
(
  ClaimedCapabilityId CHAR(38) NOT NULL,
  StandardId CHAR(38) NOT NULL,
  Evidence TEXT,
  Status INTEGER DEFAULT 0,
  FOREIGN KEY (ClaimedCapabilityId) REFERENCES ClaimedCapability(Id) ON DELETE CASCADE,
  FOREIGN KEY (StandardId) REFERENCES Standard(Id) ON DELETE CASCADE,
  PRIMARY KEY (ClaimedCapabilityId, StandardId)
);

-- AssessmentMessageContact.csv
CREATE TABLE AssessmentMessageContact
(
  AssessmentMessageId CHAR(38) NOT NULL,
  ContactId CHAR(38) NOT NULL,
  FOREIGN KEY (AssessmentMessageId) REFERENCES AssessmentMessage(Id) ON DELETE CASCADE,
  FOREIGN KEY (ContactId) REFERENCES Contact(Id) ON DELETE CASCADE,
  PRIMARY KEY (AssessmentMessageId, ContactId)
);
