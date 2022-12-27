using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using YoutubeExplode.Utils;
using YoutubeExplode.Utils.Extensions;

namespace YoutubeExplode.Bridge;

internal class PlaylistVideoExtractor
{
    private static readonly string[] DurationFormats = {@"m\:ss", @"mm\:ss", @"h\:mm\:ss", @"hh\:mm\:ss"};

    private readonly JsonElement _content;

    public PlaylistVideoExtractor(JsonElement content) => _content = content;

    public int? TryGetIndex() => Memo.Cache(this, () =>
        _content
            .GetPropertyOrNull("navigationEndpoint")?
            .GetPropertyOrNull("watchEndpoint")?
            .GetPropertyOrNull("index")?
            .GetInt32OrNull()
    );

    public string? TryGetVideoId() => Memo.Cache(this, () =>
        _content
            .GetPropertyOrNull("videoId")?
            .GetStringOrNull()
    );

    public string? TryGetVideoTitle() => Memo.Cache(this, () =>
        _content
            .GetPropertyOrNull("title")?
            .GetPropertyOrNull("simpleText")?
            .GetStringOrNull() ??

        _content
            .GetPropertyOrNull("title")?
            .GetPropertyOrNull("runs")?
            .EnumerateArrayOrNull()?
            .Select(j => j.GetPropertyOrNull("text")?.GetStringOrNull())
            .WhereNotNull()
            .ConcatToString()
    );

    private JsonElement? TryGetAuthorDetails() => Memo.Cache(this, () =>
        _content
            .GetPropertyOrNull("longBylineText")?
            .GetPropertyOrNull("runs")?
            .EnumerateArrayOrNull()?
            .ElementAtOrNull(0) ??

        _content
            .GetPropertyOrNull("shortBylineText")?
            .GetPropertyOrNull("runs")?
            .EnumerateArrayOrNull()?
            .ElementAtOrNull(0)
    );

    public string? TryGetVideoAuthor() => Memo.Cache(this, () =>
        TryGetAuthorDetails()?
            .GetPropertyOrNull("text")?
            .GetStringOrNull()
    );

    public string? TryGetVideoChannelId() => Memo.Cache(this, () =>
        TryGetAuthorDetails()?
            .GetPropertyOrNull("navigationEndpoint")?
            .GetPropertyOrNull("browseEndpoint")?
            .GetPropertyOrNull("browseId")?
            .GetStringOrNull()
    );

    public string? TryGetVideoTitleLong() => Memo.Cache(this, () =>
        _content
            .GetPropertyOrNull("title")?
            .GetPropertyOrNull("accessibility")?
            .GetPropertyOrNull("accessibilityData")?
            .GetPropertyOrNull("label")?
            .GetStringOrNull()?.Trim()
    );

    public string TryGetVideoViewCount() => Memo.Cache(this, () =>
    {
        string viewCount = "0";
        string? titleLong = TryGetVideoTitleLong();
        if (titleLong != null)
        {
            string[] words = titleLong.Split(' ');
            if (words[words.Length - 1].Equals("views"))
            {
                viewCount = words[^2].Replace(",", "");
            }
        }
        return viewCount;
    });

    public string? TryGetVideoViewCountShort() => Memo.Cache(this, () =>
    {
        string? viewCount = _content
            .GetPropertyOrNull("videoInfo")?
            .GetPropertyOrNull("runs")?
            .EnumerateArrayOrNull()?
            .ElementAtOrNull(0)?
            .GetPropertyOrNull("text")?
            .GetStringOrNull();

        if (viewCount != null) {
            viewCount= viewCount.Replace("views", "lượt xem").Replace("K", " ngàn").Replace("M", " triệu").Replace("B", " tỷ");
        }
        else
        {
            viewCount = "0";
        }
        return viewCount;
    });


    public TimeSpan? TryGetVideoDuration() => Memo.Cache(this, () =>
        _content
            .GetPropertyOrNull("lengthSeconds")?
            .GetStringOrNull()?
            .ParseDoubleOrNull()?
            .Pipe(TimeSpan.FromSeconds) ??

        _content
            .GetPropertyOrNull("lengthText")?
            .GetPropertyOrNull("simpleText")?
            .GetStringOrNull()?
            .ParseTimeSpanOrNull(DurationFormats) ??

        _content
            .GetPropertyOrNull("lengthText")?
            .GetPropertyOrNull("runs")?
            .EnumerateArrayOrNull()?
            .Select(j => j.GetPropertyOrNull("text")?.GetStringOrNull())
            .WhereNotNull()
            .ConcatToString()
            .ParseTimeSpanOrNull(DurationFormats)
    );

    public IReadOnlyList<ThumbnailExtractor> GetVideoThumbnails() => Memo.Cache(this, () =>
        _content
            .GetPropertyOrNull("thumbnail")?
            .GetPropertyOrNull("thumbnails")?
            .EnumerateArrayOrNull()?
            .Select(j => new ThumbnailExtractor(j))
            .ToArray() ??

        Array.Empty<ThumbnailExtractor>()
    );
}