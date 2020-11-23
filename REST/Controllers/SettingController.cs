using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REST.Data;
using REST.Service;

namespace REST.Controllers
{
    [Authorize]
    public class SettingController : BaseController
    {
        #region Connect db / Data System
        private readonly DbConnection _db;
        public static string _mode = Comp.FormMode.ADD;

        public SettingController(DbConnection db)
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
