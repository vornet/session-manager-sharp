using Amazon.SimpleSystemsManagement.Model;
using Amazon.SimpleSystemsManagement;
using System.Text;
using VorNet.SessionManagerSharp.Protocol;
using VorNet.SessionManagerSharp.Protocol.Models;

namespace VorNet.SessionManagerSharp
{
    internal class SessionManagerClient : ISessionManagerClient
    {
        private readonly IAmazonSimpleSystemsManagement _ssmClient;
        private readonly IProtocolClient _protocalClient;
        private readonly string _target;

        public SessionManagerClient(IAmazonSimpleSystemsManagement ssmClient, IProtocolClient protocolClient, string target)
        {
            _ssmClient = ssmClient ?? throw new ArgumentNullException(nameof(ssmClient));
            _protocalClient = protocolClient ?? throw new ArgumentNullException(nameof(protocolClient));
            _target = target ?? throw new ArgumentNullException(nameof(target));
        }

        public async Task<string[]> SendStdOutAsync(string commandText)
        {
            if (!_protocalClient.IsConnected) { await ConnectAsync(); }

            await _protocalClient.ReadAndAcknowledgeNextMessageAsync();

            // Set Terminal size - not really needed.
            //await SendMessageAsync(wsClient, new ClientMessage
            //{
            //    MessageType = "input_stream_data",
            //    SchemaVersion = 1,
            //    CreatedDate = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            //    SequenceNumber = 0,
            //    Flags = 0,
            //    MessageId = Guid.NewGuid().ToByteArray(),
            //    PayloadType = 3,
            //    Payload = Encoding.ASCII.GetBytes("{\"cols\":232,\"rows\":22}"),
            //});

            //await ReadAndAcknowledgeNextMessageAsync(wsClient);

            await _protocalClient.SendMessageAsync(new ClientMessage
            {
                MessageType = "input_stream_data",
                SchemaVersion = 1,
                CreatedDate = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                SequenceNumber = 0,
                Flags = 0,
                MessageId = Guid.NewGuid().ToByteArray(),
                PayloadType = 1,
                Payload = Encoding.ASCII.GetBytes(commandText),
            });

            // Read the echo back.
            await _protocalClient.ReadAndAcknowledgeNextMessageAsync();

            // Read actual response from command.
            ClientMessage clientMessage = await _protocalClient.ReadAndAcknowledgeNextMessageAsync();

            return new string[] { Encoding.ASCII.GetString(clientMessage.Payload) };
        }

        public async Task SendTextFileStreamAsync(Stream fileStream)
        {
            if (!_protocalClient.IsConnected) { await ConnectAsync(); }
        }

        private async Task ConnectAsync()
        {
            StartSessionResponse response = await _ssmClient.StartSessionAsync(new StartSessionRequest()
            {
                Target = _target
            });

            await _protocalClient.ConnectAsync(new Uri(response.StreamUrl), response.TokenValue);
        }
    }
}
