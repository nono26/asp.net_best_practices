
using BackEnd.Logic.Interface;

namespace BackEnd.Infrastructure.Logger;

public class ConsoleLogger<T> : ILogger<T>
{
    private readonly string _name;

    public ConsoleLogger()
    {
        _name = typeof(T).Name;
    }
    public void LogInformation(string message, params object[] args)
    {
        Console.WriteLine(message, _name, args);
    }
}
