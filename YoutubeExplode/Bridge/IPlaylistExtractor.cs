using System.Collections.Generic;

namespace YoutubeExplode.Bridge;

internal interface IPlaylistExtractor
{
    string? TryGetPlaylistTitle();

    string? TryGetPlaylistAuthor();

    string? TryGetPlaylistChannelId();

    string? TryGetPlaylistDescription();

    IReadOnlyList<ThumbnailExtractor> GetPlaylistThumbnails();
}

internal interface IPlaylisBrowsertExtractor : IPlaylistExtractor
{
    string? TryGetClickTracking();

    string? TryGetToken();
    public IReadOnlyList<PlaylistVideoExtractor> GetVideos();
}