
/**
 * Author: Adam James DeRose
 * Date: 11/6/2019
 * common site funcationlity.
 */
var appts_common = (function () {
  var p = {}, _p = {};
  _p.test = {};
  p.init = function () {
    _p.handle.register();

  };
  _p.const = { };
  _p.ids = { };
  _p.names = { };
  _p.get = { };
  _p.set = {};
  p.showNotify = function (message, type) {
    $.growl({
      message: message
    }, {
      type: type,
      allow_dismiss: false,
      label: 'Cancel',
      className: 'btn-xs btn-inverse',
      placement: {
        from: 'bottom',
        align: 'left'
      },
      delay: 2500,
      animate: {
        enter: 'animated fadeInRight',
        exit: 'animated fadeOutRight'
      },
      offset: {
        x: 30,
        y: 30
      }
    });
  };
  p.parseQuery = function (queryString) {
    var query = {};
    var pairs = (queryString[0] === '?' ? queryString.substr(1) : queryString).split('&');
    for (var i = 0; i < pairs.length; i++) {
      var pair = pairs[i].split('=');
      query[decodeURIComponent(pair[0])] = decodeURIComponent(pair[1] || '');
    }
    return query;
  };
  p.getQueryStringValue = function (key) {
    var result;
    var pairs = window.location.search.substr(1).split('&');
    for (var i = 0; i < pairs.length; i++) {
      var pair = pairs[i].split('=');
      if (pair[0] === key) {
        result = decodeURIComponent(pair[1]);
        break;
      }
    }
    return result;
  };
  p.copyStringToClipboard = function (str) {
    var el = document.createElement('textarea');
    el.value = str;
    el.setAttribute('readonly', '');
    // apparently required to make this work
    el.style = { position: 'absolute', left: '-9999px' };
    document.body.appendChild(el);
    el.select();
    document.execCommand('copy');
    document.body.removeChild(el);
  };
  /// <summary>
  /// Container to define event handlers.
  /// </summary>
  _p.handle = {
    onClick: {

    },
    onHover: { },
    onChange: {

    }
  };
  p.handle = { };
  /// <summary>
  /// Register all event handlers.
  /// </summary>
  _p.handle.register = function () {

  };
  return p;
})();
$(document).ready(function () { appts_common.init(); });