using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using REST.Controllers;
using REST.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST.ApiControllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GetSettingController : BaseController
    {
        #region Connect db
        private readonly DbConnection _db;

        public GetSettingController(DbConnection db)
        {
            _db = db;
        }
        #endregion 

        // -------------------------------------------
        // --                                       --
        // --            ACTION SETTING             --
        // --                                       --
        // -------------------------------------------

        public JsonResult ASetting()
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var item = _db.Setting.FirstOrDefault();
            return Json(item);
        }
    }
}
