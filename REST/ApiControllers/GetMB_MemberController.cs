using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST.Data;
using REST.Models;
using REST.Controllers;
using REST.Service;

namespace REST.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetMB_MemberController : BaseController
    {
        #region Connect db
        private readonly DbConnection _db;

        public GetMB_MemberController(DbConnection db)
        {
            _db = db;
        }
        #endregion 

        public ViewMB_Member ViewMemberById(string id, string branchId)
        {
            var Item = new ViewMB_Member();
            var sql = $"SELECT MemberId, Title + ' ' + FirstName + ' ' + LastName AS Name, CASE WHEN Type = 1 THEN 'ทั่วไป' WHEN Type = 2 THEN 'สมาชิก' END AS TypeName, Rebate, Score "
                    + $"FROM MB_Member "
                    + $"WHERE BranchId = '{branchId}' ";
                if (id != null)
                    sql += $"AND MemberId = '{id}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {                        
                        if (!data.IsDBNull(0))
                            Item.MemberId = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.Name = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.TypeName = data.GetString(2);
                        if (!data.IsDBNull(3))
                            Item.Rebate = data.GetInt32(3);
                        if (!data.IsDBNull(4))
                            Item.Score = Share.FormatString(data.GetDecimal(4));
                    }
                }
            }
            return Item;
        }
    }
}
