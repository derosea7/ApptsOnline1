
/**
 * Author: Adam DeRose
 * Date: 9/8/2019
 * Support viewing appointments
 * depends on appts_tz
 */
var appts_view_appts_tz = (function () {
  var p = {}, _p = {};
  _p.test = {};

  p.init = function () {
    _p.handle.register();
    _p.initDateRangePickers();
    _p.initPresetRangePopover();
    _p.initDetailModal();
    var tzPickerOptions = {
      onTimeZoneChanged: function (tzid) {
        $('[name="tz"]').val(tzid);
      },
      presetTimeZoneId: $('[name="tz"]').val()
    };
    appts_tz.init(tzPickerOptions);
  };
  p.handleFormSubmission = function (event) {
    event.preventDefault();

    return false;
  };
  _p.formats = {
    datepicker: {
      iso8601: 'yy-mm-dd',
      shortFullDate: 'D, M dd, yy'
    },
    momentJs: {
      shortFullDate: 'ddd, MMM DD, YYYY',
      netDateTime: 'M/DD/YYYY hh:mm:ss a'
    }
  };
  _p.updateModalLinks = function (apptId) {
    var baseReschedule = $('#a-uri-reschedule-base').attr('href');
    $('#a-reschedule').attr('href', baseReschedule + '/' + apptId);
    var baseCancel = $('#a-uri-cancel-base').attr('href');
    var spVanityUrl = $('#sp-vanity-url').val();
    $('#a-cancel').attr('href', baseCancel + '/' + spVanityUrl + '?apptId=' + apptId);
  };
  _p.formatPhoneNumber = function (strPhoneRaw) {
    var left10 = strPhoneRaw.substr(strPhoneRaw.length - 10, strPhoneRaw.length);
    var countryCode = strPhoneRaw.substr(0, strPhoneRaw.length - 10);
    return '+' + countryCode + ' ' + left10.substr(0, 3) + '-' + left10.substr(3, 3) + '-' + left10.substr(6, 4);
  };
  _p.initDetailModal = function () {
    $('body').on('click', '.btn-detail-link', function () {
      var apptId = $(this).data('apptId');
      var spVanityUrl = $('#sp-vanity-url').val();
      _p.getApptDetail(spVanityUrl, apptId)
        .done(function (data) {
          //console.log('appt detail gotten', data);
          var appt = data;
          var displayTz = $('#display-iana-timezone-id').val();
          var start = moment.tz(appt.startTime, appt.timeZoneId);
          start.tz(displayTz);
          var end = moment.tz(appt.endTime, appt.timeZoneId);
          end.tz(displayTz);
          $('#td-start-date').html(start.format('ddd') + ', ' + start.format('LL'));
          $('#spn-start').html(start.format('LT'));
          $('#spn-start-date').html(start.format('l'));
          $('#spn-end').html(end.format('LT'));
          $('#spn-end-date').html(end.format('l'));
          $('#td-client-email').html(appt.clientEmail);
          if (appt.clientMobile) {
            $('#td-client-mobile').html(_p.formatPhoneNumber(appt.clientMobile));
          }
          $('#td-client-fname').html(appt.clientFName);
          $('#td-client-lname').html(appt.clientLName);
          $('#td-appt-timezone').html(appt.timeZoneId);
          $('#td-appt-type-breif').html(appt.apptTypeBreif);
          $('#detailModalTitle').html('Appt ' + _p.generateStatusBadge(appt.status));
          if (appt.status.toLowerCase() === 'active') {
            $('#btn-row-modal-links').show();
          } else {
            $('#btn-row-modal-links').hide();
          }
          $('#detailModal').modal('show');
        })
        .fail(function (data) {
          console.log('fail from init detail modal');
        });
      _p.updateModalLinks(apptId);
    });
    $('#detailModal').on('show.bs.modal', function (e) {
      //console.log('my modal has been shown');
    });
  }
  _p.generateStatusBadge = function (statusText) {
    var statusClass = '';
    switch (statusText.toLowerCase()) {
      case 'active':
        statusClass = 'success';
        break;
      case 'canceled':
        statusClass = 'warning';
        break;
      case 'rescheduled':
        statusClass = 'info';
        break;
      default:
        statusClass = 'light';
        break;
    }
    return '<div class="badge badge-' + statusClass + ' ml-3">' + statusText + '</div>';
  };
  _p.getBadgeClass = function (apptStatus) {
    var statusClass = '';
    switch (apptStatus.toLowerCase()) {
      case 'active':
        statusClass = 'success';
        break;
      case 'canceled':
        statusClass = 'warning';
        break;
      case 'rescheduled':
        statusClass = 'info';
        break;
      default:
        statusClass = 'light';
        break;
    }
    return statusClass;
  };
  _p.getApptDetail = function (spVanityUrl, apptId) {
    var uri = $('#a-uri-appointment-detail').attr('href');
    var urll = $('#a-uri-appointment-detail').attr('href') + '/' + spVanityUrl + '?apptId=' + apptId;
    return $.ajax({
      url: urll,
      type: 'GET',
      contentType: 'application/json; charset=utf-8'
    })
      .done(function (data) {

      })
      .fail(function (data) {
        console.log('e update timezoneid');
      });
  };
  _p.getRangePickerOptions = function (altField) {
    return {
      altField: altField,
      altFormat: _p.formats.datepicker.iso8601,
      dateFormat: _p.formats.datepicker.shortFullDate
    };
  };
  _p.initDateRangePickers = function () {
    var startDate = $('#range-start-date').val();
    var endDate = $('#range-end-date').val();
    $('#start-date-picker')
      .datepicker(_p.getRangePickerOptions('#range-start-date'))
      .datepicker('setDate', new Date(startDate));
    $('#end-date-picker')
      .datepicker(_p.getRangePickerOptions('#range-end-date'))
      .datepicker('setDate', new Date(endDate));
    _p.initPresetRangePopoverText(startDate, endDate);
  };
  _p.initPresetRangePopoverText = function (start, end) {
    $('#btn-date-range-picker').html(
      moment(start, _p.formats.momentJs.netDateTime).format(_p.formats.momentJs.shortFullDate)
        + ' - '
      + moment(end, _p.formats.momentJs.netDateTime).format(_p.formats.momentJs.shortFullDate)
      + '&nbsp;&nbsp;<span class="feather icon-chevron-down"></span>')
      .show();
  };
  _p.initPresetRangePopover = function () {
    $.fn.popover.Constructor.Default.whiteList.div = ['data-toggle'];
    $.fn.popover.Constructor.Default.whiteList.button = ['type', 'class', 'id', 'data-offset'];
    $.fn.popover.Constructor.Default.whiteList.a = ['role', 'href', 'data-toggle'];
    $('#btn-date-range-picker').popover({
      html: true,
      content: function () {
        return $('#popover-content').html();
      },
      //sanitize: false,
      fallbackPlacement: 'counterclockwise'
    });
  };
  p.handle = {
    onChange: {

    }
  };

  /// <summary>
  /// Container to define event handlers.
  /// </summary>
  _p.handle = {
    onClick: {
      resetFilters: function () {
        $('#btn-reset-filters').on('click', function () {
          $('#start-date-picker').datepicker('setDate', -3);
          $('#end-date-picker').datepicker('setDate', 7);
          var apptType = $('[name="AppointmentTypeId"]');
          apptType.val(apptType.find('option:first').val());

          var status = $('[name="AppointmentStatus"]');
          status.val(status.find('option:first').val());
        });
      },

      presetToday: function () {
        $('body').on('click', '#btn-preset-range-today', function () {
        //$('#btn-range-next-90').on('click', function () {
          //alert('next 90');
          var today = new Date();
          $('#start-date-picker').datepicker('setDate', today);
          $('#end-date-picker').datepicker('setDate', today);
          $('#frm-filters').submit();
        });
      },
      presetRange: function () {
        $('body').on('click', '#preset-ranges .range', function () {
          var offset = parseInt($(this).data('offset'), 10);
          var today = new Date();
          if (offset < 0) {
            $('#start-date-picker').datepicker('setDate', offset);
            $('#end-date-picker').datepicker('setDate', today);
          } else {
            $('#start-date-picker').datepicker('setDate', today);
            $('#end-date-picker').datepicker('setDate', offset);
          }
          $('#frm-filters').submit();
        });
      },
      refreshTimesAfterChangingTimeZone: function () {
        $('#btn-refresh-times-after-timezone-changed').on('click', function() {
          $('#frm-filters').submit();
        });
      },
      showMoreDetails: function () {
        $('#a-more-details').on('click', function () {
          $('#tf-more-info').slideToggle();
          //$('.td-details-show-more').toggle();
          $('#tbl-more-info').toggle();
        });
      }
    },
    onChange: {

    },
    onHover: {
      apptDetail: function () {
        var header = $('.appt-item');
        header.hover(
          function () {
            $(this).find('.btn-detail-link').css('visibility', 'unset');
            $(this).find('.btn-detail-link').css('display', 'inline-block');
          },
          function () {
            $(this).find('.btn-detail-link').css('visibility', 'hidden');
          }
        );
      }
    }

  };
  _p.refreshTimes = function (timeZoneId) {
    var qs = location.search;
    var endpoint = $('#a-uri-refresh-times').attr('href') + qs;
    $.ajax({
      url: endpoint,
      type: 'POST',
      contentType: 'application/json; charset=utf-8',
      dataType: 'json',
      data: JSON.stringify({ TimeZoneId: timeZoneId })
    })
      .done(function (data) {
        //console.log('s');
        //console.log(data);

      })
      .fail(function (data) {
        console.log('e update timezoneid');

      });
  };
  /// <summary>
  /// Register all event handlers.
  /// </summary>
  _p.handle.register = function () {
    _p.handle.onClick.resetFilters();
    _p.handle.onClick.presetToday();
    _p.handle.onClick.presetRange();
    _p.handle.onHover.apptDetail();
    _p.handle.onClick.refreshTimesAfterChangingTimeZone();
    _p.handle.onClick.showMoreDetails();
  };
  return p;
})();
$(document).ready(function () { appts_view_appts_tz.init(); });