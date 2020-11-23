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
    public class MB_EmproyeeController : BaseController
    {
        #region Connect db / Data System
        private readonly DbConnection _db;
        public static string _mode = Comp.FormMode.ADD;

        public MB_EmproyeeController(DbConnection db)
        {
            _db = db;
        }
        #endregion

        public IActionResult Index()
        {
            ViewBag.Emproyee = _db.MB_Emproyee.ToList();
            return View();
        }

        public IActionResult FrmEmproyee(string mode, string id = null)
        {
            if (mode == "Add")
            {
                _mode = Comp.FormMode.ADD;
                FrmMode();
                return View();
            }
            else
            {
                _mode = Comp.FormMode.EDIT;
                FrmMode();
                var item = _db.MB_Emproyee.FirstOrDefault(x => x.EmproyeeId == id);
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
                ViewBag.Branch = _db.MG_Branch.ToList();
            }
            else
            {
                ViewData["Disible-delete"] = "";
                ViewData["Disible-save"] = "disabled";
                ViewData["Readonly"] = "readonly";
                ViewBag.Branch = _db.MG_Branch.ToList();
            }
        }
    }
}
