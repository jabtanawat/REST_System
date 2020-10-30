using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class OrderController : BaseController
    {
        #region Connection db
        private readonly DbConnection _db;

        public OrderController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        [Route("/Order/{id}")]
        public IActionResult Index(string id)
        {
            var data = TableData(id);

            var Item = new ViewTable();
            foreach (var row in data)
            {
                Item.TableId = row.TableId;
                Item.TableName = row.TableName;
                Item.Description = row.Description;
                Item.TableST = row.TableST;
                Item.Status = row.Status;
            }

            ViewBag.DataTable = Item;
            ViewBag.SL_GroupFood = SL_GroupFood();

            if (HttpContext.Session.GetString("Session_ListOrder") != null)
            {
                HttpContext.Session.Remove("Session_ListOrder");
            }

            return View();
        }

        public IActionResult SaveOrder(string id)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;

            var ListOrder = new List<ViewOrder>();

            if (HttpContext.Session.GetString("Session_ListOrder") != null)
            {
                ListOrder = JsonConvert.DeserializeObject<List<ViewOrder>>(HttpContext.Session.GetString("Session_ListOrder"));
            }

            if (ListOrder.Count > 0)
            {
                if (SaveData(ListOrder, id, BranchId))
                {
                    if (UpdateTable(id, BranchId))
                    {
                        AlertTop("บันทึกรายการอาหารเรียบร้อย !", NotificationType.success);
                        return RedirectToAction("Index", "StoreFront");
                    }
                    else
                    {
                        Alert("Error", "ไม่สามารถเปลี่ยนสถานะโต๊ะได้ !", NotificationType.error);
                        return RedirectToAction("Index", new { id = id });
                    }                    
                }
                else
                {
                    Alert("Error", "ไม่สามารถบันทึกรายการอาหารได้ !", NotificationType.error);
                    return RedirectToAction("Index", new { id = id });
                }                
            }
            else
            {
                Alert("Warning", "กรุณาเลือกรายการอาหาร !", NotificationType.warning);
                return RedirectToAction("Index", new { id = id });
            }
        }

        public Boolean SaveData(List<ViewOrder> Results, string id, string BranchId)
        {
            try
            {
                string DocRunning = GetRunning("Order");

                int i = 0;
                decimal PTotal = 0;

                var DT = new DataTable();
                var sql = $"SELECT * FROM SF_Order WHERE TableId = '{id}' AND BranchId = '{BranchId}'";
                using (var command = _db.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    _db.Database.OpenConnection();
                    using (var data = command.ExecuteReader())
                    {
                        DT.Load(data);
                    }
                }

                i = DT.Rows.Count + 1;

                // หาจำนวนเงินทั่้งหมด
                foreach (var row in Results)
                {
                    PTotal += row.Price * row.Amount;
                }

                // Save Order
                var Item = new SF_Order();
                Item.OrderId = DocRunning;
                Item.TableId = id;
                Item.OrderDt = Share.FormatDate(DateTime.Now).Date;
                Item.PriceTotal = PTotal;
                Item.BranchId = BranchId;
                Item.CreateUser = User.Identity.Name;
                Item.CreateDate = DateTime.Now;
                Item.UpdateUser = User.Identity.Name;
                Item.UpdateDate = DateTime.Now;
                _db.SF_Order.Add(Item);
                _db.SaveChanges();

                //Svae Order Sub
                foreach (var row in Results)
                {
                    var ItemSub = new SF_OrderSub();
                    ItemSub.OrderId = DocRunning;
                    ItemSub.i = i;
                    ItemSub.TableId = id;
                    ItemSub.FoodId = row.FoodId;
                    ItemSub.Amount = row.Amount;
                    ItemSub.Price = row.Price;
                    ItemSub.BranchId = BranchId;
                    _db.SF_OrderSub.Add(ItemSub);
                    _db.SaveChanges();
                }

                SetRunning("Order", DocRunning, BranchId);

                if (HttpContext.Session.GetString("Session_ListOrder") != null)
                {
                    HttpContext.Session.Remove("Session_ListOrder");
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Boolean UpdateTable(string id, string BranchId)
        {
            try
            {
                var Item = new CD_Table();
                Item = _db.CD_Table.FirstOrDefault(x => x.TableId == id && x.BranchId == BranchId);

                Item.TableST = 2;

                _db.CD_Table.Update(Item);
                _db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // ------

        public List<ViewTable> TableData(string Id)
        {
            var List = new List<ViewTable>();

            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var sql = $"SELECT TableId, TableName, Description, TableST, CASE WHEN TableST = 1 THEN 'ว่าง' WHEN TableST = 2 THEN 'ไม่ว่าง' WHEN TableST= 3 THEN 'จอง' END AS Status "
                    + $"FROM CD_Table "
                    + $"WHERE BranchId = '{BranchId}' AND TableId = '{Id}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewTable();
                        Item.TableId = data.GetString(0);
                        Item.TableName = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.Description = data.GetString(2);
                        Item.TableST = data.GetInt32(3);
                        Item.Status = data.GetString(4);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }

        public List<CD_GroupFood> SL_GroupFood()
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var List = new List<CD_GroupFood>();
            var sql = $"SELECT GroupFoodId, GroupFoodName FROM CD_GroupFood "
                    + $"WHERE BranchId = '{BranchId}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new CD_GroupFood();
                        Item.GroupFoodId = data.GetString(0);
                        Item.GroupFoodName = data.GetString(1);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }        

        [HttpPost]
        public IActionResult GetFood(string id)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var List = new List<ViewFood>();
            var sql = $"SELECT FoodId, FoodName, Price, GroupFoodId, ImageName "
                    + $"FROM CD_Food "
                    + $"WHERE BranchId = '{BranchId}' ";
            if (id != null)
                sql += $"AND GroupFoodId = '{id}'";

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
                        Item.Price = data.GetDecimal(2);
                        Item.GroupFoodId = data.GetString(3);
                        if (!data.IsDBNull(4))
                            Item.ImageName = data.GetString(4);
                        List.Add(Item);
                    }
                }
            }

            return Json(new { data = List });
        }

        // ------

        [HttpPost]
        public IActionResult AddOrder(string id)
        {            
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId").Value;
            var ListOrder = new List<ViewOrder>();

            if (id != null)
            {
                var Food = new CD_Food();
                Food = _db.CD_Food.FirstOrDefault(x => x.FoodId == id && x.BranchId == BranchId);               

                if (HttpContext.Session.GetString("Session_ListOrder") != null)
                {
                    ListOrder = JsonConvert.DeserializeObject<List<ViewOrder>>(HttpContext.Session.GetString("Session_ListOrder"));
                }

                if (ListOrder.Exists(x => x.FoodId == id))
                {
                    ListOrder.Find(x => x.FoodId == id).Amount++;
                }
                else
                {
                    var item = new ViewOrder
                    {
                        FoodId = id,
                        FoodName = Food.FoodName,
                        Price = Food.Price,
                        Amount = 1,
                    };

                    ListOrder.Add(item);
                }

                HttpContext.Session.SetString("Session_ListOrder", JsonConvert.SerializeObject(ListOrder));
            }
            else
            {
                Alert("แจ้งเตือน", "กรุณาเลือกวัตถุดิบ !", NotificationType.warning);
            }

            return Json(new { data = ListOrder });
        }

        [HttpPost]
        public IActionResult DeleteOrder(string id)
        {
            var ListOrder = new List<ViewOrder>();

            if (HttpContext.Session.GetString("Session_ListOrder") != null)
            {
                ListOrder = JsonConvert.DeserializeObject<List<ViewOrder>>(HttpContext.Session.GetString("Session_ListOrder"));
            }

            if (ListOrder.Exists(x => x.FoodId == id))
            {
                var Delete = ListOrder.Find(x => x.FoodId == id);
                ListOrder.Remove(Delete);
            }

            HttpContext.Session.SetString("Session_ListOrder", JsonConvert.SerializeObject(ListOrder));

            return Json(new { data = ListOrder });
        }

        [HttpPost]
        public IActionResult CancelOrder()
        {
            string Status = null;

            if (HttpContext.Session.GetString("Session_ListOrder") != null)
            {
                HttpContext.Session.Remove("Session_ListOrder");
                Status = "success";
            }

            return Json(new { data = Status });
        }

        [HttpPost]
        public IActionResult GetOrder()
        {
            var ListOrder = new List<ViewOrder>();

            if (HttpContext.Session.GetString("Session_ListOrder") != null)
            {
                ListOrder = JsonConvert.DeserializeObject<List<ViewOrder>>(HttpContext.Session.GetString("Session_ListOrder"));
            }                

            return Json(new { data = ListOrder });
        }

        // ------

        public string GetRunning(string id)
        {
            string DocRunning = null;
            string RunLength = null;
            int Number = 0;
            DateTime Date = DateTime.Now;
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var Running = _db.CD_Running.FirstOrDefault(x => x.Name == id && x.BranchId == BranchId);

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

        // ------

        [HttpPost]
        public IActionResult DeleteOrderPayment(string OrderId, string TableId, string FoodId)
        {
            string Status = null;

            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;

            // Delete OrderSub By Id
            var OrderSubDelete = _db.SF_OrderSub.FirstOrDefault(x => x.OrderId == OrderId && x.FoodId == FoodId && x.BranchId == BranchId);

            if(OrderSubDelete.Status == 1 || OrderSubDelete.Status == 2){
                Alert("Error", "ไม่สามารถลบได้ อาหารกำลังดำเนินการหรืออาหารเสร็จแล้ว !", NotificationType.error);
                Status = "success";
            }
            else
            {
                _db.SF_OrderSub.Remove(OrderSubDelete);
                _db.SaveChanges();

                var DT1 = new DataTable();
                var sql_Order = $"SELECT * FROM SF_OrderSub WHERE OrderId = '{OrderId}' AND BranchId = '{BranchId}'";
                using (var command = _db.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql_Order;
                    _db.Database.OpenConnection();
                    using (var data = command.ExecuteReader())
                    {
                        DT1.Load(data);
                    }
                }

                if (DT1.Rows.Count > 0)
                {
                    var Item_Order = new SF_Order();
                    Item_Order = _db.SF_Order.FirstOrDefault(x => x.OrderId == OrderId && x.BranchId == BranchId);

                    // หาข้อมูล OrderSub
                    var DataOrderSub = GetOrderSub(OrderId, TableId);

                    decimal PTotal = 0;

                    // หาจำนวนเงินทั่้งหมด
                    foreach (var row in DataOrderSub)
                    {
                        PTotal += row.Price * row.Amount;
                    }

                    // Update Order
                    Item_Order.PriceTotal = PTotal;
                    Item_Order.UpdateUser = User.Identity.Name;
                    Item_Order.UpdateDate = DateTime.Now;

                    _db.SF_Order.Update(Item_Order);
                    _db.SaveChanges();

                    Status = "success";
                }
                else
                {
                    var OrderDelete = _db.SF_Order.FirstOrDefault(x => x.OrderId == OrderId && x.BranchId == BranchId);
                    _db.SF_Order.Remove(OrderDelete);
                    _db.SaveChanges();

                    var DT2 = new DataTable();
                    var sql_2 = $"SELECT * FROM SF_Order WHERE TableId = '{TableId}' AND BranchId = '{BranchId}'";
                    using (var command = _db.Database.GetDbConnection().CreateCommand())
                    {
                        command.CommandText = sql_2;
                        _db.Database.OpenConnection();
                        using (var data = command.ExecuteReader())
                        {
                            DT2.Load(data);
                        }
                    }

                    if (DT2.Rows.Count <= 0)
                    {
                        var Item = new CD_Table();
                        Item = _db.CD_Table.FirstOrDefault(x => x.TableId == TableId && x.BranchId == BranchId);

                        Item.TableST = 1;

                        _db.CD_Table.Update(Item);
                        _db.SaveChanges();

                        Status = "error";
                    }
                    else
                    {
                        Status = "success";
                    }
                }
            }            

            return Json(new { data = Status });
        }

        public List<ViewOrder> GetOrderSub(string OrderId, string TableId)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;

            var List = new List<ViewOrder>();
            var sql = $"SELECT OrderId, SF_OrderSub.i, TableId, CD_Food.FoodId, CD_Food.FoodName, Amount, CD_Food.Price, Status "
                    + $"FROM SF_OrderSub "
                    + $"LEFT JOIN CD_Food ON SF_OrderSub.FoodId = CD_Food.FoodId "
                    + $"WHERE OrderId = '{OrderId}' AND TableId = '{TableId}' AND SF_OrderSub.BranchId = '{BranchId}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewOrder();
                        Item.OrderId = data.GetString(0);
                        Item.i = data.GetInt32(1);
                        Item.TableId = data.GetString(2);
                        Item.FoodId = data.GetString(3);
                        Item.FoodName = data.GetString(4);
                        Item.Amount = data.GetDecimal(5);
                        Item.Price = data.GetDecimal(6);
                        Item.Status = data.GetInt32(7);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }
    }
}
