namespace BackEnd.Logic.Interface;
public interface ILogger<T>
{
    void LogInformation(string message, params object[] args);
    void LogError(Exception exception, string message, params object[] args);
}