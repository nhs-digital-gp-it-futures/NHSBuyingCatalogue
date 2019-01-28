# CatalogueApi.BlobInfo

## Properties
Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**name** | **String** | Display name | [optional] 
**isFolder** | **Boolean** | true if object is a folder | [optional] 
**length** | **Number** | size of file in bytes (zero for a folder) | [optional] 
**url** | **String** | Externally accessible URL | [optional] 
**timeLastModified** | **Date** | UTC when last modified | [optional] 
**blobId** | **String** | unique identifier of binary file in blob storage system  (null for a folder)  NOTE:  this may not be a GUID eg it may be a URL  NOTE:  this is a GUID for SharePoint | [optional] 


