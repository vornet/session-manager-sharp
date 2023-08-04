using Microsoft.Extensions.Configuration;
using VorNet.SessionManagerSharp;

var builder = new ConfigurationBuilder()  
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

IConfiguration config = builder.Build();

ISessionManagerClientFactory factory = new SessionManagerClientFactory(config);
ISessionManagerClient client = factory.Create("i-0a42f2bd61658a835");

// Run a command.
Console.WriteLine(await client.SendStdOutAsync("sudo ls /home/ubuntu"));

// Send a text file.
using var fileStream = new FileStream("example.txt", FileMode.Open, FileAccess.Read);
await client.SendTextFileStreamAsync(fileStream, "/home/ubuntu/example.txt");