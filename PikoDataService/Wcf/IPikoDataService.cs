using Piko.XML.Data;
using PikoDataService.DB;
using PikoTrafficManager.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PikoDataService.Wcf
{
    [ServiceContract(Name = "ServiceDataPiko", Namespace = "http://developer.piko.com/PikoDataServices/")]
    public interface IPikoDataService
    {
        //Get
        [OperationContract]
        SupportData GetVideoInfo(string IdVideo);

        [OperationContract]
        Segment GetVideoSegmentInfo(string IdVideo, uint SegmentId);

        [OperationContract]
        Segment[] GetVideoSegmentList(string IdVideo);

        [OperationContract]
        Channel[] GetChannels();

        [OperationContract]
        Devices[] GetDeviceList();

        //Save & Delete
        [OperationContract]
        bool DeleteVideo(string IdVideo, string Volume);
        [OperationContract]
        bool SaveVideo(string IdVideo,SupportData Data);
        [OperationContract]
        SupportData CreateVideo(SupportData Data);
        //Block
        [OperationContract]
        PlaylistData[] GetBlocks();
        [OperationContract]
        bool SaveBlock(PlaylistData Data);
        [OperationContract]
        string ExportBlock(PlaylistData Data);
        [OperationContract]
        PlaylistData LoadBlock(string blockId);
        [OperationContract]
        bool DeleteBlock(string blockId);
        //Playlist
        [OperationContract]
        PlaylistData[] GetPlaylists();
        [OperationContract]
        bool SavePlaylist(PlaylistData Data);
        [OperationContract]
        string ExportPlaylist(PlaylistData Data);
        [OperationContract]
        PlaylistData LoadPlaylist(string playlistId);
        [OperationContract]
        bool DeletePlaylist(string playlistId);

        //Config & Enums
        [OperationContract]
        Config GetConfig(string Version = "");

        [OperationContract]
        String GetRemoteConfig();
        [OperationContract()]
        void CheckSystem();
        [OperationContract]
        VolumeData GetVolumeData(string VolumePath);
        [OperationContract]
        Category[] GetCategories();
        [OperationContract]
        int CreateCategory(Category categoryToCreate);
        [OperationContract]
        void DeleteCategory(long CategoryId);
        [OperationContract]
        int AddTemplateField(TemplateFieldData FieldDefinition);
        [OperationContract]
        int AddTemplateFieldToCategory(int IdCategory, int IdTemplateField);

    }
}
