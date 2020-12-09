using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REST.Data;
using REST.Models;
using REST.Service;
using REST.ViewModels;

namespace REST.Controllers
{
    [Authorize]
    public class SettingController : BaseController
    {
        #region Connect db / Data System
        private readonly DbConnection _db;
        public static string _mode = Comp.FormMode.ADD;

        public SettingController(DbConnection db)
        {
            _db = db;
        }
        #endregion

        public IActionResult Index()
        {
            var info = new ViewSetting();
            var item = _db.Setting.FirstOrDefault();
            info.TaxSell = item.TaxSell;
            info.TaxBuy = item.TaxBuy;
            info.Service = item.Service;
            info.PointNumber = Share.Cnumber(Share.FormatDouble(item.Point), 2);
            info.Perbunch = item.Perbunch;
            return View(info);
        }

        [HttpPost]
        public IActionResult Index(ViewSetting info)
        {
            if (SaveData(info))
            {
                toastrAlert("ตั้งค่าระบบ", "บันทึกข้อมูลเรียบร้อย", Enums.NotificationToastr.success);
                return RedirectToAction("Index");
            }
            else
            {
                Alert("", "ไม่สามารถบันทึกข้อมูลได้ !", Enums.NotificationType.error);
                return View();
            }
        }

        public Boolean SaveData(ViewSetting info)
        {
            var item = new Setting();
            try
            {
                item = _db.Setting.FirstOrDefault();
                item.TaxSell = info.TaxSell;
                item.TaxBuy = info.TaxBuy;
                item.Service = info.Service;
                item.Point = Share.FormatDecimal(info.PointNumber);
                item.Perbunch = info.Perbunch;

                _db.Setting.Update(item);
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
