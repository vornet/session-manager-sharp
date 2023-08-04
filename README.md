# SessionManagerSharp

An unofficial AWS Systems Managers' Session Manager client for C#/.NET based on https://github.com/aws/session-manager-plugin.

The primary motivation and use-case is to enable automation.

Two methods are supported:

- `Task<string> SendStdOutAsync(string commandText)` sends a command over standard input and returns the response from it.
- `Task SendTextFileStreamAsync(Stream stream, string destFilename)` streams a text file over standard input to a destination filename.  This is experimental.

## Example Usage

```csharp
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
}
```