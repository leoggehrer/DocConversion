namespace DocConversion.ConApp
{
    /// <summary>
    /// Represents the main program class.
    /// </summary>
    partial class Program
    {
        #region Class-Constructors
        static Program()
        {
            ClassConstructing();
            ClassConstructed();
        }
        static partial void ClassConstructing();
        static partial void ClassConstructed();
        #endregion Class-Constructors

        /// <summary>
        /// The entry point of the application.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        static void Main(string[] args)
        {
            new DocConversionApp().Run(args);
        }

        #region app methods
        /// <summary>
        /// Retrieves a collection of file paths that match the specified search pattern and extensions.
        /// </summary>
        /// <param name="path">The directory to search in.</param>
        /// <param name="searchPattern">The search pattern to match against the names of files in the directory.</param>
        /// <param name="extensions">The file extensions to filter the search results. If no extensions are provided, all files will be included.</param>
        /// <returns>A collection of file paths that match the search pattern and extensions.</returns>
        internal static IEnumerable<string> GetFiles(string path, string searchPattern, params string[] extensions)
        {
            var result = Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories)
                                  .Where(f => extensions.Length != 0 == false
                                           || extensions.Any(e => Path.GetExtension(f).Equals(e, StringComparison.CurrentCultureIgnoreCase)))
                                  .OrderBy(i => i)
                                  .ToArray();

            return result;
        }
        #endregion app methods
    }
}