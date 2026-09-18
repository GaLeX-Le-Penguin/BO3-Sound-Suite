namespace BO3SoundSuite.Models;

internal enum AliasPreset
{
    Auto,
    Effect3D,
    Effect2D,
    Voice3D,
    Music2D,
    Ambience3D,
    Ui2D
}

internal sealed class AudioInfo
{
    public bool IsWave { get; set; }
    public bool IsPcm { get; set; }
    public ushort Channels { get; set; }
    public uint SampleRate { get; set; }
    public ushort BitsPerSample { get; set; }
    public ushort AudioFormat { get; set; }
    public string Error { get; set; } = string.Empty;
    public bool IsBo3Compatible => IsWave && IsPcm && SampleRate == 48000 && BitsPerSample == 16;
}

internal sealed class SoundEntry
{
    public string AudioPath { get; set; } = string.Empty;
    public string Bo3Subfolder { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;
    public int PresetIndex { get; set; }
    public int VolumeIndex { get; set; }
    public decimal CustomVolume { get; set; } = 75;
    public bool Looping { get; set; }
    public AudioInfo? AudioInfo { get; set; }
    public string ImportedFileSpec { get; set; } = string.Empty;
    public bool ImportedFromCsv { get; set; }
    public Dictionary<string, string> ManualOverrides { get; } = new(StringComparer.OrdinalIgnoreCase);
    public bool HasAudio => !string.IsNullOrWhiteSpace(AudioPath);
    public bool CanExport => HasAudio || ImportedFromCsv;
}
