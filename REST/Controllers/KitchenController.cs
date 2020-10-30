using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST.Data;
using REST.Models;
using REST.Service;
using REST.ViewModels;

namespace REST.Controllers
{
    [Authorize]
    public class KitchenController : BaseController
    {
        #region db
        private readonly DbConnection _db;

        public KitchenController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        // ------

        public IActionResult Index()
        {
            ViewBag.ST_A = OrderST_A();
            ViewBag.ST_0 = OrderST_0();
            ViewBag.ST_1 = OrderST_1();
            return View();
        }

        public IActionResult ViewOrder(string id)
        {
            ViewBag.id = id;
            return View();
        }

        // ------

        public IActionResult SaveOrder(string id)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;

            var Item = new SF_Order();
            Item = _db.SF_Order.FirstOrDefault(x => x.OrderId == id && x.BranchId == BranchId);

            if (Item.ST == 1)
            {
                Alert("question", "สถานะอาหารเสร็จแล้ว !", Enums.NotificationType.question);
            }
            else
            {
                var List = OrderSub(id);

                foreach( var row in List)
                {
                    row.Status = 1;

                    _db.SF_OrderSub.Update(row);
                    _db.SaveChanges();
                }

                var DT = new DataTable();
                var sql = $"SELECT OrderId, i, TableId, FoodId, Amount, Price, Status, BranchId "
                    + $"FROM SF_OrderSub "
                    + $"WHERE OrderId = '{id}' AND Status = 1 AND BranchId = '{BranchId}' ";

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
                    var Order = new SF_Order();
                    Order = _db.SF_Order.FirstOrDefault(x => x.OrderId == id && x.BranchId == BranchId);

                    Order.ST = 1;

                    _db.SF_Order.Update(Order);
                    _db.SaveChanges();
                }

                AlertTop("บันทึกสถานะเสร็จเรียบร้อย !", Enums.NotificationType.success);
            }

            return RedirectToAction("Index");
        }

        // ------

        [HttpPost]
        public IActionResult TB_Order(DateTime DATE_Order)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var List = new List<SF_Order>();

            var sql = $"SELECT OrderId, TableId, PriceTotal, ST "
                    + $"FROM SF_Order "
                    + $"WHERE BranchId = '{BranchId}' ";
            if (DATE_Order != null)
                sql += $"AND OrderDt = '{Share.FormatDateTime_Q(DATE_Order)}'";

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new SF_Order();
                        if(!data.IsDBNull(0))
                            Item.OrderId = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.TableId = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.PriceTotal = data.GetDecimal(2);
                        if (!data.IsDBNull(3))
                            Item.ST = data.GetInt32(3);
                        List.Add(Item);
                    }
                }
            }

            return Json(new { data = List });
        }

        [HttpPost]
        public IActionResult TB_OrderSub(string OrderId)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var List = new List<ViewOrder>();

            var sql = $"SELECT SF_OrderSub.FoodId, FoodName, Amount, SF_OrderSub.Price, Status "
                    + $"FROM SF_OrderSub "
                    + $"LEFT JOIN CD_Food ON SF_OrderSub.FoodId = CD_Food.FoodId "
                    + $"WHERE OrderId = '{OrderId}' AND SF_OrderSub.BranchId = '{BranchId}' ";

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewOrder();
                        if (!data.IsDBNull(0))
                            Item.FoodId = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.FoodName = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.Amount = data.GetDecimal(2);
                        if (!data.IsDBNull(3))
                            Item.Price = data.GetDecimal(3);
                        if (!data.IsDBNull(4))
                            Item.Status = data.GetInt32(4);
                        List.Add(Item);
                    }
                }
            }

            return Json(new { data = List });
        }

        public List<SF_OrderSub>OrderSub(string OrderId)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var List = new List<SF_OrderSub>();

            var sql = $"SELECT OrderId, i, TableId, FoodId, Amount, Price, Status, BranchId "
                    + $"FROM SF_OrderSub "
                    + $"WHERE OrderId = '{OrderId}' AND BranchId = '{BranchId}' ";

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new SF_OrderSub();
                        if (!data.IsDBNull(0))
                            Item.OrderId = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.i = data.GetInt32(1);
                        if (!data.IsDBNull(2))
                            Item.TableId = data.GetString(2);
                        if (!data.IsDBNull(3))
                            Item.FoodId = data.GetString(3);
                        if (!data.IsDBNull(4))
                            Item.Amount = data.GetDecimal(4);
                        if (!data.IsDBNull(5))
                            Item.Price = data.GetDecimal(5);
                        if (!data.IsDBNull(6))
                            Item.Status = data.GetInt32(6);
                        if (!data.IsDBNull(7))
                            Item.BranchId = data.GetString(7);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }

        // ------

        public string OrderST_A()
        {
            string Count = null;

            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var sql = $"SELECT COUNT(*) AS OrderCount "
                    + $"FROM SF_Order "
                    + $"WHERE BranchId = '{BranchId}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        Count = data["OrderCount"].ToString();
                    }
                }
            }

            return Count;
        }
        public string OrderST_0()
        {
            string Count = null;

            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var sql = $"SELECT COUNT(*) AS OrderCount "
                    + $"FROM SF_Order "
                    + $"WHERE ST = '0' AND BranchId = '{BranchId}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        Count = data["OrderCount"].ToString();
                    }
                }
            }

            return Count;
        }
        public string OrderST_1()
        {
            string Count = null;

            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var sql = $"SELECT COUNT(*) AS OrderCount "
                    + $"FROM SF_Order "
                    + $"WHERE ST = '1' AND BranchId = '{BranchId}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        Count = data["OrderCount"].ToString();
                    }
                }
            }

            return Count;
        }

    }
}
