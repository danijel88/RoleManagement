using System;

namespace RoleManagement.Core.Exceptions
{
    public class AlreadyExistException : Exception
    {
        public AlreadyExistException(string message) : base(message)
        {
            throw new ArgumentException(message);
        }

        public AlreadyExistException(Exception exception, string message)
        {
            throw new ArgumentException(message, exception);
        }
    }
}