using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using REST.ApiControllers;
using REST.Data;
using REST.Models;
using REST.Service;

namespace REST.Controllers
{
    [Authorize]
    public class StockManualController : BaseController
    {
        #region Connect db / Data System

        private readonly DbConnection _db;
        public StockManualController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        public IActionResult Index()
        {
            var branchId = User.Claims.FirstOrDefault(x => x.Type == "BranchId").Value;
            var _Get = new GetViewController(_db);
            ViewBag.Staple = _Get.ViewStaple(branchId);
            return View();
        }

        public IActionResult Save(string id, int Tran, decimal AAfter, decimal AEdit, decimal ABalance)
        {
            string St = null;

            var branchId = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            DateTime Dates = Share.FormatDate(DateTime.Now).Date;
            string Times = DateTime.Now.ToString("HH:mm");

            try
            {
                var Item = new ST_TranManual();
                Item.StapleId = id;
                Item.Trans = Tran;
                Item.Dates = Dates;
                Item.Times = Times;
                Item.After = AAfter;
                Item.Edit = AEdit;
                Item.Balance = ABalance;
                Item.BranchId = branchId;

                _db.ST_TranManual.Add(Item);
                _db.SaveChanges();

                //----- Update Amount Staple
                if(UpdateAmountStaple(id, ABalance, branchId))
                {
                    toastrAlert("Save Data", "บันทึกข้อมูลเรียบร้อย", Enums.NotificationToastr.success);
                    St = "success";
                }
                else
                {
                    St = "error";
                }                   
            }
            catch (Exception)
            {
                St = "error";
            }

            return Json(new { data = St});
        }

        public Boolean UpdateAmountStaple(string id, decimal ABalance, string BranchId)
        {
            try
            {
                var Item = new CD_Staple();
                Item = _db.CD_Staple.FirstOrDefault(x => x.StapleId == id && x.BranchId == BranchId);

                Item.QtyBalance = ABalance;
                /* DATA */
                Item.UpdateUser = User.Identity.Name;
                Item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                _db.CD_Staple.Update(Item);
                _db.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
