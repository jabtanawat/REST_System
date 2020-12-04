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

        public IActionResult FrmPayment(string id = null)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var item = new ViewFrmPayment();
            
            if(id != null)
            {
                item = LoadData(id, branchid);
            }
            else
            {
                item.TableST = "ว่าง";
                item.Balance = "0.00";
                item.ServicePersen = "0.00";
                item.ServiceBath = "0.00";
                item.MemberPersen = "0.00";
                item.MemberBath = "0.00";
                item.Persen = "0.00";
                item.PersenBath = "0.00";
                item.SumBalance = "0.00";
            }
            return View(item);
        }        

        [HttpPost]
        public IActionResult FrmPayment(ViewFrmPayment info)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var item = new SF_Payment();            
            var _Bill = new GetSF_BillController(_db);
            int i = 0;
            try
            {
                // รายการอาหาร
                var billsub = _Bill.BillSubById(info.BillId, branchid);
                // เลข running
                var _Running = new GetRunningController(_db);
                string DocRunning = _Running.Running("Payment", branchid);

                // Updata Status Table = 1 ว่าง
                var list = _db.SF_Payment.Where(x => x.BillId == info.BillId && x.BranchId == branchid).ToList();
                if (list.Count == 0)
                {
                    var table = _db.CD_Table.FirstOrDefault(x => x.TableId == info.TableId && x.BranchId == branchid);
                    table.TableST = 1;
                    _db.CD_Table.Update(table);
                    _db.SaveChanges();
                }

                // Save Payment
                item.PaymentId = DocRunning;
                item.BillId = info.BillId;
                item.TableId = info.TableId;
                item.MemberId = info.MemberId;
                item.St = 1;
                item.Dates = Share.FormatDate(DateTime.Now).Date;
                item.Total = Share.FormatDecimal(info.Total);
                item.ServicePersen = Share.FormatDecimal(info.ServicePersen);
                item.ServiceBath = Share.FormatDecimal(info.ServiceBath);
                item.MemberPersen = Share.FormatDecimal(info.MemberPersen);
                item.MemberBath = Share.FormatDecimal(info.MemberBath);
                item.Persen = Share.FormatDecimal(info.Persen);
                item.PersenBath = Share.FormatDecimal(info.PersenBath);
                item.Balance = Share.FormatDecimal(info.Balance);
                item.Coupon = Share.FormatDecimal(info.Coupon);
                item.SumBalance = Share.FormatDecimal(info.SumBalance);
                item.Cash1 = info.Cash1;
                item.Cash1Bath = Share.FormatDecimal(info.Cash1Bath);
                item.Cash2 = info.Cash2;
                item.Cash2Bath = Share.FormatDecimal(info.Cash2Bath);
                item.Cash3 = info.Cash3;
                item.Cash3Bath = Share.FormatDecimal(info.Cash3Bath);
                item.Cash3Number = info.Cash3Number;
                item.MoneyPut = Share.FormatDecimal(info.MoneyPut);
                item.MoneyChange = Share.FormatDecimal(info.MoneyChange);
                // -------------------------------------------------------
                item.BranchId = branchid;
                item.CreateUser = User.Identity.Name;
                item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                item.UpdateUser = User.Identity.Name;
                item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                _db.SF_Payment.Add(item);
                _db.SaveChanges();

                //Svae Payment Sub
                foreach (var row in billsub)
                {
                    i++;

                    var itemSub = new SF_PaymentSub();
                    itemSub.PaymentId = DocRunning;
                    itemSub.i = i;
                    itemSub.FoodId = row.FoodId;
                    itemSub.Amount = row.Amount;
                    itemSub.Price = row.Price;
                    itemSub.BranchId = branchid;
                    _db.SF_PaymentSub.Add(itemSub);
                    _db.SaveChanges();
                }                              

                // Updata Status Bill = 2 ชำระเงินแล้ว
                var bill = _db.SF_Bill.FirstOrDefault(x => x.BillId == info.BillId && x.BranchId == branchid);
                bill.St = 2;
                _db.SF_Bill.Update(bill);
                _db.SaveChanges();

                // Set Runnig
                _Running.SetRunning("Payment", DocRunning, branchid);

                toastrAlert("ชำระเงิน", "เรียบร้อยแล้ว", Enums.NotificationToastr.success);
                return Json(new { data = "success" });
            }
            catch (Exception)
            {
                return Json(new { data = "error" });
            }
           
        }

        public IActionResult FrmPaymentView(string id)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var _GetMember = new GetMB_MemberController(_db);
            var _GetPayment = new GetSF_PaymentController(_db);
            var item = new ViewSF_Payment();
            var payment = _db.SF_Payment.FirstOrDefault(x => x.PaymentId == id && x.BranchId == branchid);
            item.PaymentId = payment.PaymentId;
            item.Dates = payment.Dates.ToString("dd/MM/yyyy");
            item.BillId = payment.BillId;
            item.TableId = payment.TableId;
            var table = _db.CD_Table.FirstOrDefault(x => x.TableId == payment.TableId && x.BranchId == branchid);
            item.TableName = table.TableName;
            item.MemberId = payment.MemberId;
            var member = _GetMember.ViewMemberById(payment.MemberId, branchid);
            item.MemberName = member.Name;
            item.MemberType = member.TypeName;

            item.SumBalance = Share.Cnumber(Share.FormatDouble(payment.SumBalance), 2);
            item.Total = Share.Cnumber(Share.FormatDouble(payment.Total), 2);
            item.ServicePersen = Share.Cnumber(Share.FormatDouble(payment.ServicePersen), 2);
            item.ServiceBath = Share.Cnumber(Share.FormatDouble(payment.ServiceBath), 2);
            item.MemberPersen = Share.Cnumber(Share.FormatDouble(payment.MemberPersen), 2);
            item.MemberBath = Share.Cnumber(Share.FormatDouble(payment.MemberBath), 2);
            item.Persen = Share.Cnumber(Share.FormatDouble(payment.Persen), 2);
            item.PersenBath = Share.Cnumber(Share.FormatDouble(payment.PersenBath), 2);
            item.Coupon = Share.Cnumber(Share.FormatDouble(payment.Coupon), 2);
            item.MoneyPut = Share.Cnumber(Share.FormatDouble(payment.MoneyPut), 2);
            item.MoneyChange = Share.Cnumber(Share.FormatDouble(payment.MoneyChange), 2);

            item.PaymentSub = _GetPayment.PaymentSubList(id, branchid);
            return View(item);
        }

        public IActionResult CancelPayment(string id)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            try
            {
                // Update Status Payment = 2 ยกเลิกเอกสาร
                var payment = _db.SF_Payment.FirstOrDefault(x => x.PaymentId == id && x.BranchId == branchid);
                payment.St = 2;
                _db.SF_Payment.Update(payment);
                _db.SaveChanges();

                // Update Status bill = 1 ปกติ
                var bill = _db.SF_Bill.FirstOrDefault(x => x.BillId == payment.BillId && x.BranchId == branchid);
                bill.St = 1;
                _db.SF_Bill.Update(bill);
                _db.SaveChanges();

                toastrAlert("ชำระเงิน", "ยกเลิกเรียบร้อยแล้ว", Enums.NotificationToastr.success);
                return RedirectToAction("FrmDailyPayment", "SF_Daily");
            }
            catch (Exception)
            {
                Alert("", "Error Data !", Enums.NotificationType.warning);
                return View();
            }            
        }

        public ViewFrmPayment LoadData(string id, string branchid)
        {
            var item = new ViewFrmPayment();

            var _Get = new GetSF_BillController(_db);
            var _bill = _Get.BillById(id, branchid);
            var _setting = _db.Setting.FirstOrDefault();

            item.BillId = _bill.BillId;
            item.TableId = _bill.TableId;
            item.TableName = _bill.TableName;
            // รายการอาหาร
            var billsub = _Get.BillSubById(id, branchid);
            decimal price = 0;
            foreach (var i in billsub)
            {
                if (i.Status != 4)
                {
                    price += i.Price * i.Amount;
                }
            }
            item.BillSub = billsub;
            // หาค่า ServiceCharge
            var B = price * _setting.Service / 100;
            item.ServicePersen = Share.Cnumber(Share.FormatDouble(_setting.Service), 2);
            item.ServiceBath = Share.Cnumber(Share.FormatDouble(B), 2);
            item.MemberPersen = "0.00";
            item.MemberBath = "0.00";
            item.Persen = "0.00";
            item.PersenBath = "0.00";
            var sum = price + B;
            item.SumBalance = Share.Cnumber(Share.FormatDouble(sum), 2);
            item.Balance = Share.Cnumber(Share.FormatDouble(price), 2);

            return item;
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
                //item.Total = Share.FormatDecimal(info.Total);
                //item.Persen = info.Persen;
                item.MemberId = info.MemberId;
                //item.Rebate = info.Rebate;
                //item.Score = info.Score;
                item.Balance = Share.FormatDecimal(info.Balance);
                //item.PayType = info.PayType;
                //item.MoneyPut = info.MoneyPut;
                //item.MoneyChange = info.MoneyChange;
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
                    //itemsub.TableId = row.TableId;
                    itemsub.FoodId = row.FoodId;
                    itemsub.Amount = row.Amount;
                    itemsub.Price = row.Price;
                    //itemsub.Status = row.Status;
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
            //item.Total = Total.ToString("N2");
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
            item.TypeName = member.TypeName;
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
