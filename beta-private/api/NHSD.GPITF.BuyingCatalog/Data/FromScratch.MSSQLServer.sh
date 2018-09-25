#! /bin/sh
TP=/opt/mssql-tools/bin
CREDS="-S localhost -U sa -P SA@Password"
BCP=$TP/bcp
SQL=$TP/sqlcmd

$SQL $CREDS -Q 'CREATE DATABASE BuyingCatalog COLLATE Latin1_General_CI_AI'

$SQL $CREDS -Q 'CREATE LOGIN NHSD WITH PASSWORD="DisruptTheMarket", CHECK_POLICY=OFF'
$SQL $CREDS -d BuyingCatalog -Q 'CREATE USER NHSD FOR LOGIN NHSD'
$SQL $CREDS -d BuyingCatalog -Q 'GRANT CONTROL ON SCHEMA::dbo TO NHSD'

$SQL $CREDS -d BuyingCatalog -i CreateDatabase.MSSQLServer.sql

import()
{
  TABLENAME=$1
  CSVFILE=\'$2\'
  IMPORT="$BCP $TABLENAME in $CSVFILE $CREDS -d BuyingCatalog -c -t ','"
  eval "$IMPORT"
}

import Organisations 'Organisations.csv'
import Contacts 'Contacts.csv'
import Solutions 'Solutions.csv'
import TechnicalContacts 'TechnicalContacts.csv'
import Capabilities 'Capabilities.csv'
import Frameworks 'Frameworks.csv'
import Standards 'Standards.csv'
import CapabilitiesImplemented 'CapabilitiesImplemented.csv'
import StandardsApplicable 'StandardsApplicable.csv'
import CapabilityFramework 'CapabilityFramework.csv'
import FrameworkSolution 'FrameworkSolution.csv'
import FrameworkStandard 'FrameworkStandard.csv'
import CapabilityStandard 'CapabilityStandard.csv'
import CapabilitiesImplementedEvidence 'CapabilitiesImplementedEvidence.csv'
import CapabilitiesImplementedReviews 'CapabilitiesImplementedReviews.csv'
import StandardsApplicableEvidence 'StandardsApplicableEvidence.csv'
import StandardsApplicableReviews 'StandardsApplicableReviews.csv'
