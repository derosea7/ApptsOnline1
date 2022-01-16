/**
 * Author: Adam James DeRose
 * Date: 9/10/2019
 * Enable adding appointment type.
 */
var appts_add_appt_type = (function () {
  var p = {}, _p = {};
  _p.test = {};
  //_p.handleOnClick = {};
  p.init = function () {
    _p.handle.register();
    //_p.testdur();
    //_p.setCancelationNoticeDdl();
    _p.initLocationHandler();
  };
  p.onChangeTimeout = {};
  // temp handler for any stuff that needs to happen when an edit
  // form is loaded--need to update the additional location info
  // location choice comes from ddl, selected by sp/user
  _p.setAdditionalLocationUi = function (locationChoice) {
    var additionalInfoRow = $('#rw-location-additional-info');
    var locWebConf = $('#cl-loc-web-conf');
    var locWeCall = $('#cl-loc-we-call');
    var locCustomerCalls = $('#cl-loc-customer-calls');
    var locWeSpecify = $('#cl-loc-we-specify');
    var locCustomerSpecifies = $('#cl-loc-customer-specifies');
    var activeEls = $('#rw-location-additional-info .col-12:not(.d-none)');
    switch (locationChoice) {
      case "-1":
        additionalInfoRow.hide();
        break;
      case 'web_conference':
        additionalInfoRow.show();
        activeEls.each(function (a, b) {
          $(b).addClass('d-none');
        });
        locWebConf.removeClass('d-none');
        break;
      case 'we_call':
        additionalInfoRow.show();
        activeEls.each(function (a, b) {
          $(b).addClass('d-none');
        });
        locWeCall.removeClass('d-none');
        break;
      case 'customer_calls':
        additionalInfoRow.show();
        activeEls.each(function (a, b) {
          $(b).addClass('d-none');
        });
        locCustomerCalls.removeClass('d-none');
        break;
      case 'we_specify':
        additionalInfoRow.show();
        activeEls.each(function (a, b) {
          $(b).addClass('d-none');
        });
        locWeSpecify.removeClass('d-none');
        break;
      case 'customer_specifies':
        additionalInfoRow.show();
        activeEls.each(function (a, b) {
          $(b).addClass('d-none');
        });
        locCustomerSpecifies.removeClass('d-none');
        break;
      default:
    }
  };
  /// <summary>Handle UI changes when Location is changed.</summary>
  // key, show different inputs based on location selection
  // if web conf, show an switch asking if sp providers conf, or client does
  // if sp provides conf address, then show an input
  // if client provdiers, than show message that the info will appear after
  // an appt is scheduled and client provides this.
  _p.initLocationHandler = function () {
    // hanlde initial state
    _p.setAdditionalLocationUi($('#ddl-location').val());
    // register handler for ongoing changes
    $('#ddl-location').on('change', function (a, b) {
      var locationChoice = $(this).val();
      clearTimeout(p.onChangeTimeout);
      p.onChangeTimeout = setTimeout(function () {
        // do stuff when the user has been idel for 1 second
        _p.setAdditionalLocationUi(locationChoice);
      }, 350);
    });
  };
  // can be called publically with function passed in
  p.handle = {
  };
  /// <summary>
  /// Container to define event handlers.
  /// </summary>
  _p.handle = {
    onLoad: {
      setCancelationNoticeDdl: function () {
        _p.setDurationDdl('cancelation-notice', 'CancelationNotice');
      },
      setRescheduleNoticeDdl: function () {
        _p.setDurationDdl('reschedule-notice', 'RescheduleNotice');
      },
      setMaxNoticeDdl: function () {
        _p.setDurationDdl('maximum-notice', 'MaximumNotice');
      },
      setMinNoticeDdl: function () {
        _p.setDurationDdl('minimum-notice', 'MinimumNotice');
      }
    },
    onClick: {},
    onChange: {
      cancelationNotice: function () {
        _p.initCustomDurationInput('cancelation-notice', 'CancelationNotice');
      },
      rescheduleNotice: function () {
        _p.initCustomDurationInput('reschedule-notice', 'RescheduleNotice');
      },
      maxNotice: function () {
        _p.initCustomDurationInput('maximum-notice', 'MaximumNotice');
      },
      minNotice: function () {
        _p.initCustomDurationInput('minimum-notice', 'MinimumNotice');
      }
    }
  };
  /// <summary>
  /// Register all event handlers.
  /// </summary>
  _p.handle.register = function () {
    _p.handle.onChange.cancelationNotice();
    _p.handle.onChange.rescheduleNotice();
    _p.handle.onChange.minNotice();
    _p.handle.onChange.maxNotice();
    _p.handle.onLoad.setCancelationNoticeDdl();
    _p.handle.onLoad.setRescheduleNoticeDdl();
    _p.handle.onLoad.setMaxNoticeDdl();
    _p.handle.onLoad.setMinNoticeDdl();
  };
  /// <summary>
  /// Set the days, hours and minutes portion of a custom duration input.
  /// </summary>
  _p.initCustomDurationInput = function (name1, name2) {
    $('#ddl-' + name1).on('change', function () {
      var selection = $(this).val();
      if (selection === '') {
        $('#ig-' + name1).css('display', 'flex');
      } else {
        $('#ig-' + name1).hide();
        if (selection !== '-1') {
          var totalMins = parseInt(selection, 10);
          var duration = _p.getDurationChoice(totalMins);
          $('#' + name2 + 'Days').val(duration.days);
          $('#' + name2 + 'Hours').val(duration.hours);
          $('#' + name2 + 'Minutes').val(duration.mins);
        }
      }
    });
  };
  /// <summary>
  /// Simple container for duration data.
  /// </summary>
  _p.duration = function (days, hours, mins) {
    this.days = days;
    this.hours = hours;
    this.mins = mins;
  };
  /// <summary>
  /// Get total seconds in the duration.
  /// Extends duration object.
  /// </summary>
  /// <returns>Number of seconds in duration</returns>
  _p.duration.prototype.calculateTotalSeconds = function () {
    return (this.days * 24 * 60 * 60) + (this.hours * 60 * 60) + (this.mins * 60);
  };
  /// <summary>
  /// Display either pre-defined time range or custom input based on total seconds in duration.
  /// NOTE: custom duration object.
  /// name1: 'cancelation-notice'
  /// name2: 'CancelationNotice'
  /// </summary>
  /// <param name="name1"></param>
  /// <param name="name2"></param>
  _p.setDurationDdl = function (name1, name2) {
    var days = $('#' + name2 + 'Days').val();
    var hours = $('#' + name2 + 'Hours').val();
    var mins = $('#' + name2 + 'Minutes').val();
    var duration = new _p.duration(days, hours, mins);
    var durationSeconds = duration.calculateTotalSeconds();
    console.log('name1', name1, 'name2', name2, 'duration', duration, 'duration Seconds', durationSeconds)
    // deos parsed duration match a default choice?
    var defaultChoice = _p.getDurationChoice(durationSeconds);
    //console.log('defaultChoice', defaultChoice)
    if (defaultChoice !== undefined && durationSeconds === defaultChoice.calculateTotalSeconds()) {
      $('#ddl-' + name1 + ' option[value="' + duration.calculateTotalSeconds() + '"]')
        .attr('selected', true);
    } else {
      // select custom
      $('#ig-' + name1).css('display', 'flex');
      $('#ddl-' + name1 + ' option:last').attr('selected', 'selected');
    }
  };
  /// <summary>
  /// Map preset durations to duration objects.
  /// </summary>
  _p.getDurationChoice = function (rangeInput) {
    var choice;
    switch (rangeInput) {
      case 0:
        choice = new _p.duration(0, 0, 0);
        break;
      case 3600:
        choice = new _p.duration(0, 1, 0);
        break;
      case 7200:
        choice = new _p.duration(0, 2, 0);
        break;
      case 14400:
        choice = new _p.duration(0, 4, 0);
        break;
      case 28800:
        choice = new _p.duration(0, 8, 0);
        break;
      case 86400:
        choice = new _p.duration(1, 0, 0);
        break;
      case 172800:
        choice = new _p.duration(2, 0, 0);
        break;
      case 259200:
        choice = new _p.duration(3, 0, 0);
        break;
      case 604800:
        choice = new _p.duration(7, 0, 0);
        break;
      case 2592000:
        choice = new _p.duration(30, 0, 0);
        break;
      case 5184000:
        choice = new _p.duration(60, 0, 0);
        break;
      case 7776000:
        choice = new _p.duration(90, 0, 0);
        break;
      default:

        break;
    }
    return choice;
  };
  return p;
})();
$(document).ready(function () { appts_add_appt_type.init(); });
