using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GlobalsFramework.Access;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.ExpressionProcessing
{
    internal class ProcessingResult
    {
        private static readonly ProcessingResult UnsuccessfulResult = new ProcessingResult(false);
        internal ProcessingResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
            IsSingleItem = false;
        }
        internal ProcessingResult(bool isSuccess, object result)
        {
            IsSuccess = isSuccess;
            Result = result;
            IsSingleItem = false;
        }
        internal ProcessingResult(bool isSuccess, object result, bool isSingleItem)
        {
            IsSuccess = isSuccess;
            Result = result;
            IsSingleItem = isSingleItem;
        }

        internal static ProcessingResult Unsuccessful
        {
            get { return UnsuccessfulResult; }
        }
        internal bool IsSuccess { get; private set; }
        internal bool IsSingleItem { get; private set; }
        internal object Result { get; private set; }

        internal IEnumerable<NodeReference> GetDeferredItems()
        {
            return Result as IEnumerable<NodeReference>;
        }
        internal List<NodeReference> GetDeferredList()
        {
            var list = Result as List<NodeReference>;
            return list ?? ((IEnumerable<NodeReference>) Result).ToList();
        }
        internal NodeReference GetDeferredItem()
        {
            return Result as NodeReference;
        }
        internal IEnumerable GetItems()
        {
            return Result as IEnumerable;
        }
        internal bool IsDeferred()
        {
            if (IsSingleItem)
                return (Result as NodeReference) != null;
            return (Result as IEnumerable<NodeReference>) != null;
        }
        internal object GetLoadedItem(Type t)
        {
            var nodeReference = Result as NodeReference;
            return nodeReference == null ? Result : DatabaseManager.ReadNode(nodeReference, t);
        }
        internal IEnumerable GetLoadedItems(Type t)
        {
            var nodeReferences = Result as IEnumerable<NodeReference>;
            var result = nodeReferences == null ? Result : DatabaseManager.ReadNodes(nodeReferences, t);
            return result as IEnumerable;
        }
    }
}
