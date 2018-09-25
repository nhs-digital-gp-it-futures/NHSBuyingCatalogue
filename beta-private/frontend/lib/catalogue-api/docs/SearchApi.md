# CatalogueApi.SearchApi

All URIs are relative to *https://localhost*

Method | HTTP request | Description
------------- | ------------- | -------------
[**apiPorcelainSearchSolutionExByKeywordByKeywordGet**](SearchApi.md#apiPorcelainSearchSolutionExByKeywordByKeywordGet) | **GET** /api/porcelain/Search/SolutionExByKeyword/{keyword} | Get existing solution/s which are related to the given keyword  Keyword is not case sensitive


<a name="apiPorcelainSearchSolutionExByKeywordByKeywordGet"></a>
# **apiPorcelainSearchSolutionExByKeywordByKeywordGet**
> PaginatedListSolutionEx apiPorcelainSearchSolutionExByKeywordByKeywordGet(keyword, opts)

Get existing solution/s which are related to the given keyword  Keyword is not case sensitive

### Example
```javascript
var CatalogueApi = require('catalogue-api');
var defaultClient = CatalogueApi.ApiClient.instance;

// Configure HTTP basic authorization: basic
var basic = defaultClient.authentications['basic'];
basic.username = 'YOUR USERNAME';
basic.password = 'YOUR PASSWORD';

// Configure OAuth2 access token for authorization: oauth2
var oauth2 = defaultClient.authentications['oauth2'];
oauth2.accessToken = 'YOUR ACCESS TOKEN';

var apiInstance = new CatalogueApi.SearchApi();

var keyword = "keyword_example"; // String | keyword describing a solution or capability

var opts = { 
  'pageIndex': 56, // Number | 1-based index of page to return.  Defaults to 1
  'pageSize': 56 // Number | number of items per page.  Defaults to 20
};
apiInstance.apiPorcelainSearchSolutionExByKeywordByKeywordGet(keyword, opts).then(function(data) {
  console.log('API called successfully. Returned data: ' + data);
}, function(error) {
  console.error(error);
});

```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **keyword** | **String**| keyword describing a solution or capability | 
 **pageIndex** | **Number**| 1-based index of page to return.  Defaults to 1 | [optional] 
 **pageSize** | **Number**| number of items per page.  Defaults to 20 | [optional] 

### Return type

[**PaginatedListSolutionEx**](PaginatedListSolutionEx.md)

### Authorization

[basic](../README.md#basic), [oauth2](../README.md#oauth2)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

