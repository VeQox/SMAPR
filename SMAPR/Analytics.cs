using System.Diagnostics;

namespace SMAPR
{
    public class Analytics : ProgressBar
    {
        #region Member Variables
        private static readonly string[] FileSizes =
        {
            "B",
            "KB",
            "MB",
            "GB",
            "TB",
        };

        private Files Files { get; set; }
        private Stopwatch Watch { get; set; }
        #endregion

        #region Constructor
        public Analytics(double total, char progressChar = '#', char placeholderChar = '-') : base(total, progressChar, placeholderChar)
        {
            Files = new();
            Watch = new();
            Watch.Start();
        }

        public Analytics(double total, char progressChar = '#', char placeholderChar = '-', ConsoleColor progressColor = ConsoleColor.Green, ConsoleColor placeholderColor = ConsoleColor.White) : base(total, progressChar, placeholderChar, progressColor, placeholderColor)
        {
            Files = new();
            Watch = new();
            Watch.Start();
        }
        #endregion

        #region Functions
        public void Update(FileInfo file, bool successful)
        {
            Console.Clear();
            if (successful)
                Files.Successful++;
            else
                Files.Failed++;

            Update(file.Length);
            PrintFileInfo(file);
        }

        public void Finish()
        {
            Watch.Stop();

            Console.Clear();
            PrintProgressBar();
            
            Console.WriteLine($"\n{Files.Successful} Files copied" +
                              $"\n{Files.Failed} Files failed\n\n" +
                              $"\nElapsed: {Watch.Elapsed.Hours:00}:{Watch.Elapsed.Minutes:00}:{Watch.Elapsed.Seconds:00}" +
                              $"\nTotal: {GetFileSize((long)Progress)}" +
                              $"\nMB/s: {Math.Round(Progress / 1024 / 1024 / (Watch.ElapsedMilliseconds / 1000.0), 2)}\n");
        }

        private void PrintFileInfo(FileInfo file)
        {
            Console.WriteLine($"Name: {file.Name}" +
                              $"\nPath: {CapString(file.FullName, Console.BufferWidth - 6)}" +
                              $"\nSize: {GetFileSize(file)}" +
                              $"\nMB/s: {Math.Round(Progress / 1024 / 1024 / (Watch.ElapsedMilliseconds / 1000), 2)}" +
                              $"\n" +
                              $"\n{Files.Successful} Files copied" +
                              $"\n{Files.Failed} Files failed");
        }

        private static string CapString(string longString, int length)
        {
            if (longString == null) return "";

            if (longString.Length <= length) return longString;

            string cappedString = longString[..(length - 3)];
            cappedString += "...";
            return cappedString;
        }

        private static string GetFileSize(FileInfo file)
        {
            return GetFileSize(file.Length);
        }

        private static string GetFileSize(long size)
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
        #endregion
    }
}
