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
            var _food = new GetCD_FoodController(_db);
            ViewBag.DT_Food = _food.FoodAll(null);
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

        public Boolean SaveData(ViewFood info, string branchid)
        {
            var item = new CD_Food();
            var _running = new GetRunningController(_db);
            dynamic staple = JsonConvert.DeserializeObject(info.Sub);
            try
            {
                switch (_mode)
                {
                    case Comp.FormMode.ADD:

                        // check data count food
                        var IsNull = _db.CD_Food.Where(x => x.FoodId == info.FoodId).ToList();
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

                            item.FoodId = info.FoodId;
                            item.FoodName = info.FoodName;
                            item.Price = Share.FormatDecimal(info.Price);
                            item.ImageName = info.ImageName;
                            item.GroupFoodId = info.GroupFoodId;
                            item.DishId = info.DishId;
                            /* DATA */
                            item.Bch = info.Bch;
                            item.BchName = info.BchName;
                            item.BranchId = branchid;
                            item.CreateUser = User.Identity.Name;
                            item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                            item.UpdateUser = User.Identity.Name;
                            item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                            _db.CD_Food.Add(item);
                            _db.SaveChanges();                            

                            // Save FoodSub
                            if (staple.Count > 0)
                            {

                                /* Input Data */
                                foreach (dynamic result in staple)
                                {
                                    string id = result.StapleId;
                                    decimal qty = result.Amount;

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
                            _running.SetRunning(info.GroupFoodId, info.FoodId, null);
                        }                        

                        break;

                    case Comp.FormMode.EDIT:

                        item = _db.CD_Food.FirstOrDefault(x => x.FoodId == info.FoodId && x.BranchId == branchid);

                        if (info.ImageFile != null)
                        {
                            var imagePath = Path.Combine(_hostEnvironment.WebRootPath + "/Images/Food/", item.ImageName);
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
                                item.ImageName = info.ImageName;
                            }
                        }

                        // Save Food                        
                        item.FoodName = info.FoodName;
                        item.GroupFoodId = info.GroupFoodId;
                        item.DishId = info.DishId;
                        /* DATA */
                        item.Bch = info.Bch;
                        item.BchName = info.BchName;
                        item.BranchId = branchid;
                        item.UpdateUser = User.Identity.Name;
                        item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                        _db.CD_Food.Update(item);
                        _db.SaveChanges();

                        // Save FoodSub
                        if (staple.Count > 0)
                        {
                            var delete = _db.CD_FoodSub.Where(x => x.FoodId == info.FoodId).ToList();
                            _db.CD_FoodSub.RemoveRange(delete);
                            _db.SaveChanges();                            

                            /* Input Data */
                            foreach (dynamic result in staple)
                            {
                                string id = result.StapleId;
                                decimal qty = result.Amount;

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

        public void FrmMode()
        {
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            if (_mode == Comp.FormMode.ADD)
            {
                ViewData["Disible-delete"] = "disabled";
                ViewData["Disible-save"] = "";
                ViewData["Readonly"] = "";
                ViewBag.Branch = _db.MG_Branch.ToList();
                var List = new List<CD_GroupFood>();
                var CD_GroupFood = _db.CD_GroupFood.ToList();
                foreach (var row in CD_GroupFood)
                {
                    var item = new CD_GroupFood();
                    if (row.Bch == 1 || row.BchName == branchid)
                    {
                        item.GroupFoodId = row.GroupFoodId;
                        item.GroupFoodName = row.GroupFoodName;
                        List.Add(item);
                    }

                }
                ViewBag.GroupFood = List;
                ViewBag.Dish = _db.CD_Dish.Where(x => x.BranchId == branchid).ToList();
            }
            else
            {
                ViewData["Disible-delete"] = "";
                ViewData["Disible-save"] = "disabled";
                ViewData["Readonly"] = "readonly";
                ViewBag.Branch = _db.MG_Branch.ToList();
                var List = new List<CD_GroupFood>();
                var CD_GroupFood = _db.CD_GroupFood.ToList();
                foreach (var row in CD_GroupFood)
                {
                    var item = new CD_GroupFood();
                    if (row.Bch == 1 || row.BchName == branchid)
                    {
                        item.GroupFoodId = row.GroupFoodId;
                        item.GroupFoodName = row.GroupFoodName;
                        List.Add(item);
                    }

                }
                ViewBag.GroupFood = List;
                ViewBag.Dish = _db.CD_Dish.Where(x => x.BranchId == branchid).ToList();
            }
        }

        public ViewFood LoadData(string id, string branchid)
        {
            var item = new ViewFood();

            var CD_Food = _db.CD_Food.FirstOrDefault(x => x.FoodId == id);
            item.FoodId = CD_Food.FoodId;
            item.FoodName = CD_Food.FoodName;
            item.Price = Share.Cnumber(Share.FormatDouble(CD_Food.Price),2);
            item.ImageName = CD_Food.ImageName;
            item.GroupFoodId = CD_Food.GroupFoodId;
            item.DishId = CD_Food.DishId;
            item.Bch = CD_Food.Bch;
            item.BchName = CD_Food.BchName;

            var _food = new GetCD_FoodController(_db);
            item.FoodSub = _food.FoodSubId(id, null);

            return item;
        }
    }
}
