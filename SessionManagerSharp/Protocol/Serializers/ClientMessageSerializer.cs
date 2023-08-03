using System.Security.Cryptography;
using System.Text;
using VorNet.SessionManagerSharp.Protocol.Models;

namespace VorNet.SessionManagerSharp.Protocol.Serializers
{
    internal class ClientMessageSerializer : IClientMessageSerializer
    {
        const int MessageTypeLength = 32;
        const int PayloadDigestLength = 32;

        public byte[] Serialize(ClientMessage message)
        {
            message.HeaderLength = 116;
            message.PayloadLength = (uint)message.Payload.Length;

            using SHA256 mySHA256 = SHA256.Create();
            message.PayloadDigest = mySHA256.ComputeHash(message.Payload);

            using MemoryStream memoryStream = new MemoryStream();

            using EndiannessAwareBinaryWriter binaryWriter = new EndiannessAwareBinaryWriter(memoryStream, endianness: EndiannessAwareBinaryWriter.Endianness.Big);
            binaryWriter.Write(message.HeaderLength);

            byte[] messageTypeBuffer = Enumerable.Repeat((byte)0x20, MessageTypeLength).ToArray();
            Encoding.ASCII.GetBytes(message.MessageType, 0, message.MessageType.Length, messageTypeBuffer, 0);
            binaryWriter.Write(messageTypeBuffer);

            binaryWriter.Write(message.SchemaVersion);
            binaryWriter.Write(message.CreatedDate);
            binaryWriter.Write(message.SequenceNumber);
            binaryWriter.Write(message.Flags);

            IEnumerable<byte> firstHalf = message.MessageId.Take(8);
            IEnumerable<byte> secondHalf = message.MessageId.Skip(8).Take(8);

            binaryWriter.Write(secondHalf.Concat(firstHalf).ToArray());

            binaryWriter.Write(message.PayloadDigest);
            binaryWriter.Write(message.PayloadType);
            binaryWriter.Write(message.PayloadLength);
            binaryWriter.Write(message.Payload);

            return memoryStream.ToArray();
        }

        public ClientMessage Deserialize(byte[] data)
        {
            ClientMessage clientMessage = new ClientMessage();

            using EndiannessAwareBinaryReader binaryReader = new EndiannessAwareBinaryReader(new MemoryStream(data), EndiannessAwareBinaryReader.Endianness.Big);
            clientMessage.HeaderLength = binaryReader.ReadUInt32();
            clientMessage.MessageType = Encoding.ASCII.GetString(binaryReader.ReadBytes(MessageTypeLength)).Trim();
            clientMessage.SchemaVersion = binaryReader.ReadUInt32();
            clientMessage.CreatedDate = binaryReader.ReadUInt64();
            clientMessage.SequenceNumber = binaryReader.ReadInt64();
            clientMessage.Flags = binaryReader.ReadUInt64();
            byte[] firstHalf = binaryReader.ReadBytes(8);
            byte[] secondHalf = binaryReader.ReadBytes(8);
            clientMessage.MessageId = secondHalf.Concat(firstHalf).ToArray();
            clientMessage.PayloadDigest = binaryReader.ReadBytes(PayloadDigestLength);
            clientMessage.PayloadType = binaryReader.ReadUInt32();
            clientMessage.PayloadLength = binaryReader.ReadUInt32();
            clientMessage.Payload = binaryReader.ReadBytes((int)clientMessage.PayloadLength);

            return clientMessage;
        }
    }
}
