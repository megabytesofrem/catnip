using Catnip.Core.Control;
using Catnip.Core.Data;

class MainClass
{
    static void Main()
    {
        Maybe<int> just = Maybe.Just(42);
        var result = just.FMap(x => x * 2); // Just(84)

        IO<string> main = from _0 in ConsoleIO.WriteLine("What is your name?")
                          from name in ConsoleIO.ReadLine()
                          from _1 in ConsoleIO.WriteLine($"Hello, {name}!")
                          from _2 in IO<string>.Pure(result switch
                          {
                              Maybe<int>.Nothing => "Nothing",
                              Maybe<int>.Just v => $"We have a value: {v.Value}!",
                              _ => "Invalid Maybe value"
                          })
                          from _3 in ConsoleIO.WriteLine(_2)
                          select "Done!";
        main.Run();
    }
}