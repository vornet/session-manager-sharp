using VorNet.SessionManagerSharp.Protocol.Models;

namespace VorNet.SessionManagerSharp.Protocol.Serializers
{
    internal interface IClientMessageSerializer
    {
        byte[] Serialize(ClientMessage message);

        ClientMessage Deserialize(byte[] data);
    }
}
