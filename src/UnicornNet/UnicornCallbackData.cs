using System;

namespace UnicornNet
{
    public class UnicornCallbackData
    {
        public UnicornCallbackData(Delegate callback, object userData)
        {
            Callback = callback;
            UserData = userData;
        }
        
        public Delegate Callback { get; }
        public object UserData { get; }
    }
}