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
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GetMB_SupplierController : BaseController
    {
        #region Connect db
        private readonly DbConnection _db;

        public GetMB_SupplierController(DbConnection db)
        {
            _db = db;
        }
        #endregion

        public List<ViewMB_Supplier> SupplierAll()
        {
            var list = new List<ViewMB_Supplier>();
            var sql = $"SELECT SupplierId, Name "
                    + $"FROM MB_Supplier";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var item = new ViewMB_Supplier();
                        if (!data.IsDBNull(0))
                            item.SupplierId = data.GetString(0);
                        if (!data.IsDBNull(1))
                            item.Name = data.GetString(1);
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        // -------------------------------------------
        // -------------------------------------------
        // ---                                     ---
        // ---           ACTION SUPPLIER           ---
        // ---                                     ---
        // -------------------------------------------
        // -------------------------------------------

        public JsonResult AShowSupplier(string id)
        {
            //var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var item = _db.MB_Supplier.FirstOrDefault(x => x.SupplierId == id);
            return Json(item);
        }
    }
}
