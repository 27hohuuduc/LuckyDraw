using System.Text.RegularExpressions;

namespace LuckyDraw.Services
{
    public class CheckStringFormat
    {
        public static bool IsPhoneNummber(string phone)
        {
            if (String.IsNullOrWhiteSpace(phone) || !Regex.IsMatch(phone, @"^([0-9]{10})$") || phone.Length != 10)
                return false;
            return true;
        }

        public static bool IsPassword(string password)
        {
            if (String.IsNullOrWhiteSpace(password) || !Regex.IsMatch(password, @"[0-9]+|[A-Z]+|.{8,20}"))
                return false;
            return true;

        }

       
    }
}
