using System;
using System.Collections.Generic;

namespace Documents
{
    public class Dates
    {
        public static string GetMonth(DateTime d)
        {
            switch (d.Month)
            {
                case 1: return "січня";
                case 2: return "лютого";
                case 3: return "березня";
                case 4: return "квітня";
                case 5: return "травня";
                case 6: return "червня";
                case 7: return "липня";
                case 8: return "серпня";
                case 9: return "вересня";
                case 10: return "жовтня";
                case 11: return "листопада";
                case 12: return "грудня";
                default: return d.Month.ToString();
            }
        }
    }

    public enum TextCase { Nominative, Genitive, Dative, Accusative, Instrumental, Prepositional };

    public class UaDateAndMoneyConverter
    {
        static Dictionary<TextCase, string[]> monthNames = new Dictionary<TextCase, string[]>
        {
            { TextCase.Nominative, new []{"", "січень", "лютий", "березень", "квітень", "травень", "червень", "липень", "серпень", "вересень", "жовтень", "листопад", "грудень"} },
            { TextCase.Genitive,   new []{"", "січня", "лютого", "березня", "квітня", "травня", "червня", "липня", "серпня", "вересня", "жовтня", "листопада", "грудня"} }
        };

        static string zero = "нуль";
        static string firstMale = "один";
        static string firstFemale = "одна";
        static string firstFemaleAccusative = "одну";
        static string firstMaleGenetive = "одного";
        static string secondMale = "два";
        static string secondFemale = "дві";
        static string secondMaleGenetive = "двох";
        static string secondFemaleGenetive = "двох";

        static string[] from3till19 =
        {
            "", "три", "чотири", "п'ять", "шість",
            "сім", "вісім", "дев'ять", "десять", "одинадцять",
            "дванадцять", "тринадцять", "чотирнадцять", "п'ятнадцять",
            "шістнадцять", "сімнадцять", "вісімнадцять", "дев'ятнадцять"
        };
        static string[] from3till19Genetive =
        {
            "", "трьох", "чотирьох", "п'яти", "шести",
            "семи", "восьми", "дев'яти", "десяти", "одинадцяти",
            "дванадцяти", "тринадцяти", "чотирнадцяти", "п'ятнадцяти",
            "шістнадцяти", "сімнадцяти", "вісімнадцяти", "дев'ятнадцяти"
        };
        static string[] tens = { "", "двадцять", "тридцять", "сорок", "п'ятдесят", "шістдесят", "сімдесят", "вісімдесят", "дев'яносто" };
        static string[] tensGenetive = { "", "двадцяти", "тридцяти", "сорока", "п'ятдесяти", "шістдесяти", "сімдесяти", "вісімдесяти", "дев'яноста" };
        static string[] hundreds = { "", "сто", "двісті", "триста", "чотириста", "п'ятсот", "шістсот", "сімсот", "вісімсот", "дев'ятсот" };
        static string[] hundredsGenetive = { "", "ста", "двохсот", "трьохсот", "чотирьохсот", "п'ятисот", "шестисот", "семисот", "восьмисот", "дев'ятисот" };

        static string[] thousands = { "", "тисяча", "тисячі", "тисяч", "тисяч" };
        static string[] thousandsAccusative = { "", "тисячу", "тисячі", "тисячі", "тисяч" };
        static string[] millions = { "", "мільйон", "мільйон", "мільйон", "мільйони" };
        static string[] billions = { "", "мільярд", "мільярд", "мільярди", "мільярди" };
        static string[] trillions = { "", "трильйон", "трильйона", "трильйонів" };

        static string[] hryvnias = { "", "гривня", "гривні", "гривень", "гривень" };
        static string[] copecks = { "", "копійка", "копійки", "копійок" };

        public static string DateToTextLong(DateTime _date, string _year)
        {
            return String.Format("«{0}» {1} {2}",
                                    _date.Day.ToString("D2"),
                                    MonthName(_date.Month, TextCase.Genitive),
                                    _date.Year.ToString()) + ((_year.Length != 0) ? " " : "") + _year;
        }

        public static string DateToTextLong(DateTime _date)
        {
            return String.Format("«{0}» {1} {2}",
                                    _date.Day.ToString("D2"),
                                    MonthName(_date.Month, TextCase.Genitive),
                                    _date.Year.ToString());
        }

        public static string DateToTextQuarter(DateTime _date)
        {
            return NumeralsRoman(DateQuarter(_date)) + " квартал " + _date.Year.ToString();
        }

        public static string DateToTextSimple(DateTime _date)
        {
            return String.Format("{0:dd.MM.yyyy}", _date);
        }

        public static int DateQuarter(DateTime _date)
        {
            return (_date.Month - 1) / 3 + 1;
        }

        static bool IsPluralGenitive(int _digits) => _digits >= 5 || _digits == 0;
        static bool IsSingularGenitive(int _digits) => _digits >= 2 && _digits <= 4;

        static int lastDigit(long _amount)
        {
            long amount = _amount;

            if (amount >= 100) amount = amount % 100;
            if (amount >= 20) amount = amount % 10;

            return (int)amount;
        }

        public static string CurrencyToTxt(double _amount, bool _firstCapital)
        {
            long hryvniaAmount = (long)Math.Floor(_amount);
            long copecksAmount = ((long)Math.Round(_amount * 100)) % 100;
            int lastHryvniaDigit = lastDigit(hryvniaAmount);
            int lastCopecksDigit = lastDigit(copecksAmount);

            string s = NumeralsToTxt(hryvniaAmount, TextCase.Nominative, true, _firstCapital) + " ";

            if (IsPluralGenitive(lastHryvniaDigit)) s += hryvnias[3] + " ";
            else if (IsSingularGenitive(lastHryvniaDigit)) s += hryvnias[2] + " ";
            else s += hryvnias[1] + " ";

            s += String.Format("{0:00} ", copecksAmount);

            if (IsPluralGenitive(lastCopecksDigit)) s += copecks[3] + " ";
            else if (IsSingularGenitive(lastCopecksDigit)) s += copecks[2] + " ";
            else s += copecks[1] + " ";

            return s.Trim();
        }

        public static string CurrencyToTxtShort(double _amount, bool _firstCapital)
        {
            long hryvniaAmount = (long)Math.Floor(_amount);
            long copecksAmount = ((long)Math.Round(_amount * 100)) % 100;
            int lastHryvniaDigit = lastDigit(hryvniaAmount);
            int lastCopecksDigit = lastDigit(copecksAmount);

            string s = String.Format("{0:N0} ", hryvniaAmount);

            if (IsPluralGenitive(lastHryvniaDigit)) s += hryvnias[3] + " ";
            else if (IsSingularGenitive(lastHryvniaDigit)) s += hryvnias[2] + " ";
            else s += hryvnias[1] + " ";

            s += String.Format("{0:00} ", copecksAmount);

            if (IsPluralGenitive(lastCopecksDigit)) s += copecks[3] + " ";
            else if (IsSingularGenitive(lastCopecksDigit)) s += copecks[2] + " ";
            else s += copecks[1] + " ";

            return s.Trim();
        }

        public static string CurrencyToTxtFull(double _amount, bool _firstCapital)
        {
            long hryvniaAmount = (long)Math.Floor(_amount);
            long copecksAmount = ((long)Math.Round(_amount * 100)) % 100;
            int lastHryvniaDigit = lastDigit(hryvniaAmount);
            int lastCopecksDigit = lastDigit(copecksAmount);

            string s = String.Format("{0} ({1}) ",
                hryvniaAmount.ToString("N0"),
                NumeralsToTxt(hryvniaAmount, TextCase.Nominative, true, _firstCapital));

            if (IsPluralGenitive(lastHryvniaDigit)) s += hryvnias[3] + " ";
            else if (IsSingularGenitive(lastHryvniaDigit)) s += hryvnias[2] + " ";
            else s += hryvnias[1] + " ";

            s += String.Format("{0:00} ", copecksAmount);

            if (IsPluralGenitive(lastCopecksDigit)) s += copecks[3] + " ";
            else if (IsSingularGenitive(lastCopecksDigit)) s += copecks[2] + " ";
            else s += copecks[1] + " ";

            return s.Trim();
        }

        public static string NumericalToTxt(double _amount, bool _firstCapital)
        {
            long hryvniaAmount = (long)Math.Floor(_amount);
            string s = String.Format("{0} ", NumeralsToTxt(hryvniaAmount, TextCase.Nominative, true, _firstCapital));
            return s.Trim();
        }

        static string MakeText(int _digits, string[] _hundreds, string[] _tens, string[] _from3till19, string _second, string _first, string[] _power)
        {
            string s = "";
            int digits = _digits;

            if (digits >= 100)
            {
                s += _hundreds[digits / 100] + " ";
                digits %= 100;
            }
            if (digits >= 20)
            {
                s += _tens[digits / 10 - 1] + " ";
                digits %= 10;
            }
            if (digits >= 3) s += _from3till19[digits - 2] + " ";
            else if (digits == 2) s += _second + " ";
            else if (digits == 1) s += _first + " ";

            if (_digits != 0 && _power.Length > 0)
            {
                digits = lastDigit(_digits);
                if (IsPluralGenitive(digits)) s += _power[3] + " ";
                else if (IsSingularGenitive(digits)) s += _power[2] + " ";
                else s += _power[1] + " ";
            }

            return s;
        }

        public static string NumeralsToTxt(long _sourceNumber, TextCase _case, bool _isMale, bool _firstCapital)
        {
            string s = "";
            long number = _sourceNumber;
            int remainder;
            int power = 0;

            if ((number >= (long)Math.Pow(10, 15)) || number < 0) return "";

            while (number > 0)
            {
                remainder = (int)(number % 1000);
                number /= 1000;

                switch (power)
                {
                    case 12:
                        s = MakeText(remainder, hundreds, tens, from3till19, secondMale, firstMale, trillions) + s;
                        break;
                    case 9:
                        s = MakeText(remainder, hundreds, tens, from3till19, secondMale, firstMale, billions) + s;
                        break;
                    case 6:
                        s = MakeText(remainder, hundreds, tens, from3till19, secondMale, firstMale, millions) + s;
                        break;
                    case 3:
                        if (_case == TextCase.Accusative)
                            s = MakeText(remainder, hundreds, tens, from3till19, secondFemale, firstFemaleAccusative, thousandsAccusative) + s;
                        else
                            s = MakeText(remainder, hundreds, tens, from3till19, secondFemale, firstFemale, thousands) + s;
                        break;
                    default:
                        string[] powerArray = { };
                        if (_case == TextCase.Genitive)
                            s = MakeText(remainder, hundredsGenetive, tensGenetive, from3till19Genetive, _isMale ? secondMaleGenetive : secondFemaleGenetive, _isMale ? firstMaleGenetive : firstFemale, powerArray) + s;
                        else if (_case == TextCase.Accusative)
                            s = MakeText(remainder, hundreds, tens, from3till19, _isMale ? secondMale : secondFemale, _isMale ? firstMale : firstFemaleAccusative, powerArray) + s;
                        else
                            s = MakeText(remainder, hundreds, tens, from3till19, _isMale ? secondMale : secondFemale, _isMale ? firstMale : firstFemale, powerArray) + s;
                        break;
                }

                power += 3;
            }

            if (_sourceNumber == 0) s = zero + " ";

            if (s != "" && _firstCapital)
                s = s.Substring(0, 1).ToUpper() + s.Substring(1);

            return s.Trim();
        }

        public static string MonthName(int _month, TextCase _case)
        {
            if (_month > 0 && _month <= 12 && monthNames.ContainsKey(_case))
                return monthNames[_case][_month];

            return "";
        }

        public static string NumeralsRoman(int _number)
        {
            return _number switch
            {
                1 => "I",
                2 => "II",
                3 => "III",
                4 => "IV",
                _ => ""
            };
        }
    }
}