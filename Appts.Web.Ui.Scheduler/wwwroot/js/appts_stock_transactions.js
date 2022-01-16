
/**
 * Author: atom
 * Date: 2/10/2021
 * Support management of page.
 */
var appts_stock_transactions = (function () {
  var p = {}, _p = {};
  _p.test = {};

  p.init = function () {
    _p.handle.register();
    //ert("sdfdf")
    $('.btn-del-transaction').on('click', function () {
      alert($(this).closest('tr').prop('id'));
      $.ajax({

      });
    });
    $('#btnaddtransaction').on('click', function () {
      var buyprice = parseFloat($('#buyprice').val());
      var sharestraded = parseFloat($('#sharestraded').val());
      console.log('buyprice', buyprice);
      console.log('sharestraded', sharestraded);
      var sellprice = parseFloat($('#sellprice').val());

      var sharepnl = sellprice - buyprice;
      var shareroi = sellprice / buyprice;
      var totalpnl = sharepnl * sharestraded

      console.log('sell price', sellprice);
      console.log('totlapnl', totalpnl);
      console.log('share profit n loss', sharepnl.toFixed(3));
      console.log('var shareroi = sellprice / buyprice;', shareroi.toFixed(5));
      console.log('if traded at 10k', ((10000 / buyprice) * sellprice) - ((10000 / buyprice) * buyprice));
      $('#percprofit').html(sharerio.toFixed(5));
      //
    });
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
$(document).ready(function () { appts_stock_transactions.init(); });
