using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAPR
{
    public static class SMAPR
    {
        private static ProgressBar? Bar { get; set; }

        public static void Backup(string source, string destination = @"\\192.168.0.22\public", bool analytics = true)
        {
            DirectoryInfo newBackupFolder = CreateFolderStructure(destination);

            if (File.Exists(source))
            {
                Backup(new FileInfo(source), newBackupFolder, analytics);
            }
            else if (Directory.Exists(source))
            {
                Backup(new DirectoryInfo(source), newBackupFolder, analytics);
            }
            else
            {
                Console.WriteLine($"Source not found {source}");
                newBackupFolder.Delete(true);
            }
        }

        public static void Backup(DirectoryInfo source, DirectoryInfo destination, bool analytics)
        {
            DirectoryInfo newBackupFolder = CreateFolderStructure(destination);

            if (source.Exists)
            {
                long size = (DirSize(source));

                if (analytics && size > 0)
                    Bar = new(DirSize(source));

                BackupDir(source, newBackupFolder);
            }
            else
            {
                Console.WriteLine($"Directory not found {source.FullName}");
                newBackupFolder.Delete(true);
            }
        }

        public static void Backup(FileInfo source, DirectoryInfo destination, bool analytics)
        {
            DirectoryInfo newBackupFolder = CreateFolderStructure(destination);

            if (source.Exists)
            {
                long size = source.Length;

                if (analytics && size > 0)
                    Bar = new(size);

                BackupFile(source, newBackupFolder);
            }
            else
            {
                Console.WriteLine($"File not found {source.FullName}");
                newBackupFolder.Delete(true);
            }
        }


        private static DirectoryInfo CreateFolderStructure(string source)
        {
            DateTime now = DateTime.Now;
            string backupFolder = Path.Combine(source, "backup");
            DirectoryInfo newBackupFolder = new(Path.Combine(backupFolder, $"{now.Day}.{now.Month}.{now.Year}_{now.Hour}-{now.Minute}-{now.Second}-{now.Millisecond}"));
            newBackupFolder.Create();
            return newBackupFolder;
        }

        private static DirectoryInfo CreateFolderStructure(DirectoryInfo source)
        {
            return CreateFolderStructure(source.FullName);
        }


        private static void BackupDir(DirectoryInfo dir, DirectoryInfo destinationDir)
        {
            destinationDir.Create();

            BackupFiles(dir.GetFiles(), destinationDir);

            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                subDir.Create();
                BackupDir(subDir, new DirectoryInfo(Path.Combine(destinationDir.FullName, subDir.Name)));
            }
        }

        private static void BackupFiles(FileInfo[] files, DirectoryInfo destinationDir)
        {
            foreach (FileInfo file in files)
            {
                BackupFile(file, destinationDir);
            }
        }

        private static void BackupFile(FileInfo file, DirectoryInfo destinationDir)
        {
            try
            {
                File.Copy(file.FullName, Path.Combine(destinationDir.FullName, file.Name));
                Bar?.Update(file, ProgressBar.Backup.Successfull);
            }
            catch
            {
                Bar?.Update(file, ProgressBar.Backup.Failed);
            }

        }


        private static long DirSize(DirectoryInfo d)
        {
            return FileSizes(d.GetFiles("*", new EnumerationOptions() { RecurseSubdirectories = true}));
        }

        private static long FileSizes(FileInfo[] files)
        {
            long size = 0;
            foreach (FileInfo file in files)
            {
                size += FileSize(file);
            }
            return size;
        }

        private static long FileSize(FileInfo file)
        {
            return file.Length;
        }
    }
}
