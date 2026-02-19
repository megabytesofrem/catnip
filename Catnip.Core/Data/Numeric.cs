namespace Catnip.Core.Data;

/// <summary>
/// Class for types that support numeric operations
/// </summary>
/// <typeparam name="M"></typeparam>
public interface Numeric<M>
{

}

/// <summary>
/// Class for types that support division and fractional literals
/// </summary>
/// <typeparam name="M"></typeparam>
public interface Fractional<M> : Numeric<M>
{

}