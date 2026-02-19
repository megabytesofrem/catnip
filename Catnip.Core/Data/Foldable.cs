namespace Catnip.Core.Data;

/// <summary>
/// Class for types that can be folded to a summary value.
/// </summary>
/// <typeparam name="F"></typeparam>
public interface Foldable<F>
{
    // fold :: Monoid m => t m -> m 
    M Fold<A, M>(Func<A, M, M> f, HKT<F, A> fa, M z) where M : Monoid<M>;

    // foldMap :: Monoid m => (a -> m) -> t a -> m 
    M FoldMap<A, M>(Func<A, M> f, HKT<F, A> fa) where M : Monoid<M>;
}