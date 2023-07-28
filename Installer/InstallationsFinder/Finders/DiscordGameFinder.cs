using SUMEInstaller.Discovery;
using SUMEInstaller.InstallationsFinder.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUMEInstaller.InstallationsFinder.Finders;

internal class DiscordGameFinder : IGameInstallFinder
{
    public (EPlatform, EGame, string)[]? FindGameInstalls(IList<string>? errors = null)
    {
        var installs = new List<(EPlatform, EGame, string)>();
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DiscordGame", "Subnautica", "content");
        if(IGameInstallFinder.HasSubnautica(path))
        {
            installs.Add(new(EPlatform.Discord, EGame.Subnautica, path));
        }
        else
        {
            path = @"C:\Games\Subnautica\content";
            if (IGameInstallFinder.HasSubnautica(path))
            {
                installs.Add(new(EPlatform.Discord, EGame.Subnautica, path));
            }
        }

        path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DiscordGame", "SubnauticaZero", "content");
        if(IGameInstallFinder.HasBelowZero(path))
        {
            installs.Add(new(EPlatform.Discord, EGame.BelowZero, path));
        }
        else
        {
            path = @"C:\Games\SubnauticaZero\content";
            if(IGameInstallFinder.HasBelowZero(path))
            {
                installs.Add(new(EPlatform.Discord, EGame.BelowZero, path));
            }
        }

        if (installs.Count > 0)
            return installs.ToArray();
        else
            return null;
    }
}
