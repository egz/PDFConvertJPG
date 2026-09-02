using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace PDFConvertJPG.Models
{
    public class ImageToPdfConverter
    {
        public void ConvertJpgsToPdf(
            IEnumerable<string> imagePaths,
            string outputPath)
        {
            using var document = new PdfDocument();

            foreach (string imagePath in imagePaths)
            {
                using var image = XImage.FromFile(imagePath);

                var page = document.AddPage();

                page.Width = XUnit.FromPoint(image.PointWidth);
                page.Height = XUnit.FromPoint(image.PointHeight);

                using var gfx = XGraphics.FromPdfPage(page);

                gfx.DrawImage(
                    image,
                    0,
                    0,
                    page.Width.Point,
                    page.Height.Point);
            }

            document.Save(outputPath);
        }
    }
}
