
namespace NewYear2011.Helper
{
    public static class MainHelper
    {
        public static string GetApplicationDir()
        {
            return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }
    }
}
