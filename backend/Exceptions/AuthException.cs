namespace backend.Exceptions
{
    public class UserNotFoundException(string message) : Exception(message)
    {
    }
    public class PasswordMismatchException(string message) : Exception(message)
    {
    }
    // already exist
    public class UserAlreadyExistException(string message) : Exception(message)
    {
    }
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException() : base("Invalid token") { }
    }
}
