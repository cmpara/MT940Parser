using System;
using System.Linq;

namespace programmersdigest.MT940Parser.Parsing {
    public static class DateParser {
        static string[] formats = new string[] { "yyMMdd", "yyyyMMdd" };
        public static DateTime Parse(string dateStr) {
            if (dateStr == null) {
                throw new ArgumentNullException(nameof(dateStr), "Date must not be null");
            }

            if (!DateTime.TryParseExact(dateStr, formats, new System.Globalization.CultureInfo("pl-PL"), System.Globalization.DateTimeStyles.None, out DateTime date)){
            
                throw new FormatException("Date has to be given in the form yyMMdd or yyyyMMdd");
            }
            

            return date;
        }
    }
}
