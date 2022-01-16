
/**
 * Author: Adam DeRose
 * Date: 1/30/21
 * Support management of site preferences.
 */
var appts_site_prefs = (function () {
  var p = {}, _p = {};
  _p.test = {};

  p.init = function () {
    _p.handle.register();
    var themecookie = _p.readCookie('theme');

    // slider code
    var presetValue = $('[name="theme-slider"]').val();
    var intialSelectionValue = parseInt(presetValue, 10);
    $('.update-theme').on('click', function () {
      var themeSelected = $(this).closest('.list-group-item').data('theme');
      console.log('clicked ' + themeSelected);
      _p.createCookie('theme', themeSelected, 30);
      window.location.reload(false);
    });
    




    //var hasDarkMode = themecookie == 'dark' ? true : false;
    ////console.log('cookie', hasDarkMode);
    //$('#cbDarkMode').prop('checked', hasDarkMode);
    //$('#cbDarkMode').removeAttr('disabled');
    //$('#cbDarkMode').change(function () {
    //  ////// this will contain a reference to the checkbox   
    //  //////console.log('checked');
    //  if (this.checked) {
    //    // the checkbox is now checked 
    //    _p.createCookie('theme', 'dark', 30);
    //    //console.log('theme updated');
    //  } else {
    //    // the checkbox is now no longer checked
    //    _p.eraseCookie('theme');
    //    _p.createCookie('theme', 'light', 30);
    //  }
    //  window.location.reload(false); 
      


      ///////post back to make server set cookie
      ///////rational being server-set might be more durable?
      ////var theme;
      ////if (this.checked) {
      ////  theme = 'dark';
      ////} else {
      ////  theme = 'light';
      ////}
      ////$.post('/Site/SetTheme', {
      ////  data: theme
      ////})
      ////  .done(function (data) {
      ////    window.location.reload(false); 
      ////  })
      ////  .fail(function (data) {

      ////  });
    //});
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

  _p.createCookie = function (name, value, days) {
    if (days) {
      var date = new Date();
      date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
      var expires = "; expires=" + date.toGMTString();
    }
    else var expires = "";
    document.cookie = name + "=" + value + expires + "; path=/";
  };

  _p.readCookie = function (name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
      var c = ca[i];
      while (c.charAt(0) == ' ') c = c.substring(1, c.length);
      if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
  };

  _p.eraseCookie = function (name) {
    _p.createCookie(name, "", -1);
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
$(document).ready(function () { appts_site_prefs.init(); });
