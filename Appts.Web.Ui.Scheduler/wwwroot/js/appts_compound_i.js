
/**
 * Author: atom
 * Date: 2/10/2021
 * Support management of page.
 */
var appts_compound_i = (function () {
  var p = {}, _p = {};
  _p.test = {};

  p.init = function () {
    _p.handle.register();
    $('#btn-calc').on('click', function () {
      $('#tblcibody tr').remove();
      var seed = parseInt($('#seed').val(), 10);
      var roi = parseFloat($('#roi').val());
      console.log('seed', seed, 'roi', roi);
      var tradesToCalc = 20;
      var profit = 0;
      var totalprofit = profit;
      for (var i = 0; i < tradesToCalc; i++) {
        profit = parseInt(seed * (roi + 1), 10) - seed;
        //reinvest
        seed = seed + profit;
        totalprofit += profit;
        //console.log('trade #', i + 1, 'seed', seed, 'profit', profit);
        $('#tblcibody').append(_p.tradeProfit(i + 1, seed, profit, totalprofit))
      }
    });
  };
  _p.tradeProfit = function (traden, seed, profit, totalprofit) {
    return '<tr><td>' + traden + '</td><td>' + seed + '</td><td>' + profit + '</td><td>' + totalprofit + '</td></tr>';
  }
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



  /// <summary>
  /// Container to define event handlers.
  /// </summary>
  _p.handle = {
    onClick: {

    },
    onHover: {

    },
    onChange: {

    }
  };

  p.handle = {

  };

  /// <summary>
  /// Register all event handlers.
  /// </summary>
  _p.handle.register = function () {

  };

  return p;
})();
$(document).ready(function () { appts_compound_i.init(); });
