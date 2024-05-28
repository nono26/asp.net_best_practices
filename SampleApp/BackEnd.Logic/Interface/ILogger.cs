namespace BackEnd.Logic.Interface;
public interface ILogger<T>
{
    void LogInformation(string message, params object[] args);
}