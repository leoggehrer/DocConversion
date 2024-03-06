namespace DocConversion.ConApp
{
    public partial class DocConversionApp : ConsoleApplication
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes the <see cref="Program"/> class.
        /// This static constructor sets up the necessary properties for the program.
        /// </remarks>
        static DocConversionApp()
        {
            ClassConstructing();
            ClassConstructed();
        }
        /// <summary>
        /// This method is called during the construction of the class.
        /// </summary>
        static partial void ClassConstructing();
        /// <summary>
        /// Represents a method that is called when a class is constructed.
        /// </summary>
        static partial void ClassConstructed();
        #endregion Class-Constructors

        #region Instance-Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DocConversionApp"/> class.
        /// </summary>
        public DocConversionApp()
        {
            Constructing();
            DocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
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

        #region app properties
        private string DocumentsPath { get; set; }
        /// <summary>
        /// Gets or sets the current page index.
        /// </summary>
        private int PageIndex { get; set; } = 0;
        /// <summary>
        /// Gets or sets the page size for pagination.
        /// </summary>
        private int PageSize { get; set; } = 10;
        #endregion app properties

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
                    Text = ToLabelText("Path", "Change source path"),
                    Action = (self) => ChangeSourcePath(),
                },
                new()
                {
                    Key = "---",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Convert", "Convert documents to markdown"),
                    Action = (self) => { new ConverterApp().Run([]); },
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Format", "Format markdown documents"),
                    Action = (self) => { },
                },
                new()
                {
                    Key = "---",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                },
            };

            var files = GetSourceCodeFiles(DocumentsPath, ["*.cs"]).ToArray();

            for (int i = PageIndex * PageSize; i < files.Length && i < (PageIndex + 1) * PageSize; i++)
            {
                var file = files[i];
                var text = file;

                menuItems.Add(new MenuItem
                {
                    Key = (++mnuIdx).ToString(),
                    OptionalKey = "a", // it's for choose option all
                    Text = ToLabelText("File", $"{file.Replace(SourcePath, string.Empty)}"),
                    Action = (self) =>
                    {
                        var path = self.Params["file"]?.ToString() ?? string.Empty;

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

            count = PrintLine(nameof(DocConversionApp));
            PrintLine('=', count);
            PrintLine();
            ForegroundColor = saveForeColor;
            PrintLine($"Force flag:     {Force}");
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

