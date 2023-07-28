using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUMEInstaller.InstallationsFinder.Constants;

internal class GameInfo  // Highly inspirated by Nitrox game install finder.
{
    public string Name { get; private set; }
    public string DisplayName { get; private set; }
    public string Executable { get; private set; }
    public int SteamAppId { get; private set; }
    public string MSStoreStartUrl { get; private set; }

    private GameInfo() { }

    public static readonly GameInfo Subnautica = new()
    {
        Name = "Subnautica",
        DisplayName = "Subnautica",
        Executable = "Subnautica.exe",
        SteamAppId = 264710,
        MSStoreStartUrl = @"ms-xbl-38616e6e:\\"
    };

    public static readonly GameInfo BelowZero = new()
    {
        Name = "SubnauticaZero",
        DisplayName = "Subnautica: Below Zero",
        Executable = "SubnauticaZero.exe",
        SteamAppId = 848450,
        MSStoreStartUrl = @"ms-xbl-6e27970f:\\"
    };
}
