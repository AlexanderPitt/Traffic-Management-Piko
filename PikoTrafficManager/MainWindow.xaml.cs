using Dragon.IO.Files.INI;
using PikoTrafficManager.Control;
using PikoTrafficManager.Data;
using PikoTrafficManager.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Vlc.DotNet.Wpf;

namespace PikoTrafficManager
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MediaDetection mediaDetectionForm = null;

        public string DataHost = "";
        public bool IsRemote = false;
        public List<Volume> PikoVolumes = new List<Volume>();
        public DataObject DragAndDropFileToBlockOrPlaylist = null;
        public DataObject DragAndDropBlockToPlaylist = null;
        public Volume currentVolume = null;
        public Support currentSupport = null;
        public Playlist currentBlock = null;
        public Playlist currentPlayList = null;

        public List<DataPikoClient.Category> Categories = new List<DataPikoClient.Category>();
        public ObservableCollection<Support> Supports { get; set; }
        public ObservableCollection<Playlist> Blocks { get; set; }
        public ObservableCollection<Playlist> Playlists { get; set; }
        public ObservableCollection<PlaylistElement> BlocksElements { get; set; }
        public ObservableCollection<PlaylistElement> PlaylistElements { get; set; }
        public ObservableCollection<DataPikoClient.TemplateFieldData> TemplateFieldList { get; set; }
        private EndpointAddress PikoDataServiceEndPoint
        {
            get
            {
                if (this.DataHost.Length > 0)
                {
                    UriBuilder uriBuilder = new UriBuilder(this.DataHost);
                    uriBuilder.Path += "ServiceDataPiko";
                    EndpointAddress endPoint = new EndpointAddress(uriBuilder.Uri.AbsoluteUri);
                    return endPoint;
                }
                else
                    return null;
            }
        }

        private NetTcpBinding Binder
        {
            get
            {
                NetTcpBinding binding = new NetTcpBinding(SecurityMode.None, false);
                binding.CloseTimeout = new TimeSpan(0, 1, 30);
                binding.OpenTimeout = new TimeSpan(0, 1, 30);
                binding.SendTimeout = new TimeSpan(0, 3, 30);
                binding.ReceiveTimeout = new TimeSpan(0, 3, 30);
                binding.TransferMode = TransferMode.Buffered;
                binding.TransactionProtocol = TransactionProtocol.OleTransactions;
                binding.TransactionFlow = false;
                binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
                binding.ListenBacklog = 10;
                binding.MaxBufferPoolSize = 1524288;
                binding.MaxBufferSize = 1524288;
                binding.MaxConnections = 10;
                binding.MaxReceivedMessageSize = 1524288;
                binding.ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas();
                binding.ReaderQuotas.MaxDepth = 32;
                binding.ReaderQuotas.MaxStringContentLength = 16384;
                binding.ReaderQuotas.MaxArrayLength = 16384;
                binding.ReaderQuotas.MaxBytesPerRead = 8196;
                binding.ReaderQuotas.MaxNameTableCharCount = 16384;
                return binding;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            this.Blocks = new ObservableCollection<Playlist>();
            this.Playlists = new ObservableCollection<Playlist>();
            this.Supports = new ObservableCollection<Support>();
            this.BlocksElements = new ObservableCollection<PlaylistElement>();
            this.PlaylistElements = new ObservableCollection<PlaylistElement>();


        }
        #region IHM Functionnalities
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //Include LOCAL/REMOTE
                if (ConfigurationManager.AppSettings["Connectivity"] == "Remote")
                {
                    this.DataHost = ConfigurationManager.AppSettings["RemoteDataHost"];
                    this.IsRemote = true;
                    this.MediaPreviewInfo.SelectedIndex = 1;
                    this.TBPreview.Visibility = Visibility.Hidden; // Hide Preview
                }
                else
                    this.DataHost = ConfigurationManager.AppSettings["LocalDataHost"];



                INIFile pikoVideoServer = null;
                using (DataPikoClient.ServiceDataPikoClient sdpc = new DataPikoClient.ServiceDataPikoClient(this.Binder, this.PikoDataServiceEndPoint))
                {
                    //Check Database & Other
                    sdpc.CheckSystem();
                    //GetConfig
                    pikoVideoServer = INIFile.ParseINIString(sdpc.GetRemoteConfig());
                    //Get Categories with fields
                    DataPikoClient.Category[] categories = sdpc.GetCategories();
                    if (categories != null && categories.Length > 0)
                        this.Categories.AddRange(categories);
                }

                //Get Volumes
                INISection storageList = pikoVideoServer.Sections.Where(s => s.Name.ToLower() == "storages").FirstOrDefault();

                for (int i = 0; i < storageList.Items.Length; i++)// (INISectionElement storageElement in storageList.Items)
                {
                    using (DataPikoClient.ServiceDataPikoClient sdpc = new DataPikoClient.ServiceDataPikoClient(this.Binder, this.PikoDataServiceEndPoint))
                    {
                        INISectionElement storageElement = storageList.Items[i];
                        string storagePath = storageElement.AsString();
                        //Remote                        
                        DataPikoClient.VolumeData volumeData = sdpc.GetVolumeData(storagePath);
                        if (volumeData != null)
                        {
                            Volume volume = new Volume(volumeData);
                            PikoVolumes.Add(volume);
                            cbVolumes.Items.Add(volume.Data.Path);
                        }

                    }
                }
                if (cbVolumes.Items.Count > 0)
                {
                    cbVolumes.SelectedIndex = 0;
                    if (PikoVolumes.Any(pv => pv.Data.SupportsData.Any(pvsd => !pvsd.IsExist)))
                    {
                        mediaDetectionForm = new MediaDetection();
                        foreach (Volume vol in PikoVolumes.Where(pv => pv.Data.SupportsData.Any(pvsd => !pvsd.IsExist)))
                        {
                            foreach (DataPikoClient.SupportData sup in vol.Data.SupportsData.Where(pvsd => !pvsd.IsExist))
                            {
                                mediaDetectionForm.listNewMedia.Add(new DetectedMedia() { SupportData = sup, Volume = vol.Path });
                            }
                        }
                        if (mediaDetectionForm.listNewMedia.Count > 0)
                        {
                            mediaDetectionForm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                            if (mediaDetectionForm.ShowDialog().Value)
                            {
                                // Launch a BackgroundWorker to add selected file in DB
                                System.ComponentModel.BackgroundWorker addNewFileWorker = new System.ComponentModel.BackgroundWorker();
                                addNewFileWorker.WorkerSupportsCancellation = true;
                                addNewFileWorker.WorkerReportsProgress = true;
                                addNewFileWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(bw_DoworkAddDetectedMedias);
                                addNewFileWorker.RunWorkerAsync(mediaDetectionForm.listNewMedia);
                            }
                        }
                    }
                }
                if (!this.IsRemote)
                {

                    string VlcDirectory = ConfigurationManager.AppSettings["VLC_Preview"]; //@"C:\Program Files (x86)\VideoLAN\VLC\";
                    //if (!System.IO.Directory.Exists(VlcDirectory))
                    //    VlcDirectory = @"C:\Program Files\VideoLAN\VLC\";
                    try
                    {
                        
                        this.vlcPreviewPlayer.MediaPlayer.VlcLibDirectory = new DirectoryInfo(VlcDirectory);
                        ///this.vlcPreviewPlayer.MediaPlayer.VlcLibDirectoryNeeded += this.OnVlcControlNeedsLibDirectory;                
                        this.vlcPreviewPlayer.MediaPlayer.EndInit();
                    }
                    catch (Exception ex)
                    {
                        this.TBPreview.Visibility = Visibility.Hidden;
                        this.TBMediaInfo.Visibility = Visibility.Visible;
                        this.MediaPreviewInfo.SelectedIndex = 1;
                    }
                }
                foreach (DataPikoClient.Category cat in this.Categories)
                    this.cbSupportCategory.Items.Add(cat.Name);
                this.cbSupportCategory.SelectedIndex = 0;

                //Get available Blocks and Playlist
                using (DataPikoClient.ServiceDataPikoClient sdpc = new DataPikoClient.ServiceDataPikoClient(this.Binder, this.PikoDataServiceEndPoint))
                {
                    DataPikoClient.PlaylistData[] blocksList = sdpc.GetBlocks();

                    foreach (DataPikoClient.PlaylistData plData in blocksList)
                    {
                        Playlist ABlock = new Playlist(plData);
                        if(ABlock != null)
                            this.Blocks.Add(ABlock);
                    }                                                           
                }
                using (DataPikoClient.ServiceDataPikoClient sdpc = new DataPikoClient.ServiceDataPikoClient(this.Binder, this.PikoDataServiceEndPoint))
                {
                    DataPikoClient.PlaylistData[] playlistsList = sdpc.GetPlaylists();

                    foreach (DataPikoClient.PlaylistData plData in playlistsList)
                    {
                        Playlist APlaylist = new Playlist(plData);
                        if (APlaylist != null)
                            this.Playlists.Add(APlaylist);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnVlcControlNeedsLibDirectory(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            if (currentDirectory == null)
                return;
            if (AssemblyName.GetAssemblyName(currentAssembly.Location).ProcessorArchitecture == ProcessorArchitecture.X86)
            {
                e.VlcLibDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, @"vlslib\x86\"));
            }
            else
            {
                e.VlcLibDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, @"vlslib\x64\"));
            }
        }

        private void MainPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tc = sender as TabControl;
            if (tc != null)
            {
                TabItem selectedItem = (TabItem)tc.SelectedItem;
                lvBlockList.Visibility = System.Windows.Visibility.Hidden;
                if (selectedItem.TabIndex == 2)
                    lvBlockList.Visibility = System.Windows.Visibility.Visible;

            }
        }

        private void cbVolumes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.currentVolume = this.PikoVolumes[this.cbVolumes.SelectedIndex];
            this.Supports.Clear();
            foreach (Support sup in this.currentVolume.Supports)
                this.Supports.Add(sup);
        }

        private void lvFilesInVolume_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.lvFilesInVolume.SelectedIndex == -1)
                return;
            this.currentSupport = this.currentVolume.Supports[this.lvFilesInVolume.SelectedIndex];

            ////Get data from database ...
            //using (DataPikoClient.ServiceDataPikoClient sdpc = new DataPikoClient.ServiceDataPikoClient(this.Binder, this.PikoDataServiceEndPoint))
            //{
            //    DataPikoClient.SupportData supportInfo = sdpc.GetVideoInfo(this.currentSupport.Data.UIdSupport);
            //    if (supportInfo != null)
            //    {
            //        this.currentSupport.Data.UIdSupport = supportInfo.UIdSupport;
            //        this.currentSupport.Data.TcStart = supportInfo.TcStart;
            //        this.currentSupport.Data.Duration = supportInfo.Duration;
            //        this.currentSupport.Data.Eom = supportInfo.Eom;
            //        this.currentSupport.Data.FileSize = supportInfo.FileSize;
            //    }
            //}

            //Populate data
            this.tbSupportFileName.Text = this.currentSupport.Data.FileName;// System.IO.Path.GetFileNameWithoutExtension(this.currentSupport.FileName);
            this.tbSupportTitle.Text = this.currentSupport.Data.Title;
            this.tbSupportFullPath.Text = this.currentSupport.Data.FullPath;
            DataPikoClient.Category currentCat = this.Categories.First(c => c.Id == this.currentSupport.Data.IdCategory);
            this.cbSupportCategory.SelectedIndex = this.Categories.IndexOf(currentCat);

            using (DataPikoClient.ServiceDataPikoClient sdpc = new DataPikoClient.ServiceDataPikoClient(this.Binder, this.PikoDataServiceEndPoint))
            {
                string mediaInfo = sdpc.GetMediaInfo(this.tbSupportFullPath.Text, DataPikoClient.MediaInfoScanType.Full);
                mediaInfoBox.Content = mediaInfo;
            }

            spAdditionalFieldSupport.Children.Clear();
            //Build TemplateFieldData
            foreach (DataPikoClient.TemplateFieldData tfData in currentCat.TemplateFields)
            {
                AdditionalField newAdditionalField = new AdditionalField();
                newAdditionalField.Name = "C" + currentCat.Id + "_" + ((int)tfData.FieldType).ToString() + "_" + tfData.FieldName.Replace(' ', '_');
                newAdditionalField.Tag = tfData.IdFieldCategory;
                newAdditionalField.lblFieldName.Content = tfData.FieldName;

                string fieldTypeString = "";
                switch (tfData.FieldType)
                {
                    case DataPikoClient.TemplateFieldType.Picture:
                        fieldTypeString = "Picture";
                        break;
                    case DataPikoClient.TemplateFieldType.Video:
                        fieldTypeString = "Video";
                        break;
                    case DataPikoClient.TemplateFieldType.Text:
                        fieldTypeString = "Text";
                        break;
                }

                newAdditionalField.lblFieldType.Content = fieldTypeString;
                newAdditionalField.tbValue.IsEnabled = this.tbSupportTitle.IsEnabled;
                if (this.currentSupport.Data.TemplateFields != null && this.currentSupport.Data.TemplateFields.Length > 0)
                {
                    if (this.currentSupport.Data.TemplateFields.Any(tfs => tfs.FieldDefinition.IdFieldCategory == tfData.IdFieldCategory))
                        newAdditionalField.tbValue.Text = this.currentSupport.Data.TemplateFields.First(tfs => tfs.FieldDefinition.IdFieldCategory == tfData.IdFieldCategory).Value;
                }

                newAdditionalField.btDeleteField.Visibility = Visibility.Visible;
                newAdditionalField.btChooseFile.Visibility = tfData.FieldType == DataPikoClient.TemplateFieldType.Text ? Visibility.Hidden : Visibility.Visible;


                spAdditionalFieldSupport.Children.Add(newAdditionalField);
            }


            //Check if Auto Play ...
            if (!this.IsRemote)
            {
                string[] options = new string[0];
                Uri src = new Uri(this.currentSupport.Data.FullPath);
                if (this.vlcPreviewPlayer.MediaPlayer.IsPlaying) //Delay to other thread play video ...
                {
                    this.vlcPreviewPlayer.MediaPlayer.Stop();
                    System.Threading.Thread.Sleep(150);
                }
                this.vlcPreviewPlayer.MediaPlayer.Play(src);
            }
        }


        /* VLC CONTROL */
        #region VLC CONTROL
        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (!this.vlcPreviewPlayer.MediaPlayer.IsPlaying)
                this.vlcPreviewPlayer.MediaPlayer.Play();
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            if (this.vlcPreviewPlayer.MediaPlayer.IsPlaying)
                this.vlcPreviewPlayer.MediaPlayer.Pause();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (this.vlcPreviewPlayer.MediaPlayer.IsPlaying)
                this.vlcPreviewPlayer.MediaPlayer.Stop();
        }

        private void btnFastForward_Click(object sender, RoutedEventArgs e)
        {
            if (this.vlcPreviewPlayer.MediaPlayer.IsPlaying)
                this.vlcPreviewPlayer.MediaPlayer.Rate += 1;
        }

        private void btnSlow_Click(object sender, RoutedEventArgs e)
        {
            if (this.vlcPreviewPlayer.MediaPlayer.IsPlaying)
            {
                this.vlcPreviewPlayer.MediaPlayer.Rate = 0.20f;
            }
        }
        #endregion

        private void btEdit_Click(object sender, RoutedEventArgs e)
        {
            //Set grid to  editable
            this.dgBlockDetail.IsReadOnly = false;
            this.tbCurrentBlockName.IsReadOnly = false;
            this.btSaveBlock.IsEnabled = true;
        }

        private void btAddNewBlock_Click(object sender, RoutedEventArgs e)
        {
            this.currentBlock = new Playlist("New Block");
            this.currentBlock.Data.PlaylistType = DataPikoClient.PlayListType.Block;
            this.Blocks.Add(this.currentBlock);
            this.tbCurrentBlockName.Text = this.currentBlock.Data.PlaylistTitle;
            this.btEdit_Click(this.btEdit, new RoutedEventArgs());
            this.tbCurrentBlockName.Focus();
            this.lvAvailableBlocks.SelectedIndex = this.lvAvailableBlocks.Items.Count - 1;
            this.BlocksElements.Clear();
            foreach (PlaylistElement elem in this.currentBlock.Elements)
                this.BlocksElements.Add(elem);
            //this.dgBlockDetail.ItemsSource = this.currentBlock.Elements;

        }

        private void lvFilesInVolume_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void tbCurrentBlockName_LostFocus(object sender, RoutedEventArgs e)
        {
            this.Blocks[this.lvAvailableBlocks.SelectedIndex].Data.PlaylistTitle = this.tbCurrentBlockName.Text;
        }

        private void btSaveBlock_Click(object sender, RoutedEventArgs e)
        {
            //Save this block
            this.Blocks[this.lvAvailableBlocks.SelectedIndex].Data.PlaylistTitle = this.tbCurrentBlockName.Text;
            //this.currentBlock.ClearElement();
            this.Blocks[this.lvAvailableBlocks.SelectedIndex].ClearElement();
            foreach (PlaylistElement elem in this.BlocksElements)
                this.Blocks[this.lvAvailableBlocks.SelectedIndex].AddElement(elem);
            //Save to XML ... Local or Remotely
            using (DataPikoClient.ServiceDataPikoClient sdpc = new DataPikoClient.ServiceDataPikoClient(this.Binder, this.PikoDataServiceEndPoint))
            {
                sdpc.SaveBlock(this.Blocks[this.lvAvailableBlocks.SelectedIndex].Data);
            }
        }

        private void lvFilesInVolume_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragAndDropFileToBlockOrPlaylist = new DataObject();
                this.DragAndDropFileToBlockOrPlaylist.SetData("Source", "File");
                this.DragAndDropFileToBlockOrPlaylist.SetData("Index", this.lvFilesInVolume.SelectedIndex);
                DragDrop.DoDragDrop(this.lvFilesInVolume, this.DragAndDropFileToBlockOrPlaylist, DragDropEffects.Copy | DragDropEffects.Move);
            }

        }

        private void dgBlockDetail_DragEnter(object sender, DragEventArgs e)
        {
            base.OnDragEnter(e);

        }

        private void dgBlockDetail_Drop(object sender, DragEventArgs e)
        {
            base.OnDrop(e);
            if (sender is DataGrid)// && !this.tbCurrentBlockName.IsReadOnly)
            {
                //TODO Detect if a block is in edition mode or not !
                Support data = this.Supports[(int)e.Data.GetData("Index")];
                if(this.lvAvailableBlocks.SelectedIndex == -1)
                {
                    this.currentBlock = new Playlist("New Block");
                    this.currentBlock.Data.PlaylistType = DataPikoClient.PlayListType.Block;
                    this.Blocks.Add(this.currentBlock);
                    this.tbCurrentBlockName.Text = this.currentBlock.Data.PlaylistTitle;
                    this.btEdit_Click(this.btEdit, new RoutedEventArgs());
                    this.tbCurrentBlockName.Focus();
                    this.lvAvailableBlocks.SelectedIndex = this.lvAvailableBlocks.Items.Count - 1;
                    this.BlocksElements.Clear();
                    foreach (PlaylistElement elem in this.currentBlock.Elements)
                        this.BlocksElements.Add(elem);
                }
                this.currentBlock.AddElement(data);
                this.BlocksElements.Add(this.currentBlock.Elements.Last());
                this.dgBlockDetail.UpdateLayout();
            }
            e.Handled = true;
        }

        private void dgBlockDetail_DragOver(object sender, DragEventArgs e)
        {
            base.OnDragOver(e);
            e.Effects = DragDropEffects.None;
            if (sender is DataGrid)// && !this.tbCurrentBlockName.IsReadOnly)
            {
                e.Effects = DragDropEffects.Copy | DragDropEffects.Move;
            }
        }

        private void dgBlockDetail_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            base.OnGiveFeedback(e);
            if (e.Effects.HasFlag(DragDropEffects.Copy))
                Mouse.SetCursor(Cursors.Cross);
            else if (e.Effects.HasFlag(DragDropEffects.Move))
                Mouse.SetCursor(Cursors.Pen);
            else
                Mouse.SetCursor(Cursors.No);
        }

        private void lvBlockList_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragAndDropBlockToPlaylist = new DataObject();
                this.DragAndDropBlockToPlaylist.SetData("Source", "Block");
                this.DragAndDropBlockToPlaylist.SetData("Index", this.lvBlockList.SelectedIndex);
                DragDrop.DoDragDrop(this.lvBlockList, this.DragAndDropBlockToPlaylist, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

        private void dgPlaylistDetail_DragEnter(object sender, DragEventArgs e)
        {
            base.OnDragEnter(e);
        }

        private void dgPlaylistDetail_DragOver(object sender, DragEventArgs e)
        {
            base.OnDragOver(e);
            e.Effects = DragDropEffects.None;
            if (sender is DataGrid) //&& !this.tbCurrentBlockName.IsReadOnly
            {
                e.Effects = DragDropEffects.Copy | DragDropEffects.Move;
            }
        }

        private void dgPlaylistDetail_Drop(object sender, DragEventArgs e)
        {
            base.OnDrop(e);

            if(sender is DataGrid)
            {
                if(lvAvailablePlaylists.SelectedIndex == -1)
                {
                    //Create a new one !
                    this.currentPlayList = new Playlist("New Playlist");
                    this.currentPlayList.Data.PlaylistTitle = "New Playlist";
                    this.currentPlayList.Data.PlaylistType = DataPikoClient.PlayListType.Classic;
                    this.Playlists.Add(this.currentPlayList);
                    this.tbCurrentPlaylistName.Text = this.currentPlayList.Data.PlaylistTitle;
                    this.btEditPlaylist_Click(this.btEditPlaylist, new RoutedEventArgs());                    
                    this.lvAvailablePlaylists.SelectedIndex = this.lvAvailablePlaylists.Items.Count - 1;
                    this.PlaylistElements.Clear();
                    foreach (PlaylistElement elem in this.currentPlayList.Elements)
                        this.PlaylistElements.Add(elem);
                }
                string source = (string)e.Data.GetData("Source");

                if (source == "File")
                {
                    Support data = this.Supports[(int)e.Data.GetData("Index")];
                    this.currentPlayList.AddElement(data);
                    this.PlaylistElements.Add(this.currentPlayList.Elements.Last());
                }
                else if (source == "Block")
                {
                    Playlist data = this.Blocks[(int)e.Data.GetData("Index")];
                    foreach (PlaylistElement element in data.Elements)
                    {
                        this.currentBlock.AddElement(element);
                        this.PlaylistElements.Add(this.currentPlayList.Elements.Last());
                    }
                }
                this.dgPlaylistDetail.UpdateLayout();
            }

            //if (sender is DataGrid && !this.tbCurrentPlaylistName.IsReadOnly)
            //{
            //    string source = (string)e.Data.GetData("Source");

            //    if (source == "File")
            //    {
            //        Support data = this.Supports[(int)e.Data.GetData("Index")];
            //        this.currentPlayList.AddElement(data);
            //        this.PlaylistElements.Add(this.currentPlayList.Elements.Last());
            //    }
            //    else if (source == "Block")
            //    {
            //        Playlist data = this.Blocks[(int)e.Data.GetData("Index")];
            //        foreach (PlaylistElement element in data.Elements)
            //        {
            //            this.currentBlock.AddElement(element);
            //            this.PlaylistElements.Add(this.currentPlayList.Elements.Last());
            //        }
            //    }
            //    this.dgPlaylistDetail.UpdateLayout();

            //}
            e.Handled = true;
        }

        private void dgPlaylistDetail_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            base.OnGiveFeedback(e);
            if (e.Effects.HasFlag(DragDropEffects.Copy))
                Mouse.SetCursor(Cursors.Cross);
            else if (e.Effects.HasFlag(DragDropEffects.Move))
                Mouse.SetCursor(Cursors.Pen);
            else
                Mouse.SetCursor(Cursors.No);
        }

        private void btSavePlaylist_Click(object sender, RoutedEventArgs e)
        {
            //Save this block
            this.Playlists[this.lvAvailablePlaylists.SelectedIndex].Data.PlaylistTitle = this.tbCurrentPlaylistName.Text;
            this.lvAvailablePlaylists.UpdateLayout();
            //this.currentBlock.ClearElement();
            this.Playlists[this.lvAvailablePlaylists.SelectedIndex].ClearElement();
            foreach (PlaylistElement elem in this.PlaylistElements)
                this.Playlists[this.lvAvailablePlaylists.SelectedIndex].AddElement(elem);
            //Save to XML ... Local or Remotely
            using (DataPikoClient.ServiceDataPikoClient sdpc = new DataPikoClient.ServiceDataPikoClient(this.Binder, this.PikoDataServiceEndPoint))
            {
                //sdpc.SaveBlock(this.Blocks[this.lvAvailableBlocks.SelectedIndex]);
                sdpc.SavePlaylist(this.Playlists[this.lvAvailablePlaylists.SelectedIndex].Data);
            }
        }

        private void btAddNewPlaylist_Click(object sender, RoutedEventArgs e)
        {
            this.currentPlayList = new Playlist("New Playlist");
            this.currentPlayList.Data.PlaylistTitle = "New Playlist";
            this.currentPlayList.Data.PlaylistType = DataPikoClient.PlayListType.Classic;
            this.Playlists.Add(this.currentPlayList);
            this.tbCurrentPlaylistName.Text = this.currentPlayList.Data.PlaylistTitle;
            this.btEditPlaylist_Click(this.btEditPlaylist, new RoutedEventArgs());
            this.tbCurrentPlaylistName.Focus();
            this.lvAvailablePlaylists.SelectedIndex = this.lvAvailablePlaylists.Items.Count - 1;
            this.PlaylistElements.Clear();
            foreach (PlaylistElement elem in this.currentPlayList.Elements)
                this.PlaylistElements.Add(elem);
            //this.dgBlockDetail.ItemsSource = this.currentBlock.Elements;


        }

        private void btEditPlaylist_Click(object sender, RoutedEventArgs e)
        {
            //Set grid to  editable
            this.dgPlaylistDetail.IsReadOnly = false;
            this.tbCurrentPlaylistName.IsReadOnly = false;
            this.btSavePlaylist.IsEnabled = true;
        }

        private void btAddFieldName_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(tbNewCustomFieldName.Text) && cbFieldType.SelectedIndex > -1)
            {
                DataPikoClient.TemplateFieldData newTemplateField = new DataPikoClient.TemplateFieldData();
                newTemplateField.FieldName = tbNewCustomFieldName.Text;

                newTemplateField.FieldType = (DataPikoClient.TemplateFieldType)cbFieldType.SelectedIndex;

                int idxCategory = cbSupportCategory.SelectedIndex;
                if (idxCategory > -1)
                {
                    List<DataPikoClient.TemplateFieldData> currentCategoriesAF = new List<DataPikoClient.TemplateFieldData>(Categories[idxCategory].TemplateFields);
                    currentCategoriesAF.Add(newTemplateField);
                    Categories[idxCategory].TemplateFields = currentCategoriesAF.ToArray();
                    //Save new categorie field ...
                    int idFieldCategoryVideo = -1;
                    using (DataPikoClient.ServiceDataPikoClient sdpc = new DataPikoClient.ServiceDataPikoClient(this.Binder, this.PikoDataServiceEndPoint))
                    {
                        int idTemplateField = sdpc.AddTemplateField(newTemplateField);
                        if (idTemplateField > -1)
                        {
                            idFieldCategoryVideo = sdpc.AddTemplateFieldToCategory(Categories[idxCategory].Id, idTemplateField);
                        }
                    }

                    AdditionalField newAdditionalField = new AdditionalField();
                    newAdditionalField.Name = "C" + Categories[idxCategory].Id + "_" + ((int)newTemplateField.FieldType).ToString() + "_" + newTemplateField.FieldName.Replace(' ', '_');
                    newAdditionalField.Tag = idFieldCategoryVideo;
                    newAdditionalField.lblFieldName.Content = newTemplateField.FieldName;

                    string fieldTypeString = "";
                    switch(newTemplateField.FieldType)
                    {
                        case DataPikoClient.TemplateFieldType.Picture:
                            fieldTypeString = "Picture";
                            break;
                        case DataPikoClient.TemplateFieldType.Video:
                            fieldTypeString = "Video";
                            break;
                        case DataPikoClient.TemplateFieldType.Text:
                            fieldTypeString = "Text";
                            break;
                    }

                    newAdditionalField.lblFieldType.Content = fieldTypeString;
                    newAdditionalField.tbValue.IsEnabled = this.tbSupportTitle.IsEnabled;
                    newAdditionalField.btDeleteField.Click += btDeleteCustomField_Click;
                    newAdditionalField.btDeleteField.Visibility = Visibility.Visible;
                    newAdditionalField.btChooseFile.Visibility = newTemplateField.FieldType == DataPikoClient.TemplateFieldType.Text ? Visibility.Hidden : Visibility.Visible;

                    spAdditionalFieldSupport.Children.Add(newAdditionalField);
                }
            }
        }

        private void btDeleteCustomField_Click(object sender, RoutedEventArgs e)
        {
            if(sender is AdditionalField)
            {
                AdditionalField aField = (AdditionalField)sender;

                //TODO When it's done remove from Database

                spAdditionalFieldSupport.Children.Remove(aField);
            }
        }

        public void RemoveCustomField()
        {

        }

        private void btEditConfig_Click(object sender, RoutedEventArgs e)
        {
            //Open Options form !
            Params paramForm = new Params();
            if (ConfigurationManager.AppSettings["Connectivity"] == "Remote")
            {
                paramForm.rbConnectivityRemote.IsChecked = true;
                paramForm.rbConnectivityLocal.IsChecked = false;
            }
            else
            {
                paramForm.rbConnectivityRemote.IsChecked = false;
                paramForm.rbConnectivityLocal.IsChecked = true;
            }

            if (ConfigurationManager.AppSettings["AutoPlayMedia"] == "True")
                paramForm.cbAutoPlayMedia.IsChecked = true;
            else
                paramForm.cbAutoPlayMedia.IsChecked = false;

            paramForm.tbRemoteURL.Text = ConfigurationManager.AppSettings["RemoteDataHost"];
            paramForm.tbLocalURL.Text = ConfigurationManager.AppSettings["LocalDataHost"];
            foreach(DataPikoClient.Category cat in this.Categories)
                paramForm.Categories.Add(cat);
            paramForm.lbCategories.UpdateLayout();
            bool? hasToSave = paramForm.ShowDialog();
            if (hasToSave.HasValue && hasToSave.Value)
            {
                /* ... Save new categories and new fields ... */
                //Save/Update new categorie field ...
                using (DataPikoClient.ServiceDataPikoClient sdpc = new DataPikoClient.ServiceDataPikoClient(this.Binder, this.PikoDataServiceEndPoint))
                {
                    List<DataPikoClient.Category> editedCategories = paramForm.Categories.ToList();
                    foreach (DataPikoClient.Category cat in editedCategories)
                    {
                        if (cat.Id == -1)
                            cat.Id = sdpc.CreateCategory(cat);
                        else
                            sdpc.CreateCategory(cat);
                    }

                    if(paramForm.categoriesToDelete.Count > 0)
                    {
                        foreach(DataPikoClient.Category catToDel in paramForm.categoriesToDelete)
                        {
                            if(catToDel.Id > 0)
                            {
                                sdpc.DeleteCategory(catToDel.Id);
                            }
                        }
                    }
                }
                this.Categories.Clear();
                this.Categories.AddRange(paramForm.Categories);
            }
        }

        private void btEditSupport_Click(object sender, RoutedEventArgs e)
        {
            //Enable Support modification
            if (this.currentSupport != null)
            {
                //Button
                this.btDeleteSupport.Visibility = Visibility.Hidden;
                this.btEditSupport.Visibility = Visibility.Hidden;
                this.btSaveSupportEdit.Visibility = Visibility.Visible;
                this.btCancelSupportEdit.Visibility = Visibility.Visible;

                //Field
                tbSupportTitle.IsEnabled = true;
                tbSupportFileName.IsEnabled = true;
                cbSupportCategory.IsEnabled = true;
                tbSupportFullPath.IsEnabled = true;

                foreach (UIElement element in spAdditionalFieldSupport.Children)
                {
                    AdditionalField supportAdditionalField = element as AdditionalField;
                    if (supportAdditionalField != null)
                    {
                        supportAdditionalField.tbValue.IsEnabled = true;
                    }
                }
            }
        }

        private void btDeleteSupport_Click(object sender, RoutedEventArgs e)
        {
            bool isDelete = false;
            //Delete Support physically then in database ...
            using (DataPikoClient.ServiceDataPikoClient sdpc = new DataPikoClient.ServiceDataPikoClient(this.Binder, this.PikoDataServiceEndPoint))
            {
                isDelete = sdpc.DeleteVideo(this.currentSupport.Data.UIdSupport, this.currentVolume.Path);
            }
            if (isDelete)
            {
                this.Supports.Remove(this.currentSupport);
                this.currentSupport = null;
                //We got supports then select first 
                if (this.Supports.Count > 0)
                    this.lvFilesInVolume.SelectedIndex = 0;
                else // Reset all fields !
                {
                    //Remove AdditionnalField controls
                    spAdditionalFieldSupport.Children.Clear();
                    //Reset fields
                    tbSupportTitle.Text = "";
                    tbSupportFileName.Text = "";
                    tbSupportFullPath.Text = "";
                    cbSupportCategory.SelectedIndex = -1;
                }
            }
        }

        private void btCancelSupportEdit_Click(object sender, RoutedEventArgs e)
        {
            //Reset all field !!

            //Button
            this.btDeleteSupport.Visibility = Visibility.Visible;
            this.btEditSupport.Visibility = Visibility.Visible;
            this.btSaveSupportEdit.Visibility = Visibility.Hidden;
            this.btCancelSupportEdit.Visibility = Visibility.Hidden;

            //Field
            tbSupportTitle.IsEnabled = false;
            tbSupportFileName.IsEnabled = false;
            cbSupportCategory.IsEnabled = false;
            tbSupportFullPath.IsEnabled = false;

            foreach (UIElement element in spAdditionalFieldSupport.Children)
            {
                AdditionalField supportAdditionalField = element as AdditionalField;
                if (supportAdditionalField != null)
                    supportAdditionalField.tbValue.IsEnabled = false;
            }
            lvFilesInVolume_SelectionChanged(null, null);
        }

        private void btExportToFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (DataPikoClient.ServiceDataPikoClient sdpc = new DataPikoClient.ServiceDataPikoClient(this.Binder, this.PikoDataServiceEndPoint))
                {
                    string xmlString = sdpc.ExportBlock(this.currentBlock.Data);
                    if (!string.IsNullOrEmpty(xmlString))
                    {
                        XElement xmltoWrite = XElement.Parse(xmlString);
                        Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                        dlg.FileName = string.IsNullOrEmpty(this.currentBlock.Data.PlaylistFileName) ? "block_" + this.currentBlock.Data.PlaylistTitle.Replace(' ', '_') : this.currentBlock.Data.PlaylistFileName; // Default file name
                        dlg.DefaultExt = ".xml"; // Default file extension
                        dlg.Filter = "Piko XML Block (.xml)|*.xml";
                        if (dlg.ShowDialog() == true)
                            xmltoWrite.Save(dlg.FileName);
                    }
                }
            }
            catch (Exception ex)
            { }
        }
        #endregion
        #region BackgroundWorker's

        private void bw_DoworkAddDetectedMedias(Object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            System.ComponentModel.BackgroundWorker worker = sender as System.ComponentModel.BackgroundWorker;
            List<DetectedMedia> listMediaToAdd = e.Argument as List<DetectedMedia>;

            for (int i = 0; (i < listMediaToAdd.Count); i++)
            {
                if ((worker.CancellationPending == true))
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    DetectedMedia mediaToAdd = listMediaToAdd[i];
                    if (mediaToAdd.State == DetectionState.ToAdd)
                    {
                        using (DataPikoClient.ServiceDataPikoClient sdpc = new DataPikoClient.ServiceDataPikoClient(this.Binder, this.PikoDataServiceEndPoint))
                        {
                            try
                            {
                                Volume currentVolume = this.PikoVolumes.Where(pv => pv.Path == mediaToAdd.Volume).FirstOrDefault();
                                Support data = currentVolume.Supports.Where(s => s.Data.UIdSupport == mediaToAdd.SupportData.UIdSupport).FirstOrDefault();
                                data.Data = sdpc.CreateVideo(mediaToAdd.SupportData);
                            }
                            catch
                            {
                                sdpc.Abort();
                            }
                        }

                    }
                    else
                    {
                        Volume currentVolume = this.PikoVolumes.Where(pv => pv.Path == mediaToAdd.Volume).FirstOrDefault();
                        Support data = currentVolume.Supports.Where(s => s.Data.UIdSupport == mediaToAdd.SupportData.UIdSupport).FirstOrDefault();
                        currentVolume.DeleteSupport(data);
                    }
                    worker.ReportProgress((i / listMediaToAdd.Count) * 100);
                }
            }

        }




        #endregion

        private void btExportPlaylistToFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (DataPikoClient.ServiceDataPikoClient sdpc = new DataPikoClient.ServiceDataPikoClient(this.Binder, this.PikoDataServiceEndPoint))
                {
                    string xmlString = sdpc.ExportPlaylist(this.currentPlayList.Data);
                    if (!string.IsNullOrEmpty(xmlString))
                    {
                        XElement xmltoWrite = XElement.Parse(xmlString);
                        Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                        dlg.FileName = string.IsNullOrEmpty(this.currentPlayList.Data.PlaylistFileName) ? "block_" + this.currentPlayList.Data.PlaylistTitle.Replace(' ', '_') : this.currentPlayList.Data.PlaylistFileName; // Default file name
                        dlg.DefaultExt = ".xml"; // Default file extension
                        dlg.Filter = "Piko XML Playlist (.xml)|*.xml";
                        if (dlg.ShowDialog() == true)
                            xmltoWrite.Save(dlg.FileName);
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        private void btSaveSupportEdit_Click(object sender, RoutedEventArgs e)
        {
            //Reset all field !!

            //Button
            this.btDeleteSupport.Visibility = Visibility.Visible;
            this.btEditSupport.Visibility = Visibility.Visible;
            this.btSaveSupportEdit.Visibility = Visibility.Hidden;
            this.btCancelSupportEdit.Visibility = Visibility.Hidden;

            //Field
            tbSupportTitle.IsEnabled = false;
            tbSupportFileName.IsEnabled = false;
            cbSupportCategory.IsEnabled = false;
            tbSupportFullPath.IsEnabled = false;

            try
            {
                this.currentSupport.Data.Title = tbSupportTitle.Text;
                this.currentSupport.Data.IdCategory = this.Categories[this.cbSupportCategory.SelectedIndex].Id;
                this.currentSupport.Data.FileName = this.tbSupportFileName.Text;
                this.currentSupport.Data.FullPath = this.tbSupportFullPath.Text;

                List<DataPikoClient.TemplateFieldValueData> currentSupportAddFields = new List<DataPikoClient.TemplateFieldValueData>();

                foreach (UIElement element in spAdditionalFieldSupport.Children)
                {
                    AdditionalField supportAdditionalField = element as AdditionalField;

                    DataPikoClient.TemplateFieldValueData newTemplateField = new DataPikoClient.TemplateFieldValueData();
                    newTemplateField.IdValue = -1;
                    newTemplateField.FieldDefinition = new DataPikoClient.TemplateFieldData();
                    newTemplateField.FieldDefinition.IdFieldCategory = (int)supportAdditionalField.Tag;
                    newTemplateField.FieldDefinition.FieldName = (string)supportAdditionalField.lblFieldName.Content;
                    string[] temp = supportAdditionalField.Name.Split('_');
                    newTemplateField.FieldDefinition.FieldType = (DataPikoClient.TemplateFieldType)(int.Parse(temp[1]));
                    newTemplateField.Value = supportAdditionalField.tbValue.Text;

                    if (supportAdditionalField != null)
                        supportAdditionalField.tbValue.IsEnabled = false;
                    currentSupportAddFields.Add(newTemplateField);
                    //AdditionalField newAdditionalField = new AdditionalField();
                    //newAdditionalField.Name = "C" + Categories[idxCategory].Id + "_" + ((int)newTemplateField.FieldType).ToString() + "_" + newTemplateField.FieldName.Replace(' ', '_');
                    //newAdditionalField.Tag = idFieldCategoryVideo;
                    //newAdditionalField.lblFieldName.Content = newTemplateField.FieldName;
                    //newAdditionalField.lblFieldType.Content = newTemplateField.FieldType == DataPikoClient.TemplateFieldType.Texte ? "Text" : "Crawl";
                    //newAdditionalField.tbValue.IsEnabled = this.tbSupportTitle.IsEnabled;


                }
                this.currentSupport.Data.TemplateFields = currentSupportAddFields.ToArray();
                using (DataPikoClient.ServiceDataPikoClient sdpc = new DataPikoClient.ServiceDataPikoClient(this.Binder, this.PikoDataServiceEndPoint))
                {
                    bool result = sdpc.SaveVideo(this.currentSupport.Data.UIdSupport,this.currentSupport.Data);
                    MessageBox.Show("Media saved");
                }
            }
            catch (Exception ex)
            { }


        }

        private void cbSupportCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Change TempalteField
            spAdditionalFieldSupport.Children.Clear();
        }

        private void lvAvailablePlaylists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(this.lvAvailablePlaylists.SelectedIndex > -1 && this.tbCurrentPlaylistName.IsReadOnly)
            {
                this.currentPlayList = this.Playlists[this.lvAvailablePlaylists.SelectedIndex];
                this.tbCurrentPlaylistName.Text = this.currentPlayList.Data.PlaylistTitle;
                this.PlaylistElements.Clear();
                foreach (PlaylistElement elem in this.currentPlayList.Elements)
                    this.PlaylistElements.Add(elem);

            }
        }

        private void lvAvailableBlocks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.lvAvailableBlocks.SelectedIndex > -1 && this.tbCurrentBlockName.IsReadOnly)
            {
                this.currentBlock = this.Blocks[this.lvAvailableBlocks.SelectedIndex];
                this.tbCurrentBlockName.Text = this.currentBlock.Data.PlaylistTitle;
                this.BlocksElements.Clear();
                foreach (PlaylistElement elem in this.currentBlock.Elements)
                    this.BlocksElements.Add(elem);
            }
        }
    }
}
