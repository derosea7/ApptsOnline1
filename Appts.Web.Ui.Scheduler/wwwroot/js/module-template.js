
/**
 * Author: atom
 * Date: 2/10/2021
 * Support management of page.
 */
var appts_manage_page = (function () {
  var p = {}, _p = {};
  _p.test = {};
  p.init = function () {
    _p.handle.register();

  };
  _p.const = { };
  _p.ids = { };
  _p.names = { };
  _p.get = { };
  _p.set = { };
  /// <summary>
  /// Container to define event handlers.
  /// </summary>
  _p.handle = {
    onClick: {

    },
    onHover: { },
    onChange: { }
  };
  p.handle = { };
  /// <summary>
  /// Register all event handlers.
  /// </summary>
  _p.handle.register = function () {

  };
  return p;
})();
$(document).ready(function () { appts_manage_page.init(); });