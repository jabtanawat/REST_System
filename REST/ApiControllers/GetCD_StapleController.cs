using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST.Controllers;
using REST.Data;
using REST.ViewModels;

namespace REST.ApiControllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GetCD_StapleController : BaseController
    {
        #region Connect db
        private readonly DbConnection _db;
        public GetCD_StapleController(DbConnection db)
        {
            _db = db;
        }
        #endregion 

        public List<ViewStaple> StapleAll(string branchid)
        {
            var List = new List<ViewStaple>();
            var sql = $"SELECT StapleId, StapleName, Amount, UnitName "
                    + $"FROM CD_Staple "
                    + $"LEFT JOIN CD_Unit ON CD_Staple.UnitId = CD_Unit.UnitId "
                    + $"WHERE CD_Staple.BranchId = '{branchid}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewStaple();
                        Item.StapleId = data.GetString(0);
                        Item.StapleName = data.GetString(1);
                        Item.Amount = data.GetDecimal(2);
                        Item.UnitName = data.GetString(3);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }

        public ViewStaple StapleId(string id, string branchid)
        {
            var item = new ViewStaple();
            var sql = $"SELECT StapleId, StapleName, Amount, UnitName "
                    + $"FROM CD_Staple "
                    + $"LEFT JOIN CD_Unit ON CD_Staple.UnitId = CD_Unit.UnitId "
                    + $"WHERE CD_Staple.BranchId = '{branchid}'";
                    if (id != null)
                        sql += $"AND StapleId = '{id}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        item.StapleId = data.GetString(0);
                        item.StapleName = data.GetString(1);
                        item.Amount = data.GetDecimal(2);
                        item.UnitName = data.GetString(3);
                    }
                }
            }

            return item;
        }

        // -------------------------------------------
        // --                                       --
        // --             ACTION STAPLE             --
        // --                                       --
        // -------------------------------------------

        public JsonResult AShowStaple(string id)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var item = StapleId(id, branchid);
            return Json(item);
        }
    }
}
