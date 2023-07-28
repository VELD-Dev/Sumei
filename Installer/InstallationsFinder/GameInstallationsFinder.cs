using Spectre.Console;
using SUMEInstaller.InstallationsFinder.Constants;
using SUMEInstaller.InstallationsFinder.Finders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUMEInstaller.Discovery;

internal class GameInstallationsFinder
{
    private static readonly Lazy<GameInstallationsFinder> main = new(() =>  new GameInstallationsFinder());
    public static GameInstallationsFinder Main => main.Value;

    private readonly IGameInstallFinder[] finders =
    {
        new DiscordGameFinder(),
        new EnvGameFinder(),
        new EpicInstallFinder(),
        new GameInDirFinder(),
        new SteamGameFinder()
    };

    public Dictionary<EPlatform, (EGame, string)[]> FindGames(ProgressTask task, IList<string>? errors = null)
    {
        errors ??= new List<string>();
        task.MaxValue = finders.Length;
        var paths = new Dictionary<EPlatform, (EGame, string)[]>();
        foreach(var finder in finders)
        {
            try
            {
                List<(EPlatform, EGame, string)>? findersPaths = finder.FindGameInstalls()?.ToList();
                if (findersPaths == null || findersPaths.Count < 1)
                    continue;

                EPlatform platform = findersPaths[0].Item1;
                var list = new List<(EGame, string)>();

                foreach (var path in findersPaths)
                    list.Add((path.Item2, path.Item3));

                paths.Add(platform, list.ToArray());
            }
            catch(Exception ex)
            {
                errors?.Add(ex.Message);
            }
            finally
            {
                task.Increment(1);
            }
        }
        if (paths.Count < 1)
            errors.Add("Absolutely no path linking to one or both of the games exist on this computer. Check if the game you want to setup is installed. \nIf you think that it is an error, put this installer inside your game install directory. If it still does not work, please make an issue on the GitHub repository.");
        return paths;
    }
}
