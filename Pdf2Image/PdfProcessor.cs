using System.Drawing.Imaging;
using Spire.Pdf;
using Spire.Pdf.Graphics;

namespace Pdf2Image
{
    /// <summary>
    ///     The pdf processor class (a. beging, 31.03.2022)
    /// </summary>
    public static class PdfProcessor
    {
        #region Public Method Process

        /// <summary>
        ///     Processes
        /// </summary>
        public static void Process()
        {
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.pdf");

            foreach (var pdfFile in files)
            {
                Console.WriteLine($"Processing {new FileInfo(pdfFile).Name}");

                var doc = new PdfDocument();
                doc.LoadFromFile(pdfFile);

                var jpegInfo = GetOutputPathTemplate(pdfFile, doc.Pages.Count > 1);
                if (string.IsNullOrWhiteSpace(jpegInfo)) continue;

                for (var i = 0; i < doc.Pages.Count; i++)
                {
                    var outputPath = string.Format(jpegInfo, i + 1);
                    if (File.Exists(outputPath)) continue;
                    var bmp = doc.SaveAsImage(i, PdfImageType.Bitmap, 600, 600);

                    Console.WriteLine($"Generating {outputPath} (600dpi)");

#pragma warning disable CA1416
                    bmp.Save(outputPath, ImageFormat.Jpeg);
#pragma warning restore CA1416
                }
            }
        }

        #endregion

        #region Private Method GetOutputPathTemplate

        /// <summary>
        ///     Gets the output path template using the specified pdf path (a. beging, 31.03.2022)
        /// </summary>
        /// <param name="pdfPath">The pdf path</param>
        /// <param name="multiPage">The multi page</param>
        /// <returns>The jpeg path</returns>
        private static string? GetOutputPathTemplate(string pdfPath, bool multiPage)
        {
            var fileInfo = new FileInfo(pdfPath);
            var directoryPath = fileInfo.DirectoryName;
            if (string.IsNullOrWhiteSpace(directoryPath)) return null;

            var nameWithoutExtension = fileInfo.Name.Replace(fileInfo.Extension, string.Empty);

            var jpegPath = Path.Combine(directoryPath, $"{nameWithoutExtension}.jpg");
            if (multiPage)
                jpegPath = Path.Combine(directoryPath, $"{nameWithoutExtension}-page{{0}}.jpg");

            return jpegPath;
        }

        #endregion
    }
}