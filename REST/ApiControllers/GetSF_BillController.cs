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
    public class GetSF_BillController : BaseController
    {
        #region Connect db
        private readonly DbConnection _db;
        public GetSF_BillController(DbConnection db)
        {
            _db = db;
        }
        #endregion 

        public List<ViewSF_Bill> GetBill_ByDate(string Date, string branchid)
        {
            var List = new List<ViewSF_Bill>();
            var sql = $"SELECT BillId, Dates, TableName, SumBalance "
                    + $"FROM SF_Bill "
                    + $"LEFT JOIN CD_Table ON SF_Bill.TableId = CD_Table.TableId "
                    + $"WHERE SF_Bill.BranchId = '{branchid}' ";
            if (Date != null)
                sql += $"AND SF_Bill.Dates = '{Share.ConvertFieldDate(Date)}' ";

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewSF_Bill();
                        if (!data.IsDBNull(0))
                            Item.BillId = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.Dates = data.GetDateTime(1).ToString("dd/MM/yyyy");
                        if (!data.IsDBNull(2))
                            Item.TableName = data.GetString(2);
                        if (!data.IsDBNull(3))
                            Item.SumBalance = Share.Cnumber(Share.FormatDouble(data.GetDecimal(3)), 2);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }

        public List<ViewSF_BillSub> GetBillSub_ById(string id, string branchid)
        {
            var List = new List<ViewSF_BillSub>();
            var sql = $"SELECT BillId, i, SF_BillSub.FoodId, FoodName, Status, Amount, SF_BillSub.Price "
                    + $"FROM SF_BillSub "
                    + $"LEFT JOIN CD_Food ON SF_BillSub.FoodId = CD_Food.FoodId "
                    + $"WHERE SF_BillSub.BranchId = '{branchid}' AND BillId = '{id}' ORDER BY i ASC ";

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewSF_BillSub();
                        if (!data.IsDBNull(0))
                            Item.BillId = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.i = data.GetInt32(1);
                        if (!data.IsDBNull(2))
                            Item.FoodId = data.GetString(2);
                        if (!data.IsDBNull(3))
                            Item.FoodName = data.GetString(3);
                        if (!data.IsDBNull(4))
                            Item.Status = data.GetInt32(4);
                        if (!data.IsDBNull(5))
                            Item.Amount = data.GetDecimal(5);
                        if (!data.IsDBNull(6))
                            Item.Price = data.GetDecimal(6);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }

        public ViewSF_BillSub GetBillSub_ByFood(string id, int i, string FoodId, string branchid)
        {
            var Item = new ViewSF_BillSub();
            var sql = $"SELECT BillId, i, SF_BillSub.FoodId, FoodName, Status, Amount, SF_BillSub.Price "
                    + $"FROM SF_BillSub "
                    + $"LEFT JOIN CD_Food ON SF_BillSub.FoodId = CD_Food.FoodId "
                    + $"WHERE SF_BillSub.BranchId = '{branchid}' AND BillId = '{id}' AND i = '{i}' AND SF_BillSub.FoodId = '{FoodId}' ";

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {                        
                        if (!data.IsDBNull(0))
                            Item.BillId = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.i = data.GetInt32(1);
                        if (!data.IsDBNull(2))
                            Item.FoodId = data.GetString(2);
                        if (!data.IsDBNull(3))
                            Item.FoodName = data.GetString(3);
                        if (!data.IsDBNull(4))
                            Item.Status = data.GetInt32(4);
                        if (!data.IsDBNull(5))
                            Item.Amount = data.GetDecimal(5);
                        if (!data.IsDBNull(6))
                            Item.Price = data.GetDecimal(6);
                    }
                }
            }

            return Item;
        }

        public ViewSF_Bill BillById(string id, string branchid)
        {
            var Item = new ViewSF_Bill();
            var sql = $"SELECT BillId, SF_Bill.TableId, TableName, PriceTotal, BillDt "
                    + $"FROM SF_Bill "
                    + $"LEFT JOIN CD_Table ON SF_Bill.TableId = CD_Table.TableId "
                    + $"WHERE SF_Bill.BranchId = '{branchid}' ";
            if (id != null)
                sql += $"AND BillId = '{id}' ";

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {                        
                        if (!data.IsDBNull(0))
                            Item.BillId = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.TableId = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.TableName = data.GetString(2);
                        if (!data.IsDBNull(3))
                            Item.PriceTotal = data.GetDecimal(3);
                        if (!data.IsDBNull(4))
                            Item.Dates = data.GetDateTime(4).ToString("dd/MM/yyyy");
                    }
                }
            }

            return Item;
        }        

        public List<ViewSF_Bill> Bill(string id, string Dt, string branchid)
        {
            var List = new List<ViewSF_Bill>();
            var sql = $"SELECT BillId, SF_Bill.TableId, TableName, PriceTotal, BillDt "
                    + $"FROM SF_Bill "
                    + $"LEFT JOIN CD_Table ON SF_Bill.TableId = CD_Table.TableId "
                    + $"WHERE SF_Bill.BranchId = '{branchid}' AND SF_Bill.St = '1' ";
                    if (id != null)
                        sql += $"AND SF_Bill.BillId = '{id}' ";
                    if (Dt != null)
                        sql += $"AND SF_Bill.BillDt = '{Share.ConvertFieldDate(Dt)}' ";
                    sql += $"ORDER BY BillId DESC, BillDt DESC";

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewSF_Bill();
                        if (!data.IsDBNull(0))
                            Item.BillId = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.TableId = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.TableName = data.GetString(2);
                        if (!data.IsDBNull(3))
                            Item.PriceTotal = data.GetDecimal(3);
                        if (!data.IsDBNull(4))
                            Item.Dates = data.GetDateTime(4).ToString("dd/MM/yyyy");                        
                        List.Add(Item);
                    }
                }
            }

            return List;
        }

        public List<ViewSF_BillSub> BillSubById(string id, string branchid)
        {
            var List = new List<ViewSF_BillSub>();
            var sql = $"SELECT B.BillId, B.i, B.TableId, T.TableName, B.FoodId, F.FoodName, B.Amount, B.Price "
                    + $"FROM SF_BillSub AS B "
                    + $"LEFT JOIN CD_Table AS T ON B.TableId = T.TableId "
                    + $"LEFT JOIN CD_Food AS F ON B.FoodId = F.FoodId "
                    + $"WHERE B.BranchId = '{branchid}' ";
            if (id != null)
                sql += $"AND B.BillId = '{id}' ";

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewSF_BillSub();
                        if (!data.IsDBNull(0))
                            Item.BillId = data.GetString(0);
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


        // -------------------------------------------
        // -------------------------------------------
        // ---                                     ---
        // ---             ACTION BILL             ---
        // ---                                     ---
        // -------------------------------------------
        // -------------------------------------------

        public JsonResult ABill(string OrderDt = null)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var List = GetBill_ByDate(OrderDt, branchid);
            return Json(List);
        }

    }
}
