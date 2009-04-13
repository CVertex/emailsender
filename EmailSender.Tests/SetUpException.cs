using System;

namespace EmailSender.Tests
{
    public class SetUpException:Exception
    {
        public SetUpException(string message) : base(message) {}

        public SetUpException(string message, Exception innerException) : base(message, innerException) { }
    }
}