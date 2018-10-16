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
    define(['ApiClient'], factory);
  } else if (typeof module === 'object' && module.exports) {
    // CommonJS-like environments that support module.exports, like Node.
    module.exports = factory(require('../ApiClient'));
  } else {
    // Browser globals (root is window)
    if (!root.CatalogueApi) {
      root.CatalogueApi = {};
    }
    root.CatalogueApi.CapabilitiesImplemented = factory(root.CatalogueApi.ApiClient);
  }
}(this, function(ApiClient) {
  'use strict';




  /**
   * The CapabilitiesImplemented model module.
   * @module model/CapabilitiesImplemented
   * @version 1.0.0-private-beta
   */

  /**
   * Constructs a new <code>CapabilitiesImplemented</code>.
   * A ‘capability’ which a ‘solution’ asserts that it provides.  This is then assessed by NHS to verify the ‘solution’ complies with the ‘capability’ it has claimed.
   * @alias module:model/CapabilitiesImplemented
   * @class
   * @param capabilityId {String} Unique identifier of capability
   * @param id {String} Unique identifier of entity
   * @param solutionId {String} Unique identifier of solution
   */
  var exports = function(capabilityId, id, solutionId) {
    var _this = this;

    _this['capabilityId'] = capabilityId;

    _this['id'] = id;
    _this['solutionId'] = solutionId;
  };

  /**
   * Constructs a <code>CapabilitiesImplemented</code> from a plain JavaScript object, optionally creating a new instance.
   * Copies all relevant properties from <code>data</code> to <code>obj</code> if supplied or a new instance if not.
   * @param {Object} data The plain JavaScript object bearing properties of interest.
   * @param {module:model/CapabilitiesImplemented} obj Optional instance to populate.
   * @return {module:model/CapabilitiesImplemented} The populated <code>CapabilitiesImplemented</code> instance.
   */
  exports.constructFromObject = function(data, obj) {
    if (data) {
      obj = obj || new exports();

      if (data.hasOwnProperty('capabilityId')) {
        obj['capabilityId'] = ApiClient.convertToType(data['capabilityId'], 'String');
      }
      if (data.hasOwnProperty('status')) {
        obj['status'] = ApiClient.convertToType(data['status'], 'String');
      }
      if (data.hasOwnProperty('id')) {
        obj['id'] = ApiClient.convertToType(data['id'], 'String');
      }
      if (data.hasOwnProperty('solutionId')) {
        obj['solutionId'] = ApiClient.convertToType(data['solutionId'], 'String');
      }
    }
    return obj;
  }

  /**
   * Unique identifier of capability
   * @member {String} capabilityId
   */
  exports.prototype['capabilityId'] = undefined;
  /**
   * Current status of this ClaimedCapability
   * @member {module:model/CapabilitiesImplemented.StatusEnum} status
   */
  exports.prototype['status'] = undefined;
  /**
   * Unique identifier of entity
   * @member {String} id
   */
  exports.prototype['id'] = undefined;
  /**
   * Unique identifier of solution
   * @member {String} solutionId
   */
  exports.prototype['solutionId'] = undefined;


  /**
   * Allowed values for the <code>status</code> property.
   * @enum {String}
   * @readonly
   */
  exports.StatusEnum = {
    /**
     * value: "Draft"
     * @const
     */
    "Draft": "Draft",
    /**
     * value: "Submitted"
     * @const
     */
    "Submitted": "Submitted",
    /**
     * value: "Remediation"
     * @const
     */
    "Remediation": "Remediation",
    /**
     * value: "Approved"
     * @const
     */
    "Approved": "Approved",
    /**
     * value: "Rejected"
     * @const
     */
    "Rejected": "Rejected"  };


  return exports;
}));

