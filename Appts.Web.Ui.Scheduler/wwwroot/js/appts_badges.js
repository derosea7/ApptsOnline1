
/**
 * Author: Adam James DeRose
 * Date: 
 * Support management of badges.
 */
var appts_manage_badges = (function () {
  var p = {}, _p = {};
  _p.test = {};

  p.init = function () {
    _p.handle.register();
    console.log('managed badges');
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



  /// <summary>
  /// Container to define event handlers.
  /// </summary>
  _p.handle = {
    onClick: {
      badgeClicked: function () {
        $('.appt-badge-clickable').on('click', function () {
          console.log('ljkadskjlsdfaklj;fsdkjl;sdf');
        });
      }
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
    _p.handle.onClick.badgeClicked();
  };

  return p;
})();
$(document).ready(function () { appts_manage_badges.init(); });