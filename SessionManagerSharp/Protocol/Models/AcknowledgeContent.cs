using System.Text.Json.Serialization;

namespace VorNet.SessionManagerSharp.Protocol.Models
{
    internal class AcknowledgeContent
    {
        [JsonPropertyName("AcknowledgedMessageType")]
        public string? MessageType { get; set; }

        [JsonPropertyName("AcknowledgedMessageId")]
        public string? MessageId { get; set; }

        [JsonPropertyName("AcknowledgedMessageSequenceNumber")]
        public long SequenceNumber { get; set; }

        [JsonPropertyName("IsSequentialMessage")]
        public bool IsSequentialMessage { get; set; }
    }
}
