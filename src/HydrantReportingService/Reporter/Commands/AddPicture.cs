using Reporter.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Reporter.Commands
{
    internal class AddPicture : ICommand
    {
        private readonly ReporterViewModel reporterViewModel;

        public event EventHandler CanExecuteChanged;

        public AddPicture(ReporterViewModel reporterViewModel)
        {
            this.reporterViewModel = reporterViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            PickPhoto();
        }

        private async void PickPhoto()
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                FileResult photo = await MediaPicker.Default.PickPhotoAsync();

                if (photo != null)
                {
                    reporterViewModel.ImagePaths.Add(photo.FullPath);
                }
            }
        }
    }
}
