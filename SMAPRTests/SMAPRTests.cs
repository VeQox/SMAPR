using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SMAPR.Tests
{
    [TestClass()]
    public class SMAPRTests
    {
        public const int Seed = 42069; // I know very mature

        [TestMethod()]
        public void CopyDirManyFilesSync()
        {
            const long min = 2048;
            const long max = 2048 * 10;

            DirectoryInfo sourceDir = new(Path.Combine(Directory.GetCurrentDirectory(), "source"));
            DirectoryInfo destinationDir = new(Path.Combine(Directory.GetCurrentDirectory(), "source"));

            ReCreateWorkingDir(sourceDir, destinationDir);
            CreateFiles(100, sourceDir, min, max);

            SMAPR.CopyDirectory(sourceDir, destinationDir, false, false, false, false);

            var sourceFiles = GetFiles(sourceDir, false);
            var destinationFiles = GetFiles(destinationDir, false);

            Assert.IsTrue(CompareFiles(sourceFiles, destinationFiles));
        }

        [TestMethod()]
        public void CopyDirOneFileSync()
        {
            const long min = 2048;
            const long max = 2048 * 10;

            DirectoryInfo sourceDir = new(Path.Combine(Directory.GetCurrentDirectory(), "source"));
            DirectoryInfo destinationDir = new(Path.Combine(Directory.GetCurrentDirectory(), "source"));

            ReCreateWorkingDir(sourceDir, destinationDir);
            CreateFiles(1, sourceDir, min, max);

            SMAPR.CopyDirectory(sourceDir, destinationDir, false, false, false, false);

            var sourceFiles = GetFiles(sourceDir, false);
            var destinationFiles = GetFiles(destinationDir, false);

            Assert.IsTrue(CompareFiles(sourceFiles, destinationFiles));
        }

        [TestMethod()]
        public void CopySubDirManyFilesSync()
        {
            const long min = 2048;
            const long max = 2048 * 10;

            DirectoryInfo sourceDir = new(Path.Combine(Directory.GetCurrentDirectory(), "source"));
            DirectoryInfo destinationDir = new(Path.Combine(Directory.GetCurrentDirectory(), "source"));

            ReCreateWorkingDir(sourceDir, destinationDir);
            DirectoryInfo subDir = sourceDir.CreateSubdirectory("subDir");

            CreateFiles(100, subDir, min, max);

            SMAPR.CopyDirectory(sourceDir, destinationDir, false, false, true, false);

            var sourceFiles = GetFiles(sourceDir, true);
            var destinationFiles = GetFiles(destinationDir, true);

            Assert.IsTrue(CompareFiles(sourceFiles, destinationFiles));
        }

        [TestMethod()]
        public void CopySubDirsManyFilesSync()
        {
            const long min = 2048;
            const long max = 2048 * 10;

            DirectoryInfo sourceDir = new(Path.Combine(Directory.GetCurrentDirectory(), "source"));
            DirectoryInfo destinationDir = new(Path.Combine(Directory.GetCurrentDirectory(), "source"));

            ReCreateWorkingDir(sourceDir, destinationDir);
            DirectoryInfo subDir = sourceDir.CreateSubdirectory("subDir");

            CreateFiles(100, sourceDir, min, max);
            CreateFiles(100, subDir, min, max);

            SMAPR.CopyDirectory(sourceDir, destinationDir, false, false, true, false);

            var sourceFiles = GetFiles(sourceDir, true);
            var destinationFiles = GetFiles(destinationDir, true);

            Assert.IsTrue(CompareFiles(sourceFiles, destinationFiles));
        }

        [TestMethod()]
        public void CopyEmptySubDirsSync()
        {
            DirectoryInfo sourceDir = new(Path.Combine(Directory.GetCurrentDirectory(), "source"));
            DirectoryInfo destinationDir = new(Path.Combine(Directory.GetCurrentDirectory(), "source"));

            ReCreateWorkingDir(sourceDir, destinationDir);
            for (int i = 0; i < 10; i++)
                sourceDir.CreateSubdirectory($"subDir {i}");
            
            SMAPR.CopyDirectory(sourceDir, destinationDir, false, false, true, false);

            var sourceFiles = GetDirectories(sourceDir, true);
            var destinationFiles = GetDirectories(destinationDir, true);

            Assert.IsTrue(CompareDirs(sourceFiles, destinationFiles));
        }

        [TestMethod()]
        public void CopyDirManyFilesThreaded()
        {
            const long min = 2048;
            const long max = 2048 * 10;

            DirectoryInfo sourceDir = new(Path.Combine(Directory.GetCurrentDirectory(), "source"));
            DirectoryInfo destinationDir = new(Path.Combine(Directory.GetCurrentDirectory(), "source"));

            ReCreateWorkingDir(sourceDir, destinationDir);
            CreateFiles(100, sourceDir, min, max);

            SMAPR.CopyDirectory(sourceDir, destinationDir, false, false, false, true);

            var sourceFiles = GetFiles(sourceDir, false);
            var destinationFiles = GetFiles(destinationDir, false);

            Assert.IsTrue(CompareFiles(sourceFiles, destinationFiles));
        }

        [TestMethod()]
        public void CopyDirOneFileThreaded()
        {
            const long min = 2048;
            const long max = 2048 * 10;

            DirectoryInfo sourceDir = new(Path.Combine(Directory.GetCurrentDirectory(), "source"));
            DirectoryInfo destinationDir = new(Path.Combine(Directory.GetCurrentDirectory(), "source"));

            ReCreateWorkingDir(sourceDir, destinationDir);
            CreateFiles(1, sourceDir, min, max);

            SMAPR.CopyDirectory(sourceDir, destinationDir, false, false, false, true);

            var sourceFiles = GetFiles(sourceDir, false);
            var destinationFiles = GetFiles(destinationDir, false);

            Assert.IsTrue(CompareFiles(sourceFiles, destinationFiles));
        }

        [TestMethod()]
        public void CopySubDirManyFilesThreaded()
        {
            const long min = 2048;
            const long max = 2048 * 10;

            DirectoryInfo sourceDir = new(Path.Combine(Directory.GetCurrentDirectory(), "source"));
            DirectoryInfo destinationDir = new(Path.Combine(Directory.GetCurrentDirectory(), "source"));

            ReCreateWorkingDir(sourceDir, destinationDir);
            DirectoryInfo subDir = sourceDir.CreateSubdirectory("subDir");

            CreateFiles(100, subDir, min, max);

            SMAPR.CopyDirectory(sourceDir, destinationDir, false, false, true, true);

            var sourceFiles = GetFiles(sourceDir, true);
            var destinationFiles = GetFiles(destinationDir, true);

            Assert.IsTrue(CompareFiles(sourceFiles, destinationFiles));
        }

        [TestMethod()]
        public void CopySubDirsManyFilesThreaded()
        {
            const long min = 2048;
            const long max = 2048 * 10;

            DirectoryInfo sourceDir = new(Path.Combine(Directory.GetCurrentDirectory(), "source"));
            DirectoryInfo destinationDir = new(Path.Combine(Directory.GetCurrentDirectory(), "source"));

            ReCreateWorkingDir(sourceDir, destinationDir);
            DirectoryInfo subDir = sourceDir.CreateSubdirectory("subDir");

            CreateFiles(100, sourceDir, min, max);
            CreateFiles(100, subDir, min, max);

            SMAPR.CopyDirectory(sourceDir, destinationDir, false, false, true, true);

            var sourceFiles = GetFiles(sourceDir, true);
            var destinationFiles = GetFiles(destinationDir, true);

            Assert.IsTrue(CompareFiles(sourceFiles, destinationFiles));
        }

        [TestMethod()]
        public void CopyEmptySubDirsThreaded()
        {
            DirectoryInfo sourceDir = new(Path.Combine(Directory.GetCurrentDirectory(), "source"));
            DirectoryInfo destinationDir = new(Path.Combine(Directory.GetCurrentDirectory(), "source"));

            ReCreateWorkingDir(sourceDir, destinationDir);
            for (int i = 0; i < 10; i++)
                sourceDir.CreateSubdirectory($"subDir {i}");

            SMAPR.CopyDirectory(sourceDir, destinationDir, false, false, true, true);

            var sourceFiles = GetDirectories(sourceDir, true);
            var destinationFiles = GetDirectories(destinationDir, true);

            Assert.IsTrue(CompareDirs(sourceFiles, destinationFiles));
        }

        [TestMethod()]
        public void CopyFileSync()
        {
            const long min = 2048;
            const long max = 2048 * 10;

            FileInfo file = new(Path.Combine(Directory.GetCurrentDirectory(), "file.txt"));
            DirectoryInfo destinationDir = new(Path.Combine(Directory.GetCurrentDirectory(), "source"));

            ReCreateWorkingDir(destinationDir);
            CreateFile(file, min, max);

            SMAPR.CopyFile(file, destinationDir, false);

            var destinationFiles = GetFiles(destinationDir, false);

            Assert.IsTrue(CompareFile(file, destinationFiles[0]));
        }

        private static bool CompareDirs(DirectoryInfo[] sourceDirs, DirectoryInfo[] copiedDirs)
        {
            if (sourceDirs.Length != copiedDirs.Length) return false;

            for (int i = 0; i < sourceDirs.Length; i++)
                if(!CompareDir(sourceDirs[i], copiedDirs[i])) return false;

            return true;
        }
        private static bool CompareDir(DirectoryInfo sourceDirs, DirectoryInfo copiedDirs)
        {
            if (sourceDirs.Name != copiedDirs.Name) return false;
            return true;
        }
        private static bool CompareFiles(FileInfo[] sourceFiles, FileInfo[] copiedFiles)
        {
            if (sourceFiles.Length != copiedFiles.Length) return false;

            for(int i = 0; i < sourceFiles.Length; i++)
                if (!CompareFile(sourceFiles[i], copiedFiles[i])) return false;

            return true;
        }
        private static bool CompareFile(FileInfo sourceFile, FileInfo copiedFiled)
        {
            if (sourceFile.Length != copiedFiled.Length) return false;
            return true;
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
                if(dir.Exists)
                    dir.Delete(true);
                dir.Create();
            }
        }

        private static FileInfo[] GetFiles(DirectoryInfo dir, bool recursive)
        {
            return dir.GetFiles("*", new EnumerationOptions() { RecurseSubdirectories = recursive });
        }
        private static DirectoryInfo[] GetDirectories(DirectoryInfo dir, bool recursive)
        {
            return dir.GetDirectories("*", new EnumerationOptions() { RecurseSubdirectories = recursive });
        }
    }
}