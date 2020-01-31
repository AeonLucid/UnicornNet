using System;
using System.Runtime.Serialization;
using UnicornNet.Data;

namespace UnicornNet
{
    public class UcException : Exception
    {
        public UcException(UcErr message) : base(message.ToString())
        {
        }

        protected UcException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public UcException(string message) : base(message)
        {
        }

        public UcException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}