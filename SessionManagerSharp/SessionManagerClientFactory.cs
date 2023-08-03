using Amazon.Extensions.NETCore.Setup;
using Amazon.SimpleSystemsManagement;
using Microsoft.Extensions.Configuration;
using VorNet.SessionManagerSharp.Protocol;
using VorNet.SessionManagerSharp.Protocol.ClientWebSocketAbstraction;
using VorNet.SessionManagerSharp.Protocol.Serializers;

namespace VorNet.SessionManagerSharp
{
    public class SessionManagerClientFactory : ISessionManagerClientFactory
    {
        private readonly IAmazonSimpleSystemsManagement _ssmClient;
        private readonly IProtocolClient _protocolClient;

        public SessionManagerClientFactory(IConfiguration configuration)
        {
            AWSOptions awsOptions = configuration.GetAWSOptions();

            _ssmClient = awsOptions.CreateServiceClient<IAmazonSimpleSystemsManagement>();
            _protocolClient = new ProtocolClient(new ClientWebSocketWrapper(new System.Net.WebSockets.ClientWebSocket()), new ClientMessageSerializer());
        }

        public ISessionManagerClient Create(string target)
        {
            return new SessionManagerClient(_ssmClient, _protocolClient, target);
        }
    }
}
