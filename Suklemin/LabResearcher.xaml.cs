using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Suklemin
{
    /// <summary>
    /// Логика взаимодействия для LabResearcher.xaml
    /// </summary>
    public partial class LabResearcher : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        /// <summary>
        /// Инициализация формы лаборанта исследователя
        /// </summary>
        public LabResearcher()
        {
            InitializeComponent();
            //Создание таймера ↓
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
            //Создание таймера ↑
            //Заполнение данных в поля ↓
            Temps.ReloadLists();
            DataGrid.ItemsSource = Temps.orders.Where(x => x.statusOrder == 2);
            ComboAna.ItemsSource = Temps.bd.Analizators_.ToList();
            //Заполнение данных в поля ↑
        }

        TimeSpan seconds = TimeSpan.FromSeconds(0);
        /// <summary>
        /// Переменная запрета на отправки более чем 1 сообщения
        /// </summary>
        bool kek = false;
        TimeSpan time = TimeSpan.FromSeconds(9000);
        /// <summary>
        /// Счётчик таймера за каждую секунду, а так же предупреждение при остатке 15 минут
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            if (kek == false && seconds >= TimeSpan.FromSeconds(8100)) { kek = true; MessageBox.Show("Осталось 15 минут"); }
            if (seconds < TimeSpan.FromSeconds(9000)) seconds = seconds.Add(TimeSpan.FromSeconds(1));
            else
            {
                // История об выходе из-за кварцевания ↓
                HistoryEnter_ enter = new HistoryEnter_()
                {
                    date = DateTime.Now,
                    userId = Temps.user.id,
                    ip = Temps.GetIp(),
                    erorId = 1
                };
                Temps.bd.HistoryEnter_.Add(enter);
                Temps.bd.SaveChanges();
                timer.Stop();
                Application.Current.Shutdown();
                // История об выходе из-за кварцевания ↑
            }
            TimerTextBlock.Text = time.Subtract(seconds).ToString(); // ← Вывод таймера на форму
        }
        /// <summary>
        /// Создание истории о ручном выходе из программы
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
        /// <summary>
        /// Таймер исследования
        /// </summary>
        DispatcherTimer researchTimer = new DispatcherTimer();
        int tiks = 0;
        /// <summary>
        /// Создание таймера на приём данных и длительность исследования
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Research_Tick(object sender, EventArgs e)
        {
            if (tiks < 30)
            {
                tiks++;
                ProgressBar.Value = tiks;
                Procent.Text = (tiks * 100 / 30).ToString() + "%";
            }
            else
            {
                ProgressBar.Value = 0;
                ProgressBar.Visibility = Visibility.Collapsed;
                Procent.Visibility = Visibility.Collapsed;
                MessageBox.Show("Загрузка окончена");
                researchTimer.Stop();
                //Получение данных от сервера ↓
                try
                {
                    GetAnalizator getAnalizator = new GetAnalizator();
                    var httpWebRequestBiorad = (HttpWebRequest)WebRequest.Create($"http://localhost:5000/api/analyzer/Biorad");
                    var httpWebRequestLedetect = (HttpWebRequest)WebRequest.Create($"http://localhost:5000/api/analyzer/Ledetect");
                    httpWebRequestBiorad.ContentType = "application/json";
                    httpWebRequestBiorad.Method = "GET";
                    httpWebRequestLedetect.ContentType = "application/json";
                    httpWebRequestLedetect.Method = "GET";
                    if (ThisResearchs2.Count > 0)
                    {
                        var httpResponseBiorad = (HttpWebResponse)httpWebRequestBiorad.GetResponse();
                        if (httpResponseBiorad.StatusCode == HttpStatusCode.OK)
                        {
                            using (Stream stream = httpResponseBiorad.GetResponseStream())
                            {
                                StreamReader reader = new StreamReader(stream);
                                string json = reader.ReadToEnd();
                                JavaScriptSerializer serializer = new JavaScriptSerializer();
                                getAnalizator = serializer.Deserialize<GetAnalizator>(json);
                                checks2(getAnalizator);

                            }
                        }
                    }
                    if (ThisResearchs1.Count > 0)
                    {
                        var httpResponseLedetect = (HttpWebResponse)httpWebRequestLedetect.GetResponse();
                        if (httpResponseLedetect.StatusCode == HttpStatusCode.OK)
                        {
                            using (Stream stream = httpResponseLedetect.GetResponseStream())
                            {
                                StreamReader reader = new StreamReader(stream);
                                string json = reader.ReadToEnd();
                                JavaScriptSerializer serializer = new JavaScriptSerializer();
                                getAnalizator = serializer.Deserialize<GetAnalizator>(json);
                                checks(getAnalizator);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                // Получение данных от сервера ↑
                ContentOfList.Text = allText; // ← Отображение данных на форме
                Temps.bd.Order_.First(x => x.id == ThisOrder.id).statusOrder = 3;
                DataGrid.ItemsSource = null;
                DataGrid.ItemsSource = Temps.orders.Where(x => x.statusOrder == 2);
            }
        }
        static string allText = null; // ← Переменная текста для вывода результатов на форму
        public void checks(GetAnalizator getAnalizator)
        {

            int i = 0;
            foreach (Services serv in getAnalizator.services)
            {
                if (double.TryParse(serv.result, NumberStyles.Any, CultureInfo.InvariantCulture, out double result)) // ← Проверка на то является ли результат числом
                {
                    // Проверка выходит ли результат исследования за рамки обычного результата ↓
                    if (result > Temps.services.First(x => x.Code == serv.servicecode).upperLimitOfNormal * 5 || result < Temps.services.First(x => x.Code == serv.servicecode).lowerLimitOfNormal / 5) 
                    {
                        // Обработка подтверждения результата, выходящего за рамки ↓
                        MessageBoxResult resultM = MessageBox.Show(Temps.services.First(x => x.Code == serv.servicecode).name + " отклоняется от нормы в 5 раз" + serv.result, "Отклонение от нормы", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                        if (resultM == MessageBoxResult.OK)
                        {
                            Work_analizators_ workanaliz = new Work_analizators_()
                            {
                                analisatorId = 1,
                                date = DateTime.Now,
                                employeeId = Temps.user.id,
                                result = serv.result,
                                historyResearchId = ThisResearchs1[i].id,
                                time = ThisResearchs1[i].Services_.completionDate
                            };
                            Temps.bd.Work_analizators_.Add(workanaliz);
                            Temps.bd.SaveChanges();
                            allText += Temps.services.First(x => x.Code == serv.servicecode).name + " " + serv.result + "\n";
                        }
                        else
                        {
                            MessageBox.Show("Отменён");
                        }
                        // Обработка подтверждения результата, выходящего за рамки ↑
                    }
                    // Результат является нормой ↓
                    else
                    {
                        Work_analizators_ workanaliz = new Work_analizators_()
                        {
                            analisatorId = 1,
                            date = DateTime.Now,
                            employeeId = Temps.user.id,
                            result = serv.result,
                            historyResearchId = ThisResearchs1[i].id,
                            time = ThisResearchs1[i].Services_.completionDate
                        };
                        Temps.bd.Work_analizators_.Add(workanaliz);
                        Temps.bd.SaveChanges();
                        allText += Temps.services.First(x => x.Code == serv.servicecode).name + " " + serv.result + "\n";
                    }
                    // Результат является нормой ↑
                    // Проверка выходит ли результат исследования за рамки обычного результата ↑
                }
                //Добавление данных в бд при результате не являющимся числом ↓
                else
                {
                    Work_analizators_ workanaliz = new Work_analizators_()
                    {
                        analisatorId = 1,
                        date = DateTime.Now,
                        employeeId = Temps.user.id,
                        result = serv.result,
                        historyResearchId = ThisResearchs1[i].id,
                        time = ThisResearchs1[i].Services_.completionDate
                    };
                    Temps.bd.Work_analizators_.Add(workanaliz);
                    Temps.bd.SaveChanges();
                    allText += Temps.services.First(x => x.Code == serv.servicecode).name + " " + serv.result + "\n";
                }
                //Добавление данных в бд при результате не являющимся числом ↑
                i++;
                Temps.ReloadLists();
            }
        }
        public void checks2(GetAnalizator getAnalizator)
        {

            int i = 0;
            foreach (Services serv in getAnalizator.services)
            {
                if (double.TryParse(serv.result, NumberStyles.Any, CultureInfo.InvariantCulture, out double result)) // ← Проверка на то является ли результат числом
                {
                    // Проверка выходит ли результат исследования за рамки обычного результата ↓
                    if (result > Temps.services.First(x => x.Code == serv.servicecode).upperLimitOfNormal * 5 || result < Temps.services.First(x => x.Code == serv.servicecode).lowerLimitOfNormal / 5)
                    {
                        // Обработка подтверждения результата, выходящего за рамки ↓
                        MessageBoxResult resultM = MessageBox.Show(Temps.services.First(x => x.Code == serv.servicecode).name + " отклоняется от нормы в 5 раз" + serv.result, "Отклонение от нормы", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                        if (resultM == MessageBoxResult.OK)
                        {
                            Work_analizators_ workanaliz = new Work_analizators_()
                            {
                                analisatorId = 1,
                                date = DateTime.Now,
                                employeeId = Temps.user.id,
                                result = serv.result,
                                historyResearchId = ThisResearchs2[i].id,
                                time = ThisResearchs2[i].Services_.completionDate
                            };
                            Temps.bd.Work_analizators_.Add(workanaliz);
                            Temps.bd.SaveChanges();
                            allText += Temps.services.First(x => x.Code == serv.servicecode).name + " " + serv.result + "\n";
                        }
                        else
                        {
                            MessageBox.Show("Отменён");
                        }
                        // Обработка подтверждения результата, выходящего за рамки ↑
                    }
                    // Результат является нормой ↓
                    else
                    {
                        Work_analizators_ workanaliz = new Work_analizators_()
                        {
                            analisatorId = 1,
                            date = DateTime.Now,
                            employeeId = Temps.user.id,
                            result = serv.result,
                            historyResearchId = ThisResearchs2[i].id,
                            time = ThisResearchs2[i].Services_.completionDate
                        };
                        Temps.bd.Work_analizators_.Add(workanaliz);
                        Temps.bd.SaveChanges();
                        allText += Temps.services.First(x => x.Code == serv.servicecode).name + " " + serv.result + "\n";
                    }
                    // Результат является нормой ↑
                    // Проверка выходит ли результат исследования за рамки обычного результата ↑
                }
                //Добавление данных в бд при результате не являющимся числом ↓
                else
                {
                    Work_analizators_ workanaliz = new Work_analizators_()
                    {
                        analisatorId = 1,
                        date = DateTime.Now,
                        employeeId = Temps.user.id,
                        result = serv.result,
                        historyResearchId = ThisResearchs2[i].id,
                        time = ThisResearchs2[i].Services_.completionDate
                    };
                    Temps.bd.Work_analizators_.Add(workanaliz);
                    Temps.bd.SaveChanges();
                    allText += Temps.services.First(x => x.Code == serv.servicecode).name + " " + serv.result + "\n";
                }
                //Добавление данных в бд при результате не являющимся числом ↑
                i++;
                Temps.ReloadLists();
            }
        }
        /// <summary>
        /// Отправка данных на форму
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Environment.CurrentDirectory.ToString() + "\\Analyzer\\LIMSAnalyzers.exe");
            List<Services> services = new List<Services>();
            ThisResearchs2 = ThisResearchs.Where(x => x.analisator == 2).ToList();
            if (ThisResearchs2.Count > 0)
            {
                foreach (HistoryResearch_ res in ThisResearchs2)
                {
                    Services services1 = new Services();
                    services1.servicecode = res.services.Value;
                    services.Add(services1);
                }
                string patient = ThisOrder.userId.ToString();
                //

                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:5000/api/analyzer/Biorad");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = new JavaScriptSerializer().Serialize(new
                    {
                        patient,
                        services
                    });
                    streamWriter.Write(json);
                }
                try
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        MessageBox.Show("Успешная отправка на Biorad");
                        if (ProgressBar.Visibility != Visibility.Visible)
                        {
                            ProgressBar.Visibility = Visibility.Visible;
                            Procent.Visibility = Visibility.Visible;
                            researchTimer.Tick += new EventHandler(Research_Tick);
                            researchTimer.Interval = new TimeSpan(0, 0, 1);
                            researchTimer.Start();
                        }
                    }
                    else { MessageBox.Show("Ошибка отправки на Biorad"); }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                tiks = 0;
            }
            services.Clear();
            ThisResearchs1 = ThisResearchs.Where(x => x.analisator == 1).ToList();

            if (ThisResearchs1.Count > 0)
            {
                string patient = ThisOrder.userId.ToString();

                foreach (HistoryResearch_ res in ThisResearchs1)
                {
                    Services services1 = new Services();
                    services1.servicecode = res.services.Value;
                    services.Add(services1);
                }
                var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:5000/api/analyzer/Ledetect");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = new JavaScriptSerializer().Serialize(new
                    {
                        patient,
                        services
                    });
                    streamWriter.Write(json);
                }
                try
                {
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        MessageBox.Show("Успешная отправка на Ledetect");
                        if (ProgressBar.Visibility != Visibility.Visible)
                        {
                            ProgressBar.Visibility = Visibility.Visible;
                            Procent.Visibility = Visibility.Visible;
                            researchTimer.Tick += new EventHandler(Research_Tick);
                            researchTimer.Interval = new TimeSpan(0, 0, 1);
                            researchTimer.Start();
                        }
                    }
                    else { MessageBox.Show("Ошибка отправки на Ledetect"); }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        public static Order_ ThisOrder = new Order_();
        static List<HistoryResearch_> ThisResearchs = new List<HistoryResearch_>();
        static List<HistoryResearch_> ThisResearchs1 = new List<HistoryResearch_>();
        static List<HistoryResearch_> ThisResearchs2 = new List<HistoryResearch_>();
        /// <summary>
        /// Добавление текущего заказа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ThisOrder = DataGrid.SelectedItem as Order_;
        }
        /// <summary>
        /// Добавление Услуг выбранного заказа в списки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ThisResearchs.Clear();
            // Распределение услуг по анализаторам ↓
            foreach (HistoryResearch_ ThisResearch in Temps.researchs.Where(x => x.orderId == ThisOrder.id))
            {
                if (ThisResearch.Services_.Code == 619) ThisResearch.analisator = 3;
                if (ThisResearch.Services_.Code == 311) ThisResearch.analisator = 1;
                if (ThisResearch.Services_.Code == 548) ThisResearch.analisator = 2;
                if (ThisResearch.Services_.Code == 258) ThisResearch.analisator = 3;
                if (ThisResearch.Services_.Code == 176) ThisResearch.analisator = 2;
                if (ThisResearch.Services_.Code == 501) ThisResearch.analisator = 1;
                if (ThisResearch.Services_.Code == 543) ThisResearch.analisator = 3;
                if (ThisResearch.Services_.Code == 557) ThisResearch.analisator = 1;
                if (ThisResearch.Services_.Code == 229) ThisResearch.analisator = 1;
                if (ThisResearch.Services_.Code == 415) ThisResearch.analisator = 1;
                if (ThisResearch.Services_.Code == 323) ThisResearch.analisator = 1;
                if (ThisResearch.Services_.Code == 855) ThisResearch.analisator = 2;
                if (ThisResearch.Services_.Code == 346) ThisResearch.analisator = 1;
                if (ThisResearch.Services_.Code == 836) ThisResearch.analisator = 2;
                if (ThisResearch.Services_.Code == 659) ThisResearch.analisator = 3;
                if (ThisResearch.Services_.Code == 797) ThisResearch.analisator = 2;
                if (ThisResearch.Services_.Code == 287) ThisResearch.analisator = 2;
                ThisResearchs.Add(ThisResearch);
                Send.IsEnabled = true;
            }
            // Распределение услуг по анализаторам ↑
            // Добавление услуг в комбобокс для ручного распределения услуг, имеющих доступ к обоим анализаторам ↓
            if (ThisResearchs.Any(x => x.analisator == 3))
            {
                ComboBox.ItemsSource = null;
                List<HistoryResearch_> Here = ThisResearchs.Where(x => x.analisator == 3).ToList();
                ComboBox.ItemsSource = Here;
            }
            // Добавление услуг в комбобокс для ручного распределения услуг, имеющих доступ к обоим анализаторам ↑
        }
        /// <summary>
        /// Ручное распределение услуг
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboAna_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox.SelectedIndex != -1)
            {
                HistoryResearch_ thisthing = ComboBox.SelectedItem as HistoryResearch_;
                if (ComboAna.SelectedIndex == 0) ThisResearchs.First(x => x == thisthing).analisator = 1;
                if (ComboAna.SelectedIndex == 1) ThisResearchs.First(x => x == thisthing).analisator = 2;
                ComboBox.ItemsSource = null;
                List<HistoryResearch_> Here = ThisResearchs.Where(x => x.analisator == 3).ToList();
                ComboBox.ItemsSource = Here;
            }
        }

        /// <summary>
        /// Очистка
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Trash_Click(object sender, RoutedEventArgs e)
        {
            Send.IsEnabled = false;
            ContentOfList.Text = "";
            ThisOrder = null;
            ThisResearchs.Clear();
            Temps.ReloadLists();
            DataGrid.ItemsSource = null;
            DataGrid.ItemsSource = Temps.orders.Where(x => x.statusOrder == 2);
        }
    }
}
