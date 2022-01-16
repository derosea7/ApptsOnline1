using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Web.Ui.Scheduler.Services;
using Appts.Models.Rest;
using Microsoft.AspNetCore.Authorization;
using Appts.Web.Ui.Scheduler.ViewModels;

namespace Appts.Web.Ui.Scheduler.Controllers
{
  public class PositionController : Controller
  {
    private readonly IApiClient _apiClient;
    private readonly IHttpContextResolverService _httpContext;
    public PositionController(IApiClient api, IHttpContextResolverService httpContext)
    {
      _apiClient = api;
      _httpContext = httpContext;
    }
    public IActionResult Index()
    {
      return View();
    }
    [Authorize]
    public IActionResult Transactions()
    {
      string userId = _httpContext.GetUserId();
      //load transactions
      var vm = new ViewTransactionsVm();
      var resonse = _apiClient.GetAsync<GetStockTransactionsResponse>(
        $"api/Stock/GetTransactionsApi?userId={userId}").GetAwaiter().GetResult();

      //foreach (StockTransactionDocument trans in resonse.Transactions)
      //{
      //  trans.
      //}

      vm.Transactions = resonse.Transactions;
      return View(vm);
    }
  }
}
