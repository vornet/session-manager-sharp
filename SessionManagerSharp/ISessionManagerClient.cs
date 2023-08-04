namespace VorNet.SessionManagerSharp
{
    public interface ISessionManagerClient
    {
        Task<string> SendStdOutAsync(string commandText);

        Task SendTextFileStreamAsync(Stream stream, string destFilename);
    }
}