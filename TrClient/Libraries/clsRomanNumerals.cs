// <copyright file="clsRomanNumerals.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace DanishNLP
{
    public static class ClsRomanNumerals
    {
        public static int RomanToArabic(string romanValue)
        {
            // Return the Arabic version of this number.
            int result = 0;
            int new_value = 0;
            int old_value = 1000;

            romanValue = romanValue.ToUpper();

            for (int i = 0; i < romanValue.Length; i++)
            {
                // See what the next character is worth.
                string ch = romanValue.Substring(i, 1);

                switch (ch)
                {
                    case "I":
                        new_value = 1;
                        break;
                    case "V":
                        new_value = 5;
                        break;
                    case "X":
                        new_value = 10;
                        break;
                    case "L":
                        new_value = 50;
                        break;
                    case "C":
                        new_value = 100;
                        break;
                    case "D":
                        new_value = 500;
                        break;
                    case "M":
                        new_value = 1000;
                        break;
                }

                // See if this character is bigger
                // than the previous one.
                if (new_value > old_value)
                {
                    // The new value > the previous one.
                    // Add this value to the result
                    // and subtract the previous one twice.
                    result = result + new_value - (2 * old_value);
                }
                else
                {
                    // The new value <= the previous one.
                    // Add it to the result.
                    result = result + new_value;
                }

                old_value = new_value;
            }

            return result;
        }

        public static string ArabicToRoman(int arabicValue)
        {
            // Return the Roman numeral version of this number.
            int arabic_number = arabicValue;
            int digit = 0;
            string result = string.Empty;

            if (arabicValue > 0)
            {
                // Pull out thousands.
                digit = arabic_number / 1000;
                arabic_number = arabic_number - (digit * 1000);
                result = result + new string('M', digit);

                // Pull out hundreds.
                digit = arabic_number / 100;
                arabic_number = arabic_number - (digit * 100);
                result = AddRomanDigits(result, digit, 'M', 'D', 'C');

                // Pull out tens.
                digit = arabic_number / 10;
                arabic_number = arabic_number - (digit * 10);
                result = AddRomanDigits(result, digit, 'C', 'L', 'X');

                // Pull out ones.
                digit = arabic_number;
                result = AddRomanDigits(result, digit, 'X', 'V', 'I');
            }

            return result;
        }

        private static string AddRomanDigits(string temp_result, int arabic_digit, char ten_letter, char five_letter, char one_letter)
        {
            string result = temp_result;

            // Add appropriate Roman digits to the result.
            // The ten_letter, five_letter, and one_letter
            // are the digits for 10, 5, and 1 at this
            // power of ten. For example, 10/5/1 = X/V/I,
            // 100/50/10 = C/L/X, etc.
            switch (arabic_digit)
            {
                case 1:
                case 2:
                case 3:
                    result = result + new string(one_letter, arabic_digit);
                    break;
                case 4:
                    result = result + one_letter + five_letter;
                    break;
                case 5:
                    result = result + five_letter;
                    break;
                case 6:
                case 7:
                case 8:
                    result = result + five_letter + new string(one_letter, arabic_digit - 5);
                    break;
                case 9:
                    result = result + one_letter + ten_letter;
                    break;
            }

            return result;
        }
    }
}
