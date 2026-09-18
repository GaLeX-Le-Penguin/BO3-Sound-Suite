using System.Text;
using BO3SoundSuite.Models;

namespace BO3SoundSuite.Services;

internal static class AliasWavInspector
{
    public static AudioInfo Read(string file)
    {
        var info = new AudioInfo();
        try
        {
            using var fs = File.OpenRead(file);
            using var br = new BinaryReader(fs);
            var riff = Encoding.ASCII.GetString(br.ReadBytes(4));
            br.ReadUInt32();
            var wave = Encoding.ASCII.GetString(br.ReadBytes(4));
            if (riff != "RIFF" || wave != "WAVE") throw new InvalidDataException("RIFF/WAVE invalide");
            info.IsWave = true;
            var foundFmt = false;
            while (fs.Position + 8 <= fs.Length)
            {
                var chunk = Encoding.ASCII.GetString(br.ReadBytes(4));
                var size = br.ReadUInt32();
                var next = fs.Position + size;
                if (chunk == "fmt ")
                {
                    info.AudioFormat = br.ReadUInt16();
                    info.Channels = br.ReadUInt16();
                    info.SampleRate = br.ReadUInt32();
                    br.ReadUInt32();
                    br.ReadUInt16();
                    info.BitsPerSample = br.ReadUInt16();
                    info.IsPcm = info.AudioFormat == 1;
                    foundFmt = true;
                    break;
                }
                fs.Position = Math.Min(next + (size % 2), fs.Length);
            }
            if (!foundFmt) throw new InvalidDataException("Chunk fmt introuvable");
        }
        catch (Exception ex) { info.Error = ex.Message; }
        return info;
    }
}
