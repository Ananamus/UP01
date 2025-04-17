using Aspose.BarCode.Generation;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using Path = System.IO.Path;

namespace Suklemin
{
    /// <summary>
    /// Логика взаимодействия для LabAssisntant.xaml
    /// </summary>
    public partial class LabAssisntant : Window
    {
        public DispatcherTimer timer = new DispatcherTimer();
        public LabAssisntant()
        {
            InitializeComponent();
            Temps.ReloadLists();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
            DataGrid.ItemsSource = Temps.orders;
            FilterPatient.ItemsSource = Temps.users.Where(x => x.role == 3).ToList();
            UsersCombo.ItemsSource = Temps.users.Where(x => x.role == 3).ToList();
            ServiceCombobox.ItemsSource = Temps.services.ToList();
            Hint.Text = (Temps.orders.Last().id + 1).ToString();
            ComboType.ItemsSource = Temps.bd.Type_.ToList();
            InsCompanyCombo.ItemsSource = Temps.bd.InsuranceCompany_.ToList();
        }
        public static TimeSpan seconds = TimeSpan.FromSeconds(0);
        public static bool kek = false;
        public static TimeSpan time = TimeSpan.FromSeconds(9000);
        private void timer_Tick(object sender, EventArgs e)
        {
            if (kek == false && seconds >= TimeSpan.FromSeconds(8100)) { kek = true; MessageBox.Show("Осталось 15 минут"); }
            if (seconds < TimeSpan.FromSeconds(9000)) seconds = seconds.Add(TimeSpan.FromSeconds(1));
            else
            {
                HistoryEnter_ enter = new HistoryEnter_()
                {
                    date = DateTime.Now,
                    userId = Temps.user.id,
                    ip = Temps.GetIp(),
                    erorId = 1
                };
                Temps.bd.HistoryEnter_.Add(enter);
                Temps.bd.SaveChanges();
                Temps.ReloadLists();
                timer.Stop();
                Application.Current.Shutdown();
            }
            TimerTextBlock.Text = time.Subtract(seconds).ToString();
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
            Temps.ReloadLists();
        }

        private void TextBoxID_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextBoxID.Text == null || TextBoxID.Text == "")
            {
                Hint.Text = (Temps.orders.Last().id + 1).ToString();
            }
            else Hint.Text = null;
        }
        public Users_ ThisUser = new Users_();
        private void FilterPatient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ThisUser = (Users_)FilterPatient.SelectedValue;
            ReloadDataGrid();
        }
        static int ch = 0;
        private void TextBoxID_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!int.TryParse(TextBoxID.Text, out int number)) TextBoxID.Text = null;
                if (TextBoxID.Text == null || TextBoxID.Text == "")
                    TextBoxID.Text = (Temps.orders.Last().id + 1).ToString();
                if (Convert.ToInt32(TextBoxID.Text) <= Temps.orders.Last().id)
                {
                    TextBoxID.Text = (Temps.orders.Last().id + 1).ToString();
                }
                var imageType = "Jpeg";// тип картинки в переменной вар
                var imageFormat = (BarCodeImageFormat)Enum.Parse(typeof(BarCodeImageFormat), imageType);// сначала штрихкод потом конвертируем формат
                var encodeType = EncodeTypes.Code128;
                string imagePath = "Code128_" + ch.ToString() + "." + imageType;// название пути 
                string imagePath1 = "Code128" + ch.ToString() + "." + imageType;// название пути 
                string TextBarcode = TextBoxID.Text;
                if (TextBoxID.Text.Count() == 1)
                {
                    TextBarcode = "00" + TextBoxID.Text;
                }
                else
                {
                    if (TextBoxID.Text.Count() == 2) TextBarcode = "0" + TextBoxID.Text;
                }
                Random rand = new Random();
                TextBarcode += DateTime.Now.ToString("ddMMyyyy") + rand.Next(100000, 999999).ToString();
                BarcodeGenerator generator = new BarcodeGenerator(encodeType, TextBarcode);
                generator.Save(imagePath, imageFormat);
                generator.Save(imagePath1, imageFormat);
                BarcodeImage.Source = new BitmapImage(new Uri(Path.GetFullPath(imagePath)));
                generator.Dispose();
                ch++;

                var document = new iTextSharp.text.Document();
                using (var writer = PdfWriter.GetInstance(document, new FileStream("result.pdf", FileMode.Create)))
                {
                    document.Open();
                    //вставка изображения штрихкода
                    var logo = iTextSharp.text.Image.GetInstance(new FileStream(Environment.CurrentDirectory.ToString() + @"\Code128" + (ch - 1).ToString() + ".Jpeg", FileMode.Open));//готовим переменую для хранения в ней фотки
                    logo.SetAbsolutePosition(0, 680);//кординаты для картинки
                    writer.DirectContent.AddImage(logo);// добавляем картинку
                    BaseFont baseFont = BaseFont.CreateFont(@"C:\Windows\Fonts\arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);//берем ариал так как нормально воспринимет русский текст
                    iTextSharp.text.Font helvetica1 = new iTextSharp.text.Font(baseFont, 78, iTextSharp.text.Font.NORMAL);
                    document.Add(new iTextSharp.text.Paragraph(" ", helvetica1));
                    document.Close();
                    writer.Close();
                }
            }

        }

        private void TextBoxID_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Temps.orders.FirstOrDefault(x => Convert.ToString(x.id) == TextBoxID.Text) != null)
            {
                MessageBox.Show("Такой код уже существует");
                TextBoxID.Text = (Temps.orders.Last().id + 1).ToString();
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DataGrid.Focus();
        }

        private void Grid_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            DataGrid.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TextBoxID.Text != null && FilterPatient.SelectedIndex != -1)
            {
                Order_ order = new Order_
                {
                    date = DateTime.Now,
                    statusOrder = 1,
                    userId = ThisUser.id
                };
                Temps.bd.Order_.Add(order);
                Temps.bd.SaveChanges();
                MessageBox.Show("Заказ создан");
                Temps.orders = Temps.bd.Order_.ToList();
                Temps.ReloadLists();
                ReloadDataGrid();
                ThisOrder = order;
            }
        }
        static Order_ ThisOrder;
        private void LoginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (Class1.Login_check(LoginTextBox.Text) && !Temps.users.Exists(x => x.login == LoginTextBox.Text))
            {
                if (Class1.Password_check(PasswordTextBox.Text))
                {
                    if (Class1.Mail_check(EmailTextBox.Text))
                    {
                        if (FirstNameTextBox.Text != null && SecondNameTextBox.Text != null && DateBirth.SelectedDate != null && PhoneTextBox.Text != null && InsNumberTextBox != null && ComboType.SelectedIndex != -1 && InsCompanyCombo.SelectedIndex != -1)
                        {
                            Users_ user = new Users_
                            {
                                firstName = FirstNameTextBox.Text,
                                secondName = SecondNameTextBox.Text,
                                thirdName = ThirdNameTextBox.Text,
                                birthDate = DateBirth.SelectedDate,
                                E_mail = EmailTextBox.Text,
                                role = 3,
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
                            DataGrid.ItemsSource = null;
                            FilterPatient.ItemsSource = null;
                            DataGrid.ItemsSource = Temps.orders;
                            FilterPatient.ItemsSource = Temps.users.Where(x => x.role == 3).ToList();
                            ClearAllTextboxes();
                            Temps.ReloadLists();
                            ReloadDataGrid();
                        }
                        else MessageBox.Show("Поля не заполнены");
                    }
                    else MessageBox.Show("Почта введена неверно");
                }
                else MessageBox.Show("Пароль введен неверно");
            }
            else MessageBox.Show("Логин введен не верно");

        }
        /// <summary>
        /// Очистка текстбоксов
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
            PasswordTextBox.Text = null;
        }
        private void ServiceCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HistoryResearch_ hist = new HistoryResearch_
            {
                services = Temps.services[ServiceCombobox.SelectedIndex].Code,
                orderId = ThisOrder.id,
                Services_ = Temps.services[ServiceCombobox.SelectedIndex]
            };

            ServiceDataGrid.Items.Add(hist);
            hists.Add(hist);
            Decimal? dec = 0;
            foreach (HistoryResearch_ histo in hists)
            {
                 dec += histo.Services_.price; 
            }
            Sum.Text = dec.ToString();
        }
        List<HistoryResearch_> hists = new List<HistoryResearch_>();
        static int index;
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            index = DataGrid.SelectedIndex;
            if (index >= 0)
            {
                ThisOrder = (Order_)DataGrid.Items[index];
            }
        }
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ThisOrder.statusOrder == 1)
            {
                ServiceDataGrid.ItemsSource = null;
                ServiceDataGrid.Items.Clear();
                Create.Focus();
                UsersCombo.SelectedValue = ThisOrder.Users_;
                Uslugi.IsEnabled = true;
                ServiceCombobox.IsEnabled = true;
                hists.Clear();
            }
            else
            {
                ServiceDataGrid.ItemsSource = null;
                ServiceDataGrid.Items.Clear();
                List <HistoryResearch_> Hists = Temps.bd.HistoryResearch_.Where(x => x.orderId == ThisOrder.id).ToList();
                ServiceDataGrid.ItemsSource = Hists;
                Decimal? dec = 0;
                foreach (HistoryResearch_ histo in Hists)
                {
                    dec += histo.Services_.price;
                }
                Sum.Text = "Итого: " + dec.ToString();
                Uslugi.IsEnabled = false;
                ServiceCombobox.IsEnabled = false;
                UsersCombo.SelectedValue = ThisOrder.Users_;
                Create.Focus();
                hists.Clear();
            }
        }

        private void UsersCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void All_Checked(object sender, RoutedEventArgs e)
        {
            Temps.ReloadLists();
            ReloadDataGrid();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Temps.ReloadLists();
            ReloadDataGrid();

        }

        private void FilterPatient_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FilterPatient.SelectedIndex = -1;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Temps.bd.Order_.First(x => x.id == ThisOrder.id).statusOrder = 2;
            Temps.bd.SaveChanges();
            Temps.ReloadLists();
            ReloadDataGrid();
            foreach (HistoryResearch_ histories in hists)
            {
                Temps.bd.HistoryResearch_.Add(histories);
                Temps.bd.SaveChanges();
                Temps.orders = Temps.bd.Order_.ToList();
                Temps.ReloadLists();
                ReloadDataGrid();
            }
            ServiceDataGrid.Items.Clear();
            hists.Clear();

            MessageBox.Show("Услуги добавлены к заказу");
        }
        public void ReloadDataGrid()
        {
            DataGrid.ItemsSource = null;
            if(All.IsChecked == true && FilterPatient.SelectedIndex == -1) DataGrid.ItemsSource = Temps.orders;
            if(All.IsChecked == false && FilterPatient.SelectedIndex == -1) DataGrid.ItemsSource = Temps.orders.Where(x => x.statusOrder == 1);
            if (All.IsChecked == true && FilterPatient.SelectedIndex != -1) DataGrid.ItemsSource = Temps.orders.Where(x => x.userId == ThisUser.id);
            if (All.IsChecked == false && FilterPatient.SelectedIndex != -1) DataGrid.ItemsSource = Temps.orders.Where(x => x.statusOrder == 1 && x.userId == ThisUser.id);
        }

        private void Create_GotFocus(object sender, RoutedEventArgs e)
        {
            Sum.Visibility = Visibility.Visible;
        }

        private void Create_LostFocus(object sender, RoutedEventArgs e)
        {
            Sum.Visibility = Visibility.Collapsed;
        }
    }
}

