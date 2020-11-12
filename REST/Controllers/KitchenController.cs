using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST.ApiControllers;
using REST.Data;
using REST.Models;
using REST.Service;
using REST.ViewModels;

namespace REST.Controllers
{
    [Authorize]
    public class KitchenController : BaseController
    {
        #region Connect db
        private readonly DbConnection _db;

        public KitchenController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        // ------

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult FrmKC_Order()
        {
            var branchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var _Order = new GetSF_OrderController(_db);
            ViewBag.DataTable = _Order.Order(null, DateTime.Now.ToString("dd/MM/yyyy"), branchId);
            return View();
        }

        public IActionResult FrmKC_OrderSub(string id)
        {
            var branch = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var _Order = new GetSF_OrderController(_db);
            ViewBag.Data = _Order.OrderSub(null, id, branch);
            ViewBag.Desc = _Order.OrderById(id, branch);
            return View();
        }

        // -----------------------------------
        // --           FUNCTION            --
        // -----------------------------------

       

    }
}
