using System.Collections.Generic;
using System.Linq;
using InterSystems.Globals;

namespace GlobalsFramework.Extensions
{
    internal static class NodeReferenceExtensions
    {
        public static NodeReference DeepCopy(this NodeReference reference)
        {
            NodeReference result = new NodeImplementation();
            result.SetName(reference.GetName());

            var subscriptCount = reference.GetSubscriptCount();

            if (subscriptCount <= 0) 
                return result;

            for (var position = 1; position <= subscriptCount; position++)
            {
                var subscript = reference.GetObjectSubscript(position);

                if (subscript is int)
                    result.AppendSubscript((int)subscript);
                else if (subscript is long)
                    result.AppendSubscript((long)subscript);
                else if (subscript is double)
                    result.AppendSubscript((double) subscript);
                else
                    result.AppendSubscript((string)subscript);
            }

            return result;
        }

        public static List<NodeReference> DeepCopy(this IEnumerable<NodeReference> references)
        {
            return references.Select(nodeReference => nodeReference.DeepCopy()).ToList();
        }

        private class NodeImplementation : NodeReference { }
    }
}
