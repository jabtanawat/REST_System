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
    public class ST_StoreController : BaseController
    {
        #region Connect db / Data System
        private readonly DbConnection _db;
        public static string _mode = Comp.FormMode.ADD;

        public ST_StoreController(DbConnection db)
        {
            _db = db;
        }
        #endregion 

        public IActionResult Index()
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            ViewBag.Trans = 
            return View();
        }

        public IActionResult FrmStore(string mode, string id = null)
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            if (mode == "Add")
            {
                _mode = Comp.FormMode.ADD;
                FrmMode();
                var item = new ViewST_Trans();

                item.DateDocument = DateTime.Now.ToString("dd/MM/yyyy");
                return View(item);
            }
            else
            {
                _mode = Comp.FormMode.EDIT;
                FrmMode();
                //var item = LoadData(id, branchid);
                return View();
            }
        }

        [HttpPost]
        public IActionResult FrmSave(ViewST_Trans info)
        {
            string status = "success";

            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;

            if (SaveData(info, branchid))
            {
                toastrAlert("บันทึกการสั่งซื้อ", "บันทึกข้อมูลเรียบร้อย", Enums.NotificationToastr.success);
                status = "success";
            }
            else
            {
                status = "error";
            }

            return Json(new { data = status });
        }

        public Boolean SaveData(ViewST_Trans info, string branchid)
        {
            var item = new ST_Trans();
            var _running = new GetRunningController(_db);
            dynamic tranSub = JsonConvert.DeserializeObject(info.Sub);
            try
            {
                switch (_mode)
                {
                    case Comp.FormMode.ADD:

                        // check data count trans
                        var IsNull = _db.ST_Trans.Where(x => x.Documents == info.Document && x.BranchId == branchid).ToList();
                        if (IsNull.Count > 0)
                        {
                            return false;
                        }
                        else
                        {
                            // Save Trans

                            item.Documents = info.Document;
                            item.DateDocument = Share.FormatDate(info.DateDocument).Date;
                            item.DateTax = Share.FormatDate(info.DateTax).Date;
                            item.TaxNumber = info.TaxNumber;
                            item.SupplierId = info.SupplierId;
                            item.Reference = info.Reference;
                            item.SumBalance = Share.FormatDecimal(info.SumBalance);
                            /* DATA */
                            item.BranchId = branchid;
                            item.CreateUser = User.Identity.Name;
                            item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                            item.UpdateUser = User.Identity.Name;
                            item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                            _db.ST_Trans.Add(item);
                            _db.SaveChanges();

                            // Save TranSub
                            if (tranSub.Count > 0)
                            {
                                int i = 1;
                                /* Input Data */
                                foreach (dynamic result in tranSub)
                                {
                                    string id = result.StapleId;
                                    decimal amount = result.Amount;
                                    decimal price = result.Price;
                                    decimal total = result.Total;

                                    var sub = new ST_TranSub();
                                    sub.Documents = info.Document;
                                    sub.StapleId = id;
                                    sub.Amount = amount;
                                    sub.Price = price;
                                    sub.Total = total;
                                    sub.i = i;
                                    /* DATA */
                                    sub.BranchId = branchid;

                                    _db.ST_TranSub.Add(sub);
                                    _db.SaveChanges();

                                    i++;
                                }
                            }

                            // Set Runnig
                            _running.SetRunning("Store", info.Document, branchid);
                        }

                        break;

                    case Comp.FormMode.EDIT:

                        item = _db.ST_Trans.FirstOrDefault(x => x.Documents == info.Document);

                        // Save Trans                     
                        item.DateTax = Share.FormatDate(info.DateTax).Date;
                        item.TaxNumber = info.TaxNumber;
                        item.SupplierId = info.SupplierId;
                        item.Reference = info.Reference;
                        item.SumBalance = Share.FormatDecimal(info.SumBalance);
                        /* DATA */
                        item.UpdateUser = User.Identity.Name;
                        item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                        _db.ST_Trans.Update(item);
                        _db.SaveChanges();

                        // Save FoodSub
                        if (tranSub.Count > 0)
                        {
                            var delete = _db.ST_TranSub.Where(x => x.Documents == info.Document).ToList();
                            _db.ST_TranSub.RemoveRange(delete);
                            _db.SaveChanges();

                            int i = 1;

                            /* Input Data */
                            foreach (dynamic result in tranSub)
                            {
                                string id = result.StapleId;
                                decimal amount = result.Amount;
                                decimal price = result.Price;
                                decimal total = result.Total;

                                var sub = new ST_TranSub();
                                sub.Documents = info.Document;
                                sub.StapleId = id;
                                sub.Amount = amount;
                                sub.Price = price;
                                sub.Total = total;
                                sub.i = i;
                                /* DATA */
                                sub.BranchId = branchid;

                                _db.ST_TranSub.Add(sub);
                                _db.SaveChanges();

                                i++;
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
                // Running
                var _Get = new GetRunningController(_db);
                string DocRunning = _Get.Running("Store", branchid);
                if (DocRunning != null)
                {
                    ViewData["Readonly"] = "readonly";
                    ViewData["DocRunning"] = DocRunning;
                }
                else
                {
                    ViewData["Readonly"] = "";
                    ViewData["DocRunning"] = "";
                }
            }
            else
            {
                ViewData["Disible-delete"] = "";
                ViewData["Disible-save"] = "disabled";
                ViewData["Readonly"] = "readonly";
                ViewData["DocRunning"] = "";
            }
        }






        public IActionResult ShowData(string id)
        {
            _mode = Comp.FormMode.EDIT;
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var Trans = _db.ST_Trans.Where(x => x.Documents == id && x.BranchId == BranchId).FirstOrDefault();
            var Item = new ViewST_Trans();
            var GetView = new GetViewController(_db);
            //Item.Documents = Trans.Documents;
            //Item.Dates = Share.FormatDate(Trans.Dates).ToString("dd/MM/yyyy");
            //Item.Description = Trans.Description;
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
                //Item.Dates = m;
                //Item.Description = Description;
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
                //Item.Dates = m;
                //Item.Description = Description;
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
                        //var a = Item.QtyBalance - qty;

                        //Item.QtyBalance = a;
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
                    //var a = before.QtyBalance + qty;

                    var Item = new CD_Staple();
                    Item = _db.CD_Staple.FirstOrDefault(x => x.StapleId == id && x.BranchId == BranchId);

                    //Item.QtyBalance = a;
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
