using CommunityToolkit.Maui.Views;
using HydrantReportingService.Library;
using Newtonsoft.Json;
using Reporter.Controls;
using Reporter.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reporter.Services
{
    public static class SubmitHelper
    {
        public static async Task SubmitAsync(ReporterViewModel reporterViewModel)
        {
            reporterViewModel.ActivityInProgress = true;
            var newReport = new HydrantReportDTO
            {
                Longitude = reporterViewModel.Location.Longitude,
                Latitude = reporterViewModel.Location.Latitude,
                Defect = reporterViewModel.Defect,
                Notes = reporterViewModel.Notes,
                Type = reporterViewModel.HydrantType,
            };
            try
            {
                var report = await SubmitRequestAsync(newReport);
                var sasUri = await RequestSasUriAsync(report.Id);

                await UploadFilesAsync(sasUri, report.Id, reporterViewModel.ImagePaths);
                reporterViewModel.ActivityInProgress = false;
                await ShowPupup("Report sucessfully created!");
            }
            catch (Exception e)
            {
                reporterViewModel.ActivityInProgress = false;
                await ShowPupup(e.Message);
            }
            finally
            {
                

                await Shell.Current.Navigation.PopAsync();
                
            }

        }

        private static async Task ShowPupup(string text)
        {
            var popup = new SimplePopup(text);

            await Shell.Current.ShowPopupAsync(popup);
        }

        private static async Task UploadFilesAsync(Uri sasUri, string reportId, IEnumerable<string> files)
        {
            foreach (var image in files)
            {
                await UploadService.UploadFile(sasUri, reportId, image);
            }
        }

        private static async Task<Uri> RequestSasUriAsync(string reportId)
        {
            var restClient = new RestClient(GlobalSettings.Settings.RequestSasUriUri.Replace("{reportId}", reportId));
            var result = await restClient.GetAsync(new RestRequest());

            if (result.IsSuccessful)
                return new Uri(result.Content.Trim('\"'));
            else
                return null;
        }

        private static async Task<HydrantReportDTO> SubmitRequestAsync(HydrantReportDTO hydrantReportDTO)
        {
            var restClient = new RestClient(GlobalSettings.Settings.SubmitReportUri);
            var postRequest = new RestRequest();
            postRequest.AddBody(hydrantReportDTO);
            var result = await restClient.PostAsync(postRequest);

            if (result.IsSuccessful)
                return JsonConvert.DeserializeObject<HydrantReportDTO>(result.Content);
            else
                return null;
        }
    }
}
