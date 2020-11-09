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

        public static bool IsNullOrEmptyObject(object obj)
        {
            if (obj == null || Convert.IsDBNull(obj) || string.IsNullOrEmpty(obj.ToString()))
                return true;
            else
                return false;
        }

        public static int FormatInteger(object obj)
        {
            
            try
            {
                if (Share.IsNullOrEmptyObject(obj))
                    return 0;
                else
                    return System.Convert.ToInt32(obj);
            }
            catch (Exception ex)
            {
                if (ex.Message != null)
                    throw;
                else
                    return 0;
            }
        }

        public static double FormatDouble(object obj)
        {
            try
            {
                if (Share.IsNullOrEmptyObject(obj))
                    return 0;
                else
                    return Math.Round(System.Convert.ToDouble(obj.ToString()), 2, MidpointRounding.AwayFromZero);
            }
            catch (Exception ex)
            {
                if (ex.Message != null)
                    throw;
                else
                    return 0;
            }
        }

        public static string Cnumber(double TempValue, byte decimal_notation)
        {
            switch (decimal_notation)
            {
                case 0:
                    {
                        // TempValue = Math.Round(TempValue, Share.CD_Constant.RoundDecimal, MidpointRounding.AwayFromZero)
                        return TempValue.ToString("#,##0");
                    }

                case 1:
                    {
                        TempValue = Math.Round(TempValue, 1, MidpointRounding.AwayFromZero);
                        return TempValue.ToString("#,##0.0");
                    }

                case 2:
                    {
                        TempValue = Math.Round(TempValue, 2, MidpointRounding.AwayFromZero);
                        return TempValue.ToString("#,##0.00");
                    }

                case 3:
                    {
                        TempValue = Math.Round(TempValue, 3, MidpointRounding.AwayFromZero);
                        return TempValue.ToString("#,##0.000");
                    }

                case 4:
                    {
                        TempValue = Math.Round(TempValue, 4, MidpointRounding.AwayFromZero);
                        return TempValue.ToString("#,##0.0000");
                    }

                default:
                    {
                        TempValue = Math.Round(TempValue, 2, MidpointRounding.AwayFromZero);
                        return TempValue.ToString("#,##0.00");
                    }
            }
        }


    }
}
