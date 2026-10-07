using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Domain.Exceptions;

/// <summary>
/// Domain-level storage exception. The adapter catches SDK exceptions
/// (e.g. Azure RequestFailedException) and re-throws as this type,
/// keeping the domain's exception hierarchy free of external dependencies.
/// </summary>
public class StorageException : Exception
{
    public string Path { get; }

    public StorageException(string path, string message, Exception? inner = null)
        : base(message, inner) => Path = path;
}
