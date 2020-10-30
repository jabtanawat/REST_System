using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace REST.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorStatus = statusCode;
                    ViewBag.ErrorMessage = "Sorry, the resource you requested could not be found";
                    break;
                case 500:
                    ViewBag.ErrorStatus = statusCode;
                    ViewBag.ErrorMessage = "Sorry, the resource you requested could not be found";
                    break;
            }

            return View("_NotFound");
        }
    }
}
