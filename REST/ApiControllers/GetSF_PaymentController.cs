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
    public class GetSF_PaymentController : BaseController
    {
        #region Connect db
        private readonly DbConnection _db;
        public GetSF_PaymentController(DbConnection db)
        {
            _db = db;
        }
        #endregion 

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

        public List<ViewSF_Payment> PaymentList(string id, string Dt, string branchid)
        {
            var List = new List<ViewSF_Payment>();
            var sql = $"SELECT PaymentId, Dates, BillId, TableName, SumBalance, St "
                    + $"FROM SF_Payment "
                    + $"LEFT JOIN CD_Table ON SF_Payment.TableId = CD_Table.TableId "
                    + $"WHERE SF_Payment.BranchId = '{branchid}' ";
                    if (id != null)
                        sql += $"AND SF_Payment.PaymentId = '{id}' ";
                    if (Dt != null)
                        sql += $"AND SF_Payment.Dates = '{Share.ConvertFieldDate(Dt)}' ";
                    sql += $"ORDER BY PaymentId DESC, Dates DESC";

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewSF_Payment();
                        if (!data.IsDBNull(0))
                            Item.PaymentId = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.Dates = data.GetDateTime(1).ToString("dd/MM/yyyy");
                        if (!data.IsDBNull(2))
                            Item.BillId = data.GetString(2);
                        if (!data.IsDBNull(3))
                            Item.TableName = data.GetString(3);
                        if (!data.IsDBNull(4))
                            Item.SumBalance = Share.Cnumber(Share.FormatDouble(data.GetDecimal(4)), 2);
                        if (!data.IsDBNull(5))
                            Item.St = data.GetInt32(5);                       
                        List.Add(Item);
                    }
                }
            }

            return List;
        }

        public List<ViewSF_PaymentSub> PaymentSubList(string id, string branchid)
        {
            var List = new List<ViewSF_PaymentSub>();
            var sql = $"SELECT PaymentId, i, P.FoodId, F.FoodName, Amount, P.Price "
                    + $"FROM SF_PaymentSub AS P "
                    + $"LEFT JOIN CD_Food AS F ON P.FoodId = F.FoodId "
                    + $"WHERE P.BranchId = '{branchid}' ";
                    if (id != null)
                        sql += $"AND PaymentId = '{id}' ";
                    sql += $"ORDER BY i DESC";

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewSF_PaymentSub();
                        if (!data.IsDBNull(0))
                            Item.PaymentId = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.i = data.GetInt32(1);
                        if (!data.IsDBNull(2))
                            Item.FoodId = data.GetString(2);
                        if (!data.IsDBNull(3))
                            Item.FoodName = data.GetString(3);
                        if (!data.IsDBNull(4))
                            Item.Amount = data.GetDecimal(4);
                        if (!data.IsDBNull(5))
                            Item.Price = data.GetDecimal(5);
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
        // ---            ACTION PAYMENT           ---
        // ---                                     ---
        // -------------------------------------------
        // -------------------------------------------

        public JsonResult APayment(string OrderDt = null)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var List = PaymentList(null, OrderDt, branchid);
            return Json(List);
        }

    }
}
