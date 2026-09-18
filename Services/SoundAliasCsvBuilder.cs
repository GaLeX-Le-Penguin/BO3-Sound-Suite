using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using BO3SoundSuite.Models;

namespace BO3SoundSuite.Services;

internal static class SoundAliasCsvBuilder
    {
        public static readonly string[] Headers =
        {
            "Name","Behavior","Storage","FileSpec","FileSpecSustain","FileSpecRelease","Template","Loadspec","Secondary","SustainAlias","ReleaseAlias","Bus","VolumeGroup","DuckGroup","Duck","ReverbSend","CenterSend","VolMin","VolMax","DistMin","DistMaxDry","DistMaxWet","DryMinCurve","DryMaxCurve","WetMinCurve","WetMaxCurve","LimitCount","LimitType","EntityLimitCount","EntityLimitType","PitchMin","PitchMax","PriorityMin","PriorityMax","PriorityThresholdMin","PriorityThresholdMax","AmplitudePriority","PanType","Pan","Futz","Looping","RandomizeType","Probability","StartDelay","EnvelopMin","EnvelopMax","EnvelopPercent","OcclusionLevel","IsBig","DistanceLpf","FluxType","FluxTime","Subtitle","Doppler","ContextType","ContextValue","ContextType1","ContextValue1","ContextType2","ContextValue2","ContextType3","ContextValue3","Timescale","IsMusic","IsCinematic","FadeIn","FadeOut","Pauseable","StopOnEntDeath","Compression","StopOnPlay","DopplerScale","FutzPatch","VoiceLimit","IgnoreMaxDist","NeverPlayTwice","ContinuousPan","FileSource","FileSourceSustain","FileSourceRelease","FileTarget","FileTargetSustain","FileTargetRelease","Platform","Language","OutputDevices","PlatformMask","WiiUMono","StopAlias","DistanceLpfMin","DistanceLpfMax","FacialAnimationName","RestartContextLoops","SilentInCPZ","ContextFailsafe","GPAD","GPADOnly","MuteVoice","MuteMusic","RowSourceFileName","RowSourceShortName","RowSourceLineNumber"
        };

        public static string SanitizeAlias(string value)
        {
            if (value == null) return "sound_alias";
            string normalized = value.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();
            bool underscore = false;

            for (int i = 0; i < normalized.Length; i++)
            {
                char c = normalized[i];
                UnicodeCategory cat = CharUnicodeInfo.GetUnicodeCategory(c);
                if (cat == UnicodeCategory.NonSpacingMark) continue;

                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(char.ToLowerInvariant(c));
                    underscore = false;
                }
                else if (!underscore)
                {
                    sb.Append('_');
                    underscore = true;
                }
            }

            string result = sb.ToString().Trim('_');
            return result.Length == 0 ? "sound_alias" : result;
        }

        public static string TryGetFileSpecFromSoundAssets(string audioFile)
        {
            if (string.IsNullOrWhiteSpace(audioFile)) return string.Empty;

            string path = audioFile.Trim().Trim('"').Replace('/', '\\');
            const string marker = "\\sound_assets\\";
            int index = path.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
                return path.Substring(index + marker.Length);

            if (path.StartsWith("sound_assets\\", StringComparison.OrdinalIgnoreCase))
                return path.Substring("sound_assets\\".Length);

            return string.Empty;
        }

        public static string ResolveFileSpec(string enteredPath, string audioFile)
        {
            string path = (enteredPath ?? string.Empty).Trim().Trim('"').Replace('/', '\\');
            string fileName = string.IsNullOrWhiteSpace(audioFile) ? "sound.wav" : Path.GetFileName(audioFile);

            while (path.StartsWith("\\")) path = path.Substring(1);
            string lower = path.ToLowerInvariant();
            int marker = lower.IndexOf("sound_assets\\", StringComparison.Ordinal);
            if (marker >= 0) path = path.Substring(marker + "sound_assets\\".Length);

            if (path.Length == 0) return fileName;
            if (path.EndsWith("\\")) return path + fileName;
            if (!string.Equals(Path.GetExtension(path), ".wav", StringComparison.OrdinalIgnoreCase))
                return path + "\\" + fileName;
            return path;
        }

        public static AliasPreset DetectPreset(string fileSpec, string alias)
        {
            string s = ((fileSpec ?? string.Empty) + " " + (alias ?? string.Empty)).ToLowerInvariant();
            if (ContainsAny(s, "music", "mus_", "jingle", "sting", "roundstart", "gameover", "song")) return AliasPreset.Music2D;
            if (ContainsAny(s, "voice", "vox", "dialog", "announcer", "anncr", "speech", "english\\sound")) return AliasPreset.Voice3D;
            if (ContainsAny(s, "ui\\", "menu", "hud", "interface")) return AliasPreset.Ui2D;
            if (ContainsAny(s, "ambient", "amb_", "amb\\", "wind", "rain", "atmos", "environment")) return AliasPreset.Ambience3D;
            return AliasPreset.Effect3D;
        }

        public static bool ShouldAutoLoop(string fileSpec, string alias)
        {
            string s = ((fileSpec ?? string.Empty) + " " + (alias ?? string.Empty)).ToLowerInvariant();
            return ContainsAny(s, "_loop", "\\loop", "looping", "amb_loop");
        }

        public static Dictionary<string, string> BuildRow(string alias, string fileSpec, AliasPreset preset, bool looping, int? volumeOverride)
        {
            Dictionary<string, string> row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < Headers.Length; i++) row[Headers[i]] = string.Empty;

            row["Name"] = SanitizeAlias(alias);
            row["FileSpec"] = fileSpec;
            row["Template"] = "UIN_MOD";
            row["Looping"] = looping ? "LOOPING" : "NONLOOPING";
            row["Probability"] = "1";
            row["Pauseable"] = "YES";
            row["PitchMin"] = "1";
            row["PitchMax"] = "1";

            switch (preset)
            {
                case AliasPreset.Effect3D:
                    row["Bus"] = "BUS_FX";
                    row["VolMin"] = "80"; row["VolMax"] = "80";
                    row["DistMin"] = "50"; row["DistMaxDry"] = "400"; row["DistMaxWet"] = "401";
                    row["LimitCount"] = "3"; row["LimitType"] = "oldest";
                    row["PanType"] = "3d"; row["Pan"] = "wpn_all";
                    break;
                case AliasPreset.Effect2D:
                    row["Bus"] = "BUS_FX";
                    row["VolMin"] = "80"; row["VolMax"] = "80";
                    row["LimitCount"] = "3"; row["LimitType"] = "oldest";
                    row["PanType"] = "2d"; row["Pan"] = "front";
                    break;
                case AliasPreset.Voice3D:
                    row["Storage"] = "streamed";
                    row["Bus"] = "BUS_VOICE"; row["VolumeGroup"] = "grp_voice"; row["DuckGroup"] = "snp_voice";
                    row["VolMin"] = "95"; row["VolMax"] = "95";
                    row["DistMin"] = "70"; row["DistMaxDry"] = "1400"; row["DistMaxWet"] = "1500";
                    row["LimitCount"] = "1"; row["PanType"] = "3d";
                    break;
                case AliasPreset.Music2D:
                    row["Storage"] = "streamed";
                    row["Bus"] = "BUS_MUSIC"; row["VolumeGroup"] = "grp_music";
                    row["VolMin"] = "100"; row["VolMax"] = "100";
                    row["PanType"] = "2d"; row["Pan"] = "music_all"; row["IsMusic"] = "yes";
                    break;
                case AliasPreset.Ambience3D:
                    row["Storage"] = "streamed";
                    row["Bus"] = "BUS_FX"; row["VolumeGroup"] = "grp_ambience"; row["DuckGroup"] = "snp_ambience";
                    row["ReverbSend"] = "60";
                    row["VolMin"] = "65"; row["VolMax"] = "65";
                    row["DistMin"] = "100"; row["DistMaxDry"] = "1200"; row["DistMaxWet"] = "1250";
                    row["LimitCount"] = "3"; row["LimitType"] = "priority";
                    row["PanType"] = "3d"; row["Pan"] = "default"; row["DistanceLpf"] = "yes";
                    break;
                case AliasPreset.Ui2D:
                    row["Storage"] = "loaded";
                    row["Bus"] = "BUS_UI";
                    row["VolMin"] = "100"; row["VolMax"] = "100";
                    row["PanType"] = "2d"; row["Pan"] = "center"; row["ReverbSend"] = "0";
                    break;
            }

            if (volumeOverride.HasValue)
            {
                int volume = Math.Max(0, Math.Min(100, volumeOverride.Value));
                string value = volume.ToString(CultureInfo.InvariantCulture);
                row["VolMin"] = value;
                row["VolMax"] = value;
            }

            return row;
        }

        public static string BuildCsv(Dictionary<string, string> row)
        {
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            if (row != null) rows.Add(row);
            return BuildCsv(rows);
        }

        public static string BuildCsv(IEnumerable<Dictionary<string, string>> rows)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(CsvLine(Headers)).Append("\r\n");
            if (rows != null)
            {
                foreach (Dictionary<string, string> row in rows)
                {
                    if (row == null) continue;
                    sb.Append(CsvLine(RowValues(row))).Append("\r\n");
                }
            }
            return sb.ToString();
        }

        public static string BuildCsvRow(Dictionary<string, string> row)
        {
            if (row == null) return string.Empty;
            return CsvLine(RowValues(row));
        }

        private static string[] RowValues(Dictionary<string, string> row)
        {
            string[] values = new string[Headers.Length];
            for (int i = 0; i < Headers.Length; i++) values[i] = row[Headers[i]];
            return values;
        }

        public static List<Dictionary<string, string>> ParseCsv(string text, out int unknownHeaderCount)
        {
            unknownHeaderCount = 0;
            List<string[]> records = ParseCsvRecords(text ?? string.Empty);
            if (records.Count == 0)
                throw new InvalidDataException("CSV_EMPTY");

            string[] sourceHeaders = records[0];
            if (sourceHeaders.Length == 0)
                throw new InvalidDataException("CSV_NO_HEADER");

            Dictionary<int, string> mappedColumns = new Dictionary<int, string>();
            HashSet<string> seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < sourceHeaders.Length; i++)
            {
                string header = (sourceHeaders[i] ?? string.Empty).Trim().TrimStart('\uFEFF');
                string? canonical = FindCanonicalHeader(header);
                if (canonical == null)
                {
                    if (header.Length > 0) unknownHeaderCount++;
                    continue;
                }
                if (seen.Add(canonical)) mappedColumns[i] = canonical;
            }

            if (!seen.Contains("Name") && !seen.Contains("FileSpec"))
                throw new InvalidDataException("CSV_UNSUPPORTED_HEADER");

            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            for (int r = 1; r < records.Count; r++)
            {
                string[] record = records[r];
                bool hasContent = false;
                for (int c = 0; c < record.Length; c++)
                {
                    if (!string.IsNullOrEmpty(record[c])) { hasContent = true; break; }
                }
                if (!hasContent) continue;

                Dictionary<string, string> row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < Headers.Length; i++) row[Headers[i]] = string.Empty;
                foreach (KeyValuePair<int, string> pair in mappedColumns)
                {
                    row[pair.Value] = pair.Key < record.Length ? (record[pair.Key] ?? string.Empty) : string.Empty;
                }
                rows.Add(row);
            }
            return rows;
        }

        private static string? FindCanonicalHeader(string header)
        {
            if (string.IsNullOrWhiteSpace(header)) return null;
            for (int i = 0; i < Headers.Length; i++)
                if (string.Equals(Headers[i], header, StringComparison.OrdinalIgnoreCase)) return Headers[i];
            return null;
        }

        private static List<string[]> ParseCsvRecords(string text)
        {
            List<string[]> records = new List<string[]>();
            List<string> row = new List<string>();
            StringBuilder field = new StringBuilder();
            bool quoted = false;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (quoted)
                {
                    if (c == '"')
                    {
                        if (i + 1 < text.Length && text[i + 1] == '"')
                        {
                            field.Append('"');
                            i++;
                        }
                        else quoted = false;
                    }
                    else field.Append(c);
                    continue;
                }

                if (c == '"' && field.Length == 0)
                {
                    quoted = true;
                }
                else if (c == ',')
                {
                    row.Add(field.ToString());
                    field.Length = 0;
                }
                else if (c == '\r' || c == '\n')
                {
                    row.Add(field.ToString());
                    field.Length = 0;
                    records.Add(row.ToArray());
                    row.Clear();
                    if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n') i++;
                }
                else field.Append(c);
            }

            if (quoted)
                throw new InvalidDataException("CSV_UNCLOSED_QUOTE");

            if (field.Length > 0 || row.Count > 0)
            {
                row.Add(field.ToString());
                records.Add(row.ToArray());
            }
            return records;
        }

        private static bool ContainsAny(string value, params string[] needles)
        {
            for (int i = 0; i < needles.Length; i++)
                if (value.IndexOf(needles[i], StringComparison.OrdinalIgnoreCase) >= 0) return true;
            return false;
        }

        private static string CsvLine(string[] values)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < values.Length; i++)
            {
                if (i > 0) sb.Append(',');
                sb.Append(CsvEscape(values[i]));
            }
            return sb.ToString();
        }

        private static string CsvEscape(string value)
        {
            if (value == null) return string.Empty;
            if (value.IndexOfAny(new[] { ',', '"', '\r', '\n' }) < 0) return value;
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
    }
