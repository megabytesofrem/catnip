namespace Catnip.Core.Data;

using Catnip.Core;

/// <summary>
/// A type is a Semigroup if it has an associative binary operation (sconcat).
/// that allows you to combine two values of that type.
/// </summary>
/// <typeparam name="M"></typeparam>
public interface Semigroup<M>
{
    // sconcat : m -> m -> m
    HKT<M, A> SConcat<A>(HKT<M, A> x, HKT<M, A> y);
}