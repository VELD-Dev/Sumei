using SUMEInstaller.Discovery;
using SUMEInstaller.InstallationsFinder.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUMEInstaller.InstallationsFinder.Finders;

internal class GameInDirFinder : IGameInstallFinder
{
    public (EPlatform, EGame, string)[]? FindGameInstalls(IList<string>? errors = null)
    {
        var install = new List<(EPlatform, EGame, string)>();
        string currentDir = Directory.GetCurrentDirectory();
        if(File.Exists(Path.Combine(currentDir, "Subnautica.exe")))
        {
            install.Add(new(EPlatform.Unknown, EGame.Subnautica, currentDir));
        }
        if(File.Exists(Path.Combine(currentDir, "SubnauticaZero.exe")))
        {
            install.Add(new(EPlatform.Unknown, EGame.BelowZero, currentDir));
        }

        if (install.Count > 0)
            return install.ToArray();
        else
            return null;
    }
}
