
/**
 * Author: Adam James DeRose
 * Date: 7/30/2019
 * Support scheduling appointments.
 */
var appts_schedule_appt = (function () {
  var p = {}, _p = {};
  _p.test = {};
  p.init = function () {
    _p.handle.register();
    var tzPickerOptions = {
      onTimeZoneChanged: function (selectedTimeZoneId, formattedTimeSelection) {
        _p.set.appointmentClientTimeZone(selectedTimeZoneId);
        //$('#current-time').html(formattedTimeSelection);
      },
      presetTimeZoneId: _p.get.appointmentClientTimeZone()
    };
    appts_tz.init(tzPickerOptions);
    $('#current-timezone').html(_p.get.appointmentClientTimeZone());
    $('#current-time').html($('#current-time-on-form').html());
    var apptIdToReschedule = $('#appt-id-to-reschedule').val();
    //console.log('isReschedule form init? ', apptIdToReschedule);
    if (apptIdToReschedule !== undefined && apptIdToReschedule !== '')
      _p.initReschedule(apptIdToReschedule);
    var isGetFromInvalidPost = $('#is-get-from-invalid-post').val();
      //console.log('is get form invalid post', isGetFromInvalidPost)
    if (isGetFromInvalidPost === 'True') {
      //console.log('is get form invalid post')
      _p.initGetFromInvalidPost();
    }
    // register event after initial hash set; ui already aligen with hash.
    location.hash = 'type';
    _p.handleHashChange();
  };
  /// <summary>
  /// Sync wizard in ui guiding user through scheduling with hash.
  /// If a user presses the back button, and the previous url was one of these
  /// hashes, the user will experience going back.
  /// </summary>
  _p.handleHashChange = function () {
    $(window).on('hashchange', function (e) {
      switch (location.hash) {
        case '#type':
          $('#wizard a#tab-appt-type').tab('show');
          break;
        case '#datetime':
          $('#wizard a#tab-appt-time').tab('show');
          break;
        case '#info':
          $('#wizard a#tab-info').tab('show');
          break;
        case '#review':
          $('#wizard a#tab-review').tab('show');
          break;
        default:
      }
    });
  };
  _p.initGetFromInvalidPost = function () {
    var businessTimeZone = _p.get.businessTimeZoneId();
    var selectedStartTime = _p.get.appointmentStartTime();
    var selectedEndTime = _p.get.appointmentEndTime();
    var clienttz = _p.get.clientTimeZoneId();
    var time = moment.tz(selectedStartTime, _p.const.jsonDateTime, businessTimeZone);
    var endTime = moment.tz(selectedEndTime, _p.const.jsonDateTime, businessTimeZone);
    if (clienttz !== businessTimeZone) {
      // then we are in client tz
      time.tz(clienttz);
      endTime.tz(clienttz);
    }
    _p.set.reviewAppointmentTime(time.format('h:mm a') + ' - ' + endTime.format('h:mm a'));
    _p.set.reviewAppointmentDate(time.format('dddd, MMMM Do YYYY'));
    _p.set.reviewAppointmentTimeZone(clienttz);
    $('#wizard a#tab-info').removeClass('disabled').tab('show');
        $('#tab-review').removeClass('disabled');
  };
  _p.apptType = {
    location: {
      '-1': 'Not designated',
      'web_conference': 'Web conference',
      'we_call': 'We will call the customer',
      'customer_calls': 'Customer will call us',
      'we_specify': 'We will specify the meeting location',
      'customer_specifies': 'Customer will specify the meeting location'
    }
  }; 
  _p.const = {
    sqlDt: 'YYYY-MM-DD',
    showDt: 'M/D/YY',
    showTime: 'h:mm a',
    militaryTime: 'HH:mm:ss',
    jsonDateTime: 'YY-MM-DDTHH:mm:ss'
  };
  _p.ids = {
    spTimeZone: '#business-timezone-id',
    businessUserId: '#business-user-id',
    scheduleUrl: '#a-schedule-link',
    saveAppointmentUrl: '#a-save-appt-link',
    appointmentUrl: '#a-appt-link',
    schedulerRole: '#scheduler-role',
    inputClientEmail: '#tb-client-email',
    inputClientFirstName: '#tb-client-fname',
    inputClientLastName: '#tb-client-lname',
    inputClientMobile: '#tb-client-mobile',
    inputAnonClientEmail: '#tb-anon-email',
    inputAnonClientFirstName: '#tb-anon-fname',
    inputAnonClientLastName: '#tb-anon-lname',
    inputAnonClientMobile: '#tb-anon-mobile',
    reviewClientEmail: '#lgi-client-email',
    reviewClientFirstName: '#lgi-client-fname',
    reviewClientLastName: '#lgi-client-lname',
    reviewClientMobile: '#lgi-client-mobile'
  };
  _p.names = {
    appointmentTypeId: '[name="AppointmentTypeId"]',
    clientTimeZone: '[name="ClientTimeZone"]',
    appointmentClientEmail: '[name="ClientEmail"]',
    appointmentClientFirstName: '[name="ClientFirstName"]',
    appointmentClientLastName: '[name="ClientLastName"]',
    appointmentClientMobile: '[name="ClientMobilePhone"]',
    appointmentStartTime: '[name="StartTime"]',
    appointmentEndTime: '[name="EndTime"]'
  };
  _p.get = {
    clientTimeZoneId: function () {
      return $(_p.names.clientTimeZone).val();
    },
    appointmentClientTimeZone: function () {
      return $(_p.names.clientTimeZone).val();
    },
    businessTimeZoneId: function () {
      return $(_p.ids.spTimeZone).val();
    },
    appointmentTypeId: function () {
      return $('[name="AppointmentTypeId"]').val();
    },
    appointmentNotes: function () {
      return $('#ta-appt-notes').val();
    },
    appointmentStartTime: function () {
      return $(_p.names.appointmentStartTime).val();
    },
    appointmentEndTime: function () {
      return $(_p.names.appointmentEndTime).val();
    },
    inputClientEmail: function () {
      return $(_p.ids.inputClientEmail).val();
    },
    inputClientFirstName: function () {
      return $(_p.ids.inputClientFirstName).val();
    },
    inputClientLastName: function () {
      return $(_p.ids.inputClientLastName).val();
    },
    inputClientMobile: function () {
      return $(_p.ids.inputClientMobile).val();
    },
    appointmentClientEmail: function () {
      return $(_p.names.appointmentClientEmail).val();
    },
    appointmentClientFirstName: function () {
      return $(_p.names.appointmentClientFirstName).val();
    },
    appointmentClientLastName: function () {
      return $(_p.names.appointmentClientLastName).val();
    },
    schedulerRole: function () {
      return $(_p.ids.schedulerRole).val();
    },
    inputAnonClientEmail: function () {
      return $(_p.ids.inputAnonClientEmail).val();
    },
    inputAnonClientFirstName: function () {
      return $(_p.ids.inputAnonClientFirstName).val();
    },
    inputAnonClientLastName: function () {
      return $(_p.ids.inputAnonClientLastName).val();
    },
    inputAnonClientMobile: function () {
      return $(_p.ids.inputAnonClientMobile).val();
    }
  };
  _p.set = {
    appointmentStartTime: function (startTime) {
      $(_p.names.appointmentStartTime).val(startTime);
    },
    appointmentEndTime: function (endTime) {
      $('[name="EndTime"]').val(endTime);
    },
    appointmentTypeId: function (typeId) {
      $(_p.names.appointmentTypeId).val(typeId);
    },
    reviewAppointmentType: function (text) {
      $('#rev-appt-type-desc').html(text);
    },
    reviewAppointmentNotes: function (notes) {
      $('#rev-appt-notes').html(notes);
    },
    reviewAppointmentTimeZone: function (timezone) {
      $('#rev-appt-time-zone').html(timezone);
    },
    reviewAppointmentDate: function (date) {
      $('#rev-appt-date').html(date);
    },
    reviewAppointmentTime: function (time) {
      $('#rev-appt-time').html(time);
    },
    apptNotes: function (notes) {
      $('[name="Notes"]').val(notes);
    },
    appointmentTypeBreif: function (breif) {
      $('[name="AppointmentTypeBreif"]').val(breif);
    },
    appointmentClientTimeZone: function (clientTimeZone) {
      $('[name="ClientTimeZone"]').val(clientTimeZone);
    },
    inputClientEmail: function (val) {
      return $(_p.ids.inputClientEmail).val(val);
    },
    inputClientFirstName: function (val) {
      return $(_p.ids.inputClientFirstName).val(val);
    },
    inputClientLastName: function (val) {
      return $(_p.ids.inputClientLastName).val(val);
    },
    inputClientMobile: function (val) {
      return $(_p.ids.inputClientMobile).val(val);
    },
    appointmentClientEmail: function (val) {
      return $(_p.names.appointmentClientEmail).val(val);
    },
    appointmentClientFirstName: function (val) {
      return $(_p.names.appointmentClientFirstName).val(val);
    },
    appointmentClientLastName: function (val) {
      return $(_p.names.appointmentClientLastName).val(val);
    },
    appointmentClientMobile: function (val) {
      return $(_p.names.appointmentClientMobile).val(val);
    },
    reviewClientEmail: function (val) {
      return $(_p.ids.reviewClientEmail).html(val);
    },
    reviewClientFirstName: function (val) {
      return $(_p.ids.reviewClientFirstName).html(val);
    },
    reviewClientLastName: function (val) {
      return $(_p.ids.reviewClientLastName).html(val);
    },
    reviewClientMobile: function (val) {
      return $(_p.ids.reviewClientMobile).html(val);
    }
  };
  /// <summary>
  /// Given an appointment type, retrieve available appointment start times
  /// from database.
  /// </summary>
  /// <params name="apptTypeId">Id of appointment type to retrieve availability for</params>
  /// <params name="startRangeDate">Start of date range to retrieve availabilit for</params>
  /// <returns>Availability for appointment types</returns>
  _p.getAvailability = function (apptTypeId, startRangeDate) {
    var topLevelErrorAlert = $('#dev-exception-report');
    topLevelErrorAlert.hide();
    var firstCallFlag = $('#is-first-call');
    var isFirstCall = firstCallFlag.val();
    var businessUserId = $(_p.ids.businessUserId).val();
    var host = window.location.protocol + '//' + window.location.host;
    var scheduleUrl = host + '/Availability/GetAvailabilityForScheduling';
    var getUrl = scheduleUrl
      + '?serviceProviderId=' + businessUserId
      + '&apptTypeId=' + apptTypeId
      + '&startRangeDate=' + startRangeDate;
    if (isFirstCall === "true") {
      getUrl += '&oneWeekOnly=false';
    } else {
      getUrl += '&oneWeekOnly=true';
    }
    var biztz = $(_p.ids.spTimeZone).val();
    var clienttz = _p.get.clientTimeZoneId();
    var spinner = $('#tab-appt-time').find('.spinner-grow');
    var availCards = $('#avail-cards');
    spinner.show();
    availCards.hide();
    availCards.find('.row').html('');
    return $.getJSON(getUrl)
      .done(function (data) {
        var period = data.period;
        var startDateTimes = [];
        var selectableDateTimes = [];
        var availability;
        var concreteDate;
        var spAvailableDate;
        if (clienttz !== biztz) {
          for (var i = 0; i < period.availability.length; i++) {
            availability = period.availability[i];
            concreteDate = availability.concreteDate.split('T')[0];
            if (availability.blocks !== null)
              startDateTimes = _p.createStartDateTimesList(availability.blocks, concreteDate);
            for (var k = 0; k < startDateTimes.length; k++) {
              selectableDateTimes.push(_p.getSelectableTime2(startDateTimes[k], biztz, clienttz));
            }
            startDateTimes.length = 0;
          }
          var thisDaysSelectableTimes = [];
          for (var l = 0; l < period.availability.length; l++) {
            concreteDate = period.availability[l].concreteDate.split('T')[0];
            spAvailableDate = moment.tz(concreteDate, biztz).format(_p.const.showDt);
            for (var m = 0; m < selectableDateTimes.length; m++) {
              if (selectableDateTimes[m].clientDate === spAvailableDate) {
                thisDaysSelectableTimes.push(selectableDateTimes[m]);
              }
            }
            _p.createAvailabilityCard_DiffTz(spAvailableDate, thisDaysSelectableTimes, biztz, period.availability[l].sequence);
            thisDaysSelectableTimes.length = 0;
          }
        } else {
          for (var z = 0; z < period.availability.length; z++) {
            availability = period.availability[z];
            concreteDate = availability.concreteDate.split('T')[0];
            spAvailableDate = moment.tz(concreteDate, biztz).format(_p.const.showDt);
            if (availability.blocks !== null)
              startDateTimes = _p.createStartDateTimesList(availability.blocks, concreteDate);
            // create dates and times with both sp and client
            for (var y = 0; y < startDateTimes.length; y++) {
              selectableDateTimes.push(_p.getSelectableTimeWithoutConversion(startDateTimes[y], biztz));
            }
            _p.createAvailabilityCard_DiffTz(spAvailableDate, selectableDateTimes, biztz, availability.sequence);
            startDateTimes.length = 0;
            selectableDateTimes.length = 0;
          }
        }
        firstCallFlag.val('false');
      })
      .fail(function (data) {
        console.log('e');
        console.log(data.responseText);
        console.log(data);
        topLevelErrorAlert.html(data.responseText).show();
      })
      .always(function () {
        spinner.hide('slide');
        availCards.show('blind');
      });
  };
  /// <summary>
  /// Container to define event handlers.
  /// </summary>
  _p.handle = {
    onClick: {
      /// <summary>
      /// User choses appointment type from list.
      /// </summary>
      apptTypeChanged: function () {
        $('.btn_appt_type').on('click', function (el) {
          var selectEl = $(el.target);
          var apptTypeId = selectEl.attr('id');
          var apptTypeDuration = selectEl.data('duration');
          $('#appt-type-duration').val(apptTypeDuration);
          var apptTypeBrief = selectEl.data('brief-desc');
          _p.set.appointmentTypeId(apptTypeId);
          _p.set.appointmentTypeBreif(apptTypeBrief);
          _p.set.reviewAppointmentType(apptTypeBrief);
          $('#hf-appt-type-location-details').val(selectEl.data('location-details'));
          var locationDescriptions = _p.apptType.location;
          var selectedLocation = selectEl.data('location');
          var locationDesc = 'Not designated';
          switch (selectedLocation) {
            case '-1': locationDesc = 'Not designated';
              break;
            case 'web_conference': locationDesc = 'Web conference';
              break;
            case 'we_call': locationDesc = 'We will call the customer';
              break;
            case 'customer_calls': locationDesc = 'Customer will call us';
              break;
            case 'we_specify': locationDesc = 'We will specifiy the meeting location';
              break;
            case 'customer_specifies': locationDesc = 'Customer will specify the meeting location';
              break;
            default:
          }
          $('#rev-appt-location').html(locationDesc);
          $('#rev-appt-location-details').html(selectEl.data('location-details'));
          $('[name="Location"]').val(selectedLocation);
          if (selectedLocation === 'we_call' || selectedLocation === 'customer_specifies') {
            $('#fg-location-details').show();
          } else {
            $('#fg-location-details').hide();
          }
          if (apptTypeId.length > 0) {
            var secondTab = $('#wizard a#tab-appt-time');
            secondTab.removeClass('disabled');
            secondTab.tab('show');
            location.hash = 'when';
            var greetingWrapper = $('#greeting-wrapper');
            if (greetingWrapper.is(':visible'))
              greetingWrapper.hide('blind', {}, 500, function () {
                $('#appt-guide_what-type-wrapper').show('blind', 500);
              });
            var clientTz = _p.get.clientTimeZoneId();
            var tzToUse = '';
            if (clientTz.length > 0) {
              tzToUse = clientTz;
            } else {
              tzToUse = moment.tz.guess();
            }
            var zonedNow = moment().tz(tzToUse).format();
            _p.getAvailability(apptTypeId, zonedNow)
              .done(function (data) {
                _p.initWeekPicker(data.daysWithAvailability);
                if (data.daysWithAvailability.length > 0) {
                  $('#no-availability-for-range').addClass('d-none');
                } else {
                  $('#no-availability-for-range').removeClass('d-none');
                }
              });
          }
        });
      },
      timeSelected: function () {
        $('body').on('click', 'a.card-link', function (el) {
          var businessTimeZone = _p.get.businessTimeZoneId();
          var selectedTime = $(el.currentTarget).data('sp');
          var clienttz = _p.get.clientTimeZoneId();
          var time = moment.tz(selectedTime, 'M/DD/YY HH:mm:ss', businessTimeZone);
          _p.set.appointmentStartTime(time.format('YYYY-MM-DDTHH:mm:ss'));
          var dur = $('#appt-type-duration').val();
          var duration = moment.duration(dur);
          var endTime = time.clone();
          endTime.add(duration);
          var endDateTime = endTime.clone();
          _p.set.appointmentEndTime(endDateTime.format('YYYY-MM-DDTHH:mm:ss'));
          ////// if necessary, convert time to client's tz and display review
          if (clienttz !== businessTimeZone) {
            // then we are in client tz
            time.tz(clienttz);
            endTime.tz(clienttz);
          }
          _p.set.reviewAppointmentTime(time.format('h:mm a') + ' - ' + endTime.format('h:mm a'));
          _p.set.reviewAppointmentDate(time.format('dddd, MMMM Do YYYY'));
          _p.set.reviewAppointmentTimeZone(clienttz);
          $('#wizard a#tab-info').removeClass('disabled').tab('show');
        });
      },
      infoEntered: function () {
        $('#btn-notes-entered').on('click', function () {
          var notes = _p.get.appointmentNotes();
          _p.set.reviewAppointmentNotes(notes);
          _p.set.apptNotes(notes);
          var clientEmail;
          var clientFirstName;
          var clientLastName;
          var clientMobile;
          var schedulerRole = _p.get.schedulerRole();
          if (schedulerRole === 'AnonymousScheduler') {
            clientEmail = _p.get.inputAnonClientEmail();
            clientFirstName = _p.get.inputAnonClientFirstName();
            clientLastName = _p.get.inputAnonClientLastName();
            _p.set.appointmentClientEmail(clientEmail);
            _p.set.appointmentClientFirstName(clientFirstName);
            _p.set.appointmentClientLastName(clientLastName);
            _p.set.reviewClientEmail(clientEmail);
            _p.set.reviewClientFirstName(clientFirstName);
            _p.set.reviewClientLastName(clientLastName);
          }
          if (schedulerRole === 'Subscriber') {
            //get client selction method
            //2 toggle switches on select existing or manually enter new
            var isSelectExistingChecked = $('#customSwitch1').prop('checked');
            if (isSelectExistingChecked) {
              //get existing client info from select ddl
              //var clientSelect = 
              clientEmail = $('#clientSelectedAsSp option:selected').data('email');
              clientFirstName = $('#clientSelectedAsSp option:selected').data('fname');
              clientLastName = $('#clientSelectedAsSp option:selected').data('lname');
              clientMobile = $('#clientSelectedAsSp option:selected').data('mobile');
            } else {
              clientEmail = _p.get.inputAnonClientEmail();
              clientFirstName = _p.get.inputAnonClientFirstName();
              clientLastName = _p.get.inputAnonClientLastName();
              clientMobile = _p.get.inputAnonClientMobile();
            }
            //console.log('client email: ', clientEmail);
            _p.set.appointmentClientEmail(clientEmail);
            _p.set.appointmentClientFirstName(clientFirstName);
            _p.set.appointmentClientLastName(clientLastName);
            _p.set.appointmentClientMobile(clientMobile);
            //clientEmail = _p.get.inputAnonClientEmail();
            //clientFirstName = _p.get.inputAnonClientFirstName();
            //clientLastName = _p.get.inputAnonClientLastName();
            _p.set.reviewClientEmail(clientEmail);
            _p.set.reviewClientFirstName(clientFirstName);
            _p.set.reviewClientLastName(clientLastName);
            _p.set.reviewClientMobile(clientMobile);
          }
          $('#tab-review').removeClass('disabled').tab('show');
        });
      },
      pickAnotherTime: function () {
        $('#a-pick-another-time').on('click', function () {
          $('#tab-appt-time').removeClass('disabled').tab('show');
        });
      },
      pickAnotherApptType: function () {
        $('#a-pick-another-appt-type').on('click', function () {
          $('#tab-appt-type').removeClass('disabled').tab('show');
        });
      },
      editInformation: function () {
        $('#a-edit-notes').on('click', function () {
          $('#tab-info').removeClass('disabled').tab('show');
        });
      },
      refreshTimesInNewZone: function () {
        $('#btn-refresh-times-after-timezone-changed').on('click', function () {
          var selectedApptTypeId = _p.get.appointmentTypeId();
          var selectedWeekStart = $('#selected-week-start-date').val();
          var btn = $(this);
          var refreshIcon = btn.find('.fa-refresh');
          var spinner = btn.find('.spinner-border');
          refreshIcon.hide();
          spinner.show();
          _p.getAvailability(selectedApptTypeId, selectedWeekStart)
            .done(function (data) {
              $('#current-time').html($('#current-time-on-form').html());
              $('#current-timezone').html(_p.get.appointmentClientTimeZone());
            })
            .always(function () {
              spinner.hide();
              refreshIcon.show();
            });
        });
      },
      formSubmitted: function () {
        $('.btn-save-appt').on('click', function () {
          $('#frm').submit();
        });
      }
    },
    onHover: {
      reviewWhenHeader: function () {
        var link = $('.review-header-link');
        var header = link.closest('.card-header');
        header.hover(
          function () {
            $(this).find('a').show();
          },
          function () {
            $(this).find('a').hide();
          }
        );
      }
    },
    onChange: {
      accordia: function () {
        $('#customSwitch1').on('change', function () {
          var showExisting = $(this).prop('checked');
          if (showExisting) {
            $('#collapseExistingClient').collapse('show');
            $('#collapseManualEntry').collapse('hide');
            $('#customSwitch1').prop('checked', true);
            $('#customSwitch2').prop('checked', false);
          } else {
            $('#collapseManualEntry').collapse('show');
            $('#collapseExistingClient').collapse('hide');
            $('#customSwitch2').prop('checked', true);
            $('#customSwitch1').prop('checked', false);
          }
        });
        $('#customSwitch2').on('change', function () {
          var showManual = $(this).prop('checked');
          if (showManual) {
            $('#collapseManualEntry').collapse('show');
            $('#collapseExistingClient').collapse('hide');
            $('#customSwitch2').prop('checked', true);
            $('#customSwitch1').prop('checked', false);
          } else {
            $('#collapseExistingClient').collapse('show');
            $('#collapseManualEntry').collapse('hide');
            $('#customSwitch1').prop('checked', true);
            $('#customSwitch2').prop('checked', false);
          }
        });
      },
      spSelectsClientFromList: function () {
        $('#clientSelectedAsSp').on('change', function () {
          var clientEmail = $(this).find(':selected').data('email');
          _p.set.inputClientEmail(clientEmail);
          $('#lgi-client-email').html(clientEmail);
        });
      },
      anonEmailEntered: function () {
        if ($('#scheduler-role').val() === 'AnonymousScheduler') {
          $('#tb-anon-email').on('change', function () {
            _p.handleRequiredAnonFields();
          });
        }
      },
      anonFNameEntered: function () {
        if ($('#scheduler-role').val() === 'AnonymousScheduler') {
          $('#tb-anon-fname').on('change', function () {
            _p.handleRequiredAnonFields();
          });
        }
      }
    }
  };
  /// <summary>
  /// Register all event handlers.
  /// </summary>
  _p.handle.register = function () {
    _p.handle.onClick.apptTypeChanged();
    _p.handle.onClick.timeSelected();
    _p.handle.onClick.infoEntered();
    _p.handle.onClick.pickAnotherTime();
    _p.handle.onClick.pickAnotherApptType();
    _p.handle.onClick.editInformation();
    _p.handle.onHover.reviewWhenHeader();
    _p.handle.onClick.refreshTimesInNewZone();
    _p.handle.onChange.anonEmailEntered();
    _p.handle.onChange.anonFNameEntered();
    _p.handle.onClick.formSubmitted();
    _p.handle.onChange.spSelectsClientFromList();
    _p.handle.onChange.accordia();
  };
  _p.initReschedule = function (apptIdToReschedule) {
    var weekStart = moment().startOf('week').format(_p.const.sqlDt);
    $('#wizard a#tab-appt-time').removeClass('disabled').tab('show');
    _p.getAvailability(apptIdToReschedule, weekStart)
      .done(function (data) {
        _p.initWeekPicker(data.daysWithAvailability);
      });
  };
  _p.handleRequiredAnonFields = function () {
    var canProceed = true;
    //if (_p.anonHasEnteredEmail() === false) {
    //  canProceed = false;
    //}
    if (_p.anonHasEnteredFName() === false) {
      canProceed = false;
    }
    $('#myTabContent #info .form-control').each(function (a, b) {
      //console.log('a', a, 'b', b)
      if ($(b).hasClass('input-validation-error') === true) {
        canProceed = false;
      }
    });
    var btnContinue = $('#btn-notes-entered');
    if (canProceed === true) {
      btnContinue.prop('disabled', false);
    } else {
      btnContinue.prop('disabled', true);
    }
  };
  _p.anonHasEnteredFName = function () {
    var fname = $('#tb-anon-fname').val();
    if (fname !== '') {
      return true;
    }
    return false;
  };
  _p.anonHasEnteredEmail = function () {
    var email = $('#tb-anon-email').val();
    if (email !== '') {
      return true;
    }
    return false;
  };
  _p.initWeekPicker = function (availableDates) {
    var start = moment();
    var end = moment();
    var weekStart = start.startOf('week');
    var weekEnd = end.endOf('week');
    var $selectedWeekStart = $('#selected-week-start-date');
    var $selectedWeekEnd = $('#selected-week-end-date');
    var selectableDates = [];
    selectableDates.push(
      //new Date(2019, 10, 13)
      '2019-10-10',
      '2019-10-11'
    );
    $('#week-picker').datepicker({
      showOtherMonths: true,
      onSelect: function (dateText, inst) {
        start = moment(dateText, 'MM/DD/YYYY');
        end = moment(dateText, 'MM/DD/YYYY');
        weekStart = start.startOf('week');
        weekEnd = end.endOf('week');
        $selectedWeekStart.val(weekStart.format(_p.const.sqlDt));
        $selectedWeekEnd.val(weekEnd.format(_p.const.sqlDt));

        $('#btn-week-picker').html(_p.createDatePickerButtonText(weekStart, weekEnd));
        var apptTypeId = _p.get.appointmentTypeId();
        _p.getAvailability(apptTypeId, $selectedWeekStart.val());
      },
      beforeShowDay: function (date) {
        var formattedDate = moment(date).format('YYYY-MM-DD');
        for (var i = 0; i < availableDates.length; i++) {
          if (availableDates[i].split('T')[0] === formattedDate) {
            return [true, '', ''];
          }
        }
        return [false, '', ''];
      },
      onChangeMonthYear: function (year, month, datePickerInstance) { s}
    });
    $('#btn-week-picker').on('click', function (e) {
      $('#week-picker').datepicker('show');
    });
    // initalize before 1st selection even occurs
    $selectedWeekStart.val(weekStart.format(_p.const.sqlDt));
    $selectedWeekEnd.val(weekEnd.format(_p.const.sqlDt));
    $('#btn-week-picker').html(_p.createDatePickerButtonText(weekStart, weekEnd));
  };
  _p.createDatePickerButtonText = function (weekStart, weekEnd) {
    return '<i class="fa fa-calendar"></i>&nbsp;&nbsp;'
      + weekStart.format('LL')
      + ' - '
      + weekEnd.format('LL')
      + ' <i class="feather icon-chevron-down"></i>';
  };
  _p.populateCards = function () { };
  // hanlde case wehn timezones are different
  _p.createAvailabilityCard_DiffTz = function (spAvailableDate, selectableTimes, spTz, sequence) {
    var cardHtml = [];
    var availMoment = moment.tz(spAvailableDate, _p.const.showDt, spTz);
    var availDayOfWeek = availMoment.format('dddd').toLowerCase();
    card = $('#avail-cards div.row');
    card.attr('data-sp-date', availMoment.format(_p.const.sqlDt));
    cardHtml.push('<div class="col-4 col-sm-3 col-md-auto">');
    cardHtml.push(_p.createCardHeader(availMoment.format('ddd'), spAvailableDate));
    if (selectableTimes.length > 0)
      cardHtml.push(_p.createAvailTimeItemList(selectableTimes));
    cardHtml.push('</div>');
    card.append(cardHtml.join(''))
  };
  _p.createAvailTimeItemList = function (selectableTimesList) {
    var html = [];
    html.push('<ul class="list-group list-group-flush" data-client-date="', selectableTimesList[0].clientDate, '" >');
    for (var i = 0; i < selectableTimesList.length; i++) {
      html.push(_p.createTimeItem(selectableTimesList[i]));
    }
    html.push('</ul>');
    return html.join('');
  };
  p.chooseTime = function (spDateTime) {
    alert('date time chosen: ' + spDateTime);
  };
  _p.createTimeItem = function (selectableTime) {
    return '<li class="list-group-item avail-time" '
      + '>'
          + '<a class="card-link" '
          + ' data-sp="'
          + selectableTime.spDate + ' ' + selectableTime.spTime
          + '" '
      + ' href="javascript:void(0);" >'
      + selectableTime.clientTime
      + '</a></li>';
  };
  _p.createCardHeader = function (title, subtitle) {
    var html = [];
    html.push(
      '<div class="card-header">',
      '  <h5 class="card-title">', title, '</h5>',
      '  <h6 class="card-subtitle text-muted">', subtitle, '</h6>',
      '</div>'
    );
    return html.join('');
  };
  _p.createStartDateTimesList = function (blocks, concreteDate) {
    var startTimesList = _p.createStartTimeListFromBlocks(blocks);
    var startDateTimesList = [];
    for (var j = 0; j < startTimesList.length; j++) {
      startDateTimesList.push(concreteDate + ' ' + startTimesList[j]);
    }
    return startDateTimesList;
  };
  _p.createStartTimeListFromBlocks = function (blocks) {
    var startTimesList = [];
    for (var i = 0; i < blocks.length; i++) {
      startTimesList.push(blocks[i].s);
    }
    return startTimesList;
  };
  _p.selectableTime = function (spDate, spTime, clientDate, clientTime) {
    this.spDate = spDate;
    this.spTime = spTime;
    this.clientDate = clientDate;
    this.clientTime = clientTime;
  };
  _p.getSelectableTime2 = function (spAvailableDateTime, spTimeZone, clientTimeZone) {
    var spMoment, clientMoment;
    var spDate, spTime, clientDate, clientTime;
    spMoment = moment.tz(spAvailableDateTime, spTimeZone);
    spDate = spMoment.format(_p.const.showDt);
    spTime = spMoment.format(_p.const.militaryTime);
    clientMoment = spMoment.clone();
    clientMoment.tz(clientTimeZone);
    clientDate = clientMoment.format(_p.const.showDt);
    clientTime = clientMoment.format(_p.const.showTime);
    return new _p.selectableTime(spDate, spTime, clientDate, clientTime);
  };
  _p.getSelectableTimeWithoutConversion = function (spAvailableDateTime, spTimeZone) {
    var spMoment;
    var spDate, spTime, spShowTime;
    spMoment = moment.tz(spAvailableDateTime, spTimeZone);
    spDate = spMoment.format(_p.const.showDt);
    spTime = spMoment.format(_p.const.militaryTime);
    spShowTime = spMoment.format(_p.const.showTime);
    return new _p.selectableTime(spDate, spTime, spDate, spShowTime);
  };
  return p;
})();
$(document).ready(function () { appts_schedule_appt.init(); });