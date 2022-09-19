using Reporter.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Reporter.Commands
{
    internal class TakePictureCommand : ICommand
    {
        private readonly ReporterViewModel reporterViewModel;

        public event EventHandler CanExecuteChanged;

        public TakePictureCommand(ReporterViewModel reporterViewModel)
        {
            this.reporterViewModel = reporterViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;        
        }

        public void Execute(object parameter)
        {
            TakePhoto();
        }

        private async void TakePhoto()
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

                if (photo != null)
                {
                    // save the file into local storage
                    string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                    using Stream sourceStream = await photo.OpenReadAsync();
                    using FileStream localFileStream = File.OpenWrite(localFilePath);

                    await sourceStream.CopyToAsync(localFileStream);
                    reporterViewModel.ImagePaths.Add(localFilePath);
                }
            }
        }
    }
}
