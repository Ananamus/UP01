# Лаборатория  
### Первый запуск  
При запуске приложения первое, что видит пользователь — окно входа. В нём пользователю предлагается ввести логин и пароль. Только после успешной авторизации пользователь получает доступ к остальным модулям системы.  

### Роли  
При вводе пароль скрыт маской, но также предусмотрена возможность его отображения. После авторизации пользователь получает доступ к соответствующему функционалу:  
- **Лаборант** может:  
  - принимать биоматериал и создавать на его основе заказы,  
  - добавлять новых клиентов,  
  - включать услуги в конкретный заказ.  
- **Лаборант-исследователь** может работать с анализатором.  
- **Администратор** имеет доступ к просмотру истории входов и выходов.  

### Кварцевание  
Сеанс работы лаборантов ограничен 2 часами 30 минутами, так как через этот промежуток времени необходимо проводить кварцевание помещений. За 15 минут до окончания сеанса появляется предупреждение. По истечении времени сеанса происходит автоматический выход из учётной записи, а вход блокируется на 30 минут.  

### Капча  
После первой неудачной попытки авторизации система выводит сообщение об ошибке и требует ввода капчи, состоящей из 4 символов (цифр и букв латинского алфавита) с графическим шумом. Пользователь может запросить новую капчу, если символы плохо различимы. После неудачной попытки с капчей вход блокируется на 10 секунд.  

### Администратор  
#### История входов  
Администратор видит, какие пользователи вошли в систему, время входа, а также причины выхода.  

#### Создание пользователей  
При открытии окна создания пользователя система отображает 13 обязательных полей. Пользователь создаётся после заполнения полей и нажатия кнопки, при соблюдении условий:  
- Логин должен содержать только латинские буквы и цифры и быть длиной 6 символов.  
- Пароль должен содержать минимум 1 букву, 1 цифру и 1 специальный символ, а его минимальная длина — 8 символов.  

### Лаборант  
#### Заказ  
При приёме биоматериала лаборант формирует заказ, который включает внесение в базу данных кода биоматериала и перечня исследований. Для создания заказа необходимо:  
1. Открыть окно формирования заказа.  
2. Ввести код пробирки вручную или отсканировать штрих-код.  

#### Услуга  
Чтобы добавить услугу в заказ:  
1. Дважды нажать на заказ со статусом "в обработке".  
2. В открывшемся окне выбрать нужные услуги и подтвердить заказ.  

Для удобства есть кнопка фильтрации заказов по статусу "в обработке". Если открыть заказ с другим статусом, отобразится информация о выполненных услугах, а интерфейс редактирования будет заблокирован. Также доступен поиск по пользователю.  

#### Создание пациента  
При создании пациента система запрашивает заполнение 12 обязательных полей. Условия для логина и пароля аналогичны требованиям при создании пользователя администратором.  

### Лаборант-исследователь  
#### Отправка анализов  
Для отправки анализов:  
1. Выбрать заказ из таблицы и дважды нажать на него.  
2. Услуги заказа появятся в списках для отправки на анализаторы.  
3. Для услуг, доступных на нескольких анализаторах, выбрать нужный в выпадающем списке.  
4. Нажать кнопку отправки.  

Ошибки можно исправить, очистив данные соответствующей кнопкой.  

#### Получение анализов  
Анализы поступают в течение 30 секунд после отправки. Система автоматически фиксирует результаты. Если показатели превышают норму в 5 раз, пользователь может подтвердить или оспорить результат.  
