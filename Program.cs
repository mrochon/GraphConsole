// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;

IConfiguration Configuration = new ConfigurationBuilder()
  .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
  .AddUserSecrets<Program>()
  .AddEnvironmentVariables()
  .AddCommandLine(args)
  .Build();

Console.WriteLine("Starting GraphConsole");

var updater = new GraphConsole.UpdateUsers(Configuration);
var updated = updater.Update().Result;
Console.WriteLine("Listed: {0} users", updated);

