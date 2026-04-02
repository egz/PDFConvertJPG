using System.ComponentModel;

namespace PDFConvertJPG.ViewModels
{
    public class FileItem : INotifyPropertyChanged
    {
        public FileItem(string filePath)
        {
            FilePath = filePath;
        }

        public string FilePath { get; }

        private bool _isChecked = true;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked == value) return;
                _isChecked = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}