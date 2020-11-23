using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using REST.ApiControllers;
using REST.Data;
using REST.Models;
using REST.Service;
using REST.ViewModels;

namespace REST.Controllers
{
    [Authorize]
    public class CD_DrinkController : BaseController
    {
        #region Connect db / Data System

        private readonly DbConnection _db;
        public static string _mode = Comp.FormMode.ADD;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CD_DrinkController(DbConnection db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
        }

        #endregion 

        public IActionResult Index()
        {
            ViewBag.Drink = _db.CD_Drink.ToList();
            return View();
        }

        public IActionResult FrmDrink(string mode, string id = null)
        {
            if (mode == "Add")
            {
                _mode = Comp.FormMode.ADD;
                FrmMode();
                var item = new ViewCD_Drink();
                item.Price = "0.00";
                return View(item);
            }
            else
            {
                _mode = Comp.FormMode.EDIT;
                FrmMode();
                var item = LoadData(id);
                return View(item);
            }
        }

        [HttpPost]
        public IActionResult FrmDrink(ViewCD_Drink info)
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            if (ModelState.IsValid)
            {
                if (SaveData(info, branchid))
                {
                    toastrAlert("เครื่องดื่ม", "บันทึกข้อมูลเรียบร้อย", Enums.NotificationToastr.success);
                    return RedirectToAction("Index");
                }
                else
                {
                    FrmMode();
                    return View();
                }
            }
            else
            {
                FrmMode();
                Alert("", "ไม่สามารถบันทึกข้อมูลได้ !", Enums.NotificationType.error);
                return View();
            }
        }

        [HttpPost]
        public IActionResult Delete(ViewCD_Drink info)
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            try
            {
                var item = _db.CD_Drink.FirstOrDefault(x => x.DrinkId == info.DrinkId);

                if (item.ImageName != null)
                {
                    var imagePath = Path.Combine(_hostEnvironment.WebRootPath + "/Images/Drink/", item.ImageName);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _db.CD_Drink.Remove(item);
                _db.SaveChanges();

                toastrAlert("เครื่องดื่ม", "ลบข้อมูลเรียบร้อยแล้ว", Enums.NotificationToastr.success);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Alert("", "Error Data", Enums.NotificationType.warning);
                return RedirectToAction("FrmDrink", new { mode = "Edit", id = info.DrinkId });
            }
        }

        public Boolean SaveData(ViewCD_Drink info, string branchid)
        {
            var item = new CD_Drink();
            var _running = new GetRunningController(_db);
            try
            {
                switch (_mode)
                {
                    case Comp.FormMode.ADD:

                        // check data count food
                        var IsNull = _db.CD_Drink.Where(x => x.DrinkId == info.DrinkId).ToList();
                        if (IsNull.Count > 0)
                        {
                            Alert("", "รหัสนี้มีอยู่แล้ว !", Enums.NotificationType.warning);
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
                                info.ImageName = fileName = fileName.Replace(fileName, info.DrinkId + "_Drink") + extension;
                                string path = Path.Combine(wwwRootPath + "/Images/Drink/", fileName);
                                using (var fileStream = new FileStream(path, FileMode.Create))
                                {
                                    info.ImageFile.CopyTo(fileStream);
                                }
                            }

                            item.DrinkId = info.DrinkId;
                            item.DrinkName = info.DrinkName;
                            item.Price = Share.FormatDecimal(info.Price);
                            item.ImageName = info.ImageName;
                            item.GroupFoodId = info.GroupFoodId;
                            /* DATA */
                            item.Bch = info.Bch;
                            item.BchName = info.BchName;
                            item.BranchId = branchid;
                            item.CreateUser = User.Identity.Name;
                            item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                            item.UpdateUser = User.Identity.Name;
                            item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                            _db.CD_Drink.Add(item);
                            _db.SaveChanges();                            

                            // Set Runnig
                            _running.SetRunning(info.GroupFoodId, info.DrinkId, null);
                        }

                        break;

                    case Comp.FormMode.EDIT:

                        item = _db.CD_Drink.FirstOrDefault(x => x.DrinkId == info.DrinkId);

                        if (info.ImageFile != null)
                        {
                            var imagePath = Path.Combine(_hostEnvironment.WebRootPath + "/Images/Drink/", item.ImageName);
                            if (System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                            }

                            // Save Image to wwwroot/
                            string wwwRootPath = _hostEnvironment.WebRootPath;
                            string fileName = Path.GetFileNameWithoutExtension(info.ImageFile.FileName);
                            string extension = Path.GetExtension(info.ImageFile.FileName);
                            info.ImageName = fileName = fileName.Replace(fileName, info.DrinkId + "_Drink") + extension;
                            string path = Path.Combine(wwwRootPath + "/Images/Drink/", fileName);
                            using (var fileStream = new FileStream(path, FileMode.Create))
                            {
                                info.ImageFile.CopyToAsync(fileStream);
                                item.ImageName = info.ImageName;
                            }
                        }

                        // Save Food                        
                        item.DrinkName = info.DrinkName;
                        item.Price = Share.FormatDecimal(info.Price);
                        item.GroupFoodId = info.GroupFoodId;
                        /* DATA */
                        item.Bch = info.Bch;
                        item.BchName = info.BchName;
                        item.BranchId = branchid;
                        item.UpdateUser = User.Identity.Name;
                        item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                        _db.CD_Drink.Update(item);
                        _db.SaveChanges();                   

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
            }
        }

        public ViewCD_Drink LoadData(string id)
        {
            var item = new ViewCD_Drink();

            var CD_Drink = _db.CD_Drink.FirstOrDefault(x => x.DrinkId == id);
            item.DrinkId = CD_Drink.DrinkId;
            item.DrinkName = CD_Drink.DrinkName;
            item.Price = Share.Cnumber(Share.FormatDouble(CD_Drink.Price), 2);
            item.ImageName = CD_Drink.ImageName;
            item.GroupFoodId = CD_Drink.GroupFoodId;
            item.Bch = CD_Drink.Bch;
            item.BchName = CD_Drink.BchName;

            return item;
        }
    }
}
