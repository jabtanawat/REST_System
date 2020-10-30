using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Service
{
    public class Share
    {
        public static string FormatDateTime_Q(DateTime val)
        {
            int Y = val.Year;
            string Dt = val.ToString(Y + "-MM-dd HH:mm:ss.fff");
            return Dt;
        }

        public static DateTime FormatDate(Object dates)
        {
            var datereturn = new DateTime(2015, 1, 1);
            try
            {
                if (dates == null)
                    datereturn = new DateTime(2015, 1, 1);
                else
                {
                    try
                    {
                        datereturn = Convert.ToDateTime(dates);
                    }
                    catch (Exception)
                    {
                        datereturn = DateTime.ParseExact(dates.ToString(), "d/M/yyyy", CultureInfo.InvariantCulture);
                    }

                    if (datereturn.Year > 2350)
                        datereturn = datereturn.AddYears(-543);
                    else if (datereturn.Year < 1733)
                        datereturn = datereturn.AddYears(543);
                }
            }
            catch (Exception)
            {
                return datereturn;
            }

            return datereturn;
        }
    }
}
