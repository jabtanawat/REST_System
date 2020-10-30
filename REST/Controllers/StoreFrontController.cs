using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST.Data;
using REST.Models;

namespace REST.Controllers
{
    [Authorize]
    public class StoreFrontController : Controller
    {
        #region db
        private readonly DbConnection _db;

        public StoreFrontController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        public IActionResult Index()
        {
            ViewBag.TableCount = TableCount();
            ViewBag.TableST_1 = TableST_1();
            ViewBag.TableST_2 = TableST_2();
            ViewBag.TableST_3 = TableST_3();

            ViewBag.SL_Zone = SL_Zone();

            return View();
        }

        // ------

        public string TableCount()
        {
            string Count = null;

            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var sql = $"SELECT COUNT(*) AS TableCount "
                    + $"FROM CD_Table "
                    + $"WHERE BranchId = '{BranchId}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        Count = data["TableCount"].ToString();
                    }
                }
            }

            return Count;
        }

        public string TableST_1()
        {
            string Count = null;

            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var sql = $"SELECT COUNT(*) AS TableCount "
                    + $"FROM CD_Table "
                    + $"WHERE TableST = '1' AND BranchId = '{BranchId}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        Count = data["TableCount"].ToString();
                    }
                }
            }

            return Count;
        }

        public string TableST_2()
        {
            string Count = null;

            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var sql = $"SELECT COUNT(*) AS TableCount "
                    + $"FROM CD_Table "
                    + $"WHERE TableST = '2' AND BranchId = '{BranchId}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        Count = data["TableCount"].ToString();
                    }
                }
            }

            return Count;
        }

        public string TableST_3()
        {
            string Count = null;

            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var sql = $"SELECT COUNT(*) AS TableCount "
                    + $"FROM CD_Table "
                    + $"WHERE TableST = '3' AND BranchId = '{BranchId}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        Count = data["TableCount"].ToString();
                    }
                }
            }

            return Count;
        }

        // ------

        [HttpPost]
        public IActionResult GetTable(string ZoneId, string Status)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var List = new List<CD_Table>();
            var sql = $"SELECT TableId, TableName, Personal, Description, TableST "
                    + $"FROM CD_Table "
                    + $"WHERE BranchId = '{BranchId}' ";
            if(Status != null)
                sql += $"AND TableST = '{Status}'";
            if (ZoneId != null)
                sql += $"AND ZoneId = '{ZoneId}'";

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new CD_Table();
                        Item.TableId = data.GetString(0);
                        Item.TableName = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.Personal = data.GetInt32(2);
                        if (!data.IsDBNull(3))
                            Item.Description = data.GetString(3);                     
                        Item.TableST = data.GetInt32(4);
                        List.Add(Item);
                    }
                }
            }

            return Json(new { data = List });
        }

        public List<CD_Zone> SL_Zone()
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var List = new List<CD_Zone>();
            var sql = $"SELECT ZoneId, ZoneName FROM CD_Zone "
                    + $"WHERE BranchId = '{BranchId}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new CD_Zone();
                        Item.ZoneId = data.GetString(0);
                        Item.ZoneName = data.GetString(1);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }
    }
}
