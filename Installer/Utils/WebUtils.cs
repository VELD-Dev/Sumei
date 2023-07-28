using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SUMEInstaller.Utils;

public class WebUtils
{
    /*
    public static async string DownloadBepInExInstaller()
    {
        using HttpClient client = new(new HttpClientHandler());
        client.Timeout = TimeSpan.FromSeconds(60);

        using FileStream snStableFileStream = new(Path.Combine(Directory.GetCurrentDirectory(), "BepInExInstaller.zip"), FileMode.Create, FileAccess.Write, FileShare.None);
        await client.DownloadAsync()
    }
    */

    /// <summary>
    /// Download nautilus latest releases.
    /// </summary>
    /// <param name="nautilusSnTask"></param>
    /// <param name="nautilusBzTask"></param>
    /// <returns>A tuple containing a path to Nautilus Stable for SN1 and another path to Nautilus Stable for BZ.</returns>
    public static async Task<(string, string)> DownloadAllNautilusLatestAsync(ProgressTask nautilusSnTask, ProgressTask nautilusBzTask)
    {
        using HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(60);

        var snStableFileData = await GetLatestReleaseFileDataAsync("https://github.com/SubnauticaModding/Nautilus/", "Nautilus_SN.STABLE.zip");
        var bzStableFileData = await GetLatestReleaseFileDataAsync("https://github.com/SubnauticaModding/Nautilus/", "Nautilus_BZ.STABLE.zip");

        var snStableFilePath = Path.Combine(Directory.GetCurrentDirectory(), snStableFileData.Filename);
        var bzStableFilePath = Path.Combine(Directory.GetCurrentDirectory(), bzStableFileData.Filename);

        using FileStream snStableFileStream = new(snStableFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
        using FileStream bzStableFileStream = new(bzStableFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

        await client.DownloadAsync(snStableFileData, snStableFileStream, nautilusSnTask);
        await client.DownloadAsync(bzStableFileData, bzStableFileStream, nautilusBzTask);
        return (snStableFilePath, bzStableFilePath);
    }

    public static (string, string) DownloadAllNautilusLatest(ProgressTask nautilusSnTask, ProgressTask nautilusBzTask)
    {
        var task = DownloadAllNautilusLatestAsync(nautilusSnTask, nautilusBzTask);
        task.RunSynchronously();
        return task.Result;
    }

    public static async Task<DownloadInfo> GetLatestReleaseFileDataAsync(string githubURL, string? filename = null, bool includePrerelease = false)
    {
        using HttpClient client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(60);
        var uriPath = githubURL.Replace("https://github.com/", "");
        var user = uriPath.Split('/')[0];
        var repo = uriPath.Split('/')[1];
        var repoApiUrl = $"https://api.github.com/repos/{user}/{repo}/releases";

        using HttpResponseMessage data = await client.GetAsync(repoApiUrl, HttpCompletionOption.ResponseContentRead);
        string json = await data.Content.ReadAsStringAsync();
        IGithubRelease[]? releases = JsonSerializer.Deserialize<IGithubRelease[]>(json);
        if (releases == null || releases.Length > 1)
            throw new HttpRequestException("The Github repo does not have any release.");

        IGithubRelease release;
        if(includePrerelease)
            release = releases[0];
        else
        {
            var filteredReleases = releases.Where(rel => rel.prerelease == false);
            if (filteredReleases.Count() < 1)
                throw new HttpRequestException("The Github repo does not have any release that is not a prerelease. Please include pre-releases if it's what you want.");
            release = filteredReleases.FirstOrDefault();
        }

        var flength = string.IsNullOrEmpty(filename) ? release.assets[0].size : release.assets.Single(ast => ast.name == filename).size;
        var fname = string.IsNullOrEmpty(filename) ? release.assets[0].name : filename;
        var fileUrl = string.IsNullOrEmpty(filename) ? release.assets[0].browser_download_url : release.assets.Single(ast => ast.name == filename).browser_download_url;

        var info = new DownloadInfo(fname, flength, fileUrl);
        return info;
    }

    public readonly struct DownloadInfo
    {
        public DownloadInfo(string filename, long size, string sourceUrl)
        {
            Filename = filename;
            Size = size;
            SourceUrl = sourceUrl;
        }
        public string Filename { get; }
        public long Size { get; }
        public string SourceUrl { get; }
    }
}
