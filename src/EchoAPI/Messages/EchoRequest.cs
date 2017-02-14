using System;

namespace EchoAPI.Messages
{
    public class EchoRequest
    {
        public string Message { get; }

        public EchoRequest(String message)
        {
            Message = message;
        }
    }
}
