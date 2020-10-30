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
    public class FoodSubController : BaseController
    {
        #region Connect db
        private readonly DbConnection _db;

        public FoodSubController(DbConnection db)
        {
            _db = db;
        }

        #endregion 

        [HttpPost]
        public IActionResult Add(string id, decimal Amount)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId").Value;
            var ListStaple = new List<ViewFoodSub>();

            if (id != null)
            {
                if (Amount > 0)
                {
                    var Staple = new CD_Staple();
                    Staple = _db.CD_Staple.FirstOrDefault(x => x.StapleId == id && x.BranchId == BranchId);

                    var Unit = new CD_Unit();
                    Unit = _db.CD_Unit.FirstOrDefault(x => x.UnitId == Staple.UnitId && x.BranchId == Staple.BranchId);                    

                    if (HttpContext.Session.GetString("Session_StapleList") != null)
                    {
                        ListStaple = JsonConvert.DeserializeObject<List<ViewFoodSub>>(HttpContext.Session.GetString("Session_StapleList"));
                    }

                    if (ListStaple.Exists(x => x.StapleId == Staple.StapleId))
                    {
                        Alert("", "วัตถุดิบนี้มีอยู่แล้ว", Enums.NotificationType.warning);
                    }
                    else
                    {
                        var item = new ViewFoodSub
                        {
                            StapleId = Staple.StapleId,
                            StapleName = Staple.StapleName,
                            Amount = Amount,
                            UnitId = Unit.UnitId,
                            UnitName = Unit.UnitName
                        };

                        ListStaple.Add(item);
                    }

                    HttpContext.Session.SetString("Session_StapleList", JsonConvert.SerializeObject(ListStaple));
                }
                else
                {
                    if (HttpContext.Session.GetString("Session_StapleList") != null)
                    {
                        ListStaple = JsonConvert.DeserializeObject<List<ViewFoodSub>>(HttpContext.Session.GetString("Session_StapleList"));
                    }

                    Alert("", "กรุณาใส่จำนวนวัตถุุดิบ !", Enums.NotificationType.warning);
                }
            }
            else
            {
                if (HttpContext.Session.GetString("Session_StapleList") != null)
                {
                    ListStaple = JsonConvert.DeserializeObject<List<ViewFoodSub>>(HttpContext.Session.GetString("Session_StapleList"));
                }

                Alert("", "กรุณาเลือกวัตถุดิบ !", Enums.NotificationType.warning);
            }

            return Json(new { data = ListStaple });
        }

        public IActionResult Remove(string id)
        {

            var ListStaple = new List<ViewFoodSub>();

            if (HttpContext.Session.GetString("Session_StapleList") != null)
            {
                ListStaple = JsonConvert.DeserializeObject<List<ViewFoodSub>>(HttpContext.Session.GetString("Session_StapleList"));
            }

            if (ListStaple.Exists(x => x.StapleId == id))
            {
                var DETELE = ListStaple.Find(x => x.StapleId == id);
                ListStaple.Remove(DETELE);
            }

            HttpContext.Session.SetString("Session_StapleList", JsonConvert.SerializeObject(ListStaple));

            return Json(new { data = ListStaple });
        }

        public IActionResult Delete(string id)
        {
            string Status = null;

            if (HttpContext.Session.GetString("Session_StapleList") != null)
            {
                HttpContext.Session.Remove("Session_StapleList");
                Status = "success";
            }

            return Json(new { data = Status });
        }

        public IActionResult GetStaple(string id)
        {
            var BranchId = User.Claims.FirstOrDefault(c => c.Type == "BranchId")?.Value;
            var List = new List<ViewFoodSub>();
            var sql = $"SELECT FoodId, CD_Staple.StapleId, CD_Staple.StapleName, CD_FoodSub.Amount, CD_Unit.UnitId, CD_Unit.UnitName "
                    + $"FROM CD_FoodSub "
                    + $"LEFT JOIN CD_Unit on CD_FoodSub.UnitId = CD_Unit.UnitId "
                    + $"LEFT JOIN CD_Staple on CD_FoodSub.StapleId = CD_Staple.StapleId "
                    + $"WHERE CD_FoodSub.FoodId = '{id}' AND CD_FoodSub.BranchId = '{BranchId}' AND CD_Staple.BranchId = '{BranchId}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewFoodSub();
                        Item.FoodId = data.GetString(0);
                        Item.StapleId = data.GetString(1);
                        Item.StapleName = data.GetString(2);
                        Item.Amount = data.GetDecimal(3);
                        Item.UnitId = data.GetString(4);
                        Item.UnitName = data.GetString(5);
                        List.Add(Item);
                    }
                }

                HttpContext.Session.SetString("Session_StapleList", JsonConvert.SerializeObject(List));
            }
            return Json(new { data = List });
        }     

    }
}
