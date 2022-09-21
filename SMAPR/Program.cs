using System.Data;
using System.Diagnostics;

namespace SMAPR
{
    public class Program
    {
        public static void Main(string[] args)
        {
#pragma warning disable CA1416 // Validate platform compatibility
            Console.SetWindowSize(120, 20);
            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
#pragma warning restore CA1416 // Validate platform compatibility

            string workingDir = Directory.GetCurrentDirectory();

            Console.WriteLine($"Clearing Workingdir {workingDir}");
            string source = Path.Combine(workingDir, "source");
            string destination = Path.Combine(workingDir, "sourceCopy");

            DirectoryInfo sourceDir = new(source);
            DirectoryInfo destinationDir = new(destination);
            ReCreateWorkingDir(sourceDir, destinationDir);

            Console.WriteLine("Creating Files...");
            int amount = 1000;
            long min = 1_024 * 2;
            long max = 1_024 * 1_024 * 10;
            Console.WriteLine($"Est. DirSize: {FormatedFileSize(((max - min) / 2 + min) * amount)}");
            CreateFiles(amount, sourceDir, min, max);

            Console.WriteLine("Files Created");

            Console.Clear();

            SMAPR.CopyDirectory(sourceDir, destinationDir, true, true, true, true);
        }
        private static void CreateFiles(int amount, DirectoryInfo sourceDir, long min, long max)
        {
            if (sourceDir.Exists)
                sourceDir.Delete(true);

            sourceDir.Create();

            FileInfo[] sourceFiles = new FileInfo[amount];

            for (int i = 0; i < amount; i++)
            {
                sourceFiles[i] = new(Path.Combine(sourceDir.FullName, $"{i}.txt"));
                CreateFile(sourceFiles[i], min, max);
            }
        }
        private static void CreateFile(FileInfo file, long min, long max)
        {
            Random random = new();

            FileStream fs = file.OpenWrite();
            byte[] fileContent = new byte[random.NextInt64(min, max)];

            for (int i = 0; i < fileContent.Length; i++)
            {
                fileContent[i] = (byte)random.Next(0, 256);
            }

            fs.Write(fileContent, 0, fileContent.Length);
            fs.Close();
        }

        private static void ReCreateWorkingDir(params DirectoryInfo[] ndirs)
        {
            foreach (DirectoryInfo dir in ndirs)
            {
                if (dir.Exists)
                    dir.Delete(true);
                dir.Create();
            }
        }

        private static readonly string[] FileSizes =
{
            "B",
            "KB",
            "MB",
            "GB",
            "TB",
        };
        private static string FormatedFileSize(long size)
        {
            int fileSizeIndex = 0;

            for (int i = 1; i < size || i < FileSizes.Length; i = (int)Math.Pow(1024, fileSizeIndex))
            {
                if (size / i < 1024)
                {
                    return $"{size / i} {FileSizes[fileSizeIndex]}";
                }
                fileSizeIndex++;
            }

            return size.ToString();
        }
    }
}