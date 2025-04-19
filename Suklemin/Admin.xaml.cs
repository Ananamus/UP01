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
using System.Windows.Threading;

namespace Suklemin
{
    /// <summary>
    /// Логика взаимодействия для LabResearcher.xaml
    /// </summary>
    public partial class Admin : Window
    {
        /// <summary>
        /// Инициализация формы администратора
        /// </summary>
        public Admin()
        {
            InitializeComponent();
            Temps.ReloadLists();
            Grid.ItemsSource = Temps.enters;
            LoginCombobox.ItemsSource = Temps.users;
        }
        /// <summary>
        /// Отображения текста в зависимости от вкладок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (History.IsSelected)
            {
                LoginCombobox.Visibility = Visibility.Visible;
                DateEnd.Visibility = Visibility.Visible;
                DateStart.Visibility = Visibility.Visible;
            }
            else
            {    
                LoginCombobox.Visibility = Visibility.Collapsed;
                DateEnd.Visibility = Visibility.Collapsed;
                DateStart.Visibility = Visibility.Collapsed;
            }
        }

        private void LoginCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Grid.ItemsSource = null;
            if (DateEnd.SelectedDate == null || DateStart.SelectedDate == null)
            {
                Grid.ItemsSource = Temps.enters.Where(x => x.Users_ == LoginCombobox.SelectedValue).ToList();
            }
            else
            {
                Grid.ItemsSource = Temps.enters.Where(x => x.Users_ == LoginCombobox.SelectedValue && x.date <= DateEnd.SelectedDate && x.date >= DateStart.SelectedDate).ToList();
            }
        }

        private void DateEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if(DateStart.SelectedDate != null && DateEnd.SelectedDate != null && LoginCombobox.SelectedIndex != -1)
            {
                Grid.ItemsSource = null;
                Grid.ItemsSource = Temps.enters.Where(x => x.Users_ == LoginCombobox.SelectedValue && x.date <= DateEnd.SelectedDate && x.date >= DateStart.SelectedDate).ToList();
            }
            if(DateStart.SelectedDate != null && DateEnd.SelectedDate != null && LoginCombobox.SelectedIndex == -1)
            {
                Grid.ItemsSource = null;
                Grid.ItemsSource = Temps.enters.Where(x => x.date <= DateEnd.SelectedDate && x.date >= DateStart.SelectedDate).ToList();
            }
        }

        private void DateStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DateStart.SelectedDate != null && DateEnd.SelectedDate != null && LoginCombobox.SelectedIndex != -1)
            {
                Grid.ItemsSource = null;
                Grid.ItemsSource = Temps.enters.Where(x => x.Users_ == LoginCombobox.SelectedValue && x.date <= DateEnd.SelectedDate && x.date >= DateStart.SelectedDate).ToList();
            }
            if (DateStart.SelectedDate != null && DateEnd.SelectedDate != null && LoginCombobox.SelectedIndex == -1)
            {
                Grid.ItemsSource = null;
                Grid.ItemsSource = Temps.enters.Where(x => x.date <= DateEnd.SelectedDate && x.date >= DateStart.SelectedDate).ToList();
            }
        }

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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (Class1.Login_check(LoginTextBox.Text) && !Temps.users.Exists(x => x.login == LoginTextBox.Text))
            {
                if (Class1.Password_check(PassportTextBox.Text))
                {
                    if (Class1.Mail_check(EmailTextBox.Text))
                    {
                        if (FirstNameTextBox.Text != null && SecondNameTextBox.Text != null && DateBirth.SelectedDate != null && PhoneTextBox.Text != null && InsNumberTextBox != null && ComboType.SelectedIndex != -1 && InsCompanyCombo.SelectedIndex != -1 && RolesCombo.SelectedIndex != -1)
                        {
                            Users_ user = new Users_
                            {
                                firstName = FirstNameTextBox.Text,
                                secondName = SecondNameTextBox.Text,
                                thirdName = ThirdNameTextBox.Text,
                                birthDate = DateBirth.SelectedDate,
                                E_mail = EmailTextBox.Text,
                                role = RolesCombo.SelectedIndex + 1,
                                login = LoginTextBox.Text,
                                passport = PassportTextBox.Text,
                                password = PasswordTextBox.Text,
                                phoneNumber = PhoneTextBox.Text,
                                type = ComboType.SelectedIndex + 1,
                                insuranceNumber = Convert.ToDouble(InsNumberTextBox.Text),
                                insuranceCompany = InsCompanyCombo.SelectedIndex + 1
                            };
                            Temps.bd.Users_.Add(user);
                            Temps.bd.SaveChanges();
                            MessageBox.Show("Пользователь создан");
                            ClearAllTextboxes();
                        }
                        else MessageBox.Show("Заполните поля");
                    } else MessageBox.Show("Почта введена неверно");
                } else MessageBox.Show("Пароль введён неверно");
            }
            else MessageBox.Show("Логин введён неверно");
        }
        /// <summary>
        /// Очистка текстбоксов в отдельном методе из-за излишнего кода там ↑
        /// </summary>
        public void ClearAllTextboxes()
        {
            FirstNameTextBox.Text = null;
            SecondNameTextBox.Text = null;
            ThirdNameTextBox.Text = null;
            DateBirth.SelectedDate = null;
            EmailTextBox.Text = null;
            LoginTextBox.Text = null;
            PassportTextBox.Text = null;
            PhoneTextBox.Text = null;
            InsNumberTextBox.Text = null;
        }
    }
}
