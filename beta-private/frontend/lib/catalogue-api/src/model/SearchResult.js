/**
 * catalogue-api
 * NHS Digital GP IT Futures Buying Catalog API
 *
 * OpenAPI spec version: 1.0.0-private-beta
 *
 * NOTE: This class is auto generated by the swagger code generator program.
 * https://github.com/swagger-api/swagger-codegen.git
 *
 * Swagger Codegen version: 2.4.0-SNAPSHOT
 *
 * Do not edit the class manually.
 *
 */

(function(root, factory) {
  if (typeof define === 'function' && define.amd) {
    // AMD. Register as an anonymous module.
    define(['ApiClient', 'model/SolutionEx'], factory);
  } else if (typeof module === 'object' && module.exports) {
    // CommonJS-like environments that support module.exports, like Node.
    module.exports = factory(require('../ApiClient'), require('./SolutionEx'));
  } else {
    // Browser globals (root is window)
    if (!root.CatalogueApi) {
      root.CatalogueApi = {};
    }
    root.CatalogueApi.SearchResult = factory(root.CatalogueApi.ApiClient, root.CatalogueApi.SolutionEx);
  }
}(this, function(ApiClient, SolutionEx) {
  'use strict';




  /**
   * The SearchResult model module.
   * @module model/SearchResult
   * @version 1.0.0-private-beta
   */

  /**
   * Constructs a new <code>SearchResult</code>.
   * A SolutionEx and an indication of its relevance (Distance)
   * @alias module:model/SearchResult
   * @class
   */
  var exports = function() {
    var _this = this;



  };

  /**
   * Constructs a <code>SearchResult</code> from a plain JavaScript object, optionally creating a new instance.
   * Copies all relevant properties from <code>data</code> to <code>obj</code> if supplied or a new instance if not.
   * @param {Object} data The plain JavaScript object bearing properties of interest.
   * @param {module:model/SearchResult} obj Optional instance to populate.
   * @return {module:model/SearchResult} The populated <code>SearchResult</code> instance.
   */
  exports.constructFromObject = function(data, obj) {
    if (data) {
      obj = obj || new exports();

      if (data.hasOwnProperty('solutionEx')) {
        obj['solutionEx'] = SolutionEx.constructFromObject(data['solutionEx']);
      }
      if (data.hasOwnProperty('distance')) {
        obj['distance'] = ApiClient.convertToType(data['distance'], 'Number');
      }
    }
    return obj;
  }

  /**
   * SolutionEx
   * @member {module:model/SolutionEx} solutionEx
   */
  exports.prototype['solutionEx'] = undefined;
  /**
   * How far away the SolutionEx is from ideal:    zero     =&gt; SolutionEx has exactly capabilities required    positive =&gt; SolutionEx has more capabilities than required    negative =&gt; SolutionEx has less capabilities than required
   * @member {Number} distance
   */
  exports.prototype['distance'] = undefined;



  return exports;
}));


