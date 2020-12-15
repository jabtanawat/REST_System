using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST.Controllers;
using REST.Data;
using REST.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using REST.Service;

namespace REST.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetGL_PatternController : BaseController
    {
        #region Connect db
        private readonly DbConnection _db;
        public GetGL_PatternController(DbConnection db)
        {
            _db = db;
        }
        #endregion 

        public List<ViewGL_Pattern> GetPattern()
        {
            var List = new List<ViewGL_Pattern>();
            string sqlWhere = null;
            string sql = $"SELECT P_ID, P_Name, MenuId "
                    + $"FROM GL_Pattern ";
            if (sqlWhere != null)
                sql += "WHERE " + sqlWhere;

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewGL_Pattern();
                        if (!data.IsDBNull(0))
                            Item.P_ID = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.P_Name = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.MenuId = data.GetString(2);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }

        public List<ViewGL_PatternSub> GetPatternSubById(string id)
        {
            var List = new List<ViewGL_PatternSub>();
            string sqlWhere = null;
            string sql = $"SELECT P_ID, GL_PatternSub.AccNo, AccName, GL_PatternSub.DrCr, Amount, Status, i "
                    + $"FROM GL_PatternSub "
                    + $"LEFT JOIN GL_AccountChart ON GL_PatternSub.AccNo = GL_AccountChart.AccNo ";
            if (id != null)
                if(sqlWhere != null)
                    sqlWhere += $"AND P_ID = '{id}' ";
                else
                    sqlWhere += $"P_ID = '{id}' ";

            if (sqlWhere != null)
                sql += "WHERE " + sqlWhere;

            sql += "ORDER BY i ASC";

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewGL_PatternSub();
                        if (!data.IsDBNull(0))
                            Item.P_ID = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.AccNo = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.AccName = data.GetString(2);
                        if (!data.IsDBNull(3))
                            Item.DrCr = Share.FormatString(data.GetInt32(3));
                        if (!data.IsDBNull(4))
                            Item.Amount = data.GetString(4);
                        if (!data.IsDBNull(5))
                            Item.Status = data.GetString(5);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }
    }
}
