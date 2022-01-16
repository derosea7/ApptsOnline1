/**
 * Author: Adam James DeRose
 * Date: 10/23/2019
 * Support management of availability periods.
 */
var appts_manage_periods = (function () {
  var p = {}, _p = {};
  _p.test = {};
  p.chart = {};
  p.init = function () {
    _p.handle.register();
    _p.formatTimesWithMoment();
    _p.initPeriodGantt();
  };
  _p.initPeriodGantt = function () {
    var periodsJson = JSON.parse($('#hf-periods-json').val());
    _p.testAmChart(periodsJson);
  };
  _p.testAmChart = function (availabilityData) {
    // Themes begin
    am4core.useTheme(am4themes_animated);
    // Themes end

    //var chart = am4core.create("chartdiv", am4charts.XYChart);
    p.chart = am4core.create("chartdiv1", am4charts.XYChart);
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
    dateAxis.dateFormatter.dateFormat = "MM/dd/yyyy";
    dateAxis.renderer.minGridDistance = 96;
    dateAxis.baseInterval = { count: 15, timeUnit: "day" };
    dateAxis.min = new Date(2019, 10, 1, 0, 0, 0).getTime();
    dateAxis.max = new Date(2020, 11, 31, 24, 0, 0, 0).getTime();

    var d = new Date();
    dateAxis.min = new Date(d.getFullYear(), d.getMonth() - 3, 1, 0, 0, 0).getTime();
    dateAxis.max = new Date(d.getFullYear(), d.getMonth() + 9, 28, 23, 59, 59).getTime();

    dateAxis.strictMinMax = true;
    dateAxis.renderer.tooltipLocation = 0;

    dateAxis.renderer.labels.template.adapter.add('text', function (text, target) {
      //return _p.createDateTimeMoment(text).format('MM/DD/YYYY');
      //return moment(text, 'MM/DD/YYYY')
      return text;
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
  _p.createDateTimeMoment = function (timeSpan) {
    var n = moment(moment().format('YYYY-MM-DD'), 'YYYY-MM-DD');
    var t = moment.duration(timeSpan);
    var nt = n.add(t);
    return nt;
  };
  _p.initNotifications = function () {
    var deletedType = $('#qs-deleted-type').val();
    if (deletedType !== undefined && deletedType === 't') {
      _p.showNotify('Successfully deleted general availability!', 'inverse');
    }
    var addedType = $('#qs-added-type').val();
    if (addedType !== undefined && addedType === 't') {
      _p.showNotify('Successfully added general availability!', 'inverse');
    }
    var updatedType = $('#qs-updated-type').val();
    if (updatedType !== undefined && updatedType === 't') {
      _p.showNotify('Successfully updated general availability!', 'inverse');
    }
  };
  _p.formatTimesWithMoment = function () {
    var start, end;
    var ms, me;
    var currentBlock;
    $('.lg-block').each(function (i, e) {
      currentBlock = $(e);
      //console.log('currentlg block', currentBlock)
      start = currentBlock.data('start');
      end = currentBlock.data('end');
      ms = moment(start, 'HH:mm:ss').format('h:mma');
      me = moment(end, 'HH:mm:ss').format('h:mma');
      //console.log('ms me', ms + ' - ' + me);
      currentBlock.html(ms + ' - ' + me);
    });
  };
  /// <summary>
  /// Container to define event handlers.
  /// </summary>
  _p.handle = {
    onClick: {
      deletePeriod: function () {
        $('body').on('click', '.a-delete-period', function () {
          var periodId = $(this).closest('.card').attr('id');
          console.log(periodId);
          var modal = $('#mdl-confirm-delete');
          $('#frm-confirm-delete').attr('action', '/Availability/DeletePeriod/' + periodId);
          modal.modal('show');
        });
      }
    },
    onHover: {

    },
    onChange: {

    }
  };
  /// <summary>
  /// Register all event handlers.
  /// </summary>
  _p.handle.register = function () {
    _p.handle.onClick.deletePeriod();
  };
  return p;
})();
$(document).ready(function () { appts_manage_periods.init(); });
