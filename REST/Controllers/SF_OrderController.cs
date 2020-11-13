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
using REST.Service;

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
            var item = _Get.TableById(id, branchid);
            ViewBag.Table = item;
            ViewBag.Food = _db.CD_Food.Where(x => x.BranchId == branchid).ToList();
            if (mode == null)
            {
                if (HttpContext.Session.GetString("Session_Order") != null)
                {
                    HttpContext.Session.Remove("Session_Order");
                }
            }
            if (item.TableST == 2)
            {                
                return RedirectToAction("OrderOld", new { id = id, mode = mode });
            }
            else
            {
                return View();
            }            
        }

        public IActionResult OrderOld(string id, string mode = null)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            ViewBag.GroupFood = _db.CD_GroupFood.Where(x => x.BranchId == branchid).ToList();
            var _Get = new GetCD_TableController(_db);
            var _Order = new GetSF_OrderController(_db);
            var item = _Get.TableById(id, branchid);
            ViewBag.Table = item;
            ViewBag.Food = _db.CD_Food.Where(x => x.BranchId == branchid).ToList();
            ViewBag.Order = _Order.OrderSub(id, null, branchid);
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
                Alert("", "กรุณาเลือกออร์เดอร์ !", Enums.NotificationType.warning);
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
                Alert("", "กรุณาเลือกออร์เดอร์ที่ต้องการลบ !", Enums.NotificationType.warning);
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
                    Alert("", "Error Data !", Enums.NotificationType.error);
                    return RedirectToAction("Order", new { id = id, mode = "view" });
                }
            }
            else
            {
                Alert("", "กรุณาเลือกรายการ !", Enums.NotificationType.warning);
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

                if (UpdateTable(id, 2, branchid))
                    return true;
                else
                    return false;             
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Boolean UpdateTable(string id, int status, string BranchId)
        {
            try
            {
                var Item = new CD_Table();
                Item = _db.CD_Table.FirstOrDefault(x => x.TableId == id && x.BranchId == BranchId);

                Item.TableST = status;

                _db.CD_Table.Update(Item);
                _db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // ----------------------------------------------------------------------------
        // Order Sub ***** Payment

        [HttpPost]
        public IActionResult CancelOrder(string orderid, string tableid, string foodid)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var Order = _db.SF_Order.FirstOrDefault(x => x.OrderId == orderid && x.BranchId == branchid);
            var OrderSub = _db.SF_OrderSub.Where(x => x.OrderId == orderid && x.TableId == tableid && x.FoodId == foodid && x.BranchId == branchid).FirstOrDefault();

            if (Order.ST == 1)
            {
                if (OrderSub.Status == 1)
                {
                    if (DeleteOrderSub(orderid, tableid, foodid, branchid))
                    {
                        toastrAlert("ข้อมูลโต๊ะอาหาร", "ยกเลิกเรียบร้อย", Enums.NotificationToastr.success);                     
                    }
                    else
                    {
                        Alert("", "Error Data", Enums.NotificationType.error);
                    }                    
                }
                else if (OrderSub.Status == 2)
                {
                    Alert("", "ไม่สามารถยกเลิกได้ เนื่องจากรายการนี้กำลังทำ", Enums.NotificationType.warning);
                }
                else
                {
                    Alert("", "ไม่สามารถยกเลิกได้ เนื่องจากรายการนี้ทำแล้ว", Enums.NotificationType.warning);
                }
            }
            else
            {
                Alert("", "ไม่สามารถยกเลิกได้ เนื่องจากออเดอร์นี้ทำแล้ว", Enums.NotificationType.warning);
            }
            

            return Json(new { data = "success" });
        }

        public Boolean DeleteOrderSub(string orderid, string tableid, string foodid, string branchid)
        {
            var Order = _db.SF_Order.FirstOrDefault(x => x.OrderId == orderid && x.BranchId == branchid);
            
            try
            {
                // ลบ Order Sub
                var i_ordersub = new SF_OrderSub();
                i_ordersub = _db.SF_OrderSub.FirstOrDefault(x => x.OrderId == orderid && x.TableId == tableid && x.FoodId == foodid && x.BranchId == branchid);
                _db.SF_OrderSub.Remove(i_ordersub);
                _db.SaveChanges();

                // เช็คว่า OrderSub ยังมีอยู่ไหม ถ้าไม่มีให้ลบ Order ทิ้ง 
                var OrderSub = _db.SF_OrderSub.Where(x => x.OrderId == orderid && x.TableId == tableid && x.BranchId == branchid).ToList();
                var i_order = new SF_Order();
                i_order = _db.SF_Order.FirstOrDefault(x => x.OrderId == orderid && x.TableId == tableid && x.BranchId == branchid);
                if (OrderSub.Count > 0)
                {                    
                    i_order.PriceTotal = i_order.PriceTotal - i_ordersub.Price;
                    // Data History
                    i_order.UpdateUser = User.Identity.Name;
                    i_order.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                    _db.SF_Order.Update(i_order);
                    _db.SaveChanges();
                }
                else
                {
                    _db.SF_Order.Remove(i_order);
                    _db.SaveChanges();
                }
                
                // ถ้า Table ใน Order ไม่มีแล้วให้เปลี่ยนสถานะ Table เป็นว่าง 1
                var Table = _db.SF_Order.Where(x => x.TableId == tableid && x.BranchId == branchid).ToList();
                if (Table.Count == 0)
                {
                    if (UpdateTable(tableid, 1, branchid))
                        return true;
                    else
                        return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // ----------------------------------------------------------------------------
        // Order ***** Kitchen

        public IActionResult OrderViewKitchen(string tableid, string orderdate)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var _Order = new GetSF_OrderController(_db);
            var List = _Order.Order(tableid, orderdate, branchid);
            return Json(new { data = List });
        }

        // -------------------------------------------
        // --                                       --
        // --           FUNCTION KITCHEN            --
        // --                                       --
        // -------------------------------------------

        public IActionResult receiveOrder(string OrderId, string FoodId)
        {
            var branch = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            // save data status ordersub 2 = กำลังดำเนินการ
            var i_OrderSub = _db.SF_OrderSub.FirstOrDefault(x => x.OrderId == OrderId && x.FoodId == FoodId && x.BranchId == branch);
            i_OrderSub.Status = 2;
            _db.SF_OrderSub.Update(i_OrderSub);
            _db.SaveChanges();
            // check status order = 1, 2
            var list1 = _db.SF_OrderSub.Where(x => x.OrderId == OrderId && x.Status == 1 || x.Status == 2 && x.BranchId == branch).ToList();
            if(list1.Count > 0)
            {
                // give status order = 2
                UpdateStatusOrder(OrderId, 2, branch);
            }
            else
            {
                // give status order = 3
                UpdateStatusOrder(OrderId, 3, branch);
            }

            return RedirectToAction("FrmKC_OrderSub", "Kitchen", new { id = OrderId });
        }

        public IActionResult completeOrder(string OrderId, string FoodId)
        {
            var branch = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            // search order sub
            var i_OrderSub = _db.SF_OrderSub.FirstOrDefault(x => x.OrderId == OrderId && x.FoodId == FoodId && x.BranchId == branch);
            if(i_OrderSub.Status == 2)
            {
                // save data status ordersub 3 = ทำเสร็จเรียบร้อยแล้ว
                i_OrderSub.Status = 3;
                _db.SF_OrderSub.Update(i_OrderSub);
                _db.SaveChanges();
                // check status ordersub = 1, 2
                var list1 = _db.SF_OrderSub.Where(x => x.OrderId == OrderId && x.Status == 1 || x.Status == 2 && x.BranchId == branch).ToList();
                if (list1.Count > 0)
                {
                    // give status order = 2
                    UpdateStatusOrder(OrderId, 2, branch);
                }
                else
                {
                    // give status order = 3
                    UpdateStatusOrder(OrderId, 3, branch);
                }
            }
            else
            {
                // if status not equal to status 2 not possible
                Alert("", "ไม่สามารถยืนยันอาหารได้ กรุณารับรายการอาหารก่อน", Enums.NotificationType.warning);
            }            

            return RedirectToAction("FrmKC_OrderSub", "Kitchen", new { id = OrderId });
        }

        public IActionResult cancelOrder(string OrderId, string FoodId)
        {
            var branch = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var i_OrderSub = _db.SF_OrderSub.FirstOrDefault(x => x.OrderId == OrderId && x.FoodId == FoodId && x.BranchId == branch);
            i_OrderSub.Status = 4;
            _db.SF_OrderSub.Update(i_OrderSub);
            _db.SaveChanges();
            // check status order = 1, 2
            var list1 = _db.SF_OrderSub.Where(x => x.OrderId == OrderId && x.Status == 1 || x.Status == 2 && x.BranchId == branch).ToList();
            if (list1.Count > 0)
            {
                // give status order = 2
                UpdateStatusOrder(OrderId, 2, branch);
            }
            else
            {
                // give status order = 3
                UpdateStatusOrder(OrderId, 3, branch);
            }

            return RedirectToAction("FrmKC_OrderSub", "Kitchen", new { id = OrderId });
        }

        public Boolean UpdateStatusOrder(string OrderId, int status, string branchid)
        {
            try
            {
                var Item = _db.SF_Order.FirstOrDefault(x => x.OrderId == OrderId && x.BranchId == branchid);
                Item.ST = status;
                _db.SF_Order.Update(Item);
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
