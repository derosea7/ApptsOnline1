/**
 * Author: Adam James DeRose
 * Date: 10/6/2019
 * Support management of service provider settings.
 */
var appts_manage_sp = (function (appts_common) {
  var p = {}, _p = {};
  _p.test = {};
  p.init = function () {
    _p.handle.register();
    _p.initPopovers();
    var tzPickerOptions = {
      onTimeZoneChanged: function (timeZoneId) {
        $('[name="TimeZoneId"]').val(timeZoneId);
      },
      presetTimeZoneId: $('[name="TimeZoneId"]').val()
    };
    appts_tz.init(tzPickerOptions);
    _p.initSchedulingPrivacyLevelRange();
    //var isOnboarding = false;
    if (appts_common.getQueryStringValue('onboarding') === 't') {
      //isOnboarding = true;
      _p.initOnboarding();
    }
    //console.log('edit name? ', $('#hf-edit-name-successful').val());
    window.location.hash = '';
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
    var updateNameSuccessful = $('#hf-edit-name-successful').val();
    if (updateNameSuccessful !== undefined && updateNameSuccessful === 'True') {
      _p.showNotify('Successfully updated name', 'inverse');
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
    var endpoint = 'https://' + window.location.host + '/ServiceProvider/UpdateTzId';
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
  _p.initPopovers = function () {
    $('.popover-dismiss').popover();
  };
  _p.initSchedulingPrivacyLevelRange = function () {
    var presetLevel = $('[name="Business.SchedulingPrivacyLevel"]').val();
    var intLevel = parseInt(presetLevel, 10);
    _p.updateRangeStateByPrivacyLevel(intLevel);
  };
  _p.generateDeleteButton = function () {
    return '<button type="button" class="btn btn-secondary p-2">'
      + '<span class="feather icon-trash-2"></span>';
  };
  _p.generateWhitelistEntryTd = function (entry) {
    return '<td class="td-white">' + entry + '</td>';
  };
  _p.generateButtonTd = function () {
    return '<td class="td-btn">' + _p.generateDeleteButton() + '</td>';
  };
  /// <summary>
  /// Container to define event handlers.
  /// </summary>
  _p.handle = {
    onClick: {
      copyUrlToClipboard: function () {
        $('#btn-copy-url-to-clipboard').on('click', function () {
          appts_common.copyStringToClipboard($('#hf-full-vanity-url').val());
          appts_common.showNotify('Copied to clipboard', 'inverse');
        });
      },
      manageWhitelist: function () {
        $('#btn-manage-whitelist').on('click', function () {
          // load white list

          // update dom
          _p.getWhitelist()
            .done(function (data) {
              var html = [];

              for (var i = 0; i < data.length; i++) {
                html.push(
                  '<tr>',
                  _p.generateWhitelistEntryTd(data[i]),
                  _p.generateButtonTd(),
                  '</tr>'
                );
              }
              $('#tbl-whitelist tbody').html(html.join(''));
              $('#mdl-manage-whitelist').modal('show');
            })
            .fail(function (data) {
            });
        });
      },
      addWhitelistEntry: function () {
        $('#btn-whitelist-email').on('click', function () {
          var newEntry = $('#tb-new-whitelist-entry').val();
          $('#tbl-whitelist tbody').append('<tr>' + _p.generateWhitelistEntryTd(newEntry) + _p.generateButtonTd() + '</tr>');
        });
      },
      saveUpdatedWhitelist: function () {
        var btn = $('#btn-save-whitelist');
        var btnSpinner = btn.find('.spinner-grow');
        var btnText = btn.find('.btn-text');
        var successAlert = $('#update-whitelist-success-msg');
        btn.on('click', function () {
          btnSpinner.show();
          btnText.hide();
          successAlert.hide();
          var whitelist = [];
          $('#tbl-whitelist tbody td:nth-child(1)').each(function (i, e) {
            whitelist.push($(e).html());
          });
          _p.saveWhitelist(whitelist)
            .done(function (data) {
              console.log('s', data);
              $('#tb-new-whitelist-entry').val('');
              successAlert.show();
            })
            .fail(function (data) {
              console.log('e', data);
            })
            .always(function () {
              btnSpinner.hide();
              btnText.show();
            });
        });
      },
      removeWhitelistEntry: function () {
        $('body').on('click', '#tbl-whitelist tbody td:nth-child(2)', function () {
          $(this).closest('tr').remove();
        });
      }
    },
    onHover: {
    },
    onChange: {
      schedulingPrivacyLevel: function () {
        $('#rng-scheduling-privacy-level').on('input', function () {
          var selection = parseInt($(this).val(), 10);
          _p.updateRangeStateByPrivacyLevel(selection);
        });
      }
    }
  };
  _p.updateRangeStateByPrivacyLevel = function (privacyLevel) {
    switch (privacyLevel) {
      case 1:
        $('#clp-manage-whitelist').collapse('hide');
        break;
      case 2:
        $('#clp-manage-whitelist').collapse('hide');
        break;
      case 3:
        $('#clp-manage-whitelist').collapse('show');
        break;
      default:
    }
  };
  _p.getWhitelist = function () {
    var endpoint = $('#a-uri-get-whitelist').attr('href');
    return $.ajax({
      url: endpoint,
      type: 'GET',
      contentType: 'application/json; charset=utf-8',
      dataType: 'json',
      success: function (data) {
        console.log('s', data);
      },
      error: function (data) {
        console.log('e', data);
      }
    });
  };
  _p.saveWhitelist = function (whitelist) {
    var endpoint = $('#a-uri-update-whitelist').attr('href');
    return $.ajax({
      url: endpoint,
      type: 'POST',
      contentType: 'application/json; charset=utf-8',
      dataType: 'json',
      data: JSON.stringify({
        whitelistEntries: whitelist
      })
    });
  };
  p.handle = {
  };
  /// <summary>
  /// Register all event handlers.
  /// </summary>
  _p.handle.register = function () {
    _p.handle.onChange.schedulingPrivacyLevel();
    _p.handle.onClick.manageWhitelist();
    _p.handle.onClick.addWhitelistEntry();
    _p.handle.onClick.saveUpdatedWhitelist();
    _p.handle.onClick.removeWhitelistEntry();
    _p.handle.onClick.copyUrlToClipboard();
  };
  return p;
})(appts_common);
$(document).ready(function () { appts_manage_sp.init(); });