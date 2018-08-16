using PikoTrafficManager.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PikoTrafficManager.Control
{
    public enum DetectionState
    {
        New,
        ToAdd,
        ToRemove
    }


    public class DetectedMedia
    {
        public DataPikoClient.SupportData SupportData;
        public string Volume;
        public DetectionState State = DetectionState.New;
    }


    /// <summary>
    /// Logique d'interaction pour MediaDetection.xaml
    /// </summary>
    public partial class MediaDetection : Window
    {
        public List<DetectedMedia> listNewMedia = new List<DetectedMedia>();
        public MediaDetection()
        {
            InitializeComponent();
        }

        private void btCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void btAddSelected_Click(object sender, RoutedEventArgs e)
        {
            int i = this.spNewFilesInVolumes.Children.Count - 1;
            while(i > -1)
            {
                NewMediaDetectionItem element = this.spNewFilesInVolumes.Children[i] as NewMediaDetectionItem;
                string SupportUID = (string)element.lblMediaUID.Content;
                DetectedMedia currentSupport = listNewMedia.Where(lmm => lmm.SupportData.UIdSupport == SupportUID).FirstOrDefault();
                if (element.chkbxSelected.IsChecked.HasValue && element.chkbxSelected.IsChecked.Value)
                {
                    currentSupport.State = DetectionState.ToAdd;
                }
                else
                {
                    currentSupport.State = DetectionState.New;
                }
                i--;
            }           
            this.DialogResult = true;
            this.Close();
        }

        private void MediaDetectionForm_Loaded(object sender, RoutedEventArgs e)
        {
            foreach(DetectedMedia media in this.listNewMedia)
            {
                NewMediaDetectionItem item = new NewMediaDetectionItem();
                item.lblMediaUID.Content = media.SupportData.UIdSupport;
                item.lblMediaUID.ToolTip = media.SupportData.UIdSupport;
                item.lblMediaVolume.Content = media.Volume;
                item.lblMediaVolume.ToolTip = media.Volume;
                this.spNewFilesInVolumes.Children.Add(item);
            }
        }
    }
}
