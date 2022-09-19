using Reporter.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Reporter.Commands
{
    internal class RemovePictureCommand : ICommand
    {
        private readonly ReporterViewModel reporterViewModel;

        public event EventHandler CanExecuteChanged;

        public RemovePictureCommand(ReporterViewModel reporterViewModel)
        {
            this.reporterViewModel = reporterViewModel;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is string stringParameter)
                reporterViewModel.ImagePaths.Remove(stringParameter);
        }
    }
}
