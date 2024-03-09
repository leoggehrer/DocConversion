
namespace DocConversion.ConApp
{
    /// <summary>
    /// Represents the application for converting documents to different formats.
    /// </summary>
    internal partial class ConverterApp : CommonTool.ConsoleApplication
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
            DocumentsPath = Path.Combine(UserPath, "Downloads");// Program.DocumentsPath;);
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
                    Key = "---",
                    Text = new string('-', 65),
                    Action = (self) => { },
                    ForegroundColor = ConsoleColor.DarkGreen,
               },
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

            var files = Program.GetFiles(DocumentsPath, "*.*", [".pdf", ".doc", ".docx"]).ToArray();

            menuItems.AddRange(CreatePageMenuItems(ref mnuIdx, files, (item, menuItem) =>
            {
                menuItem.Text = ToLabelText("Convert", $"{item.Replace(DocumentsPath, string.Empty)}");
                menuItem.Action = (self) =>
                {
                    var file = self.Params["file"]?.ToString() ?? string.Empty;

                    ConvertDocument(file, TargetPath);
                };
                menuItem.Params = new() { { "file", item } };
            }));
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

            count = PrintLine("Document-Converter");
            PrintLine('=', count);
            PrintLine();
            ForegroundColor = saveForeColor;
            PrintLine($"Force flag:    {Force}");
            PrintLine($"Document path: {DocumentsPath}");
            PrintLine($"Target path:   {TargetPath}");
            PrintLine();
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
        #endregion Methods
    }
}
