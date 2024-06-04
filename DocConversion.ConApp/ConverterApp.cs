
using CommonTool.Extensions;

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
            ConversionFileName = "Task.md";
            ConversionPath = SourcePath;
            DocumentsPath = Path.Combine(UserPath, "Downloads");
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
            MaxSubPathDepth = 1;
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
        public static string ConversionFileName { get; set; }
        /// <summary>
        /// Gets or sets the document path.
        /// </summary>
        private static string DocumentsPath { get; set; }
        /// <summary>
        /// Gets or sets the target path.
        /// </summary>
        private static string ConversionPath { get; set; }
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
                    Text = ToLabelText($"{Force}", "Change force flag"),
                    Action = (self) => ChangeForce(),
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText($"{MaxSubPathDepth}", "Change max sub path depth"),
                    Action = (self) => ChangeMaxSubPathDepth(),
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Document path", "Change document path"),
                    Action = (self) => 
                    {
                        var savePath = DocumentsPath;
                        
                        DocumentsPath = SelectOrChangeToSubPath(DocumentsPath, MaxSubPathDepth, [ SourcePath ]);
                        if (savePath != DocumentsPath)
                        {
                            PageIndex = 0;
                        }
                    },
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Conversion path", "Change conversion path"),
                    Action = (self) => ConversionPath =  SelectOrChangeToSubPath(ConversionPath, MaxSubPathDepth, SourcePath),
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("Change file name", "Change conversion file name"),
                    Action = (self) => ConversionFileName = ChangeConversionFileName(),
                },
                CreateMenuSeparator(),
            };

            if (mnuIdx % 10 != 0)
            {
                mnuIdx += 10 - (mnuIdx % 10);
            }

            var files = Program.GetFiles(DocumentsPath, "*.*", [".pdf", ".doc", ".docx"]).ToArray();

            menuItems.AddRange(CreatePageMenuItems(ref mnuIdx, files, (item, menuItem) =>
            {
                menuItem.Text = ToLabelText("Convert", $"{item.Replace(DocumentsPath, string.Empty)}");
                menuItem.Action = (self) =>
                {
                    var file = self.Params["file"]?.ToString() ?? string.Empty;

                    ConvertDocument(file, ConversionPath, ConversionFileName);
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
            base.PrintHeader(nameof(DocConversionApp),
            [
                new("Document path:", DocumentsPath),
                new("Conversion path:", ConversionPath),
                new("Conversion file name:", ConversionFileName),
            ]);
        }
        #endregion overrides

        #region Methods
        /// <summary>
        /// Changes the conversion file name based on user input.
        /// </summary>
        /// <returns>The new conversion file name.</returns>
        private static string ChangeConversionFileName()
        {
            PrintLine();
            Print("Type new conversion file name: ");

            var result = ReadLine();

            return result.HasContent() ? result : ConversionFileName;
        }
        /// <summary>
        /// Converts a document to a specified format.
        /// </summary>
        /// <param name="file">The path of the document file to convert.</param>
        /// <param name="targetPath">The target path where the converted document will be saved.</param>
        /// <param name="conversionFileName">The name of the converted document file.</param>
        private static void ConvertDocument(string file, string targetPath, string conversionFileName)
        {
            var extension = Path.GetExtension(file);

            switch (extension)
            {
                case ".pdf":
                case ".doc":
                case ".docx":
                    Logic.MarkdownConverter.ConversionTo(file, targetPath, conversionFileName);
                    break;
                default:
                    Console.WriteLine($"The file extension '{extension}' is not supported.");
                    break;
            }
        }
        #endregion Methods
    }
}
