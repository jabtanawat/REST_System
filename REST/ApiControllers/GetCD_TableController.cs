﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST.Data;
using REST.Models;
using REST.ViewModels;
using REST.Service;

namespace REST.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetCD_TableController : ControllerBase
    {
        #region Connect db
        private readonly DbConnection _db;
        public GetCD_TableController(DbConnection db)
        {
            _db = db;
        }
        #endregion 

        public List<ViewTable> Table(string branchid)
        {
            var List = new List<ViewTable>();
            string sqlWhrer = null;
            var sql = $"SELECT TableId, TableName, Personal, TableST, CASE WHEN TableST = 1 THEN 'ว่าง' WHEN TableST = 2 THEN 'ไม่ว่าง' WHEN TableST= 3 THEN 'จอง' END AS Status, ZoneName, CD_Table.BchName "
                    + $"FROM CD_Table "
                    + $"LEFT JOIN CD_Zone ON CD_Table.ZoneId = CD_Zone.ZoneId ";

            if (branchid != null)
                sqlWhrer += $"CD_Table.BranchId = '{branchid}' ";

            if (sqlWhrer != null)
                sql += "WHERE " + sqlWhrer;

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewTable();
                        if (!data.IsDBNull(0))
                            Item.TableId = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.TableName = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.Personal = data.GetInt32(2);
                        if (!data.IsDBNull(3))
                            Item.TableST = data.GetInt32(3);
                        if (!data.IsDBNull(4))
                            Item.Status = data.GetString(4);
                        if (!data.IsDBNull(5))
                            Item.ZoneName = data.GetString(5);
                        if (!data.IsDBNull(6))
                            Item.BchName = data.GetString(6);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }

        public List<ViewTable> TableZoneStatus(string ZoneId, string Status, string branchid)
        {
            var List = new List<ViewTable>();
            var sql = $"SELECT ZoneId, TableId, TableName, Personal, Description, TableST, "
                    + $"(SELECT ISNULL(SUM(Amount * Price), 0) FROM SF_OrderSub LEFT JOIN SF_Order ON SF_OrderSub.OrderId = SF_Order.OrderId WHERE SF_OrderSub.BranchId = '{branchid}' AND SF_Order.Success = '1' AND SF_OrderSub.TableId = CD_Table.TableId) AS Total "
                    + $"FROM CD_Table "
                    + $"WHERE BranchId = '{branchid}' ";
            if (Status != null)
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
                        var Item = new ViewTable();
                        if (!data.IsDBNull(0))
                            Item.ZoneId = data.GetString(0);
                        Item.TableId = data.GetString(1);
                        Item.TableName = data.GetString(2);
                        if (!data.IsDBNull(3))
                            Item.Personal = data.GetInt32(3);
                        if (!data.IsDBNull(4))
                            Item.Description = data.GetString(4);
                        Item.TableST = data.GetInt32(5);
                        if (!data.IsDBNull(6))
                            Item.Total = Share.Cnumber(Share.FormatDouble(data.GetDecimal(6)), 0);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }

        public ViewTable TableById(string id, string branchid)
        {
            var Item = new ViewTable();

            var sql = $"SELECT TableId, TableName, Description, TableST, CASE WHEN TableST = 1 THEN 'ว่าง' WHEN TableST = 2 THEN 'ไม่ว่าง' WHEN TableST= 3 THEN 'จอง' END AS Status "
                    + $"FROM CD_Table "
                    + $"WHERE BranchId = '{branchid}' AND TableId = '{id}'";
            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {                        
                        if (!data.IsDBNull(0))
                            Item.TableId = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.TableName = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.Description = data.GetString(2);
                        if (!data.IsDBNull(3))
                            Item.TableST = data.GetInt32(3);
                        if (!data.IsDBNull(4))
                            Item.Status = data.GetString(4);
                    }
                }
            }

            return Item;
        }
    }
}
