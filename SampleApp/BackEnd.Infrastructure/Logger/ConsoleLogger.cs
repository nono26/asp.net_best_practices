
using BackEnd.Logic.Interface;

namespace BackEnd.Infrastructure.Logger;

/// <summary>
/// Console Logger, it logs information to the console. It's a implementation of ILogger
/// </summary>
/// <typeparam name="T"></typeparam>
public class ConsoleLogger<T> : ILogger<T>
{
    private readonly string _name;

    public ConsoleLogger()
    {
        _name = typeof(T).Name;
    }

    /// <summary>
    /// Log an error message
    /// </summary>
    /// <param name="exception"></param>
    /// <param name="message"></param>
    /// <param name="args"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void LogError(Exception exception, string message, params object[] args)
    {
        Console.WriteLine(string.Concat(exception.Message, message), _name, args);
    }

    /// <summary>
    /// Log an information message
    /// </summary>
    /// <param name="message"></param>
    /// <param name="args"></param>
    public void LogInformation(string message, params object[] args)
    {
        Console.WriteLine(message, _name, args);
    }
}
