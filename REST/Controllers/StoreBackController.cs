using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace REST.Controllers
{
    [Authorize]
    public class StoreBackController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
