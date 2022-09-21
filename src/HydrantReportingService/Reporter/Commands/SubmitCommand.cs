﻿using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using HydrantReportingService.Library;
using Newtonsoft.Json;
using Reporter.Controls;
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
            Shell.Current.Dispatcher.Dispatch(() =>
            {
                reporterViewModel.MaxSteps = 2 + reporterViewModel.ImagePaths.Count;
            });

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
                Shell.Current.Dispatcher.Dispatch(() =>
                {
                    reporterViewModel.CurrentSteps = 0;
                });
                var reportId = SubmitRequest(newReport).Id;
                Shell.Current.Dispatcher.Dispatch(() =>
                {
                    reporterViewModel.CurrentSteps++;
                });

                var sasUri = RequestSasUri(reportId);
                Shell.Current.Dispatcher.Dispatch(() =>
                {
                    reporterViewModel.CurrentSteps++;
                });

                UploadFiles(sasUri, reportId);
                ShowPupup("Report sucessfully created!");
            }
            catch (Exception e)
            {
                ShowPupup(e.Message);
            }

            Shell.Current.Navigation.PopAsync();
        }

        private void ShowPupup(string text)
        {
            var popup = new SimplePopup(text);

            Shell.Current.ShowPopup(popup);
        }

        private void UploadFiles(Uri sasUri, string reportId)
        {
            foreach (var image in reporterViewModel.ImagePaths)
            {
                UploadService.UploadFile(sasUri, reportId, image);
                Shell.Current.Dispatcher.Dispatch(() =>
                {
                    reporterViewModel.CurrentSteps++;
                });
            }
        }

        private Uri RequestSasUri(string reportId)
        {
            var restClient = new RestClient(GlobalSettings.Settings.RequestSasUriUri.Replace("{reportId}", reportId));
            var result = restClient.Get(new RestRequest());

            if (result.IsSuccessful)
                return new Uri(result.Content.Trim('\"'));
            else
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
            else
                return null;
        }
    }
}
