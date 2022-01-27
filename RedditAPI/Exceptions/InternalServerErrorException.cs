using System;

namespace RedditAPI.Exceptions
{
    public class InternalServerErrorException : Exception
    {
        public InternalServerErrorException(string message) : base(message)
        {

        }
    }
}
