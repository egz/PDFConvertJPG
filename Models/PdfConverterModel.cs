using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using PdfiumViewer;


namespace PDFConvertJPG.Models
{

    public class PdfConverterModel
    {

        public void ConvertPdfToJpg(string pdfPath, string outputBaseDir, string folderName)
        {
            string targetFolder = Path.Combine(outputBaseDir, folderName);
            if (!Directory.Exists(targetFolder)) Directory.CreateDirectory(targetFolder);

            string pdfFileName = Path.GetFileNameWithoutExtension(pdfPath);

            using (var document = PdfDocument.Load(pdfPath))
            {
                for (int i = 0; i < document.PageCount; i++)
                {
                    // 讀取該頁面的原始尺寸 (Units in Points)
                    var pageSize = document.PageSizes[i];

                    // 直接以原始寬高進行渲染，確保不失真、不變形
                    // 第二個參數為 DPI，通常 96 是標準，若要更清晰可設為 150 或 300
                    int renderWidth = (int)pageSize.Width;
                    int renderHeight = (int)pageSize.Height;

                    string baseFileName = $"{pdfFileName}_{i + 1}";
                    string finalPath = GetUniqueFilePath(targetFolder, baseFileName, ".jpg");

                    // 呼叫 Pdfium 渲染原始尺寸
                    using (var image = document.Render(i, renderWidth, renderHeight, true))
                    {
                        image.Save(finalPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
            }
        }

        // 檢查重複檔名並自動編號邏輯
        private string GetUniqueFilePath(string folder, string fileName, string extension)
        {
            string fullPath = Path.Combine(folder, fileName + extension);
            int count = 1;

            while (File.Exists(fullPath))
            {
                fullPath = Path.Combine(folder, $"{fileName}({count}){extension}");
                count++;
            }
            return fullPath;
        }
    }
}


