using PDFConvertJPG.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PDFConvertJPG.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly PdfConverterModel _model = new PdfConverterModel();

        private const int MaxDegreeOfParallelism = 5;

        public ObservableCollection<FileItem> SelectedFiles { get; set; } = new ObservableCollection<FileItem>();

        private string _outputPath;
        public string OutputPath
        {
            get => _outputPath;
            set
            {
                if (_outputPath == value) return;
                _outputPath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OutputPath)));
            }
        }

        public int ImageWidth { get; set; } = 1080;
        public int ImageHeight { get; set; } = 1920;

        // 通知介面更新的事件
        public event PropertyChangedEventHandler PropertyChanged;

        public async Task ProcessConversionAsync()
        {
            var toProcess = SelectedFiles.Where(f => f.IsChecked).Select(f => f.FilePath).ToList();
            if (toProcess.Count == 0 || string.IsNullOrEmpty(OutputPath)) return;

            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            using (SemaphoreSlim semaphore = new SemaphoreSlim(MaxDegreeOfParallelism))
            {
                var tasks = new List<Task>();
                foreach (var file in toProcess)
                {
                    await semaphore.WaitAsync();
                    tasks.Add(Task.Run(() =>
                    {
                        try
                        {
                            _model.ConvertPdfToJpg(file, OutputPath, timestamp);
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }));
                }
                await Task.WhenAll(tasks);
            }
        }
    }
}
