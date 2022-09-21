using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Channels;

namespace SMAPR
{
    public static class SMAPR
    {
        private static Analytics? Analytics;

        #region Copy
        public static void CopyDirectory(DirectoryInfo sourceDir, DirectoryInfo destinationDir, bool analytics)
        {
            CopyDirectory(sourceDir, destinationDir, analytics, false);
        }
        public static void CopyDirectory(DirectoryInfo sourceDir, DirectoryInfo destinationDir, bool analytics, bool overwrite)
        {
            CopyDirectory(sourceDir, destinationDir, analytics, overwrite, false);
        }
        public static void CopyDirectory(DirectoryInfo sourceDir, DirectoryInfo destinationDir, bool analytics, bool overwrite, bool recursive)
        {
            CopyDirectory(sourceDir, destinationDir, analytics, overwrite, recursive, false);
        }
        public static void CopyDirectory(DirectoryInfo sourceDir, DirectoryInfo destinationDir, bool analytics, bool overwrite, bool recursive, bool threaded)
        {
            if(analytics)
                Analytics = new(DirSize(sourceDir, recursive), '#', '-', ConsoleColor.Green, ConsoleColor.White);

            if (destinationDir.Exists)
                destinationDir.Create();

            MirrorSubDirs(sourceDir,destinationDir, recursive);

            if (threaded)
                CopyDirThreaded(sourceDir, destinationDir, overwrite, recursive);
            else
                foreach (Tuple<FileInfo, bool> latestFile in CopyDir(sourceDir, destinationDir, overwrite, recursive))
                    Analytics?.Update(latestFile.Item1, latestFile.Item2);

            Analytics?.Finish();
        }

        private static void CopyDirThreaded(DirectoryInfo sourceDir, DirectoryInfo destinationDir, bool overwrite, bool recursive)
        {
            CopyFilesThreaded(GetFiles(sourceDir, recursive), destinationDir, overwrite);
        }
        private static void CopyFilesThreaded(FileInfo[] files, DirectoryInfo destinationDir, bool overwrite)
        {
            Mutex mutex = new();
            Thread[] threads = new Thread[Environment.ProcessorCount];

            files = files.ToList().OrderBy(x => x.Length).ToArray();

            List<FileInfo>[] distributedFiles = new List<FileInfo>[Environment.ProcessorCount];
            for (int i = 0; i < distributedFiles.Length; i++)
                distributedFiles[i] = new List<FileInfo>();

            int tmp = 0;
            foreach(FileInfo file in files)
            {
                distributedFiles[tmp].Add(file);

                tmp++;
                if (tmp >= Environment.ProcessorCount) tmp = 0;
            }

            for(int i = 0; i < threads.Length; i++)
            {
                int id = i;
                threads[id] = new(() =>
                {
                    FileInfo[] files = distributedFiles[id].ToArray();

                    foreach (FileInfo file in files)
                    {
                        bool status = CopyFile(file, destinationDir, overwrite);
                        mutex.WaitOne();
                        Analytics?.Update(file, status);
                        mutex.ReleaseMutex();
                    }
                });
                threads[id].Start();
            }

            foreach (Thread thread in threads)
            {
                thread.Join();
            }
        }

        private static IEnumerable<Tuple<FileInfo, bool>> CopyDir(DirectoryInfo sourceDir, DirectoryInfo destinationDir, bool overwrite, bool recursive)
        {
            foreach (Tuple<FileInfo, bool> latestFile in CopyFiles(GetFiles(sourceDir, recursive), destinationDir, overwrite))
            {
                yield return latestFile;
            }
        }
        private static IEnumerable<Tuple<FileInfo, bool>> CopyFiles(FileInfo[] files, DirectoryInfo destinationDir, bool overwrite)
        {
            for (int i = 0; i < files.Length; i++)
            {
                bool status = CopyFile(files[i], destinationDir, overwrite);
                yield return new Tuple<FileInfo, bool>(files[i], status);
            }
        }  

        public static bool CopyFile(FileInfo file, DirectoryInfo destinationDir, bool overwrite)
        {
            try
            {
                file.CopyTo(Path.Combine(destinationDir.FullName, file.Name), overwrite);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Backup
        public static void Backup(string source, string destination, bool analytics, bool threaded)
        {
            DirectoryInfo backupFolder = CreateFolderStructure(destination);

            if (File.Exists(source))
            {
                BackupFile(new FileInfo(source), backupFolder);
            }
            else if (Directory.Exists(source))
            {
                DirectoryInfo sourceDir = new(source);
                BackupDir(sourceDir, backupFolder, analytics, threaded);
            }
            else
            {
                Console.WriteLine($"Source not found {source}");
                backupFolder.Delete(true);
            }
        }
        private static void BackupDir(DirectoryInfo sourceDir, DirectoryInfo destinationDir, bool analytics, bool threaded)
        {
            CopyDirectory(sourceDir, destinationDir, false, true, analytics, threaded);
        }
        private static void BackupFile(FileInfo sourceFile, DirectoryInfo destinationDir)
        {
            CopyFile(sourceFile, destinationDir, true);
        }

        private static DirectoryInfo CreateFolderStructure(string source)
        {
            DateTime now = DateTime.Now;
            string backupFolder = Path.Combine(source, "backup");
            DirectoryInfo newBackupFolder = new(Path.Combine(backupFolder, $"{now.Day}.{now.Month}.{now.Year}_{now.Hour}-{now.Minute}-{now.Second}-{now.Millisecond}"));
            newBackupFolder.Create();
            return newBackupFolder;
        }
        #endregion

        #region Functions
        private static FileInfo[] GetFiles(DirectoryInfo dir, bool recursive)
        {
            return dir.GetFiles("*", new EnumerationOptions() { RecurseSubdirectories = recursive });
        }
        private static DirectoryInfo[] GetSubDirs(DirectoryInfo dir, bool recursive)
        {
            return dir.GetDirectories("*", new EnumerationOptions() { RecurseSubdirectories = recursive });
        }

        private static void MirrorSubDirs(DirectoryInfo sourceDir, DirectoryInfo destinationDir, bool recursive)
        {
            DirectoryInfo[] subDirs = GetSubDirs(sourceDir, recursive);

            foreach (DirectoryInfo subDir in subDirs)
            {
                string path = subDir.FullName.Replace(sourceDir.FullName, destinationDir.FullName);
                DirectoryInfo destinationSubDir = new(path);
                destinationSubDir.Create();
            }
        }

        private static long DirSize(DirectoryInfo dir, bool recursive)
        {
            return FileSizes(GetFiles(dir, recursive));
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
        #endregion
    }
}