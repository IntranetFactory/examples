using System;
using System.Collections.Generic;
using System.Globalization;

public static class DateTimeExtension
{
    private static string[] dateTimeFormats = { "yyMMdd", "yyMMddHHmm", "yyyyMMdd", "yyyyMMddHHmm", "yyyyMMddHHmmss", "yyyy-MM-ddTHH:mm:ssK" };

    public static DateTime ToDateTime(this string dateTimeString)
    {
        DateTime result = new DateTime();
        Boolean ok = false;

        ok = DateTime.TryParseExact(dateTimeString, dateTimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out result);

        if (!ok)
        {
            ok = DateTime.TryParse(dateTimeString, out result);
        }

        // convert local to UTC if result is recognized as local
        if (ok && result.Kind == DateTimeKind.Local)
        {
            result = result.ToUniversalTime();
        }

        // ensure that result is UTC
        if (ok && result.Kind != DateTimeKind.Utc)
        {
            result = DateTime.SpecifyKind(result, DateTimeKind.Utc);
        }

        return result;
    }


    public static bool IsNullOrEmpty(this DateTime date)
    {
        if (date == null) return true;

        return date == ("").ToDateTime();
    }


    public static string ToJsonString(this DateTime date)
    {
        if (date.Kind == DateTimeKind.Unspecified) date = DateTime.SpecifyKind(date, DateTimeKind.Utc);  // ensure that unknown kinds are converted to UTC as all our dates are UTC
        return date.ToUniversalTime().ToString("yyyy-MM-ddTHH':'mm':'ssK"); //without '' it returns . instead of :
    }

    public static long ToUnixTime(this DateTime datetime)
    {
        if (datetime.Kind == DateTimeKind.Unspecified) datetime = DateTime.SpecifyKind(datetime, DateTimeKind.Utc);  // ensure that unknown kinds are converted to UTC as all our dates are UTC
        DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return (long)(datetime - sTime).TotalSeconds;
    }

    public static DateTime FromUnixTime(this long unixtime)
    {
        DateTime sTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return sTime.AddSeconds(unixtime);
    }


    public static DateTime FromUnixTime(this string unixtime)
    {
        if (unixtime.IndexOf('.') > 0)
        {
            unixtime = unixtime.Substring(0, unixtime.IndexOf('.'));
        }

        long seconds = long.Parse(unixtime, CultureInfo.InvariantCulture);
        return seconds.FromUnixTime();
    }

    /* *ma* I know this looks wired -- but please keep untouched
     */
    #region wired

    //// ebg63("rot5")
    //private static string ebg0(string s)
    //{
    //    string k = "";

    //    foreach (char c in s)
    //    {
    //        if ((int)c >= 48 && (int)c <= 57)
    //        {
    //            // Zahlen
    //            k += (char)((((c - 48) + 5) % 10) + 48);
    //            continue;
    //        }

    //        k += c;
    //    }

    //    return k;
    //}

    //// ebg63("rot13")
    //private static string ebg68(string s)
    //{
    //    string k = "";

    //    foreach (char c in s)
    //    {
    //        if ((int)c >= 65 && (int)c <= 90)
    //        {
    //            // Großbuchstaben
    //            k += (char)((((c - 65) + 13) % 26) + 65);
    //            continue;

    //        }

    //        if ((int)c >= 97 && (int)c <= 122)
    //        {
    //            // Kleinbuchstaben
    //            k += (char)((((c - 97) + 13) % 26) + 97);
    //            continue;
    //        }

    //        k += c;
    //    }

    //    return k;
    //}

    //// ebg63("rot18")
    //public static string ebg63(this string DateTime)
    //{
    //    return ebg0(ebg68(DateTime));
    //}

    // ebg92 = ROT18("ROT47")
    public static string ebg92(this string DateTime)
    {
        string k = "";

        foreach (char c in DateTime)
        {
            if ((int)c >= 33 && (int)c <= 126)
            {
                // Ascii 33-126
                k = (char)((((c - 33) + 47) % 94) + 33) + k;
                continue;
            }

            k = c + k;
        }

        return k;
    }


    public static string ebg93(this DateTime dt)
    {
        int doy = dt.DayOfYear;
        doy += (1000 * (dt.Year - 1966));

        char[] clistarr = "0123456789abcdefghijklmnopqrstuvwxyz".ToCharArray();
        var result = new Stack<char>();
        while (doy != 0)
        {
            result.Push(clistarr[doy % 36]);
            doy /= 36;
        }

        return new string(result.ToArray());
    }

    #endregion

}
