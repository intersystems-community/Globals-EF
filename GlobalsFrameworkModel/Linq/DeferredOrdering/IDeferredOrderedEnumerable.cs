using System.Linq;

namespace GlobalsFramework.Linq.DeferredOrdering
{
    internal interface IDeferredOrderedEnumerable
    {
        IOrderedEnumerable<TResult> GetLoadedOrderedEnumerable<TResult>();
    }
}
