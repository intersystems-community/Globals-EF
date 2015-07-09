using System.Threading;
using InterSystems.Globals;

namespace GlobalsFramework.Utils.ConnectionManagement
{
    internal static class ConnectionManager
    {
        //  ConnectionManager used for initializing connection in the another thread with increased stack size,
        //because opening of connection by InterSystems.GlobalsDB.dll needs default stack size for windows applications(1MB),
        //but max stack size for IIS - is 256 kb, and connection will not be created.
        //  That`s why I manually increase stack size to 1MB with own thread. I create only one thread per application to decrease overhead of
        //creating and releasing thread, because connection can be opened and closed very often during application lifetime.

        private static readonly ManualResetEvent WorkerEvent = new ManualResetEvent(false);
        private static readonly ThreadResult<Connection> ThreadResult = new ThreadResult<Connection>();
        private static readonly ConnectionParameters Parameters = new ConnectionParameters();
        private static readonly Mutex Mutex = new Mutex();

        static ConnectionManager()
        {
            var thread = new Thread(BackgroundWorker, 1024*1024);
            thread.Start();
        }

        internal static Connection GetOpenedConnectionSyncronously()
        {
            Mutex.WaitOne();

            Parameters.ClearData();

            WorkerEvent.Set();
            ThreadResult.WaitHandle.WaitOne();

            var result = ThreadResult.Result;

            ThreadResult.WaitHandle.Reset();
            Mutex.ReleaseMutex();

            return result;
        }

        internal static Connection GetOpenedConnectionSyncronously(string namespc, string user, string password)
        {
            Mutex.WaitOne();

            Parameters.AssignParameters(namespc, user, password);

            WorkerEvent.Set();
            ThreadResult.WaitHandle.WaitOne();

            var result = ThreadResult.Result;

            ThreadResult.WaitHandle.Reset();
            Mutex.ReleaseMutex();

            return result;
        }

        private static void BackgroundWorker()
        {
            while (true)
            {
                WorkerEvent.WaitOne();

                var connection = ConnectionContext.GetConnection();
                //connection can be opened by another DataContext
                if (!connection.IsConnected())
                {
                    if (Parameters.IsAssigned)
                        connection.Connect(Parameters.Namespace, Parameters.User, Parameters.Password);
                    else
                        connection.Connect();
                }

                ThreadResult.Result = connection;
                ThreadResult.WaitHandle.Set();
                WorkerEvent.Reset();
            }
        }
    }
}
