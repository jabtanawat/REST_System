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
using REST.Service;

namespace REST.Controllers
{
    [Authorize]
    public class StoreFrontController : BaseController
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
            ViewBag.Table = _Table.TableById(id, branchid);
            var _OrderSub = new GetSF_OrderController(_db);
            var OrderSub = _OrderSub.OrderSub(id, null, null, branchid);
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

        public IActionResult FrmDataTable(string id)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var _Table = new GetCD_TableController(_db);
            var table = _Table.TableById(id, branchid);
            if(table.TableST == 3)
            {
                ViewData["check-bill"] = "disabled";
            }
            else
            {
                ViewData["check-bill"] = "";
            }
            ViewBag.Table = table;
            var _OrderSub = new GetSF_OrderController(_db);
            ViewBag.OrderSub = _OrderSub.OrderSubTable(id, null, branchid);
            return View();
        }

        public IActionResult CheckBill(string id)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;    
            var _OrderSub = new GetSF_OrderController(_db);
            try
            {
                var ordersub = _OrderSub.OrderSub(id, null, "1", branchid);

                if(ordersub.Count == 0)
                {
                    Alert("", "ไม่มีรายการอาหาร !", Enums.NotificationType.warning);
                    return RedirectToAction("FrmDataTable", "StoreFront", new { id = id });
                }

                var _get = new GetRunningController(_db);
                string DocRunning = _get.Running("Bill", branchid);

                int i = 0;
                decimal PTotal = 0;

                // หาจำนวนเงินทั่้งหมด
                foreach (var row in ordersub)
                {
                    PTotal += row.Price * row.Amount;
                }

                // Save Bill
                var item = new SF_Bill();
                item.BillId = DocRunning;
                item.TableId = id;
                item.St = 1;
                item.Dates = Share.FormatDate(DateTime.Now).Date;
                item.BranchId = branchid;
                item.CreateUser = User.Identity.Name;
                item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                item.UpdateUser = User.Identity.Name;
                item.UpdateDate = Share.FormatDate(DateTime.Now).Date;
                _db.SF_Bill.Add(item);
                _db.SaveChanges();

                //Svae Order Sub
                foreach (var row in ordersub)
                {
                    i++;

                    var itemSub = new SF_BillSub();
                    itemSub.BillId = DocRunning;
                    itemSub.i = i;
                    itemSub.FoodId = row.FoodId;
                    itemSub.Amount = row.Amount;
                    itemSub.Price = row.Price;
                    itemSub.BranchId = branchid;
                    _db.SF_BillSub.Add(itemSub);
                    _db.SaveChanges();                    
                }

                // Update Success Order = 2 เช็คบิลออเดอร์เรียบร้อยแล้ว
                foreach (var row in ordersub)
                {
                    var itemSub = _db.SF_Order.FirstOrDefault(x => x.OrderId == row.OrderId && x.BranchId == branchid);
                    itemSub.Success = 2;
                    _db.SF_Order.Update(itemSub);
                    _db.SaveChanges();
                }

                // Updata Status Table = 1 สถานะว่าง
                var table = _db.CD_Table.FirstOrDefault(x => x.TableId == id && x.BranchId == branchid);
                table.TableST = 1;
                _db.CD_Table.Update(table);
                _db.SaveChanges();

                // Set Runnig
                _get.SetRunning("Bill", DocRunning, branchid);

                toastrAlert("เช็คบิล", "เรียบร้อยแล้ว", Enums.NotificationToastr.success);
                return RedirectToAction("Index", "StoreFront");
            }
            catch (Exception)
            {
                Alert("", "Error Data !", Enums.NotificationType.warning);
                return RedirectToAction("FrmDataTable", "StoreFront", new { id = id });
            }
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
