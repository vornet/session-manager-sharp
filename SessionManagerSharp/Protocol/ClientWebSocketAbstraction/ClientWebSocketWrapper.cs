using System.Net.WebSockets;

namespace VorNet.SessionManagerSharp.Protocol.ClientWebSocketAbstraction
{
    internal class ClientWebSocketWrapper : IClientWebSocket
    {
        public ClientWebSocket _clientWebSocket;

        public ClientWebSocketWrapper(ClientWebSocket clientWebSocket)
        {
            _clientWebSocket = clientWebSocket ?? throw new ArgumentNullException(nameof(clientWebSocket));
        }

        public Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
        {
            return _clientWebSocket.ConnectAsync(uri, cancellationToken);
        }

        public Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            return _clientWebSocket.ReceiveAsync(buffer, cancellationToken);
        }

        public Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken)
        {
            return _clientWebSocket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);
        }
    }
}
