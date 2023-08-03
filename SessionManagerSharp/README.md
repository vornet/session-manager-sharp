# SessionManagerSharp

An unofficial AWS Systems Managers' Session Manager client for C#/.NET based on https://github.com/aws/session-manager-plugin.

THe primary motivation is to enable automation through it from .NET.

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

string[] response = await client.SendStdOutAsync("ls\r");

foreach (string responseLine in response)
{
    Console.WriteLine(responseLine);
}
```