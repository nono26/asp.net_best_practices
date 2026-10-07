using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Refactoring.Application.Abstration.Srlztn;
namespace BackEnd.Domain.Services;

public class OrderService(IReportStorage reportStorage, ISerializer serializer)
{
    public async Task SaveAsync(Order order, CancellationToken cancellationToken = default)
    {
        var reportName = $"Order_{order.Id}.json";
        var reportContent = serializer.Serialize(order);
        await reportStorage.SaveReportAsync(reportName, reportContent, cancellationToken);
    }

    public async Task<Order> LoadAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var reportName = $"Order_{orderId}.json";
        var reportContent = await reportStorage.LoadReportAsync(reportName, cancellationToken);
        return serializer.Deserialize<Order>(reportContent);
    }
}
