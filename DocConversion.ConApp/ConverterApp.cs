
namespace DocConversion.ConApp
{
    /// <summary>
    /// Represents the application for converting documents to different formats.
    /// </summary>
    internal partial class ConverterApp
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
            DocumentPath = Program.SourcePath;
            TargetPath = Program.TargetPath;
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

        #region Properties
        /// <summary>
        /// Gets or sets the document path.
        /// </summary>
        private static string DocumentPath { get; set; }
        /// <summary>
        /// Gets or sets the target path.
        /// </summary>
        private static string TargetPath { get; set; }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Prints the screen with the menu items and returns an array of MenuItem objects.
        /// </summary>
        /// <returns>An array of MenuItem objects.</returns>
        private static Models.MenuItem[] PrintScreen()
        {
            var saveForeColor = Console.ForegroundColor;
            var menuItems = CreateMenuItems(DocumentPath, StaticLiterals.DocumentFileExtensions);

            Console.Clear();
            Console.ForegroundColor = ProgressBar.ForegroundColor;
            PrintHeader(DocumentPath);
            menuItems.ToList().ForEach(m => Console.WriteLine($"[{m.Key,2}] {m.Text}"));
            PrintFooter();
            Console.ForegroundColor = saveForeColor;
            return menuItems;
        }
        /// <summary>
        /// Runs the application and prompts the user to select various options related to code generation.
        /// </summary>
        public static void RunApp()
        {
            var running = true;
            var input = default(string?);
            var saveForeColor = Console.ForegroundColor;

            do
            {
                var menuItems = PrintScreen();

                input = Console.ReadLine()?.ToLower() ?? String.Empty;
                foreach (var item in input.Split(','))
                {
                    menuItems.FirstOrDefault(m => m.Key.Equals(item))?.Action();
                    running = item.Equals("x") ? false : running;
                }
                ProgressBar.Stop();
            } while (running);
        }

        /// <summary>
        /// Prints the header for the Template Code Formatter program.
        /// </summary>
        /// <param name="path">The source path of the code to be formatted.</param>
        private static void PrintHeader(string path)
        {
            Console.Clear();
            Console.ForegroundColor = ProgressBar.ForegroundColor;
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Document-Conversion-Converter");
            Console.WriteLine("=============================");
            Console.WriteLine();
            Console.WriteLine($"Document path: {path}");
            Console.WriteLine($"Target path:   {path}");
            Console.WriteLine();
        }
        /// <summary>
        /// Prints a footer message on the console.
        /// </summary>
        private static void PrintFooter()
        {
            Console.WriteLine();
            Console.Write("Choose [n|n,n|x|X]: ");
        }
        /// <summary>
        /// Creates an array of menu items based on the specified path and extensions.
        /// </summary>
        /// <param name="path">The path to search for files.</param>
        /// <param name="extensions">The file extensions to include.</param>
        /// <returns>An array of menu items.</returns>
        private static Models.MenuItem[] CreateMenuItems(string path, string[] extensions)
        {
            var mnuIdx = 0;
            var files = Program.GetFiles(path, "*.*", extensions);
            var result = new List<Models.MenuItem>()
            {
                new ()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = "Change document path",
                    Action = () => DocumentPath = ChangeDocumentPath(DocumentPath),
                },
                new ()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = "Change target path",
                    Action = () => TargetPath = ChangeTargetPath(TargetPath),
                },
            };

            foreach (var file in files)
            {
                result.Add(new Models.MenuItem()
                {
                    Key = (++mnuIdx).ToString(),
                    Text = $"Convert {file.Replace(path, "...")}",
                    Action = () => ConvertDocument(file, TargetPath),
                });
            }

            result.Add(new Models.MenuItem()
            {
                Key = (++mnuIdx).ToString(),
                Text = "x ... Exit",
                Action = () => Console.WriteLine("Exiting..."),
            });
            return [.. result];
        }

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
        /// Changes the document path based on user input.
        /// </summary>
        /// <param name="path">The original document path.</param>
        /// <returns>The updated document path.</returns>
        private static string ChangeDocumentPath(string path)
        {
            var result = path;

            Console.WriteLine();
            Console.Write("Enter the path of the documents to be formatted: ");
            var input = Console.ReadLine()!;
            if (Directory.Exists(input))
            {
                result = input;
            }
            else
            {
                Console.WriteLine($"The path '{input}' does not exist.");
            }
            return result;
        }
        /// <summary>
        /// Changes the target path for saving the conversion.
        /// </summary>
        /// <param name="path">The original path.</param>
        /// <returns>The updated target path.</returns>
        private static string ChangeTargetPath(string path)
        {
            var result = path;

            Console.WriteLine();
            Console.Write("Enter the target path for saving the conversion: ");
            var input = Console.ReadLine()!;
            if (Directory.Exists(input))
            {
                result = input;
            }
            else
            {
                Console.WriteLine($"The path '{input}' does not exist.");
            }
            return result;
        }
        #endregion Methods
    }
}
