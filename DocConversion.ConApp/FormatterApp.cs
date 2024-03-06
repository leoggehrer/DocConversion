
namespace DocConversion.ConApp
{
    /// <summary>
    /// Represents an application for formatting documents.
    /// </summary>
    internal partial class FormatterApp : ConsoleApplication
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatterApp"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor is called when the <see cref="FormatterApp"/> class is first accessed.
        /// </remarks>
        static FormatterApp()
        {
            ClassConstructing();
            DocumentsPath = Path.Combine(UserPath, "Downloads");// Program.DocumentsPath;);
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
        /// Initializes a new instance of the <see cref="FormatterApp"/> class.
        /// </summary>
        public FormatterApp()
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
        /// Gets or sets the documents path.
        /// </summary>
        private static string DocumentsPath { get; set; }
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
                    Key = "---",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                },
            };

            var files = Program.GetFiles(DocumentsPath, "*.*", [".md"]).ToArray();

            if (files.Length > 0)
            {
                for (int i = PageIndex * PageSize; i < files.Length && i < (PageIndex + 1) * PageSize; i++)
                {
                    var file = files[i];
                    var text = file;

                    menuItems.Add(new()
                    {
                        Key = (++mnuIdx).ToString(),
                        OptionalKey = "a", // it's for choose option all
                        Text = ToLabelText("Convert", $"{file.Replace(DocumentsPath, string.Empty)}"),
                        Action = (self) =>
                        {
                            var file = self.Params["file"]?.ToString() ?? string.Empty;
                            
                            FormatDocument(file);
                        },
                        Params = new() { { "file", file } },
                    });
                }

                var pageLabel = $"{PageIndex * PageSize}..{Math.Min((PageIndex + 1) * PageSize, files.Length)}/{files.Length}";

                menuItems.Add(new()
                {
                    Key = "---",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                });
                menuItems.Add(new()
                {
                    Key = "",
                    Text = ToLabelText(pageLabel, string.Empty, 20, ' '),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
                });
                menuItems.Add(new()
                {
                    Key = "---",
                    Text = new string('-', 65),
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
        protected override void PrintHeader()
        {
            var count = 0;
            var saveForeColor = ForegroundColor;

            ForegroundColor = ConsoleColor.Green;

            count = PrintLine("Document-Formatter");
            PrintLine('=', count);
            PrintLine();
            ForegroundColor = saveForeColor;
            PrintLine($"Force flag:    {Force}");
            PrintLine($"Document path: {DocumentsPath}");
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
        /// Formats the specified document based on its file extension.
        /// </summary>
        /// <param name="file">The path of the document file.</param>
        private static void FormatDocument(string file)
        {
            var extension = Path.GetExtension(file);

            switch (extension)
            {
                case ".md":
                    Logic.MarkdownConverter.CleaningAndFormatting(file);
                    break;
                default:
                    Console.WriteLine($"The file extension '{extension}' is not supported.");
                    break;
            }
        }
        #endregion Methods
    }
}
