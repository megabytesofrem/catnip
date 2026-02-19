namespace Catnip.Core.Control;

using Catnip.Core.Data;

public static class ConsoleIO
{
    // ConsoleIO.WriteLine : string -> IO Unit
    public static IO<Unit> WriteLine(string text) =>
        new IO<Unit>(() =>
        {
            Console.WriteLine(text);
            return Unit.Value;
        });

    // ConsoleIO.ReadLine : IO string
    public static IO<string> ReadLine() =>
        new IO<string>(() => Console.ReadLine() ?? string.Empty);
}