using VorNet.SessionManagerSharp.Protocol.Models;

namespace VorNet.SessionManagerSharp.Protocol
{
    internal interface IProtocolClient
    {
        public delegate void MessageReceived(ClientMessage message);
        public delegate void OutputStreamReceived(byte[] outputStream);

        bool IsConnected { get; }

        Task ConnectAsync(Uri streamUri, string tokenValue);

        Task SendMessageAsync(ClientMessage clientMessage);

        event MessageReceived OnMessageReceived;
        event OutputStreamReceived OnOutputStreamReceived;
    }
}