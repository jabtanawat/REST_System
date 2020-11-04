using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using REST.ApiControllers;
using REST.Data;
using REST.Models;
using REST.Service;
using REST.ViewModels;
using static REST.Service.Enums;

namespace REST.Controllers
{
    [Authorize]
    public class SF_OrderController : BaseController
    {
        #region Connection db
        private readonly DbConnection _db;
        public SF_OrderController(DbConnection db)
        {
            _db = db;
        }
        #endregion 

        public IActionResult Order(string id, string mode = null)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            ViewBag.GroupFood = _db.CD_GroupFood.Where(x => x.BranchId == branchid).ToList();
            var _Get = new GetCD_TableController(_db);
            ViewBag.Table = _Get.TableById(id, branchid).FirstOrDefault();
            ViewBag.Food = _db.CD_Food.Where(x => x.BranchId == branchid).ToList();
            if (mode == null)
            {
                if (HttpContext.Session.GetString("Session_Order") != null)
                {
                    HttpContext.Session.Remove("Session_Order");
                }
            }
            return View();
        }

        public IActionResult GetFood(string id)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            if(id != null)
            {
                var List = _db.CD_Food.Where(x => x.GroupFoodId == id && x.BranchId == branchid).ToList();
                return Json(new { data = List });
            }
            else
            {
                var List = _db.CD_Food.Where(x => x.BranchId == branchid).ToList();
                return Json(new { data = List });
            }           
        }

        public IActionResult GetOrder()
        {
            var List = new List<ViewOrder>();
            if (HttpContext.Session.GetString("Session_Order") != null)
            {
                List = JsonConvert.DeserializeObject<List<ViewOrder>>(HttpContext.Session.GetString("Session_Order"));
            }
            return Json(new { data = List });
        }

        [HttpPost]
        public IActionResult Add(string id, decimal amount)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var List = new List<ViewOrder>();
            if (id != null)
            {
                var Food = new CD_Food();
                Food = _db.CD_Food.FirstOrDefault(x => x.FoodId == id && x.BranchId == branchid);

                if (HttpContext.Session.GetString("Session_Order") != null)
                {
                    List = JsonConvert.DeserializeObject<List<ViewOrder>>(HttpContext.Session.GetString("Session_Order"));
                }

                if (List.Exists(x => x.FoodId == id))
                {
                    // ถ้ามีรหัสอยู่แล้ว ให้ลบทิ้งก่อน แล้วค่อยเพิ่มเข้าไปใหม่
                    var delete = List.Find(x => x.FoodId == id);
                    List.Remove(delete);

                    var item = new ViewOrder
                    {
                        FoodId = id,
                        FoodName = Food.FoodName,
                        Price = Food.Price,
                        Amount = amount,
                    };

                    List.Add(item);
                }
                else
                {
                    var item = new ViewOrder
                    {
                        FoodId = id,
                        FoodName = Food.FoodName,
                        Price = Food.Price,
                        Amount = amount,
                    };

                    List.Add(item);
                }

                HttpContext.Session.SetString("Session_Order", JsonConvert.SerializeObject(List));
            }
            else
            {
                Alert("", "กรุณาเลือกออร์เดอร์ !", NotificationType.warning);
            }

            return Json(new { data = List });
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var List = new List<ViewOrder>();
            if (id != null)
            {

                if (HttpContext.Session.GetString("Session_Order") != null)
                {
                    List = JsonConvert.DeserializeObject<List<ViewOrder>>(HttpContext.Session.GetString("Session_Order"));
                }

                if (List.Exists(x => x.FoodId == id))
                {
                    // ลบข้อมูลใน session 
                    var delete = List.Find(x => x.FoodId == id);
                    List.Remove(delete);
                }

                HttpContext.Session.SetString("Session_Order", JsonConvert.SerializeObject(List));
            }
            else
            {
                Alert("", "กรุณาเลือกออร์เดอร์ที่ต้องการลบ !", NotificationType.warning);
            }

            return Json(new { data = List });
        }

        [HttpPost]
        public IActionResult Cancel()
        {
            string status = "success";
            if (HttpContext.Session.GetString("Session_Order") != null)
            {
                HttpContext.Session.Remove("Session_Order");
            }
            return Json(new { data = status });
        }


        public IActionResult Save(string id)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var List = new List<ViewOrder>();
            if (HttpContext.Session.GetString("Session_Order") != null)
            {
                List = JsonConvert.DeserializeObject<List<ViewOrder>>(HttpContext.Session.GetString("Session_Order"));
            }

            if(List.Count > 0 )
            {
                if (SaveData(List, id, branchid))
                {
                    toastrAlert("รับออเดอร์", "บันทึกข้อมูลเรียบร้อย", Enums.NotificationToastr.success);
                    return RedirectToAction("Index", "StoreFront");
                }
                else
                {
                    Alert("", "Error Data !", NotificationType.error);
                    return RedirectToAction("Order", new { id = id, mode = "view" });
                }
            }
            else
            {
                Alert("", "กรุณาเลือกรายการ !", NotificationType.warning);
                return RedirectToAction("Order", new { id = id });
            }            
        }        

        public Boolean SaveData(List<ViewOrder> Results, string id, string branchid)
        {
            try
            {
                var _Get = new GetRunningController(_db);
                string DocRunning = _Get.Running("Order", branchid);

                int i = 0;
                decimal PTotal = 0;

                // ค้นหาออร์เดอร์ว่ามี ออร์เดอร์อยู่ไหม ถ้านับออร์เดอร์เพิ่ม ++
                var Order = _db.SF_Order.Where(x => x.TableId == id && x.BranchId == branchid).ToList();

                i = Order.Count + 1;

                // หาจำนวนเงินทั่้งหมด
                foreach (var row in Results)
                {
                    PTotal += row.Price * row.Amount;
                }

                // Save Order
                var Item = new SF_Order();
                Item.OrderId = DocRunning;
                Item.TableId = id;
                Item.OrderDt = Share.FormatDate(DateTime.Now).Date;
                Item.PriceTotal = PTotal;
                Item.ST = 1;
                Item.BranchId = branchid;
                Item.CreateUser = User.Identity.Name;
                Item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                Item.UpdateUser = User.Identity.Name;
                Item.UpdateDate = Share.FormatDate(DateTime.Now).Date;
                _db.SF_Order.Add(Item);
                _db.SaveChanges();

                //Svae Order Sub
                foreach (var row in Results)
                {
                    var ItemSub = new SF_OrderSub();
                    ItemSub.OrderId = DocRunning;
                    ItemSub.i = i;
                    ItemSub.TableId = id;
                    ItemSub.FoodId = row.FoodId;
                    ItemSub.Amount = row.Amount;
                    ItemSub.Price = row.Price;
                    ItemSub.Status = 1;
                    ItemSub.BranchId = branchid;
                    _db.SF_OrderSub.Add(ItemSub);
                    _db.SaveChanges();
                }                

                if (HttpContext.Session.GetString("Session_Order") != null)
                {
                    HttpContext.Session.Remove("Session_Order");
                }

                // Set Runnig
                _Get.SetRunning("Order", DocRunning, branchid);

                if (UpdateTable(id, branchid))
                    return true;
                else
                    return false;             
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Boolean UpdateTable(string id, string BranchId)
        {
            try
            {
                var Item = new CD_Table();
                Item = _db.CD_Table.FirstOrDefault(x => x.TableId == id && x.BranchId == BranchId);

                Item.TableST = 2;

                _db.CD_Table.Update(Item);
                _db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }    
    }
}
