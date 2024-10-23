export class TrackSearchMode {
    public static readonly YouTube = new TrackSearchMode("ytsearch");
    public static readonly YouTubeMusic = new TrackSearchMode("ytmsearch");
    public static readonly Link = new TrackSearchMode("linksearch");

    // Only available when using the Lavasearch integration
    public static readonly Spotify = new TrackSearchMode("spsearch");
    public static readonly Deezer = new TrackSearchMode("dzsearch");

    private constructor(public readonly prefix: string) {}
}