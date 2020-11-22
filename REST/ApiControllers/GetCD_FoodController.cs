using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST.Controllers;
using REST.Data;
using REST.Service;
using REST.ViewModels;

namespace REST.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetCD_FoodController : BaseController
    {
        #region Connect db
        private readonly DbConnection _db;
        public GetCD_FoodController(DbConnection db)
        {
            _db = db;
        }
        #endregion 

        public List<ViewFood> FoodAll(string branchid)
        {
            var List = new List<ViewFood>();
            string sqlWhrer = null;
            string sql = $"SELECT FoodId, FoodName, DishName, Price, GroupFoodName "
                    + $"FROM CD_Food "
                    + $"LEFT JOIN CD_GroupFood ON CD_Food.GroupFoodId = CD_GroupFood.GroupFoodId "
                    + $"LEFT JOIN CD_Dish ON CD_Food.DishId = CD_Dish.DishId ";

            if (branchid != null)
                sqlWhrer += $"CD_Food.BranchId = '{branchid}' ";

            if (sqlWhrer != null)
                sql += "WHERE " + sqlWhrer;

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewFood();
                        if (!data.IsDBNull(0))
                            Item.FoodId = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.FoodName = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.DishName = data.GetString(2);
                        if (!data.IsDBNull(3))
                            Item.Price = Share.Cnumber(Share.FormatDouble(data.GetDecimal(3)), 2);
                        if (!data.IsDBNull(4))
                            Item.GroupFoodName = data.GetString(4);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }

        public List<ViewFoodSub> FoodSubId(string id, string branchid)
        {
            var List = new List<ViewFoodSub>();
            string sqlWhrer = null;
            string sql = $"SELECT CD_FoodSub.StapleId, CD_Staple.StapleName, CD_FoodSub.Amount, CD_FoodSub.UnitId, CD_Unit.UnitName "
                    + $"FROM CD_FoodSub "
                    + $"LEFT JOIN CD_Staple ON CD_FoodSub.UnitId = CD_Staple.StapleId "
                    + $"LEFT JOIN CD_Unit ON CD_FoodSub.UnitId = CD_Unit.UnitId ";

            if (branchid != null)
                sqlWhrer += $"CD_FoodSub.BranchId = '{branchid}' ";

            if (sqlWhrer != null)
                sql += "WHERE " + sqlWhrer;

            if (id != null)
                if (sqlWhrer != null)
                    sql += $"CD_FoodSub.FoodId = '{id}' ";
                else
                    sql += $"AND CD_FoodSub.FoodId = '{id}' ";

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewFoodSub();
                        if (!data.IsDBNull(0))
                            Item.StapleId = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.StapleName = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.Amount = data.GetDecimal(2);
                        if (!data.IsDBNull(3))
                            Item.UnitId = data.GetString(3);
                        if (!data.IsDBNull(4))
                            Item.UnitName = data.GetString(4);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }
    }
}
