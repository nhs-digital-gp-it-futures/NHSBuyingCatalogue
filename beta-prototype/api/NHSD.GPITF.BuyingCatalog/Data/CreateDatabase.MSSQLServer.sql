-- assumes database collation is:
--    Latin1_General_CI_AI

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
-- create data tables
-- NOTE:  maximum text field lengths is 425 characters because 
--        max index size (on relationship tables) is 1700 bytes (425 = 1700/4)


-- Organisation.csv
CREATE TABLE Organisation
(
  Id NVARCHAR(36) NOT NULL UNIQUE,
  Name NVARCHAR(425) NOT NULL UNIQUE,
  OdsCode NVARCHAR(MAX) NOT NULL,
  PrimaryRoleId NVARCHAR(MAX) NOT NULL DEFAULT 'RO92', 
  Status NVARCHAR(MAX) NOT NULL DEFAULT 'Active',
  Description NVARCHAR(MAX),
  PRIMARY KEY (Id)
);

-- Contact.csv
CREATE TABLE Contact
(
  Id NVARCHAR(36) NOT NULL UNIQUE,
  OrganisationId NVARCHAR(36) NOT NULL,
  FirstName NVARCHAR(MAX),
  LastName NVARCHAR(MAX),
  EmailAddress1 NVARCHAR(MAX) NOT NULL,
  PhoneNumber1 NVARCHAR(MAX),
  FOREIGN KEY (OrganisationId) REFERENCES Organisation(Id) ON DELETE CASCADE,
  PRIMARY KEY (Id)
);

-- Solution.csv
CREATE TABLE Solution
(
  Id NVARCHAR(36) NOT NULL UNIQUE,
  OrganisationId NVARCHAR(36) NOT NULL,
  Version NVARCHAR(MAX) NOT NULL DEFAULT '',
  Status INTEGER DEFAULT 0,
  Name NVARCHAR(MAX) NOT NULL,
  Description NVARCHAR(MAX),
  ProductPage NVARCHAR(MAX),
  FOREIGN KEY (OrganisationId) REFERENCES Organisation(Id) ON DELETE CASCADE,
  PRIMARY KEY (Id)
);

-- TechnicalContact.csv
CREATE TABLE TechnicalContact
(
  Id NVARCHAR(36) NOT NULL UNIQUE,
  SolutionId NVARCHAR(36) NOT NULL,
  ContactType NVARCHAR(MAX) NOT NULL,
  FirstName NVARCHAR(MAX),
  LastName NVARCHAR(MAX),
  EmailAddress NVARCHAR(MAX) NOT NULL,
  PhoneNumber NVARCHAR(MAX),
  FOREIGN KEY (SolutionId) REFERENCES Solution(Id) ON DELETE CASCADE,
  PRIMARY KEY (Id)
);

-- Capability.csv
CREATE TABLE Capability
(
  Id NVARCHAR(36) NOT NULL UNIQUE,
  Name NVARCHAR(MAX) NOT NULL,
  Description NVARCHAR(MAX),
  URL NVARCHAR(MAX),
  PRIMARY KEY (Id)
);

-- Framework.csv
CREATE TABLE Framework
(
  Id NVARCHAR(36) NOT NULL UNIQUE,
  Name NVARCHAR(MAX) NOT NULL,
  Description NVARCHAR(MAX),
  PRIMARY KEY (Id)
);

-- Standard.csv
CREATE TABLE Standard
(
  Id NVARCHAR(36) NOT NULL UNIQUE,
  IsOverarching INTEGER DEFAULT 0,
  Name NVARCHAR(MAX) NOT NULL,
  Description NVARCHAR(MAX),
  URL NVARCHAR(MAX),
  PRIMARY KEY (Id)
);

-- AssessmentMessage.csv
CREATE TABLE AssessmentMessage
(
  Id NVARCHAR(36) NOT NULL UNIQUE,
  SolutionId NVARCHAR(36) NOT NULL,
  ContactId NVARCHAR(36) NOT NULL,
  Timestamp NVARCHAR(MAX) NOT NULL,
  Message NVARCHAR(MAX),
  PRIMARY KEY (Id),
  FOREIGN KEY (SolutionId) REFERENCES Solution(Id) ON DELETE NO ACTION,
  FOREIGN KEY (ContactId) REFERENCES Contact(Id) ON DELETE NO ACTION
);


-- create relationship tables

-- ClaimedCapability.csv
CREATE TABLE ClaimedCapability
(
  Id NVARCHAR(36) NOT NULL UNIQUE,
  SolutionId NVARCHAR(36) NOT NULL,
  CapabilityId NVARCHAR(36) NOT NULL,
  Evidence NVARCHAR(MAX),
  Status INTEGER DEFAULT 0,
  FOREIGN KEY (SolutionId) REFERENCES Solution(Id) ON DELETE CASCADE,
  FOREIGN KEY (CapabilityId) REFERENCES Capability(Id) ON DELETE CASCADE,
  PRIMARY KEY (Id)
);

-- ClaimedStandard.csv
CREATE TABLE ClaimedStandard
(
  Id NVARCHAR(36) NOT NULL UNIQUE,
  SolutionId NVARCHAR(36) NOT NULL,
  StandardId NVARCHAR(36) NOT NULL,
  Evidence NVARCHAR(MAX),
  Status INTEGER DEFAULT 0,
  FOREIGN KEY (SolutionId) REFERENCES Solution(Id) ON DELETE CASCADE,
  FOREIGN KEY (StandardId) REFERENCES Standard(Id) ON DELETE CASCADE,
  PRIMARY KEY (Id)
);

-- CapabilityFramework.csv
CREATE TABLE CapabilityFramework
(
  CapabilityId NVARCHAR(36) NOT NULL,
  FrameworkId NVARCHAR(36) NOT NULL,
  FOREIGN KEY (CapabilityId) REFERENCES Capability(Id) ON DELETE NO ACTION,
  FOREIGN KEY (FrameworkId) REFERENCES Framework(Id) ON DELETE NO ACTION,
  PRIMARY KEY (CapabilityId, FrameworkId)
);

-- FrameworkSolution.csv
CREATE TABLE FrameworkSolution
(
  FrameworkId NVARCHAR(36) NOT NULL,
  SolutionId NVARCHAR(36) NOT NULL,
  FOREIGN KEY (FrameworkId) REFERENCES Framework(Id) ON DELETE CASCADE,
  FOREIGN KEY (SolutionId) REFERENCES Solution(Id) ON DELETE CASCADE,
  PRIMARY KEY (FrameworkId, SolutionId)
);

-- FrameworkStandard.csv
CREATE TABLE FrameworkStandard
(
  FrameworkId NVARCHAR(36) NOT NULL,
  StandardId NVARCHAR(36) NOT NULL,
  FOREIGN KEY (FrameworkId) REFERENCES Framework(Id) ON DELETE NO ACTION,
  FOREIGN KEY (StandardId) REFERENCES Standard(Id) ON DELETE NO ACTION,
  PRIMARY KEY (FrameworkId, StandardId)
);

-- CapabilityStandard.csv
CREATE TABLE CapabilityStandard
(
  CapabilityId NVARCHAR(36) NOT NULL,
  StandardId NVARCHAR(36) NOT NULL,
  IsOptional INTEGER DEFAULT 0,
  FOREIGN KEY (CapabilityId) REFERENCES Capability(Id) ON DELETE NO ACTION,
  FOREIGN KEY (StandardId) REFERENCES Standard(Id) ON DELETE NO ACTION,
  PRIMARY KEY (CapabilityId, StandardId)
);

-- ClaimedCapabilityStandard.csv
CREATE TABLE ClaimedCapabilityStandard
(
  ClaimedCapabilityId NVARCHAR(36) NOT NULL,
  StandardId NVARCHAR(36) NOT NULL,
  Evidence NVARCHAR(MAX),
  Status INTEGER DEFAULT 0,
  FOREIGN KEY (ClaimedCapabilityId) REFERENCES ClaimedCapability(Id) ON DELETE CASCADE,
  FOREIGN KEY (StandardId) REFERENCES Standard(Id) ON DELETE CASCADE,
  PRIMARY KEY (ClaimedCapabilityId, StandardId)
);

-- AssessmentMessageContact.csv
CREATE TABLE AssessmentMessageContact
(
  AssessmentMessageId NVARCHAR(36) NOT NULL,
  ContactId NVARCHAR(36) NOT NULL,
  FOREIGN KEY (AssessmentMessageId) REFERENCES AssessmentMessage(Id) ON DELETE CASCADE,
  FOREIGN KEY (ContactId) REFERENCES Contact(Id) ON DELETE CASCADE,
  PRIMARY KEY (AssessmentMessageId, ContactId)
);

