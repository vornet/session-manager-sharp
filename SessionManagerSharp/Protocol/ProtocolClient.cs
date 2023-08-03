using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using VorNet.SessionManagerSharp.Protocol.ClientWebSocketAbstraction;
using VorNet.SessionManagerSharp.Protocol.Models;
using VorNet.SessionManagerSharp.Protocol.Serializers;

namespace VorNet.SessionManagerSharp.Protocol
{
    internal class ProtocolClient : IProtocolClient
    {
        private readonly IClientWebSocket _clientWebSocket;
        private readonly IClientMessageSerializer _clientMessageSerializer;

        public ProtocolClient(IClientWebSocket clientWebSocket, IClientMessageSerializer clientMessageSerializer)
        {
            _clientWebSocket = clientWebSocket ?? throw new ArgumentNullException(nameof(clientWebSocket));
            _clientMessageSerializer = clientMessageSerializer ?? throw new ArgumentNullException(nameof(clientWebSocket));
        }

        public bool IsConnected { get; private set; }

        public async Task ConnectAsync(Uri streamUri, string tokenValue)
        {
            await _clientWebSocket.ConnectAsync(streamUri, CancellationToken.None);

            var handShakeMessage = JsonSerializer.Serialize(new OpenDataChannelInput
            {
                MessageSchemaVersion = "1.0",
                RequestId = Guid.NewGuid().ToString(),
                TokenValue = tokenValue,
                ClientId = Guid.NewGuid().ToString(),
            });

            await _clientWebSocket.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(handShakeMessage)), WebSocketMessageType.Text, true, CancellationToken.None);

            IsConnected = true;
        }

        public async Task<ClientMessage> ReadAndAcknowledgeNextMessageAsync()
        {
            ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[8192]);
            using var ms = new MemoryStream();

            WebSocketReceiveResult result;
            do
            {
                result = await _clientWebSocket.ReceiveAsync(buffer, CancellationToken.None);
                ms.Write(buffer.Array, buffer.Offset, result.Count);
            }
            while (!result.EndOfMessage);


            ClientMessage clientMessage = _clientMessageSerializer.Deserialize(ms.ToArray());

            if (clientMessage.MessageType == "output_stream_data")
            {
                var acknowledgeContent = new AcknowledgeContent()
                {
                    MessageType = clientMessage.MessageType,
                    MessageId = FormatGuid(clientMessage.MessageId),
                    SequenceNumber = clientMessage.SequenceNumber,
                    IsSequentialMessage = true,
                };

                var acknowledgeContentJson = JsonSerializer.Serialize(acknowledgeContent);

                var payloadBytes = Encoding.ASCII.GetBytes(acknowledgeContentJson);

                var ackMessage = new ClientMessage
                {
                    MessageType = "acknowledge",
                    SchemaVersion = 1,
                    CreatedDate = (ulong)DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    SequenceNumber = 0,
                    Flags = 3,
                    MessageId = Guid.NewGuid().ToByteArray(),
                    Payload = payloadBytes,
                };

                var awkMessageBytes = _clientMessageSerializer.Serialize(ackMessage);

                await _clientWebSocket.SendAsync(new ArraySegment<byte>(awkMessageBytes), WebSocketMessageType.Binary, true, CancellationToken.None);
            }

            return clientMessage;
        }

        public async Task SendMessageAsync(ClientMessage clientMessage)
        {
            var awkMessageBytes = _clientMessageSerializer.Serialize(clientMessage);

            await _clientWebSocket.SendAsync(new ArraySegment<byte>(awkMessageBytes), WebSocketMessageType.Binary, true, CancellationToken.None);

            ArraySegment<Byte> buffer = new ArraySegment<byte>(new Byte[8192]);
            using var ms = new MemoryStream();

            WebSocketReceiveResult result;
            do
            {
                result = await _clientWebSocket.ReceiveAsync(buffer, CancellationToken.None);
                ms.Write(buffer.Array, buffer.Offset, result.Count);
            }
            while (!result.EndOfMessage);

            clientMessage = _clientMessageSerializer.Deserialize(ms.ToArray());
        }

        private string FormatGuid(byte[] messageId)
        {
            return (Convert.ToHexString(messageId, 0, 4) + "-"
                + Convert.ToHexString(messageId, 4, 2) + "-"
                + Convert.ToHexString(messageId, 6, 2) + "-"
                + Convert.ToHexString(messageId, 8, 2) + "-"
                + Convert.ToHexString(messageId, 10, 6)).ToLower();
        }
    }
}
