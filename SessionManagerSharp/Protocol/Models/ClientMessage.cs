namespace VorNet.SessionManagerSharp.Protocol.Models
{
    internal class ClientMessage
    {
        public uint HeaderLength { get; set; }

        public string? MessageType { get; set; }

        public uint SchemaVersion { get; set; }

        public ulong CreatedDate { get; set; }

        public long SequenceNumber { get; set; }

        public ulong Flags { get; set; }

        public byte[]? MessageId { get; set; }

        public byte[]? PayloadDigest { get; set; }

        public uint PayloadType { get; set; }

        public uint PayloadLength { get; set; }

        public byte[]? Payload { get; set; }
    }
}
