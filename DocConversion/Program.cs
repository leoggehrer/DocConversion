namespace DocConversion
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
            HomePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
                        Environment.OSVersion.Platform == PlatformID.MacOSX)
                       ? Environment.GetEnvironmentVariable("HOME")
                       : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");

            UserPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            SourcePath = Path.Combine(UserPath, "Downloads");
            TargetPath = "C:\\Users\\g.gehrer\\source\\repos\\leoggehrer\\34_ABIF_ACIF_POSE_EXERCISES";// UserPath;
            ClassConstructed();
        }
        static partial void ClassConstructing();
        static partial void ClassConstructed();
        #endregion Class-Constructors

        #region Properties
        internal static string? HomePath { get; set; }
        internal static string UserPath { get; set; }
        internal static string SourcePath { get; set; }
        internal static string TargetPath { get; set; }
        internal static string TargetExtension => ".md";
        #endregion Properties

        /// <summary>
        /// The entry point of the application.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        static void Main(string[] args)
        {
            if (args.Length > 0 && Directory.Exists(args[0]))
            {
                SourcePath = args[0];
                TargetPath = args[0];
            }
            if (args.Length > 1 && Directory.Exists(args[1]))
            {
                TargetPath = args[1];
            }
            RunApp();
        }

        #region App methods
        /// <summary>
        /// Runs the application and allows the user to interactively choose source and target paths, as well as perform file conversions.
        /// </summary>
        public static void RunApp()
        {
            var saveForeColor = Console.ForegroundColor;
            bool running;

            do
            {
                PrintHeader(SourcePath, TargetPath);
                PrintFooter();

                var input = Console.ReadLine()!.ToLower();
                Console.ForegroundColor = saveForeColor;
                running = input?.Equals("x") == false;
                if (running)
                {
                    if (Int32.TryParse(input, out var select))
                    {
                        if (select == 1)
                        {
                            Console.WriteLine();
                            Console.Write("Enter source path: ");
                            var selectOrPath = Console.ReadLine();

                            if (Directory.Exists(selectOrPath))
                            {
                                SourcePath = selectOrPath;
                            }
                            else
                            {
                                Console.WriteLine("Invalid path!");
                            }
                        }
                        else if (select == 2)
                        {
                            Console.WriteLine();
                            Console.Write("Enter target path: ");
                            var selectOrPath = Console.ReadLine();

                            if (Directory.Exists(selectOrPath))
                            {
                                TargetPath = selectOrPath;
                            }
                            else
                            {
                                Console.WriteLine("Invalid path!");
                            }
                        }
                        else if (select == 3)
                        {
                            FormatterApp.RunApp();
                        }
                        else if (select == 4)
                        {
                            ConverterApp.RunApp();
                        }
                    }
                }
            } while (running);
        }
        /// <summary>
        /// Prints the header information for the document conversion.
        /// </summary>
        /// <param name="sourcePath">The path of the source document.</param>
        /// <param name="targetPath">The path of the target document.</param>
        /// <returns>The menu count.</returns>
        internal static void PrintHeader(string sourcePath, string targetPath)
        {
            var mnuIdx = 0;

            Console.Clear();
            Console.ForegroundColor = ProgressBar.ForegroundColor;
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Document-Conversion");
            Console.WriteLine("===================");
            Console.WriteLine();
            Console.WriteLine($"Source path:   {sourcePath}");
            Console.WriteLine($"Target path:   {targetPath}");
            Console.WriteLine($"Conversion to: {(TargetExtension.Equals(".md", StringComparison.CurrentCultureIgnoreCase) ? $"Markdown [{TargetExtension}]" : TargetExtension)}");
            Console.WriteLine();
            Console.WriteLine($"[{++mnuIdx,2}] Source path............Change path");
            Console.WriteLine($"[{++mnuIdx,2}] Target path............Change path");
            Console.WriteLine($"[{++mnuIdx,2}] Format documents.......Formatting documents");
            Console.WriteLine($"[{++mnuIdx,2}] Conversion documents...Conversion documents");
            Console.WriteLine();
        }

        /// <summary>
        /// Prints a footer message on the console.
        /// </summary>
        private static void PrintFooter()
        {
            Console.WriteLine();
            Console.Write("Choose: ");
        }

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
        #endregion App methods
    }
}