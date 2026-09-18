using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace BO3SoundSuite.Services;

public static class Bo3PathService
{
    private const string GameFolderName = "Call of Duty Black Ops III";
    private static readonly Lazy<string?> CachedGameRoot = new(DetectGameRoot);

    public static string GetGameRoot() => CachedGameRoot.Value ?? string.Empty;

    public static string GetSoundAssetsDirectory() => GetGameSubdirectory("sound_assets");

    public static string GetAliasesDirectory() => GetGameSubdirectory("share", "raw", "sound", "aliases");

    public static string GetUsermapsDirectory() => GetGameSubdirectory("usermaps");

    public static string GetDownloadsDirectory()
    {
        // Prefer the Windows Known Folder value so redirected/localized Downloads folders work.
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders");
            var raw = key?.GetValue("{374DE290-123F-4565-9164-39C4925E467B}") as string;
            if (!string.IsNullOrWhiteSpace(raw))
            {
                var expanded = Environment.ExpandEnvironmentVariables(raw);
                if (Directory.Exists(expanded)) return expanded;
            }
        }
        catch { }

        var profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var fallback = Path.Combine(profile, "Downloads");
        return Directory.Exists(fallback) ? fallback : profile;
    }

    public static string GetExistingOrNearestParent(string? path, string? fallback = null)
    {
        if (!string.IsNullOrWhiteSpace(path))
        {
            try
            {
                var current = Path.GetFullPath(path);
                if (File.Exists(current)) current = Path.GetDirectoryName(current) ?? string.Empty;
                while (!string.IsNullOrWhiteSpace(current))
                {
                    if (Directory.Exists(current)) return current;
                    current = Path.GetDirectoryName(current) ?? string.Empty;
                }
            }
            catch { }
        }

        if (!string.IsNullOrWhiteSpace(fallback) && Directory.Exists(fallback)) return fallback;
        return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    }

    private static string GetGameSubdirectory(params string[] parts)
    {
        var root = GetGameRoot();
        if (string.IsNullOrWhiteSpace(root)) return string.Empty;
        var path = root;
        foreach (var part in parts) path = Path.Combine(path, part);
        return GetExistingDirectory(path);
    }

    private static string GetExistingDirectory(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return string.Empty;
        return Directory.Exists(path) ? path : string.Empty;
    }

    private static string? DetectGameRoot()
    {
        foreach (var library in EnumerateSteamLibraries())
        {
            try
            {
                var candidate = Path.Combine(library, "steamapps", "common", GameFolderName);
                if (Directory.Exists(candidate)) return candidate;
            }
            catch { }
        }

        // Last-resort common SteamLibrary layouts on all mounted drives.
        foreach (var drive in DriveInfo.GetDrives())
        {
            try
            {
                if (!drive.IsReady) continue;
                foreach (var relative in new[]
                {
                    Path.Combine("SteamLibrary", "steamapps", "common", GameFolderName),
                    Path.Combine("Steam", "steamapps", "common", GameFolderName),
                    Path.Combine("Program Files (x86)", "Steam", "steamapps", "common", GameFolderName),
                    Path.Combine("Program Files", "Steam", "steamapps", "common", GameFolderName)
                })
                {
                    var candidate = Path.Combine(drive.RootDirectory.FullName, relative);
                    if (Directory.Exists(candidate)) return candidate;
                }
            }
            catch { }
        }

        return null;
    }

    private static IEnumerable<string> EnumerateSteamLibraries()
    {
        var roots = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        AddRegistryPath(Registry.CurrentUser, @"Software\Valve\Steam", "SteamPath", roots);
        AddRegistryPath(Registry.LocalMachine, @"SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath", roots);
        AddRegistryPath(Registry.LocalMachine, @"SOFTWARE\Valve\Steam", "InstallPath", roots);

        var pf86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        var pf = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        if (!string.IsNullOrWhiteSpace(pf86)) roots.Add(Path.Combine(pf86, "Steam"));
        if (!string.IsNullOrWhiteSpace(pf)) roots.Add(Path.Combine(pf, "Steam"));

        foreach (var steamRoot in roots.ToArray())
        {
            if (!Directory.Exists(steamRoot)) continue;
            yield return steamRoot;

            var vdf = Path.Combine(steamRoot, "steamapps", "libraryfolders.vdf");
            if (!File.Exists(vdf)) continue;

            string text;
            try { text = File.ReadAllText(vdf); }
            catch { continue; }

            // Current Steam format: "path" "D:\\SteamLibrary".
            foreach (Match match in Regex.Matches(text, "\\\"path\\\"\\s*\\\"(?<path>[^\\\"]+)\\\"", RegexOptions.IgnoreCase))
            {
                var value = DecodeVdfPath(match.Groups["path"].Value);
                if (Directory.Exists(value) && roots.Add(value)) yield return value;
            }

            // Older libraryfolders.vdf variants stored numeric keys directly as paths.
            foreach (Match match in Regex.Matches(text, "\\\"\\d+\\\"\\s*\\\"(?<path>[A-Za-z]:[^\\\"]+)\\\""))
            {
                var value = DecodeVdfPath(match.Groups["path"].Value);
                if (Directory.Exists(value) && roots.Add(value)) yield return value;
            }
        }
    }

    private static string DecodeVdfPath(string value) => value.Replace("\\\\", "\\").Trim();

    private static void AddRegistryPath(RegistryKey hive, string subKey, string valueName, HashSet<string> target)
    {
        try
        {
            using var key = hive.OpenSubKey(subKey);
            var value = key?.GetValue(valueName)?.ToString();
            if (!string.IsNullOrWhiteSpace(value)) target.Add(value.Replace('/', '\\'));
        }
        catch { }
    }
}
