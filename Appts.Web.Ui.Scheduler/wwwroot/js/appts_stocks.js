
/**
 * Author: atom
 * Date: 2/10/2021
 * Support management of stocks, stock trends.
 */
var appts_stocks = (function () {
  var p = {}, _p = {};
  _p.test = {};
  p.init = function () {
    _p.handle.register();
    //alert('loaded');
    //_p.createHeatmap();
    var heatObjs = JSON.parse($('#hf-json').val());
    _p.createHeatmapWithObject();
  };
  _p.const = { };
  _p.ids = { };
  _p.names = { };
  _p.get = { };
  _p.set = {};
  _p.createHeatmapWithObject = function (stockTrendHeatMap) {
    $('#chartdiv').height(stockTrendHeatMap.length * 25)
    // Apply chart themes
    am4core.useTheme(am4themes_animated);
    var chart = am4core.create("chartdiv", am4charts.XYChart);
    chart.maskBullets = false;
    var xAxis = chart.xAxes.push(new am4charts.CategoryAxis());
    var yAxis = chart.yAxes.push(new am4charts.CategoryAxis());
    xAxis.dataFields.category = "time";
    yAxis.dataFields.category = "symbol";
    xAxis.renderer.grid.template.disabled = true;
    xAxis.renderer.minGridDistance = 40;
    yAxis.renderer.grid.template.disabled = true;
    yAxis.renderer.inversed = true;
    yAxis.renderer.minGridDistance = 30;
    var series = chart.series.push(new am4charts.ColumnSeries());
    series.dataFields.categoryX = "time";
    series.dataFields.categoryY = "symbol";
    series.dataFields.value = "interest";
    series.sequencedInterpolation = true;
    series.defaultState.transitionDuration = 3000;
    series.columns.template.width = am4core.percent(100);
    series.columns.template.height = am4core.percent(100);
    var columnTemplate = series.columns.template;
    columnTemplate.strokeWidth = 2;
    columnTemplate.strokeOpacity = 1;
    columnTemplate.stroke = am4core.color("#ffffff");
    columnTemplate.tooltipText = "{time}, {symbol}: {interest.workingValue.formatNumber('#.')}";
    columnTemplate.column.adapter.add("fill", function (fill, target) {
      if (target.dataItem) {
        if (target.dataItem.value < 50) {
          return am4core.color("#222222")
        }
        else if (target.dataItem.value < 75) {
          return am4core.color("#ff0")
        }
        else {
          return am4core.color("#f00");
        }
      }
      return fill;
    });
    var legend = new am4charts.Legend();
    legend.parent = chart.chartContainer;
    legend.data = [{
      "name": ">= 6000",
      "fill": am4core.color("#0f0")
    }, {
      "name": ">= 4000",
      "fill": am4core.color("#ff0")
    }, {
      "name": "< 4000",
      "fill": am4core.color("#f00")
    }];
    //chart.categoryAxis.labelRotation = 45;
    chart.data = stockTrendHeatMap;
  }
  _p.createHeatmapWithJson = function (jsonin) {
    var stockTrendHeatMap = JSON.parse(jsonin);
    $('#chartdiv').height(parseInt(stockTrendHeatMap.length, 10) * 50)
    // Apply chart themes
    am4core.useTheme(am4themes_animated);
    var chart = am4core.create("chartdiv", am4charts.XYChart);
    chart.maskBullets = false;
    var xAxis = chart.xAxes.push(new am4charts.CategoryAxis());
    var yAxis = chart.yAxes.push(new am4charts.CategoryAxis());
    xAxis.dataFields.category = "time";
    yAxis.dataFields.category = "symbol";
    xAxis.renderer.grid.template.disabled = true;
    xAxis.renderer.minGridDistance = 40;
    yAxis.renderer.grid.template.disabled = true;
    yAxis.renderer.inversed = true;
    yAxis.renderer.minGridDistance = 30;
    var series = chart.series.push(new am4charts.ColumnSeries());
    series.dataFields.categoryX = "time";
    series.dataFields.categoryY = "symbol";
    series.dataFields.value = "interest";
    series.sequencedInterpolation = true;
    series.defaultState.transitionDuration = 3000;
    series.columns.template.width = am4core.percent(100);
    series.columns.template.height = am4core.percent(100);
    var columnTemplate = series.columns.template;
    columnTemplate.strokeWidth = 2;
    columnTemplate.strokeOpacity = 1;
    columnTemplate.stroke = am4core.color("#ffffff");
    columnTemplate.tooltipText = "{time}, {symbol}: {interest.workingValue.formatNumber('#.')}";
    columnTemplate.column.adapter.add("fill", function (fill, target) {
      if (target.dataItem) {
        if (target.dataItem.value < 50) {
          return am4core.color("#222222")
        }
        else if (target.dataItem.value < 75) {
          return am4core.color("#ff0")
        }
        else {
          return am4core.color("#f00");
        }
      }
      return fill;
    });
    var legend = new am4charts.Legend();
    legend.parent = chart.chartContainer;
    legend.data = [{
      "name": ">= 6000",
      "fill": am4core.color("#0f0")
    }, {
      "name": ">= 4000",
      "fill": am4core.color("#ff0")
    }, {
      "name": "< 4000",
      "fill": am4core.color("#f00")
    }];
    //chart.categoryAxis.labelRotation = 45;
    chart.data = stockTrendHeatMap;
  }
  _p.createHeatmap = function () {
    $('#chartdiv').height(parseInt($('#hf-trends-count').val(), 10) * 50)
    // Apply chart themes
    am4core.useTheme(am4themes_animated);
    var chart = am4core.create("chartdiv", am4charts.XYChart);
    chart.maskBullets = false;
    var xAxis = chart.xAxes.push(new am4charts.CategoryAxis());
    var yAxis = chart.yAxes.push(new am4charts.CategoryAxis());
    xAxis.dataFields.category = "time";
    yAxis.dataFields.category = "symbol";
    xAxis.renderer.grid.template.disabled = true;
    xAxis.renderer.minGridDistance = 40;
    yAxis.renderer.grid.template.disabled = true;
    yAxis.renderer.inversed = true;
    yAxis.renderer.minGridDistance = 30;
    var series = chart.series.push(new am4charts.ColumnSeries());
    series.dataFields.categoryX = "time";
    series.dataFields.categoryY = "symbol";
    series.dataFields.value = "interest";
    series.sequencedInterpolation = true;
    series.defaultState.transitionDuration = 3000;
    series.columns.template.width = am4core.percent(100);
    series.columns.template.height = am4core.percent(100);
    var columnTemplate = series.columns.template;
    columnTemplate.strokeWidth = 2;
    columnTemplate.strokeOpacity = 1;
    columnTemplate.stroke = am4core.color("#ffffff");
    columnTemplate.tooltipText = "{time}, {symbol}: {interest.workingValue.formatNumber('#.')}";
    columnTemplate.column.adapter.add("fill", function (fill, target) {
      if (target.dataItem) {
        if (target.dataItem.value < 50) {
          return am4core.color("#222222")
        }
        else if (target.dataItem.value < 75) {
          return am4core.color("#ff0")
        }
        else {
          return am4core.color("#f00");
        }
      }
      return fill;
    });
    var legend = new am4charts.Legend();
    legend.parent = chart.chartContainer;
    legend.data = [{
      "name": ">= 6000",
      "fill": am4core.color("#0f0")
    }, {
      "name": ">= 4000",
      "fill": am4core.color("#ff0")
    }, {
      "name": "< 4000",
      "fill": am4core.color("#f00")
    }];
    //chart.categoryAxis.labelRotation = 45;
    chart.data = JSON.parse($('#hf-json').val());
  }
  /// <summary>
  /// Container to define event handlers.
  /// </summary>
  _p.handle = {
    onClick: {
      getNews: function () {
        $('#btn-get-news').on('click', function () {
          var tickers = $('#tb-tickers-for-news').val();
          console.log('running ajax')
          $.ajax({
            type: 'GET',
            //data: JSON.stringify('helllo'),
            dataType: 'json',
            url: 'https://stocknewsapi.com/api/v1?tickers=' + tickers + '&items=50&token=c0wm0lld4eign5cf0cdv3tflyugrc0uyjvcyhvuv&sentiment=positive,negative&type=article',
            contentType: 'application/json'
          }).done(function (data) {
            console.log('s', data)
          }).fail(function (data) { console.log('f', data) })
        })
        //$('#btn-set-chart').on('click', function () { })
      }
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
$(document).ready(function () { appts_stocks.init(); });