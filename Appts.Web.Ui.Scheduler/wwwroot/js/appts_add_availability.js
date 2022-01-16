
/**
 * Author: Adam James DeRose
 * Date: 10/24/2019
 * Support adding availability.
 */
var appts_add_availability = (function () {
  var p = {}, _p = {};
  _p.test = {};

  p.init = function () {
    _p.handle.register();
    _p.initDatePickers();
    _p.initTimePickers();
    _p.initInputMasks();
    //_p.initDroppables();  
    _p.initEmptyHint();

    _p.initPeriodFormUi();

    //_p.handleWeeklyAvailabilityChartHeaderClick();
    _p.initPopovers();

    var originalFormState = $('#frmPeriod').serialize();
    originalFormState += _p.serializeBlocksReadyForSave();
    $('#original-form-state').val(originalFormState);
    //original-form-state
  };
  _p.initPopovers = function () {
    $('.popover-dismiss').popover();
  };
  p.chart = {};
  p.daychart = {};

  //_p.handleWeeklyAvailabilityChartHeaderClick = function () {
  //  $('#clps-visualize-availability').on('show.bs.collapse', function () {
  //    console.log('about to show collappse');
  //  });
  //};

  /// <summary>
  /// Refresh the daily gannt chart when user opens or updates
  /// the input modal.
  /// </summary>
  _p.refreshDayGantt = function (chartData) {
    if (p.daychart.baseId !== undefined) {
      // update just the data
      p.daychart.data = chartData;
    } else {
      // need to create the chart
      _p.createAmChart_Day(chartData);
    }
  };

  /// <summary>
  /// Redraw chart with updated data from users selections.
  /// </summary>
  _p.refreshGantt = function () {
    var colorSet = new am4core.ColorSet();
    colorSet.saturation = 0.4;
    var period = {
      availability: []
    };

    //var availability;
    var blocks;
    var days = _p.weekdays;
    var day;
    for (var d = 0; d < days.length; d++) {
      day = days[d];
      period.availability.push({
        dayOfWeek: day,
        blocks: _p.loadBlocksFromDom(day)
      });
    }
    _p.addExtendedAvailabilityProperties2(period);
    var chartData = _p.getGanttAvailabilityChartData(period);

    p.chart.data = chartData;
  };

  _p.testAmChart = function (availabilityData) {
    // Themes begin
    am4core.useTheme(am4themes_animated);
    // Themes end

    //var chart = am4core.create("chartdiv", am4charts.XYChart);
    p.chart = am4core.create("chartdiv", am4charts.XYChart);
    //var chart = p.chart;

    p.chart.hiddenState.properties.opacity = 0; // this creates initial fade-in

    //chart.paddingRight = 30;
    p.chart.dateFormatter.inputDateFormat = "yyyy-MM-dd HH:mm";

    var colorSet = new am4core.ColorSet();
    colorSet.saturation = 0.4;
    p.chart.paddingRight = 50;
    p.chart.data = availabilityData;

    var categoryAxis = p.chart.yAxes.push(new am4charts.CategoryAxis());
    categoryAxis.dataFields.category = "name";
    categoryAxis.renderer.grid.template.location = 0;
    categoryAxis.renderer.inversed = true;

    var dateAxis = p.chart.xAxes.push(new am4charts.DateAxis());
    //dateAxis.dateFormatter.dateFormat = "yyyy-MM-dd HH:mm";
    dateAxis.dateFormatter.dateFormat = "h:mma";
    dateAxis.renderer.minGridDistance = 96;
    dateAxis.baseInterval = { count: 15, timeUnit: "minute" };
    var d = new Date();
    dateAxis.min = new Date(d.getFullYear(), d.getMonth(), d.getDate(), 0, 0, 0).getTime();
    dateAxis.max = new Date(d.getFullYear(), d.getMonth(), d.getDate(), 23, 59, 59).getTime();
    dateAxis.strictMinMax = true;
    dateAxis.renderer.tooltipLocation = 0;

    dateAxis.renderer.labels.template.adapter.add('text', function (text, target) {
      return _p.createDateTimeMoment(text).format('h:mma');
    });

    var series1 = p.chart.series.push(new am4charts.ColumnSeries());
    series1.columns.template.width = am4core.percent(80);
    series1.columns.template.tooltipText = "{name}: {openDateX} - {dateX}";

    series1.dataFields.openDateX = "fromDate";
    series1.dataFields.dateX = "toDate";
    series1.dataFields.categoryY = "name";
    series1.columns.template.propertyFields.fill = "color"; // get color from data
    series1.columns.template.propertyFields.stroke = "color";
    series1.columns.template.strokeOpacity = 1;
    
    p.chart.scrollbarX = new am4core.Scrollbar();
  };

  _p.createAmChart_Day = function (availabilityData) {
    // Themes begin
    am4core.useTheme(am4themes_animated);
    // Themes end

    p.daychart = am4core.create("amchartdaily", am4charts.XYChart);
    p.daychart.hiddenState.properties.opacity = 0; // this creates initial fade-in
    p.daychart.dateFormatter.inputDateFormat = "yyyy-MM-dd HH:mm";

    var colorSet = new am4core.ColorSet();
    colorSet.saturation = 0.4;
    p.daychart.paddingRight = 30;
    p.daychart.data = availabilityData;

    var categoryAxis = p.daychart.yAxes.push(new am4charts.CategoryAxis());
    categoryAxis.dataFields.category = "name";
    categoryAxis.renderer.grid.template.location = 0;
    categoryAxis.renderer.inversed = true;

    var dateAxis = p.daychart.xAxes.push(new am4charts.DateAxis());
    //dateAxis.dateFormatter.dateFormat = "yyyy-MM-dd HH:mm";
    dateAxis.dateFormatter.dateFormat = "h:mma";
    dateAxis.renderer.minGridDistance = 96;
    dateAxis.baseInterval = { count: 15, timeUnit: "minute" };
    var d = new Date();
    dateAxis.min = new Date(d.getFullYear(), d.getMonth(), d.getDate(), 0, 0, 0).getTime();
    dateAxis.max = new Date(d.getFullYear(), d.getMonth(), d.getDate(), 23, 59, 59).getTime();
    dateAxis.strictMinMax = true;
    dateAxis.renderer.tooltipLocation = 0;

    dateAxis.renderer.labels.template.adapter.add('text', function (text, target) {
      return _p.createDateTimeMoment(text).format('h:mma');
    });

    var series1 = p.daychart.series.push(new am4charts.ColumnSeries());
    series1.columns.template.width = am4core.percent(80);
    series1.columns.template.tooltipText = "{name}: {openDateX} - {dateX}";

    series1.dataFields.openDateX = "fromDate";
    series1.dataFields.dateX = "toDate";
    series1.dataFields.categoryY = "name";
    series1.columns.template.propertyFields.fill = "color"; // get color from data
    series1.columns.template.propertyFields.stroke = "color";
    series1.columns.template.strokeOpacity = 1;

    //p.daychart.scrollbarX = new am4core.Scrollbar();
  };

  _p.getChartData = function () {

  };

  _p.createDateTimeMoment = function (timeSpan) {
    var n = moment(moment().format('YYYY-MM-DD'), 'YYYY-MM-DD');
    var t = moment.duration(timeSpan);
    var nt = n.add(t);
    return nt;
  };

  _p.initEmptyHint = function () {
    var mondayCard = $('.card#Monday');
    mondayCard.find('.cl_add_time_block').removeClass('btn-secondary').addClass('btn-success');
    mondayCard.find('.empty-hint').html('<i class="text-secondary");><b>Click Add<br/>below<b></i>');
  };

  _p.const = {

  };

  _p.ids = {

  };

  _p.names = {

  };

  _p.get = {
    errMsg_newBlockOverlapsWithCurrentBlockFront:
      'The time you\'ve entered overlaps with the start of the highlighted block.',
    errMsg_newBlockOverlapsWithCurrentBlockEnd:
      'The time you\'ve entered overlaps with the end of the highlighted block.',
    errMsg_newBlockEncompassesCurrentBlock:
      'The time you\'ve entered encompasses the highlighted block.',
    errMsg_newBlockEncompassedByCurrentBlock:
      'The time you\'ve entered falls within the highlighted block.',

    // period validation
    errMsg_newPeriodOverlapsWithExistingPeriodFront:
      'The period you\'ve entered overlaps with the start of an existing period.',
    errMsg_newPeriodOverlapsWithExistingPeriodEnd:
      'The period you\'ve entered overlaps with the end of an existing period.',
    errMsg_newPeriodEncompassesExistingPeriod:
      'The period you\'ve entered encompasses an existing period.',
    errMsg_newPeriodEncompassedByExistingPeriod:
      'The period you\'ve entered falls within an existing period.'
  };
  _p.set = {

  };
  _p.generateHiddenInput = function (name, value) {
    return '<input type="hidden" name="' + name + '" value="' + value + '" />';
  };
  _p.weekdays = [
    'Sunday',
    'Monday',
    'Tuesday',
    'Wednesday',
    'Thursday',
    'Friday',
    'Saturday'
  ];

  p.submitForm = function (event) {
    var weekDays = [
      'Sunday',
      'Monday',
      'Tuesday',
      'Wednesday',
      'Thursday',
      'Friday',
      'Saturday'
    ];
    var shadow = $('#shadow');
    var blocks = [];
    for (var i = 0; i < weekDays.length; i++) {
      blocks = _p.loadBlocksFromDom(weekDays[i]);
      shadow.append(_p.generateHiddenInput('AvailabilityDays[' + i + '].DayOfWeek', weekDays[i]));
      for (j = 0; j < blocks.length; j++) {
        shadow.append(_p.generateHiddenInput('AvailabilityDays[' + i + '].Blocks[' + j + '].StartTime', moment(blocks[j].s, 'h:mma').format('HH:mm:ss')));
        shadow.append(_p.generateHiddenInput('AvailabilityDays[' + i + '].Blocks[' + j + '].EndTime', moment(blocks[j].e, 'h:mma').format('HH:mm:ss')));
      }
    }

    //event.preventDefault();
    //event.stopPropagation();
  };
  _p.serializeBlocksReadyForSave = function () {
    var weekDays = [
      'Sunday',
      'Monday',
      'Tuesday',
      'Wednesday',
      'Thursday',
      'Friday',
      'Saturday'
    ];
    var blocks = [];
    var cereal = '';
    for (var i = 0; i < weekDays.length; i++) {
      blocks = _p.loadBlocksFromDom(weekDays[i]);
      for (j = 0; j < blocks.length; j++) {
        cereal += '' + i + blocks[j].s + blocks[j].e;
      }
    }
    return cereal;
  };
  /// <summary>
  /// Called to hanlde editing existing period, failed POST during add.
  /// </summary>
  _p.initPeriodFormUi = function () {
    var period = _p.loadJson('hf-period-json');
    if (period !== undefined) {
      _p.addExtendedAvailabilityProperties(period);
      _p.initAvailabilityCardsUi(period);

      //TODO: lazy load gantt chart when shown and when refreshed
      //or maybe better idea to greedily load when period data from json
      //is available?
      var chartData = _p.getGanttAvailabilityChartData(period);
      _p.testAmChart(chartData);
    }
  };
  _p.dayColor = function (day) {
    switch (day) {
      case 'Sunday':
        return '#517dbb';
      case 'Monday':
        return '#666699';
      case 'Tuesday':
        return '#993366';
      case 'Wednesday':
        return '#999966';
      case 'Thursday':
        return '#666633';
      case 'Friday':
        return '#339966';
      case 'Saturday':
        return '#5fc169';
      default:
        return '#dedede';
    }
  };
  _p.getGanttAvailabilityChartData_DayOnly = function (availabilityDay) {
    var chartData = [], block;
    var colorSet = new am4core.ColorSet();
    colorSet.saturation = 0.4;

    if (availabilityDay.blocks !== null && availabilityDay.blocks.length > 0) {
      for (var j = 0; j < availabilityDay.blocks.length; j++) {
        block = availabilityDay.blocks[j];
        chartData.push({
          name: availabilityDay.dayOfWeek.substring(0, 3),
          fromDate: block.ganttStartDate,
          fromDateChartFormatted: block.fromDateChartFormatted,
          toDate: block.ganttEndDate,
          color: _p.dayColor(availabilityDay.dayOfWeek)
        });
      }
    }

    return chartData;
  };

  _p.getGanttAvailabilityChartData = function (period) {
    var chartData = [], availabilityDay, block;
    var colorSet = new am4core.ColorSet();
    colorSet.saturation = 0.4;
    for (var i = 0; i < period.availability.length; i++) {
      availabilityDay = period.availability[i];
      if (availabilityDay.blocks !== null && availabilityDay.blocks.length > 0) {
        for (var j = 0; j < availabilityDay.blocks.length; j++) {
          block = availabilityDay.blocks[j];
          chartData.push({
            name: availabilityDay.dayOfWeek.substring(0, 3),
            fromDate: block.ganttStartDate,
            fromDateChartFormatted: block.fromDateChartFormatted,
            toDate: block.ganttEndDate,
            color: _p.dayColor(availabilityDay.dayOfWeek)
          });
        }
      } else {
        chartData.push({name: availabilityDay.dayOfWeek.substring(0, 3)})
      }
    }
    return chartData;
  };

  /// <summary>
  /// When page is loaded with availability days on a GET, this method
  /// will take the JSON and print it out to UI.
  /// Could be GET on invalid POST, or GET to edit existing availability.
  /// </summary>
  /// <param name="period">Period containing availability data to reflect in UI</param>
  _p.initAvailabilityCardsUi = function (period) {
    _p.updateAvailabilityCardsUi(period);
  };

  /// <summary>
  /// Load Json from hidden field.
  /// </summary>
  /// <param name="fieldName">Name if hidden input with json.</param>
  /// <returns>Object parsed from json in hidden input.</returns>
  _p.loadJson = function (fieldName) {
    var json = $('#' + fieldName).val();
    var result;
    if (json !== '' && json.trim().length > 0) {
      result = JSON.parse(json);
    }

    return result;
  };

  /// <summary>
  /// Given a period, add extended properties to the availability,
  /// which include formatted dates required for certain tasks.
  /// </summary>
  _p.addExtendedAvailabilityProperties = function (period) {
    var availabilityDay;
    for (var i = 0; i < period.availability.length; i++) {
      availabilityDay = period.availability[i];
      if (availabilityDay.blocks !== null) {
        period.availability[i].blocks = _p.addFormattedPropertiesToBlocks(availabilityDay);
      }
    }
  };

  /// <summary>
  /// Given a period, add extended properties to the availability,
  /// which include formatted dates required for certain tasks.
  /// </summary>
  _p.addExtendedAvailabilityProperties2 = function (period) {
    var availabilityDay;
    for (var i = 0; i < period.availability.length; i++) {
      availabilityDay = period.availability[i];
      if (availabilityDay.blocks !== null) {
        period.availability[i].blocks = _p.addFormattedPropertiesToBlocks2(availabilityDay);
      }
    }
  };

  /// <summary>
  /// Given a period, add extended properties to the availability,
  /// which include formatted dates required for certain tasks.
  /// </summary>
  _p.addExtendedAvailabilityProperties3 = function (availabilityDay) {
    if (availabilityDay.blocks !== null && availabilityDay.blocks.length > 0) {
      availabilityDay.blocks = _p.addFormattedPropertiesToBlocks2(availabilityDay);
    }
  };

  /// <summary>
  /// Given availability data, refresh the main display.
  /// </summary>
  /// <param name="availability">Object array to reflect in UI.</param>
  _p.updateAvailabilityCardsUi = function (period) {
    var availabilityDay;
    for (var i = 0; i < period.availability.length; i++) {
      availabilityDay = period.availability[i];
      if (availabilityDay.blocks !== null) {
        _p.updateAvailabilityDisplay(availabilityDay.dayOfWeek, availabilityDay.blocks);
        _p.updateAvailabilityContext(availabilityDay.dayOfWeek, availabilityDay.blocks.length);
      }
    }
  };

  /// <summay>
  /// Given a .NET Block, copy to an array with formatted date property.
  /// </summary>
  /// <param name="timeblocks">
  /// <returns>Copy of source array, additionally containing moment.js formatted dates.</returns>
  _p.addFormattedPropertiesToBlocks = function (availabilityDay) {
    var formattedBlocks = [];
    if (availabilityDay.blocks !== null) {
      for (var j = 0; j < availabilityDay.blocks.length; j++) {
        var startMoment = moment(availabilityDay.blocks[j].s, 'HH:mm:ss');
        var endMoment = moment(availabilityDay.blocks[j].e, 'HH:mm:ss');
        formattedBlocks.push({
          s: availabilityDay.blocks[j].s,
          e: availabilityDay.blocks[j].e,
          formattedStart: startMoment.format('h:mma'),
          formattedEnd: endMoment.format('h:mma'),
          ganttStartDate: _p.createDateTimeMoment(availabilityDay.blocks[j].s).format('YYYY-MM-DD HH:mm'),
          ganttEndDate: _p.createDateTimeMoment(availabilityDay.blocks[j].e).format('YYYY-MM-DD HH:mm')
        });
      }
    }

    return formattedBlocks;
  };

  /// <summay>
  /// Given a .NET Block, copy to an array with formatted date property.
  /// </summary>
  /// <param name="timeblocks">
  /// <returns>Copy of source array, additionally containing moment.js formatted dates.</returns>
  _p.addFormattedPropertiesToBlocks2 = function (availabilityDay) {
    var formattedBlocks = [];
    if (availabilityDay.blocks !== null) {
      for (var j = 0; j < availabilityDay.blocks.length; j++) {
        var startMoment = moment(availabilityDay.blocks[j].s, 'h:mma');
        var endMoment = moment(availabilityDay.blocks[j].e, 'h:mma');
        var startDur = startMoment.format('HH:mm');
        var endDur = endMoment.format('HH:mm');
        formattedBlocks.push({
          s: availabilityDay.blocks[j].s,
          e: availabilityDay.blocks[j].e,
          formattedStart: startMoment.format('h:mma'),
          formattedEnd: endMoment.format('h:mma'),
          ganttStartDate: _p.createDateTimeMoment(startDur).format('YYYY-MM-DD HH:mm'),
          ganttEndDate: _p.createDateTimeMoment(endDur).format('YYYY-MM-DD HH:mm')
        });
      }
    }

    return formattedBlocks;
  };

  _p.initDatePickers = function () {
    var dpStart = $('#date-start');
    var dpEnd = $('#date-end');

    // load period dates json from DOM
    //hf-period-dates-json
    var periodDates = _p.loadJson('hf-period-dates-json');

    dpStart.datepicker({
      minDate: new Date(),
      //dateFormat: 'm/d/yy',
      dateFormat: 'DD, MM d, yy',
      altField: '#hf-period-start',
      altFormat: 'yy-mm-dd',
      //onChangeMonthYear: function (year, month, inst) {
      //  console.log('year, month', year, month)
      //},
      beforeShowDay: function (date) {
        var formattedDate = moment(date);

        // for each period ..
        // if date starts on or after period date
        // or date ends on or before period date
        for (var i = 0; i < periodDates.length; i++) {
          var periodStartDateMoment = moment(periodDates[i].PeriodStart.split('T')[0], 'YYYY-MM-DD');
          var periodEndDateMoment = moment(periodDates[i].PeriodEnd.split('T')[0], 'YYYY-MM-DD');

          if (formattedDate.isBetween(periodStartDateMoment, periodEndDateMoment, null, '[]')) {
            //console.log('formattedDate is between period', formattedDate.format());
            return [false, '', ''];
          }
        }

        // break when done. date should only appear once in any period
        return [true, '', ''];
      },
      onSelect: function (dateText, inst) {
        dpEnd.datepicker('option', 'minDate', dateText);

        _p.validatePeriodDatesSelected();
        _p.handle.onChange.formUpdated();
      }
    });

    dpEnd.datepicker({
      minDate: new Date(),
      //dateFormat: 'm/d/yy',
      dateFormat: 'DD, MM d, yy',
      altField: '#hf-period-end',
      altFormat: 'yy-mm-dd',
      beforeShowDay: function (date) {
        var formattedDate = moment(date);

        // for each period ..
        // if date starts on or after period date
        // or date ends on or before period date
        for (var i = 0; i < periodDates.length; i++) {
          var periodStartDateMoment = moment(periodDates[i].PeriodStart.split('T')[0], 'YYYY-MM-DD');
          var periodEndDateMoment = moment(periodDates[i].PeriodEnd.split('T')[0], 'YYYY-MM-DD');
          if (formattedDate.isBetween(periodStartDateMoment, periodEndDateMoment, null, '[]')) {
            return [false, '', ''];
          }
        }

        // break when done. date should only appear once in any period
        return [true, '', ''];
      },
      onSelect: function (dateText, inst) {
        dpStart.datepicker('option', 'maxDate', dateText);
        _p.handle.onChange.formUpdated();
      }
    });
  };

  _p.initTimePickers = function () {
    _p.initTimePicker($('#start-time'))
      .on('change', function (e, date) {
        _p.prePopulateEndTimeIfEmpty(date);
      });
    _p.initTimePicker($('#end-time'));
  };
  _p.prePopulateEndTimeIfEmpty = function (startTime) {
    var endTime = $('#end-time');
    if (endTime.val() === '') {
      $('#end-time').val(startTime.add(60, 'minutes').format('h:mma'));
    }
  };
  _p.initTimePicker = function ($tpInput) {
    return $tpInput.bootstrapMaterialDatePicker({
      date: false,
      format: 'h:mma',
      shortTime: true
    }).on('change', function (e, date) {
      console.log('time selected', date.minutes())

      // selected minutes closer to next quarter
      var minutesAfterQuarter = date.minutes() % 15;
      if (minutesAfterQuarter >= 8) {
        date.add(15 - minutesAfterQuarter, 'minutes');
      } else {
        date.subtract(minutesAfterQuarter, 'minutes');
      }

      $tpInput.val(date.format('h:mma'));
    });
  };
  _p.initInputMasks = function () {
    _p.initTimeInputMask($('#start-time'));
    _p.initTimeInputMask($('#end-time'));
  };
  _p.initTimeInputMask = function ($timeInput) {
    $timeInput.inputmask({
      mask: "9{1,2}:99a{0,2}",
      greedy: false
    });
  };
  /// <summary>
  /// Called on time picker modal to determine if data has changed on modal.
  /// </summary>
  /// <param name="blocksInDom">Array of start and end times to serialize</param>
  /// <returns>string of serialized start and end times that can be compared when modal is updated</returns>
  _p.serializeModalBlocks = function (blocksInDom) {
    var serializedBlocks = '';
    for (var i = 0; i < blocksInDom.length; i++) {
      serializedBlocks += '' + blocksInDom[i].s + blocksInDom[i].e;
    }
    return serializedBlocks;
  };
  /// <summary>
  /// Container to define event handlers.
  /// </summary>
  _p.handle = {
    onClick: {

      /// <summary>
      /// Opens modal so user can edit days availability.
      /// Will try to copy any existing availability to modal.
      /// </summary>
      manageDailyAvailability: function () {
        $('.cl_add_time_block').on('click', function () {
          // remove hint context from UI now that user knows how to add / edit times
          _p.removeHintContext();


          var dow = $(this).closest('.card').attr('id');
          var mdl = $('#mdl-manage-daily-availability');
          mdl.find('#dow').html(dow);
          mdl.data('dow', dow);

          // clear modal table
          var tbl = mdl.find('#tbl-blocks tbody');
          tbl.html('');

          // clear inputs
          mdl.find('#start-time').val('');
          mdl.find('#end-time').val('');

          // load times from DOM
          var blocksInDom = _p.loadBlocksFromDom(dow);
          console.log('block in dom origianl state', blocksInDom);
          // load times into modal
          for (var i = 0; i < blocksInDom.length; i++) {
            tbl.append('<tr>' + _p.generateEntryHtml(blocksInDom[i].s, blocksInDom[i].e) + '</tr>');
          }
          //create serialized blocks for comparison and unsaved changes alert
          var serializedBlocks = _p.serializeModalBlocks(_p.getBlocksFromModalDom());
          console.log('serialized blocks', serializedBlocks);
          $('#hf-original-serialized-modal-blocks').val(serializedBlocks);
          var availability = {
            blocks: blocksInDom,
            dayOfWeek: dow
          };

          // need to get to gant line.
          _p.addExtendedAvailabilityProperties3(availability);
          var chartData = _p.getGanttAvailabilityChartData_DayOnly(availability);
          _p.refreshDayGantt(chartData);

          mdl.modal('show');
        });
      },

      /// <summary>
      /// When user selects time in modal, this will validate the times and
      /// add to table in modal.
      /// </summary>
      addBlockEntry: function () {
        $('#btn-add-block').on('click', function () {
          //time picker inputs, not using as of 2/21/2021
          //var start = $('#start-time').val();
          //var end = $('#end-time').val();

          //console.log('start', start)
          //console.log('end', end)

          var sminsraw = $('#start-time-list').val();
          var eminsraw = $('#end-time-list').val();
          // validate input is valid time block
          if (sminsraw === '-1' || eminsraw === '-1') {
            $('#block-validation-alert').html('Must enter both start and end time.');
            $('#block-validation-alert').fadeIn();
            return;
          }
          var shrs = parseInt(sminsraw / 60, 10);
          var smins = sminsraw % 60;
          var mstart = moment().hours(shrs).minutes(smins);
          var start = mstart.format('h:mma');
          var ehrs = parseInt(eminsraw / 60, 10);
          var emins = eminsraw % 60;
          var mend = moment().hours(ehrs).minutes(emins);
          var end = mend.format('h:mma');
          var ms = moment(start, 'h:mma');
          var me = moment(end, 'h:mma');

          if (!ms.isValid || !me.isValid) {
            $('#block-validation-alert').html('Start or end time is not a valid time.');
            $('#block-validation-alert').fadeIn();

            return;
          }

          if (me.isSameOrBefore(ms)) {
            $('#block-validation-alert').html('End time must be after start time.');
            $('#block-validation-alert').fadeIn();
            $('#end-time').addClass('is-invalid');
            $('#spn-end-time-validation').show();

            return;
          }

          var existingBlocks = _p.getBlocksFromModalDom();
          _p.clearModelEntryContext();
          var isValidInput = _p.validateInputAgainstOtherTimes(ms, me, existingBlocks);

          isValidInput && $('#tbl-blocks tbody').append('<tr>' + _p.generateEntryHtml(start, end) + '</tr>');

          var blocks = _p.getBlocksFromModalDom();

          //// calculate minutes scheduled
          //var freeMinutes = _p.calculateTotalMinutesInBlocks(blocks);
          //_p.recalculateFreeBusyProgressBar(freeMinutes);
          var mdl = $('#mdl-manage-daily-availability');
          var dow = mdl.data('dow');
          var availability = {
            blocks: blocks,
            dayOfWeek: dow
          };

          // need to get to gant line.
          _p.addExtendedAvailabilityProperties3(availability);
          var chartData = _p.getGanttAvailabilityChartData_DayOnly(availability);
          _p.refreshDayGantt(chartData);

          //// sort entries
          _p.reprintBlocksInModalSorted();
          _p.handle.onChange.modalFormChanged();
          $(this).removeClass('pulse-button');
        });
      },

      /// <summary>
      /// Transfer times from modal to page when user done editing a given day.
      /// </summary>
      saveUpdatedAvailabilityToDom: function () {
        var btn = $('#btn-save-blocks');
        btn.on('click', function () {
          var blocks = _p.getBlocksFromModalDom();

          // target card / dow availability
          var dayOfWeek = $('#mdl-manage-daily-availability').data('dow');
          _p.updateAvailabilityDisplay(dayOfWeek, blocks);
          _p.updateAvailabilityContext(dayOfWeek, blocks.length);
          _p.handle.onChange.formUpdated();
        });
      },
      saveUpdatedAvailability: function () {
        var btn = $('#btn-save-blocks');
        //var btnSpinner = btn.find('.spinner-grow');
        //var btnText = btn.find('.btn-text');
        //var successAlert = $('#update-whitelist-success-msg');
        btn.on('click', function () {
          //btnSpinner.show();
          //btnText.hide();
          //successAlert.hide();
          //var whitelist = [];
          //$('#tbl-blocks tbody td:nth-child(1)').each(function (i, e) {
          //  whitelist.push($(e).html());
          //});

          var blocks = [];
          $('#tbl-blocks tbody tr').each(function (i, e) {
            var tr = $(e);
            var start = tr.find('td:nth-child(1)').html();
            var end = tr.find('td:nth-child(2)').html();
            //blocks.push(new _p.AvailabilityBlock(start, end));
            //blocks.push({ s: '19:30:00', e: '09:20:20' });

            blocks.push({
              s: moment(start, 'h:mma').format('HH:mm:ss'),
              e: moment(end, 'h:mma').format('HH:mm:ss')
            });

          });

          console.log('blocks', blocks);

          _p.saveBlocks(blocks)
            .done(function (data) {
              console.log('s', data);
              //$('#tb-new-whitelist-entry').val('');
              //successAlert.show();
            })
            .fail(function (data) {
              //console.log('e', data);
              console.log('e', data.responseText);

            })
            .always(function () {
              //btnSpinner.hide();
              //btnText.show();
            });
        });
      },
      removeEntry: function () {
        $('body').on('click', '#tbl-blocks tbody tr button', function () {
          $(this).closest('tr').remove();
          _p.handle.onChange.modalFormChanged();
        });
      },

      showCopyDayModal: function () {
        $('.copy_day').on('click', function () {
          var dow = $(this).closest('.card').attr('id');
          var mdl = $('#mdl-copy-day');
          var weekdays = _p.weekdays;

          // save source day
          mdl.data('sourceDay', dow);

          var optionsHtml = [];
          for (var i = 0; i < weekdays.length; i++) {
            if (weekdays[i] !== dow) {
              optionsHtml.push(_p.generateOption(weekdays[i]));
            }
          }

          $('#sl-days-to-copy-to').html(optionsHtml.join(''));
          mdl.find('#dow').html(dow);
          mdl.modal('show');
        });
      },

      /// <summary>
      /// Copy times from source day to targets.
      /// </summary>
      copyDays: function () {
        $('#btn-copy-days').on('click', function () {
          var selectedDays = $('#sl-days-to-copy-to').val();
          var sourceDay = $('#mdl-copy-day').data('sourceDay');
          if (selectedDays.length > 0) {
            var dayOfWeek;
            var overwrite = $('#cb-overwrite-on-copy').is(':checked');
            var sourceBlocks = _p.loadBlocksFromDom(sourceDay);
            var formattedSourceBlocks = [];
            for (var j = 0; j < sourceBlocks.length; j++) {
              formattedSourceBlocks.push({
                formattedStart: sourceBlocks[j].s,
                formattedEnd: sourceBlocks[j].e
              });
            }
            var targetBlocks;
            for (var i = 0; i < selectedDays.length; i++) {
              dayOfWeek = selectedDays[i];
              if (overwrite !== true) {
                targetBlocks = _p.loadBlocksFromDom(dayOfWeek);
                if (targetBlocks === null || targetBlocks.length === 0) {
                  _p.updateAvailabilityDisplay(dayOfWeek, formattedSourceBlocks);
                  _p.updateAvailabilityContext(dayOfWeek, formattedSourceBlocks.length);
                }
              } else {
                _p.updateAvailabilityDisplay(dayOfWeek, formattedSourceBlocks);
                _p.updateAvailabilityContext(dayOfWeek, formattedSourceBlocks.length);
              }
            }
          }
          _p.handle.formUpdated();
        });
      },

      refreshGantt: function () {
//        _p.refresh
        $('#btn-refresh-gantt').on('click', function () {
          _p.refreshGantt();
        });
      }
    },
    onHover: {

    },
    onChange: {

      newBlockStartTimeSelect: function () {
        $('#start-time-list').on('change', function (e) {
          //check if end time selected
          // if so, hint the plus button
          console.log('start list hange', $(this).val())
          if ($(this).val() !== '-1' && $('#end-time-list').val() !== '-1') {
            $('#btn-add-block').addClass('pulse-button');
          } else {
            $('#btn-add-block').removeClass('pulse-button');
          }
        });
      },
      newBlockEndTimeSelect: function () {
        $('#end-time-list').on('change', function () {
          //check if end time selected
          // if so, hint the plus button
          if ($(this).val() !== '-1' && $('#start-time-list').val() !== '-1') {
            $('#btn-add-block').addClass('pulse-button');
          } else {
            $('#btn-add-block').removeClass('pulse-button');
          }
        });
      },

      /// <summary>
      /// Handle user's selection of day to copy to.
      /// </summary>
      selecteDayToCopy: function () {
        var copyBtn = $('#btn-copy-days');
        $('body').on('change', '#sl-days-to-copy-to', function () {
          if ($(this).val().length > 0)
            copyBtn.prop('disabled', false);
        });
      },

      activeStatusChanged: function () {
        $('#switch-active').on('change', function () {
          _p.handle.onChange.formUpdated();
        });
      },

      modalFormChanged: function () {
        var modalBlocks = _p.getBlocksFromModalDom();
        var newFormState = _p.serializeModalBlocks(modalBlocks);
        var originalFormState = $('#hf-original-serialized-modal-blocks').val();
        var btn = $('#btn-save-blocks');
        if (originalFormState !== newFormState) {
          btn.prop('disabled', false);
          btn.addClass('pulse-button');
          $('#modal-has-unsaved-changes').show();
        } else {
          btn.prop('disabled', true);
          btn.removeClass('pulse-button');
          $('#modal-has-unsaved-changes').hide();
        }
      },

      formUpdated: function () {
        if (_p.validateForm() === true) {
          var newFormState = $('#frmPeriod').serialize();
          newFormState += _p.serializeBlocksReadyForSave();
          var originalFormState = $('#original-form-state').val();
          if (newFormState !== originalFormState) {
            $('#unsaved-changes-alert').show();
          } else {
            $('#unsaved-changes-alert').hide();
          }

          $('#btn-submit-save-period').prop('disabled', false);
        } else {
          //$('#btn-submit-save-period').prop('disabled', true);
        }
      }
    },
    updatedPeriodRange: function (periodStartMoment, periodEndMoment) {
      var stat = $('#b-total-days-in-period');
      var days = moment.duration(periodEndMoment.diff(periodStartMoment)).as('days');

      stat.html(days);
    }
  };

  /// <summary>
  /// Check if form is valid to submit; if so, show save button.
  /// Assumption is that validation happening during the individual change events.
  /// </summary>
  /// <returns>True if form in valid state, false otherwise</returns>
  _p.validateForm = function () {
    var startValid = false;
    var endValid = false;
    var hasAtLeastOneBlock = false;

    // validate dates
    if ($('#date-start').val().trim().length > 0)
      startValid = true;

    if ($('#date-end').val().trim().length > 0)
      endValid = true;

    // validate that at least one day has time associated to it
    if ($('.availability-card .list-group-item').length > 0)
      hasAtLeastOneBlock = true;

    if (startValid && endValid && hasAtLeastOneBlock)
      return true;
    else
      return false;
  };

  _p.generateOption = function (dow) {
    return '<option value="' + dow + '" >' + dow + '</option>';
  };

  _p.removeHintContext = function () {
    //$('.card#Monday .cl_add_time_block').hasClass('')
    var hasHint = $('#hf-has-hint').val() === 'true' ? true : false;
    console.log('hasHint', hasHint);

    if (hasHint) {
      var mondayCard = $('.card#Monday');
      mondayCard.find('.cl_add_time_block').removeClass('btn-success').addClass('btn-secondary');
      mondayCard.find('.empty-hint').html('empty');
    }
  };

  /// <summary>
  /// Show / hide copy button and empty hint context after
  /// user adds / edits availability for a day.
  /// </summary>
  _p.updateAvailabilityContext = function (dayOfWeek, blockCount) {
    if (blockCount > 0) {
      $('.card#' + dayOfWeek + ' .copy_day').show();
      $('.card#' + dayOfWeek + ' .empty-hint').hide();
      $('.card#' + dayOfWeek).find('.dow-legend-marker').show();
    } else {
      $('.card#' + dayOfWeek + ' .copy_day').hide();
      $('.card#' + dayOfWeek + ' .empty-hint').show();
      $('.card#' + dayOfWeek).find('.dow-legend-marker').hide();
    }
  };

  /// <summary>
  /// Transfer times selected by user in modal to availability card for day of week.
  /// </summary>
  /// <param name="dayOfWeek">
  /// E.g. Sunday. Used to select availability card in DOM.
  /// </param>
  /// <param name="blocks">
  /// Times to write out into day of week card.
  /// </param>
  _p.updateAvailabilityDisplay = function (dayOfWeek, blocks) {
    var ul = $('.card#' + dayOfWeek + ' .list-group');
    var liHtml = [];
    for (var i = 0; i < blocks.length; i++) {
      liHtml.push('<li class="list-group-item">' + blocks[i].formattedStart + ' - ' + blocks[i].formattedEnd + '</li>');
    }

    ul.html(liHtml.join(''));
  };

  _p.validatePeriodDatesSelected = function () {
    var validationAlert = $('#period-validation-alert');
    validationAlert.hide();
    var ds, de;
    ds = $('[name="PeriodStart"]').val();
    de = $('[name="PeriodEnd"]').val();

    var pds, pde;
    if (ds !== '' && de !== '') {
      var periodDatesJson = [];
      var $periods = $('#hf-period-dates-json');
      if ($periods.length > 0) {
        periodDatesJson = JSON.parse($('#hf-period-dates-json').val());
      }

      var newStart, newEnd;
      var existingStart, existingEnd;
      var sqlFormat = 'YYYY-MM-DD';

      newStart = moment(ds, sqlFormat);
      newEnd = moment(de, sqlFormat);

      if (!newStart.isValid() || !newEnd.isValid()) {
        validationAlert.html('Start or end is not a valid date.').show();

        return;
      }

      // TODO: move to more appropriate place in code
      _p.handle.updatedPeriodRange(newStart, newEnd);

      for (var i = 0; i < periodDatesJson.length; i++) {
        pds = periodDatesJson[i].PeriodStart.split('T')[0];
        pde = periodDatesJson[i].PeriodEnd.split('T')[0];
        existingStart = moment(pds, sqlFormat);
        existingEnd = moment(pde, sqlFormat);

        // eval start
        var newStartBeforeExistingStart = newStart.isSameOrBefore(existingStart);
        var newStartBetween = newStart.isBetween(existingStart, existingEnd, null, '[]');

        // eval end
        var newEndAfterExistingEnd = newEnd.isSameOrAfter(existingEnd);
        var newEndBetween = newEnd.isBetween(existingStart, existingEnd, null, '[]');

        // eval start and end
        var newPeriodOverlapsWithBeginningOfExisting = (newStartBeforeExistingStart && newEndBetween);
        if (newPeriodOverlapsWithBeginningOfExisting) {
          validationAlert.html(_p.get.errMsg_newPeriodOverlapsWithExistingPeriodFront).show();

          break;
        }

        //=========================

        var newBlockOverlapsWithEndingOfExisting = (newStartBetween && newEndAfterExistingEnd);
        if (newBlockOverlapsWithEndingOfExisting) {
          validationAlert.html(_p.get.errMsg_newPeriodOverlapsWithExistingPeriodEnd).show();

          break;
        }

        var newBlockEncompassesExisting = (newStartBeforeExistingStart && newEndAfterExistingEnd);
        if (newBlockEncompassesExisting) {
          validationAlert.html(_p.get.errMsg_newPeriodEncompassesExistingPeriod).show();

          break;
        }

        var newBlockEncompassedByExisting = (newStartBetween && newEndBetween);
        if (newBlockEncompassedByExisting) {
          validationAlert.html(_p.get.errMsg_newPeriodEncompassedByExistingPeriod).show();

          break;
        }
      }
    }
  };

  _p.reprintBlocksInModalSorted = function () {
    // sort entries
    var startTimeMinutes = [];
    var blocksFromDom = _p.getBlocksFromModalDom();

    // clear DOM now that data is in memory
    $('#tbl-blocks tbody').html('');

    var block;
    for (var m = 0; m < blocksFromDom.length; m++) {
      block = blocksFromDom[m];
      startTimeMinutes.push({
        formattedStart: block.formattedStart,
        s: block.s,
        formattedEnd: block.formattedEnd,
        e: block.e,
        startTimeAsMinutes: moment.duration(moment(block.s, 'h:mma').format('HH:mm')).asMinutes()
      });
    }
    startTimeMinutes = startTimeMinutes.sort(function (a, b) {
      return a.startTimeAsMinutes - b.startTimeAsMinutes;
    });

    // load times into modal
    var tbl = $('#tbl-blocks tbody');
    for (var i = 0; i < startTimeMinutes.length; i++) {
      tbl.append('<tr>' + _p.generateEntryHtml(startTimeMinutes[i].formattedStart, startTimeMinutes[i].formattedEnd) + '</tr>');
    }
  };
  _p.calculateTotalMinutesInBlocks = function (blocks) {
    // calculate minutes scheduled
    var freeMinutes = 0;
    var block;
    for (var i = 0; i < blocks.length; i++) {
      block = blocks[i];
      var minutes = moment.duration(moment(block.e, 'h:mma')
        .diff(moment(block.s, 'h:mma')))
        .as('minutes');
      freeMinutes += minutes;
    }

    return freeMinutes;
  };

  _p.recalculateFreeBusyProgressBar = function (freeMinutes) {
    var percentFreeDecimal = Math.round(freeMinutes / (24 * 60) * 100);
    var percentFree = percentFreeDecimal + '%';
    var percentNa = Math.round(100 - percentFreeDecimal) + '%';

    _p.setFreeBusyProgressBar(percentFree, percentNa, freeMinutes);
  };

  _p.resetFreeBusyProgressBar = function () {
    _p.setFreeBusyProgressBar('0%', '100%', 0);
  };

  _p.setFreeBusyProgressBar = function (percentFreeString, percentNaString, freeMinutes) {
    $('#pb-free').css('width', percentFreeString);
    $('#pb-na').css('width', percentNaString);

    $('#pb-free-perc').html(percentFreeString);
    $('#pb-na-perc').html(percentNaString);

    $('#pb-free-minutes').html(freeMinutes);
    $('#pb-na-minutes').html((24 * 60) - freeMinutes);
  };

  _p.validateInputAgainstOtherTimes = function (startMoment, endMoment, blocksFromDom) {
    var ms = startMoment;
    var me = endMoment;

    //var blocksFromDom = _p.getBlocksFromModalDom();
    //_p.clearModelEntryContext();

    var isValidInput = true;
    for (var i = 0; i < blocksFromDom.length; i++) {
      var currentBlock = blocksFromDom[i];
      var currentStartMoment = moment(currentBlock.formattedStart, 'h:mma')
      var currentEndMoment = moment(currentBlock.formattedEnd, 'h:mma');

      // eval start
      var newStartBeforeCurrentStart = ms.isSameOrBefore(currentStartMoment);
      var newStartBetween = ms.isBetween(currentStartMoment, currentEndMoment, null, '[]');

      // eval end
      var newEndAfterCurrentEnd = me.isSameOrAfter(currentEndMoment);
      var newEndBetween = me.isBetween(currentStartMoment, currentEndMoment, null, '[]');

      // eval start and end

      // new block overlaps with beginning part of current block
      var newBlockOverlapsWithBeginningOfCurrent = (newStartBeforeCurrentStart && newEndBetween);
      if (newBlockOverlapsWithBeginningOfCurrent) {

        $('#tbl-blocks tbody tr:nth-child(' + (i + 1) + ')').addClass('table-info');
        $('#block-validation-alert').html(_p.get.errMsg_newBlockOverlapsWithCurrentBlockFront);
        isValidInput = false;
        $('#block-validation-alert').fadeIn();
        break;
      }

      var newBlockOverlapsWithEndingOfCurrent = (newStartBetween && newEndAfterCurrentEnd);
      if (newBlockOverlapsWithEndingOfCurrent) {

        $('#tbl-blocks tbody tr:nth-child(' + (i + 1) + ')').addClass('table-info');
        $('#block-validation-alert').html(_p.get.errMsg_newBlockOverlapsWithCurrentBlockEnd);
        isValidInput = false;
        $('#block-validation-alert').fadeIn();
        break;
      }

      var newBlockEncompassesCurrent = (newStartBeforeCurrentStart && newEndAfterCurrentEnd);
      if (newBlockEncompassesCurrent) {
        $('#tbl-blocks tbody tr:nth-child(' + (i + 1) + ')').addClass('table-info');

        $('#block-validation-alert').html(_p.get.errMsg_newBlockEncompassesCurrentBlock);
        isValidInput = false;
        $('#block-validation-alert').fadeIn();
        break;
      }

      var newBlockEncompassedByCurrent = (newStartBetween && newEndBetween);
      if (newBlockEncompassedByCurrent) {
        $('#tbl-blocks tbody tr:nth-child(' + (i + 1) + ')').addClass('table-info');

        $('#block-validation-alert').html(_p.get.errMsg_newBlockEncompassedByCurrentBlock);
        isValidInput = false;
        $('#block-validation-alert').fadeIn();
        break;
      }
    }

    return isValidInput;
  };

  _p.clearModelEntryContext = function () {
    $('#block-validation-alert').fadeOut();
    $('#spn-end-time-validation').hide();
    $('#end-time').removeClass('is-invalid');
    $('#tbl-blocks tbody tr').each(function (i, e) {
      var tr = $(e).removeClass('table-info');
    });
  };

  /// <summary>
  /// Load user created time blocks into object array from modal dom.
  /// </summary>
  /// <returns>Object array of time blocks; start and end formatted for dispaly and database.</returns>
  _p.getBlocksFromModalDom = function () {
    var blocks = [];
    $('#tbl-blocks tbody tr').each(function (i, e) {
      var tr = $(e);
      var start = tr.find('td:nth-child(1)').html();
      var end = tr.find('td:nth-child(2)').html();
      blocks.push({
        formattedStart: start,
        s: moment(start, 'h:mma').format('HH:mm:ss'),

        formattedEnd: end,
        e: moment(end, 'h:mma').format('HH:mm:ss')
      });

    });

    return blocks;
  };

  /// <summary>
  /// Load time blocks into object array from the DOM of an availability card.
  /// </summary>
  /// <param name="dayOfWeek">Availability day to load into object array</param>
  /// <returns>Object array of time blocks</returns>
  _p.loadBlocksFromDom = function (dayOfWeek) {
    var ul = $('.card#' + dayOfWeek + ' .list-group .list-group-item');
    var blocks = [];
    ul.each(function (i, e) {
      var times = $(e).html().split(' - ');
      blocks.push({
        s: times[0],
        e: times[1]
      });
    });

    return blocks;
  };
  _p.saveBlocks = function (blocks) {
    var endpoint = 'https://localhost:44352/Availability/SaveBlocks';
    return $.ajax({
      url: endpoint,
      type: 'POST',
      contentType: 'application/json; charset=utf-8',
      dataType: 'json',
      data: JSON.stringify({
        'blocks': blocks
      })

      // works with [FromBody]List<AvailabilityBlocks>
      //data: JSON.stringify(blocks)
    });
  };

  _p.generateEntryHtml = function (start, end) {
    return '<td class="centered_cell">' + start + '</td><td class="centered_cell">' + end + '</td>' + _p.generateButtonTd();
  };

  p.handle = {

  };

  /// <summary>
  /// Register all event handlers.
  /// </summary>
  _p.handle.register = function () {
    _p.handle.onClick.manageDailyAvailability();
    _p.handle.onClick.addBlockEntry();
    //_p.handle.onClick.saveUpdatedAvailability();
    _p.handle.onClick.saveUpdatedAvailabilityToDom();
    _p.handle.onClick.removeEntry();
    _p.handle.onClick.showCopyDayModal();
    _p.handle.onClick.copyDays();
    _p.handle.onClick.refreshGantt();
    _p.handle.onChange.selecteDayToCopy();
    _p.handle.onChange.activeStatusChanged();
    _p.handle.onChange.newBlockStartTimeSelect();
    _p.handle.onChange.newBlockEndTimeSelect();
  };

  _p.generateDeleteButton = function () {
    return '<button type="button" class="btn btn-info">'
      + '<span class="feather-minus"></span>';
  };
  _p.generateWhitelistEntryTd = function (entry) {
    return '<td class="td-white">' + entry + '</td>';
  };
  _p.generateButtonTd = function () {
    return '<td class="td-btn">' + _p.generateDeleteButton() + '</td>';
  };

  return p;
})();
$(document).ready(function () { appts_add_availability.init(); });

  //_p.initDroppables = function () {
  //  $("#draggable").draggable();
  //  $("#droppable").droppable({
  //    drop: function (event, ui) {
  //      //$(this)
  //      //  .addClass("ui-state-highlight")
  //      //  .find("p")
  //      //  .html("Dropped!");
  //      $(ui.draggable).detach().css({ top: 0, left: 0 }).appendTo(this);
  //    }
  //  });

  //  $('#Sunday .list-group-item').draggable({
  //    helper: 'clone'
  //    , zIndex: 1000
  //  });

  //  $('.card').droppable({
  //    drop: function (ev, ui) {
  //      $(ui.draggable).detach().css({ top: 0, left: 0 }).appendTo($(this).find('.list-group'))
  //    }
  //  })
  //};