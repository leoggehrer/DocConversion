
namespace DocConversion.ConApp
{
    /// <summary>
    /// Represents the application for converting documents to different formats.
    /// </summary>
    internal partial class ConverterApp : ConsoleApplication
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatterApp"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor is called when the <see cref="FormatterApp"/> class is first accessed.
        /// </remarks>
        static ConverterApp()
        {
            ClassConstructing();
            DocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            TargetPath = Path.Combine(DocumentsPath, "Convert");// Program.TargetPath;
            ClassConstructed();
        }
        /// <summary>
        /// Represents the method that is called before a class is being constructed.
        /// </summary>
        /// <remarks>
        /// This method can be implemented to perform any initialization tasks before the class is fully constructed.
        /// </remarks>
        static partial void ClassConstructing();
        /// <summary>
        /// This method is called when the class is constructed.
        /// </summary>
        /// <remarks>
        /// This method is called as part of the class construction process.
        /// It can be used to perform some initialization tasks before the class is fully constructed.
        /// </remarks>
        static partial void ClassConstructed();
        #endregion Class-Constructors

        #region Instance-Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DocConversionApp"/> class.
        /// </summary>
        public ConverterApp()
        {
            Constructing();
            Constructed();
        }
        /// <summary>
        /// This method is called during the construction of the object.
        /// </summary>
        partial void Constructing();
        /// <summary>
        /// This method is called when the object is constructed.
        /// </summary>
        partial void Constructed();
        #endregion Instance-Constructors

        #region Properties
        /// <summary>
        /// Gets or sets the document path.
        /// </summary>
        private static string DocumentsPath { get; set; }
        /// <summary>
        /// Gets or sets the target path.
        /// </summary>
        private static string TargetPath { get; set; }
        /// <summary>
        /// Gets or sets the current page index.
        /// </summary>
        private int PageIndex { get; set; } = 0;
        /// <summary>
        /// Gets or sets the page size for pagination.
        /// </summary>
        private int PageSize { get; set; } = 10;

        #endregion Properties

        #region overrides
        /// <summary>
        /// Creates an array of menu items for the application menu.
        /// </summary>
        /// <returns>An array of MenuItem objects representing the menu items.</returns>
        protected override MenuItem[] CreateMenuItems()
        {
            var mnuIdx = 0;
            var menuItems = new List<MenuItem>
            {
                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Force", "Change force flag"),
                    Action = (self) => ChangeForce(),
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Document path", "Change document path"),
                    Action = (self) => DocumentsPath =  ChangePath("New document path: ", DocumentsPath),
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Target path", "Change target path"),
                    Action = (self) => TargetPath =  ChangePath("New target path: ", TargetPath),
                },
                new()
                {
                    Key = "---",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                },
            };

            var files = GetFiles(DocumentsPath, "*.*", [".pdf", ".doc", ".docx"]).ToArray();

            if (files.Length > 0)
            {
                for (int i = PageIndex * PageSize; i < files.Length && i < (PageIndex + 1) * PageSize; i++)
                {
                    var file = files[i];
                    var text = file;

                    menuItems.Add(new MenuItem
                    {
                        Key = (++mnuIdx).ToString(),
                        OptionalKey = "a", // it's for choose option all
                        Text = ToLabelText("Convert", $"{file.Replace(DocumentsPath, string.Empty)}"),
                        Action = (self) =>
                        {
                            var file = self.Params["file"]?.ToString() ?? string.Empty;
                            
                            ConvertDocument(file, TargetPath);
                        },
                        Params = new() { { "file", file } },
                    });
                }

                var pageLabel = $"{PageIndex * PageSize}..{Math.Min((PageIndex + 1) * PageSize, files.Length)}/{files.Length}";

                menuItems.Add(new()
                {
                    Key = "---",
                    Text = ToLabelText(pageLabel, string.Empty, 20, ' '),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                });
                menuItems.Add(new()
                {
                    Key = "+",
                    Text = ToLabelText("Next", "Load next path page"),
                    Action = (self) =>
                    {
                        PageIndex = (PageIndex + 1) * PageSize <= files.Length ? PageIndex + 1 : PageIndex;
                        PrintScreen();
                    },
                    ForegroundColor = ConsoleColor.DarkGreen,
                });

                menuItems.Add(new()
                {
                    Key = "-",
                    Text = ToLabelText("Previous", "Load previous path page"),
                    Action = (self) =>
                    {
                        PageIndex = Math.Max(0, PageIndex - 1);
                        PrintScreen();
                    },
                    ForegroundColor = ConsoleColor.DarkGreen,
                });
            }
            return [.. menuItems.Union(CreateExitMenuItems())];
        }

        /// <summary>
        /// Prints the header for the PlantUML application.
        /// </summary>
        /// <param name="sourcePath">The path of the solution.</param>
        protected override void PrintHeader()
        {
            var count = 0;
            var saveForeColor = ForegroundColor;

            ForegroundColor = ConsoleColor.Green;

            count = PrintLine("Document-Converter");
            PrintLine('=', count);
            PrintLine();
            ForegroundColor = saveForeColor;
            PrintLine($"Force flag:    {Force}");
            PrintLine($"Document path: {DocumentsPath}");
            PrintLine($"Target path:   {TargetPath}");
            PrintLine();
        }
        /// <summary>
        /// Prints the footer of the application.
        /// </summary>
        protected override void PrintFooter()
        {
            PrintLine();
            Print("Choose [n|n,n|a...all|x|X]: ");
        }
        #endregion overrides


        #region Methods
        /// <summary>
        /// Converts a document to a different format based on its file extension.
        /// </summary>
        /// <param name="file">The path of the document file to be converted.</param>
        /// <param name="targetPath">The path where the converted document will be saved.</param>
        private static void ConvertDocument(string file, string targetPath)
        {
            var extension = Path.GetExtension(file);

            switch (extension)
            {
                case ".pdf":
                case ".doc":
                case ".docx":
                    Logic.MarkdownConverter.ConversionTo(file, targetPath, "ReadMe.md");
                    break;
                default:
                    Console.WriteLine($"The file extension '{extension}' is not supported.");
                    break;
            }
        }
        /// <summary>
        /// Retrieves a collection of file paths that match the specified search pattern and extensions.
        /// </summary>
        /// <param name="path">The directory to search in.</param>
        /// <param name="searchPattern">The search pattern to match against the names of files in the directory.</param>
        /// <param name="extensions">The file extensions to filter the search results. If no extensions are provided, all files will be included.</param>
        /// <returns>A collection of file paths that match the search pattern and extensions.</returns>
        public static IEnumerable<string> GetFiles(string path, string searchPattern, params string[] extensions)
        {
            var result = Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories)
                                  .Where(f => extensions.Length != 0 == false
                                           || extensions.Any(e => Path.GetExtension(f).Equals(e, StringComparison.CurrentCultureIgnoreCase)))
                                  .OrderBy(i => i)
                                  .ToArray();

            return result;
        }
        #endregion Methods
    }
}
