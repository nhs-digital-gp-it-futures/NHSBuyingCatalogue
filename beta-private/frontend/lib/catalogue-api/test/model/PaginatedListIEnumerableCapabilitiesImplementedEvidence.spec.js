/**
 * catalogue-api
 * NHS Digital GP IT Futures Buying Catalog API
 *
 * OpenAPI spec version: 1.0.0-private-beta
 *
 * NOTE: This class is auto generated by the swagger code generator program.
 * https://github.com/swagger-api/swagger-codegen.git
 *
 * Swagger Codegen version: 2.4.2-SNAPSHOT
 *
 * Do not edit the class manually.
 *
 */

(function(root, factory) {
  if (typeof define === 'function' && define.amd) {
    // AMD.
    define(['expect.js', '../../src/index'], factory);
  } else if (typeof module === 'object' && module.exports) {
    // CommonJS-like environments that support module.exports, like Node.
    factory(require('expect.js'), require('../../src/index'));
  } else {
    // Browser globals (root is window)
    factory(root.expect, root.CatalogueApi);
  }
}(this, function(expect, CatalogueApi) {
  'use strict';

  var instance;

  beforeEach(function() {
    instance = new CatalogueApi.PaginatedListIEnumerableCapabilitiesImplementedEvidence();
  });

  var getProperty = function(object, getter, property) {
    // Use getter method if present; otherwise, get the property directly.
    if (typeof object[getter] === 'function')
      return object[getter]();
    else
      return object[property];
  }

  var setProperty = function(object, setter, property, value) {
    // Use setter method if present; otherwise, set the property directly.
    if (typeof object[setter] === 'function')
      object[setter](value);
    else
      object[property] = value;
  }

  describe('PaginatedListIEnumerableCapabilitiesImplementedEvidence', function() {
    it('should create an instance of PaginatedListIEnumerableCapabilitiesImplementedEvidence', function() {
      // uncomment below and update the code to test PaginatedListIEnumerableCapabilitiesImplementedEvidence
      //var instance = new CatalogueApi.PaginatedListIEnumerableCapabilitiesImplementedEvidence();
      //expect(instance).to.be.a(CatalogueApi.PaginatedListIEnumerableCapabilitiesImplementedEvidence);
    });

    it('should have the property pageIndex (base name: "pageIndex")', function() {
      // uncomment below and update the code to test the property pageIndex
      //var instance = new CatalogueApi.PaginatedListIEnumerableCapabilitiesImplementedEvidence();
      //expect(instance).to.be();
    });

    it('should have the property totalPages (base name: "totalPages")', function() {
      // uncomment below and update the code to test the property totalPages
      //var instance = new CatalogueApi.PaginatedListIEnumerableCapabilitiesImplementedEvidence();
      //expect(instance).to.be();
    });

    it('should have the property pageSize (base name: "pageSize")', function() {
      // uncomment below and update the code to test the property pageSize
      //var instance = new CatalogueApi.PaginatedListIEnumerableCapabilitiesImplementedEvidence();
      //expect(instance).to.be();
    });

    it('should have the property items (base name: "items")', function() {
      // uncomment below and update the code to test the property items
      //var instance = new CatalogueApi.PaginatedListIEnumerableCapabilitiesImplementedEvidence();
      //expect(instance).to.be();
    });

    it('should have the property hasPreviousPage (base name: "hasPreviousPage")', function() {
      // uncomment below and update the code to test the property hasPreviousPage
      //var instance = new CatalogueApi.PaginatedListIEnumerableCapabilitiesImplementedEvidence();
      //expect(instance).to.be();
    });

    it('should have the property hasNextPage (base name: "hasNextPage")', function() {
      // uncomment below and update the code to test the property hasNextPage
      //var instance = new CatalogueApi.PaginatedListIEnumerableCapabilitiesImplementedEvidence();
      //expect(instance).to.be();
    });

  });

}));
