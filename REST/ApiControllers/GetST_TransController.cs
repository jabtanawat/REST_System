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

        public List<ViewST_Trans> FoodAll(string branchid)
        {
            var List = new List<ViewST_Trans>();
            string sql = $"SELECT Documents, DateDocument, Title, FirstName, LastName "
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
                            Item.DishName = data.GetString(2);
                        if (!data.IsDBNull(3))
                            Item.Price = Share.Cnumber(Share.FormatDouble(data.GetDecimal(3)), 2);
                        if (!data.IsDBNull(4))
                            Item.GroupFoodName = data.GetString(4);
                        List.Add(Item);
                    }
                }
            }

            return List;
        }
    }
}
