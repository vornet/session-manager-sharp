namespace VorNet.SessionManagerSharp
{
    public interface ISessionManagerClient
    {
        Task SendStdOutAsync(string commandText);

        Task<string> WaitForStdInAsync(string text);

        Task SendTextFileStreamAsync(Stream stream, string destFilename);
    }
}