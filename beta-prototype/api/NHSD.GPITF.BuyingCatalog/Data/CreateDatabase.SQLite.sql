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
  Id TEXT NOT NULL UNIQUE,
  Name TEXT NOT NULL UNIQUE,
  OdsCode TEXT NOT NULL,
  PrimaryRoleId TEXT NOT NULL DEFAULT 'RO92', 
  Status TEXT NOT NULL DEFAULT 'Active',
  Description TEXT,
  PRIMARY KEY (Id)
);

-- Contact.csv
CREATE TABLE Contact
(
  Id TEXT NOT NULL UNIQUE,
  OrganisationId TEXT NOT NULL,
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
  Id TEXT NOT NULL UNIQUE,
  OrganisationId TEXT NOT NULL,
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
  Id TEXT NOT NULL UNIQUE,
  SolutionId TEXT NOT NULL,
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
  Id TEXT NOT NULL UNIQUE,
  Name TEXT NOT NULL,
  Description TEXT,
  URL TEXT,
  PRIMARY KEY (Id)
);

-- Framework.csv
CREATE TABLE Framework
(
  Id TEXT NOT NULL UNIQUE,
  Name TEXT NOT NULL,
  Description TEXT,
  PRIMARY KEY (Id)
);

-- Standard.csv
CREATE TABLE Standard
(
  Id TEXT NOT NULL UNIQUE,
  IsOverarching INTEGER DEFAULT 0,
  Name TEXT NOT NULL,
  Description TEXT,
  URL TEXT,
  PRIMARY KEY (Id)
);

-- AssessmentMessage.csv
CREATE TABLE AssessmentMessage
(
  Id TEXT NOT NULL UNIQUE,
  SolutionId TEXT NOT NULL,
  ContactId TEXT NOT NULL,
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
  Id TEXT NOT NULL UNIQUE,
  SolutionId TEXT NOT NULL,
  CapabilityId TEXT NOT NULL,
  Evidence TEXT,
  Status INTEGER DEFAULT 0,
  FOREIGN KEY (SolutionId) REFERENCES Solution(Id) ON DELETE CASCADE,
  FOREIGN KEY (CapabilityId) REFERENCES Capability(Id) ON DELETE CASCADE,
  PRIMARY KEY (Id)
);

-- ClaimedStandard.csv
CREATE TABLE ClaimedStandard
(
  Id TEXT NOT NULL UNIQUE,
  SolutionId TEXT NOT NULL,
  StandardId TEXT NOT NULL,
  Evidence TEXT,
  Status INTEGER DEFAULT 0,
  FOREIGN KEY (SolutionId) REFERENCES Solution(Id) ON DELETE CASCADE,
  FOREIGN KEY (StandardId) REFERENCES Standard(Id) ON DELETE CASCADE,
  PRIMARY KEY (Id)
);

-- CapabilityFramework.csv
CREATE TABLE CapabilityFramework
(
  CapabilityId TEXT NOT NULL,
  FrameworkId TEXT NOT NULL,
  FOREIGN KEY (CapabilityId) REFERENCES Capability(Id) ON DELETE NO ACTION,
  FOREIGN KEY (FrameworkId) REFERENCES Framework(Id) ON DELETE NO ACTION,
  PRIMARY KEY (CapabilityId, FrameworkId)
);

-- FrameworkSolution.csv
CREATE TABLE FrameworkSolution
(
  FrameworkId TEXT NOT NULL,
  SolutionId TEXT NOT NULL,
  FOREIGN KEY (FrameworkId) REFERENCES Framework(Id) ON DELETE CASCADE,
  FOREIGN KEY (SolutionId) REFERENCES Solution(Id) ON DELETE CASCADE,
  PRIMARY KEY (FrameworkId, SolutionId)
);

-- FrameworkStandard.csv
CREATE TABLE FrameworkStandard
(
  FrameworkId TEXT NOT NULL,
  StandardId TEXT NOT NULL,
  FOREIGN KEY (FrameworkId) REFERENCES Framework(Id) ON DELETE NO ACTION,
  FOREIGN KEY (StandardId) REFERENCES Standard(Id) ON DELETE NO ACTION,
  PRIMARY KEY (FrameworkId, StandardId)
);

-- CapabilityStandard.csv
CREATE TABLE CapabilityStandard
(
  CapabilityId TEXT NOT NULL,
  StandardId TEXT NOT NULL,
  IsOptional INTEGER DEFAULT 0,
  FOREIGN KEY (CapabilityId) REFERENCES Capability(Id) ON DELETE NO ACTION,
  FOREIGN KEY (StandardId) REFERENCES Standard(Id) ON DELETE NO ACTION,
  PRIMARY KEY (CapabilityId, StandardId)
);

-- ClaimedCapabilityStandard.csv
CREATE TABLE ClaimedCapabilityStandard
(
  ClaimedCapabilityId TEXT NOT NULL,
  StandardId TEXT NOT NULL,
  Evidence TEXT,
  Status INTEGER DEFAULT 0,
  FOREIGN KEY (ClaimedCapabilityId) REFERENCES ClaimedCapability(Id) ON DELETE CASCADE,
  FOREIGN KEY (StandardId) REFERENCES Standard(Id) ON DELETE CASCADE,
  PRIMARY KEY (ClaimedCapabilityId, StandardId)
);

-- AssessmentMessageContact.csv
CREATE TABLE AssessmentMessageContact
(
  AssessmentMessageId TEXT NOT NULL,
  ContactId TEXT NOT NULL,
  FOREIGN KEY (AssessmentMessageId) REFERENCES AssessmentMessage(Id) ON DELETE CASCADE,
  FOREIGN KEY (ContactId) REFERENCES Contact(Id) ON DELETE CASCADE,
  PRIMARY KEY (AssessmentMessageId, ContactId)
);

