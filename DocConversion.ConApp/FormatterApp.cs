
using System.Runtime.CompilerServices;

namespace DocConversion.ConApp
{
    /// <summary>
    /// Represents an application for formatting documents.
    /// </summary>
    internal partial class FormatterApp : CommonTool.ConsoleApplication
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
                CreateMenuSeparator(),
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
                    Action = (self) => DocumentsPath =  SelectOrChangeToSubPath(DocumentsPath, SourcePath),
                },
                CreateMenuSeparator(),
            };

            var files = Program.GetFiles(DocumentsPath, "*.*", [".md"]).ToArray();

            menuItems.AddRange(CreatePageMenuItems(ref mnuIdx, files, (item, menuItem) => {
                menuItem.Text = ToLabelText("Convert", $"{item.Replace(DocumentsPath, string.Empty)}");
                menuItem.Action = (self) =>
                {
                    var file = self.Params["file"]?.ToString() ?? string.Empty;
                    
                    FormatDocument(file);
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

            count = PrintLine("Document-Formatter");
            PrintLine('=', count);
            PrintLine();
            ForegroundColor = saveForeColor;
            PrintLine($"Force flag:    {Force}");
            PrintLine($"Document path: {DocumentsPath}");
            PrintLine();
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
