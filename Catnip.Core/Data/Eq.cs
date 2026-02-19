namespace Catnip.Core.Data;

/// <summary>
/// Class for types that support equality testing
/// </summary>
/// <typeparam name="M"></typeparam>
public interface Eq<M>
{
    // eq : a -> a -> bool
    bool Eq<A>(HKT<M, A> x, HKT<M, A> y);

    // notEq : a -> a -> bool
    bool NotEq<A>(HKT<M, A> x, HKT<M, A> y) => !Eq(x, y);
}