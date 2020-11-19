using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REST.Data;

namespace REST.Controllers
{
    [Authorize]
    public class ManageController : BaseController
    {
        #region Connect db / Data System

        private readonly DbConnection _db;
        public ManageController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        public IActionResult Index()
        {
            return View();
        }
    }
}
