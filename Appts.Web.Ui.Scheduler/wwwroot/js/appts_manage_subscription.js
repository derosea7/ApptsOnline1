/**
 * Author: Adam James DeRose
 * Date: 10/6/2019
 * Support management of service provider subscription.
 */
var appts_manage_subscription = (function () {
  var p = {}, _p = {};
  _p.test = {};
  p.init = function () {
    _p.handle.register();
  };
  _p.const = {};
  _p.ids = {};
  _p.names = {};
  _p.get = {};
  _p.set = {};
  /// <summary>
  /// Container to define event handlers.
  /// </summary>
  _p.handle = {
    onClick: {
      payNow: function () {
        $('#btn-pay-now').on('click', function () {
          var regularButtonText = $(this).find('.btn_text');
          var loading = $(this).find('.loading_StripeCheckout');
          regularButtonText.hide();
          loading.show();
          _p.startCheckoutSession()
            .done(function (data) {
              console.log('s', data);
              var stripe = Stripe('pk_test_U3f8UpUfYMOowD4OJVgiKFzs00N7krR159');
              stripe.redirectToCheckout({
                // Make the id field from the Checkout Session creation API response
                // available to this file, so you can provide it as parameter here
                // instead of the {{CHECKOUT_SESSION_ID}} placeholder.
                sessionId: data.checkoutSessionId
              }).then(function (result) {
                // If `redirectToCheckout` fails due to a browser or network
                // error, display the localized error message to your customer
                // using `result.error.message`.
                console.log('failed redirectToCheckout', result);
              });
            })
            .fail(function (data) {
              console.log('f', data);
              regularButtonText.show();
              loading.hide();
            });
        });
      },
      showConfirmCancelModal: function () {
        $('#btn-cancel-subscription').on('click', function () {
          $('#mdl-confirm-cancelation').modal('show');
        });
      }
    },
    onHover: {},
    onChange: {}
  };
  p.handle = {};
  /// <summary>
  /// Register all event handlers.
  /// </summary>
  _p.handle.register = function () {
    _p.handle.onClick.payNow();
    _p.handle.onClick.showConfirmCancelModal();
  };
  _p.startCheckoutSession = function () {
    var endpoint = 'https://' + window.location.host + '/Stripe/StartCheckoutSession';
    return $.ajax({
      url: endpoint,
      type: 'POST',
      contentType: 'application/json; charset=utf-8',
      dataType: 'json'
    });
  };
  return p;
})();
$(document).ready(function () { appts_manage_subscription.init(); });