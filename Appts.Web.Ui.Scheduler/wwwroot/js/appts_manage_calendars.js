/**
 * Author: Adam James DeRose
 * Date: 12/1/2019
 * Support management of connected social calendars.
 */
var appts_manage_calendars = (function (appts_common) {
  var p = {}, _p = {};
  _p.test = {};
  p.init = function () {
    _p.handle.register();
    var disconnectedCalendar = $('#hf-calendar-disconnected').val();
    if (disconnectedCalendar === 'True') {
      appts_common.showNotify('Successfully disconnected Google Calendar', 'inverse');
    }
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
      whatToDoAboutPendingVerification: function () {
        $('#btn-what-to-do').on('click', function () {
          $('#mdl-help-app-isnt-verified').modal('show');
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
    _p.handle.onClick.whatToDoAboutPendingVerification();
  };
  return p;
})(appts_common);
$(document).ready(function () { appts_manage_calendars.init(); });