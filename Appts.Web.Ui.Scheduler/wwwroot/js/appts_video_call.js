
/**
 * Author: Adam DeRose
 * Date: 2/10/2021
 * Support management of business.
 */
var appts_video_call = (function () {
  var p = {}, _p = {};
  _p.test = {};

  p.init = function () {
    _p.handle.register();
    _p.initVideoCall();
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
  _p.initVideoCall = function () {

  };
  _p.handleFail = function (err) {
    console.log('error : ', err);
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
$(document).ready(function () { appts_video_call.init(); });
