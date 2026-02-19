using Catnip.Core.Data;

namespace Catnip.Core.Control;

// Witness type to hold a Higher-Kinded Type for State monad
public sealed class StateW<S> { }

/// <summary>
/// State monad for tracking state transformations.
/// 
/// The state is represented as a function that takes an initial state `s` and returns (`a`, `s`)
/// where `a` is the result of the computation and `s` is the new state after the computation.
/// </summary>
/// <typeparam name="S"></typeparam>
/// <typeparam name="A"></typeparam>
public sealed class State<S, A>
    : HKT<StateW<S>, A>
    , MonadState<StateW<S>, S>
    , MonadFail<StateW<S>>
{
    private readonly Func<S, (A, S)> _runState;

    // Project: HKT<StateW<S>, A> -> State<S, A>
    public static State<S, A> Proj(HKT<StateW<S>, A> hkt) => (State<S, A>)hkt;

    public State(Func<S, (A, S)> runState) => _runState = runState;

    #region Applicative/Monad/MonadFail
    public HKT<StateW<S>, B> Ap<A1, B>(HKT<StateW<S>, Func<A1, B>> mf, HKT<StateW<S>, A1> ma)
    {
        var mf_ = State<S, Func<A1, B>>.Proj(mf);
        var ma_ = State<S, A1>.Proj(ma);
        return new State<S, B>(s =>
        {
            var (f, s1) = mf_.RunState(s);
            var (a, s2) = ma_.RunState(s1);
            return (f(a), s2);
        });
    }

    public HKT<StateW<S>, B> FMap<A1, B>(Func<A1, B> f, HKT<StateW<S>, A1> ma)
    {
        var ma_ = State<S, A1>.Proj(ma);
        return new State<S, B>(s =>
        {
            var (a, s1) = ma_.RunState(s);
            return (f(a), s1);
        });
    }

    public HKT<StateW<S>, B> Bind<A1, B>(HKT<StateW<S>, A1> ma, Func<A1, HKT<StateW<S>, B>> f)
    {
        var ma_ = State<S, A1>.Proj(ma);
        return new State<S, B>(s =>
        {
            var (a, s1) = ma_.RunState(s);
            var mb = f(a);
            var mb_ = State<S, B>.Proj(mb);
            return mb_.RunState(s1);
        });
    }

    public HKT<StateW<S>, A1> Pure<A1>(A1 a)
    {
        var pureState = new State<S, A1>(s => (a, s));
        return pureState;
    }

    public HKT<StateW<S>, A1> Fail<A1>(string message)
    {
        var failState = new State<S, A1>(s => throw new InvalidOperationException(message));
        return failState;
    }
    #endregion

    // runState : s -> (a, s)
    public (A, S) RunState(S state) => _runState(state);

    public HKT<StateW<S>, S> Get()
    {
        return new State<S, S>(s => (s, s));
    }

    public HKT<StateW<S>, Unit> Put(S s)
    {
        return new State<S, Unit>(_ => (Unit.Value, s));
    }
}
