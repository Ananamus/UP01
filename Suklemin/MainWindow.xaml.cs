using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Suklemin
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Метод создающий и рандомизирующий капчу
        /// </summary>
        public void Captch()
        {
            string pwd = string.Empty;

            System.Random r = new System.Random();
            var charactersAvailable = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

            string rst = string.Empty;
            for (int i = 0; i < 4; i++)
            {
                rst = charactersAvailable[r.Next(0, charactersAvailable.Length - 1)].ToString();
                pwd += rst;

            }
            capthText.FontSize = r.Next(10, 20);
            capthText.Text = pwd;
        }
        BataDase bd = new BataDase();
        /// <summary>
        /// Основное окно программы
        /// </summary>
        public MainWindow()
        {
            
            InitializeComponent();
            Captch();
            Temps.users = bd.Users_.ToList();
            Temps.enters = bd.HistoryEnter_.ToList();
            Temps.orders = bd.Order_.ToList();
            Temps.services = bd.Services_.ToList();
        }
        /// <summary>
        /// Метод, прячущий или показывающий пароль
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hideButton_click(object sender, RoutedEventArgs e)
        {
            if (TextBox.Visibility == Visibility.Collapsed) TextBox.Visibility = Visibility.Visible;
            else TextBox.Visibility = Visibility.Collapsed;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            TextBox.Text = PasswordBox.Password;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PasswordBox.Password = TextBox.Text;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (enterin)
            {
                if (capth.Text == capthText.Text || capth.Visibility == Visibility.Collapsed)
                {
                    if (PasswordBox.Password != string.Empty && LoginTextBox.Text != string.Empty)
                    {
                        Users_ user = Temps.users.FirstOrDefault(x => x.login == LoginTextBox.Text);
                        if (user != null)
                        {
                            TimeSpan? schet = TimeSpan.MaxValue;
                            if (user.role == 4 || user.role == 5)
                            {
                                List<HistoryEnter_> Historys = bd.HistoryEnter_.Where(x => x.userId == user.id && x.erorId == 1).ToList();
                                if (Historys.Count > 0)
                                {
                                    schet = DateTime.Now - Historys.Last().date;
                                }
                                else
                                {
                                    schet = DateTime.Now - DateTime.MinValue;
                                }
                            }

                            if (schet > TimeSpan.FromHours(3))
                            {
                                if (user.password == PasswordBox.Password)
                                {
                                    Temps.user = user;
                                    HistoryEnter_ enter = new HistoryEnter_()
                                    {
                                        date = DateTime.Now,
                                        erorId = null,
                                        ip = Temps.GetIp(),
                                        userId = user.id
                                    };
                                    bd.HistoryEnter_.Add(enter);
                                    bd.SaveChanges();
                                    Temps.enters = bd.HistoryEnter_.ToList();
                                    
                                    switch (user.role)
                                    {
                                        case 1: // Админ
                                            Admin admin = new Admin();
                                            admin.Name.Text = $"Здравстуйте {user.secondName} {user.firstName} {user.thirdName}\n{user.Role_.name}";
                                            Hide();
                                            admin.ShowDialog();
                                            Show();
                                            break;
                                        case 2: // Бухгалтер
                                            break;
                                        case 3: //Пациент
                                            break;
                                        case 4: // Лаборант
                                            LabAssisntant LA = new LabAssisntant();
                                            LA.Name.Text = $"Здравстуйте {user.secondName} {user.firstName} {user.thirdName}\n{user.Role_.name}";
                                            Hide();
                                            LA.ShowDialog();
                                            Show();
                                            break;
                                        case 5: // Лаборант-исследователь
                                            LabResearcher LR = new LabResearcher();
                                            LR.Name.Text = $"Здравстуйте {user.secondName} {user.firstName} {user.thirdName}\n{user.Role_.name}";
                                            Hide();
                                            LR.ShowDialog();
                                            Show();
                                            break;
                                    }
                                }
                                else
                                {
                                    HistoryEnter_ enter = new HistoryEnter_
                                    {
                                        date = DateTime.Now,
                                        erorId = 2,
                                        ip = Temps.GetIp(),
                                        userId = user.id
                                    };
                                    bd.HistoryEnter_.Add(enter);
                                    bd.SaveChanges();
                                    MessageBox.Show("Данные введены неверно");
                                    capth.Visibility = Visibility.Visible;
                                    capthText.Visibility = Visibility.Visible;
                                    ButtonRe.Visibility = Visibility.Visible;
                                }
                            }
                            else
                            {
                                MessageBox.Show("Подождите, идёт кварцевание");
                                HistoryEnter_ enter = new HistoryEnter_
                                {
                                    date = DateTime.Now,
                                    erorId = 4,
                                    ip = Temps.GetIp(),
                                    userId = user.id
                                };
                            }
                        }
                        else
                        {
                            MessageBox.Show("Данные введены неверно");
                            capth.Visibility = Visibility.Visible;
                            capthText.Visibility = Visibility.Visible;
                            ButtonRe.Visibility = Visibility.Visible;
                        }
                    }
                    else MessageBox.Show("Заполните данные");
                }
                else
                {
                    MessageBox.Show("Капча введена неверно");
                    Captch();

                    timer.Tick += new EventHandler(timer_Tick);
                    timer.Interval = new TimeSpan(0, 0, 1);
                    enterin = false;
                    timer.Start();
                }
            }
            else MessageBox.Show("Капча была введена неверно, подождите 10 секунд");
        }
        DispatcherTimer timer = new DispatcherTimer();
        static int block = 10;
        static bool enterin = true;
        private void timer_Tick(object sender, EventArgs e)
        {
            if(block > 0)
            {
                block--;
            }
            else
            {
                timer.Stop();
                MessageBox.Show("Вход разблокирован");
                enterin = true;
                block = 10;
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Captch();
        }
    }
}
