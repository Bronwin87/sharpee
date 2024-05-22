using System.Reflection;

namespace StandardLibrary
{
    public class TitleCard
    {
        public string Title { get; }
        public string[] Authors { get; }
        public string Publisher { get; }
        public int MajorVersion { get; }
        public int MinorVersion { get; }
        public int PatchVersion { get; }
        public string IFID { get; }
        public DateTime DateCompiled { get; }

        public TitleCard(string title, string[] authors, string publisher,
                            int majorVersion, int minorVersion, int patchVersion,
                            string IFID)
        {
            this.Title = title;
            this.Authors = authors;
            this.Publisher = publisher;
            this.MajorVersion = MajorVersion;
            this.MinorVersion = minorVersion;
            this.PatchVersion = patchVersion;
            this.IFID = IFID;
            this.DateCompiled = GetAssemblyBuildDate();
        }

        private DateTime GetAssemblyBuildDate()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            DateTime buildDate = default;

            if (assembly.Location != null)
            {
                buildDate = File.GetLastWriteTime(assembly.Location);
            }
            else
            {
                // Fallback to the current date and time if the assembly location is not available
                buildDate = DateTime.Now;
            }

            return buildDate;
        }

    }
}
