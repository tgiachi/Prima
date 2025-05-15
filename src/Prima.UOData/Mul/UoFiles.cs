using System.Security.Cryptography;
using Serilog;

namespace Prima.UOData.Mul;

public class UoFiles
{
    /// <summary>
    ///     Should loaded Data be cached
    /// </summary>
    public static bool CacheData { get; set; } = true;

    /// <summary>
    ///     Should a Hashfile be used to speed up loading
    /// </summary>
    public static bool UseHashFile { get; set; }

    public static string RootDir { get; set; }

    private static readonly ILogger _logger = Log.ForContext<UoFiles>();

    /// <summary>
    ///     Contains the path infos
    /// </summary>
    public static Dictionary<string, string> MulPath { get; } = new();

    private static readonly string[] _files =
    [
        "anim.idx", "anim.mul", "anim2.idx", "anim2.mul", "anim3.idx", "anim3.mul", "anim4.idx", "anim4.mul", "anim5.idx",
        "anim5.mul", "animdata.mul", "art.mul", "artidx.mul", "artlegacymul.uop", "body.def", "bodyconv.def", "client.exe",
        "cliloc.custom1", "cliloc.custom2", "cliloc.deu", "cliloc.enu", "equipconv.def", "facet00.mul", "facet01.mul",
        "facet02.mul", "facet03.mul", "facet04.mul", "facet05.mul", "fonts.mul", "gump.def", "gumpart.mul", "gumpidx.mul",
        "gumpartlegacymul.uop", "hues.mul", "light.mul", "lightidx.mul", "map0.mul", "map1.mul", "map2.mul", "map3.mul",
        "map4.mul", "map5.mul", "map0legacymul.uop", "map1legacymul.uop", "map2legacymul.uop", "map3legacymul.uop",
        "map4legacymul.uop", "map5legacymul.uop", "mapdif0.mul", "mapdif1.mul", "mapdif2.mul", "mapdif3.mul", "mapdif4.mul",
        "mapdifl0.mul", "mapdifl1.mul", "mapdifl2.mul", "mapdifl3.mul", "mapdifl4.mul", "mobtypes.txt", "multi.idx",
        "multi.mul", "multimap.rle", "radarcol.mul", "skillgrp.mul", "skills.idx", "skills.mul", "sound.def", "sound.mul",
        "soundidx.mul", "soundlegacymul.uop", "speech.mul", "stadif0.mul", "stadif1.mul", "stadif2.mul", "stadif3.mul",
        "stadif4.mul", "stadifi0.mul", "stadifi1.mul", "stadifi2.mul", "stadifi3.mul", "stadifi4.mul", "stadifl0.mul",
        "stadifl1.mul", "stadifl2.mul", "stadifl3.mul", "stadifl4.mul", "staidx0.mul", "staidx1.mul", "staidx2.mul",
        "staidx3.mul", "staidx4.mul", "staidx5.mul", "statics0.mul", "statics1.mul", "statics2.mul", "statics3.mul",
        "statics4.mul", "statics5.mul", "texidx.mul", "texmaps.mul", "tiledata.mul", "unifont.mul", "unifont1.mul",
        "unifont2.mul", "unifont3.mul", "unifont4.mul", "unifont5.mul", "unifont6.mul", "unifont7.mul", "unifont8.mul",
        "unifont9.mul", "unifont10.mul", "unifont11.mul", "unifont12.mul", "uotd.exe", "verdata.mul"
    ];


    public static void ReLoadDirectory()
    {
        MulPath.Clear();

        ScanForFiles(RootDir);
    }


    public static void ScanForFiles(string path = "")
    {
        RootDir = path;
        var files = Directory.GetFiles(path, "*.*");
        _logger.Information("Found {Count} files", files.Length);

        foreach (var file in files)
        {
            var exists = _files.Any(f => f.Equals(Path.GetFileName(file), StringComparison.OrdinalIgnoreCase));

            if (exists)
            {
                var fileName = Path.GetFileName(file);
                var filePath = Path.GetDirectoryName(file);
                if (filePath != null)
                {
                    MulPath[fileName.ToLower()] = Path.Combine(filePath, fileName);
                    _logger.Debug("Found UO {File}", fileName.ToLower());
                }
            }
        }
    }

    /// <summary>
    ///     Sets <see cref="MulPath" /> key to path
    /// </summary>
    /// <param name="path"></param>
    /// <param name="key"></param>
    public static void SetMulPath(string path, string key)
    {
        MulPath[key] = path;
    }

    public static string GetFilePath(string fileName)
    {
        return MulPath.GetValueOrDefault(fileName.ToLower());
    }

    public static string? FindDataFile(string fileName, bool throwError = true)
    {
        var filePath = MulPath.GetValueOrDefault(fileName.ToLower());
        if (filePath == null && throwError)
        {
            throw new FileNotFoundException($"File {fileName} not found in {RootDir}");
        }

        return filePath;
    }

    /// <summary>
    ///     Compares given MD5 hash with hash of given file
    /// </summary>
    /// <param name="file"></param>
    /// <param name="hash"></param>
    /// <returns></returns>
    public static bool CompareMD5(string file, string hash)
    {
        if (file == null)
        {
            return false;
        }

        var FileCheck = File.OpenRead(file);
        using MD5 md5 = MD5.Create();
        byte[] md5Hash = md5.ComputeHash(FileCheck);
        FileCheck.Close();
        string md5string = BitConverter.ToString(md5Hash).Replace("-", "").ToLower();
        return md5string == hash;
    }

    /// <summary>
    ///     Returns MD5 hash from given file
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static byte[] GetMD5(string? file)
    {
        if (file == null)
        {
            return null;
        }

        var FileCheck = File.OpenRead(file);
        using MD5 md5 = MD5.Create();
        byte[] md5Hash = md5.ComputeHash(FileCheck);
        FileCheck.Close();
        return md5Hash;
    }

    /// <summary>
    ///     Compares MD5 hash from given mul file with hash in responsible hash-file
    /// </summary>
    /// <param name="what"></param>
    /// <returns></returns>
    public static bool CompareHashFile(string what, string path)
    {
        string FileName = Path.Combine(path, $"UOFiddler{what}.hash");
        if (File.Exists(FileName))
        {
            try
            {
                using var bin = new BinaryReader(new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read));
                int length = bin.ReadInt32();
                var buffer = new byte[length];
                bin.Read(buffer, 0, length);
                string hashold = BitConverter.ToString(buffer).Replace("-", "").ToLower();
                return CompareMD5(GetFilePath($"{what}.mul"), hashold);
            }
            catch
            {
                return false;
            }
        }

        return false;
    }
}
