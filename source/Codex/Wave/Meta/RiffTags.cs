namespace ChaosFramework.Sound.Codex.Wave.Meta
{
    public enum RiffTags
    {
#pragma warning disable ChaosCC0102 // Bad Identifier Case - Must excatly match RIFF tags in wave files
        fmt = 0x20746D66,
        data = 0x61746164,
        smpl = 0x6C706D73,
        LIST = 0x5453494C,
        cue = 0x20657563,
        tlst = 0x74736C74,
        acid = 0x64696361,
        strc = 0x63727473,
        inst = 0x74736E69,
        fact = 0x74636166,
        labl = 0x6C62616C,
        ISFT = 0x54465349,
        ICMT = 0x544D4349
#pragma warning restore ChaosCC0102 // Bad Identifier Case
    }
}
