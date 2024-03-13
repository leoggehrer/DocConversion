using System.Reflection;
using CommonTool.Extensions;

namespace DocConversion.ConApp
{
    public partial class ReadMeCreatorApp : CommonTool.ConsoleApplication
    {
        #region Class-Constructors
        /// <summary>
        /// Initializes the <see cref="Program"/> class.
        /// This static constructor sets up the necessary properties for the program.
        /// </remarks>
        static ReadMeCreatorApp()
        {
            ClassConstructing();
            DocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
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
        /// Initializes a new instance of the <see cref="ReadMeCreatorApp"/> class.
        /// </summary>
        public ReadMeCreatorApp()
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

        #region app properties
        /// <summary>
        /// Gets or sets the path to the documents.
        /// </summary>
        private static string DocumentsPath { get; set; }
        #endregion app properties

        #region overrides
        /// <summary>
        /// Prints the header for the PlantUML application.
        /// </summary>
        protected override void PrintHeader()
        {
            var saveForeColor = ForegroundColor;

            ForegroundColor = ConsoleColor.Green;

            var count = PrintLine("ReadMe-Creator");
            PrintLine('=', count);
            PrintLine();
            ForegroundColor = saveForeColor;
            PrintLine($"Force flag:    {Force}");
            PrintLine($"Document path: {DocumentsPath}");
            PrintLine();
        }

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
                    Text = ToLabelText("Path", "Change source path"),
                    Action = (self) => DocumentsPath = SelectOrChangeToSubPath(DocumentsPath, [ SourcePath ]),
                },
                CreateMenuSeparator(),
            };
            var files = Program.GetFiles(DocumentsPath, "rm_creator.md", [".md"]).ToArray();

            menuItems.AddRange(CreatePageMenuItems(ref mnuIdx, files, (item, menuItem) =>
            {
                menuItem.OptionalKey = "a";
                menuItem.Text = ToLabelText("Convert", $"{item.Replace(DocumentsPath, string.Empty)}");
                menuItem.Action = (self) =>
                {
                    var file = self.Params["file"]?.ToString() ?? string.Empty;

                    CreateReadMe(file, Force);
                };
                menuItem.Params = new() { { "file", item } };
            }));
            return [.. menuItems.Union(CreateExitMenuItems())];
        }
        #endregion overrides

        #region app-methods
        /// <summary>
        /// Creates a ReadMe file based on the provided file path, with optional force overwrite.
        /// </summary>
        /// <param name="filePath">The path of the file to be used as a template for the ReadMe file.</param>
        /// <param name="force">A boolean value indicating whether to force overwrite an existing ReadMe file.</param>
        private void CreateReadMe(string filePath, bool force)
        {
            var path = Path.GetDirectoryName(filePath)!;
            var readMeFilePath = Path.Combine(path!, "ReadMe.md");
            var lines = File.ReadAllLines(filePath);
            var result = new List<string>();

            foreach (var line in lines)
            {
                if (line.StartsWith("[insert_file]", StringComparison.CurrentCultureIgnoreCase))
                {
                    var includeFilePath = ConvertFilePath(line.Betweenstring("(", ")"));
                    var includeFileLevel = line.Betweenstring(")(", ")");
                    var includePath = Path.GetDirectoryName(includeFilePath);

                    if (includePath.IsNullOrEmpty())
                    {
                        includeFilePath = Path.Combine(path, includeFilePath);
                    }

                    int.TryParse(includeFileLevel, out int level);

                    result.AddRange(IncludeReadMe(path, includeFilePath, level));
                }
                else if (line.StartsWith("[insert_acinfo]", StringComparison.CurrentCultureIgnoreCase))
                {
                    var includeUrl = ConvertFilePath(line.Betweenstring("(", ")"));
                    var includeFilePath = line.Betweenstring(")(", ")");

                    result.AddRange(IncludeActivityDiagrams(path, includeFilePath, includeUrl, 3));
                }
                else
                {
                    result.Add(line);
                }
            }

            if (File.Exists(readMeFilePath) == false || force)
            {
                File.WriteAllLines(readMeFilePath, result);
            }
        }
        /// <summary>
        /// Includes the content of a README file in a specified path, while processing images and adjusting heading levels.
        /// </summary>
        /// <param name="path">The destination path where the README file content will be included.</param>
        /// <param name="filePath">The path of the README file to be included.</param>
        /// <param name="level">The level of heading adjustment to be applied to the README file content.</param>
        /// <returns>A list of strings representing the modified content of the README file.</returns>
        private static List<string> IncludeReadMe(string path, string filePath, int level)
        {
            var result = new List<string>();
            var sourcePath = Path.GetDirectoryName(filePath)!;
            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                if (line.Trim().StartsWith("!["))
                {
                    var imageFilePath = ConvertFilePath(line.Betweenstring("(", ")"));
                    var sourceFilePath = Path.Combine(sourcePath, imageFilePath);
                    var destinationFilePath = Path.Combine(path, imageFilePath);

                    try
                    {
                        File.Copy(sourceFilePath, destinationFilePath, true);
                    }
                    catch (System.Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error in {MethodBase.GetCurrentMethod()!.Name}: {ex.Message}");   // ignore
                    }
                    result.Add(line);
                }
                else if (line.Trim().StartsWith("#"))
                {
                    result.Add(new string('#', level) + line);
                }
                else
                {
                    result.Add(line);
                }
            }
            return result;
        }
        /// <summary>
        /// Includes activity diagrams in the result list based on the provided file path, sub-file path, URL, and level.
        /// </summary>
        /// <param name="path">The base path where the files are located.</param>
        /// <param name="subFilePath">The sub-file path relative to the base path.</param>
        /// <param name="url">The URL where the diagrams are hosted.</param>
        /// <param name="level">The level of the heading for each diagram.</param>
        /// <returns>A list of strings containing the included activity diagrams.</returns>
        private static List<string> IncludeActivityDiagrams(string path, string subFilePath, string url, int level)
        {
            var first = false;
            var title = string.Empty;
            var result = new List<string>();
            var filePath = Path.Combine(path, subFilePath);
            var lines = File.Exists(filePath) ? File.ReadAllLines(filePath) : [];

            foreach (var line in lines)
            {
                var data = line.Split(':');

                if (data[0] == "title")
                    title = data[1];
                else if (data[0] == "fileName")
                {
                    if (first)
                    {
                        result.Add(string.Empty);
                    }

                    result.Add($"{new string('#', level)} {title}");
                    result.Add(string.Empty);
                    result.Add($"![{title}]({url}/{data[1]})");
                    first = true;
                }
            }
            return result;
        }
        /// <summary>
        /// Converts the file path to use the appropriate directory separator character.
        /// </summary>
        /// <param name="filePath">The file path to convert.</param>
        /// <returns>The converted file path.</returns>
        private static string ConvertFilePath(string filePath) => filePath.Replace('/', Path.DirectorySeparatorChar);
        #endregion app-methods
    }
}