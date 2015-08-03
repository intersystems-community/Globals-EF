using System;
using System.Collections;
using System.Collections.Concurrent;

namespace GlobalsFramework.Utils.RuntimeMethodInvocation
{
    internal sealed class RuntimeTypeBinding : IEnumerable
    {
        private readonly ConcurrentBag<Tuple<Type, IRuntimeType>> _runtimeTypes;

        internal RuntimeTypeBinding()
        {
            _runtimeTypes = new ConcurrentBag<Tuple<Type, IRuntimeType>>();
        }

        public IEnumerator GetEnumerator()
        {
            return _runtimeTypes.GetEnumerator();
        }

        internal void Add(IRuntimeType runtimeType)
        {
            _runtimeTypes.Add(new Tuple<Type, IRuntimeType>(runtimeType.GetType(), runtimeType));
        }

        internal Type GetUnderlyingType(Type type)
        {
            foreach (var runtimeType in _runtimeTypes)
            {
                if (runtimeType.Item1 == type)
                    return runtimeType.Item2.UnderlyingType;
            }

            return type;
        }
    }
}
