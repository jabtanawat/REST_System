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
    public class SF_DailyController : BaseController
    {
        #region Connect db
        private readonly DbConnection _db;

        public SF_DailyController(DbConnection db)
        {
            _db = db;
        }
        #endregion 

        public IActionResult FrmDailyOrder()
        {
            var branchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var _Order = new GetSF_OrderController(_db);
            ViewBag.DataTable = _Order.Order(null, DateTime.Now.ToString("dd/MM/yyyy"), branchId);
            return View();
        }

        public IActionResult FrmDailyOrderSub(string id)
        {
            var branch = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var _Order = new GetSF_OrderController(_db);
            ViewBag.Data = _Order.OrderSub(null, id, branch);
            ViewBag.Desc = _Order.OrderById(id, branch);
            return View();
        }

        public IActionResult FrmDailyBill()
        {
            var branchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var _bill = new GetSF_BillController(_db);
            ViewBag.Table = _bill.Bill(null, DateTime.Now.ToString("dd/MM/yyyy"), branchId);
            return View();
        }

        // -------------------------------------------
        // -------------------------------------------
        // ---                                     ---
        // ---        FUNCTION DAILY ORDER         ---
        // ---                                     ---
        // -------------------------------------------
        // -------------------------------------------

        public IActionResult removeOrder(string OrderId, string FoodId, string TableId)
        {
            // สาขา
            var branchid = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;

            var i_OrderSub = _db.SF_OrderSub.FirstOrDefault(x => x.OrderId == OrderId && x.FoodId == FoodId && x.BranchId == branchid);
            // ตรวจสอบสถานะออเดอร์
            if(i_OrderSub.Status == 1)
            {
                _db.SF_OrderSub.Remove(i_OrderSub);
                _db.SaveChanges();

                var L_OrderSub = _db.SF_OrderSub.Where(x => x.OrderId == OrderId && x.BranchId == branchid).ToList();
                // ตรวจสอบออเดอร์ย่อยว่ามีไหม ถ้าไม่มีออเดอร์ย่อยให้ทำการลบออเดอร์หลัก
                if(L_OrderSub.Count > 0)
                {
                    toastrAlert("ออดอเร์รายวัน", "ยกเลิกเรียบร้อย", Enums.NotificationToastr.success);
                    return RedirectToAction("FrmDailyOrderSub", "SF_DailyOrder", new { id = OrderId });
                }
                else
                {
                    var i_Order = _db.SF_Order.FirstOrDefault(x => x.OrderId == OrderId && x.BranchId == branchid);
                    _db.SF_Order.Remove(i_Order);
                    _db.SaveChanges();

                    var i_OrderTable = _db.SF_Order.Where(x => x.TableId == TableId && x.BranchId == branchid).ToList();
                    // ตรวจสอบข้อมูลโต๊ะว่ามีออเดอร์ไหม ถ้าไม่มีให้เปลี่ยนสถานะโต๊ะเป็นว่าง 
                    if(i_OrderTable.Count > 0)
                    {
                        toastrAlert("ออดอเร์รายวัน", "ยกเลิกเรียบร้อย", Enums.NotificationToastr.success);
                        return RedirectToAction("FrmDailyOrderSub", "SF_DailyOrder", new { id = OrderId });
                    }
                    else
                    {
                        var Item = new CD_Table();
                        Item = _db.CD_Table.FirstOrDefault(x => x.TableId == TableId && x.BranchId == branchid);
                        Item.TableST = 1;
                        _db.CD_Table.Update(Item);
                        _db.SaveChanges();

                        toastrAlert("ออดอเร์รายวัน", "ยกเลิกเรียบร้อย", Enums.NotificationToastr.success);
                        return RedirectToAction("FrmDailyOrder", "SF_DailyOrder");
                    }                    
                }
            }
            else
            {
                Alert("", "ไม่สามารถยกเลิกได้ เนื่องจากรายการนี้ทำแล้ว", Enums.NotificationType.warning);
                return RedirectToAction("FrmDailyOrderSub", "SF_DailyOrder", new { id = OrderId });
            }                               
        }
    }
}
