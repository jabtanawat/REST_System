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
    public class GetGL_TemplateController : BaseController
    {
        #region Connect db
        private readonly DbConnection _db;
        public GetGL_TemplateController(DbConnection db)
        {
            _db = db;
        }
        #endregion 

        public List<ViewGL_Template> GetTemplate()
        {
            var List = new List<ViewGL_Template>();
            string sqlWhrer = null;
            string sql = $"SELECT M_ID, M_Name, GL_Template.BookId, BookName  "
                    + $"FROM GL_Template "
                    + $"LEFT JOIN GL_AccountBook ON GL_Template.BookId = GL_AccountBook.BookId ";
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
                        var Item = new ViewGL_Template();
                        if (!data.IsDBNull(0))
                            Item.M_ID = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.M_Name = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.BookId = data.GetString(2);
                        if (!data.IsDBNull(3))
                            Item.BookName = data.GetString(3);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }

        public List<ViewGL_TemplateSub> GetTemplateSubById(string id)
        {
            var List = new List<ViewGL_TemplateSub>();
            string sqlWhere = null;
            string sql = $"SELECT M_ID, GL_TemplateSub.AccNo, AccName, GL_TemplateSub.DrCr, Amount, i "
                    + $"FROM GL_TemplateSub "
                    + $"LEFT JOIN GL_AccountChart ON GL_TemplateSub.AccNo = GL_AccountChart.AccNo ";
            if (id != null)
                if(sqlWhere != null)
                    sqlWhere += $"AND M_ID = '{id}' ";
                else
                    sqlWhere += $"M_ID = '{id}' ";

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
                        var Item = new ViewGL_TemplateSub();
                        if (!data.IsDBNull(0))
                            Item.M_ID = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.AccNo = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.AccName = data.GetString(2);
                        if (!data.IsDBNull(3))
                            Item.DrCr = Share.FormatString(data.GetInt32(3));
                        if (!data.IsDBNull(4))
                            Item.Amount = data.GetString(4);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }
    }
}
