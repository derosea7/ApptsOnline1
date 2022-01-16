
/**
 * Author: atom
 * Date: 6/5/2021
 * 
 */
var appts_view_hist_stk_charts = (function () {
  var p = {}, _p = {};
  _p.test = {};
  p.init = function () {
    _p.handle.register();
    //alert('loaded');
    ////_p.createHeatmap();
    //var heatObjs = JSON.parse($('#hf-json').val());
    //_p.createHeatmapWithObject();
    var daily = JSON.parse($('#hf-json').val());
    _p.initCandlestickChart(daily);
    //_p.initCloseLineChart(daily);
    var comparedocs = JSON.parse($('#hf-stock-compare-json').val());
    console.log(comparedocs);
    
    _p.initCompareChart(comparedocs);
  };
  _p.const = { };
  _p.ids = { };
  _p.names = { };
  _p.get = { };
  _p.set = {};
  _p.initCompareChart = function (comparedocs) {
    // Themes begin
    am4core.useTheme(am4themes_dark);
    am4core.useTheme(am4themes_animated);
    // Themes end

    var chart = am4core.create("chartdiv3", am4charts.XYChart);
    chart.padding(0, 15, 0, 15);
    chart.colors.step = 3;

    var data = [];
    var price1 = 1000;
    var price2 = 2000;
    var price3 = 3000;
    var quantity = 1000;
    for (var i = 15; i < 3000; i++) {
      price1 += Math.round((Math.random() < 0.5 ? 1 : -1) * Math.random() * 100);
      price2 += Math.round((Math.random() < 0.5 ? 1 : -1) * Math.random() * 100);
      price3 += Math.round((Math.random() < 0.5 ? 1 : -1) * Math.random() * 100);

      if (price1 < 100) {
        price1 = 100;
      }

      if (price2 < 100) {
        price2 = 100;
      }

      if (price3 < 100) {
        price3 = 100;
      }

      quantity += Math.round((Math.random() < 0.5 ? 1 : -1) * Math.random() * 500);

      if (quantity < 0) {
        quantity *= -1;
      }
      data.push({ date: new Date(2000, 0, i), price1: price1, price2: price2, price3: price3, quantity: quantity });
    }


    //chart.data = data;
    chart.data = comparedocs.PriceDataDocs;
    // the following line makes value axes to be arranged vertically.
    chart.leftAxesContainer.layout = "vertical";

    // uncomment this line if you want to change order of axes
    //chart.bottomAxesContainer.reverseOrder = true;

    var dateAxis = chart.xAxes.push(new am4charts.DateAxis());
    dateAxis.renderer.grid.template.location = 0;
    dateAxis.renderer.ticks.template.length = 8;
    dateAxis.renderer.ticks.template.strokeOpacity = 0.1;
    dateAxis.renderer.grid.template.disabled = true;
    dateAxis.renderer.ticks.template.disabled = false;
    dateAxis.renderer.ticks.template.strokeOpacity = 0.2;
    dateAxis.renderer.minLabelPosition = 0.01;
    dateAxis.renderer.maxLabelPosition = 0.99;
    dateAxis.keepSelection = true;

    dateAxis.groupData = true;
    dateAxis.minZoomCount = 5;

    // these two lines makes the axis to be initially zoomed-in
    // dateAxis.start = 0.7;
    // dateAxis.keepSelection = true;

    var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
    valueAxis.tooltip.disabled = true;
    valueAxis.zIndex = 1;
    valueAxis.renderer.baseGrid.disabled = true;
    // height of axis
    valueAxis.height = am4core.percent(65);

    valueAxis.renderer.gridContainer.background.fill = am4core.color("#000000");
    valueAxis.renderer.gridContainer.background.fillOpacity = 0.05;
    valueAxis.renderer.inside = true;
    valueAxis.renderer.labels.template.verticalCenter = "bottom";
    valueAxis.renderer.labels.template.padding(2, 2, 2, 2);

    //valueAxis.renderer.maxLabelPosition = 0.95;
    valueAxis.renderer.fontSize = "0.8em"

    var series1 = chart.series.push(new am4charts.LineSeries());
    series1.dataFields.dateX = "date";
    series1.dataFields.valueY = "price1";
    series1.dataFields.valueYShow = "changePercent";
    series1.tooltipText = "{name}: {valueY.changePercent.formatNumber('[#0c0]+#.00|[#c00]#.##|0')}%";
    series1.name = "tsla";
    series1.tooltip.getFillFromObject = false;
    series1.tooltip.getStrokeFromObject = true;
    series1.tooltip.background.fill = am4core.color("#fff");
    series1.tooltip.background.strokeWidth = 2;
    series1.tooltip.label.fill = series1.stroke;
    series1.cursorTooltipEnabled = false;

    var series2 = chart.series.push(new am4charts.LineSeries());
    series2.dataFields.dateX = "date";
    series2.dataFields.valueY = "price2";
    series2.dataFields.valueYShow = "changePercent";
    series2.tooltipText = "{name}: {valueY.changePercent.formatNumber('[#0c0]+#.00|[#c00]#.##|0')}%";
    series2.name = "pton";
    series2.tooltip.getFillFromObject = false;
    series2.tooltip.getStrokeFromObject = true;
    series2.tooltip.background.fill = am4core.color("#fff");
    series2.tooltip.background.strokeWidth = 2;
    series2.tooltip.label.fill = series2.stroke;
    series2.cursorTooltipEnabled = false;
    //var series3 = chart.series.push(new am4charts.LineSeries());
    //series3.dataFields.dateX = "date";
    //series3.dataFields.valueY = "price3";
    //series3.dataFields.valueYShow = "changePercent";
    //series3.tooltipText = "{name}: {valueY.changePercent.formatNumber('[#0c0]+#.00|[#c00]#.##|0')}%";
    //series3.name = "upst";
    //series3.tooltip.getFillFromObject = false;
    //series3.tooltip.getStrokeFromObject = true;
    //series3.tooltip.background.fill = am4core.color("#fff");
    //series3.tooltip.background.strokeWidth = 2;
    //series3.tooltip.label.fill = series3.stroke;

    var valueAxis2 = chart.yAxes.push(new am4charts.ValueAxis());
    valueAxis2.tooltip.disabled = true;
    // height of axis
    valueAxis2.height = am4core.percent(35);
    valueAxis2.zIndex = 3
    // this makes gap between panels
    valueAxis2.marginTop = 30;
    valueAxis2.renderer.baseGrid.disabled = true;
    valueAxis2.renderer.inside = true;
    valueAxis2.renderer.labels.template.verticalCenter = "bottom";
    valueAxis2.renderer.labels.template.padding(2, 2, 2, 2);
    //valueAxis.renderer.maxLabelPosition = 0.95;
    valueAxis2.renderer.fontSize = "0.8em";

    valueAxis2.renderer.gridContainer.background.fill = am4core.color("#000000");
    valueAxis2.renderer.gridContainer.background.fillOpacity = 0.05;

    var volumeSeries = chart.series.push(new am4charts.StepLineSeries());
    volumeSeries.fillOpacity = 1;
    volumeSeries.fill = series1.stroke;
    volumeSeries.stroke = series1.stroke;
    volumeSeries.dataFields.dateX = "date";
    volumeSeries.dataFields.valueY = "quantity";
    volumeSeries.yAxis = valueAxis2;
    volumeSeries.tooltipText = "Volume: {valueY.value}";
    volumeSeries.name = "Series 2";
    // volume should be summed
    volumeSeries.groupFields.valueY = "sum";
    volumeSeries.tooltip.label.fill = volumeSeries.stroke;
    chart.cursor = new am4charts.XYCursor();

    var scrollbarX = new am4charts.XYChartScrollbar();
    scrollbarX.series.push(series1);
    scrollbarX.marginBottom = 20;
    var sbSeries = scrollbarX.scrollbarChart.series.getIndex(0);
    sbSeries.dataFields.valueYShow = undefined;
    chart.scrollbarX = scrollbarX;

    // Add range selector
    var selector = new am4plugins_rangeSelector.DateAxisRangeSelector();
    selector.container = document.getElementById("controls");
    selector.axis = dateAxis;

  }
  _p.initCloseLineChart = function (dailyObj) {
    // Themes begin
    //am4core.useTheme(am4themes_dark);
    am4core.useTheme(am4themes_animated);
    // Themes end

    // Create chart
    var chart = am4core.create("chartdiv2", am4charts.XYChart);
    chart.padding(0, 15, 0, 15);

    // Load external data
    chart.data = dailyObj;

    // the following line makes value axes to be arranged vertically.
    chart.leftAxesContainer.layout = "vertical";

    // uncomment this line if you want to change order of axes
    //chart.bottomAxesContainer.reverseOrder = true;

    var dateAxis = chart.xAxes.push(new am4charts.DateAxis());
    dateAxis.renderer.grid.template.location = 0;
    dateAxis.renderer.ticks.template.length = 8;
    dateAxis.renderer.ticks.template.strokeOpacity = 0.1;
    dateAxis.renderer.grid.template.disabled = true;
    dateAxis.renderer.ticks.template.disabled = false;
    dateAxis.renderer.ticks.template.strokeOpacity = 0.2;
    dateAxis.renderer.minLabelPosition = 0.01;
    dateAxis.renderer.maxLabelPosition = 0.99;
    dateAxis.keepSelection = true;
    dateAxis.minHeight = 30;

    dateAxis.groupData = true;
    dateAxis.minZoomCount = 5;

    // these two lines makes the axis to be initially zoomed-in
    // dateAxis.start = 0.7;
    // dateAxis.keepSelection = true;

    var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
    valueAxis.tooltip.disabled = true;
    valueAxis.zIndex = 1;
    valueAxis.renderer.baseGrid.disabled = true;
    // height of axis
    valueAxis.height = am4core.percent(65);

    valueAxis.renderer.gridContainer.background.fill = am4core.color("#000000");
    valueAxis.renderer.gridContainer.background.fillOpacity = 0.05;
    valueAxis.renderer.inside = true;
    valueAxis.renderer.labels.template.verticalCenter = "bottom";
    valueAxis.renderer.labels.template.padding(2, 2, 2, 2);

    //valueAxis.renderer.maxLabelPosition = 0.95;
    valueAxis.renderer.fontSize = "0.8em"

    var series = chart.series.push(new am4charts.LineSeries());
    series.dataFields.dateX = "date";
    series.dataFields.valueY = "close";
    series.tooltipText = "{valueY.value}";
    series.name = "MSFT: Value";
    series.defaultState.transitionDuration = 0;

    var valueAxis2 = chart.yAxes.push(new am4charts.ValueAxis());
    valueAxis2.tooltip.disabled = true;
    // height of axis
    valueAxis2.height = am4core.percent(35);
    valueAxis2.zIndex = 3
    // this makes gap between panels
    valueAxis2.marginTop = 30;
    valueAxis2.renderer.baseGrid.disabled = true;
    valueAxis2.renderer.inside = true;
    valueAxis2.renderer.labels.template.verticalCenter = "bottom";
    valueAxis2.renderer.labels.template.padding(2, 2, 2, 2);
    //valueAxis.renderer.maxLabelPosition = 0.95;
    valueAxis2.renderer.fontSize = "0.8em"

    valueAxis2.renderer.gridContainer.background.fill = am4core.color("#000000");
    valueAxis2.renderer.gridContainer.background.fillOpacity = 0.05;

    var series2 = chart.series.push(new am4charts.ColumnSeries());
    series2.dataFields.dateX = "date";
    series2.dataFields.valueY = "volume";
    series2.yAxis = valueAxis2;
    series2.tooltipText = "{valueY.value}";
    series2.name = "MSFT: Volume";
    // volume should be summed
    series2.groupFields.valueY = "sum";
    series2.defaultState.transitionDuration = 0;

    chart.cursor = new am4charts.XYCursor();

    var scrollbarX = new am4charts.XYChartScrollbar();
    scrollbarX.series.push(series);
    scrollbarX.marginBottom = 20;
    scrollbarX.scrollbarChart.xAxes.getIndex(0).minHeight = undefined;
    chart.scrollbarX = scrollbarX;


    //// Add range selector
    //var selector = new am4plugins_rangeSelector.DateAxisRangeSelector();
    //selector.container = document.getElementById("controls");
    //selector.axis = dateAxis;

  }
  _p.initCandlestickChart = function (dailyObj) {
    //am4core.ready(function () {

      // Themes begin
      am4core.useTheme(am4themes_animated);
      // Themes end

      var chart = am4core.create("chartdiv", am4charts.XYChart);
      chart.paddingRight = 20;

      chart.dateFormatter.inputDateFormat = "yyyy-MM-dd";

      var dateAxis = chart.xAxes.push(new am4charts.DateAxis());
      dateAxis.renderer.grid.template.location = 0;

      var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());
      valueAxis.tooltip.disabled = true;

      var series = chart.series.push(new am4charts.CandlestickSeries());
      series.dataFields.dateX = "date";
      series.dataFields.valueY = "close";
      series.dataFields.openValueY = "open";
      series.dataFields.lowValueY = "low";
      series.dataFields.highValueY = "high";
      series.simplifiedProcessing = true;
      series.tooltipText = "Open:${openValueY.value}\nLow:${lowValueY.value}\nHigh:${highValueY.value}\nClose:${valueY.value}";

      chart.cursor = new am4charts.XYCursor();

      // a separate series for scrollbar
      var lineSeries = chart.series.push(new am4charts.LineSeries());
      lineSeries.dataFields.dateX = "date";
      lineSeries.dataFields.valueY = "close";
      // need to set on default state, as initially series is "show"
      lineSeries.defaultState.properties.visible = false;

      // hide from legend too (in case there is one)
      lineSeries.hiddenInLegend = true;
      lineSeries.fillOpacity = 0.5;
      lineSeries.strokeOpacity = 0.5;

      var scrollbarX = new am4charts.XYChartScrollbar();
      scrollbarX.series.push(lineSeries);
      chart.scrollbarX = scrollbarX;

    chart.data = dailyObj.dailyBars;

      //chart.data = [{
      //  "date": "2011-08-01",
      //  "open": "136.65",
      //  "high": "136.96",
      //  "low": "134.15",
      //  "close": "136.49"
      //}, {
     

    //}); // end am4core.ready()
  }
  //_p.createHeatmapWithObject = function (stockTrendHeatMap) {
  //  $('#chartdiv').height(stockTrendHeatMap.length * 25)
  //  // Apply chart themes
  //  am4core.useTheme(am4themes_animated);
  //  var chart = am4core.create("chartdiv", am4charts.XYChart);
  //  chart.maskBullets = false;
  //  var xAxis = chart.xAxes.push(new am4charts.CategoryAxis());
  //  var yAxis = chart.yAxes.push(new am4charts.CategoryAxis());
  //  xAxis.dataFields.category = "time";
  //  yAxis.dataFields.category = "symbol";
  //  xAxis.renderer.grid.template.disabled = true;
  //  xAxis.renderer.minGridDistance = 40;
  //  yAxis.renderer.grid.template.disabled = true;
  //  yAxis.renderer.inversed = true;
  //  yAxis.renderer.minGridDistance = 30;
  //  var series = chart.series.push(new am4charts.ColumnSeries());
  //  series.dataFields.categoryX = "time";
  //  series.dataFields.categoryY = "symbol";
  //  series.dataFields.value = "interest";
  //  series.sequencedInterpolation = true;
  //  series.defaultState.transitionDuration = 3000;
  //  series.columns.template.width = am4core.percent(100);
  //  series.columns.template.height = am4core.percent(100);
  //  var columnTemplate = series.columns.template;
  //  columnTemplate.strokeWidth = 2;
  //  columnTemplate.strokeOpacity = 1;
  //  columnTemplate.stroke = am4core.color("#ffffff");
  //  columnTemplate.tooltipText = "{time}, {symbol}: {interest.workingValue.formatNumber('#.')}";
  //  columnTemplate.column.adapter.add("fill", function (fill, target) {
  //    if (target.dataItem) {
  //      if (target.dataItem.value < 50) {
  //        return am4core.color("#222222")
  //      }
  //      else if (target.dataItem.value < 75) {
  //        return am4core.color("#ff0")
  //      }
  //      else {
  //        return am4core.color("#f00");
  //      }
  //    }
  //    return fill;
  //  });
  //  var legend = new am4charts.Legend();
  //  legend.parent = chart.chartContainer;
  //  legend.data = [{
  //    "name": ">= 6000",
  //    "fill": am4core.color("#0f0")
  //  }, {
  //    "name": ">= 4000",
  //    "fill": am4core.color("#ff0")
  //  }, {
  //    "name": "< 4000",
  //    "fill": am4core.color("#f00")
  //  }];
  //  //chart.categoryAxis.labelRotation = 45;
  //  chart.data = stockTrendHeatMap;
  //}
  //_p.createHeatmapWithJson = function (jsonin) {
  //  var stockTrendHeatMap = JSON.parse(jsonin);
  //  $('#chartdiv').height(parseInt(stockTrendHeatMap.length, 10) * 50)
  //  // Apply chart themes
  //  am4core.useTheme(am4themes_animated);
  //  var chart = am4core.create("chartdiv", am4charts.XYChart);
  //  chart.maskBullets = false;
  //  var xAxis = chart.xAxes.push(new am4charts.CategoryAxis());
  //  var yAxis = chart.yAxes.push(new am4charts.CategoryAxis());
  //  xAxis.dataFields.category = "time";
  //  yAxis.dataFields.category = "symbol";
  //  xAxis.renderer.grid.template.disabled = true;
  //  xAxis.renderer.minGridDistance = 40;
  //  yAxis.renderer.grid.template.disabled = true;
  //  yAxis.renderer.inversed = true;
  //  yAxis.renderer.minGridDistance = 30;
  //  var series = chart.series.push(new am4charts.ColumnSeries());
  //  series.dataFields.categoryX = "time";
  //  series.dataFields.categoryY = "symbol";
  //  series.dataFields.value = "interest";
  //  series.sequencedInterpolation = true;
  //  series.defaultState.transitionDuration = 3000;
  //  series.columns.template.width = am4core.percent(100);
  //  series.columns.template.height = am4core.percent(100);
  //  var columnTemplate = series.columns.template;
  //  columnTemplate.strokeWidth = 2;
  //  columnTemplate.strokeOpacity = 1;
  //  columnTemplate.stroke = am4core.color("#ffffff");
  //  columnTemplate.tooltipText = "{time}, {symbol}: {interest.workingValue.formatNumber('#.')}";
  //  columnTemplate.column.adapter.add("fill", function (fill, target) {
  //    if (target.dataItem) {
  //      if (target.dataItem.value < 50) {
  //        return am4core.color("#222222")
  //      }
  //      else if (target.dataItem.value < 75) {
  //        return am4core.color("#ff0")
  //      }
  //      else {
  //        return am4core.color("#f00");
  //      }
  //    }
  //    return fill;
  //  });
  //  var legend = new am4charts.Legend();
  //  legend.parent = chart.chartContainer;
  //  legend.data = [{
  //    "name": ">= 6000",
  //    "fill": am4core.color("#0f0")
  //  }, {
  //    "name": ">= 4000",
  //    "fill": am4core.color("#ff0")
  //  }, {
  //    "name": "< 4000",
  //    "fill": am4core.color("#f00")
  //  }];
  //  //chart.categoryAxis.labelRotation = 45;
  //  chart.data = stockTrendHeatMap;
  //}
  //_p.createHeatmap = function () {
  //  $('#chartdiv').height(parseInt($('#hf-trends-count').val(), 10) * 50)
  //  // Apply chart themes
  //  am4core.useTheme(am4themes_animated);
  //  var chart = am4core.create("chartdiv", am4charts.XYChart);
  //  chart.maskBullets = false;
  //  var xAxis = chart.xAxes.push(new am4charts.CategoryAxis());
  //  var yAxis = chart.yAxes.push(new am4charts.CategoryAxis());
  //  xAxis.dataFields.category = "time";
  //  yAxis.dataFields.category = "symbol";
  //  xAxis.renderer.grid.template.disabled = true;
  //  xAxis.renderer.minGridDistance = 40;
  //  yAxis.renderer.grid.template.disabled = true;
  //  yAxis.renderer.inversed = true;
  //  yAxis.renderer.minGridDistance = 30;
  //  var series = chart.series.push(new am4charts.ColumnSeries());
  //  series.dataFields.categoryX = "time";
  //  series.dataFields.categoryY = "symbol";
  //  series.dataFields.value = "interest";
  //  series.sequencedInterpolation = true;
  //  series.defaultState.transitionDuration = 3000;
  //  series.columns.template.width = am4core.percent(100);
  //  series.columns.template.height = am4core.percent(100);
  //  var columnTemplate = series.columns.template;
  //  columnTemplate.strokeWidth = 2;
  //  columnTemplate.strokeOpacity = 1;
  //  columnTemplate.stroke = am4core.color("#ffffff");
  //  columnTemplate.tooltipText = "{time}, {symbol}: {interest.workingValue.formatNumber('#.')}";
  //  columnTemplate.column.adapter.add("fill", function (fill, target) {
  //    if (target.dataItem) {
  //      if (target.dataItem.value < 50) {
  //        return am4core.color("#222222")
  //      }
  //      else if (target.dataItem.value < 75) {
  //        return am4core.color("#ff0")
  //      }
  //      else {
  //        return am4core.color("#f00");
  //      }
  //    }
  //    return fill;
  //  });
  //  var legend = new am4charts.Legend();
  //  legend.parent = chart.chartContainer;
  //  legend.data = [{
  //    "name": ">= 6000",
  //    "fill": am4core.color("#0f0")
  //  }, {
  //    "name": ">= 4000",
  //    "fill": am4core.color("#ff0")
  //  }, {
  //    "name": "< 4000",
  //    "fill": am4core.color("#f00")
  //  }];
  //  //chart.categoryAxis.labelRotation = 45;
  //  chart.data = JSON.parse($('#hf-json').val());
  //}
  /// <summary>
  /// Container to define event handlers.
  /// </summary>
  _p.handle = {
    onClick: {
      //getNews: function () {
      //  $('#btn-get-news').on('click', function () {
      //    var tickers = $('#tb-tickers-for-news').val();
      //    console.log('running ajax')
      //    $.ajax({
      //      type: 'GET',
      //      //data: JSON.stringify('helllo'),
      //      dataType: 'json',
      //      url: 'https://stocknewsapi.com/api/v1?tickers=' + tickers + '&items=50&token=c0wm0lld4eign5cf0cdv3tflyugrc0uyjvcyhvuv&sentiment=positive,negative&type=article',
      //      contentType: 'application/json'
      //    }).done(function (data) {
      //      console.log('s', data)
      //    }).fail(function (data) { console.log('f', data) })
      //  })
      //  //$('#btn-set-chart').on('click', function () { })
      //}
    },
    onHover: { },
    onChange: {

    }
  };
  p.handle = { };
  /// <summary>
  /// Register all event handlers.
  /// </summary>
  _p.handle.register = function () {

  };

  return p;
})();
$(document).ready(function () { appts_view_hist_stk_charts.init(); });