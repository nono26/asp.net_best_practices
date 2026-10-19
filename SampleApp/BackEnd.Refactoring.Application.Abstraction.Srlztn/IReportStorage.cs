namespace BackEnd.Refactoring.Application.Abstraction.Srlztn;

public interface IReportStorage
{
    Task SaveReportAsync(string reportName, string reportContent, CancellationToken cancellationToken = default);
    Task<string> LoadReportAsync(string reportName, CancellationToken cancellationToken = default);
}