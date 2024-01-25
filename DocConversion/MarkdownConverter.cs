﻿using Aspose.Words;
using System.Text;
using System.Text.RegularExpressions;

namespace DocConversion
{
    internal class MarkdownConverter
    {
        /// <summary>
        /// Converts a source file to a target format and saves it to the specified target path.
        /// </summary>
        /// <param name="sourceFile">The path of the source file to be converted.</param>
        /// <param name="targetPath">The path where the converted file will be saved.</param>
        /// <param name="targetExtension">The extension of the target file format.</param>
        public static void ConversionTo(string sourceFile, string targetPath, string targetExtension)
        {
            var sourceExtension = Path.GetExtension(sourceFile);
            var targetFileName = $"{Path.GetFileNameWithoutExtension(sourceFile)}{targetExtension}";

            targetPath = Path.Combine(targetPath!, Path.GetFileNameWithoutExtension(targetFileName));

            var targetFile = Path.Combine(targetPath, targetFileName);

            if (File.Exists(targetFile) && targetExtension.Equals(".md", StringComparison.CurrentCultureIgnoreCase))
            {
                CleaningAndFormatting(targetFile);
            }
            else if (File.Exists(sourceFile) && sourceExtension.Equals(".md", StringComparison.CurrentCultureIgnoreCase))
            {
                CleaningAndFormatting(sourceFile);
            }
            else if (File.Exists(sourceFile))
            {
                if (Path.Exists(targetPath))
                {
                    Directory.Delete(targetPath, true);
                }
                Directory.CreateDirectory(targetPath);

                try
                {
                    var document = new Document(sourceFile);

                    document.Save(targetFile, SaveFormat.Markdown);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                CleaningAndFormatting(targetFile);
            }
        }
        /// <summary>
        /// Cleans up a Markdown file by removing unnecessary lines and replacing image tags with a generic placeholder.
        /// </summary>
        /// <param name="filePath">The path to the Markdown file.</param>
        public static void CleaningAndFormatting(string filePath)
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
                    if (line.StartsWith("**") && line.EndsWith("**") && line.Contains("Aspose.Words", StringComparison.CurrentCultureIgnoreCase))
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
                        prevLine ??= line;
                        if (line.StartsWith('#'))
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
                            if (prevLine.StartsWith('#') && string.IsNullOrWhiteSpace(line) == false)
                            {
                                newLines.Add(string.Empty);
                            }
                            newLines.Add(line.Replace("\t", "  ").TrimEnd());
                        }
                        prevLine = line;
                    }
                }

                var formatedTableLines = FormatMarkdownTables(newLines);
                var text = formatedTableLines.Aggregate(new StringBuilder(), (sb, l) => sb.AppendLine(l)).ToString();
                string pattern = @"!\[[^\]]*\]";
                string replacement = "![Illustration]";
                string result = Regex.Replace(text, pattern, replacement);

                File.WriteAllText(filePath, result);
            }
        }

        /// <summary>
        /// Formats the markdown tables in the given collection of lines.
        /// </summary>
        /// <param name="lines">The collection of lines containing markdown tables.</param>
        /// <returns>A collection of formatted lines with markdown tables.</returns>
        public static IEnumerable<string> FormatMarkdownTables(IEnumerable<string> lines)
        {
            var result = new List<string>();
            var tableRows = new List<string>();

            foreach (var line in lines)
            {
                if (line.TrimStart(' ', '\t').StartsWith('|'))
                {
                    tableRows.Add(line);
                }
                else
                {
                    result.AddRange(FormatMarkdownTable(tableRows));
                    result.Add(line);
                    tableRows.Clear();
                }
            }
            if (tableRows.Count > 0)
            {
                result.AddRange(FormatMarkdownTable(tableRows));
            }
            return result;
        }
        /// <summary>
        /// Formats a markdown table by aligning the columns and adjusting the column widths.
        /// </summary>
        /// <param name="lines">The lines of the markdown table.</param>
        /// <returns>The formatted markdown table.</returns>
        public static IEnumerable<string> FormatMarkdownTable(IEnumerable<string> lines)
        {
            var result = new List<string>();
            var table = lines.Select(r => r.Split('|', StringSplitOptions.RemoveEmptyEntries)).ToArray();

            if (table.Length > 0)
            {
                var columnWidths = new int[table[0].Length];
                for (var i = 0; i < table[0].Length; i++)
                {
                    columnWidths[i] = table.Max(r => r[i].Length);
                }
                foreach (var row in table)
                {
                    var line = "|";

                    for (var i = 0; i < row.Length; i++)
                    {
                        line += $"{row[i].PadRight(columnWidths[i])}|";
                    }
                    result.Add(line);
                }
            }
            else
            {
                result.AddRange(lines);
            }
            return result;
        }
    }
}
