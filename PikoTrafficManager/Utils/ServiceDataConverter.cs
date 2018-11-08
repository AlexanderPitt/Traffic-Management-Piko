using PikoTrafficManager.Data;
using PikoTrafficManager.DataPikoClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PikoTrafficManager.Utils
{
    public static class ServiceDataConverter
    {
        //public static PikoTrafficManager.Data.SupportData ConvertServiceSupportDataToSupportData(PikoTrafficManager.DataPikoClient.SupportData data)
        //{
        //    PikoTrafficManager.Data.SupportData result = null;

        //    result.UIdSupport = data.UIdSupport;
        //    result.Duration = data.Duration;
        //    result.Eom = data.Eom;
        //    result.Extension = data.Extension;
        //    result.FileName = data.FileName;
        //    result.FileSize = data.FileSize;
        //    result.FullPath = data.FullPath;
        //    result.Height = data.Height;
        //    result.IdCategory = data.IdCategory;
        //    result.TcStart = data.TcStart;
        //    result.IsExist = data.IsExist;
        //    result.FrameRate = (Data.FrameRate)((int)data.FrameRate);
        //    result.Title = data.Title;
        //    result.Width = data.Width;
        //    result.TemplateFields = data.TemplateFields.Select(rf => new Piko.XML.Data.TemplateFieldValueData()
        //    {
        //        Value = rf.Value,
        //        IdValue = rf.IdValue,
        //        FieldDefinition = new Piko.XML.Data.TemplateFieldData()
        //        {
        //            FieldName = rf.FieldDefinition.FieldName,
        //            FieldType = (Piko.XML.Data.TemplateFieldType)((int)rf.FieldDefinition.FieldType),
        //            Id = rf.FieldDefinition.Id,
        //            IdFieldCategory = rf.FieldDefinition.IdFieldCategory
        //        }
        //    }).ToArray();

        //    return result;
        //}

        ////Playlist
        //public static PikoTrafficManager.Data.PlaylistData ConvertServicePlaylistDataToPlayListData(PikoTrafficManager.DataPikoClient.PlaylistData data)
        //{
        //    PikoTrafficManager.Data.PlaylistData result = new Data.PlaylistData();


        //    return result;
        //}

        //public static PikoTrafficManager.DataPikoClient.PlaylistData ConvertPlayListDataToServicePlayListData(Data.PlaylistData data)
        //{
        //    DataPikoClient.PlaylistData result = new DataPikoClient.PlaylistData();

        //    return result;
        //}
    }
}
