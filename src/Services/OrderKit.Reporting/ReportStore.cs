using System.IO;

namespace OrderKit;

/// <summary>Reads and writes report files on disk.</summary>
public static class ReportStore
{
    public static void Save(string path, string content)
    {
        File.WriteAllText(path, content ?? string.Empty);
    }

    public static string Load(string path)
    {
        return File.Exists(path) ? File.ReadAllText(path) : null;
    }

    public static string LoadFromRoot(string root, string name)
    {
        string full = Path.Combine(root, name);
        return Load(full);
    }

    public static int CountLines(string path)
    {
        string text = Load(path);
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }

        int lines = 0;
        foreach (string line in text.Split('\n'))
        {
            if (line.Length > 0)
            {
                lines++;
            }
        }

        return lines;
    }
}
