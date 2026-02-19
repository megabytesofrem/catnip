namespace Catnip.Core.Data;

using Catnip.Core;

/// <summary>
/// Functor type class for types that can be mapped over.
/// </summary>
/// <typeparam name="M"></typeparam>
public interface Functor<M>
{
    // fmap: (a -> b) -> f a -> f b
    HKT<M, B> FMap<A, B>(Func<A, B> f, HKT<M, A> ma);
}