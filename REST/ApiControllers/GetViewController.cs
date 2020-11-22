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
using REST.ViewModels;

namespace REST.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetViewController : BaseController
    {
        #region Connect db
        private readonly DbConnection _db;

        public GetViewController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        public List<ViewST_TranSub> ViewST_TranSub(string id, string branchId)
        {
            var List = new List<ViewST_TranSub>();
            try
            {
                var sql = $"SELECT t.StapleId, s.StapleName, t.Amount, Price, Total "
                    + $"FROM ST_TranSub AS t "
                    + $"LEFT JOIN CD_Staple AS s ON t.StapleId = s.StapleId "
                    + $"WHERE t.BranchId = '{branchId}' AND Documents = '{id}' ORDER BY i ASC";
                using (var command = _db.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    _db.Database.OpenConnection();
                    using (var data = command.ExecuteReader())
                    {
                        while (data.Read())
                        {
                            var Item = new ViewST_TranSub();
                            if (!data.IsDBNull(0))
                                Item.StapleId = data.GetString(0);
                            if (!data.IsDBNull(1))
                                Item.StapleName = data.GetString(1);
                            if (!data.IsDBNull(2))
                                Item.Amount = data.GetDecimal(2);
                            if (!data.IsDBNull(3))
                                Item.Price = data.GetDecimal(3);
                            if (!data.IsDBNull(4))
                                Item.Total = data.GetDecimal(4);
                            List.Add(Item);
                        }
                    }
                }

                return List;
            }
            catch (Exception)
            {
                return null;
            }            
        }

        public ViewStaple ViewStapleById(string id, string branchId)
        {
            var Item = new ViewStaple();
            var sql = $"SELECT StapleId, StapleName, Amount, UnitName "
                    + $"FROM CD_Staple "
                    + $"LEFT JOIN CD_Unit ON CD_Staple.UnitId = CD_Unit.UnitId "
                    + $"WHERE CD_Staple.BranchId = '{branchId}' AND CD_Staple.StapleId = '{id}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {                        
                        if (!data.IsDBNull(0))
                            Item.StapleId = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.StapleName = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.Amount = data.GetDecimal(2);
                        if (!data.IsDBNull(3))
                            Item.UnitName = data.GetString(3);
                    }
                }
            }

            return Item;
        }

        public List<ViewStaple> ViewStaple(string branchId)
        {
            var List = new List<ViewStaple>();
            var sql = $"SELECT StapleId, StapleName, Amount, UnitName "
                    + $"FROM CD_Staple "
                    + $"LEFT JOIN CD_Unit ON CD_Staple.UnitId = CD_Unit.UnitId "
                    + $"WHERE CD_Staple.BranchId = '{branchId}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewStaple();
                        if(!data.IsDBNull(0))
                            Item.StapleId = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.StapleName = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.Amount = data.GetDecimal(2);
                        if (!data.IsDBNull(3))
                            Item.UnitName = data.GetString(3);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }

        public List<ViewST_TranManual> ViewTranManual(string branchId)
        {
            var List = new List<ViewST_TranManual>();
            var sql = $"SELECT SP.StapleName, CASE WHEN Trans = 1 THEN 'เพิ่มจำนวน' WHEN Trans = 2 THEN 'ลบจำนวน' END AS Trans, Dates, Times, After, Edit, Balance "
                    + $"FROM ST_TranManual AS TM "
                    + $"LEFT JOIN CD_Staple AS SP ON TM.StapleId = SP.StapleId "
                    + $"WHERE TM.BranchId = '{branchId}' AND SP.BranchId = '{branchId}' "
                    + $"ORDER BY i ASC";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewST_TranManual();
                        if (!data.IsDBNull(0))
                            Item.StapleName = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.Trans = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.Dates = data.GetDateTime(2);
                        if (!data.IsDBNull(3))
                            Item.Times = data.GetString(3);
                        if (!data.IsDBNull(4))
                            Item.After = data.GetDecimal(4);
                        if (!data.IsDBNull(5))
                            Item.Edit = data.GetDecimal(5);
                        if (!data.IsDBNull(6))
                            Item.Balance = data.GetDecimal(6);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }

        public List<ViewMB_Member> ViewMember(string branchid)
        {
            var List = new List<ViewMB_Member>();
            string sqlWhrer = null;
            var sql = $"SELECT MemberId, CASE WHEN Type = 1 THEN 'ทั่วไป' WHEN Type = 2 THEN 'สมาชิก' END AS TypeName, Title + ' ' + FirstName + ' ' + LastName AS Name "
                    + $"FROM MB_Member ";

            if (branchid != null)
                sqlWhrer += $"BranchId = '{branchid}' ";

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
                        var Item = new ViewMB_Member();
                        if (!data.IsDBNull(0))
                            Item.MemberId = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.TypeName = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.Name = data.GetString(2);                        
                        List.Add(Item);
                    }
                }
            }

            return List;
        }
    }
}
