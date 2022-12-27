using System;
using System.Threading.Tasks;
using YoutubeExplode.DemoConsole.Utils;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace YoutubeExplode.DemoConsole;

// This demo prompts for video ID and downloads one media stream.
// It's intended to be very simple and straight to the point.
// For a more involved example - check out the WPF demo.
public static class Program
{
    public static async Task Main()
    {
        Console.Title = "YoutubeExplode Demo";

        var youtube = new YoutubeClient();

        // Get the video ID
        Console.Write("Testing get all videos of a channel with view count! \n");
        int counter = 0;
        await foreach (var batch in youtube.Playlists.GetVideoBatchesNewAsync(
            "https://www.youtube.com/playlist?list=UUT2X19JJaJGUN7mrYuImANQ"
            //"https://www.youtube.com/playlist?list=PLMNTgm9tMGhLlG8gbVCH33RvubZpnj7hG"
        ))
        {
            foreach (var video in batch.Items)
            {
                var id = video.Id;
                var title = video.Title;
                var author = video.Author;
                var duration = video.Duration.ToString();
                var viewCount = video.ViewCount;

                Console.Write(
                    $" {++counter} --> '{viewCount} views': '{id}': '{title}' \n"
                );
                //if (counter == 1) break;
            }
            //if (counter == 1) break;
        }

        /*
        var videoId = VideoId.Parse(Console.ReadLine() ?? "");

        // Get available streams and choose the best muxed (audio + video) stream
        var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoId);
        var streamInfo = streamManifest.GetMuxedStreams().TryGetWithHighestVideoQuality();
        if (streamInfo is null)
        {
            // Available streams vary depending on the video and it's possible
            // there may not be any muxed streams at all.
            // See the readme to learn how to handle adaptive streams.
            Console.Error.WriteLine("This video has no muxed streams.");
            return;
        }

        // Download the stream
        var fileName = $"{videoId}.{streamInfo.Container.Name}";

        Console.Write(
            $"Downloading stream: {streamInfo.VideoQuality.Label} / {streamInfo.Container.Name}... "
        );

        using (var progress = new ConsoleProgress())
            await youtube.Videos.Streams.DownloadAsync(streamInfo, fileName, progress);

        Console.WriteLine("Done");
        Console.WriteLine($"Video saved to '{fileName}'");
        */
    }
}