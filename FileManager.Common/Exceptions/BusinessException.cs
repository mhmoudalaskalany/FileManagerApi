using System;

namespace FileManager.Common.Exceptions
{
    public class BusinessException : BaseException
    {

        public BusinessException() : base($"Error From Server")
        {

        }
        public BusinessException(string message) : base(message)
        {

        }

        public BusinessException(string message, Exception innerException) : base(message, innerException)
        {

        }

        
    }
}
