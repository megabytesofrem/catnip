namespace Catnip.Core.Data;

public interface Traversable<M> : Functor<M>, Foldable<M>
{
    // traverse :: Applicative f => (a -> f b) -> t a -> f (t b) 
    HKT<F, HKT<M, B>> Traverse<F, A, B>(Func<A, HKT<F, B>> f, HKT<M, A> ma) where F : Applicative<F>;
}