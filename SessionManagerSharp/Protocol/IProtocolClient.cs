using VorNet.SessionManagerSharp.Protocol.Models;

namespace VorNet.SessionManagerSharp.Protocol
{
    internal interface IProtocolClient
    {
        bool IsConnected { get; }

        Task ConnectAsync(Uri streamUri, string tokenValue);

        Task<ClientMessage> ReadAndAcknowledgeNextMessageAsync();

        Task SendMessageAsync(ClientMessage clientMessage);
    }
}