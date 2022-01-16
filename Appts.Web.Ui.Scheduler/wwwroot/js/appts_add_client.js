
/**
 * Author: atom
 * Date: 2/10/2021
 * Support management of page.
 */
var appts_add_client = (function (appts_common) {
  var p = {}, _p = {};
  _p.test = {};
  p.init = function () {
    _p.handle.register();
    //alert('loaded');
    _p.initNotifications();
  };
  _p.const = {};
  _p.ids = {};
  _p.names = {};
  _p.get = {};
  _p.set = {};
  _p.initNotifications = function () {
    appts_common.showNotify('notify', 'inverse')
  };
  /// <summary>
  /// Container to define event handlers.
  /// </summary>
  _p.handle = {
    onClick: {

    },
    onHover: {},
    onChange: {}
  };
  p.handle = {};
  /// <summary>
  /// Register all event handlers.
  /// </summary>
  _p.handle.register = function () {

  };
  return p;
})(appts_common);
$(document).ready(function () { appts_add_client.init(); });