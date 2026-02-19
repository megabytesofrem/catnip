namespace Catnip.Core.Control;

/// <summary>
/// Class for monads that can fail with an error message
/// </summary>
/// <typeparam name="M"></typeparam>
public interface MonadFail<M> : Monad<M>
{
    // fail : String -> m a
    HKT<M, A> Fail<A>(string message);
}