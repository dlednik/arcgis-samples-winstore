using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media;

namespace InteractiveSDK.DataModel
{
    public class GroupDetailPageVM : INotifyPropertyChanged
    {
        public GroupDetailPageVM()
        {
            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                UniqueId = Title = "Mapping";
            }
        }

        private void LoadSamples(string groupName)
        {
            List<Sample> samples = new List<Sample>();
            var sampleDataGroups = SampleDataSource.GetGroup(groupName);
            if (sampleDataGroups != null)
            {
                foreach (var sample in sampleDataGroups.Items)
                {
                    samples.Add(new Sample()
                    {
                        Title = sample.Title,
                        Description = sample.Description,
                        UniqueId = sample.UniqueId,
                        Image = sample.Image
                    });
                }
            }
            Samples = samples;
        }

        private IEnumerable<Sample> m_Samples;

        public IEnumerable<Sample> Samples
        {
            get { return m_Samples; }
            private set
            {
                m_Samples = value;
                OnPropertyChanged();
            }
        }

        private string m_Title;

        public string Title
        {
            get { return m_Title; }
            set
            {
                m_Title = value;
                OnPropertyChanged();
            }
        }

        private string m_UniqueID;

        public string UniqueId
        {
            get { return m_UniqueID; }
            set
            {
                m_UniqueID = value;
                OnPropertyChanged();
                LoadSamples(value);
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class Sample
    {
        public string UniqueId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ImageSource Image { get; set; }
    }
}
