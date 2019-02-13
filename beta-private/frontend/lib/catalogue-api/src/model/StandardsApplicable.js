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
    root.CatalogueApi.StandardsApplicable = factory(root.CatalogueApi.ApiClient);
  }
}(this, function(ApiClient) {
  'use strict';




  /**
   * The StandardsApplicable model module.
   * @module model/StandardsApplicable
   * @version 1.0.0-private-beta
   */

  /**
   * Constructs a new <code>StandardsApplicable</code>.
   * A ‘Standard’ which a ‘Solution’ asserts that it provides.  This is then assessed by NHS to verify the ‘Solution’ complies with the ‘Standard’ it has claimed.
   * @alias module:model/StandardsApplicable
   * @class
   * @param standardId {String} Unique identifier of standard
   * @param id {String} Unique identifier of entity
   * @param solutionId {String} Unique identifier of solution
   */
  var exports = function(standardId, id, solutionId) {
    var _this = this;

    _this['standardId'] = standardId;


    _this['id'] = id;
    _this['solutionId'] = solutionId;

  };

  /**
   * Constructs a <code>StandardsApplicable</code> from a plain JavaScript object, optionally creating a new instance.
   * Copies all relevant properties from <code>data</code> to <code>obj</code> if supplied or a new instance if not.
   * @param {Object} data The plain JavaScript object bearing properties of interest.
   * @param {module:model/StandardsApplicable} obj Optional instance to populate.
   * @return {module:model/StandardsApplicable} The populated <code>StandardsApplicable</code> instance.
   */
  exports.constructFromObject = function(data, obj) {
    if (data) {
      obj = obj || new exports();

      if (data.hasOwnProperty('standardId')) {
        obj['standardId'] = ApiClient.convertToType(data['standardId'], 'String');
      }
      if (data.hasOwnProperty('status')) {
        obj['status'] = ApiClient.convertToType(data['status'], 'String');
      }
      if (data.hasOwnProperty('qualityId')) {
        obj['qualityId'] = ApiClient.convertToType(data['qualityId'], 'String');
      }
      if (data.hasOwnProperty('id')) {
        obj['id'] = ApiClient.convertToType(data['id'], 'String');
      }
      if (data.hasOwnProperty('solutionId')) {
        obj['solutionId'] = ApiClient.convertToType(data['solutionId'], 'String');
      }
      if (data.hasOwnProperty('ownerId')) {
        obj['ownerId'] = ApiClient.convertToType(data['ownerId'], 'String');
      }
    }
    return obj;
  }

  /**
   * Unique identifier of standard
   * @member {String} standardId
   */
  exports.prototype['standardId'] = undefined;
  /**
   * Current status of this ClaimedStandard
   * @member {module:model/StandardsApplicable.StatusEnum} status
   */
  exports.prototype['status'] = undefined;
  /**
   * Unique identifier of Standard
   * @member {String} qualityId
   */
  exports.prototype['qualityId'] = undefined;
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
   * Unique identifier of supplier Contact who is responsible for this claim
   * @member {String} ownerId
   */
  exports.prototype['ownerId'] = undefined;


  /**
   * Allowed values for the <code>status</code> property.
   * @enum {String}
   * @readonly
   */
  exports.StatusEnum = {
    /**
     * value: "NotStarted"
     * @const
     */
    "NotStarted": "NotStarted",
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
     * value: "ApprovedFirstOfType"
     * @const
     */
    "ApprovedFirstOfType": "ApprovedFirstOfType",
    /**
     * value: "ApprovedPartial"
     * @const
     */
    "ApprovedPartial": "ApprovedPartial",
    /**
     * value: "Rejected"
     * @const
     */
    "Rejected": "Rejected"  };


  return exports;
}));


