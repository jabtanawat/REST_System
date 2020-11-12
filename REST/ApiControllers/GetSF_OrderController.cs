using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST.Controllers;
using REST.Data;
using REST.Models;
using REST.Service;
using REST.ViewModels;

namespace REST.ApiControllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GetSF_OrderController : BaseController
    {
        #region Connect db
        private readonly DbConnection _db;
        public GetSF_OrderController(DbConnection db)
        {
            _db = db;
        }
        #endregion 

        public ViewTb_SF_Order OrderById(string OrderId, string branchid)
        {
            var Item = new ViewTb_SF_Order();
            var sql = $"SELECT OrderId, SF_Order.TableId, TableName, ST, PriceTotal, OrderDt "
                    + $"FROM SF_Order "
                    + $"LEFT JOIN CD_Table ON SF_Order.TableId = CD_Table.TableId "
                    + $"WHERE SF_Order.BranchId = '{branchid}' ";
            if (OrderId != null)
                sql += $"AND OrderId = '{OrderId}' ";

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        
                        if (!data.IsDBNull(0))
                            Item.OrderId = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.TableId = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.TableName = data.GetString(2);
                        if (!data.IsDBNull(3))
                            Item.ST = data.GetInt32(3);
                        if (!data.IsDBNull(4))
                            Item.PriceTotal = data.GetDecimal(4);
                        if (!data.IsDBNull(5))
                            Item.Dates = data.GetDateTime(5).ToString("dd/MM/yyyy");
                    }
                }
            }

            return Item;
        }        

        public List<ViewTb_SF_Order> Order(string TableId, string OrderDt, string branchid)
        {
            var List = new List<ViewTb_SF_Order>();
            var sql = $"SELECT OrderId, SF_Order.TableId, TableName, ST, PriceTotal, OrderDt "
                    + $"FROM SF_Order "
                    + $"LEFT JOIN CD_Table ON SF_Order.TableId = CD_Table.TableId "
                    + $"WHERE SF_Order.BranchId = '{branchid}' ";
                    if (TableId != null)
                        sql += $"AND SF_Order.TableId = '{TableId}' ";
                    if (OrderDt != null)
                        sql += $"AND SF_Order.OrderDt = '{Share.ConvertFieldDate(OrderDt)}' ";
                    sql += $"ORDER BY OrderId DESC, OrderDt DESC";

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewTb_SF_Order();
                        if (!data.IsDBNull(0))
                            Item.OrderId = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.TableId = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.TableName = data.GetString(2);
                        if (!data.IsDBNull(3))
                            Item.ST = data.GetInt32(3);
                        if (!data.IsDBNull(4))
                            Item.PriceTotal = data.GetDecimal(4);
                        if (!data.IsDBNull(5))
                            Item.Dates = data.GetDateTime(5).ToString("dd/MM/yyyy");                        
                        List.Add(Item);
                    }
                }
            }

            return List;
        }

        public List<ViewSF_OrderSub> OrderSubByTableId(string TableId, string branchid)
        {
            var List = new List<ViewSF_OrderSub>();
            var sql = $"SELECT o.OrderId, o.i, o.TableId, t.TableName, o.FoodId, f.FoodName, o.Amount, o.Price, o.Status "
                    + $"FROM SF_OrderSub AS o "
                    + $"LEFT JOIN CD_Table AS t ON o.TableId = t.TableId "
                    + $"LEFT JOIN CD_Food AS f ON o.FoodId = f.FoodId "
                    + $"WHERE o.BranchId = '{branchid}' ";
            if (TableId != null)
                sql += $"AND o.TableId = '{TableId}' AND o.Status != 4";

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewSF_OrderSub();
                        if (!data.IsDBNull(0))
                            Item.OrderId = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.i = data.GetInt32(1);
                        if (!data.IsDBNull(2))
                            Item.TableId = data.GetString(2);
                        if (!data.IsDBNull(3))
                            Item.TableName = data.GetString(3);
                        if (!data.IsDBNull(4))
                            Item.FoodId = data.GetString(4);
                        if (!data.IsDBNull(5))
                            Item.FoodName = data.GetString(5);
                        if (!data.IsDBNull(6))
                            Item.Amount = data.GetDecimal(6);
                        if (!data.IsDBNull(7))
                            Item.Price = data.GetDecimal(7);
                        if (!data.IsDBNull(8))
                            Item.Status = data.GetInt32(8);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }

        public List<ViewSF_OrderSub> OrderSub(string TableId, string OrderId, string branchid)
        {
            var List = new List<ViewSF_OrderSub>();
            var sql = $"SELECT o.OrderId, o.i, o.TableId, o.FoodId, f.FoodName, o.Amount, o.Price, o.Status "
                    + $"FROM SF_OrderSub AS o "
                    + $"LEFT JOIN CD_Food AS f ON o.FoodId = f.FoodId "
                    + $"WHERE o.BranchId = '{branchid}' ";
            if (TableId != null)
                sql += $"AND o.TableId = '{TableId}'";
            if (OrderId != null)
                sql += $"AND o.OrderId = '{OrderId}'";

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewSF_OrderSub();
                        if (!data.IsDBNull(0))
                            Item.OrderId = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.i = data.GetInt32(1);
                        if (!data.IsDBNull(2))
                            Item.TableId = data.GetString(2);
                        if (!data.IsDBNull(3))
                            Item.FoodId = data.GetString(3);
                        if (!data.IsDBNull(4))
                            Item.FoodName = data.GetString(4);
                        if (!data.IsDBNull(5))
                            Item.Amount = data.GetDecimal(5);
                        if (!data.IsDBNull(6))
                            Item.Price = data.GetDecimal(6);
                        if (!data.IsDBNull(7))
                            Item.Status = data.GetInt32(7);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }


        //----------- ACTION -----------

        public JsonResult AOrder(string OrderDt)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var List = Order(null, OrderDt, branchid);
            return Json(List);
        }

    }
}
