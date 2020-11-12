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
            ViewBag.Table = _Get.TableZoneStatus(null, null, branchid);

            return View();
        }     

        public IActionResult FrmSF_Table(string id)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var _Table = new GetCD_TableController(_db);
            ViewBag.Table = _Table.TableById(id, branchid).FirstOrDefault();
            var _OrderSub = new GetSF_OrderController(_db);
            var OrderSub = _OrderSub.OrderSub(id, null, branchid);
            if (OrderSub.Count > 0)
            {
                ViewBag.OrderSub = OrderSub;
                ViewData["Disible"] = "";
            }
            else
            {
                ViewBag.OrderSub = OrderSub;
                ViewData["Disible"] = "disabled";
            }
           
            return View();
        }        

        [HttpPost]
        public IActionResult GetTable(string ZoneId, string Status)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var _Get = new GetCD_TableController(_db);
            var List = _Get.TableZoneStatus(ZoneId, Status, branchid);
            return Json(new { data = List });
        }
    }
}
