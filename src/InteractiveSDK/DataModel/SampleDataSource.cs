using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace InteractiveSDK.DataModel
{
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : InteractiveSDK.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String title, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;

            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        public String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Item data model.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String uniqueId, String title, String imagePath, String description, SampleDataGroup group, Type samplePage)
            : base(uniqueId, title, imagePath, description)
        {
            this._group = group;
            this._samplePage = samplePage;
        }

        private Type _samplePage;
        public Type SamplePage
        {
            get { return this._samplePage; }
            set { this.SetProperty(ref this._samplePage, value); }
        }

        private SampleDataGroup _group;
        public SampleDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    public class SampleDataGroup : SampleDataCommon
    {
        public SampleDataGroup(String uniqueId, String title, String imagePath, String description)
            : base(uniqueId, title, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<SampleDataItem> _topItem = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> TopItems
        {
            get { return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// SampleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");

            return _sampleDataSource.AllGroups;
        }

        public static SampleDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static SampleDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        // Sample groups loaded in MainPageVM.cs
        // Sample items loaded in GroupDetailPageVM.cs

        public SampleDataSource()
        {
            // MAPPING
            var groupMapping = new SampleDataGroup("Mapping",
                    "Mapping",
                    "Assets/DarkGray.png",
                    "");

            groupMapping.Items.Add(new SampleDataItem("Mapping-Item-1",
                    "Tiled Layer",
                    "Assets/TiledLayer.png",
                    "Add an ArcGIS Server cached map service layer to a map",
                    groupMapping, typeof(Samples.Maps.TiledMap)));

            groupMapping.Items.Add(new SampleDataItem("Mapping-Item-2",
                   "Dynamic Layer",
                   "Assets/DynamicLayer.png",
                   "Add an ArcGIS Server dynamic (non-cached) map service layer to a map",
                   groupMapping,
                   typeof(Samples.Maps.DynamicMap)));

            groupMapping.Items.Add(new SampleDataItem("Mapping-Item-3",
                    "Tiled & Dynamic Layers",
                    "Assets/TiledDynamic.png",
                    "Add an ArcGIS cached map service layer and dynamic map service layer to a map",
                    groupMapping,
                     typeof(Samples.Maps.DynamicAndTile)));

            groupMapping.Items.Add(new SampleDataItem("Mapping-Item-4",
                 "Feature Layer",
                 "Assets/FeatureLayer.png",
                 "Add ArcGIS feature layers from a feature service to a map",
                 groupMapping,
                 typeof(Samples.Maps.FeatureLayers)));

            groupMapping.Items.Add(new SampleDataItem("Mapping-Item-5",
             "Location Display",
             "Assets/LocationDisplay.png",
             "Display current location and heading",
             groupMapping,
             typeof(Samples.Maps.LocationDisplay)));

            groupMapping.Items.Add(new SampleDataItem("Mapping-Item-7",
             "WMS Layer",
             "Assets/WMSLayer.png",
             "Add a WMS layer to a map",
             groupMapping,
             typeof(Samples.Maps.WmsLayerSimple)));
            
            groupMapping.Items.Add(new SampleDataItem("Mapping-Item-8",
             "CSV Layer",
             "Assets/CsvLayerSimple.png",
             "Add CSV data to a map",
             groupMapping,
             typeof(Samples.Maps.CsvLayerSimple)));            

            groupMapping.Items.Add(new SampleDataItem("Mapping-Item-9",
                 "Overview Map",
                 "Assets/OverviewMap.png",
                  "Use a second map as an overview map",
                 groupMapping,
                 typeof(Samples.Maps.OverviewMap)));

            groupMapping.Items.Add(new SampleDataItem("Mapping-Item-6",
             "Bind to Map Properties",
             "Assets/BindMapProperties.png",
             "Bind to map properties to be notified of changes in property values",
             groupMapping,
             typeof(Samples.Maps.MapProperties)));

            groupMapping.Items.Add(new SampleDataItem("Mapping-Item-10",
               "Enable Touch Rotation",
               "Assets/EnableTouchRotation.png",
               "Set the ManipulationMode to 'All' to enable map rotation via touch input",
               groupMapping,
               typeof(Samples.Maps.EnableTouchRotation)));

            this.AllGroups.Add(groupMapping);


            // GRAPHICS
            var groupGraphics = new SampleDataGroup("Graphics",
                    "Graphics",
                    "Assets/DarkGray.png",
                    "");

            groupGraphics.Items.Add(new SampleDataItem("Group-2-Item-3",
             "Add using XAML",
             "Assets/AddGraphicsXAML.png",
             "Define symbols and graphics in XAML",
             groupGraphics,
             typeof(Samples.Graphics.AddGraphicsXAML)));

            groupGraphics.Items.Add(new SampleDataItem("Group-2-Item-4",
             "Add point on tap",
             "Assets/AddPointOnTap.png",
             "Tap on the map to add a graphic",
             groupGraphics,
             typeof(Samples.Graphics.AddPointOnTap)));

            groupGraphics.Items.Add(new SampleDataItem("Group-2-Item-6",
             "Graphic Pointer Events",
             "Assets/GraphicEvents.png",
             "Listen for user interaction events on graphics",
             groupGraphics,
             typeof(Samples.Graphics.GraphicEvents)));

            groupGraphics.Items.Add(new SampleDataItem("Group-2-Item-7",
             "Pen Drawing",
             "Assets/PenDrawing.png",
             "Redline using a pen device",
             groupGraphics,
             typeof(Samples.Graphics.PenDrawing)));

            this.AllGroups.Add(groupGraphics);


            // EXTRAS
            var groupExtras = new SampleDataGroup("Extras",
                    "Extras",
                    "Assets/DarkGray.png",
                    "");          

            groupExtras.Items.Add(new SampleDataItem("Group-3-Item-4",
                 "Layer list",
                 "Assets/LayerList.png",
                 "Show all layers, manipulate opacity, visibility and order",
                 groupExtras,
                 typeof(Samples.Misc.LayerList)));

            groupExtras.Items.Add(new SampleDataItem("Group-3-Item-5",
                 "Symbol Swatches",
                 "Assets/SymbolSwatches.png",
                 "Create Symbol Swatches from Symbols",
                 groupExtras,
                 typeof(Samples.Misc.SymbolSwatches)));

            //groupExtras.Items.Add(new SampleDataItem("Group-3-Item-6",
            // "Simple MapTip",
            // "Assets/MediumGray.png",
            // "",
            // groupExtras,
            // typeof(Samples.Misc.SimpleMapTip)));

            this.AllGroups.Add(groupExtras);


            // IMAGE SERVICES
            var groupImageServices = new SampleDataGroup("Image Services",
                    "Image Services",
                    "Assets/DarkGray.png",
                    "");

            groupImageServices.Items.Add(new SampleDataItem("Group-4-Item-1",
             "Simple Image Service",
             "Assets/ImageServiceSimple.png",
             "Add an ArcGIS Server image service layer to a map",
             groupImageServices,
             typeof(Samples.ImageServices.ImageServiceSimple)));

            groupImageServices.Items.Add(new SampleDataItem("Group-4-Item-2",
             "Shaded Relief, Slope",
             "Assets/ShadedReliefSlope.png",
             "Use a RenderingRule to display imagery with different display formats",
             groupImageServices,
             typeof(Samples.ImageServices.ShadedReliefSlope)));
            
            groupImageServices.Items.Add(new SampleDataItem("Group-4-Item-4",
             "Stretch",
             "Assets/Stretch.png",
             "Use a RenderingRule to display imagery with different stretch formats",
             groupImageServices,
             typeof(Samples.ImageServices.Stretch)));
            
            groupImageServices.Items.Add(new SampleDataItem("Group-4-Item-3",
             "Raster Functions",
             "Assets/RasterFunctions.png",
             "Apply raster functions provided by the image service to change layer display results",
             groupImageServices,
             typeof(Samples.ImageServices.RasterFunctions)));

            this.AllGroups.Add(groupImageServices);


            // DYNAMIC LAYERS
            var groupDynamicLayers = new SampleDataGroup("Dynamic Layers",
                    "Dynamic Layers",
                    "Assets/DarkGray.png",
                    "");

            groupDynamicLayers.Items.Add(new SampleDataItem("Group-5-Item-1",
             "Dynamic Layers In Code",
             "Assets/DynamicLayersInCode.png",
             "",
             groupDynamicLayers,
             typeof(Samples.DynamicLayer.DynamicLayersInCode)));

            this.AllGroups.Add(groupDynamicLayers);


            // QUERY
            var groupQueryTask = new SampleDataGroup("Query",
                    "Query",
                    "Assets/DarkGray.png",
                    "");

            //groupQueryTask.Items.Add(new SampleDataItem("Group-6-Item-1",
            // "Query Only",
            // "Assets/QueryOnly.png",
            // "",
            // groupQueryTask,
            // typeof(Samples.Query.QueryOnly)));

            groupQueryTask.Items.Add(new SampleDataItem("Group-6-Item-2",
            "Attribute Query",
            "Assets/AttributeQuery.png",
            "",
            groupQueryTask,
            typeof(Samples.Query.AttributeQuery)));

            //groupQueryTask.Items.Add(new SampleDataItem("Group-6-Item-3",
            // "Statistics",
            // "Assets/Statistics.png",
            // "",
            // groupQueryTask,
            // typeof(Samples.Query.Statistics)));
            
            groupQueryTask.Items.Add(new SampleDataItem("Group-6-Item-4",
             "Query Related Records",
             "Assets/QueryRelatedRecords.png",
             "",
             groupQueryTask,
             typeof(Samples.Query.QueryRelatedRecords)));

            groupQueryTask.Items.Add(new SampleDataItem("Group-6-Item-5",
               "Identify",
               "Assets/Identify.png",
               "",
               groupQueryTask,
               typeof(Samples.Query.Identify)));

            //groupQueryTask.Items.Add(new SampleDataItem("Group-6-Item-7",
            // "Buffer",
            // "Assets/Statistics.png",
            // "",
            // groupQueryTask,
            // typeof(Samples.Query.QueryWithBuffer)));

            this.AllGroups.Add(groupQueryTask);


            // NETWORK
            var groupNetwork = new SampleDataGroup("Network",
                    "Network",
                    "Assets/DarkGray.png",
                    "");

            groupNetwork.Items.Add(new SampleDataItem("Group-7-Item-1",
             "Routing",
             "Assets/Routing.png",
             "",
             groupNetwork,
             typeof(Samples.Network.Routing)));

            groupNetwork.Items.Add(new SampleDataItem("Group-7-Item-2",
              "Routing with Barriers",
              "Assets/RoutingWithBarriers.png",
              "",
              groupNetwork,
              typeof(Samples.Network.RoutingWithBarriers)));

            groupNetwork.Items.Add(new SampleDataItem("Group-7-Item-3",
                "Driving Directions",
                "Assets/DrivingDirections.png",
                "",
                groupNetwork,
                typeof(Samples.Network.DrivingDirections)));
            this.AllGroups.Add(groupNetwork);


            // LOCATOR
            var groupLocator = new SampleDataGroup("Address Matching",
                    "Address Matching",
                    "Assets/DarkGray.png",
                    "");

            groupLocator.Items.Add(new SampleDataItem("Group-8-Item-1",
             "Address of location",
             "Assets/AddressOfLocation.png",
             "",
             groupLocator,
             typeof(Samples.Locator.AddressOfLocation)));

            groupLocator.Items.Add(new SampleDataItem("Group-8-Item-2",
             "Batch geocoding",
             "Assets/BatchGeocoding.png",
             "",
             groupLocator,
             typeof(Samples.Locator.BatchGeocoding)));

            groupLocator.Items.Add(new SampleDataItem("Group-8-Item-3",
             "Find an address",
             "Assets/FindAnAddress.png",
             "",
             groupLocator,
             typeof(Samples.Locator.FindAnAddress)));

            this.AllGroups.Add(groupLocator);
        }
    }
}
