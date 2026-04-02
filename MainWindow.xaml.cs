using Microsoft.Win32;
using PDFConvertJPG.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PDFConvertJPG
{
    public partial class MainWindow : Window
    {
        MainViewModel _vm = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = _vm;
        }

        // ... 省略 BtnSelectFiles 和 BtnSelectPath (與之前相同) ...

        // 1. 選擇檔案的事件 (對應 XAML 中的 BtnSelectFiles_Click)
        private void BtnSelectFiles_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "PDF Files (*.pdf)|*.pdf"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    // 檢查是否已在清單中，避免重複加入
                    if (!_vm.SelectedFiles.Any(fi => fi.FilePath == filename))
                    {
                        _vm.SelectedFiles.Add(new FileItem(filename));
                        // 移除: FileList.Items.Add(filename);
                    }
                }
            }
        }

        private void BtnClearFiles_Click(object sender, RoutedEventArgs e)
        {
            _vm.SelectedFiles.Clear();
            var a = FileList.Items;
            // 移除: FileList.Items.Clear();
        }

        // 2. 選擇路徑的事件 (對應 XAML 中的 BtnSelectPath_Click)
        private void BtnSelectPath_Click(object sender, RoutedEventArgs e)
        {
            // WPF 原生沒有漂亮的資料夾選擇器，常用 OpenFileDialog 的小技巧
            // 或者使用 WinForms 的 FolderBrowserDialog
            var dialog = new OpenFolderDialog();

            if ( dialog.ShowDialog() == true)
            {
                _vm.OutputPath = dialog.FolderName;
                TxtOutputPath.Text = _vm.OutputPath;
            }
        }

        // 3. 開始轉換的事件 (對應 XAML 中的 BtnConvert_Click)
        private async void BtnConvert_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                // 設定滑鼠指標為轉圈圈 (僅在此視窗)
                Mouse.OverrideCursor = Cursors.Wait;
                btn.IsEnabled = false;

                try
                {
                    await _vm.ProcessConversionAsync();
                    MessageBox.Show("所有檔案轉換完成！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"發生錯誤: {ex.Message}", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    // 恢復滑鼠指標與按鈕狀態
                    Mouse.OverrideCursor = null;
                    btn.IsEnabled = true;
                }
            }
        }

        private void MenuDeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            // 正確移除選取項目
            var selected = FileList.SelectedItems.Cast<FileItem>().ToList();
            foreach (var fi in selected) _vm.SelectedFiles.Remove(fi);
        }
    }
}
