

<h1>ADONET-FinalProject</h1>
<p><strong>Учебный проект</strong> по работе с <strong>ADO.NET</strong> в <strong>.NET 10.0</strong> с использованием <strong>MySQL / MariaDB</strong>.</p>
<p>Реализует CRUD-операции над связанными сущностями: пользователи, адреса и группы.</p>

<hr>

<h2>Функциональные возможности</h2>
<ul>
    <li><strong>Просмотр таблицы пользователей</strong> — отображение всех пользователей с их адресами и группами. Реализована фильтрация по группе пользователей.</li>
    <li><strong>Добавление пользователя</strong> — создание нового пользователя с привязкой к адресу и группе.</li>
    <li><strong>Редактирование пользователя</strong> — обновление личных данных, адреса и группы.</li>
    <li><strong>Удаление пользователя</strong> — удаление записи пользователя с каскадным удалением связанного адреса (при необходимости).</li>
</ul>

<hr>

<h2>Технологический стек</h2>
<ul>
    <li><strong>Язык:</strong> C#</li>
    <li><strong>Платформа:</strong> .NET 10.0</li>
    <li><strong>Доступ к данным:</strong> ADO.NET (SqlConnection, SqlCommand)</li>
    <li><strong>СУБД:</strong> MySQL / MariaDB</li>
    <li><strong>Тип приложения:</strong> Windows Forms / Console</li>
</ul>

<hr>

<h2>Установка и запуск</h2>

<h3>1. Клонирование репозитория</h3>
<pre>git clone https://github.com/your-username/ADONET-FinalProject.git
cd ADONET-FinalProject</pre>

<h3>2. Настройка строки подключения</h3>
<p>В файле <code>App.config</code> или в коде укажите строку подключения к вашей СУБД:</p>
<pre>&lt;connectionStrings&gt;
&lt;add name="DefaultConnection" 
   connectionString="Server=localhost;Database=YourDB;Uid=root;Pwd=yourpassword;" /&gt;
&lt;/connectionStrings&gt;</pre>

<h3>3. Применение миграций (или выполнение SQL-скрипта)</h3>
<p>Создайте базу данных и таблицы с помощью скрипта <code>schema.sql</code>.</p>

<h3>4. Запуск приложения</h3>
<pre>dotnet run</pre>

<hr>

<h2>Пример SQL-скрипта для создания таблиц</h2>
<pre>CREATE TABLE Groups (
Id INT PRIMARY KEY AUTO_INCREMENT,
Name VARCHAR(50) NOT NULL,
Description VARCHAR(255)
);

CREATE TABLE Addresses (
Id INT PRIMARY KEY AUTO_INCREMENT,
Street VARCHAR(100) NOT NULL,
City VARCHAR(50) NOT NULL,
PostalCode VARCHAR(20),
Country VARCHAR(50) NOT NULL
);

CREATE TABLE Users (
Id INT PRIMARY KEY AUTO_INCREMENT,
FirstName VARCHAR(50) NOT NULL,
LastName VARCHAR(50) NOT NULL,
Email VARCHAR(100) UNIQUE NOT NULL,
GroupId INT,
AddressId INT,
FOREIGN KEY (GroupId) REFERENCES Groups(Id) ON DELETE SET NULL,
FOREIGN KEY (AddressId) REFERENCES Addresses(Id) ON DELETE CASCADE
);</pre>

<hr>

<h2>Примеры использования</h2>

<h3>Фильтрация по группе</h3>
<p>Выберите группу из выпадающего списка, и таблица обновится, показывая только пользователей, принадлежащих к этой группе.</p>

<h3>Добавление пользователя</h3>
<p>Заполните поля формы (имя, фамилия, email, группа, адрес) и нажмите "Добавить".</p>

<h3>Редактирование</h3>
<p>Выберите пользователя в таблице, измените данные в полях ввода и нажмите "Обновить".</p>

<h3>Удаление</h3>
<p>Выберите пользователя и нажмите "Удалить" — запись будет удалена вместе с адресом (при условии каскадного удаления).</p>

<hr>

<h2>Особенности реализации</h2>
<ul>
    <li>Использование <code>SqlConnection</code> и <code>SqlCommand</code> для выполнения запросов.</li>
    <li>Обработка параметризованных запросов для защиты от SQL-инъекций.</li>
    <li>Поддержка транзакций при добавлении/изменении пользователя (связь Address + User).</li>
    <li>Событийная модель обновления данных (DataGridView обновляется автоматически).</li>
</ul>

<hr>

<h2>Планы по улучшению</h2>
<ul>
    <li>Добавить интерфейс на ASP.NET Core MVC</li>
    <li>Переход на Entity Framework Core</li>
    <li>Реализовать поиск пользователей по имени/email</li>
    <li>Добавить валидацию данных на клиентской стороне</li>
</ul>

<hr>

<h2>Лицензия</h2>
<p>Проект распространяется под лицензией MIT. Подробнее в файле <code>LICENSE</code>.</p>

<hr>

<h2>Автор</h2>
<p>
    <strong>Ваше Имя</strong><br>
    GitHub: <a href="https://github.com/your-username">@your-username</a><br>
    Email: your.email@example.com
</p>

<hr>

<p>Если у вас есть вопросы или предложения — создавайте <strong>Issue</strong> или пишите напрямую!</p>

</body>
</html>
