﻿
/**
 * Author: Adam DeRose
 * Date: 2/9/21
 */
var appts_a = (function () {
  var p = {}, _p = {};
  _p.test = {};

  p.init = function () {
    _p.handle.register();
    //console.log('hiii');

  };

  _p.const = {

  };

  _p.ids = {

  };

  _p.names = {

  };

  _p.get = {

  };
  _p.set = {

  };

  _p.showNotify = function (message, type) {

    $.growl({
      message: message
    }, {
      type: type,
      allow_dismiss: false,
      label: 'Cancel',
      className: 'btn-xs btn-inverse',
      placement: {
        from: 'bottom',
        align: 'right'
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

  /// <summary>
  /// Container to define event handlers.
  /// </summary>
  _p.handle = {
    onClick: {

    },
    onHover: {

    },
    onChange: {

    }
  };

  p.handle = {

  };

  /// <summary>
  /// Register all event handlers.
  /// </summary>
  _p.handle.register = function () {

  };

  return p;
})();
$(document).ready(function () { appts_a.init(); });
