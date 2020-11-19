using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using REST.ApiControllers;
using REST.Data;
using REST.Models;
using REST.Service;
using REST.ViewModels;
using static REST.Service.Enums;

namespace REST.Controllers
{
    [Authorize]
    public class CD_FoodController : BaseController
    {
        #region Connect db / Data System

        private readonly DbConnection _db;
        public static string _mode = Comp.FormMode.ADD;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CD_FoodController(DbConnection db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
        }

        #endregion 

        public IActionResult Index()
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            var _food = new GetCD_FoodController(_db);
            ViewBag.DT_Food = _food.FoodAll(branchid);
            return View();
        }

        public IActionResult FrmFood(string mode, string id = null)
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            if (mode == "Add")
            {
                _mode = Comp.FormMode.ADD;
                FrmMode();
                var item = new ViewFood();
                item.Price = "0.00";
                return View(item);
            }
            else
            {
                _mode = Comp.FormMode.EDIT;
                FrmMode();
                var item = LoadData(id, branchid);
                return View(item);
            }
        }

        [HttpPost]
        public IActionResult FrmSave(ViewFood info)
        {
            string status = "success";

            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;

            if (SaveData(info, branchid))
            {
                toastrAlert("อาหาร", "บันทึกข้อมูลเรียบร้อย", Enums.NotificationToastr.success);
                status = "success";
            }
            else
            {
                status = "error";
            }

            return Json(new { data = status });
        }

        public Boolean SaveData(ViewFood info, string branchid)
        {
            var item1 = new CD_Food();
            var _running = new GetRunningController(_db);
            try
            {
                switch (_mode)
                {
                    case Comp.FormMode.ADD:

                        // check data count food
                        var IsNull = _db.CD_Food.Where(x => x.FoodId == info.FoodId && x.BranchId == branchid).ToList();
                        if (IsNull.Count > 0)
                        {
                            return false;
                        }
                        else
                        {
                            // Save Food

                            if (info.ImageFile != null)
                            {
                                // Save Image to wwwroot/
                                string wwwRootPath = _hostEnvironment.WebRootPath;
                                string fileName = Path.GetFileNameWithoutExtension(info.ImageFile.FileName);
                                string extension = Path.GetExtension(info.ImageFile.FileName);
                                info.ImageName = fileName = fileName.Replace(fileName, info.FoodId + "_Food") + extension;
                                string path = Path.Combine(wwwRootPath + "/Images/Food/", fileName);
                                using (var fileStream = new FileStream(path, FileMode.Create))
                                {
                                    info.ImageFile.CopyTo(fileStream);
                                }
                            }

                            item1.FoodId = info.FoodId;
                            item1.FoodName = info.FoodName;
                            item1.Price = Share.FormatDecimal(info.Price);
                            item1.ImageName = info.ImageName;
                            item1.GroupFoodId = info.GroupFoodId;
                            item1.DishId = info.DishId;
                            /* DATA */
                            item1.BranchId = branchid;
                            item1.CreateUser = User.Identity.Name;
                            item1.CreateDate = Share.FormatDate(DateTime.Now).Date;
                            item1.UpdateUser = User.Identity.Name;
                            item1.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                            _db.CD_Food.Add(item1);
                            _db.SaveChanges();

                            // Save FoodSub
                            if (info.Sub != null)
                            {
                                dynamic staple = JsonConvert.DeserializeObject(info.Sub);

                                /* Input Data */
                                foreach (dynamic item in staple)
                                {
                                    string id = item.StapleId;
                                    decimal qty = item.Amount;

                                    var unit = _db.CD_Staple.FirstOrDefault(x => x.StapleId == id && x.BranchId == branchid);

                                    var Item = new CD_FoodSub();
                                    Item.FoodId = info.FoodId;
                                    Item.StapleId = id;
                                    Item.Amount = qty;
                                    Item.UnitId = unit.UnitId;
                                    /* DATA */
                                    Item.BranchId = branchid;

                                    _db.CD_FoodSub.Add(Item);
                                    _db.SaveChanges();
                                }
                            }

                            // Set Runnig
                            _running.SetRunning(info.GroupFoodId, info.FoodId, branchid);
                        }                        

                        break;

                    case Comp.FormMode.EDIT:

                        item1 = _db.CD_Food.FirstOrDefault(x => x.FoodId == info.FoodId && x.BranchId == branchid);

                        if (info.ImageFile != null)
                        {
                            var imagePath = Path.Combine(_hostEnvironment.WebRootPath + "/Images/Food/", item1.ImageName);
                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                            }

                            // Save Image to wwwroot/
                            string wwwRootPath = _hostEnvironment.WebRootPath;
                            string fileName = Path.GetFileNameWithoutExtension(info.ImageFile.FileName);
                            string extension = Path.GetExtension(info.ImageFile.FileName);
                            info.ImageName = fileName = fileName.Replace(fileName, info.FoodId + "_Food") + extension;
                            string path = Path.Combine(wwwRootPath + "/Images/Food/", fileName);
                            using (var fileStream = new FileStream(path, FileMode.Create))
                            {
                                info.ImageFile.CopyToAsync(fileStream);
                                item1.ImageName = info.ImageName;
                            }
                        }

                        // Save Food                        
                        item1.FoodName = info.FoodName;
                        item1.Price = Share.FormatDecimal(info.Price);                        
                        item1.GroupFoodId = info.GroupFoodId;
                        item1.DishId = info.DishId;
                        /* DATA */
                        item1.BranchId = branchid;
                        item1.UpdateUser = User.Identity.Name;
                        item1.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                        _db.CD_Food.Update(item1);
                        _db.SaveChanges();

                        // Save FoodSub
                        if (info.Sub != null)
                        {
                            var delete = _db.CD_FoodSub.Where(x => x.FoodId == info.FoodId).ToList();
                            _db.CD_FoodSub.RemoveRange(delete);
                            _db.SaveChanges();

                            dynamic staple = JsonConvert.DeserializeObject(info.Sub);

                            /* Input Data */
                            foreach (dynamic item in staple)
                            {
                                string id = item.StapleId;
                                decimal qty = item.Amount;

                                var unit = _db.CD_Staple.FirstOrDefault(x => x.StapleId == id && x.BranchId == branchid);

                                var Item = new CD_FoodSub();
                                Item.FoodId = info.FoodId;
                                Item.StapleId = id;
                                Item.Amount = qty;
                                Item.UnitId = unit.UnitId;
                                /* DATA */
                                Item.BranchId = branchid;

                                _db.CD_FoodSub.Add(Item);
                                _db.SaveChanges();
                            }
                        }

                        break;
                }

                return true;
            }
            catch (Exception)
            {
                Alert("", "Error Data !", Enums.NotificationType.error);
                return false;
            }
        }

        [HttpPost]
        public IActionResult Delete(ViewFood info)
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            try
            {
                // Delete Dish
                var item = _db.CD_Food.FirstOrDefault(x => x.FoodId == info.FoodId && x.BranchId == branchid);

                if (item.ImageName != null)
                {
                    var imagePath = Path.Combine(_hostEnvironment.WebRootPath + "/Images/Food/", item.ImageName);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _db.CD_Food.Remove(item);
                _db.SaveChanges();                

                var isData = _db.CD_FoodSub.Where(x => x.FoodId == info.FoodId && x.BranchId == branchid).ToList();
                if (isData.Count > 0)
                {
                    _db.CD_FoodSub.RemoveRange(isData);
                    _db.SaveChanges();
                }

                toastrAlert("อาหาร", "ลบข้อมูลเรียบร้อยแล้ว", Enums.NotificationToastr.success);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Alert("", "Error Data", Enums.NotificationType.warning);
                return RedirectToAction("FrmFood", new { mode = "Edit", id = info.FoodId });
            }
        }

        public void FrmMode()
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            if (_mode == Comp.FormMode.ADD)
            {
                ViewData["Disible-delete"] = "disabled";
                ViewData["Disible-save"] = "";
                ViewData["Readonly"] = "";
                ViewBag.GroupFood = _db.CD_GroupFood.Where(x => x.BranchId == BranchId).ToList();
                ViewBag.Dish = _db.CD_Dish.Where(x => x.BranchId == BranchId).ToList();
            }
            else
            {
                ViewData["Disible-delete"] = "";
                ViewData["Disible-save"] = "disabled";
                ViewData["Readonly"] = "readonly";
                ViewBag.GroupFood = _db.CD_GroupFood.Where(x => x.BranchId == BranchId).ToList();
                ViewBag.Dish = _db.CD_Dish.Where(x => x.BranchId == BranchId).ToList();
            }
        }

        public ViewFood LoadData(string id, string branchid)
        {
            var item = new ViewFood();

            var CD_Food = _db.CD_Food.FirstOrDefault(x => x.FoodId == id && x.BranchId == branchid);
            item.FoodId = CD_Food.FoodId;
            item.FoodName = CD_Food.FoodName;
            item.Price = Share.Cnumber(Share.FormatDouble(CD_Food.Price),2);
            item.ImageName = CD_Food.ImageName;
            item.GroupFoodId = CD_Food.GroupFoodId;
            item.DishId = CD_Food.DishId;

            var _food = new GetCD_FoodController(_db);
            item.FoodSub = _food.FoodSubId(id, branchid);

            return item;
        }
    }
}
