using Reporter.Commands;
using Reporter.DataContracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
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

        public string SelectedType { get => selectedType; set => SetProperty(ref selectedType,value); }

        public ObservableCollection<string> ImagePaths { get => imagePaths; set => SetProperty(ref imagePaths, value); }

        public ICommand AddPictureCommand { get; private set; }
        public ICommand GetLocationCommand { get; private set; }
        public ICommand TakePictureCommand { get; private set; }
        public ICommand SubmitCommand { get; private set; }

        public ICommand RemovePictureCommand { get; private set; }
        public ReporterViewModel()
        {
            RemovePictureCommand = new RemovePictureCommand(this);
            AddPictureCommand = new AddPicture(this);
            GetLocationCommand = new GetLocationCommand(this);
            TakePictureCommand = new TakePictureCommand(this);
            SubmitCommand = new SubmitCommand(this);
            ImagePaths = new ObservableCollection<string>();
        }
    }
}
