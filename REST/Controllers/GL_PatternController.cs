using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REST.Data;
using REST.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Controllers
{
    [Authorize]
    public class GL_PatternController : BaseController
    {
        #region Connect db / Data System
        private readonly DbConnection _db;
        public static string _mode = Comp.FormMode.ADD;

        public GL_PatternController(DbConnection db)
        {
            _db = db;
        }
        #endregion 

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FrmPattern(string mode, string id = null)
        {
            if (mode == "Add")
            {
                _mode = Comp.FormMode.ADD;
                FrmMode();
                ViewBag.DataTable = _db.GL_AccountChart.ToList();
                return View();
            }
            else
            {
                _mode = Comp.FormMode.EDIT;
                FrmMode();
                ViewBag.DataTable = _db.GL_AccountChart.ToList();
                var item = _db.GL_Pattern.FirstOrDefault();
                return View(item);
            }
        }

        public void FrmMode()
        {
            if (_mode == Comp.FormMode.ADD)
            {
                ViewData["Disible-delete"] = "disabled";
                ViewData["Disible-save"] = "";
                ViewData["Readonly"] = "";
                ViewBag.AccountBook = _db.GL_AccountBook.ToList();
            }
            else
            {
                ViewData["Disible-delete"] = "";
                ViewData["Disible-save"] = "";
                ViewData["Readonly"] = "readonly";
                ViewBag.AccountBook = _db.GL_AccountBook.ToList();
            }
        }
    }
}
