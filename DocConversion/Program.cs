using Aspose.Words;
using System.Text;
using System.Text.RegularExpressions;

namespace DocConversion
{
    internal partial class Program
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
            TargetPath = "/Users/ggehrer/source/repos/leoggehrer/34_ABIF_ACIF_POSE_EXERCISES";// UserPath;
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
        internal static List<string> SourceFiles { get; } = new();
        internal static string TargetExetension => ".md";

        internal static bool CanBusyPrint { get; set; } = true;
        internal static bool RunBusyProgress { get; set; }
        internal static ConsoleColor ForegroundColor { get; set; } = Console.ForegroundColor;
        #endregion Properties

        #region App methods
        public static void RunApp()
        {
            var running = false;
            var saveForeColor = Console.ForegroundColor;

            do
            {
                var offset = 2;
                var input = string.Empty;
                var targetPaths = new List<string>();

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                PrintBusyProgress();

                RunBusyProgress = false;
                Task.Delay(250).Wait();
                PrintHeader(SourcePath, TargetPath);
                Console.WriteLine($"[x|X...Quit]: ");
                Console.WriteLine();
                Console.Write("Choose [n|n,n|x|X]: ");

                input = Console.ReadLine()?.ToLower();
                Console.ForegroundColor = saveForeColor;
                running = input?.Equals("x") == false;
                if (running)
                {
                    if (Int32.TryParse(input, out var select))
                    {
                        if (select == 0)
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
                        if (select == 1)
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
                        if (select > 1 && select - offset < SourceFiles.Count)
                        {
                            var sourceFile = SourceFiles[select - offset];

                            PrintBusyProgress();
                            ConversionTo(sourceFile, TargetPath, TargetExetension);
                        }
                    }
                    else
                    {
                        var numbers = input?.Trim()
                                            .Split(',').Where(s => Int32.TryParse(s, out int n))
                                            .Select(s => Int32.Parse(s))
                                            .Distinct()
                                            .ToArray();

                        PrintBusyProgress();
                        foreach (var number in numbers!)
                        {
                            if (number > 0 && number - offset < SourceFiles.Count)
                            {
                                var sourceFile = SourceFiles[number - offset];

                                ConversionTo(sourceFile, TargetPath, TargetExetension);
                            }
                        }
                    }
                    PrintHeader(SourcePath, TargetPath);
                }
            } while (running);
        }
        private static void PrintHeader(string sourcePath, string targetPath)
        {
            var mnuIdx = 0;

            Console.Clear();
            Console.ForegroundColor = Program.ForegroundColor;
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Document-Conversion");
            Console.WriteLine("===================");
            Console.WriteLine();
            Console.WriteLine($"Source path:   {sourcePath}");
            Console.WriteLine($"Target path:   {targetPath}");
            Console.WriteLine($"Conversion to: {(TargetExetension.Equals(".md", StringComparison.CurrentCultureIgnoreCase) ? $"Markdown [{TargetExetension}]" : TargetExetension)}");
            Console.WriteLine();
            Console.WriteLine($"[{mnuIdx++, 2}] Source path............Change path");
            Console.WriteLine($"[{mnuIdx++, 2}] Target path............Change path");
            Console.WriteLine();
            SourceFiles.Clear();
            foreach (var document in GetDocuments(sourcePath))
            {
                SourceFiles.Add(document);
                Console.WriteLine($"[{mnuIdx++, 2}] {Path.GetFileName(document)}");
            }
            Console.WriteLine();
        }
        private static void ConversionTo(string sourceFile, string targetPath, string targetExtension)
        {
            var sourceExtension = Path.GetExtension(sourceFile);
            var targetFileName = $"{Path.GetFileNameWithoutExtension(sourceFile)}{targetExtension}";

            targetPath = Path.Combine(targetPath!, Path.GetFileNameWithoutExtension(targetFileName));
            if (Path.Exists(targetPath))
            {
                Directory.Delete(targetPath, true);
            }
            if (string.IsNullOrWhiteSpace(targetPath) == false)
            {
                if (sourceExtension.Equals(".md", StringComparison.CurrentCultureIgnoreCase))
                {
                    CleanupMarkdown(sourceFile);
                }
                else
                {
                    var document = new Document(sourceFile);
                    var targetFile = Path.Combine(targetPath, targetFileName);

                    Directory.CreateDirectory(targetPath);

                    try
                    {
                        document.Save(targetFile, SaveFormat.Markdown);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
//                    document.Save(targetFile);

                    CleanupMarkdown(targetFile);
                }
            }
        }
        private static void CleanupMarkdown(string filePath)
        {
            var targetPath = Path.GetDirectoryName(filePath);

            if (File.Exists(filePath))
            {
                var ignoreLine = false;
                var prevLine = default(string);
                var newLines = new List<string>();
                var oldLines = File.ReadAllLines(filePath)
                                   .SkipWhile(l => string.IsNullOrWhiteSpace(l));

                foreach (var line in oldLines)
                {
                    if (line.StartsWith("**") && line.EndsWith("**"))
                    {
                        ignoreLine = true;
                    }
                    else if (ignoreLine && string.IsNullOrWhiteSpace(line))
                    {
                        ignoreLine = true;
                    }
                    else
                    {
                        ignoreLine = false;
                    }
                    if (ignoreLine == false)
                    {
                        if (prevLine == default)
                        {
                            prevLine = line;
                        }
                        else if (line.StartsWith("#"))
                        {
                            if (string.IsNullOrWhiteSpace(prevLine) == false)
                            {
                                newLines.Add(string.Empty);
                            }
                            newLines.Add(line.Replace("\t", "  ").TrimEnd());
                        }
                        else if (string.IsNullOrWhiteSpace(line) == false 
                                 || string.IsNullOrWhiteSpace(prevLine) == false)
                        {
                            if (prevLine.StartsWith("#") && string.IsNullOrWhiteSpace(line) == false)
                            {
                                newLines.Add(string.Empty);
                            }
                            newLines.Add(line.Replace("\t", "  ").TrimEnd());
                        }
                        prevLine = line;
                    }
                }

                var text = newLines.Aggregate(new StringBuilder(), (sb, l) => sb.AppendLine(l))
                                   .ToString();
                string pattern = @"!\[[^\]]*\]";
                string replacement = "![Illustration]";
                string result = Regex.Replace(text, pattern, replacement);

                File.WriteAllText(filePath, result);
            }
        }
        private static IEnumerable<string> GetDocuments(string path, string searchPattern)
        {
            var result = new List<string>();
            var files = Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly)
                                 .Where(f => StaticLiterals.DocumentFileExtensions.Any(e => Path.GetExtension(f).Equals(e, StringComparison.CurrentCultureIgnoreCase)))
                                 .OrderBy(i => i);

            result.AddRange(files);
            return result;
        }
        private static IEnumerable<string> GetDocuments(string path)
        {
            return GetDocuments(path, "*.*");
        }
        #endregion App methods


        internal static void PrintBusyProgress()
        {
            if (RunBusyProgress == false)
            {
                var sign = "\\";

                Console.WriteLine();
                RunBusyProgress = true;
                Task.Factory.StartNew(async () =>
                {
                    while (RunBusyProgress)
                    {
                        if (CanBusyPrint)
                        {
                            if (Console.CursorLeft > 0)
                                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);

                            Console.Write($".{sign}");
                            sign = sign == "\\" ? "/" : "\\";
                        }
                        await Task.Delay(250).ConfigureAwait(false);
                    }
                });
            }
        }

        static void Main(string[] args)
        {
            if (args.Any() && Directory.Exists(args[0]))
            {
                SourcePath = args[0];
                TargetPath = args[0];
            }
            RunApp();
        }
    }
}