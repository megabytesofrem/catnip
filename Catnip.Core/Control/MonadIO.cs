namespace Catnip.Core.Control;

// Witness type for IO monad
public sealed class IOW { }

// IO is a computation that performs side effects when ran
public sealed class IO<A> : HKT<IOW, A>
{
    private readonly Func<A> _action;
    public IO(Func<A> action) => _action = action;

    // Projection function to extract the Maybe<A> from the HKT.
    public static IO<A> Proj(HKT<IOW, A> h) => (IO<A>)h;

    // pure : a -> IO a
    public static IO<A> Pure(A a) => new IO<A>(() => a);


    // run : IO a -> a
    public A Run() => _action();

    #region LINQ
    public IO<B> Select<B>(Func<A, B> f) => new IO<B>(() => f(_action()));
    public IO<B> SelectMany<B>(Func<A, IO<B>> f) => new IO<B>(() => f(_action()).Run());
    public IO<C> SelectMany<B, C>(Func<A, IO<B>> f, Func<A, B, C> g) => new IO<C>(() =>
    {
        var a = _action();
        var b = f(a).Run();
        return g(a, b);
    });
    #endregion
}

public sealed class IOK : MonadIO<IOW>
{
    public static readonly IOK Instance = new IOK();

    // ap : MonadIO => IO (a -> b) -> IO a -> IO b
    public HKT<IOW, B> Ap<A, B>(HKT<IOW, Func<A, B>> mf, HKT<IOW, A> ma)
    {
        var mf_ = IO<Func<A, B>>.Proj(mf);
        var ma_ = IO<A>.Proj(ma);
        return new IO<B>(() => mf_.Run()(ma_.Run()));
    }

    // bind : MonadIO => IO a -> (a -> IO b) -> IO b
    public HKT<IOW, B> Bind<A, B>(HKT<IOW, A> ma, Func<A, HKT<IOW, B>> f)
    {
        var ma_ = IO<A>.Proj(ma);
        return new IO<B>(() =>
        {
            var a = ma_.Run();
            var mb = f(a);
            var mb_ = IO<B>.Proj(mb);
            return mb_.Run();
        });
    }

    // fmap : MonadIO => (a -> b) -> IO a -> IO b
    public HKT<IOW, B> FMap<A, B>(Func<A, B> f, HKT<IOW, A> ma)
    {
        var ma_ = IO<A>.Proj(ma);
        return new IO<B>(() => f(ma_.Run()));
    }

    // pure : MonadIO => a -> IO a
    public HKT<IOW, A> Pure<A>(A a)
    {
        return new IO<A>(() => a);
    }

    // liftIO : MonadIO => IO a -> m a
    public HKT<IOW, A> LiftIO<A>(IO<A> io) => io;
}

/// <summary>
/// Class for monads that can perform IO actions
/// </summary>
/// <typeparam name="M"></typeparam>
public interface MonadIO<M> : Monad<M>
{
    // liftIO : IO a -> m a
    HKT<M, A> LiftIO<A>(IO<A> io);
}

public static class MonadIO
{
    public static HKT<M, A> LiftIO<M, A>(IO<A> io) where M : MonadIO<M>
    {
        if (default(M) == null || default(M) is not MonadIO<M>)
            throw new InvalidOperationException($"Type {typeof(M)} does not implement MonadIO");

        return ((M)default! as MonadIO<M>).LiftIO(io);
    }
}

/// <summary>
/// Helper class for running IO actions in the Main method
/// </summary>
/// 
/// <remarks>
/// MainIO.Run should <b>only</b> be called once in the entire program and be in `Main`
/// </remarks>
public static class MainIO
{
    // run : IO a -> a
    public static void Run<A>(IO<A> action) => action.Run();
}