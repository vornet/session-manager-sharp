namespace VorNet.SessionManagerSharp.Protocol.Models
{
    internal class OpenDataChannelInput
    {
        public string? MessageSchemaVersion { get; set; }

        public string? RequestId { get; set; }

        public string? TokenValue { get; set; }

        public string? ClientId { get; set; }
    }
}
