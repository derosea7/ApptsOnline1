/**
 * Author: Adam DeRose
 * Date: 2/4/21
 * Support management of client settings.
 */
var appts_manage_client = (function (appts_common) {
  var p = {}, _p = {};
  _p.test = {};
  p.init = function () {
    _p.handle.register();
    var tzPickerOptions = {
      onTimeZoneChanged: function (timeZoneId) {
        $('[name="TimeZoneId"]').val(timeZoneId);
      },
      presetTimeZoneId: $('[name="TimeZoneId"]').val()
    };
    appts_tz.init(tzPickerOptions);
    //var isOnboarding = false;
    if (appts_common.getQueryStringValue('onboarding') === 't') {
      //isOnboarding = true;
      _p.initOnboarding();
    }
    _p.initNotifications();
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
  _p.initNotifications = function () {
    var updateNameSuccessful = $('#hf-edit-successful').val();
    if (updateNameSuccessful !== undefined && updateNameSuccessful === 'True') {
      _p.showNotify('Successfully updated profile!', 'inverse');
    }
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
  /// Do any thing required when sp still being onbaorded.
  /// Update time zone from browser guess.
  /// </summary>
  _p.initOnboarding = function () {
    // get tz from browser on first load
    var tzId = $('[name="TimeZoneId"]').val();
    //var tzId = 'Europe/London';
    _p.updateTimeZoneId(tzId)
      .done(function (data) {
        console.log('s', data);
      })
      .fail(function (data) {
        console.log('f', data);
        $('#alert-warning')
          .html('Unable to save guessed time zone. Please, review time zone and try saving again.')
          .show();
      });
  };
  _p.updateTimeZoneId = function (tzId) {
    var endpoint = 'https://' + window.location.host + '/Client/UpdateTzId';
    return $.ajax({
      url: endpoint,
      type: 'POST',
      contentType: 'application/json; charset=utf-8',
      dataType: 'json',
      data: JSON.stringify({
        TimeZoneId: tzId
      })
    });
  };

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
})(appts_common);
$(document).ready(function () { appts_manage_client.init(); });