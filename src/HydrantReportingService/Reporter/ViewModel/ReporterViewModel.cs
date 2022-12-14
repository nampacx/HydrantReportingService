using CommunityToolkit.Mvvm.Input;
using HydrantReportingService.Library;
using Reporter.Commands;
using Reporter.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Reporter.ViewModel
{
    public class ReporterViewModel : INotifyPropertyChanged
    {
        private Location location;
        private string locationString;
        private ObservableCollection<string> imagePaths;
        private string notes;
        private bool defect;
        private string selectedType;
        private int maxSteps;
        private int currentSteps;
        private double progress;
        private bool activityInProgress;

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Object.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        #endregion

        public Location Location { get => location; set => SetProperty(ref location, value); }
        public string LocationString { get => locationString; set => SetProperty(ref locationString, value); }
        public string Notes { get => notes; set => SetProperty(ref notes, value); }
        public bool Defect { get => defect; set => SetProperty(ref defect, value); }
        public IReadOnlyList<string> AllTypes { get; } = Enum.GetNames(typeof(HydrantType));

        public string SelectedType
        {
            get => selectedType; set
            {
                SetProperty(ref selectedType, value);
                if (Enum.TryParse(value, out HydrantType hydrantType))
                {
                    HydrantType = hydrantType;
                }
            }
        }

        public bool ActivityInProgress { get => activityInProgress; set => SetProperty(ref activityInProgress , value); }

        public HydrantType HydrantType { get; set; }
        public ObservableCollection<string> ImagePaths { get => imagePaths; set => SetProperty(ref imagePaths, value); }

        public ICommand AddPictureCommand { get; private set; }
        public ICommand GetLocationCommand { get; private set; }
        public ICommand TakePictureCommand { get; private set; }
        public ICommand SubmitCommand { get; private set; }

        public double Progress { get => progress; set => SetProperty(ref progress, value); }

        public ICommand RemovePictureCommand { get; private set; }
        public ReporterViewModel()
        {
            RemovePictureCommand = new RemovePictureCommand(this);
            AddPictureCommand = new AsyncRelayCommand(AddPictureAsync);
            GetLocationCommand = new GetLocationCommand(this);
            TakePictureCommand = new TakePictureCommand(this);
            SubmitCommand = new AsyncRelayCommand(() => SubmitHelper.SubmitAsync(this));
            ImagePaths = new ObservableCollection<string>();
        }

        public async Task AddPictureAsync()
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                FileResult photo = await MediaPicker.Default.PickPhotoAsync();

                if (photo != null)
                {
                    this.ImagePaths.Add(photo.FullPath);
                }
            }
        }
    }
}
