using Amazon.SimpleSystemsManagement.Model;
using Amazon.SimpleSystemsManagement;
using System.Text;
using VorNet.SessionManagerSharp.Protocol;
using VorNet.SessionManagerSharp.Protocol.Models;
using System.IO;
using Amazon.Runtime;

namespace VorNet.SessionManagerSharp
{
    internal class SessionManagerClient : ISessionManagerClient
    {
        private readonly IAmazonSimpleSystemsManagement _ssmClient;
        private readonly IProtocolClient _protocalClient;
        private readonly string _target;

        private int _currentSequenceNumber = 0;

        public SessionManagerClient(IAmazonSimpleSystemsManagement ssmClient, IProtocolClient protocolClient, string target)
        {
            _ssmClient = ssmClient ?? throw new ArgumentNullException(nameof(ssmClient));
            _protocalClient = protocolClient ?? throw new ArgumentNullException(nameof(protocolClient));
            _target = target ?? throw new ArgumentNullException(nameof(target));
        }

        public async Task<string> SendStdOutAsync(string commandText)
        {
            if (!_protocalClient.IsConnected) { await ConnectAsync(); }

            if (!commandText.EndsWith("\r"))
            {
                // Always add a carriage return to commands.
                commandText += "\r";
            }

            return await SendStdOutRawAsync(Encoding.ASCII.GetBytes(commandText));
        }

        public async Task SendTextFileStreamAsync(Stream stream, string destFilename)
        {
            if (!_protocalClient.IsConnected) { await ConnectAsync(); }

            // Start streaming stdin to the file.
            await SendStdOutAsync($"sudo cp /dev/stdin {destFilename}");

            await Task.Delay(1000);

            // Stream the file.
            using var streamReader = new StreamReader(stream, new ASCIIEncoding());
            await SendStdOutAsync(await streamReader.ReadToEndAsync());

            // End the stream.
            await SendStdOutRawAsync(new byte[] { 13, 4 });

            // Give other users rw access to the file.
            await SendStdOutAsync($"sudo chmod o+rw {destFilename}");
        }

        private async Task<string> SendStdOutRawAsync(byte[] commandText)
        {
            if (!_protocalClient.IsConnected) { await ConnectAsync(); }

            await _protocalClient.SendMessageAsync(new ClientMessage
            {
                MessageType = "input_stream_data",
                SchemaVersion = 1,
                CreatedDate = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                SequenceNumber = _currentSequenceNumber++,
                Flags = 0,
                MessageId = Guid.NewGuid().ToByteArray(),
                PayloadType = 1,
                Payload = commandText,
            });

            // Wait for the echo.
            ClientMessage clientMessage;
            int retry = 5;
            do
            {
                clientMessage = await _protocalClient.ReadAndAcknowledgeNextMessageAsync();
                retry--;
            }
            while (retry > 0 && Encoding.ASCII.GetString(commandText).Trim() != Encoding.ASCII.GetString(clientMessage.Payload).Trim());

            // Read actual response from command.
            clientMessage = await _protocalClient.ReadAndAcknowledgeNextMessageAsync();
            return Encoding.ASCII.GetString(clientMessage.Payload);
        }

        private async Task ConnectAsync()
        {
            StartSessionResponse response = await _ssmClient.StartSessionAsync(new StartSessionRequest()
            {
                Target = _target
            });

            await _protocalClient.ConnectAsync(new Uri(response.StreamUrl), response.TokenValue);
            await _protocalClient.ReadAndAcknowledgeNextMessageAsync();
        }
    }
}
