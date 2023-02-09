// See https://aka.ms/new-console-template for more information
using Microsoft.Identity.Client;

Console.WriteLine("Hello, World!");

var updater = new GraphConsole.UpdateUsers();
var updated = updater.Update().Result;
Console.WriteLine("Listed: {0} users", updated);

