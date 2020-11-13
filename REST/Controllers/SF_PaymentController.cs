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

        public IActionResult FrmPayment(string TableId = null)
        {
            var branch = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var _ordersub = new GetSF_OrderController(_db);
            var item = new ViewFrmPayment();
            if(TableId != null)
            {
                var order = _db.SF_OrderSub.Where(x => x.TableId == TableId && x.Status == 1 || x.Status == 2 && x.BranchId == branch).ToList();
                if(order.Count > 0)
                {
                    item.TableId = TableId;
                    Alert("", "รายการอาหาร ยังทำไม่เสร็จหรือได้ไม่ครบ", Enums.NotificationType.warning);
                    return View(item);
                }
                else
                {
                    var info = _ordersub.OrderSubByTableId(TableId, branch);
                    decimal price = 0;
                    foreach (var row in info)
                    {
                        price += row.Price * row.Amount;
                    }
                    // input pricer ordersub
                    item.TableId = TableId;
                    item.TableName = TableId;
                    item.Total = Share.Cnumber(Share.FormatDouble(price), 2);
                    item.OrderSub = _ordersub.OrderSub(TableId, null, branch);

                    return View(item);
                }                
            }
            else
            {
                return View(item);
            }            
        }

        public IActionResult Payment1(string id)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;            
            var _Get1 = new GetCD_TableController(_db);
            var _Get2 = new GetSF_OrderController(_db);
            ViewBag.Table = _Get1.TableById(id, branchid).FirstOrDefault();
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
            var item = _Get.ViewMemberById(id, branchid);
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
