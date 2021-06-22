using System;

// <summary>
// Namespace where we define custom exceptions to be thrown by Controllers
// </summary>
namespace Gordon360.Exceptions.CustomExceptions
{
    public class ResourceNotFoundException : Exception
    {
        public string ExceptionMessage { get; set; }
    }
    public class ResourceCreationException : Exception
    {
        public string ExceptionMessage { get; set; }
    }
    public class BadInputException : Exception
    {
        public string ExceptionMessage { get; set; }
    }
}
