using Catnip.Core;
using Catnip.Core.Control;
using Catnip.Core.Data;

public sealed class MIOStateW<S> { }
public sealed class MIOState<S, A> : HKT<MIOStateW<S>, A>
{
    private readonly Func<S, (A, S)> _runState;
    public MIOState(Func<S, (A, S)> runState) => _runState = runState;

    // Project: HKT<MIOStateW<S>, A> -> MIOState<S, A>
    public static MIOState<S, A> Proj(HKT<MIOStateW<S>, A> hkt) => (MIOState<S, A>)hkt;

    public (A, S) RunState(S state) => _runState(state);
    public static MIOState<S, A> Pure(A a) => new MIOState<S, A>(s => (a, s));

    #region LINQ
    public MIOState<S, B> Select<B>(Func<A, B> f)
    {
        return new MIOState<S, B>(s =>
        {
            var (a, s1) = _runState(s);
            return (f(a), s1);
        });
    }
    public MIOState<S, B> SelectMany<B>(Func<A, MIOState<S, B>> f)
    {
        return new MIOState<S, B>(s =>
        {
            var (a, s1) = _runState(s);
            return f(a).RunState(s1);
        });
    }

    public MIOState<S, C> SelectMany<B, C>(Func<A, MIOState<S, B>> f, Func<A, B, C> project)
    {
        return SelectMany(a => f(a).Select(b => project(a, b)));
    }

    #endregion
}

public sealed class IOStateT<S>
    : MonadState<MIOStateW<S>, S>
    , MonadIO<MIOStateW<S>>
{
    public static readonly IOStateT<S> Instance = new IOStateT<S>();

    public HKT<MIOStateW<S>, B> Ap<A, B>(HKT<MIOStateW<S>, Func<A, B>> mf, HKT<MIOStateW<S>, A> ma)
    {
        var mf_ = MIOState<S, Func<A, B>>.Proj(mf);
        var ma_ = MIOState<S, A>.Proj(ma);
        return new MIOState<S, B>(s =>
        {
            var (f, s1) = mf_.RunState(s);
            var (a, s2) = ma_.RunState(s1);
            return (f(a), s2);
        });
    }

    public HKT<MIOStateW<S>, B> Bind<A, B>(HKT<MIOStateW<S>, A> ma, Func<A, HKT<MIOStateW<S>, B>> f)
    {
        var ma_ = MIOState<S, A>.Proj(ma);
        return new MIOState<S, B>(s =>
        {
            var (a, s1) = ma_.RunState(s);
            var mb = f(a);
            var mb_ = MIOState<S, B>.Proj(mb);
            return mb_.RunState(s1);
        });
    }

    public HKT<MIOStateW<S>, B> FMap<A, B>(Func<A, B> f, HKT<MIOStateW<S>, A> ma)
    {
        var ma_ = MIOState<S, A>.Proj(ma);
        return new MIOState<S, B>(s =>
        {
            var (a, s1) = ma_.RunState(s);
            return (f(a), s1);
        });
    }

    public HKT<MIOStateW<S>, S> Get()
    {
        return new MIOState<S, S>(s => (s, s));
    }

    public HKT<MIOStateW<S>, A> LiftIO<A>(IO<A> io)
    {
        return new MIOState<S, A>(s =>
        {
            var a = io.Run();
            return (a, s);
        });
    }

    public HKT<MIOStateW<S>, A> Pure<A>(A a)
    {
        return new MIOState<S, A>(s => (a, s));
    }

    public HKT<MIOStateW<S>, Unit> Put(S s)
    {
        return new MIOState<S, Unit>(_ => (Unit.Value, s));
    }
}


class MainClass
{
    static readonly Func<string> buildString = () => "Hello world";
    static readonly State<int, int> increment = new State<int, int>(s => (s + 1, s));

    static MIOState<int, T> liftIO<T>(IO<T> io) =>
            MIOState<int, T>.Proj(IOStateT<int>.Instance.LiftIO(io));

    static readonly MIOState<int, Unit> main =
        from _ in liftIO<Unit>(ConsoleIO.WriteLine(buildString()))
        from _prompt in liftIO<Unit>(ConsoleIO.WriteLine("What is your name?"))
        from name in liftIO<string>(ConsoleIO.ReadLine())
        from _1 in liftIO<Unit>(ConsoleIO.WriteLine(name))
        from _2 in liftIO<int>(new IO<int>(() => increment.RunState(0).Item1))
        from _3 in liftIO<int>(new IO<int>(() => increment.RunState(_2).Item1))
        from _4 in liftIO<Unit>(ConsoleIO.WriteLine($"Incremented: {_2}"))
        select Unit.Value;

    // Stupid ape-tier wrapper to call our IO monad
    static void Main()
    {
        var (result, finalState) = main.RunState(0);
        Console.WriteLine($"Final state: {finalState}");
    }
}