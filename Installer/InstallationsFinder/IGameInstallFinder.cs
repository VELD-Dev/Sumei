using SUMEInstaller.InstallationsFinder.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUMEInstaller.Discovery;  // Highly inspirated by Nitrox' game installation finder.

internal interface IGameInstallFinder
{
    (EPlatform, EGame, string)[]? FindGameInstalls(IList<string>? errors = null);

    internal static bool HasSubnautica(string path)
    {
        return File.Exists(Path.Combine(path, GameInfo.Subnautica.Executable));
    }

    internal static bool HasBelowZero(string path)
    {
        return File.Exists(Path.Combine(path, GameInfo.BelowZero.Executable));
    }
}
