namespace WinSyncScroll.Exceptions;

public class ServiceException : Exception
{
    public ServiceException(string errorMessage) : base(errorMessage)
    {
    }

    public ServiceException(string errorMessage, Exception innerException) : base(errorMessage, innerException)
    {
    }
}
