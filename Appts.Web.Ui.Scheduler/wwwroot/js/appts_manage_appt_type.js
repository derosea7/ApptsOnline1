/**
 * Author: Adam James DeRose
 * Date: 10/22/2019
 * Support management of appointment types.
 */
var appts_manage_appt_type = (function () {
  var p = {}, _p = {};
  _p.test = {};
  p.init = function () {
    _p.handle.register();
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
    var deletedType = $('#qs-deleted-type').val();
    if (deletedType !== undefined && deletedType === 't') {
      _p.showNotify('Successfully deleted type!', 'inverse');
    }
    var addedType = $('#qs-added-type').val();
    if (addedType !== undefined && addedType === 't') {
      _p.showNotify('Successfully added type!', 'inverse');
    }
    var updatedType = $('#qs-updated-type').val();
    if (updatedType !== undefined && updatedType === 't') {
      _p.showNotify('Successfully updated type!', 'inverse');
    }
  };
  /// <summary>
  /// Container to define event handlers.
  /// </summary>
  _p.handle = {
    onClick: {
      delete: function () {
        $('body').on('click', '.btn_delete', function () {
         // var baseDeleteUri = $('#a-uri-delete-appt-type').attr('href');
          var apptTypeId = $(this).closest('.card').attr('id');
          // combine vals, show modal with button with route
         // var uri = baseDeleteUri + '/' + apptTypeId;
          //$('#btn-confirm-delete').attr('href', uri);
          $('#frm-cofirm-delete').attr('action', '/AppointmentType/Delete/' + apptTypeId);
          $('#mdl-confirm-delete').modal('show');
        });
        //a-uri-delete-appt-type
      },
      viewDetails: function () {
        $('body').on('click', '.btn_details', function () {
          //show spinner on button, hide orginal text
          var regularButtonText = $(this).find('.details-btn-text');
          var loading = $(this).find('.loading-details');
          regularButtonText.hide();
          loading.show();
          var typeId = $(this).closest('.card').attr('id');
          //alert('clicked, id is ' + typeId)
          _p.getAppointmentTypeDetails(typeId)
            .done(function (data) {
              //hide the spinner and show the regular details btn again
              $('#mdl-details').modal('show');
            })
            .always(function () {
              loading.hide();
              regularButtonText.show();
            });
        });
      }
    },
    onHover: {

    },
    onChange: {

    }
  };
  _p.generateStatusBadge = function (isActive) {
    var statusClass, statusText;
    if (isActive) {
      statusClass = 'success';
      statusText = 'Active';
    } else {
      statusClass = 'light';
      statusText = 'Inactive';
    }
    return '<div class="badge badge-' + statusClass + ' ml-3">' + statusText + '</div>';
  };
  _p.getAppointmentTypeDetails = function (apptTypeId) {
    var endpoint = 'https://' + window.location.host + '/AppointmentType/Detail/' + apptTypeId;
    //return $.getJSON('https://localhost:44352/AppointmentType/Detail/' + apptTypeId)
    return $.getJSON(endpoint)
      .done(function (data) {
        console.log('s', data);
        //$('#mdl-details .modal-title').html(data.name);
        var cardTitle = data.name +_p.generateStatusBadge(data.isActive);
        $('#mdl-details .modal-title').html(cardTitle);
        $('#td-appt-type-description').html(data.description);
        var readableDuration = _p.getHumanReadableDuration(data.duration);
        var readableBufferBefore = _p.getHumanReadableDuration(data.bufferBefore);
        var readableBufferAfter = _p.getHumanReadableDuration(data.bufferAfter);
        var readableMinNotice = _p.getHumanReadableDuration(data.minimumNotice);
        var readableMaxNotice = _p.getHumanReadableDuration(data.maximumNotice);
        var readableCancelationNotice = _p.getHumanReadableDuration(data.cancelationNotice);
        var readableRescheduleNotice = _p.getHumanReadableDuration(data.rescheduleNotice);
        $('#td-appt-type-duration').html(readableDuration);
        $('#td-appt-type-buffer-before').html(readableBufferBefore);
        $('#td-appt-type-buffer-after').html(readableBufferAfter);
        $('#td-appt-type-min-notice').html(readableMinNotice);
        $('#td-appt-type-max-notice').html(readableMaxNotice);
        $('#td-appt-type-reschedule-notice').html(readableRescheduleNotice);
        $('#td-appt-type-cancelation-notice').html(readableCancelationNotice);
        //map location to value
        //$('#td-appt-type-location').html(data.location);
        $('#td-appt-type-location').html(_p.location[data.location]);
        $('#td-appt-type-location-details').html(data.locationDetails);
      })
      .fail(function (data) {
        console.log('e', data);
      });
  };
  _p.location = {
    '-1': 'Not designated',
    'web_conference': 'Web conference',
    'we_call': 'We will call the customer',
    'customer_calls': 'Customer will call us',
    'we_specify': 'We will specify the meeting location',
    'customer_specifies': 'Customer will specify the meeting location'
  };
  p.handle = {

  };
  /// <summary>
  /// Convert duration string to humanly readable time.
  /// </summary>
  _p.getHumanReadableDuration = function (duration) {
    var m = moment.duration(duration);
    var durationParts = [];
    var days = m.get('days');
    var hours = m.get('hours');
    var minutes = m.get('minutes');
    //console.log('days', days, 'hours', hours, 'mins', minutes);
    if (days > 0) {
      var mdays = m.clone();
      durationParts.push(mdays.humanize());
    }
    //isolate hours then humanize
    if (hours > 0) {
      var mhours = m.clone();
      if (days > 0) {
        mhours.subtract(days, 'days');
      }
      if (minutes > 0) {
        mhours.subtract(minutes, 'minutes');
      }
      durationParts.push(mhours.humanize());
    }
    //isolate minutes then humanize
    if (minutes > 0) {
      var mmins = m.clone();
      if (days > 0) {
        mmins.subtract(days, 'days');
      }
      if (hours > 0) {
        mmins.subtract(hours, 'hours');
      }
      durationParts.push(mmins.humanize());
    }
    //console.log('duration in readable format', durationParts.join('|'));
    return durationParts.join(', ');
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
  /// Register all event handlers.
  /// </summary>
  _p.handle.register = function () {
    _p.handle.onClick.delete();
    _p.handle.onClick.viewDetails();
  };

  //_p.deleteApptType = function (apptTypeId) {
  //  var endpoint = $('#a-uri-update-whitelist').attr('href');
  //  return $.ajax({
  //    url: endpoint,
  //    type: 'POST',
  //    contentType: 'application/json; charset=utf-8',
  //    dataType: 'json',
  //    data: JSON.stringify({
  //      apptTypeId: apptTypeId
  //    })
  //  });
  //};

  return p;
})();
$(document).ready(function () { appts_manage_appt_type.init(); });
