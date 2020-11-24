using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REST.ApiControllers;
using REST.Data;
using REST.Models;
using REST.Service;

namespace REST.Controllers
{
    [Authorize]
    public class MB_ProfileController : BaseController
    {
        #region Connect db / Data System

        private readonly DbConnection _db;
        public static string _mode = Comp.FormMode.ADD;
        public MB_ProfileController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        public IActionResult Index()
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            var _Get = new GetViewController(_db);
            ViewBag.MB_Profile = _Get.ViewMember(null);
            return View();
        }

        public IActionResult FrmProfile(string mode, string id = null)
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
                var Item = _db.MB_Member.FirstOrDefault(x => x.MemberId == id && x.BranchId == branchid);
                var data = info(Item);
                return View(data);
            }
        }

        [HttpPost]
        public IActionResult FrmProfile(MB_Member Info)
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            if (ModelState.IsValid)
            {
                if (SaveData(Info, branchid))
                {
                    toastrAlert("ทะเบียนสมาชิก", "บันทึกข้อมูลเรียบร้อย", Enums.NotificationToastr.success);
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

        public Boolean SaveData(MB_Member Info, string branchid)
        {
            var Item = new MB_Member();
            var _Get = new GetRunningController(_db);
            string Doc = null;
            try
            {
                switch (_mode)
                {
                    case Comp.FormMode.ADD:

                        var IsNull = _db.MB_Member.Where(x => x.MemberId == Info.MemberId && x.BranchId == branchid).ToList();
                        if (IsNull.Count > 0)
                        {
                            Alert("", "รหัสนี้มีอยู่แล้ว !", Enums.NotificationType.warning);
                        }
                        else
                        {
                            /* Get Running */
                            Doc = _Get.Running("Member", branchid);
                            if (Doc == null)
                                Doc = Info.MemberId;

                            Item.MemberId = Doc;
                            Item.Type = Info.Type;
                            Item.DateRegister = Share.FormatDate(Info.DateRegister).Date;
                            Item.IdCard = Info.IdCard;
                            Item.Title = Info.Title;
                            Item.FirstName = Info.FirstName;
                            Item.LastName = Info.LastName;
                            Item.DateBirthday = Info.DateBirthday;
                            Item.Status = Info.Status;
                            Item.PhoneNumber = Info.PhoneNumber;
                            Item.Email = Info.Email;
                            Item.AddrNo = Info.AddrNo;
                            Item.Moo = Info.Moo;
                            Item.Road = Info.Road;
                            Item.Soi = Info.Soi;
                            Item.Locality = Info.Locality;
                            Item.District = Info.District;
                            Item.Province = Info.Province;
                            Item.ZibCode = Info.ZibCode;
                            Item.Rebate = Info.Rebate;                            
                            /* DATA */
                            Item.BranchId = branchid;
                            Item.CreateUser = User.Identity.Name;
                            Item.CreateDate = Share.FormatDate(DateTime.Now).Date;
                            Item.UpdateUser = User.Identity.Name;
                            Item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                            _db.MB_Member.Add(Item);
                            _db.SaveChanges();

                            // Set Running
                            _Get.SetRunning("Member", Doc, branchid);
                        }

                        break;

                    case Comp.FormMode.EDIT:
                        Item = _db.MB_Member.FirstOrDefault(x => x.MemberId == Info.MemberId && x.BranchId == branchid);
                        Item.Type = Info.Type;
                        Item.IdCard = Info.IdCard;
                        Item.Title = Info.Title;
                        Item.FirstName = Info.FirstName;
                        Item.LastName = Info.LastName;
                        Item.DateBirthday = Info.DateBirthday;
                        Item.Status = Info.Status;
                        Item.PhoneNumber = Info.PhoneNumber;
                        Item.Email = Info.Email;
                        Item.AddrNo = Info.AddrNo;
                        Item.Moo = Info.Moo;
                        Item.Road = Info.Road;
                        Item.Soi = Info.Soi;
                        Item.Locality = Info.Locality;
                        Item.District = Info.District;
                        Item.Province = Info.Province;
                        Item.ZibCode = Info.ZibCode;
                        Item.Rebate = Info.Rebate;
                        /* DATA */
                        Item.BranchId = branchid;
                        Item.UpdateUser = User.Identity.Name;
                        Item.UpdateDate = Share.FormatDate(DateTime.Now).Date;

                        _db.MB_Member.Update(Item);
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

        [HttpPost]
        public IActionResult Delete(MB_Member Info)
        {

            string branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            try
            {
                var Item = _db.MB_Member.FirstOrDefault(x => x.MemberId == Info.MemberId && x.BranchId == branchid);
                _db.MB_Member.Remove(Item);
                _db.SaveChanges();

                toastrAlert("ทะเบียนสมาชิก", "ลบข้อมูลเรียบร้อยแล้ว", Enums.NotificationToastr.success);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Alert("", "ไม่สามารถลบข้อมูลได้", Enums.NotificationType.warning);
                return RedirectToAction("FrmProfile", new { mode = "Edit", id = Info.MemberId });
            }
        }

        public void FrmMode()
        {
            var branchid = User.Claims.FirstOrDefault(b => b.Type == "BranchId").Value;
            if (_mode == Comp.FormMode.ADD)
            {
                // Running
                var _Get = new GetRunningController(_db);
                string DocRunning = _Get.Running("Member", branchid);
                ViewData["mode"] = _mode;
                ViewData["Disible-delete"] = "disabled";
                ViewData["Disible-save"] = "";
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
                ViewData["DateNow"] = DateTime.Now.ToString("dd/MM/yyyy");
                ViewBag.Branch = _db.MG_Branch.ToList();
                ViewBag.CD_Title = _db.CD_Title.Where(x => x.BranchId == branchid).ToList();
            }
            else
            {
                ViewData["mode"] = _mode;
                ViewData["Disible"] = "";
                ViewData["Disible-save"] = "disabled";
                ViewData["Readonly"] = "readonly";
                ViewBag.Branch = _db.MG_Branch.ToList();
                ViewBag.CD_Title = _db.CD_Title.Where(x => x.BranchId == branchid).ToList();
            }
        }

        public ViewMB_Member info(MB_Member info)
        {
            var item = new ViewMB_Member();

            item.MemberId = info.MemberId;
            item.Type = info.Type;
            item.DateRegister = info.DateRegister.ToString("dd/MM/yyyy");
            item.IdCard = info.IdCard;
            item.Title = info.Title;
            item.FirstName = info.FirstName;
            item.LastName = info.LastName;
            item.DateBirthday = info.DateBirthday.ToString("dd/MM/yyyy");
            item.Status = info.Status;
            item.PhoneNumber = info.PhoneNumber;
            item.Email = info.Email;
            item.AddrNo = info.AddrNo;
            item.Moo = info.Moo;
            item.Road = info.Road;
            item.Soi = info.Soi;
            item.Locality = info.Locality;
            item.District = info.District;
            item.Province = info.Province;
            item.ZibCode = info.ZibCode;
            item.Rebate = info.Rebate;
            item.Score = Share.Cnumber(Share.FormatDouble(info.Score), 2);

            return item;
        }

    }
}
