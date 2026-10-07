namespace BackEnd.Refactoring.Infra.Storage.Azure;

/// <summary>
/// Adapts Azure Blob Storage to the domain's IReportStorage interface.
///
/// This is the ONLY file in the solution that references Azure.Storage.Blobs.
/// Its single responsibility is translation: domain concepts in, Azure SDK calls out.
///
/// Note the exception translation in each method: Azure throws RequestFailedException,
/// but the domain catches StorageException. The adapter is the translator.
/// </summary>
public sealed class AzureBlobStorageAdapter : IReportStorage
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName;

    public AzureBlobStorageAdapter(BlobServiceClient blobServiceClient, string containerName)
    {
        _blobServiceClient = blobServiceClient;
        _containerName = containerName;
    }

    public async Task SaveReportAsync(string reportName, string reportContent, CancellationToken cancellationToken = default)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            var blobClient = containerClient.GetBlobClient(reportName);
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(reportContent));
            await blobClient.UploadAsync(stream, overwrite: true, cancellationToken: cancellationToken);
        }
        catch (Azure.RequestFailedException ex)
        {
            throw new StorageException(reportName, "Failed to save report to Azure Blob Storage.", ex);
        }
    }


    public async Task<string> LoadReportAsync(string reportName, CancellationToken cancellationToken = default)
    {
        try
        {

            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(reportName);

            if (await blobClient.ExistsAsync(cancellationToken))
            {
                var downloadInfo = await blobClient.DownloadAsync(cancellationToken);
                using var reader = new StreamReader(downloadInfo.Value.Content);
                return await reader.ReadToEndAsync();
            }
        }
        catch (Azure.RequestFailedException ex)
        {
            throw new StorageException(reportName, "Failed to load report from Azure Blob Storage.", ex);
        }

    }
}
