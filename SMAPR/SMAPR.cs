using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMAPR
{
    static class SMAPR
    {
        private const string NAS = @"\\192.168.0.22\public";
        private static readonly string backupFolder = Path.Combine(NAS, "backup");

        public static void Backup(string location)
        {
            if(CreateFolderStructure(location, out DirectoryInfo newBackupFolder))
            {
                if (Directory.Exists(location))
                    BackupDir(location, newBackupFolder.FullName);

                else if (File.Exists(location))
                    CopyFile(location, newBackupFolder.FullName);
            }
        }

        public static bool CreateFolderStructure(string location, out DirectoryInfo newBackupFolder)
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type. bc its bs
            newBackupFolder = default;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            if (!File.Exists(location) && !Directory.Exists(location))
                return false;

            DateTime now = DateTime.Now;
            newBackupFolder = new(Path.Combine(backupFolder, $"{now.Day}.{now.Month}.{now.Year}_{now.Hour}-{now.Minute}-{now.Second}-{now.Millisecond}"));

            return true;
        }

        public static void BackupDir(string sourceDir, string destinationDir)
        {
            ProgressBar bar = new(DirSize(new DirectoryInfo(sourceDir)));

            CopyDir(sourceDir, destinationDir, bar);

            bar.Finish();
        }

        public static void CopyDir(string sourceDir, string destinationDir, ProgressBar bar)
        {
            DirectoryInfo dir = new(sourceDir);

            DirectoryInfo[] dirs = dir.GetDirectories();

            Directory.CreateDirectory(destinationDir);

            bar.Update("", 0);

            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                try
                {
                    file.CopyTo(targetFilePath);
                    bar.successfullFiles.Add(file.Name);
                    bar.Update(file.Name, file.Length);
                }
                catch
                {
                    bar.failedFiles.Add(file.Name);
                    bar.Total -= file.Length;
                }
            }

            foreach (DirectoryInfo subDir in dirs)
            {
                string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDir(subDir.FullName, newDestinationDir, bar);
            }
        }

        public static void CopyFile(string source, string destinationDir)
        {
            File.Copy(source, destinationDir);
        }

        /// <summary>
        /// https://stackoverflow.com/questions/468119/whats-the-best-way-to-calculate-the-size-of-a-directory-in-net
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static long DirSize(DirectoryInfo d)
        {
            long size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirSize(di);
            }
            return size;
        }
    }
}
