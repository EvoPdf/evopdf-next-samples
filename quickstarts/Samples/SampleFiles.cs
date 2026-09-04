namespace EvoPdf.Next.Samples
{
    // Input documents ship in the Files folder (copied next to the executable); results go to the output folder.
    internal static class SampleFiles
    {
        public static string Input(string name) => Path.Combine(AppContext.BaseDirectory, "Files", name);

        public static string Output(string name)
        {
            string folder = Path.Combine(AppContext.BaseDirectory, "output");
            Directory.CreateDirectory(folder);
            return Path.Combine(folder, name);
        }
    }
}
