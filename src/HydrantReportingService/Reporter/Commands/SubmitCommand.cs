using Reporter.Services;
using Reporter.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Reporter.Commands
{
    public class SubmitCommand : ICommand
    {
        private string sas = @"https://hrsvcdevsa.blob.core.windows.net/test?sp=racwl&st=2022-09-19T12:07:38Z&se=2022-09-19T20:07:38Z&spr=https&sv=2021-06-08&sr=c&sig=rN312FHKarf8oS78XxEV21YRN8dVB6f3wxdAUEZDx3Q%3D"; 

        private readonly ReporterViewModel reporterViewModel;

        public event EventHandler CanExecuteChanged;

        public SubmitCommand(ReporterViewModel reporterViewModel)
        {
            this.reporterViewModel = reporterViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            //foreach (var image in reporterViewModel.ImagePaths)
            //{
            //    UploadService.UploadFile(new Uri(sas), "testitest", image);
            //}

            Shell.Current.Navigation.PopAsync();
        }
    }
}
