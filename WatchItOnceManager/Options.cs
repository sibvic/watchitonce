using CommandLine;

namespace WatchItOnce.Manager
{
    enum PlayMode
    {
        Default,
        Quad
    }
    [Verb("play", HelpText = "Play files.")]
    class Options
    {
        [Option("volume", Required = false, HelpText = "Tomato timer in seconds.", Default = 100)]
        public int Volume { get; set; }
        [Option("speed", Required = false, HelpText = "Speed multiplier 1.x.", Default = 0)]
        public int Speed { get; set; }
        [Option("mode", Required = false, HelpText = "Play mode.", Default = PlayMode.Default)]
        public PlayMode Mode { get; set; }
        [Option("extensions", Required = false, HelpText = "Extensions to use.", Default = "*.mkv;*.avi;*.mp4;*.webm;*.wmv;*.vob;*.ts;*.mpg;*.m4v;*.mp3;*.m4a;*.webm")]
        public string Extensions { get; set; }
    }
}
