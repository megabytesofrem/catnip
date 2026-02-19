namespace Catnip.Core.Data;

public sealed class EitherW { }

/// <summary>
/// The Either type represents a value that can be one of two types, Left or Right.
/// 
/// Left is the error state, and Right is the success state.
/// </summary>
/// <typeparam name="L"></typeparam>
/// <typeparam name="R"></typeparam>
public abstract record Either<L, R> : HKT<EitherW, (L, R)>
{
    public sealed record Left(L Value) : Either<L, R>;
    public sealed record Right(R Value) : Either<L, R>;

    // Projection function to extract the Either<L, R> from the HKT.
    public static Either<L, R> Proj(HKT<EitherW, (L, R)> h) => (Either<L, R>)h;
}

/// <summary>
/// The Either type represents a value that can be one of two types, Left or Right.
/// 
/// Left is the error state, and Right is the success state.
/// </summary>
/// <typeparam name="L"></typeparam>
/// <typeparam name="R"></typeparam>
public static class Either
{
    public static Either<L, R> Left<L, R>(L value) => new Either<L, R>.Left(value);
    public static Either<L, R> Right<L, R>(R value) => new Either<L, R>.Right(value);
}