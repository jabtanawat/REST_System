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
    public class GetCD_TableController : ControllerBase
    {
        #region Connect db
        private readonly DbConnection _db;
        public GetCD_TableController(DbConnection db)
        {
            _db = db;
        }
        #endregion 

        public List<CD_Table> Table(string ZoneId, string Status, string branchid)
        {
            var List = new List<CD_Table>();
            var sql = $"SELECT TableId, TableName, Personal, Description, TableST "
                    + $"FROM CD_Table "
                    + $"WHERE BranchId = '{branchid}' ";
            if (Status != null)
                sql += $"AND TableST = '{Status}'";
            if (ZoneId != null)
                sql += $"AND ZoneId = '{ZoneId}'";

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new CD_Table();
                        Item.TableId = data.GetString(0);
                        Item.TableName = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.Personal = data.GetInt32(2);
                        if (!data.IsDBNull(3))
                            Item.Description = data.GetString(3);
                        Item.TableST = data.GetInt32(4);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }
    }
}
