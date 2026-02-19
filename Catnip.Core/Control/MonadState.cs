using Catnip.Core.Data;

namespace Catnip.Core.Control;

public interface MonadState<M, S> : Monad<M>
{
    // get : MonadState m => m s
    HKT<M, S> Get();

    // put : MonadState m => s -> m ()
    HKT<M, Unit> Put(S s);
}

public static class MonadState
{
    // get : MonadState m => m s
    public static HKT<M, S> Get<M, S>() where M : MonadState<M, S>
    {
        if (default(M) == null || default(M) is not MonadState<M, S>)
            throw new InvalidOperationException($"Type {typeof(M)} does not implement MonadState<{typeof(M)}, {typeof(S)}>");
        return ((M)default! as MonadState<M, S>).Get();
    }

    // put : MonadState m => s -> m ()
    public static HKT<M, Unit> Put<M, S>(S s) where M : MonadState<M, S>
    {
        if (default(M) == null || default(M) is not MonadState<M, S>)
            throw new InvalidOperationException($"Type {typeof(M)} does not implement MonadState<{typeof(M)}, {typeof(S)}>");
        return ((M)default! as MonadState<M, S>).Put(s);
    }
}