using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUMEInstaller.Utils;

public static class ErrorsHandler
{
    public static void CatchErrors(IList<string> errors)
    {
        if (errors.Count > 0)
        {
            AnsiConsole.MarkupLine("\n[bold red]Errors have occured while the operation.[/]");

            var logpath = Path.Combine(Directory.GetCurrentDirectory(), "SumeiErrorsOutput.log");
            var logstr = new StringBuilder();
            logstr.AppendLine($"// Error log file generated the {DateTime.Now.ToLocalTime().ToLongDateString()}\n\n");
            foreach (var error in errors)
            {
                AnsiConsole.MarkupLine($"[white on red3]{error}[/]");
                logstr.AppendLine($"[ERROR] {error}\n");
            }
            logstr.AppendLine($"\n// End of the error logs.");

            AnsiConsole.MarkupLine($"\nPress ESC to exit.\nPress L to write all the errors in a text file at location [gold3]'{logpath}'[/] and quit.\nPress K to write all the errors in a text file and continue [underline red](not recommended)[/].");
            ConsoleUtils.ExecuteWhenKeyPressed(values: new (ConsoleKey, Action<ConsoleKeyInfo>)[]{
                (
                    ConsoleKey.Escape,
                    (cki) => Environment.Exit(0)
                ),
                (
                    ConsoleKey.L,
                    (cki) =>
                    {
                        File.WriteAllText(logpath, logstr.ToString());
                        Process.Start("notepad.exe", logpath);
                        Environment.Exit(0);
                    }
                ),
                (
                    ConsoleKey.K,
                    (cki) =>
                    {
                        File.WriteAllText(logpath, logstr.ToString());
                        Process.Start("notepad.exe", logpath);
                    }
                )
            }, true);
        }
    }
}
