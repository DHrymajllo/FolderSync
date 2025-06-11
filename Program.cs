using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

class FolderSync
{
    static string sourcePath = "", replicaPath = "", logFilePath = "";
    static int intervalSeconds;

    static void Main(string[] args)
    {
        if (args.Length != 4)
        {
            Console.WriteLine("Usage: FolderSync.exe <sourcePath> <replicaPath> <intervalSeconds> <logFilePath>");
            return;
        }

        sourcePath = args[0];
        replicaPath = args[1];
        intervalSeconds = int.Parse(args[2]);
        logFilePath = args[3];

        Log("---- FolderSync started ----");

        while (true)
        {
            try
            {
                SyncDirectories(sourcePath, replicaPath);
                Log("Synchronization completed at " + DateTime.Now);
            }
            catch (Exception ex)
            {
                Log("Error: " + ex.Message);
            }

            Thread.Sleep(intervalSeconds * 1000);
        }
    }

    static void SyncDirectories(string source, string replica)
    {
        // Create replica folder if it doesn't exist
        if (!Directory.Exists(replica))
            Directory.CreateDirectory(replica);

        // 1. Copy and update files from source to replica
        foreach (var sourceFilePath in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
        {
            string relativePath = Path.GetRelativePath(source, sourceFilePath);
            string replicaFilePath = Path.Combine(replica, relativePath);

            string? replicaDir = Path.GetDirectoryName(replicaFilePath);
            if (!string.IsNullOrEmpty(replicaDir))
            {
                Directory.CreateDirectory(replicaDir);
            }

            // If file doesn't exist or contents differ, copy it
            if (!File.Exists(replicaFilePath) || !FilesAreEqual(sourceFilePath, replicaFilePath))
            {
                File.Copy(sourceFilePath, replicaFilePath, true);
                Log($"Copied/Updated: {relativePath}");
            }
        }

        // 2. Remove files in replica that don't exist in source
        foreach (var replicaFilePath in Directory.GetFiles(replica, "*", SearchOption.AllDirectories))
        {
            string relativePath = Path.GetRelativePath(replica, replicaFilePath);
            string sourceFilePath = Path.Combine(source, relativePath);

            if (!File.Exists(sourceFilePath))
            {
                File.Delete(replicaFilePath);
                Log($"Deleted: {relativePath}");
            }
        }

        // 3. Remove directories in replica that don't exist in source
        foreach (var replicaDir in Directory.GetDirectories(replica, "*", SearchOption.AllDirectories).OrderByDescending(d => d.Length))
        {
            string relativePath = Path.GetRelativePath(replica, replicaDir);
            string sourceDir = Path.Combine(source, relativePath);

            if (!Directory.Exists(sourceDir))
            {
                Directory.Delete(replicaDir, true);
                Log($"Deleted directory: {relativePath}");
            }
        }
    }

    static bool FilesAreEqual(string file1, string file2)
    {
        // Fast check: length
        FileInfo fi1 = new FileInfo(file1);
        FileInfo fi2 = new FileInfo(file2);
        if (fi1.Length != fi2.Length) return false;

        // Slow check: content hash
        using (var md5 = MD5.Create())
        using (var stream1 = File.OpenRead(file1))
        using (var stream2 = File.OpenRead(file2))
        {
            var hash1 = md5.ComputeHash(stream1);
            var hash2 = md5.ComputeHash(stream2);
            return hash1.SequenceEqual(hash2);
        }
    }

    static void Log(string message)
    {
        string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}";
        Console.WriteLine(line);
        File.AppendAllText(logFilePath, line + Environment.NewLine);
    }
}
