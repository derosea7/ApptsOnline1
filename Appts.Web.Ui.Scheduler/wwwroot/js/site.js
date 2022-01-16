// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


/**
 * Author: atom
 * Date: 2/10/2021
 * Support management of site.
 */
var appts_core = (function () {
  var p = {}, _p = {};
  _p.test = {};

  p.init = function () {
    _p.handle.register();
    lazyload();
    $('.navbar .dropdown').hover(function () {
      $(this).find('.dropdown-menu').first().stop(true, true).delay(250).slideDown();
    }, function () {
      $(this).find('.dropdown-menu').first().stop(true, true).delay(100).slideUp()
    });
    $('img.avatar-base').each(function (a, b) {
      //console.log('b from core', b, b.naturalWidth);
      if (b.naturalWidth === 0) {
        b.src = 'https://saapptscdn.blob.core.windows.net/thumbnail-avatars/default-user-avatar.jpg';
      }
    })
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
$(document).ready(function () { appts_core.init(); });