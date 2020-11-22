using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using REST.Controllers;
using REST.Data;
using REST.Models;
using REST.Service;

namespace REST.ApiControllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GetRunningController : BaseController
    {
        #region Connect db
        private readonly DbConnection _db;

        public GetRunningController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        public string Running(string id, string branchid)
        {
            string DocRunning = null;
            string RunLength = null;
            int Number = 0;
            DateTime Date = Share.FormatDate(DateTime.Now).Date;

            if (id != null)
            {
                var Running = new CD_Running();
                if(branchid != null)
                    Running = _db.CD_Running.FirstOrDefault(x => x.Name == id && x.BranchId == branchid);
                else
                    Running = _db.CD_Running.FirstOrDefault(x => x.Name == id);

                if (Running.AutoRun == true)
                {
                    RunLength = null;
                    for (int i = 0; i < Running.Number.Length; i++)
                    {
                        RunLength += '0';
                    }
                    Number = Int32.Parse(Running.Number) + 1;
                    DocRunning = Running.Front + Number.ToString(RunLength);

                    if (Running.AutoDate == true)
                    {
                        RunLength = null;
                        for (int i = 0; i < Running.Number.Length; i++)
                        {
                            RunLength += '0';
                        }
                        Number = Int32.Parse(Running.Number) + 1;
                        DocRunning = Running.Front + Date.ToString(Running.SetDate) + Number.ToString(RunLength);
                    }
                }
            }            

            return DocRunning;
        }

        public void SetRunning(string Name, string DocRunning, string branchid)
        {
            var Running = new CD_Running();
            if(branchid != null)
                Running = _db.CD_Running.FirstOrDefault(x => x.Name == Name && x.BranchId == branchid);
            else
                Running = _db.CD_Running.FirstOrDefault(x => x.Name == Name);

            if (Running.AutoRun == true)
            {
                int RunLength = Running.Number.Length;

                Running.Number = DocRunning.Substring(DocRunning.Length - RunLength);

                /* SAVE DB */
                _db.CD_Running.Update(Running);
                _db.SaveChanges();
            }
        }

        // -------------------------------------------
        // --                                       --
        // --            ACTION RUNNING             --
        // --                                       --
        // -------------------------------------------

        public JsonResult ARunning(string id)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var item = Running(id, null);
            return Json(item);
        }
    }
}
