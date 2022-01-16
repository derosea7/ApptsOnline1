
/**
 * Author: Adam James DeRose
 * Date: 1/23/21
 * supports viewing appts on full calendar
 */
var appts_calendar_view = (function () {
  var p = {}, _p = {};
  _p.test = {};
  p.init = function (calendarName) {
    _p.handle.register();
    if (calendarName == 'dashboard') {
      _p.initCalendar_Dashboard();
    }
    else if (calendarName == 'schedule') {
      _p.initCalendar_Schedule();
    }
    //_p.initCalendar(strIntialView);
    //_p.getApptDetail();
    _p.initDetailModal();
  };
  _p.initCalendar_Dashboard = function () {
    //var eventsRaw = JSON.parse($('#appts-json').val());
    // configure calendar
    var y = new Date();
    y.setDate(y.getDate() - 1);
    var calendarEl = document.getElementById('calendar');
    var calendar = new FullCalendar.Calendar(calendarEl, {
      initialView: 'timeGridFourDay',
      initialDate: y.toISOString(),
      headerToolbar: {
        left: 'prev,next today',
        center: 'title',
        right: 'dayGridMonth,timeGridFourDay,timeGridDay'
      },
      views: {
        timeGridFourDay: {
          type: 'timeGrid',
          duration: { days: 4 },
          buttonText: '4 day',
          slotMinTime: '06:00:00',
          slotMaxTime: '22:00:00'
        }
      },
      ///fullcalendar automatically passes startParam and endParam as ?start=...
      eventSources: {
        url: '/Schedule/GetApptsForCalendar',
        method: 'GET'
      }
    });
    calendar.render();
  };
  _p.initCalendar_Schedule = function () {
    // configure calendar
    var y = new Date();
    y.setDate(y.getDate() - 1);
    var calendarEl = document.getElementById('calendar');
    var calendar = new FullCalendar.Calendar(calendarEl, {
      //initialView: 'dayGridMonth',
      headerToolbar: {
        left: 'prev,next today',
        center: 'title',
        right: 'dayGridMonth,timeGridWeek,timeGridDay'
      },
      views: {
        timeGridFourDay: {
          type: 'timeGrid',
          duration: { days: 4 },
          buttonText: '4 day',
          slotMinTime: '06:00:00',
          slotMaxTime: '22:00:00'
        }
      },
      ///fullcalendar automatically passes startParam and endParam as ?start=...
      eventSources: {
        url: '/Schedule/GetApptsForCalendar',
        method: 'GET'
      }
    });
    calendar.render();
  };
  _p.initDetailModal = function () {
    $('body').on('click', '.btn-detail-link', function () {
      _p.getApptDetail()
        .done(function (data) {
          //console.log('aptp', appt);
        })
        .fail(function (data) {
          //console.log('fail from init detail modal');
        });
    });
  };
  _p.getApptDetail = function () {
    var uri = '/Schedule/GetApptsForCalendar?start=20210101&end=20210131'
    return $.ajax({
      url: uri,
      type: 'GET',
      contentType: 'application/json; charset=utf-8'
    })
      .done(function (data) {
        //console.log('success ajax call to /Schedule/GetApptsForCalender', data);
      })
      .fail(function (data) {
        //console.log('failed ajax call to /Schedule/GetApptsForCalender', data);
      });
  };
  _p.getApptEvents = function (start, end) {
    var uri = '/Schedule/GetApptsForCalendar?start=' + start + '&end=' + end;
    return $.ajax({
      url: uri,
      type: 'GET',
      contentType: 'application/json; charset=utf-8'
    })
      .done(function (data) {
        //console.log('success ajax call to /Schedule/GetApptsForCalender', data);
      })
      .fail(function (data) {
        //console.log('failed ajax call to /Schedule/GetApptsForCalender', data);
      });
  };
  _p.createCalendarEvent = function (start, end, title) {
    this.start = start;
    this.end = end;
    this.title = title;
  };
  _p.updateCalendar = function (start, end) {
    
    _p.getApptEvents(start, end)
      .done(function (data) {
        //console.log('success update calendar', data);
        
      })
      .fail(function (data) {
        //console.log('failed to update calendar', data);
      });
  };

  // can be called publically with function passed in
  p.handle = {
    onChange: {
    }
  };
  /// <summary>
  /// Container to define event handlers.
  /// </summary>
  _p.handle = {
    onClick: {
    },
    onChange: {
    },
    onHover: {
    }
  };
  /// <summary>
  /// Register all event handlers.
  /// </summary>
  _p.handle.register = function () {
    //_p.handle.onClick.resetFilters();
    //_p.handle.onClick.presetToday();
    //_p.handle.onClick.presetRange();
    //_p.handle.onHover.apptDetail();
    //_p.handle.onClick.refreshTimesAfterChangingTimeZone();
    //_p.handle.onClick.showMoreDetails();
  };
  return p;
})();
//this js file should be invoked by the specific page on which
// calendar required
//$(document).ready(function () { appts_calendar_view.init(); });
