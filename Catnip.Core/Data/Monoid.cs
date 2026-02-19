namespace Catnip.Core.Data;

using Catnip.Core;
using Catnip.Core.Data;

/// <summary>
/// A semigroup with an identity element (mempty).
/// </summary>
/// <typeparam name="M"></typeparam>
public interface Monoid<M> : Semigroup<M>
{
    // mempty : M
    HKT<M, A> MEmpty<A>();
}