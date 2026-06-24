using System;
using ADONET_Final_project.Data;
using ADONET_Final_project.UI;

string path = ".env";
string connectionString = File.ReadAllText(path);

var userRepo = new UserRepository(connectionString);
var groupRepo = new GroupRepository(connectionString);
var addressRepo = new AddressRepository(connectionString);

var ui = new ConsoleUI(userRepo, groupRepo, addressRepo);
ui.ShowMenu();