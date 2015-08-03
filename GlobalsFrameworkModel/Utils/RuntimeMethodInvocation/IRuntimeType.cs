using System;

namespace GlobalsFramework.Utils.RuntimeMethodInvocation
{
    internal interface IRuntimeType
    {
        Type UnderlyingType { get; }
    }
}
