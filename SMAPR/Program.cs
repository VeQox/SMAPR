namespace SMAPR
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.SetBufferSize(Console.BufferWidth, Console.WindowHeight);

            string location = Path.Combine(Directory.GetCurrentDirectory(), "hallo");

            SMAPR.Backup.NewBackup(location, @"\\192.168.0.22\public", true);
        }
    }
}