
/**
 * Author: atom
 * Date: 2/10/2021
 * Support management of stocks, stock trends.
 */
var appts_stocks_comp_trends = (function () {
  var p = {}, _p = {};
  _p.test = {};
  p.init = function () {
    _p.handle.register();
    //alert('loaded');
    //var heatObjs = JSON.parse($('#hf-json').val());

    var searchTrendsRaw = JSON.parse($('#hf-json').val()).trends[0];
    var chartdata = _p.createChartDataFromRawPyTrends(searchTrendsRaw);
    _p.initSearchTrendsLineChart(chartdata);

    //// compare chart
    var prices = JSON.parse($('#hf-golden-json').val());
    var compareChartData = _p.createTrendToPriceChartData(
      searchTrendsRaw, prices);
    _p.initTrendsToPriceLineChart(compareChartData);


    ////candlesticks
    var candledata = JSON.parse($('#hf-candlestick-json').val());
    _p.initCandleStickChart(candledata);
  };
  _p.const = { };
  _p.ids = { };
  _p.names = { };
  _p.get = { };
  _p.set = {};
  _p.createTrendToPriceChartData = function (searchTrendsRaw, prices) {
    dateidx = [];
    for (var dt = 0; dt < searchTrendsRaw.index.length; dt++) {
      dateidx.push(new Date(searchTrendsRaw.index[dt]));
      //console.log(new Date(searchTrendsRaw.index[dt]));
    }
    var datalen = dateidx.length;
    values1 = [];
    for (var i = 0; i < datalen ; i++) {
      values1.push(searchTrendsRaw.data[i]);
    }

    chartdata = [];
    for (var i = 0; i < datalen; i++) {
      //chartdata.push({ date: dateidx[i], value1: values1[i], value2: values2[i] });
      chartdata.push({ date: dateidx[i], value1: values1[i], value2: 0 });
    }
    //console.log(chartdata);
    console.log(prices);

    //for (var p = 0; p < prices.length; p++) {
    //  pricedt = new Date(prices[p].date);
    //  for (var t = 0; t < chartdata.length; t++) {
    //    if (chartdata[t].date == pricedt) {
    //      chartdata[t].value2 = prices[p].price1;
    //    }
    //  }
    //}

    chartdata2 = []
    for (var cd = 0; cd < chartdata.length; cd++) {
      var date = chartdata[cd].date;
      var trend = chartdata[cd].value1;
      var price = 1
      for (var p = 0; p < prices.length; p++) {
        pricedt = new Date(prices[p].date);
        if (chartdata[cd].date.getTime() == pricedt.getTime()) {
          //300 multiple works for pqeff and ozsc
          //price = (prices[p].price1 * 300);
          //price = prices[p].price1 * 5; // worked well for iht
          //price = prices[p].price1;
          price = prices[p].price1 * 5;
          break;
        }
      }

      chartdata2.push({ date: date, trendValue: trend, stockPrice: price });
    }

    console.log(chartdata2);

    return chartdata2;
  }
  _p.initCandleStickChart = function (candleObj) {
    // Themes begin
    am4core.useTheme(am4themes_animated);
    // Themes end

    var chart = am4core.create("chartdiv3", am4charts.XYChart);
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

    chart.data = candleObj.dailyBars;

  }
  _p.createChartDataFromRawPyTrends = function (searchTrendsRaw) {
    //console.log(searchTrendsRaw.index);

    dateidx = [];
    for (var dt = 0; dt < searchTrendsRaw.index.length; dt++) {
      dateidx.push(new Date(searchTrendsRaw.index[dt]));
      //console.log(new Date(searchTrendsRaw.index[dt]));
    }
    var datalen = dateidx.length;

    values1 = [];
    for (var i = 0; i < datalen; i++) {
      values1.push(searchTrendsRaw.data[i]);
    }
    //console.log(values1)

    values2 = [];
    for (var i = datalen; i < datalen * 2; i++) {
      values2.push(searchTrendsRaw.data[i]);
    }
    //console.log(values2)

    chartdata = [];
    for (var i = 0; i < datalen; i++) {
      chartdata.push({ date: dateidx[i], value1: values1[i], value2: values2[i] });
    }
    //console.log(chartdata);

    return chartdata;
  }
  _p.initTrendsToPriceLineChart = function (trendsToPriceData) {

    // Themes begin
    am4core.useTheme(am4themes_animated);
    // Themes end

    var chart = am4core.create("chartdiv2", am4charts.XYChart);

    var data = [];
    var trendValue = 50;
    var stockPrice = 50;
    for (var i = 0; i < 300; i++) {
      var date = new Date();
      date.setHours(0, 0, 0, 0);
      date.setDate(i);
      trendValue -= Math.round((Math.random() < 0.5 ? 1 : -1) * Math.random() * 10);
      stockPrice -= Math.round((Math.random() < 0.5 ? 1 : -1) * Math.random() * 10);
      data.push({ date: date, trendValue: trendValue, stockPrice: stockPrice });
    }

    //chart.data = data;
    //console.log(data);
    chart.data = trendsToPriceData;

    // Create axes
    var dateAxis = chart.xAxes.push(new am4charts.DateAxis());
    dateAxis.renderer.minGridDistance = 60;

    var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());

    // Create series
    var series = chart.series.push(new am4charts.LineSeries());
    series.dataFields.valueY = "trendValue";
    series.dataFields.dateX = "date";
    series.tooltipText = "{value}"

    // Create series
    var series = chart.series.push(new am4charts.LineSeries());
    series.dataFields.valueY = "stockPrice";
    series.dataFields.dateX = "date";
    series.tooltipText = "{value}"
    series.stroke = am4core.color("#ff0000"); 

    series.tooltip.pointerOrientation = "vertical";

    chart.cursor = new am4charts.XYCursor();
    chart.cursor.snapToSeries = series;
    chart.cursor.xAxis = dateAxis;

    //chart.scrollbarY = new am4core.Scrollbar();
    chart.scrollbarX = new am4core.Scrollbar();
  }
  _p.initSearchTrendsLineChart = function (searchTrendsLineChartData) {

    // Themes begin
    am4core.useTheme(am4themes_animated);
    // Themes end

    var chart = am4core.create("chartdiv", am4charts.XYChart);

    var data = [];
    var value = 50;
    for (var i = 0; i < 300; i++) {
      var date = new Date();
      date.setHours(0, 0, 0, 0);
      date.setDate(i);
      value -= Math.round((Math.random() < 0.5 ? 1 : -1) * Math.random() * 10);
      data.push({ date: date, value: value });
    }

    //chart.data = data;
    //console.log(data);
    chart.data = searchTrendsLineChartData;

    // Create axes
    var dateAxis = chart.xAxes.push(new am4charts.DateAxis());
    dateAxis.renderer.minGridDistance = 60;

    var valueAxis = chart.yAxes.push(new am4charts.ValueAxis());

    //// Create series
    //var series = chart.series.push(new am4charts.LineSeries());
    //series.dataFields.valueY = "value";
    //series.dataFields.dateX = "date";
    //series.tooltipText = "{value}"

    // Create series
    var series = chart.series.push(new am4charts.LineSeries());
    series.dataFields.valueY = "value1";
    series.dataFields.dateX = "date";
    series.tooltipText = "{value}"
    series.stroke = am4core.color("#ff0000"); 

    // Create series
    var series = chart.series.push(new am4charts.LineSeries());
    series.dataFields.valueY = "value2";
    series.dataFields.dateX = "date";
    series.tooltipText = "{value}"

    series.tooltip.pointerOrientation = "vertical";

    chart.cursor = new am4charts.XYCursor();
    chart.cursor.snapToSeries = series;
    chart.cursor.xAxis = dateAxis;

    //chart.scrollbarY = new am4core.Scrollbar();
    chart.scrollbarX = new am4core.Scrollbar();
  }
  /// <summary>
  /// Container to define event handlers.
  /// </summary>
  _p.handle = {
    onClick: {

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
$(document).ready(function () { appts_stocks_comp_trends.init(); });