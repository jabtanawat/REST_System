using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using REST.Controllers;
using REST.Data;
using REST.Service;
using REST.ViewModels;

namespace REST.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetST_TransController : BaseController
    {
        #region Connect db
        private readonly DbConnection _db;
        public GetST_TransController(DbConnection db)
        {
            _db = db;
        }
        #endregion 

        public List<ViewST_Trans> TransAll(string branchid)
        {
            var List = new List<ViewST_Trans>();
            string sql = $"SELECT Documents, DateDocument, Title, FirstName, LastName, SumBalance "
                    + $"FROM ST_Trans  "
                    + $"LEFT JOIN MB_Supplier ON ST_Trans.SupplierId = MB_Supplier.SupplierId "
                    + $"WHERE ST_Trans.BranchId = '{branchid}' ";

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewST_Trans();
                        if (!data.IsDBNull(0))
                            Item.Document = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.DateDocument = data.GetDateTime(1).ToString("dd/MM/yyyy");
                        if (!data.IsDBNull(2))
                            Item.SupplierName = data.GetString(2);
                        if (!data.IsDBNull(3))
                            Item.SupplierName += ' ' + data.GetString(3);
                        if (!data.IsDBNull(4))
                            Item.SupplierName += ' ' + data.GetString(4);
                        if (!data.IsDBNull(5))
                            Item.SumBalance = Share.Cnumber(Share.FormatDouble(data.GetDecimal(5)), 2);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }

        public List<ViewST_TranSub> TranSubId(string id, string branchid)
        {
            var List = new List<ViewST_TranSub>();
            string sqlWhere = null;
            string sql = $"SELECT Documents, ST_TranSub.StapleId, CD_Staple.StapleName, Amount, ST_TranSub.Unit, Price, Discount, Total, Vat "
                    + $"FROM ST_TranSub "
                    + $"LEFT JOIN CD_Staple ON ST_TranSub.StapleId = CD_Staple.StapleId ";

            if (branchid != null)
                if (sqlWhere != null)
                    sqlWhere += $"AND ST_TranSub.BranchId = '{branchid}' ";
                else
                    sqlWhere += $"ST_TranSub.BranchId = '{branchid}' ";

            if (id != null)
                if (sqlWhere != null)
                    sqlWhere += $"AND Documents = '{id}' ";
                else
                    sqlWhere += $"Documents = '{id}' ";

            if (sqlWhere != null)
                sql += "WHERE " + sqlWhere + "ORDER BY i ASC ";

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewST_TranSub();
                        if (!data.IsDBNull(0))
                            Item.Documents = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.StapleId = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.StapleName = data.GetString(2);
                        if (!data.IsDBNull(3))
                            Item.Amount = data.GetDecimal(3);
                        if (!data.IsDBNull(4))
                            Item.Unit = data.GetString(4);
                        if (!data.IsDBNull(5))
                            Item.Price = data.GetDecimal(5);
                        if (!data.IsDBNull(6))
                            Item.Discount = data.GetDecimal(6);
                        if (!data.IsDBNull(7))
                            Item.Total = data.GetDecimal(7);
                        if (!data.IsDBNull(8))
                            Item.Vat = data.GetString(8);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }

        public List<ViewStaple> StapleStore(string branchid)
        {
            var List = new List<ViewStaple>();

            string sqlWhere = null;
            string sql = $"SELECT CD_Staple.StapleId, StapleName, ISNULL(QtyBalance, 0 ), Unit "
                    + $"FROM CD_Staple  "
                    + $"LEFT JOIN StapleBalance ON CD_Staple.StapleId = StapleBalance.StapleId ";

            if (branchid != null)
                if (sqlWhere != null)
                    sqlWhere += $"AND CD_Staple.BranchId = '{branchid}' ";
                else
                    sqlWhere += $"CD_Staple.BranchId = '{branchid}' ";

            if (sqlWhere != null)
                sql += "WHERE " + sqlWhere + "AND QtyBalance <= QtyLow";
            else
                sql += "WHERE QtyBalance <= QtyLow";

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                _db.Database.OpenConnection();
                using (var data = command.ExecuteReader())
                {
                    while (data.Read())
                    {
                        var Item = new ViewStaple();
                        if (!data.IsDBNull(0))
                            Item.StapleId = data.GetString(0);
                        if (!data.IsDBNull(1))
                            Item.StapleName = data.GetString(1);
                        if (!data.IsDBNull(2))
                            Item.QtyBalance = Share.Cnumber(Share.FormatDouble(data.GetDecimal(2)), 2);
                        if (!data.IsDBNull(3))
                            Item.Unit = data.GetString(3);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }

    }
}
