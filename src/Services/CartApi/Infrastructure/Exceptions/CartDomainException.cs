using System;

namespace MicroServicesOnDocker.Services.CartApi.Infrastructure.Exceptions
{
    /// <summary>
    /// Exception type for app exceptions
    /// </summary>
    public class CartDomainException : Exception
    {
        public CartDomainException()
        { }

        public CartDomainException(string message)
            : base(message)
        { }

        public CartDomainException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
