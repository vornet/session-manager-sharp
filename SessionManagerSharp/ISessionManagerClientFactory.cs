namespace VorNet.SessionManagerSharp
{
    public interface ISessionManagerClientFactory
    {
        ISessionManagerClient Create(string target);
    }
}
