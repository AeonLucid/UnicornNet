using System;

namespace UnicornNet
{
    public class UnicornCallbackData
    {
        public UnicornCallbackData(Delegate callback, Delegate userCallback, object userData)
        {
            Callback = callback;
            UserCallback = userCallback;
            UserData = userData;
        }

        public Delegate Callback { get; }
        public Delegate UserCallback { get; }
        public object UserData { get; }
    }
}