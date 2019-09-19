using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace B2CApp.Controllers
{
    [Route("/Redirect")]
    public class RedirectController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}