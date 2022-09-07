using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAPR
{
    class ProgressBar
    {
        public enum Backup
        {
            Successfull,
            Failed
        }

        private static int Length { get { return Console.BufferWidth - 2; } }

        public long Total { get; private set; }
        private long Progress { get; set; }
        private Files Files { get; set; }


        public ProgressBar(long total)
        {
            Files = new();
            Total = total;
        }

        public void Update(FileInfo file, Backup status)
        {
            UpdateFiles(status);
            Console.Clear();

            Progress += file.Length;
            PrintProgressBar();
            PrintFileInfo(file);

        }

        private void UpdateFiles(Backup status)
        {
            if (status == Backup.Successfull)
                Files.Successful++;
            else if (status == Backup.Failed)
                Files.Failed++;
        }


        private void PrintProgressBar()
        {
            // Amount of # in the ProgressBar
            double mask = (double)Progress / Total * Length;

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("[");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(new string('#', (int)mask));

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"{new string('-', Length - (int)mask)}]\n");
        }

        private void PrintFileInfo(FileInfo file)
        {
            Console.WriteLine($"Name: {file.Name}");
            Console.WriteLine($"Path: {CapString(file.FullName, Console.BufferWidth-6)}");
            Console.WriteLine($"Size: {GetFileSize(file)}");

            Console.WriteLine($"\n{Files.Successful} Files backuped");
            Console.WriteLine($"{Files.Failed} Files failed");
        }

        private static string CapString(string longString, int length)
        {
            if (longString == null) return "";
            
            if(longString.Length <= length) return longString;

            string cappedString = longString[..(length - 3)];
            cappedString += "...";
            return cappedString;
        }


        private static string GetFileSize(FileInfo file)
        {
            string[] FileSizes =
            {
                "B",
                "KB",
                "MB",
                "GB",
                "TB",
            };

            long size = file.Length;
            int fileSizeIndex = 0;

            for (int i = 1; i < size || i < FileSizes.Length; i = (int)Math.Pow(1024, fileSizeIndex))
            {
                if(size / i < 1024)
                {
                    return $"{size / i} {FileSizes[fileSizeIndex]}";
                }
                fileSizeIndex++;
            }

            return size.ToString();
        }

        public void Finish()
        {
            Console.Clear();
            PrintProgressBar();

            Console.Write("\nBackup Finished");
            Console.Write($"\n\n{Files.Successful} Files backuped");
            Console.Write($"\n{Files.Failed} Files failed\n\n");
        }
    }
}