namespace Catnip.Core.Data;

using Catnip.Core;


/// <summary>
/// The Void type represents a type that has no values.
/// </summary>
public record Unit
{
    public static readonly Unit Value = new Unit();
}