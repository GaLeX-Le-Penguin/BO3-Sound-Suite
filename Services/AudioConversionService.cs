using NAudio.Wave;

namespace BO3SoundSuite.Services;

public static class AudioConversionService
{
    public static void ConvertToBo3Wav(string input, string output, bool backupExisting)
    {
        if (string.IsNullOrWhiteSpace(input)) throw new ArgumentException("Input path is empty.", nameof(input));
        if (string.IsNullOrWhiteSpace(output)) throw new ArgumentException("Output path is empty.", nameof(output));
        if (!File.Exists(input)) throw new FileNotFoundException("Input audio file not found.", input);

        input = Path.GetFullPath(input);
        output = Path.GetFullPath(output);
        var targetDir = Path.GetDirectoryName(output) ?? Environment.CurrentDirectory;
        Directory.CreateDirectory(targetDir);
        var temp = Path.Combine(targetDir, $".__bo3sound_{Guid.NewGuid():N}.wav");

        try
        {
            // The reader is disposed before replacement. This is required when the
            // user converts a WAV over itself inside sound_assets or Downloads.
            using (var reader = new AudioFileReader(input))
            {
                var channels = Math.Clamp(reader.WaveFormat.Channels, 1, 2);
                var targetFormat = new WaveFormat(48000, 16, channels);
                using var resampler = new MediaFoundationResampler(reader, targetFormat) { ResamplerQuality = 60 };
                WaveFileWriter.CreateWaveFile(temp, resampler);
            }

            if (backupExisting && File.Exists(output))
                FileSafetyService.CreateTimestampedBackup(output);

            // Replace directly instead of deleting the existing output first. Because
            // temp is created in the target directory, the move stays on the same volume
            // and avoids a delete-then-move window that could lose the old file on failure.
            File.Move(temp, output, true);
        }
        finally
        {
            if (File.Exists(temp)) File.Delete(temp);
        }
    }

    public static string Describe(string path)
    {
        try
        {
            using var reader = new AudioFileReader(path);
            return $"{reader.WaveFormat.SampleRate} Hz / {reader.WaveFormat.BitsPerSample} bit / {reader.WaveFormat.Channels} ch";
        }
        catch { return "?"; }
    }
}
