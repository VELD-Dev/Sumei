using Microsoft.Win32;
using SUMEInstaller.Discovery;
using SUMEInstaller.InstallationsFinder.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SUMEInstaller.InstallationsFinder.Finders;

internal class SteamGameFinder : IGameInstallFinder
{
    public (EPlatform, EGame, string)[]? FindGameInstalls(IList<string>? errors = null)
    {
        var installs = new List<(EPlatform, EGame, string)>();

        string? steamPath = "";
        if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if(!string.IsNullOrEmpty(homePath) )
            {
                steamPath = Path.Combine(homePath, ".local", "share", "Steam");
            }
            else
            {
                errors?.Add(@"User HOME is not defined.");
            }
        }
        else if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            object? regValue;
            regValue = Registry.CurrentUser.GetValue(@"Software\\Valve\\Steam\SteamPath");
            if (regValue != null)
                goto Found;
            regValue = Registry.Users.GetValue(@"Software\\Valve\\Steam\SteamPath");
            if (regValue != null)
                goto Found;
            regValue = Registry.LocalMachine.GetValue(@"Software\\Valve\\Steam\SteamPath");
            if(regValue != null)
                goto Found;

            Found:
            steamPath = regValue?.ToString();
        }

        if(string.IsNullOrEmpty(steamPath))
        {
            errors?.Add("Steam is not installed on this device, appearantly.");
            return null;
        }

        string appsPath = Path.Combine(steamPath, "steamapps");
        if(File.Exists(Path.Combine(appsPath, $"appmanifest_{GameInfo.Subnautica.SteamAppId}.acf")))
        {
            installs.Add(new(EPlatform.Steam, EGame.Subnautica, Path.Combine(appsPath, "common", GameInfo.Subnautica.Name)));
        }
        if(File.Exists(Path.Combine(appsPath, $"appmanifest_{GameInfo.BelowZero.SteamAppId}.acf")))
        {
            installs.Add(new(EPlatform.Steam, EGame.BelowZero, Path.Combine(appsPath, "common", GameInfo.BelowZero.Name)));
        }

        if (installs.Count > 0)
            return installs.ToArray();
        else
            return null;
    }
}
