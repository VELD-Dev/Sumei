using SUMEInstaller.Discovery;
using SUMEInstaller.InstallationsFinder.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUMEInstaller.InstallationsFinder.Finders;

internal class EnvGameFinder : IGameInstallFinder
{
    public (EPlatform, EGame, string)[]? FindGameInstalls(IList<string>? errors = null)
    {
        var installs = new List<(EPlatform, EGame, string)>();
        string? path = Environment.GetEnvironmentVariable("SUBNAUTICA_INSTALLATION_PATH");
        if(string.IsNullOrEmpty(path))
        {
            errors?.Add(@"Env. variable SUBNAUTICA_INSTALLATION_PATH has an empty value or was not found.");
        }
        else if(!Directory.Exists(Path.Combine(path, "Subnautica_Managed", "Managed")))
        {
            errors?.Add($@"Game install directory '{path}' is invalid.");
        }
        else
        {
            installs.Add(new(EPlatform.Unknown, EGame.Subnautica, path));
        }

        path = Environment.GetEnvironmentVariable("SUBNAUTICAZERO_INSTALLATION_PATH");
        if(string.IsNullOrEmpty(path))
        {
            errors?.Add(@"Env. variable SUBNAUTICAZERO_INSTALLATION_PATH has an empty value or was not found.");
        }
        else if(!Directory.Exists(Path.Combine(path, "SubnauticaZero_Managed", "Managed")))
        {
            errors?.Add(@$"Game install directory '{path}' is invalid.");
        }
        else
        {
            installs.Add(new(EPlatform.Unknown, EGame.BelowZero, path));
        }

        if(installs.Count > 0)
            return installs.ToArray();
        else
            return null;
    }
}
