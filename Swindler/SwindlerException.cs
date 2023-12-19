using System;

namespace Swindler;

/// <summary>
/// Exception that can be thrown if the Swindler runs into trouble
/// </summary>
[Serializable]
public class SwindlerException : ApplicationException
{
    /// <summary>
    /// Creates the exception with the given message
    /// </summary>
    public SwindlerException(string message) : base(message)
    {
    }

    /// <summary>
    /// Creates the exception with the given message and inner exception
    /// </summary>
    public SwindlerException(string message, Exception innerException) : base(message, innerException)
    {
    }
}