using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using PikoDataService.DB;
using System.Data.OleDb;
using PikoTrafficManager.Data;
using Piko.XML.Data;
using Piko.XML;
using Piko.XML.Element;
using System.Xml.Linq;

namespace PikoDataService.Wcf
{
    [ServiceBehavior(Name = "ServiceDataPiko", Namespace = "http://developer.piko.com/PikoDataServices/", InstanceContextMode = InstanceContextMode.PerCall)]
    public partial class ServiceDataPiko : IPikoDataService
    {
        public Channel[] GetChannels()
        {
            List<Channel> channelList = new List<Channel>();
            try
            {
                using (PikoDataContext dataContext = new PikoDataContext(PikoDataServiceApp.DatabasePath, "PikoServer"))
                {
                    OleDbDataReader result = dataContext.Select("SELECT CHANNEL_NAME, LAST_HASRUN_DATE FROM Channels");
                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            Channel AChannel = new Channel();
                            int idxColumn = result.GetOrdinal("CHANNEL_NAME");
                            AChannel.ChannelName = result.GetString(idxColumn);
                            idxColumn = result.GetOrdinal("LAST_HASRUN_DATE");
                            AChannel.LastHasRunDate = result.GetDateTime(idxColumn);
                            channelList.Add(AChannel);
                        }
                    }
                    result.Close();
                }
            }
            catch (Exception ex)
            { }
            return channelList.ToArray();
        }

        public Config GetConfig(string Version = "")
        {
            Config pikoConfig = null;
            try
            {
                using (PikoDataContext dataContext = new PikoDataContext(PikoDataServiceApp.DatabasePath, "PikoServer"))
                {
                    String sqlQuery = "SELECT VERSION,SUPPORTS_ID,HASRUNS_ID FROM Config";
                    if (!String.IsNullOrEmpty(Version))
                        sqlQuery += " WHERE VERSION = '" + Version + "'";
                    OleDbDataReader result = dataContext.Select(sqlQuery);
                    if (result.HasRows)
                    {
                        pikoConfig = new Config();
                        int idxColumn = result.GetOrdinal("VERSION");
                        pikoConfig.Version = (ulong)result.GetInt64(idxColumn);
                        idxColumn = result.GetOrdinal("SUPPORTS_ID");
                        pikoConfig.SupportsId = result.GetString(idxColumn);
                        idxColumn = result.GetOrdinal("HASRUNS_ID");
                        pikoConfig.HasRunsId = result.GetString(idxColumn);
                    }
                    result.Close();
                }
            }
            catch (Exception ex)
            { }
            return pikoConfig;
        }

        public Devices[] GetDeviceList()
        {
            List<Devices> deviceList = new List<Devices>();
            try
            {
                using (PikoDataContext dataContext = new PikoDataContext(PikoDataServiceApp.DatabasePath, "PikoServer"))
                {
                    OleDbDataReader result = dataContext.Select("SELECT DEVICE,DEVICE-TYPE FROM Devices");
                    if (result.HasRows)
                    {
                        while (result.Read())
                        {
                            Devices ADevice = new Devices();
                            int idxColumn = result.GetOrdinal("DEVICE");
                            ADevice.Device = result.GetString(idxColumn);
                            idxColumn = result.GetOrdinal("DEVICE-TYPE");
                            ADevice.DeviceType = (uint)result.GetInt32(idxColumn);
                            deviceList.Add(ADevice);
                        }
                    }
                    result.Close();
                }
            }
            catch (Exception ex)
            { }
            return deviceList.ToArray();
        }

        public SupportData GetVideoInfo(string IdVideo)
        {
            SupportData videoInfo = null;
            try
            {
                using (PikoDataContext dataContext = new PikoDataContext(PikoDataServiceApp.DatabasePath, "PikoServer"))
                {
                    String sqlQuery = "SELECT ID, EXTENSION,TC_START,EOM,DURATION,WIDTH,HEIGHT,FRAMERATE,AUDIO_COUNT,FILE_SIZE,FILE_NAME,STATE,SUPPORT_ID,ID_CATEGORY,TITLE FROM Videos WHERE Id = '" + IdVideo + "'";
                    OleDbDataReader result = dataContext.Select(sqlQuery);
                    if (result.HasRows)
                    {
                        if (result.Read())
                        {
                            videoInfo = new SupportData();
                            int idxColumn = result.GetOrdinal("ID");
                            videoInfo.UIdSupport = result.GetString(idxColumn);
                            if (videoInfo.UIdSupport == null)
                                videoInfo.UIdSupport = IdVideo;
                            idxColumn = result.GetOrdinal("EXTENSION");
                            videoInfo.Extension = result.GetString(idxColumn);
                            if (videoInfo.Extension == null)
                                videoInfo.Extension = "";
                            idxColumn = result.GetOrdinal("TC_START");
                            videoInfo.TcStart = (ulong)result.GetInt32(idxColumn);
                            idxColumn = result.GetOrdinal("FILE_NAME");
                            videoInfo.FileName = result.GetString(idxColumn);
                            if (videoInfo.FileName == null)
                                videoInfo.FileName = "";
                            videoInfo.IsExist = true;
                            idxColumn = result.GetOrdinal("FILE_SIZE");
                            videoInfo.FileSize = long.Parse(result.GetString(idxColumn));
                            idxColumn = result.GetOrdinal("ID_CATEGORY");
                            videoInfo.IdCategory = result.GetInt32(idxColumn);
                            videoInfo.FullPath = videoInfo.FileName + videoInfo.Extension;
                            idxColumn = result.GetOrdinal("TITLE");
                            videoInfo.Title = result.GetString(idxColumn);

                            result.Close();

                            sqlQuery = "SELECT TFC.ID, TFC.ID_TEMPLATE_FIELD, TF.LABEL AS LABEL_TEMPLATE_FIELD,TF.ID_FIELD_TYPE " + Environment.NewLine;
                            sqlQuery += "FROM TEMPLATE_FIELD AS TF " + Environment.NewLine + "INNER JOIN TEMPLATE_FIELD_CATEGORY AS TFC ON TF.ID = TFC.ID_TEMPLATE_FIELD " + Environment.NewLine;
                            sqlQuery += "WHERE ID_CATEGORY = " + videoInfo.IdCategory.ToString();
                            //sqlQuery += "LEFT JOIN TEMPLATE_FIELD_VIDEO_VALUES AS TFVV ON TFC.ID = TFVV.ID_TEMPLATE_FIELD " + Environment.NewLine;
                            //sqlQuery += "WHERE TFVV.ID_VIDEO = '" + IdVideo + "' AND ID_CATEGORY = " + videoInfo.IdCategory.ToString();
                            result = dataContext.Select(sqlQuery);
                            if (result.HasRows)
                            {
                                List<TemplateFieldValueData> listTFData = new List<TemplateFieldValueData>();
                                while (result.Read())
                                {
                                    idxColumn = result.GetOrdinal("ID");
                                    int idTfc = result.GetInt32(idxColumn);
                                    //TFVV.ID AS ID_VALUE,TFVV.VALUE,TFVV.ID_VIDEO
                                    string sqlQuery2 = "SELECT ID,FIELD_VALUE,ID_VIDEO " + Environment.NewLine;
                                    sqlQuery2 += "FROM TEMPLATE_FIELD_VIDEO_VALUES " + Environment.NewLine;
                                    sqlQuery2 += "WHERE ID_VIDEO = '" + IdVideo + "' AND ID_TEMPLATE_FIELD_CATEGORY = " + idTfc.ToString();

                                    OleDbDataReader result2 = dataContext.Select(sqlQuery2);
                                    TemplateFieldValueData tfValueData = new TemplateFieldValueData();
                                    //FIELD DEFINITION
                                    tfValueData.FieldDefinition = new TemplateFieldData();
                                    idxColumn = result.GetOrdinal("ID");
                                    tfValueData.FieldDefinition.Id = result.GetInt32(idxColumn);
                                    tfValueData.FieldDefinition.IdFieldCategory = idTfc;
                                    idxColumn = result.GetOrdinal("LABEL_TEMPLATE_FIELD");
                                    tfValueData.FieldDefinition.FieldName = result.GetString(idxColumn);
                                    idxColumn = result.GetOrdinal("ID_FIELD_TYPE");
                                    tfValueData.FieldDefinition.FieldType = (TemplateFieldType)result.GetInt32(idxColumn);
                                    if (result2.HasRows && result2.Read())
                                    {                                                                               
                                        //VALUE
                                        idxColumn = result2.GetOrdinal("FIELD_VALUE");
                                        tfValueData.Value = result2.GetString(idxColumn);
                                    }
                                    else
                                        tfValueData.Value = "";
                                    result2.Close();
                                    listTFData.Add(tfValueData);
                                }
                                videoInfo.TemplateFields = listTFData.ToArray();
                            }
                            else
                            {
                                videoInfo.TemplateFields = new TemplateFieldValueData[0];
                            }
                            result.Close();
                        }

                    }
                    
                }
            }
            catch (Exception ex)
            {

            }
            if(videoInfo == null)
            {
                videoInfo = new SupportData();
                videoInfo.TemplateFields = new TemplateFieldValueData[0];
                videoInfo.FullPath = "";
                videoInfo.Title = "";
                videoInfo.FileName = "";
                videoInfo.Extension = "";
                videoInfo.UIdSupport = "";
            }           
            return videoInfo;
        }

        public Segment GetVideoSegmentInfo(string IdVideo, uint SegmentId)
        {
            throw new NotImplementedException();
        }

        public Segment[] GetVideoSegmentList(string IdVideo)
        {
            throw new NotImplementedException();
        }

        public String GetRemoteConfig()
        {

            // INIFile pikoVideoServer = new INIFile(PikoDataServiceApp.PikoIniPath);
            //Get configuration of local Piko ...
            return System.IO.File.ReadAllText(PikoDataServiceApp.PikoIniPath);
        }

        public void CheckSystem()
        {
            try
            {
                using (PikoDataContext dataContext = new PikoDataContext(PikoDataServiceApp.DatabasePath, "PikoServer"))
                {
                    // CHECK IF DATABASE IS OK
                    String sqlQuery = "SELECT COUNT(Column_Name) as AS_CATEGORY FROM sys.columns WHERE name='Video' AND Column_Name = 'ID_CATEGORY'";
                    if (!dataContext.CheckSchema("ID_CATEGORY", "Videos"))
                    {
                        //CATEGORIES
                        sqlQuery = "CREATE TABLE CATEGORIES(ID Integer CONSTRAINT PK_CATEGORY PRIMARY KEY,LABEL Text )";
                        dataContext.Execute(sqlQuery);
                        sqlQuery = "INSERT INTO CATEGORIES(ID,LABEL) VALUES(0,'Default')";
                        dataContext.InsertData(sqlQuery);
                        sqlQuery = "ALTER TABLE Videos ADD COLUMN ID_CATEGORY Integer default 0";
                        dataContext.Execute(sqlQuery);
                        sqlQuery = "ALTER TABLE Videos ADD CONSTRAINT FK_CATEGORY FOREIGN KEY(ID_CATEGORY) REFERENCES CATEGORIES(ID)";
                        dataContext.Execute(sqlQuery);


                        //TEMPLATE FIELDS
                        sqlQuery = "CREATE TABLE TEMPLATE_FIELD(ID Integer CONSTRAINT PK_TEMPLATE_FIELD PRIMARY KEY, LABEL Text, ID_FIELD_TYPE Integer)";
                        dataContext.Execute(sqlQuery);
                        sqlQuery = "CREATE TABLE TEMPLATE_FIELD_TYPE(ID Integer CONSTRAINT PK_TEMPLATE_FIELD_TYPE PRIMARY KEY, LABEL Text)";
                        dataContext.Execute(sqlQuery);
                        sqlQuery = "INSERT INTO TEMPLATE_FIELD_TYPE(ID,LABEL) VALUES(0,'Texte')";
                        dataContext.InsertData(sqlQuery);
                        sqlQuery = "INSERT INTO TEMPLATE_FIELD_TYPE(ID,LABEL) VALUES(1,'Picture')";
                        dataContext.InsertData(sqlQuery);
                        sqlQuery = "INSERT INTO TEMPLATE_FIELD_TYPE(ID,LABEL) VALUES(2,'Video')";
                        dataContext.InsertData(sqlQuery);
                        sqlQuery = "ALTER TABLE TEMPLATE_FIELD ADD CONSTRAINT FK_TEMPLATE_FIELD_TYPE FOREIGN KEY(ID_FIELD_TYPE) REFERENCES TEMPLATE_FIELD_TYPE(ID)";
                        dataContext.Execute(sqlQuery);
                        sqlQuery = "CREATE TABLE TEMPLATE_FIELD_CATEGORY(ID Integer CONSTRAINT PK_TEMPLATE_FIELD_CATEGORY PRIMARY KEY,ID_CATEGORY Integer,ID_TEMPLATE_FIELD Integer)";
                        dataContext.Execute(sqlQuery);
                        sqlQuery = "ALTER TABLE TEMPLATE_FIELD_CATEGORY ADD CONSTRAINT FK_TEMPLATE_FIELD_CATEGORY_CATEGORY FOREIGN KEY(ID_CATEGORY) REFERENCES CATEGORIES(ID)";
                        dataContext.Execute(sqlQuery);
                        sqlQuery = "ALTER TABLE TEMPLATE_FIELD_CATEGORY ADD CONSTRAINT FK_TEMPLATE_FIELD_TEMPLATE_FIELD FOREIGN KEY(ID_TEMPLATE_FIELD) REFERENCES TEMPLATE_FIELD(ID)";
                        dataContext.Execute(sqlQuery);

                        //TEMPLATE FIELD VALUES
                        sqlQuery = "CREATE TABLE TEMPLATE_FIELD_VIDEO_VALUES(ID Integer CONSTRAINT PK_TEMPLATE_FIELD_VIDEO_VALUES PRIMARY KEY, FIELD_VALUE Text, ID_VIDEO Text, ID_TEMPLATE_FIELD_CATEGORY Integer)";
                        dataContext.Execute(sqlQuery);
                        sqlQuery = "ALTER TABLE TEMPLATE_FIELD_VIDEO_VALUES ADD CONSTRAINT FK_TEMPLATE_FIELD_VIDEO_VALUES_VIDEO FOREIGN KEY(ID_VIDEO) REFERENCES Videos(ID)";
                        dataContext.Execute(sqlQuery);
                        sqlQuery = "ALTER TABLE TEMPLATE_FIELD_VIDEO_VALUES ADD CONSTRAINT FK_TFVV_TFC FOREIGN KEY(ID_TEMPLATE_FIELD_CATEGORY) REFERENCES TEMPLATE_FIELD_CATEGORY(ID)";
                        dataContext.Execute(sqlQuery);
                    }

                    if(!dataContext.CheckSchema("TITLE","Videos"))
                    {
                        sqlQuery = "ALTER TABLE Videos ADD COLUMN TITLE Text";
                        dataContext.Execute(sqlQuery);
                    }

                }
            }
            catch (Exception ex)
            { }
        }

        public VolumeData GetVolumeData(string VolumePath)
        {
            if (System.IO.Directory.Exists(VolumePath))
            {
                Piko.XML.Element.Volume volume = Piko.XML.Element.VolumeManager.LoadVolume(VolumePath);
                //volume.Data.SupportsData = volume.Supports
                List<SupportData> supportData = new List<SupportData>();
                foreach (Support sup in volume.Supports)
                {
                    SupportData data = sup.Data;
                    data = this.GetVideoInfo(sup.Data.UIdSupport);
                    if (data != null && !String.IsNullOrEmpty(data.UIdSupport))
                    {
                        sup.Data.IsExist = true;
                        sup.Data.Title = data.Title;
                        sup.Data.TemplateFields = data.TemplateFields;
                    }
                    else
                    {                        
                        sup.Data.TemplateFields = new TemplateFieldValueData[0];
                        if (string.IsNullOrEmpty(sup.Data.FileName))
                            sup.Data.FileName = "";
                        sup.Data.UIdSupport = sup.Data.FileName;
                        sup.Data.IsExist = false;                        
                    }
                    if (string.IsNullOrEmpty(sup.Data.Title))
                        sup.Data.Title = "";

                    supportData.Add(sup.Data);
                    
                }
                volume.Data.SupportsData = supportData.ToArray();
                return volume.Data;
            }
            return null;
        }

        public Category[] GetCategories()
        {
            List<Category> result = new List<Category>();
            using (PikoDataContext dataContext = new PikoDataContext(PikoDataServiceApp.DatabasePath, "PikoServer"))
            {
                // CHECK IF DATABASE IS OK
                String sqlQuery = "SELECT ID, LABEL FROM CATEGORIES";
                OleDbDataReader queryData = dataContext.Select(sqlQuery);
                if (queryData.HasRows)
                {
                    bool hasRecord = queryData.Read();
                    while (hasRecord)
                    {
                        Category newCatagory = new Category();
                        int idxColumn = queryData.GetOrdinal("ID");
                        newCatagory.Id = queryData.GetInt32(idxColumn);
                        idxColumn = queryData.GetOrdinal("LABEL");
                        newCatagory.Name = queryData.GetString(idxColumn);

                        string queryTemplateField = "SELECT TFC.ID AS ID_TFC, TF.LABEL AS LABEL_FIELD, TF.ID_FIELD_TYPE AS TYPE_FIELD " + Environment.NewLine
                                                    + " FROM TEMPLATE_FIELD_CATEGORY AS TFC INNER JOIN TEMPLATE_FIELD AS TF ON TFC.ID_TEMPLATE_FIELD=TF.ID" + Environment.NewLine
                                                    + " WHERE TFC.ID_CATEGORY = " + newCatagory.Id;
                        List<TemplateFieldData> categoryTfDatas = new List<TemplateFieldData>();
                        OleDbDataReader tfFieldData = dataContext.Select(queryTemplateField);
                        if (tfFieldData.HasRows)
                        {
                            bool hasTfData = true;
                            while (tfFieldData.Read())
                            {
                                TemplateFieldData dataTF = new TemplateFieldData();
                                idxColumn = tfFieldData.GetOrdinal("ID_TFC");
                                dataTF.Id = tfFieldData.GetInt32(idxColumn);
                                idxColumn = tfFieldData.GetOrdinal("LABEL_FIELD");
                                dataTF.FieldName = tfFieldData.GetString(idxColumn);
                                idxColumn = tfFieldData.GetOrdinal("TYPE_FIELD");
                                int typeId = tfFieldData.GetInt32(idxColumn);
                                dataTF.FieldType = (TemplateFieldType)typeId;
                                dataTF.IdFieldCategory = dataTF.Id;
                                //hasTfData = tfFieldData.Read();
                                categoryTfDatas.Add(dataTF);
                            }
                        }
                        tfFieldData.Close();
                        newCatagory.TemplateFields = categoryTfDatas.ToArray();
                        result.Add(newCatagory);
                        hasRecord = queryData.Read();
                    }
                    queryData.Close();
                }
            }
            return result.ToArray();
        }

        public Category GetCategory(long IdCategory)
        {
            Category result = null;
            using (PikoDataContext dataContext = new PikoDataContext(PikoDataServiceApp.DatabasePath, "PikoServer"))
            {
                // CHECK IF DATABASE IS OK
                String sqlQuery = "SELECT ID, LABEL FROM CATEGORIES WHERE ID = " + IdCategory;
                OleDbDataReader queryData = dataContext.Select(sqlQuery);
                if (queryData.HasRows)
                {
                    queryData.Read();
                    result = new Category();
                    int idxColumn = queryData.GetOrdinal("ID");
                    result.Id = queryData.GetInt32(idxColumn);
                    idxColumn = queryData.GetOrdinal("LABEL");
                    result.Name = queryData.GetString(idxColumn);

                    string queryTemplateField = "SELECT TFC.ID AS ID_TFC, TF.LABEL AS LABEL_FIELD, TF.ID_FIELD_TYPE AS TYPE_FIELD " + Environment.NewLine
                                                + " FROM TEMPLATE_FIELD_CATEGORY AS TFC INNER JOIN TEMPLATE_FIELD AS TF ON TFC.ID_TEMPLATE_FIELD=TF.ID" + Environment.NewLine
                                                + " WHERE TFC.ID_CATEGORY = " + IdCategory;
                    List<TemplateFieldData> categoryTfDatas = new List<TemplateFieldData>();
                    OleDbDataReader tfFieldData = dataContext.Select(queryTemplateField);
                    if (tfFieldData.HasRows)
                    {
                        bool hasTfData = true;
                        while (hasTfData)
                        {
                            TemplateFieldData dataTF = new TemplateFieldData();
                            idxColumn = queryData.GetOrdinal("ID_TFC");
                            dataTF.Id = queryData.GetInt32(idxColumn);
                            idxColumn = queryData.GetOrdinal("LABEL_FIELD");
                            dataTF.FieldName = queryData.GetString(idxColumn);
                            idxColumn = queryData.GetOrdinal("TYPE_FIELD");
                            int typeId = queryData.GetInt32(idxColumn);
                            dataTF.FieldType = (TemplateFieldType)typeId;
                            hasTfData = tfFieldData.Read();
                        }
                    }
                    tfFieldData.Close();
                    result.TemplateFields = categoryTfDatas.ToArray();                    
                    queryData.Close();
                }
            }
            return result;
        }

        public int CreateCategory(Category categoryToCreate)
        {
            int categoryId = -1;
            using (PikoDataContext dataContext = new PikoDataContext(PikoDataServiceApp.DatabasePath, "PikoServer"))
            {
                if(categoryToCreate.Id == -1)
                {
                    String sqlQuery = "SELECT MAX(ID) FROM CATEGORIES";
                    OleDbDataReader queryData = dataContext.Select(sqlQuery);
                    if (queryData.HasRows && queryData.Read())
                    {
                        int idxColumn = queryData.GetOrdinal("ID");
                        categoryToCreate.Id = queryData.GetInt32(idxColumn) + 1;
                        queryData.Close();
                        sqlQuery = "INSERT INTO CATEGORIES(ID,LABEL) VALUES(" + categoryToCreate.Id + ",'" + categoryToCreate.Name +"')";
                        dataContext.Execute(sqlQuery);
                    }
                }
                else
                {
                    //UPDATE
                    String sqlQuery = "UPDATE CATEGORIES SET " + Environment.NewLine;
                    sqlQuery += "LABEL='" + categoryToCreate.Name + "'" + Environment.NewLine;
                    sqlQuery += "WHERE ID=" + categoryToCreate.Id;
                    dataContext.Execute(sqlQuery);

                }

                if(categoryToCreate.TemplateFields != null && categoryToCreate.TemplateFields.Length > 0)
                {
                    Category currentDbCategory = this.GetCategory(categoryToCreate.Id);

                    foreach(TemplateFieldData tfData in categoryToCreate.TemplateFields)
                    {
                        if(tfData.Id == -1)
                        {
                            if (!this.TemplateFieldExists(tfData.FieldName, tfData.FieldType))
                                this.AddTemplateField(tfData);
                        }
                        else
                        {
                            //Update template field
                            this.UpdateTemplateField(tfData.Id, tfData.FieldName, tfData.FieldType);
                        }
                    }
                }

            }
            return categoryId;
        }

        public void DeleteCategory(long CategoryId)
        {
            using (PikoDataContext dataContext = new PikoDataContext(PikoDataServiceApp.DatabasePath, "PikoServer"))
            {
                String sqlQuery = "SELECT * FROM CATEGORIES WHERE ID = " + CategoryId;
                OleDbDataReader queryData = dataContext.Select(sqlQuery);
                if (queryData.HasRows && queryData.Read())
                {
                    string queryTemplateField = "SELECT TFC.TEMPLATE_FIELD_ID AS ID_TF " + Environment.NewLine
                                               + " FROM TEMPLATE_FIELD_CATEGORY AS TFC INNER JOIN TEMPLATE_FIELD AS TF ON TFC.ID_TEMPLATE_FIELD=TF.ID" + Environment.NewLine
                                               + " WHERE TFC.ID_CATEGORY = " + CategoryId;
                    List<TemplateFieldData> categoryTfDatas = new List<TemplateFieldData>();
                    OleDbDataReader tfFieldData = dataContext.Select(queryTemplateField);
                    if (tfFieldData.HasRows)
                    {
                        while(tfFieldData.Read())
                        {

                            int idxColumn = queryData.GetOrdinal("ID_TF");
                            long tfFieldId = queryData.GetInt32(idxColumn);
                            this.DeleteTemplateField(tfFieldId);
                        }
                    }
                }                   
            }
        }

        public bool DeleteVideo(string IdVideo, string Volume)
        {
            bool result = false;
            using (PikoDataContext dataContext = new PikoDataContext(PikoDataServiceApp.DatabasePath, "PikoServer"))
            {
                SupportData data = GetVideoInfo(IdVideo);
                dataContext.Execute("DELETE FROM Videos WHERE ID ='" + IdVideo + "'");
                System.IO.File.Delete(System.IO.Path.Combine(Volume, data.FileName + data.Extension));
                result = true;
            }
            return result;
        }

        public bool SaveVideo(string IdVideo, SupportData Data)
        {
            using (PikoDataContext dataContext = new PikoDataContext(PikoDataServiceApp.DatabasePath, "PikoServer"))
            {
                if (Data.IsExist)
                {
                    //Purge TEMPLATE_FIELD
                    String sqlQuery = "DELETE FROM TEMPLATE_FIELD_VIDEO_VALUES WHERE ID_VIDEO ='" + Data.UIdSupport + "'";
                    dataContext.Execute(sqlQuery);

                    //UPDATE
                    sqlQuery = "UPDATE Videos SET " + Environment.NewLine;
                    sqlQuery += "ID='" + Data.UIdSupport + "'," + Environment.NewLine;
                    sqlQuery += "EXTENSION='" + Data.Extension + "'," + Environment.NewLine;
                    sqlQuery += "TC_START=" + Data.TcStart.ToString() + "," + Environment.NewLine;
                    sqlQuery += "EOM=" + Data.Eom.ToString() + "," + Environment.NewLine;
                    sqlQuery += "DURATION=" + Data.Duration.ToString() + "," + Environment.NewLine;
                    sqlQuery += "WIDTH=" + Data.Width.ToString() + "," + Environment.NewLine;
                    sqlQuery += "HEIGHT=" + Data.Height.ToString() + "," + Environment.NewLine;
                    sqlQuery += "FRAMERATE=" + (int)Data.FrameRate + "," + Environment.NewLine;
                    //sqlQuery += "AUDIO_COUNT=" + Data.AudioCount.ToString() + Environment.NewLine;
                    sqlQuery += "FILE_SIZE=" + Data.FileSize.ToString() + "," + Environment.NewLine;
                    sqlQuery += "FILE_NAME='" + Data.FileName + "'," + Environment.NewLine;
                    //sqlQuery += "STATE=" + Data.State + Environment.NewLine;
                    //sqlQuery += "SUPPORT_ID=" + Environment.NewLine;
                    sqlQuery += "TITLE='" + Data.Title + "'," + Environment.NewLine;
                    sqlQuery += "ID_CATEGORY=" + Data.IdCategory.ToString() + Environment.NewLine;
                    sqlQuery += " WHERE ID='" + IdVideo + "'";
                    dataContext.Execute(sqlQuery);

                }
                else
                {
                    //INSERT
                    CreateVideo(Data);
                }
                //INSERT NEW TEMPLATE FIELD VALUE
                foreach (TemplateFieldValueData TFData in Data.TemplateFields)
                    AddTemplateFieldValueForVideo(Data.UIdSupport, Data.IdCategory, TFData);
            }
            return true;
        }

        public SupportData CreateVideo(SupportData Data)
        {
            SupportData result = Data;
            using (PikoDataContext dataContext = new PikoDataContext(PikoDataServiceApp.DatabasePath, "PikoServer"))
            {
                String sqlQuery = "INSERT INTO Videos(ID,EXTENSION,TC_START,EOM,DURATION,WIDTH,HEIGHT,FRAMERATE,AUDIO_COUNT,FILE_SIZE,FILE_NAME,STATE,SUPPORT_ID,ID_CATEGORY,TITLE)" + Environment.NewLine;
                sqlQuery += "VALUES('" + Data.UIdSupport + "','" + Data.Extension + "'," + Data.TcStart + "," + Data.Eom + "," + Data.Duration + "," + Data.Width + "," + Data.Height + "," + (int)Data.FrameRate + "," + 0 + ",'" + Data.FileSize + "','" + Data.FileName + "'," + 0 + ",'0'," + Data.IdCategory + ",'" + Data.Title + "')";
                dataContext.Execute(sqlQuery);
                result = GetVideoInfo(Data.UIdSupport);
            }
            return result;
        }

        public bool SaveBlock(PlaylistData Data)
        {
            try
            {
                Playlist block = new Playlist(Data);
                PikoXML.SaveXML(block, System.IO.Path.Combine(PikoDataServiceApp.PikoPlaylistPath, Data.PlaylistFileName + ".xml"));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public PlaylistData LoadBlock(string blockId)
        {
            try
            {
                PlaylistData xmlPlayList = PikoXML.LoadXML(System.IO.Path.Combine(PikoDataServiceApp.PikoBlockPath, blockId + ".xml"));
                return xmlPlayList;
            }
            catch
            {
                return null;
            }
        }

        public bool DeleteBlock(string blockId)
        {
            try
            {
                string blockxmlPath = System.IO.Path.Combine(PikoDataServiceApp.PikoBlockPath, blockId + ".xml");
                if (System.IO.File.Exists(blockxmlPath))
                    System.IO.File.Delete(blockxmlPath);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public PlaylistData[] GetBlocks()
        {
            try
            {
                List<PlaylistData> blocksList = new List<PlaylistData>();
                string[] blockFiles = System.IO.Directory.GetFiles(PikoDataServiceApp.PikoBlockPath);
                foreach (string FileName in blockFiles)
                {
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(FileName);
                    if(!string.IsNullOrEmpty(fileInfo.Extension) && fileInfo.Extension == ".xml")
                    {
                        PlaylistData ABlock = Piko.XML.PikoXML.LoadXML(FileName);
                        if (ABlock != null)
                            blocksList.Add(ABlock);
                    }
                }
                return blocksList.ToArray();
            }
            catch
            {
                return new PlaylistData[0];
            }
        }

        public PlaylistData[] GetPlaylists()
        {
            try
            {
                List<PlaylistData> playlistsList = new List<PlaylistData>();
                string[] playlistFiles = System.IO.Directory.GetFiles(PikoDataServiceApp.PikoPlaylistPath);
                foreach (string FileName in playlistFiles)
                {
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(FileName);
                    if (!string.IsNullOrEmpty(fileInfo.Extension) && fileInfo.Extension == ".xml")
                    {
                        PlaylistData APlaylist = Piko.XML.PikoXML.LoadXML(FileName);
                        if (APlaylist != null)
                            playlistsList.Add(APlaylist);
                    }
                }
                return playlistsList.ToArray();
            }
            catch
            {
                return new PlaylistData[0];
            }
        }
        public bool SavePlaylist(PlaylistData Data)
        {
            try
            {
                Playlist playlist = new Playlist(Data);
                PikoXML.SaveXML(playlist, System.IO.Path.Combine(PikoDataServiceApp.PikoPlaylistPath, Data.PlaylistTitle + ".xml"));
                return true;
            }
            catch
            {
                return false;
            }

        }

        public PlaylistData LoadPlaylist(string playlistId)
        {
            try
            {
                PlaylistData xmlPlayList = PikoXML.LoadXML(System.IO.Path.Combine(PikoDataServiceApp.PikoPlaylistPath, playlistId + ".xml"));
                return xmlPlayList;
            }
            catch
            {
                return null;
            }
        }

        public bool DeletePlaylist(string playlistId)
        {
            try
            {
                string blockxmlPath = System.IO.Path.Combine(PikoDataServiceApp.PikoPlaylistPath, playlistId + ".xml");
                if (System.IO.File.Exists(blockxmlPath))
                    System.IO.File.Delete(blockxmlPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int AddTemplateField(TemplateFieldData FieldDefinition)
        {
            return _AddTemplateField(FieldDefinition);
        }

        public int AddTemplateFieldToCategory(int IdCategory, int IdTemplateField)
        {
            return _AddTemplateFieldToCategory(IdTemplateField, IdCategory);
        }

        #region PRIVATE FUNCTIONS
        private void AddTemplateFieldValueForVideo(string IdVideo, int IdCategory, TemplateFieldValueData ValueData)
        {
            //Test if Field Exist for this category
            int idFieldCategory = ValueData.FieldDefinition.IdFieldCategory;
            if (idFieldCategory == -1)
            {
                int idTemplateField = ValueData.FieldDefinition.Id;
                if (idTemplateField == -1)
                {
                    idTemplateField = _AddTemplateField(ValueData.FieldDefinition);
                }
                if (idFieldCategory == -1)
                {
                    idFieldCategory = _AddTemplateFieldToCategory(idTemplateField, IdCategory);
                }
            }
            using (PikoDataContext dataContext = new PikoDataContext(PikoDataServiceApp.DatabasePath, "PikoServer"))
            {
                String sqlQuery = "";
                if (ValueData.IdValue == -1)
                {
                    sqlQuery = "SELECT MAX(ID) + 1 AS NEW_ID FROM TEMPLATE_FIELD_VIDEO_VALUES";
                    OleDbDataReader resultData = dataContext.Select(sqlQuery);
                    int newId = 1;
                    if (resultData.HasRows && resultData.Read() == true)
                    {
                        int idxColumn = resultData.GetOrdinal("NEW_ID");
                        object value = resultData.GetValue(idxColumn);
                        if (typeof(DBNull) == value.GetType())
                            newId = 1;
                        else
                            newId = (int)value;//resultData.GetInt32(idxColumn);
                    }
                    sqlQuery = "INSERT INTO TEMPLATE_FIELD_VIDEO_VALUES(ID,FIELD_VALUE,ID_VIDEO,ID_TEMPLATE_FIELD_CATEGORY) VALUES(" + newId + ",'" + ValueData.Value + "','" + IdVideo + "'," + idFieldCategory + ")";
                }
                else
                {
                    sqlQuery = "UPDATE TEMPLATE_FIELD_VIDEO_VALUES SET " + Environment.NewLine;
                    sqlQuery += "FIELD_VALUE='" + ValueData.Value + "'" + Environment.NewLine;
                    sqlQuery += "WHERE ID_VIDEO ='" + IdVideo + "'" + Environment.NewLine;
                }
                dataContext.Execute(sqlQuery);
            }

        }

        private int _AddTemplateField(TemplateFieldData FieldData)
        {
            int result = -1;
            using (PikoDataContext dataContext = new PikoDataContext(PikoDataServiceApp.DatabasePath, "PikoServer"))
            {
                String sqlQuery = "SELECT MAX(ID) + 1 AS NEW_ID FROM TEMPLATE_FIELD";
                OleDbDataReader resultData = dataContext.Select(sqlQuery);
                if (resultData.HasRows && resultData.Read())
                {
                    int idxColumn = resultData.GetOrdinal("NEW_ID");
                    object value = resultData.GetValue(idxColumn);
                    if (typeof(DBNull) == value.GetType())
                        result = 1;
                    else
                        result = (int)value;//resultData.GetInt32(idxColumn);
                }
                else
                    result = 1;
                resultData.Close();
                sqlQuery = "INSERT INTO TEMPLATE_FIELD(ID,LABEL,ID_FIELD_TYPE) VALUES(" + result + ",'" + FieldData.FieldName + "'," + (FieldData.FieldType == TemplateFieldType.Text ? "0" : "1") + ")";
                dataContext.Execute(sqlQuery);
                

            }
            return result;
        }

        private int _AddTemplateFieldToCategory(int IdTemplateField, int IdCategory)
        {
            int result = -1;
            using (PikoDataContext dataContext = new PikoDataContext(PikoDataServiceApp.DatabasePath, "PikoServer"))
            {
                String sqlQuery = "SELECT MAX(ID) + 1 AS NEW_ID FROM TEMPLATE_FIELD_CATEGORY";
                OleDbDataReader resultData = dataContext.Select(sqlQuery);
                if (resultData.HasRows && resultData.Read())
                {
                    int idxColumn = resultData.GetOrdinal("NEW_ID");
                    object value = resultData.GetValue(idxColumn);
                    if (typeof(DBNull) == value.GetType())
                        result = 1;
                    else
                        result = (int)value;//resultData.GetInt32(idxColumn);
                }
                else
                    result = 1;
                sqlQuery = "INSERT INTO TEMPLATE_FIELD_CATEGORY(ID,ID_CATEGORY,ID_TEMPLATE_FIELD) VALUES(" + result + "," + IdCategory + "," + IdTemplateField + ")";
                dataContext.Execute(sqlQuery);                
                resultData.Close();
            }
            return result;
        }

        public bool TemplateFieldExists(string name,TemplateFieldType type)
        {
            bool result = false;
            using (PikoDataContext dataContext = new PikoDataContext(PikoDataServiceApp.DatabasePath, "PikoServer"))
            {
                String sqlQuery = "SELECT COUNT(ID) + 1 AS ISEXIST FROM TEMPLATE_FIELD WHERE LABEL = '" + name + "' AND ID_FIELD_TYPE = " + type;
                OleDbDataReader resultData = dataContext.Select(sqlQuery);
                if (resultData.HasRows && resultData.Read())
                {
                    int idxColumn = resultData.GetOrdinal("ISEXIST");
                    object value = resultData.GetValue(idxColumn);
                    if (typeof(DBNull) == value.GetType())
                        result = false;
                    else
                        result = true;//resultData.GetInt32(idxColumn);
                }
                resultData.Close();
            }
            return result;
        }

        public void UpdateTemplateField(int IdTemplateField , string newName,TemplateFieldType newType)
        {
            using (PikoDataContext dataContext = new PikoDataContext(PikoDataServiceApp.DatabasePath, "PikoServer"))
            {
                //UPDATE
                String sqlQuery = "UPDATE TEMPLATE_FIELD SET " + Environment.NewLine;
                sqlQuery += "LABEL='" + newName + "'" + Environment.NewLine;
                sqlQuery += "ID_FIELD_TYPE=" + newType + Environment.NewLine;
                sqlQuery += "WHERE ID='" + IdTemplateField + "'";
                dataContext.Execute(sqlQuery);

            }
        }

        public TemplateFieldData GetTemplateField(int IdTemplateField)
        {
            TemplateFieldData result = null;
            using (PikoDataContext dataContext = new PikoDataContext(PikoDataServiceApp.DatabasePath, "PikoServer"))
            {
                String sqlQuery = "SELECT ID, LABEL, ID_FIELD_TYPE  FROM TEMPLATE_FIELD WHERE ID = " + IdTemplateField;
                OleDbDataReader resultData = dataContext.Select(sqlQuery);
                if (resultData.HasRows)
                {
                    result = new TemplateFieldData();
                    int idxColumn = resultData.GetOrdinal("ID");
                    result.Id = resultData.GetInt32(idxColumn);
                    idxColumn = resultData.GetOrdinal("LABEL");
                    result.FieldName = resultData.GetString(idxColumn);
                    idxColumn = resultData.GetOrdinal("ID");
                    result.FieldType = (TemplateFieldType)resultData.GetInt32(idxColumn);
                }
            }
            return result;
        }

        public bool TemplateFieldCategoryExists(int IdCategory,int IdTemplateField)
        {
            bool result = false;
            using (PikoDataContext dataContext = new PikoDataContext(PikoDataServiceApp.DatabasePath, "PikoServer"))
            {
                String sqlQuery = "SELECT COUNT(ID) + 1 AS NEW_ID FROM TEMPLATE_FIELD_CATEGORY WHERE ID_CATEGORY = " + IdCategory + " AND ID_TEMPLATE_FIELD = " + IdTemplateField;
                OleDbDataReader resultData = dataContext.Select(sqlQuery);
                if (resultData.HasRows)
                {
                    int idxColumn = resultData.GetOrdinal("ISEXIST");
                    result = resultData.GetInt32(idxColumn) > 0;
                }
            }
            return result;
        }

        public string ExportBlock(PlaylistData Data)
        {
            try
            {
                Playlist block = new Playlist(Data);
                //PikoXML.SaveXML(block, System.IO.Path.Combine(PikoDataServiceApp.PikoPlaylistPath, Data.FileName + ".xml"));
                return Piko.XML.PikoXML.BuildXML(block);
            }
            catch
            {
                return "";
            }
        }

        public string ExportPlaylist(PlaylistData Data)
        {
            try
            {
                Playlist playlist = new Playlist(Data);
                //PikoXML.SaveXML(block, System.IO.Path.Combine(PikoDataServiceApp.PikoPlaylistPath, Data.FileName + ".xml"));
                return Piko.XML.PikoXML.BuildXML(playlist);
            }
            catch
            {
                return "";
            }
        }

        private void DeleteTemplateField(long TemplateFieldId)
        {
            using (PikoDataContext dataContext = new PikoDataContext(PikoDataServiceApp.DatabasePath, "PikoServer"))
            {
                String sqlQuery = "SELECT ID FROM TEMPLATE_FIELD_CATEGORY WHERE ID_TEMPLATE_FIELD = " + TemplateFieldId;
                OleDbDataReader resultData = dataContext.Select(sqlQuery);
                if (resultData.HasRows)
                {
                    while (resultData.Read())
                    {
                        int idxColumn = resultData.GetOrdinal("ID");
                        long IdTFC = resultData.GetInt32(idxColumn);
                        sqlQuery = "DELETE FROM TEMPLATE_FIELD_VIDEO_VALUES  WHERE ID_TEMPLATE_FIELD_CATEGORY = " + IdTFC;
                        dataContext.Execute(sqlQuery);
                    }
                    sqlQuery = "DELETE FROM TEMPLATE_FIELD_CATEGORY  WHERE ID_TEMPLATE_FIELD = " + TemplateFieldId;
                    dataContext.Execute(sqlQuery);
                    sqlQuery = "DELETE FROM TEMPLATE_FIELD  WHERE ID = " + TemplateFieldId;
                    dataContext.Execute(sqlQuery);
                }
            }
        }




        #endregion
    }
}
