
/**
 * Author: Adam DeRose
 * Date: 2/9/2021
 * Support management of user's data.
 */
var appts_manage_data = (function () {
  var p = {}, _p = {};
  _p.test = {};

  p.init = function () {
    _p.handle.register();

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
      delApptsScheduledWMeClicked: function () {
        $('#a-delete-appts-scheduled-w-me').on('click', function () {
          //alert('are you sure?')
          //$('')
          $('#btn-confirm-delete').on('click', function () {
            $.post('/Data/DeleteApptsScheduledWithMe')
              .done(function (data) {

              })
              .fail(function (data) {

              })
          });
          $('#mdl-confirm-delete').modal('show');
        });
      }
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
    _p.handle.onClick.delApptsScheduledWMeClicked();
  };

  return p;
})();
$(document).ready(function () { appts_manage_data.init(); });
