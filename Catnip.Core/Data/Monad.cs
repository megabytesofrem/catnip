namespace Catnip.Core.Data;

using Catnip.Core;
using Catnip.Core.Data;

/// <summary>
/// Defines the basic operations for a Monad, which is a type of Applicative Functor that 
/// supports chaining operations together
/// </summary>
/// <typeparam name="M"></typeparam>
public interface Monad<M> : Applicative<M>
{
    // bind: m a -> (a -> m b) -> m b
    HKT<M, B> Bind<A, B>(HKT<M, A> ma, Func<A, HKT<M, B>> f);
}