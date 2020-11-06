using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using static REST.Service.Enums;

namespace REST.Controllers
{
    public class BaseController : Controller
    {
        public void Alert(string title, string message, NotificationType notificationType)
        {
            var msg = "<script>swal.fire('" + title + "', '" + message + "','" + notificationType + "')" + "</script>";
            TempData["notification"] = msg;
        }

        public void AlertTop(string message, NotificationType notificationType)
        {
            var msg = "<script>swal.fire({ position: 'top-end',"
                    + " icon: '" + notificationType + "', "
                    + "title: '"+ message + "', "
                    + "showConfirmButton: false, "
                    + "timer: 1500})"
                    + "</script>";
            TempData["notification"] = msg;
        }

        public void toastrAlert(string title, string message, NotificationToastr NotificationToastr)
        {
            var msg = "<script>"
                    + "toastr." + NotificationToastr + "('" + title +"', '" + message + "', { timeOut: 3000 });"
                    + "</script>";
            TempData["notification"] = msg;
        }

        public void AlertConfirm(string message, NotificationType notificationType, string id)
        {
            var msg = "<script>swal.fire({'" + message + "','" + notificationType + "', "
                    + "showCancelButton: true,"
                    + "confirmButtonColor: '#3085d6', "
                    + "cancelButtonColor: '#d33', "
                    + "confirmButtonText: 'Yes', "
                    + "cancelButtonText: 'No'}).then((result) => {"
                    + "if (result.value) {"
                    + "window.location.href = '@Url.Action('Delete', 'CD_Title', new {id = " + id + "})'"
                    + "}"
                    + "})</script>";
            TempData["notification"] = msg;
        }

        public IActionResult calculatorAnt2(string A, string P)
        {
            decimal Total = 0;
            decimal As = 0;
            decimal Ps = 0;

            try
            {
                if (A != null)
                    As = Convert.ToDecimal(A);
                if (P != null)
                    Ps = Convert.ToDecimal(P);

                Total = (As * Ps);

                return Json(new { data = Total });
            }
            catch (Exception)
            {
                return Json(new { data = Total });
            }
        }

        public IActionResult calculatorPlus2(string A1, string A2)
        {
            decimal Total = 0;
            decimal As1 = 0;
            decimal As2 = 0;

            try
            {
                if (A1 != null)
                    As1 = Convert.ToDecimal(A1);
                if (A2 != null)
                    As2 = Convert.ToDecimal(A2);

                Total = (As1 + As2);

                return Json(new { data = Total });
            }
            catch (Exception)
            {
                return Json(new { data = Total });
            }
        }

        public IActionResult calculatorRub2(string A1, string A2)
        {
            decimal Total = 0;
            decimal As1 = 0;
            decimal As2 = 0;

            try
            {
                if (A1 != null)
                    As1 = Convert.ToDecimal(A1);
                if (A2 != null)
                    As2 = Convert.ToDecimal(A2);

                Total = (As1 - As2);

                return Json(new { data = Total });
            }
            catch (Exception)
            {
                return Json(new { data = Total });
            }
        }

        public IActionResult FrmPayment1(string price, string persen)
        {
            decimal Total = 0;
            decimal As1 = 0;
            decimal As2 = 0;

            try
            {
                if (price != null)
                    As1 = Convert.ToDecimal(price);
                if (persen != null)
                    As2 = Convert.ToDecimal(persen);

                Total = As1 - (As1 * As2) / 100;

                return Json(new { data = Total });
            }
            catch (Exception)
            {
                return Json(new { data = Total });
            }
        }
    }
}
