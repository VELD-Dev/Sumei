using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SUMEInstaller.Utils;

public static class HttpClientExtensions
{
    public static async Task DownloadAsync(this HttpClient client, WebUtils.DownloadInfo dlInfo, Stream destination, ProgressTask? progress = null, CancellationToken cancellationToken = default)
    {
        using var response = await client.GetAsync(dlInfo.SourceUrl, HttpCompletionOption.ResponseHeadersRead);
        var contentLength = dlInfo.Size;

        using var dl = await response.Content.ReadAsStreamAsync();
        if (progress == null)
        {
            await dl.CopyToAsync(destination, cancellationToken);
            return;
        }

        progress.StartTask();
        if (progress.MaxValue != contentLength)
            progress.MaxValue = contentLength;

        var relativeProgress = new Progress<long>(ttbytes => progress.Value = ttbytes);
        await dl.CopyToAsync(destination, 81920, relativeProgress, cancellationToken);
    }
}
