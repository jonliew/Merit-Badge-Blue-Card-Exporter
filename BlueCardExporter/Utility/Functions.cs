using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BlueCardExporter.Utility
{
    public static class Functions
    {
        /// <summary>
        /// Validates the date string from the import
        /// Must be 8 digits and a valid date once parsed
        /// </summary>
        /// <param name="date">String containing the date to validate</param>
        /// <param name="outDate">The converted date if valid</param>
        /// <returns>If the string is valid, true. Otherwise, false.</returns>
        public static bool ValidateDate(this string date, out DateTime outDate)
        {
            Regex expression = new Regex(@"^[0-9]{8}$");
            outDate = default;
            return expression.IsMatch(date)
                && DateTime.TryParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out outDate);
        }
    }
}
