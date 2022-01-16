
/**
 * Author: Adam DeRose
 * Date: 9/8/2019
 * Support viewing user lookups
 */
var appts_a_view_user_lookups = (function () {
  var p = {}, _p = {};
  _p.test = {};
  p.init = function () {
    _p.handle.register();
    _p.initDetailModal();
  };
  _p.initDetailModal = function () {
    $('body').on('click', '.a-get-docs-for-lookup', function () {
      var userId = $(this).closest('.card').attr('id');
      console.log('user id from card id ', userId)
      _p.getUserDocuments(userId)
        .done(function (data) {
          //console.log('appt detail gotten', data);
          $('#detailModal').modal('show');
        })
        .fail(function (data) {
          console.log('fail from init detail modal');
        });
    });
    $('#detailModal').on('show.bs.modal', function (e) {
      //console.log('my modal has been shown');
    });
  }
  _p.getUserDocuments = function (userId) {    
    return $.ajax({
      url: '/Admin/GetUserDocumentsById?userId=' + userId,
      type: 'GET',
      contentType: 'application/json; charset=utf-8'
    })
      .done(function (data) {
        //clear previous info
        $('.modal-body .card#user-docs pre code').each(
          function (a, b) {
            $(b).html('');
          }
        );
        $('.modal-body .card#subscription-docs pre code').each(
          function (a, b) {
            $(b).html('');
          }
        );
        $('.modal-body .card#availability-docs .list-group').html('');
        $('.modal-body .card#appt-type-docs .list-group').html('');
        $('.modal-body .card#appointment-docs .list-group').html('');
        console.log('s get user doc by id', data)
        if (data.subscriptionJson) {
          var sub = $('#subscription-json');
          sub.html(data.subscriptionJson);
        }
        if (data.serviceProviderJson) {
          var sub = $('#service-provider-json');
          sub.html(data.serviceProviderJson);
        }
        if (data.trialPromoJson) {
          var sub = $('#trial-promo-json');
          sub.html(data.trialPromoJson);
        }
        if (data.stripeCustomerJson) {
          var sub = $('#stripe-customer-json');
          sub.html(data.stripeCustomerJson);
        }
        if (data.clientJson) {
          var sub = $('#client-json');
          sub.html(data.clientJson);
        }
        if (data.availabilityJson) {
          var lg = $('#availability-docs .list-group');
          for (var i = 0; i < data.availabilityJson.length; i++) {
            lg.append('<li class="list-group-item"><pre><code>' + data.availabilityJson[i] + '</pre></code></li>')
          }
        }
        if (data.appointmentTypesJson) {
          var lg = $('#appt-type-docs .list-group');
          for (var i = 0; i < data.appointmentTypesJson.length; i++) {
            lg.append('<li class="list-group-item"><pre><code>' + data.appointmentTypesJson[i] + '</pre></code></li>')
          }
        }
        if (data.appointmentsJson) {
          var lg = $('#appointment-docs .list-group');
          for (var i = 0; i < data.appointmentsJson.length; i++) {
            lg.append('<li class="list-group-item"><pre><code>' + data.appointmentsJson[i] + '</pre></code></li>')
          }
        }
      })
      .fail(function (data) {
        console.log('e update timezoneid');
        $('.modal-body').append('<p>there was an error</p>');
      });
  };
  p.handle = {
    onChange: {

    }
  };

  /// <summary>
  /// Container to define event handlers.
  /// </summary>
  _p.handle = {
    onClick: {
      showMoreDetails: function () {
        $('#a-more-details').on('click', function () {
          $('#tf-more-info').slideToggle();
          //$('.td-details-show-more').toggle();
          $('#tbl-more-info').toggle();
        });
      }
    },
    onChange: {

    },
    onHover: {

    }
  };
  /// <summary>
  /// Register all event handlers.
  /// </summary>
  _p.handle.register = function () {

  };
  return p;
})();
$(document).ready(function () { appts_a_view_user_lookups.init(); });