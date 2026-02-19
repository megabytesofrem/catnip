namespace Catnip.Core.Data;

using Catnip.Core;

/// <summary>
/// Applicative functors are a generalization of function application
/// </summary>
/// <typeparam name="M"></typeparam>
public interface Applicative<M> : Functor<M>
{
    // pure: A -> M A
    HKT<M, A> Pure<A>(A a);

    // ap: M (A -> B) -> M A -> M B
    HKT<M, B> Ap<A, B>(HKT<M, Func<A, B>> mf, HKT<M, A> ma);
}