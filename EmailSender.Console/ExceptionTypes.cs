using System;

namespace EmailSender.Console
{
    public class SenderExceptionBase:Exception
    {
        public SenderExceptionBase(string message):base(message)
        {
        }

        public SenderExceptionBase(string message, Exception exception)
            : base(message,exception) {
        }
    }

    public class SenderCreationException : SenderExceptionBase {
        public SenderCreationException(string msg)
            : base(msg) {
        }

    }

    public class SenderInvocationException : SenderExceptionBase {
        public SenderInvocationException(string msg)
            : base(msg) {
        }
        public SenderInvocationException(string msg, Exception inner)
            : base(msg,inner) {
        }
    }

    public class NoParametersException : SenderExceptionBase {
        public NoParametersException()
            : base("Not enough supplied parameters") {
        }

    }
}