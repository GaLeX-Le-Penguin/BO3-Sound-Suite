namespace BO3SoundSuite.Services;

public static class FileSafetyService
{
    public static string CreateTimestampedBackup(string sourcePath)
    {
        if (string.IsNullOrWhiteSpace(sourcePath))
            throw new ArgumentException("Source path is empty.", nameof(sourcePath));
        if (!File.Exists(sourcePath))
            throw new FileNotFoundException("Source file not found.", sourcePath);

        var directory = Path.GetDirectoryName(sourcePath) ?? Environment.CurrentDirectory;
        var baseName = Path.GetFileNameWithoutExtension(sourcePath);
        var extension = Path.GetExtension(sourcePath);
        var stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
        var candidate = Path.Combine(directory, $"{baseName}.backup_{stamp}{extension}");

        var suffix = 2;
        while (File.Exists(candidate))
        {
            candidate = Path.Combine(directory, $"{baseName}.backup_{stamp}_{suffix}{extension}");
            suffix++;
        }

        File.Copy(sourcePath, candidate, false);
        return candidate;
    }
}
