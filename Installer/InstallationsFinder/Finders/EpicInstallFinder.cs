using SUMEInstaller.Discovery;
using SUMEInstaller.InstallationsFinder.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SUMEInstaller.InstallationsFinder.Finders;

internal class EpicInstallFinder : IGameInstallFinder
{
    public (EPlatform, EGame, string)[]? FindGameInstalls(IList<string>? errors = null)
    {
        Regex installLocationRegexp = new("\"InstallLocation\"[^\"]*\"(.*)\"");

        var installs = new List<(EPlatform, EGame, string)>();

        string epicManifestDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "Epic", "EpicGameLauncher", "Data", "Manifests");

        if(!Directory.Exists(epicManifestDir))
        {
            errors?.Add(@"Epic Games manifest directory does not exist. Is Epic Games Store installed ?");
            return null;
        }

        string[] files = Directory.GetFiles(epicManifestDir, "*.item");
        foreach(string file in files)
        {
            string text = File.ReadAllText(file);
            Match match = installLocationRegexp.Match(text);
            if (!match.Success)
                continue;

            if(match.Value.Contains("Subnautica") && !match.Value.Contains("Below"))
            {
                installs.Add(new(EPlatform.EpicGames, EGame.Subnautica, match.Groups[1].Value));
            }
            else if(match.Value.Contains("Subnautica") && match.Value.Contains("Below"))
            {
                installs.Add(new(EPlatform.EpicGames, EGame.BelowZero, match.Groups[1].Value));
            }
        }

        if (installs.Count > 0)
            return installs.ToArray();
        else
            return null;
    }
}
