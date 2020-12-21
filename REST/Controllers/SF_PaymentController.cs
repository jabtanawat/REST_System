using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    public class SF_PaymentController : BaseController
    {
        #region Connection db
        private readonly DbConnection _db;

        public SF_PaymentController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        public IActionResult FrmBill(string id)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var item = new ViewFrmPayment();

            item = LoadOrder(id, branchid);

            return View(item);
        }

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
        public IActionResult FrmBill(ViewFrmPayment info)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var item = new SF_Bill();
            int i = 0;
            try
            {
                // เลข running
                var _Running = new GetRunningController(_db);
                string DocRunning = _Running.Running("Bill", branchid);                

                // รายการอาหาร
                var _GetOrder = new GetSF_OrderController(_db);
                var _order = _GetOrder.OrderSubTable(info.TableId, null, branchid);

                // Save Bill
                item.BillId = DocRunning;
                item.TableId = info.TableId;
                item.MemberId = info.MemberId;
                item.St = 1;
                item.Dates = Share.FormatDate(DateTime.Now).Date;                
                item.SumBalance = Share.FormatDecimal(info.SumBalance);
                item.Balance = Share.FormatDecimal(info.Balance);
                item.ServicePersen = Share.FormatDecimal(info.ServicePersen);
                item.ServiceBath = Share.FormatDecimal(info.ServiceBath);
                item.MemberPersen = Share.FormatDecimal(info.MemberPersen);
                item.MemberBath = Share.FormatDecimal(info.MemberBath);
                item.Persen = Share.FormatDecimal(info.Persen);
                item.PersenBath = Share.FormatDecimal(info.PersenBath);
                item.VatPersen = Share.FormatDecimal(info.VatPersen);
                item.VatBath = Share.FormatDecimal(info.VatBath);
                item.VatPersen = Share.FormatDecimal(info.VatPersen);
                item.VatBath = Share.FormatDecimal(info.VatBath);
                item.BeforeVat = Share.FormatDecimal(info.BeforeVat);
                item.AfterVat = Share.FormatDecimal(info.AfterVat);
                // -------------------------------------------------------
                item.BranchId = branchid;
                item.CreateUser = User.Identity.Name;
                item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                item.UpdateUser = User.Identity.Name;
                item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                _db.SF_Bill.Add(item);
                _db.SaveChanges();

                

                //Svae Payment Sub
                foreach (var row in _order)
                {
                    i++;

                    decimal Total = row.Amount * row.Price;

                    var itemSub = new SF_BillSub();
                    itemSub.BillId = DocRunning;
                    itemSub.i = i;
                    itemSub.FoodId = row.FoodId;
                    itemSub.Status = 1;
                    itemSub.Amount = row.Amount;
                    itemSub.Price = row.Price;
                    itemSub.Total = Total;
                    itemSub.BranchId = branchid;
                    _db.SF_BillSub.Add(itemSub);
                    _db.SaveChanges();
                }

                // Set Runnig
                _Running.SetRunning("Bill", DocRunning, branchid);

                // Updata Status Table = 1 ว่าง
                var table = _db.CD_Table.FirstOrDefault(x => x.TableId == info.TableId && x.BranchId == branchid);
                table.TableST = 1;
                _db.CD_Table.Update(table);
                _db.SaveChanges();

                // Update Success Order = 2 เช็คบิลออเดอร์เรียบร้อยแล้ว
                var ordersub = _GetOrder.OrderSub(info.TableId, null, "1", branchid);                
                foreach (var row in ordersub)
                {
                    var itemSub = _db.SF_Order.FirstOrDefault(x => x.OrderId == row.OrderId && x.BranchId == branchid);
                    itemSub.Success = 2;
                    _db.SF_Order.Update(itemSub);
                    _db.SaveChanges();
                }

                toastrAlert("เช็คบิล", "เรียบร้อยแล้ว", Enums.NotificationToastr.success);
                return RedirectToAction("Index", "StoreFront");
            }
            catch (Exception)
            {
                return View(info);
            }

        }

        [HttpPost]
        public IActionResult EditBill(ViewFrmPayment info)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            string Status = "success";
            var item = new SF_Bill();
            dynamic Sub = JsonConvert.DeserializeObject(info.Sub);
            int i = 0;
            try
            {               

                item = _db.SF_Bill.FirstOrDefault(x => x.BillId == info.BillId && x.BranchId == branchid);

                if (DateTime.Now.Date > item.Dates)
                {
                    return Json(new { data = "errorDate" });
                }

                // Save Bill
                item.MemberId = info.MemberId;
                item.St = 1;
                item.Dates = Share.FormatDate(DateTime.Now).Date;
                item.SumBalance = Share.FormatDecimal(info.SumBalance);
                item.Balance = Share.FormatDecimal(info.Balance);
                item.ServicePersen = Share.FormatDecimal(info.ServicePersen);
                item.ServiceBath = Share.FormatDecimal(info.ServiceBath);
                item.MemberPersen = Share.FormatDecimal(info.MemberPersen);
                item.MemberBath = Share.FormatDecimal(info.MemberBath);
                item.Persen = Share.FormatDecimal(info.Persen);
                item.PersenBath = Share.FormatDecimal(info.PersenBath);
                item.VatPersen = Share.FormatDecimal(info.VatPersen);
                item.VatBath = Share.FormatDecimal(info.VatBath);
                item.BeforeVat = Share.FormatDecimal(info.BeforeVat);
                item.AfterVat = Share.FormatDecimal(info.AfterVat);
                // -------------------------------------------------------
                item.BranchId = branchid;
                item.CreateUser = User.Identity.Name;
                item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                item.UpdateUser = User.Identity.Name;
                item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                _db.SF_Bill.Update(item);
                _db.SaveChanges();


                // Delete Data BillSub And Save BillSub
                var delete = _db.SF_BillSub.Where(x => x.BillId == info.BillId && x.BranchId == branchid).ToList();
                _db.SF_BillSub.RemoveRange(delete);
                _db.SaveChanges();

                //Svae Payment Sub
                foreach (dynamic result in Sub)
                {                   
                    i++;

                    string FoodId = result.FoodId;
                    decimal Amount = result.Amount;
                    decimal Price = result.Price;
                    decimal Total = result.Total;

                    var itemSub = new SF_BillSub();
                    itemSub.BillId = info.BillId;
                    itemSub.i = i;
                    itemSub.FoodId = FoodId;
                    itemSub.Status = 1;
                    itemSub.Amount = Amount;
                    itemSub.Price = Price;
                    itemSub.Total = Total;
                    itemSub.BranchId = branchid;
                    _db.SF_BillSub.Add(itemSub);
                    _db.SaveChanges();
                }

                toastrAlert("เช็คบิล", "แก้ไขบิลเรียบร้อยแล้ว", Enums.NotificationToastr.success);
                Status = "success";
            }
            catch (Exception)
            {
                Status = "error";
            }

            return Json(new { data = Status });
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
                
                var bill = _db.SF_Bill.FirstOrDefault(x => x.BillId == info.BillId && x.BranchId == branchid);
                // ตรวจสอบวันที่ ว่าวันเดียวกันไหม
                if (DateTime.Now.Date != bill.Dates)
                {
                    return Json(new { data = "errorDate" });
                }
                // ตรวจสอบว่าบิล มีการแก้ไขยอดชำระหรือไหม เอายอดเงินมาเปรียบเทียบกัน
                if (bill.SumBalance != Share.FormatDecimal(info.Balance))
                {
                    return Json(new { data = "errorSumBalance" });
                }
                // รายการอาหาร
                var billsub = _db.SF_BillSub.Where(x => x.BillId == info.BillId && x.BranchId == branchid).ToList();
                // เลข running
                var _Running = new GetRunningController(_db);
                string DocRunning = _Running.Running("Payment", branchid);

                // Save Payment
                item.PaymentId = DocRunning;
                item.BillId = info.BillId;
                item.TableId = bill.TableId;
                item.MemberId = bill.MemberId;
                item.St = 1;
                item.Dates = Share.FormatDate(DateTime.Now).Date;
                item.SumBalance = Share.FormatDecimal(bill.SumBalance);
                item.Balance = Share.FormatDecimal(bill.Balance);                
                item.ServicePersen = Share.FormatDecimal(bill.ServicePersen);
                item.ServiceBath = Share.FormatDecimal(bill.ServiceBath);
                item.MemberPersen = Share.FormatDecimal(bill.MemberPersen);
                item.MemberBath = Share.FormatDecimal(bill.MemberBath);
                item.Persen = Share.FormatDecimal(bill.Persen);
                item.PersenBath = Share.FormatDecimal(bill.PersenBath);
                item.VatPersen = Share.FormatDecimal(bill.VatPersen);
                item.VatBath = Share.FormatDecimal(bill.VatBath);
                item.BeforeVat = Share.FormatDecimal(bill.BeforeVat);
                item.AfterVat = Share.FormatDecimal(bill.AfterVat);
                item.Coupon = Share.FormatDecimal(info.Coupon);
                item.Total = Share.FormatDecimal(info.Total);
                item.Cash1 = info.Cash1;
                item.Cash1Bath = Share.FormatDecimal(info.Cash1Bath);
                item.Cash2 = info.Cash2;
                item.Cash2Bath = Share.FormatDecimal(info.Cash2Bath);
                item.Cash3 = info.Cash3;
                item.Cash3Bath = Share.FormatDecimal(info.Cash3Bath);
                item.Cash3Type = info.Cash3Type;
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
                    itemSub.Amount = Share.FormatDecimal(row.Amount);
                    itemSub.Price = Share.FormatDecimal(row.Price);
                    itemSub.Total = Share.FormatDecimal(row.Total);
                    itemSub.BranchId = branchid;
                    _db.SF_PaymentSub.Add(itemSub);
                    _db.SaveChanges();
                }                              

                // Updata Status Bill = 2 ชำระเงินแล้ว
                var bill1 = _db.SF_Bill.FirstOrDefault(x => x.BillId == info.BillId && x.BranchId == branchid);
                bill1.St = 2;
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
            //item.Total = Share.Cnumber(Share.FormatDouble(payment.Total), 2);
            item.ServicePersen = Share.Cnumber(Share.FormatDouble(payment.ServicePersen), 2);
            item.ServiceBath = Share.Cnumber(Share.FormatDouble(payment.ServiceBath), 2);
            item.MemberPersen = Share.Cnumber(Share.FormatDouble(payment.MemberPersen), 2);
            item.MemberBath = Share.Cnumber(Share.FormatDouble(payment.MemberBath), 2);
            item.Persen = Share.Cnumber(Share.FormatDouble(payment.Persen), 2);
            item.PersenBath = Share.Cnumber(Share.FormatDouble(payment.PersenBath), 2);
            item.BeforeVat = Share.Cnumber(Share.FormatDouble(payment.BeforeVat), 2);
            item.VatPersen = Share.Cnumber(Share.FormatDouble(payment.VatPersen), 2);
            item.VatBath = Share.Cnumber(Share.FormatDouble(payment.VatBath), 2);
            item.AfterVat = Share.Cnumber(Share.FormatDouble(payment.AfterVat), 2);
            item.Coupon = Share.Cnumber(Share.FormatDouble(payment.Coupon), 2);
            item.Total = Share.Cnumber(Share.FormatDouble(payment.Total), 2);
            item.MoneyPut = Share.Cnumber(Share.FormatDouble(payment.MoneyPut), 2);
            item.MoneyChange = Share.Cnumber(Share.FormatDouble(payment.MoneyChange), 2);

            item.Cash1 = payment.Cash1;
            item.Cash1Bath = Share.Cnumber(Share.FormatDouble(payment.Cash1Bath), 2);
            item.Cash2 = payment.Cash2;
            item.Cash2Bath = Share.Cnumber(Share.FormatDouble(payment.Cash2Bath), 2);
            item.Cash3 = payment.Cash3;
            item.Cash3Bath = Share.Cnumber(Share.FormatDouble(payment.Cash3Bath), 2);
            item.Cash3Type = payment.Cash3Type;
            item.Cash3Number = payment.Cash3Number;

            item.PaymentSub = _GetPayment.PaymentSubList(id, branchid);
            return View(item);
        }

        public IActionResult CancelPayment(string id)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            try
            {
                
                var payment = _db.SF_Payment.FirstOrDefault(x => x.PaymentId == id && x.BranchId == branchid);
                // ตรวจสอบวัน ว่าวันเดียวกันไหม
                if (DateTime.Now.Date != payment.Dates)
                {
                    Alert("", "วันที่ไม่ตรงกัน !", Enums.NotificationType.warning);
                    return View();
                }
                // Update Status Payment = 2 ยกเลิกเอกสาร
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

        public ViewFrmPayment LoadOrder(string id, string branchid)
        {
            var item = new ViewFrmPayment();
            // รายการอาหาร
            var _GetOrder = new GetSF_OrderController(_db);            
            var _order = _GetOrder.OrderSubTable(id, null, branchid);
            
            var _setting = _db.Setting.FirstOrDefault();

            // ข้อมูลโต๊ะ
            var _Table = new GetCD_TableController(_db);
            var table = _Table.TableById(id, branchid);

            // Setting
            var _get = new GetRunningController(_db);
            string DocRunning = _get.Running("Bill", branchid);

            item.BillId = DocRunning;
            item.TableId = table.TableId;
            item.TableName = table.TableName;

            decimal price = 0;
            foreach (var i in _order)
            {
                if (i.Status != 4)
                {
                    price += i.Price * i.Amount;
                }
            }
            // หาค่า ServiceCharge
            var B = price * _setting.Service / 100;
            // รวม
            var sum = price + B;
            item.OrderSub = _order;
            // หาค่า Vat
            var V = sum * 7 / 107;
            
            item.ServicePersen = Share.Cnumber(Share.FormatDouble(_setting.Service), 2);
            item.ServiceBath = Share.Cnumber(Share.FormatDouble(B), 2);
            item.MemberPersen = "0.00";
            item.MemberBath = "0.00";
            item.Persen = "0.00";
            item.PersenBath = "0.00";
            item.BeforeVat = Share.Cnumber(Share.FormatDouble(sum - V), 2);
            item.VatPersen = Share.Cnumber(Share.FormatDouble(7), 2);
            item.VatBath = Share.Cnumber(Share.FormatDouble(V), 2);
            item.AfterVat = Share.Cnumber(Share.FormatDouble(sum), 2);

            item.SumBalance = Share.Cnumber(Share.FormatDouble(sum), 2);
            item.Balance = Share.Cnumber(Share.FormatDouble(price), 2);

            return item;
        }

        public ViewFrmPayment LoadData(string id, string branchid)
        {
            var item = new ViewFrmPayment();

            //var _Get = new GetSF_BillController(_db);
            //var _bill = _Get.BillById(id, branchid);
            //var _setting = _db.Setting.FirstOrDefault();

            //item.BillId = _bill.BillId;
            //item.TableId = _bill.TableId;
            //item.TableName = _bill.TableName;
            //// รายการอาหาร
            //var billsub = _Get.BillSubById(id, branchid);
            //decimal price = 0;
            //foreach (var i in billsub)
            //{
            //    if (i.Status != 4)
            //    {
            //        price += i.Price * i.Amount;
            //    }
            //}
            //item.BillSub = billsub;
            //// หาค่า ServiceCharge
            //var B = price * _setting.Service / 100;
            //item.ServicePersen = Share.Cnumber(Share.FormatDouble(_setting.Service), 2);
            //item.ServiceBath = Share.Cnumber(Share.FormatDouble(B), 2);
            //item.MemberPersen = "0.00";
            //item.MemberBath = "0.00";
            //item.Persen = "0.00";
            //item.PersenBath = "0.00";
            //var sum = price + B;
            //item.SumBalance = Share.Cnumber(Share.FormatDouble(sum), 2);
            //item.Balance = Share.Cnumber(Share.FormatDouble(price), 2);

            var Bill = _db.SF_Bill.FirstOrDefault(x => x.BillId == id && x.BranchId == branchid);

            item.BillId = Bill.BillId;
            item.TableId = Bill.TableId;

            var Table = _db.CD_Table.FirstOrDefault(x => x.TableId == Bill.TableId && x.BranchId == branchid);
            item.TableName = Table.TableName;

            if(Bill.MemberId != null)
            {
                var Member = _db.MB_Member.FirstOrDefault(x => x.MemberId == Bill.MemberId && x.BranchId == branchid);
                item.MemberId = Bill.MemberId;
                item.MemberName = Member.Title + " " + Member.FirstName + " " + Member.LastName;
                if(Member.Type == 1)
                {
                    item.MemberType = "ทั่วไป";
                }
                else
                {
                    item.MemberType = "สมาชิก";
                }                
            }

            item.Balance = Share.Cnumber(Share.FormatDouble(Bill.Balance), 2);
            item.ServicePersen = Share.Cnumber(Share.FormatDouble(Bill.ServicePersen), 2);
            item.ServiceBath = Share.Cnumber(Share.FormatDouble(Bill.ServiceBath), 2);
            item.MemberPersen = Share.Cnumber(Share.FormatDouble(Bill.MemberPersen), 2);
            item.MemberBath = Share.Cnumber(Share.FormatDouble(Bill.MemberBath), 2);
            item.Persen = Share.Cnumber(Share.FormatDouble(Bill.Persen), 2);
            item.PersenBath = Share.Cnumber(Share.FormatDouble(Bill.PersenBath), 2);
            item.VatPersen = Share.Cnumber(Share.FormatDouble(Bill.VatPersen), 2);
            item.VatBath = Share.Cnumber(Share.FormatDouble(Bill.VatBath), 2);
            item.BeforeVat = Share.Cnumber(Share.FormatDouble(Bill.BeforeVat), 2);
            item.AfterVat = Share.Cnumber(Share.FormatDouble(Bill.AfterVat), 2);
            item.SumBalance = Share.Cnumber(Share.FormatDouble(Bill.SumBalance), 2);

            var _Get = new GetSF_BillController(_db);
            var _Bill = _Get.GetBillSub_ById(id, branchid);
            item.BillSub = _Bill;

            return item;
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
