using System;
using UnicornNet.Data;

namespace UnicornNet
{
    public class UcException : Exception
    {

        public UcException(UcErr ucErr) : base(ucErr.ToString())
        {
            UcicornError = ucErr;
        }

        public UcException(string message, UcErr ucErr) : base(message)
        {
        }
        
        public UcErr UcicornError { get; }
    }
}