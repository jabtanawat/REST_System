using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using REST.Data;
using REST.Models;

namespace REST.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly DbConnection _db;
        public HomeController(DbConnection db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
