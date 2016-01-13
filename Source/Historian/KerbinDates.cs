using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KSEA.Historian
{
    public static class KerbinDates
    {
        public static string[] KerbinMonthNames = { "Unnam", "Dosnam", "Trenam", "Cuatnam", "Cinqnam", "Seinam", "Sietnam", "Ocnam", "Nuevnam", "Diznam", "Oncnam", "Docenam" };
        public static string[] KerbinDayNames = { "Akant", "Brant", "Casant", "Dovant", "Esant", "Flant" };

        public static string KerbinMonthName(this int dayNumber)
        {
            return KerbinMonthNames[KerbinMonth(dayNumber) - 1];
        }

        public static int KerbinMonth(this int dayNumber)
        {
            return (int)Math.Floor((dayNumber - 0.01f) / 35.5) + 1;
        }

        public static int[] FirstDayOfMonth = { 1, 36, 72, 107, 143, 178, 214, 249, 285, 320, 356, 391 };

        public static int KerbinDayOfMonth(this int dayNumber)
        {
            var month = KerbinMonth(dayNumber);
            return dayNumber - FirstDayOfMonth[month - 1] + 1;
        }

        public static int KerbinDayOfWeek(this int dayNumber)
        {
            return ((dayNumber - 1) % 6) + 1;
        }

        internal static int ParseRepeatPattern(String format, int pos, char patternChar)
        {
            int len = format.Length;
            int index = pos + 1;
            while ((index < len) && (format[index] == patternChar))
            {
                index++;
            }
            return (index - pos);
        }

        // var value = string.Format("Y{0}, D{1:D2}, {2}:{3:D2}:{4:D2}", time[4] + m_baseYear, time[3] + 1, time[2], time[1], time[0]);
        enum TimePart
        {
            Second = 0,
            Minute = 1,
            Hour = 2,
            Day = 3,
            Year = 4
        }

        public static string[] DigitFormat = { "", "0", "00", "000", "0000", "00000" };

        public static string FormattedDate(this int[] kerbinDate, string format, int baseYear = 1)
        {
            StringBuilder result = new StringBuilder();

            int i = 0;
            int tokenLen;
            int kerbinDay = kerbinDate[(int)TimePart.Day];

            while (i < format.Length)
            {
                char ch = format[i];
                int nextChar;
                switch (ch)
                {
                    case 'h':
                    case 'H':
                        tokenLen = ParseRepeatPattern(format, i, ch);
                        result.Append(FormatDigits(kerbinDate[(int)TimePart.Hour], tokenLen, 1));
                        break;
                    case 'm':
                        tokenLen = ParseRepeatPattern(format, i, ch);
                        result.Append(FormatDigits(kerbinDate[(int)TimePart.Minute], tokenLen, 2));
                        break;
                    case 's':
                        tokenLen = ParseRepeatPattern(format, i, ch);
                        result.Append(FormatDigits(kerbinDate[(int)TimePart.Second], tokenLen, 2));
                        break;

                    case 'd':
                        //
                        // tokenLen == 1 : Day of month as digits with no leading zero.
                        // tokenLen == 2 : Day of month as digits with leading zero for single-digit months.
                        // tokenLen == 3 : Day of week as a three-leter abbreviation.
                        // tokenLen >= 4 : Day of week as its full name.
                        //
                        tokenLen = ParseRepeatPattern(format, i, ch);
                        if (tokenLen <= 2)
                        {
                            int day = kerbinDay.KerbinDayOfMonth();
                            result.Append(FormatDigits(day, tokenLen, 2));
                        }
                        else
                        {
                            int dayOfWeek = kerbinDay.KerbinDayOfWeek();
                            result.Append(FormatDayOfWeek(dayOfWeek, tokenLen));
                        }
                        break;
                    case 'M':
                        // 
                        // tokenLen == 1 : Month as digits with no leading zero.
                        // tokenLen == 2 : Month as digits with leading zero for single-digit months.
                        // tokenLen == 3 : Month as a three-letter abbreviation.
                        // tokenLen >= 4 : Month as its full name.
                        //
                        tokenLen = ParseRepeatPattern(format, i, ch);
                        int month = kerbinDay.KerbinMonth();
                        result.Append(FormatMonth(month, tokenLen));
                        break;
                    case 'y':
                        int year = kerbinDate[(int)TimePart.Year] + baseYear;
                        tokenLen = ParseRepeatPattern(format, i, ch);
                        result.Append(FormatYear(year, tokenLen));
                        break;
                    case ':':
                    case '/':
                        result.Append(ch);
                        tokenLen = 1;
                        break;
                    default:
                        // NOTENOTE : we can remove this rule if we enforce the enforced quote
                        // character rule.
                        // That is, if we ask everyone to use single quote or double quote to insert characters,
                        // then we can remove this default block.
                        result.Append(ch);
                        tokenLen = 1;
                        break;
                }
                i += tokenLen;
            }
            return result.ToString();

        }

        private static string FormatDigits(int value, int tokenLen, int maxPadding)
        {
            if (tokenLen > maxPadding)
                tokenLen = maxPadding;
            return value.ToString(DigitFormat[tokenLen]);
        }

        private static string FormatYear(int year, int tokenLen)
        {
            return year.ToString(new string('0', tokenLen));
        }

        private static string FormatMonth(int month, int tokenLen)
        {
            switch (tokenLen)
            {
                case 1:
                    return month.ToString();
                case 2:
                    return month.ToString("00");
                case 3:
                    return KerbinMonthNames[month - 1].Substring(0, 3);
                case 4:
                default:
                    return KerbinMonthNames[month - 1];
            }
        }

        private static string FormatDayOfWeek(int dayOfWeek, int tokenLen)
        {
            switch (tokenLen)
            {
                case 3:
                    return KerbinDayNames[dayOfWeek - 1].Substring(0, 3);
                default:
                    return KerbinDayNames[dayOfWeek - 1];
            }
        }
    }
}
