
/**
 * Author: Adam James DeRose
 * Date: 10/6/2019
 * Support management of business.
 */
var appts_send_test_email_reminder = (function () {
  var p = {}, _p = {};
  _p.test = {};

  p.init = function () {
    _p.handle.register();

    var testSent = $('#qs-test-sent').val();
    if (testSent !== undefined && testSent === 't') {
      _p.showNotify('Your reminder is on its way!', 'inverse');
    }
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
      sendTestEmailReminder: function () {
        $('#btn-send-test').on('click', function () {

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

  };

  return p;
})();
$(document).ready(function () { appts_send_test_email_reminder.init(); });
