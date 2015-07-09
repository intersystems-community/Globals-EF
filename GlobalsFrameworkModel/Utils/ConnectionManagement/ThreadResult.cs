using System.Threading;

namespace GlobalsFramework.Utils.ConnectionManagement
{
    internal class ThreadResult<T>
    {
        internal T Result { get; set; }
        internal ManualResetEvent WaitHandle { get; private set; }

        internal ThreadResult()
        {
            WaitHandle = new ManualResetEvent(false);
        }
    }
}
