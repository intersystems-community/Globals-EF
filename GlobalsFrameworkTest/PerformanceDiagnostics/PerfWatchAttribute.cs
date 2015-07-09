using System;

namespace GlobalsFrameworkTest.PerformanceDiagnostics
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class PerfWatchAttribute : Attribute
    {

    }
}
