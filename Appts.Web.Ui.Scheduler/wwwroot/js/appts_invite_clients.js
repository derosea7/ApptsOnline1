/**
 * Author: Adam James DeRose
 * Date: 12.1.2019
 * Support management of clients for service providers.
 */
var appts_invite_clients = (function (appts_common) {
  var p = {}, _p = {};
  _p.test = {};
  p.init = function () {
    _p.handle.register();
    //a-po-copy-vanity-url
    $('#a-po-copy-vanity-url').popover();
  };
  _p.const = {};
  _p.ids = {};
  _p.names = {};
  _p.get = {};
  _p.set = {};
  /// <summary>
  /// Container to define event handlers.
  /// </summary>
  _p.handle = {
    onClick: {
      copyVanityUrlToClipboard: function () {
        $('#btn-copy-url-to-clipboard').on('click', function () {
          appts_common.copyStringToClipboard($('#tb-vanity-url').val());
        });
      }
    },
    onHover: {},
    onChange: {}
  };
  p.handle = {};
  /// <summary>
  /// Register all event handlers.
  /// </summary>
  _p.handle.register = function () {
    _p.handle.onClick.copyVanityUrlToClipboard();
  };
  return p;
})(appts_common);
$(document).ready(function () { appts_invite_clients.init(); });