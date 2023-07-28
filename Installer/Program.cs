using Spectre.Console;
using SUMEInstaller.Discovery;
using SUMEInstaller.Utils;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SUMEInstaller;

internal class Program
{
    static void Main(string[] args)
    {
        Console.Title = "SUMEInstaller";
        if(args.Length == 0 || !args[0].StartsWith('-'))
        {
            if (args.Length > 0 && !Path.Exists(args[0]))
                return;
            Start(args);
        }
        else
        {
            switch(args[0])
            {
                case "-repair":
                    AnsiConsole.WriteLine("Not handled.");
                break;
                case "-download":
                    switch(args[1])
                    {
                        case "essential":
                            AnsiConsole.WriteLine("Not handled.");
                        break;
                        case "optional":
                            AnsiConsole.WriteLine("Not handled.");
                        break;
                        default:
                            return;
                    }
                break;
                case "-uninstall":
                    AnsiConsole.WriteLine("Not handled.");
                break;
                default:
                    Console.WriteLine("Unhandled arguments.");
                break;
            }
        }
    }

    static async void Start(string[] args)
    {
#if DEBUG
        bool verbose = true;
#elif RELEASE
        bool verbose = false;
#endif
        AnsiConsole.Write(new FigletText("Sumei").LeftJustified().Color(Color.CornflowerBlue));
        AnsiConsole.MarkupLine("Starting installation of the [blue]Subnautica Universal Mods Environement[/]. Please take notes:");
        AnsiConsole.WriteLine("\t- The provided mods are directly downloaded from their official sources.");
        AnsiConsole.WriteLine("\t- This installer is not official. It is not an official product and it is not made by Unknown Worlds Entertainment (UWE).");
        AnsiConsole.WriteLine("\t- This installer uses a few other programs in some cases. If you prefer downloading everything manually, do not use this.");
        AnsiConsole.WriteLine("\t- If an install of the mods environment already exists and already have mods, but for some reason it is broken, this will automatically repair your install.");
        AnsiConsole.WriteLine("\t- Remember that piracy is bad, and that downloading illegaly copies of a digital product is prohibited in almost all countries in the world.");

        AnsiConsole.WriteLine("\n");
        AnsiConsole.WriteLine("Press ENTER to continue.\n\n");
        ConsoleUtils.WaitForKey(ConsoleKey.Enter, true);

        AnsiConsole.Write(new Rule("[gold3]Search game installations[/]").LeftJustified());
        AnsiConsole.Write("\n");

        var sw = Stopwatch.StartNew();
        List<string> errs = AnsiConsole.Progress()
            .Start((ctx) =>
            {
                var task = ctx.AddTask("Searching for game(s) install(s)", true);
                var errors = new List<string>();

                GameInstallationsFinder.Main.FindGames(task, errors);
                return errors;
            });
        sw.Stop();
        AnsiConsole.MarkupLine($"Game install discovery took [blue]{float.Round((float)sw.Elapsed.TotalSeconds, 3)} seconds[/].\n");
        ErrorsHandler.CatchErrors(errs);

        AnsiConsole.WriteLine("Press ENTER to continue.");
        ConsoleUtils.WaitForKey(ConsoleKey.Enter, true);

        // DOWNLOADING LIBRARIES

        AnsiConsole.Write(new Rule("[gold3]Installing essential libraries[/]").LeftJustified());
        errs.Clear();
        sw = Stopwatch.StartNew();
        errs = await AnsiConsole.Progress()
            .StartAsync(async ctx =>
            {
                var snTask = ctx.AddTask("Nautilus_SN.STABLE.zip", false);
                var bzTask = ctx.AddTask("Nautilus_BZ.STABLE.zip", false);

                try
                {
                    var dlTask = await WebUtils.DownloadAllNautilusLatestAsync(snTask, bzTask);
                }
                catch (Exception ex)
                {
                    errs.Add(ex.ToString());
                    if (snTask.IsFinished != true)
                        snTask.Description("ERROR");
                    if (bzTask.IsFinished != true)
                        bzTask.Description("ERROR");
                }
                return errs;
            });
        sw.Stop();
        AnsiConsole.MarkupLine($"Download essential libraries took [blue]{float.Round((float)sw.Elapsed.TotalSeconds, 3)} seconds[/].");
        ErrorsHandler.CatchErrors(errs);

        AnsiConsole.WriteLine("Press ESC to exit.");
        ConsoleUtils.WaitForKey(ConsoleKey.Escape);
    }
}