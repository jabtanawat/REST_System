using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST.ApiControllers;
using REST.Data;
using REST.Models;
using REST.Service;
using REST.ViewModels;
using static REST.Service.Enums;

namespace REST.Controllers
{
    [Authorize]
    public class SF_PaymentController : BaseController
    {
        #region Connection db
        private readonly DbConnection _db;

        public SF_PaymentController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        public IActionResult FrmPayment()
        {
            return View();
        }

        //public IActionResult FrmPayment(string TableId = null)
        //{
        //    var branch = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
        //    var _table = new GetCD_TableController(_db);
        //    var _ordersub = new GetSF_OrderController(_db);
        //    var item = new ViewFrmPayment();
        //    if(TableId != null)
        //    {
        //        var order = _db.SF_OrderSub.Where(x => x.TableId == TableId && x.Status == 1 || x.Status == 2 && x.BranchId == branch).ToList();
        //        if(order.Count > 0)
        //        {
        //            item.TableId = TableId;
        //            Alert("", "รายการอาหาร ยังทำไม่เสร็จหรือได้ไม่ครบ", Enums.NotificationType.warning);
        //            return View(item);
        //        }
        //        else
        //        {
        //            var table = _table.TableById(TableId, branch);
        //            var info = _ordersub.OrderSubByTableId(TableId, branch);                    
        //            decimal price = 0;
        //            foreach (var row in info)
        //            {
        //                price += row.Price * row.Amount;
        //            }
        //            // input pricer ordersub
        //            item.TableId = TableId;
        //            item.TableName = table.TableName;
        //            item.St = table.TableST;
        //            if (table.TableST == 1)
        //            {                        
        //                item.Status = "ว่าง";
        //            }else if(table.TableST == 2)
        //            {
        //                item.Status = "ใช้บริการอยู่";
        //            }
        //            else if(table.TableST == 3)
        //            {
        //                item.Status = "จอง";
        //            }
        //            item.Total = Share.Cnumber(Share.FormatDouble(price), 2);
        //            item.Balance = Share.Cnumber(Share.FormatDouble(price), 2);
        //            item.OrderSub = _ordersub.OrderSub(TableId, null, branch);

        //            return View(item);
        //        }                
        //    }
        //    else
        //    {
        //        return View(item);
        //    }            
        //}

        [HttpPost]
        public IActionResult FrmPayment(ViewFrmPayment info)
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            if (ModelState.IsValid)
            {
                if (SaveData(info, branchid))
                {
                    toastrAlert("ชำระเงิน", "ชำระเงินเรียบร้อย", Enums.NotificationToastr.success);
                    return RedirectToAction("Index", "StoreFront");
                }
                else
                {                    
                    return View();
                }                
            }
            else
            {
                Alert("", "ไม่สามารถบันทึกข้อมูลได้ !", Enums.NotificationType.error);
                return View();
            }            
        }

        public Boolean SaveData(ViewFrmPayment info, string branchid)
        {         
            var _Get = new GetRunningController(_db);
            var _order = new SF_OrderController(_db);
            string Doc = null;
            try
            {
                /* Get Running */
                Doc = _Get.Running("Payment", branchid);

                // Save Payment
                var item = new SF_Payment();
                item.PaymentId = Doc;
                item.TableId = info.TableId;
                item.Total = Share.FormatDecimal(info.Total);
                item.Persen = info.Persen;
                item.MemberId = info.MemberId;
                item.Rebate = info.Rebate;
                item.Score = info.Score;
                item.Balance = Share.FormatDecimal(info.Balance);
                item.PayType = info.PayType;
                item.MoneyPut = info.MoneyPut;
                item.MoneyChange = info.MoneyChange;
                item.Dates = Share.FormatDate(DateTime.Now).Date;
                /* DATA */
                item.BranchId = branchid;
                item.CreateUser = User.Identity.Name;
                item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                item.UpdateUser = User.Identity.Name;
                item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                _db.SF_Payment.Add(item);
                _db.SaveChanges();

                // Save Payment Sub
                var result = _db.SF_OrderSub.Where(x => x.TableId == info.TableId && x.BranchId == branchid).ToList();
                foreach (var row in result)
                {
                    var itemsub = new SF_PaymentSub();
                    itemsub.PaymentId = Doc;
                    itemsub.i = row.i;
                    itemsub.TableId = row.TableId;
                    itemsub.FoodId = row.FoodId;
                    itemsub.Amount = row.Amount;
                    itemsub.Price = row.Price;
                    itemsub.Status = row.Status;
                    itemsub.BranchId = branchid;

                    _db.SF_PaymentSub.Add(itemsub);
                    _db.SaveChanges();
                }

                // Set Running
                _Get.SetRunning("Payment", Doc, branchid);

                // Set Update Table Status 1
                _order.UpdateTable(info.TableId, 1, branchid);

                // Remove Order
                var order = _db.SF_Order.Where(x => x.TableId == info.TableId && x.BranchId == branchid).ToList();
                _db.SF_Order.RemoveRange(order);
                _db.SaveChanges();

                // Remove OrderSub
                var ordersub = _db.SF_OrderSub.Where(x => x.TableId == info.TableId && x.BranchId == branchid).ToList();
                _db.SF_OrderSub.RemoveRange(ordersub);
                _db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                Alert("", "Error Data !", Enums.NotificationType.error);
                return false;
            }
        }

        public ViewFrmPayment info(ViewFrmPayment info)
        {
            var item = new ViewFrmPayment();

            return item;
        }

        public IActionResult Payment1(string id)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;            
            var _Get1 = new GetCD_TableController(_db);
            var _Get2 = new GetSF_OrderController(_db);
            ViewBag.Table = _Get1.TableById(id, branchid);
            ViewBag.OrderSub = _Get2.OrderSub(id, null, branchid);
            var order = _db.SF_Order.Where(x => x.TableId == id && x.BranchId == branchid).ToList();
            decimal Total = 0;
            foreach (var row in order)
            {
                Total += row.PriceTotal;
            }
            var item = new ViewFrmPayment();
            item.Total = Total.ToString("N2");
            item.Balance = Total.ToString("N2");
            return View(item);
        }

        [HttpPost]
        public IActionResult ShowMember(string id)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var _Get = new GetMB_MemberController(_db);
            var member = _Get.ViewMemberById(id, branchid);
            var item = new ViewMB_Member();
            item.MemberId = id;
            item.Name = member.Name;
            item.Rebate = member.Rebate;
            item.Score = member.Score;
            return Json(new { data = item });
        }

        [Route("/Payment/{id}")]
        public IActionResult Index(string id)
        {
            // ข้อมูล OrderSub
            var OrderSub = GetOrderSub(id);
            ViewBag.OrderSub = OrderSub;

            // รหัส Table Id
            ViewBag.TableId = id;

            return View();
        }

        [HttpPost]
        public IActionResult Paymenta(string TableId)
        {
            string Status = null;

            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;

            var DT = new DataTable();
            var sql = $"SELECT * FROM SF_Order WHERE TableId = '{TableId}' AND BranchId = '{BranchId}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    DT.Load(data);
                }
            }

            if(DT.Rows.Count > 0)
            {
                // Delete OrderSub
                var Delete_OrderSub = $"DELETE FROM SF_OrderSub WHERE TableId = '{TableId}' AND BranchId = '{BranchId}'";
                using (var command = _db.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = Delete_OrderSub;
                    _db.Database.OpenConnection();
                    using (var data = command.ExecuteReader())
                    {
                        data.Read();
                    }
                }

                // Delete Order
                var Delete_Order = $"DELETE FROM SF_Order WHERE TableId = '{TableId}' AND BranchId = '{BranchId}'";
                using (var command = _db.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = Delete_Order;
                    _db.Database.OpenConnection();
                    using (var data = command.ExecuteReader())
                    {
                        data.Read();
                    }
                }

                // Update Status Table
                var Item = new CD_Table();
                Item = _db.CD_Table.FirstOrDefault(x => x.TableId == TableId && x.BranchId == BranchId);

                Item.TableST = 1;

                _db.CD_Table.Update(Item);
                _db.SaveChanges();
                                
            }

            Status = "success";

            AlertTop("ชำระเงินเรียบร้อยแล้ว !", NotificationType.success);
            return Json(new { data = Status });
        }

        // ------
        public List<ViewOrder> GetOrderSub(string id)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;

            var List = new List<ViewOrder>();
            var sql = $"SELECT OrderId, SF_OrderSub.i, TableId, CD_Food.FoodId, CD_Food.FoodName, Amount, CD_Food.Price, Status "
                    + $"FROM SF_OrderSub "
                    + $"LEFT JOIN CD_Food ON SF_OrderSub.FoodId = CD_Food.FoodId "
                    + $"WHERE TableId = '{id}' AND SF_OrderSub.BranchId = '{BranchId}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewOrder();
                        Item.OrderId = data.GetString(0);
                        Item.i = data.GetInt32(1);
                        Item.TableId = data.GetString(2);
                        Item.FoodId = data.GetString(3);
                        Item.FoodName = data.GetString(4);
                        Item.Amount = data.GetDecimal(5);
                        Item.Price = data.GetDecimal(6);
                        Item.Status = data.GetInt32(7);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }

        [HttpPost]
        public IActionResult GetOrder(string id)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;

            var List = new List<ViewOrder>();
            var sql = $"SELECT OrderId, SF_OrderSub.i, TableId, CD_Food.FoodId, CD_Food.FoodName, Amount, CD_Food.Price, Status "
                    + $"FROM SF_OrderSub "
                    + $"LEFT JOIN CD_Food ON SF_OrderSub.FoodId = CD_Food.FoodId "
                    + $"WHERE TableId = '{id}' AND SF_OrderSub.BranchId = '{BranchId}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewOrder();
                        Item.OrderId = data.GetString(0);
                        Item.i = data.GetInt32(1);
                        Item.TableId = data.GetString(2);
                        Item.FoodId = data.GetString(3);
                        Item.FoodName = data.GetString(4);
                        Item.Amount = data.GetDecimal(5);
                        Item.Price = data.GetDecimal(6);
                        Item.Status = data.GetInt32(7);
                        List.Add(Item);
                    }
                }
            }

           return Json(new { data = List });
        }

    }
}
