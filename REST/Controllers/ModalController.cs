using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REST.ApiControllers;
using REST.Data;
using REST.ViewModels;

namespace REST.Controllers
{
    [Authorize]
    public class ModalController : BaseController
    {
        #region Connect db
        private readonly DbConnection _db;

        public ModalController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        public IActionResult _ModalStaple()
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            ViewBag.Staple = _db.CD_Staple.Where(x => x.BranchId == BranchId).ToList();
            return PartialView("_ModalStaple");
        }

        public IActionResult _StTrans()
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            ViewBag.Trans = _db.ST_Trans.Where(x => x.BranchId == BranchId).ToList();
            return PartialView("_StTrans");
        }

        public IActionResult _FrmStockStaple(string id)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var GetView = new GetViewController(_db);
            var Item = new ViewStaple();
            Item = GetView.ViewStapleById(id, BranchId);
            ViewBag.StapleId = Item.StapleId;
            ViewBag.StapleName = Item.StapleName;
            ViewBag.Amount = Item.Amount;
            ViewBag.Unit = Item.Unit;
            return PartialView("_FrmStockStaple");
        }

        public IActionResult _ViewStockManual()
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var GetView = new GetViewController(_db);
            ViewBag.View = GetView.ViewTranManual(BranchId);
            return PartialView("_TBViewTranManual");
        }

        public IActionResult _ModalST(string id)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            ViewBag.TableId = _db.CD_Table.FirstOrDefault(x => x.TableId == id && x.BranchId == branchid).TableId;
            ViewBag.TableName = _db.CD_Table.FirstOrDefault(x => x.TableId == id && x.BranchId == branchid).TableName;
            return PartialView("_ModalST");
        }

        public IActionResult _AmountFood(string id)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            ViewBag.FoodAmount = _db.CD_Food.FirstOrDefault(x => x.FoodId == id && x.BranchId == branchid);
            return PartialView("_AmountFood");
        }

        // -------------------------------------------
        // ---                                     ---
        // ***                POPUP                ***
        // ---                                     ---
        // -------------------------------------------

        public IActionResult _popupDiscount()
        {
            return PartialView("_popupDiscount");
        }

        public IActionResult _popupMember()
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var _get = new GetViewController(_db);
            ViewBag.View = _get.ViewMember(branchid);
            return PartialView("_popupMember");
        }

        public IActionResult _popupTable()
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var _get = new GetCD_TableController(_db);
            ViewBag.View = _get.Table(branchid);
            return PartialView("_popupTable");
        }

        public IActionResult _popupStaple()
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            ViewBag.Staple = _db.CD_Staple.Where(x => x.BranchId == BranchId).ToList();
            return PartialView("_popupStaple");
        }

        public IActionResult _popupSupplier()
        {
            var _Get = new GetMB_SupplierController(_db);
            ViewBag.Supplier = _Get.SupplierAll();
            return PartialView("_popupSupplier");
        }

        public IActionResult _popupStapleBalance(string id)
        {
            ViewBag.StapleBalance = _db.StapleBalance.Where(x => x.StapleId == id).ToList();
            return PartialView("_popupStapleBalance");
        }

        public IActionResult _popupCalculate()
        {
           return PartialView("_popupCalculate");
        }

        public IActionResult _popupAccountChart()
        {
            ViewBag.AccountChart = _db.GL_AccountChart.ToList();
            return PartialView("_popupAccountChart");
        }

        public IActionResult _popupMenuTable(string id)
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var item = _db.CD_Table.FirstOrDefault(x => x.TableId == id && x.BranchId == branchid);
            ViewBag.TableId = item.TableId;
            ViewBag.TableName = item.TableName;
            ViewBag.TableST = item.TableST;
            return PartialView("_popupMenuTable");
        }

        public IActionResult _popupEditFood(string BillId, int i, string FoodId)
        {
            var _Get = new GetSF_BillController(_db);
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var item = _Get.GetBillSub_ByFood(BillId, i, FoodId, branchid);
            return PartialView("_popupEditFood", item);
        }
    }
}
