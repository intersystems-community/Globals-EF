
namespace GlobalsFramework.Utils.ConnectionManagement
{
    internal class ConnectionParameters
    {
        internal ConnectionParameters()
        {
            ClearData();
        }

        internal string Namespace { get; private set; }
        internal string User { get; private set; }
        internal string Password { get; private set; }
        internal bool IsAssigned { get; private set; }

        internal void AssignParameters(string namespc, string user, string password)
        {
            Namespace = namespc;
            User = user;
            Password = password;
            IsAssigned = true;
        }

        internal void ClearData()
        {
            IsAssigned = false;
        }
    }
}
