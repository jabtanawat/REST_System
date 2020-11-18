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
            ViewBag.DT_Food = GetFood();
            return View();
        }

        public IActionResult FrmFood(string mode, string id = null)
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            if (mode == "Add")
            {
                _mode = Comp.FormMode.ADD;
                FrmMode();
                return View();
            }
            else
            {
                _mode = Comp.FormMode.EDIT;
                FrmMode();
                var item = _db.CD_Dish.FirstOrDefault(x => x.DishId == id && x.BranchId == branchid);
                return View(item);
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







        public IActionResult Insert()
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;

            if (HttpContext.Session.GetString("Session_StapleList") != null)
            {
                HttpContext.Session.Remove("Session_StapleList");
            }

            ViewBag.GroupFood = _db.CD_GroupFood.Where(x => x.BranchId == BranchId).ToList();
            ViewBag.Dish = _db.CD_Dish.Where(x => x.BranchId == BranchId).ToList();
            ViewBag.Staple = _db.CD_Staple.Where(x => x.BranchId == BranchId).ToList();

            return View();
        }

        public IActionResult Update(string id)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var Food = _db.CD_Food.FirstOrDefault(x => x.FoodId == id && x.BranchId == BranchId);

            if (HttpContext.Session.GetString("Session_StapleList") != null)
            {
                HttpContext.Session.Remove("Session_StapleList");
            }

            ViewBag.GroupFood = _db.CD_GroupFood.Where(x => x.BranchId == BranchId).ToList();
            ViewBag.Dish = _db.CD_Dish.Where(x => x.BranchId == BranchId).ToList();
            ViewBag.Staple = _db.CD_Staple.Where(x => x.BranchId == BranchId).ToList();

            return View(Food);
        }

        // ------

        public IActionResult Delete(string Id)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;

            if(DeleteFood(Id, BranchId))
            {
                AlertTop("ลบข้อมูลเรียบร้อยแล้ว", NotificationType.success);
                return RedirectToAction("Index");
            }
            else
            {
                Alert("", "ไม่สามารถไม่ข้อมูลได้ !", NotificationType.error);
                return RedirectToAction("Index");
            }
            
        }

        // -----

        [HttpPost]
        public IActionResult Insert(CD_Food Results)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;

            if (ModelState.IsValid)
            {                
                var DT = new DataTable();
                var sql = $"SELECT * FROM CD_Food WHERE FoodId = '{Results.FoodId}' AND BranchId = '{BranchId}'";
                using (var command = _db.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    _db.Database.OpenConnection();
                    using (var data = command.ExecuteReader())
                    {
                        DT.Load(data);
                    }
                }

                if (DT.Rows.Count > 0)
                {
                    Alert("", "รหัส นี้มีอยู่แล้ว !", Enums.NotificationType.error);
                    return View();
                }
                else
                {
                    // SAVE FOOD
                    if (SaveData(Results, BranchId))
                    {
                        // SAVE FOODSUB
                        if (SaveSub(Results, BranchId))
                        {
                            // SET RUNNING
                            if (SetRunning(Results, BranchId))
                            {
                                AlertTop("บันทึกข้อมูลเรียบร้อย", Enums.NotificationType.success);
                                return RedirectToAction("Index");
                            }
                            else
                            {
                                Alert("", "Error Running !", NotificationType.error);
                                ViewBag.GroupFood = _db.CD_GroupFood.Where(x => x.BranchId == BranchId).ToList();
                                ViewBag.Dish = _db.CD_Dish.Where(x => x.BranchId == BranchId).ToList();
                                ViewBag.Staple = _db.CD_Staple.Where(x => x.BranchId == BranchId).ToList();
                                return View();
                            }
                        }
                        else
                        {
                            Alert("", "Error Food Sub !", NotificationType.error);
                            ViewBag.GroupFood = _db.CD_GroupFood.Where(x => x.BranchId == BranchId).ToList();
                            ViewBag.Dish = _db.CD_Dish.Where(x => x.BranchId == BranchId).ToList();
                            ViewBag.Staple = _db.CD_Staple.Where(x => x.BranchId == BranchId).ToList();
                            return View();
                        }                        
                    }
                    else
                    {
                        Alert("", "Error Food !", NotificationType.error);
                        ViewBag.GroupFood = _db.CD_GroupFood.Where(x => x.BranchId == BranchId).ToList();
                        ViewBag.Dish = _db.CD_Dish.Where(x => x.BranchId == BranchId).ToList();
                        ViewBag.Staple = _db.CD_Staple.Where(x => x.BranchId == BranchId).ToList();
                        return View();
                    }
                }
            }
            else
            {
                Alert("", "ไม่สามารถบันทึกข้อมูลได้ !", NotificationType.error);
                ViewBag.GroupFood = _db.CD_GroupFood.Where(x => x.BranchId == BranchId).ToList();
                ViewBag.Dish = _db.CD_Dish.Where(x => x.BranchId == BranchId).ToList();
                ViewBag.Staple = _db.CD_Staple.Where(x => x.BranchId == BranchId).ToList();
                return View();
            }
        }

        [HttpPost]
        public IActionResult Update(CD_Food Results)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            if (ModelState.IsValid)
            {           
                // UPDATE FOOD
                if (UpdateData(Results, BranchId))
                {
                    // SAVE FOODSUB
                    if (SaveSub(Results, BranchId))
                    {
                        AlertTop("แก้ไขข้อมูลเรียบร้อย", NotificationType.success);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        Alert("", "Error Food Sub !", NotificationType.error);
                        ViewBag.GroupFood = _db.CD_GroupFood.Where(x => x.BranchId == BranchId).ToList();
                        ViewBag.Dish = _db.CD_Dish.Where(x => x.BranchId == BranchId).ToList();
                        ViewBag.Staple = _db.CD_Staple.Where(x => x.BranchId == BranchId).ToList();
                        return View();
                    }                    
                }
                else {
                    Alert("", "ไม่สามารถแก้ไขข้อมูลได้ !", NotificationType.error);
                    ViewBag.GroupFood = _db.CD_GroupFood.Where(x => x.BranchId == BranchId).ToList();
                    ViewBag.Dish = _db.CD_Dish.Where(x => x.BranchId == BranchId).ToList();
                    ViewBag.Staple = _db.CD_Staple.Where(x => x.BranchId == BranchId).ToList();
                    return View();
                }           
            }
            else
            {
                Alert("", "ไม่สามารถแก้ไขข้อมูลได้ !", NotificationType.error);
                ViewBag.GroupFood = _db.CD_GroupFood.Where(x => x.BranchId == BranchId).ToList();
                ViewBag.Dish = _db.CD_Dish.Where(x => x.BranchId == BranchId).ToList();
                ViewBag.Staple = _db.CD_Staple.Where(x => x.BranchId == BranchId).ToList();
                return View();
            }
        }

        // ------

        public Boolean SaveData(CD_Food Results, string BranchId)
        {
            try
            {
                if (Results.ImageFile != null)
                {
                    // Save Image to wwwroot/
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(Results.ImageFile.FileName);
                    string extension = Path.GetExtension(Results.ImageFile.FileName);
                    Results.ImageName = fileName = fileName.Replace(fileName, Results.FoodId + "_Food") + extension;
                    string path = Path.Combine(wwwRootPath + "/Images/Food/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        Results.ImageFile.CopyTo(fileStream);
                    }
                }

                var Item = new CD_Food();
                Item.FoodId = Results.FoodId;
                Item.FoodName = Results.FoodName;
                Item.Price = Results.Price;
                Item.ImageName = Results.ImageName;
                Item.GroupFoodId = Results.GroupFoodId;
                Item.DishId = Results.DishId;

                /* DATA */
                Item.BranchId = BranchId;
                Item.CreateUser = User.Identity.Name;
                Item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                Item.UpdateUser = User.Identity.Name;
                Item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                // Insert record
                _db.CD_Food.Add(Item);
                _db.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Boolean SaveSub(CD_Food Results, string BranchId)
        {
            try
            {
                var ListFoodSub = new List<ViewFoodSub>();

                if (HttpContext.Session.GetString("Session_StapleList") != null)
                {
                    ListFoodSub = JsonConvert.DeserializeObject<List<ViewFoodSub>>(HttpContext.Session.GetString("Session_StapleList"));
                }

                if (ListFoodSub != null)
                {
                    var DT = new DataTable();
                    var sql1 = $"SELECT * FROM CD_FoodSub WHERE FoodId = '{Results.FoodId}' AND BranchId = '{BranchId}'";
                    using (var command = _db.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = sql1;
                        _db.Database.OpenConnection();
                        using (var data = command.ExecuteReader())
                        {
                            DT.Load(data);
                        }
                    }

                    if (DT.Rows.Count > 0)
                    {
                        var sql2 = $"DELETE FROM CD_FoodSub WHERE FoodId = '{Results.FoodId}' AND BranchId = '{BranchId}'";
                        using (var command = _db.Database.GetDbConnection().CreateCommand())
                        {
                            command.CommandText = sql2;
                            _db.Database.OpenConnection();
                            using (var data = command.ExecuteReader())
                            {
                                data.Read();                                
                            }                            
                        }
                    }

                    foreach (var row in ListFoodSub)
                    {
                        var Item = new CD_FoodSub();
                        Item.FoodId = Results.FoodId;
                        Item.StapleId = row.StapleId;
                        Item.Amount = row.Amount;
                        Item.UnitId = row.UnitId;
                        Item.BranchId = BranchId;
                        _db.CD_FoodSub.Add(Item);
                        _db.SaveChanges();
                    }

                    if (HttpContext.Session.GetString("Session_StapleList") != null)
                    {
                        HttpContext.Session.Remove("Session_StapleList");
                    }
                }          

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Boolean UpdateData(CD_Food Results, string BranchId)
        {
            try
            {
                var imageModel = _db.CD_Food.FirstOrDefault(x => x.FoodId == Results.FoodId && x.BranchId == BranchId);
                Results.ImageName = imageModel.ImageName;

                if (Results.ImageFile != null)
                {
                    var imagePath = Path.Combine(_hostEnvironment.WebRootPath + "/Images/Food/", imageModel.ImageName);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }

                    // Save Image to wwwroot/
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(Results.ImageFile.FileName);
                    string extension = Path.GetExtension(Results.ImageFile.FileName);
                    Results.ImageName = fileName = fileName.Replace(fileName, Results.FoodId + "_Food") + extension;
                    string path = Path.Combine(wwwRootPath + "/Images/Food/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        Results.ImageFile.CopyToAsync(fileStream);
                    }
                }

                var Item = new CD_Food();
                Item = _db.CD_Food.FirstOrDefault(x => x.FoodId == Results.FoodId && x.BranchId == BranchId);
                Item.FoodName = Results.FoodName;
                Item.DishId = Results.DishId;
                Item.Price = Results.Price;
                Item.ImageName = Results.ImageName;
                /* DATA */
                Item.UpdateUser = User.Identity.Name;
                Item.UpdateDate = DateTime.Now;

                // Update record
                _db.CD_Food.Update(Item);
                _db.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Boolean DeleteFood(string Id, string BranchId)
        {
            try
            {
                var Food = _db.CD_Food.FirstOrDefault(x => x.FoodId == Id && x.BranchId == BranchId);

                if (Food.ImageName != null)
                {
                    var imagePath = Path.Combine(_hostEnvironment.WebRootPath + "/Images/Food/", Food.ImageName);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                var DT = new DataTable();
                var sql1 = $"SELECT * FROM CD_FoodSub WHERE FoodId = '{Id}' AND BranchId = '{BranchId}'";
                using (var command = _db.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql1;
                    _db.Database.OpenConnection();
                    using (var data = command.ExecuteReader())
                    {
                        DT.Load(data);
                    }
                }

                if (DT.Rows.Count > 0)
                {
                    var sql2 = $"DELETE FROM CD_FoodSub WHERE FoodId = '{Id}' AND BranchId = '{BranchId}'";
                    using (var command = _db.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = sql2;
                        _db.Database.OpenConnection();
                        using (var data = command.ExecuteReader())
                        {
                            data.Read();
                        }
                    }
                }

                _db.CD_Food.Remove(Food);
                _db.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return true;
            }
        }

        // ------

        private List<ViewFood> GetFood()
        {
            var List = new List<ViewFood>();
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var sql = $"SELECT FoodId, FoodName, DishName, Price, GroupFoodName "
                    + $"FROM CD_Food "
                    + $"LEFT JOIN CD_GroupFood ON CD_Food.GroupFoodId = CD_GroupFood.GroupFoodId "
                    + $"LEFT JOIN CD_Dish ON CD_Food.DishId = CD_Dish.DishId "
                    + $"WHERE CD_Food.BranchId = '{BranchId}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewFood();
                        Item.FoodId = data.GetString(0);
                        Item.FoodName = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.DishName = data.GetString(2);
                        Item.Price = data.GetDecimal(3);
                        Item.GroupFoodName = data.GetString(4);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }       

               

        // Running

        public string GetRunning(string id)
        {
            string DocRunning = null;
            string RunLength = null;
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var Running = _db.CD_Running.FirstOrDefault(x => x.Name == id && x.BranchId == BranchId);
            if (id != null)
            {
                if (Running.AutoRun == true)
                {
                    for (int i = 0; i < Running.Number.Length; i++)
                    {
                        RunLength += '0';
                    }
                    int Number = Int32.Parse(Running.Number) + 1;
                    DocRunning = Running.Front + Number.ToString(RunLength);
                }
            }

            return DocRunning;
        }

        private Boolean SetRunning(CD_Food Results, string BranchId)
        {
            var Running = new CD_Running();
            Running = _db.CD_Running.FirstOrDefault(x => x.Name == Results.GroupFoodId && x.BranchId == BranchId);

            try
            {
                if (Running.AutoRun == true)
                {
                    int RunLength = Running.Number.Length;
                    Running.Number = Results.FoodId.Substring(Results.FoodId.Length - RunLength);

                    /* DATA */
                    Running.UpdateUser = User.Identity.Name;
                    Running.UpdateDate = DateTime.Now;

                    /* SAVE DB */
                    _db.CD_Running.Update(Running);
                    _db.SaveChanges();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }            
        }
    }
}
