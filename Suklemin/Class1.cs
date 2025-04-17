using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Suklemin
{
    /// <summary>
    /// Класс вместо библеотеки классов
    /// </summary>
    public class Class1
    {
        /// <summary>
        /// Проверка почты на её конструкцию
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public static bool Mail_check(string mail)
        {
            Regex listener = new Regex(@"(\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)");

            if (listener.IsMatch(mail))
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// Проверка пароля на спец символ, букву, цифру
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool Password_check(string password)
        {
            Regex listener = new Regex(@"(?=.{8,})(?=(.*\d){1,})(?=(.*\W){1,})(?=.*[a-z])(?=.*[A-Z])");

            if (listener.IsMatch(password))
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// Проверка логина на латинские символы и 6 знаков
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public static bool Login_check(string login)
        {
            Regex listener = new Regex(@"[0-9a-zA-Z]{6,}");

            if (listener.IsMatch(login))
            {
                return true;
            }

            return false;
        }
    }
}
