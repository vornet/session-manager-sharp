using System.Net.WebSockets;

namespace VorNet.SessionManagerSharp.Protocol.ClientWebSocketAbstraction
{
    internal interface IClientWebSocket
    {
        Task ConnectAsync(Uri uri, CancellationToken cancellationToken);

        Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken);

        Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken);
    }
}
