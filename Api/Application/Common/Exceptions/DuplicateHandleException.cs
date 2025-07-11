using System;

namespace Api.Application.Common.Exceptions
{
    public class DuplicateHandleException : Exception
    {
        public string Handle { get; }

        public DuplicateHandleException(string handle) 
            : base($"A document with handle '{handle}' already exists.")
        {
            Handle = handle;
        }

        public DuplicateHandleException(string handle, Exception innerException) 
            : base($"A document with handle '{handle}' already exists.", innerException)
        {
            Handle = handle;
        }
    }
}
