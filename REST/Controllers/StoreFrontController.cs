using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST.ApiControllers;
using REST.Data;
using REST.Models;

namespace REST.Controllers
{
    [Authorize]
    public class StoreFrontController : Controller
    {
        #region db
        private readonly DbConnection _db;

        public StoreFrontController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        public IActionResult Index()
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            ViewBag.TableCount = _db.CD_Table.Where(x => x.BranchId == branchid).Count();
            ViewBag.TableST_1 = _db.CD_Table.Where(x => x.TableST == 1 & x.BranchId == branchid).Count();
            ViewBag.TableST_2 = _db.CD_Table.Where(x => x.TableST == 2 & x.BranchId == branchid).Count();
            ViewBag.TableST_3 = _db.CD_Table.Where(x => x.TableST == 3 & x.BranchId == branchid).Count();
            ViewBag.SL_Zone = _db.CD_Zone.Where(x => x.BranchId == branchid).ToList();            
            var _Get = new GetCD_TableController(_db);
            ViewBag.Table = _Get.Table(null, null, branchid);

            return View();
        }     

        [HttpPost]
        public IActionResult GetTable(string ZoneId, string Status)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var _Get = new GetCD_TableController(_db);
            var List = _Get.Table(ZoneId, Status, branchid);
            return Json(new { data = List });
        }
    }
}
