using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST.Data;
using REST.Models;
using REST.ViewModels;

namespace REST.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetSF_OrderController : ControllerBase
    {
        #region Connect db
        private readonly DbConnection _db;
        public GetSF_OrderController(DbConnection db)
        {
            _db = db;
        }
        #endregion 

        public List<ViewSF_OrderSub> OrderSub(string TableId, string branchid)
        {
            var List = new List<ViewSF_OrderSub>();
            var sql = $"SELECT o.OrderId, o.i, o.TableId, o.FoodId, f.FoodName, o.Amount, o.Price, o.Status "
                    + $"FROM SF_OrderSub AS o "
                    + $"LEFT JOIN CD_Food AS f ON o.FoodId = f.FoodId "
                    + $"WHERE o.BranchId = '{branchid}' ";
            if (TableId != null)
                sql += $"AND o.TableId = '{TableId}'";

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
    }
}
