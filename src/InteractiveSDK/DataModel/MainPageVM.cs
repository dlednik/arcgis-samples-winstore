using System.Collections.Generic;

namespace InteractiveSDK.DataModel
{
    public class MainPageVM
    {
        public MainPageVM()
        {
            List<Group> groups = new List<Group>();
            var sampleDataGroups = SampleDataSource.GetGroups("AllGroups");
            foreach (var group in sampleDataGroups)
            {
                groups.Add(new Group() { Title = group.Title, UniqueId = group.UniqueId });
            }
            m_Groups = groups;
        }

        private IEnumerable<Group> m_Groups;

        public IEnumerable<Group> Groups
        {
            get { return m_Groups; }
            private set { m_Groups = value; }
        }
    }

    public class Group
    {
        public string Title { get; set; }
        public string UniqueId { get; set; }
    }
}
