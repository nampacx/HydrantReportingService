using HydrantReportingService.Library;
using Newtonsoft.Json;
using Reporter.Services;
using Reporter.ViewModel;
using RestSharp;
using System.Windows.Input;

namespace Reporter.Commands
{
    public class SubmitCommand : ICommand
    {
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
            var newReport = new HydrantReportDTO
            {
                Defect = reporterViewModel.Defect,
                Notes = reporterViewModel.Notes,
                Type = reporterViewModel.HydrantType
            };

            var reportId = SubmitRequest(newReport).Id;
            var sasUri = RequestSasUri(reportId);
            UploadFiles(sasUri);

            Shell.Current.Navigation.PopAsync();
        }

        private void UploadFiles(Uri sasUri)
        {
            foreach (var image in reporterViewModel.ImagePaths)
            {
                UploadService.UploadFile(sasUri, "", image);
            }
        }

        private Uri RequestSasUri(object reportId)
        {
            var restClient = new RestClient($"{GlobalSettings.Settings.RequestSasUriUri}/{reportId}");
            var result = restClient.Get(new RestRequest());

            if (result.IsSuccessful)
                return new Uri(result.Content);

            return null;
        }

        private HydrantReportDTO SubmitRequest(HydrantReportDTO hydrantReportDTO)
        {
            var restClient = new RestClient(GlobalSettings.Settings.SubmitReportUri);
            var postRequest = new RestRequest();
            postRequest.AddBody(hydrantReportDTO);
            var result = restClient.Post(postRequest);

            if (result.IsSuccessful)
                return JsonConvert.DeserializeObject<HydrantReportDTO>(result.Content);

            return null;
        }
    }
}
