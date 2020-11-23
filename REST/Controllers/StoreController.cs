using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using REST.Data;
using REST.Models;
using REST.Service;
using REST.ViewModels;
using REST.ApiControllers;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace REST.Controllers
{
    [Authorize]
    public class StoreController : BaseController
    {
        #region Connect db / Data System

        private readonly DbConnection _db;
        public static string _mode = Comp.FormMode.ADD;

        public StoreController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        public IActionResult Index()
        {
            _mode = Comp.FormMode.ADD;
            return View();
        }

        public IActionResult ShowData(string id)
        {
            _mode = Comp.FormMode.EDIT;
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var Trans = _db.ST_Trans.Where(x => x.Documents == id && x.BranchId == BranchId).FirstOrDefault();
            var Item = new ViewST_Trans();
            var GetView = new GetViewController(_db);
            Item.Documents = Trans.Documents;
            Item.Dates = Share.FormatDate(Trans.Dates).ToString("dd/MM/yyyy");
            Item.Description = Trans.Description;
            Item.TranSub = GetView.ViewST_TranSub(id, BranchId);
            return Json(new { data = Item });
        }

        public IActionResult ShowTable(string id)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var Item = _db.ST_Trans.Where(x => x.Documents == id && x.BranchId == BranchId).FirstOrDefault();            
            return Json(new { data = Item });
        }

        public IActionResult Add(string id)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var List = _db.CD_Staple.FirstOrDefault(x => x.StapleId == id && x.BranchId == BranchId);
            return Json(new { data = List });
        }

        public IActionResult Save(string h, string d)
        {
            string Status = null;
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            /* Data */
            dynamic Header = JsonConvert.DeserializeObject(h);
            dynamic Detail = JsonConvert.DeserializeObject(d);

            switch (_mode)
            {
                case Comp.FormMode.ADD:

                    /* Check Id Table */
                    string Document = Header.Document;
                    var c = _db.ST_Trans.Where(x => x.Documents == Document && x.BranchId == BranchId).ToList();
                    if (c.Count > 0)
                    {
                        Alert("", "เลขที่เอกสารนี้มีอยู่แล้ว", Enums.NotificationType.error);
                        Status = "error id";
                    }
                    else
                    {
                        /* Header Data Input */
                        if (SaveHeader(Header, Detail, BranchId))
                        {
                            toastrAlert("Save Data", "บันทึกข้อมูลเรียบร้อย", Enums.NotificationToastr.success);
                            Status = "success";
                        }
                        else
                        {
                            Status = "error";
                        }
                    }

                    break;

                case Comp.FormMode.EDIT:

                    /* Header Data Input */
                    if (UpdateHeader(Header, Detail, BranchId))
                    {
                        toastrAlert("Save Data", "บันทึกข้อมูลเรียบร้อย", Enums.NotificationToastr.success);
                        Status = "success";
                    }
                    else
                    {
                        Status = "error";
                    }

                    break;
            }

            return Json(new { data = Status });
        }

        public Boolean SaveHeader(dynamic Header, dynamic Detail, string BranchId)
        {            
            try
            {
                string Running = null;
                Running = GetRunning("Store");
                if (Running != null)
                    Header.Document = Running;

                /* Input Data */
                string Document = Header.Document;
                string Dates = Header.Date;
                DateTime m = Share.FormatDate(Dates);
                string Description = Header.Description;                

                var Item = new ST_Trans();
                Item.Documents = Document;
                Item.Dates = m;
                Item.Description = Description;
                /* DATA */
                Item.BranchId = BranchId;
                Item.CreateUser = User.Identity.Name;
                Item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                Item.UpdateUser = User.Identity.Name;
                Item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                _db.ST_Trans.Add(Item);
                _db.SaveChanges();

                /* Description Data Input */
                if (SaveDetail(Document, Detail, BranchId))
                {
                    /* Set Running */
                    SetRunning("Store", Document, BranchId);

                    /* Update Amount Data Staple */
                    if (UpdateAmountStaple(Detail, BranchId))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }                
            }
            catch (Exception)
            {
                return false;
            }            
        }

        public Boolean UpdateHeader(dynamic Header, dynamic Detail, string BranchId)
        {
            try
            {
                string id = Header.Document;                

                /* Input Data */
                string Document = Header.Document;
                string Dates = Header.Date;
                DateTime m = Share.FormatDate(Dates);
                string Description = Header.Description;

                // Update Data
                var Item = new ST_Trans();
                Item = _db.ST_Trans.Where(x => x.BranchId == BranchId && x.Documents == id).FirstOrDefault();
                Item.Dates = m;
                Item.Description = Description;
                /* DATA */
                Item.BranchId = BranchId;
                Item.UpdateUser = User.Identity.Name;
                Item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                _db.ST_Trans.Update(Item);
                _db.SaveChanges();

                /* Description Data Input */
                if (SaveDetail(Document, Detail, BranchId))
                {
                    /* Update Amount Data Staple */
                    if (UpdateAmountStaple(Detail, BranchId))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Boolean SaveDetail(string Document, dynamic Detail, string BranchId)
        {
            try
            {

                /* Check Id Table */
                var z = _db.ST_TranSub.Where(x => x.Documents == Document && x.BranchId == BranchId).ToList();

                if (_mode == Comp.FormMode.EDIT)
                {
                    foreach (var item in z)
                    {
                        string id = item.StapleId;
                        decimal qty = item.Amount;

                        var Item = new CD_Staple();
                        Item = _db.CD_Staple.FirstOrDefault(x => x.StapleId == id && x.BranchId == BranchId);
                        var a = Item.QtyBalance - qty;

                        Item.QtyBalance = a;
                        /* DATA */
                        Item.UpdateUser = User.Identity.Name;
                        Item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                        _db.CD_Staple.Update(Item);
                        _db.SaveChanges();
                    }                    
                }

                if (z.Count > 0)
                {     

                    _db.ST_TranSub.RemoveRange(z);
                }

                int i = 1;

                /* Input Data */
                foreach (dynamic item in Detail)
                {
                    string id = item.id;
                    decimal qty = item.qty;
                    decimal price = item.price;
                    decimal total = item.total;

                    var Item = new ST_TranSub();
                    Item.Documents = Document;
                    Item.StapleId = id;
                    Item.Amount = qty;
                    Item.Price = price;
                    Item.Total = total;
                    Item.i = i;
                    /* DATA */
                    Item.BranchId = BranchId;

                    _db.ST_TranSub.Add(Item);
                    _db.SaveChanges();

                    i++;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Boolean UpdateAmountStaple(dynamic Detail, string BranchId)
        {
            try
            {
                foreach (dynamic item in Detail)
                {
                    string id = item.id;
                    decimal qty = item.qty;
                    decimal price = item.price;
                    decimal total = item.total;

                    /* Data Before */
                    var before = _db.CD_Staple.FirstOrDefault(x => x.StapleId == id && x.BranchId == BranchId);
                    var a = before.QtyBalance + qty;

                    var Item = new CD_Staple();
                    Item = _db.CD_Staple.FirstOrDefault(x => x.StapleId == id && x.BranchId == BranchId);

                    Item.QtyBalance = a;
                    /* DATA */
                    Item.UpdateUser = User.Identity.Name;
                    Item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                    _db.CD_Staple.Update(Item);
                    _db.SaveChanges();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }            
        }

        public string GetRunning(string id)
        {
            string DocRunning = null;
            string RunLength = null;
            int Number = 0;
            DateTime Date = Share.FormatDate(DateTime.Now).Date;
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var Running = _db.CD_Running.FirstOrDefault(x => x.Name == id && x.BranchId == BranchId);
            try
            {
                if (id != null)
                {
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
            }
            catch (Exception)
            {
                throw;
            }           

            return DocRunning;
        }

        public Boolean SetRunning(string Name, string DocRunning, string BranchId)
        {
            var Running = new CD_Running();
            Running = _db.CD_Running.FirstOrDefault(x => x.Name == Name && x.BranchId == BranchId);
            try
            {
                if (Running.AutoRun == true)
                {
                    int RunLength = Running.Number.Length;
                    Running.Number = DocRunning.Substring(DocRunning.Length - RunLength);

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
