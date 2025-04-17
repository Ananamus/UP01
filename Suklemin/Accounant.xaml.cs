using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Suklemin
{
    /// <summary>
    /// Логика взаимодействия для Accounant.xaml
    /// </summary>
    public partial class Accounant : Window
    {
        /// <summary>
        /// Инициализация формы бухгалтера
        /// </summary>
        public Accounant()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Сохранение выхода при выходе из прогаммы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            HistoryEnter_ enter = new HistoryEnter_()
            {
                date = DateTime.Now,
                userId = Temps.user.id,
                ip = Temps.GetIp(),
                erorId = 3
            };
            Temps.bd.HistoryEnter_.Add(enter);
            Temps.bd.SaveChanges();
        }
    }
}
