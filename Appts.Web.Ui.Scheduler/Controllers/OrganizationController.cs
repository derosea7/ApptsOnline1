using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Appts.Web.Ui.Scheduler.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Appts.Web.Ui.Scheduler.Controllers
{
  [Authorize]
  public class OrganizationController : Controller
  {
    private readonly IOrganizationRepository _organizationRepository;
    public OrganizationController(IOrganizationRepository organizationRepository)
    {
      _organizationRepository = organizationRepository;
    }
    public IActionResult Index()
    {
      string[] testVales = _organizationRepository.GetTestValuesAsync()
        .GetAwaiter().GetResult();
      return View(testVales);
    }
    public IActionResult Onboard()
    {
      return View();
    }

    public IActionResult Settings()
    {
      return View();
    }
  }
}
