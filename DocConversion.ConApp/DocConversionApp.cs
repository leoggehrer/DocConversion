﻿namespace DocConversion.ConApp
{
    public partial class DocConversionApp : CommonTool.ConsoleApplication
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
                CreateMenuSeparator(),
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
                    Action = (self) => { new FormatterApp().Run([]); },
                },
                new()
                {
                    Key = $"{++mnuIdx}",
                    Text = ToLabelText("ReadMe", "Creates ReadMe.md documents"),
                    Action = (self) => { new ReadMeCreatorApp().Run([]); },
                },
            };

            return [.. menuItems.Union(CreateExitMenuItems())];
        }

        /// <summary>
        /// Prints the header for the PlantUML application.
        /// </summary>
        /// <param name="sourcePath">The path of the solution.</param>
        protected override void PrintHeader()
        {
            base.PrintHeader(nameof(DocConversionApp));
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

