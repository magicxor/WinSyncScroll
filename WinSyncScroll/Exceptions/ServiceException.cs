namespace WinSyncScroll.Exceptions;

public class ServiceException : Exception
{
    public ServiceException()
    {
    }

    public ServiceException(string errorMessage)
        : base(errorMessage)
    {
    }

    public ServiceException(string errorMessage, Exception innerException)
        : base(errorMessage, innerException)
    {
    }
}
