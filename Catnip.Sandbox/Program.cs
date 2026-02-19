using Catnip.Core.Control;
using Catnip.Core.Data;

class MainClass
{
    static readonly Func<string> buildString = () => "Hello world";

    static readonly IO<string> main = from _greet in IO<string>.Pure(buildString())
                                      from _ in ConsoleIO.WriteLine(_greet)
                                      from _prompt in ConsoleIO.WriteLine("What is your name?")
                                      from name in ConsoleIO.ReadLine()
                                      from __ in ConsoleIO.WriteLine(name)
                                      select "Done";

    // Stupid ape-tier wrapper to call our IO monad
    static void Main() => main.Run();
}