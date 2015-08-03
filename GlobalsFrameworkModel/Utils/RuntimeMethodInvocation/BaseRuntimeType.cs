using System;

namespace GlobalsFramework.Utils.RuntimeMethodInvocation
{
    internal abstract class BaseRuntimeType : IRuntimeType
    {
        protected BaseRuntimeType(Type underlyingType)
        {
            UnderlyingType = underlyingType;
        }

        public Type UnderlyingType { get; private set; }
    }

    internal sealed class RuntimeType1 : BaseRuntimeType
    {
        public RuntimeType1(Type underlyingType) : base(underlyingType) { }
    }

    internal sealed class RuntimeType2 : BaseRuntimeType
    {
        public RuntimeType2(Type underlyingType) : base(underlyingType) { }
    }

    internal sealed class RuntimeType3 : BaseRuntimeType
    {
        public RuntimeType3(Type underlyingType) : base(underlyingType) { }
    }

    internal sealed class RuntimeType4 : BaseRuntimeType
    {
        public RuntimeType4(Type underlyingType) : base(underlyingType) { }
    }

    internal sealed class RuntimeType5 : BaseRuntimeType
    {
        public RuntimeType5(Type underlyingType) : base(underlyingType) { }
    }
}
