using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST.Data;
using REST.Models;

namespace REST.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetMB_MemberController : ControllerBase
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
            var sql = $"SELECT MemberId, Title + ' ' + FirstName + ' ' + LastName AS Name "
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
                    }
                }
            }
            return Item;
        }
    }
}
