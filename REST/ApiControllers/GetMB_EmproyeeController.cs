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

namespace REST.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetMB_EmproyeeController : BaseController
    {
        #region Connect db
        private readonly DbConnection _db;

        public GetMB_EmproyeeController(DbConnection db)
        {
            _db = db;
        }
        #endregion 

        public List<ViewMB_Emproyee> EmproyeeAll()
        {
            var list = new List<ViewMB_Emproyee>();
            var sql = $"SELECT EmproyeeId, Title + ' ' + FirstName + ' ' + LastName AS Name, PositionName "
                    + $"FROM MB_Emproyee "
                    + $"LEFT JOIN CD_Position ON MB_Emproyee.PositionId = CD_Position.PositionId";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var item = new ViewMB_Emproyee();
                        if (!data.IsDBNull(0))
                            item.EmproyeeId = data.GetString(0);
                        if (!data.IsDBNull(1))
                            item.Name = data.GetString(1);
                        if (!data.IsDBNull(2))
                            item.PositionName = data.GetString(2);
                        list.Add(item);
                    }
                }
            }
            return list;
        }
    }
}
