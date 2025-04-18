using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Suklemin
{
    /// <summary>
    /// Хранение временных данных
    /// </summary>
    class Temps
    {
        public static BataDase bd = new BataDase();
        public static Users_ user = new Users_();
        public static List<Users_> users = new List<Users_>();
        public static List<HistoryEnter_> enters = new List<HistoryEnter_>();
        public static List<Order_> orders = new List<Order_>();
        public static List<Services_> services = new List<Services_>();
        public static List<HistoryResearch_> researchs = new List<HistoryResearch_>();
        /// <summary>
        /// Получение ip адреса
        /// </summary>
        /// <returns></returns>
        public static string GetIp()
        {
            string ipAddress = string.Empty;
            if (Dns.GetHostAddresses(Dns.GetHostName()).Length > 0)
            {
                ipAddress = Dns.GetHostAddresses(Dns.GetHostName())[0].ToString();
            }
            return ipAddress;
        }
        public static void ReloadLists()
        {
            users = bd.Users_.ToList();
            enters = bd.HistoryEnter_.ToList();
            researchs = bd.HistoryResearch_.ToList();
            orders = bd.Order_.ToList();
            services = bd.Services_.ToList();
        }
    }
    /// <summary>
    /// Класс для отправки данных на "сервер"
    /// </summary>
    public class Services
    {
        /// <summary>
        /// Код услуги
        /// </summary>
        public int servicecode { get; set; }
        /// <summary>
        /// Результат
        /// </summary>
        public string result { get; set; }
    }
    public class GetAnalizator
    {
        /// <summary>
        /// Номер пациента
        /// </summary>
        public string patient { get; set; }
        /// <summary>
        /// Список результатов полученных от анализатора
        /// </summary>
        public List<Services> services { get; set; }
        /// <summary>
        /// Прогресс выполнения анализа
        /// </summary>
        public int progress { get; set; }
    }

}
