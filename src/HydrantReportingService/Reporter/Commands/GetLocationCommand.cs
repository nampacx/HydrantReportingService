using Reporter.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Reporter.Commands
{
    internal class GetLocationCommand : ICommand
    {
        private readonly ReporterViewModel reporterViewModel;

        public GetLocationCommand(ReporterViewModel reporterViewModel)
        {
            this.reporterViewModel = reporterViewModel;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            try
            {
                Location location = Geolocation.Default.GetLastKnownLocationAsync().Result;

                if (location != null)
                {
                    reporterViewModel.Location = location;
                    reporterViewModel.LocationString = $"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}";

                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
            }
            catch (Exception ex)
            {
                // Unable to get location
            }
        }
    }
}
