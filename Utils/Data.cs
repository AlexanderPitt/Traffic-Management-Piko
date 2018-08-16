using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PikoTrafficManager.Data
{
    public enum ElementType : uint
    {
        Support = 0,
        Break = 1
    }

    public class Volume
    {

        public DataPikoClient.VolumeData Data { get; set; }
        public Volume(DataPikoClient.VolumeData VolumeData = null)
        {
            if (VolumeData != null)
            {
                this.Data = VolumeData;
                foreach(DataPikoClient.SupportData datSup in this.Data.SupportsData)
                    this._Supports.Add(new Support(datSup));
            }
            else
            {
                this.Data = new DataPikoClient.VolumeData();
            }
        }

        private List<Support> _Supports = new List<Support>();

        public Support[] Supports
        {
            get
            {
                if (this._Supports.Count == 0)
                {
                    foreach (DataPikoClient.SupportData dSup in Data.SupportsData)
                        this._Supports.Add(new Support(dSup));
                }
                return this._Supports.ToArray();
            }
        }

        public bool DeleteSupport(Support supportToDelete)
        {
            return this._Supports.Remove(supportToDelete);
        }

        public string Path
        {
            get
            {
                return this.Data.Path;
            }
            set
            {
                this.Data.Path = value;
            }
        }
    }

    public abstract class AElement
    {
        public AElement(ElementType Type)
        {
            this.ElementType = Type;
        }

        public ElementType ElementType { get; set; }

    }

    public class Support : AElement
    {
        public DataPikoClient.SupportData Data
        {
            get;
            set;
        }

        public Support(DataPikoClient.SupportData Data = null) : base(ElementType.Support)
        {
            if (Data != null)
            {
                this.Data = Data;
            }
            else
            {
                this.Data = new DataPikoClient.SupportData();
            }
        }
    }

    public class PlaylistElement
    {
        private AElement _element;
        public DataPikoClient.PlaylistElementData Data;
        public PlaylistElement(AElement Element, DataPikoClient.StartMode Mode = DataPikoClient.StartMode.Auto)
        {
            this._element = Element;
            this.Data = new DataPikoClient.PlaylistElementData();
            if (this._element != null)
            {
                
                if (this.Element is Support)
                {
                    Support Support = ((Support)this.Element);
                    this.Data.Uid = Support.Data.UIdSupport;
                    this.Data.Title = String.IsNullOrEmpty(Support.Data.Title) ? "" : Support.Data.Title;
                    this.Data.FileName = Support.Data.FileName;
                    this.Data.TCIn = 0;
                    this.Data.Duration = 0;
                    this.Data.FrameRate = DataPikoClient.FrameRate.PAL;
                    this.Data.ElementType = DataPikoClient.ElementType.Support;
                }
                else
                {
                    this.Data.Uid = new Guid().ToString();
                    this.Data.Title = "";
                    this.Data.ElementType = DataPikoClient.ElementType.Break;
                }
                this.Data.StartMode = Mode;
            }
            else
            {
                //Default
                this.Data.Uid = new Guid().ToString();
                this.Data.Title = "";
                this.Element = null;
                this.Data.TCIn = 0;
                this.Data.FrameRate = DataPikoClient.FrameRate.PAL;
                this.Data.Duration = 0;
                this.Data.StartMode = DataPikoClient.StartMode.Auto;
                this.Data.ElementType = DataPikoClient.ElementType.Break;
            }
        }

        public PlaylistElement(DataPikoClient.PlaylistElementData Data)
        {
            this.Data = Data;
            switch (this.Data.ElementType)
            {
                case DataPikoClient.ElementType.Support:
                    Support support = new Support();
                    support.Data = new DataPikoClient.SupportData();
                    support.Data.UIdSupport = this.Data.Uid;
                    support.Data.Title = this.Data.Title;
                    support.Data.FileName = this.Data.FileName;
                    support.Data.TcStart = this.Data.TCIn;
                    support.Data.Duration = 0;
                    support.Data.FrameRate = DataPikoClient.FrameRate.PAL;
                    break;
            }
        }

        public AElement Element
        {
            get
            {
                return this._element;
            }
            set
            {
                this._element = value;
            }
        }
        public string TCInDisplay
        {
            get
            {
                string formatedTC = "";
                int totalSec = 0;
                int totalMin = 0;
                int totalHour = 0;
                switch (this.Data.FrameRate)
                {
                    case DataPikoClient.FrameRate.NTSC:
                        {
                            totalSec = (int)(this.Data.TCIn / 30);
                            totalMin = (int)(totalSec / 60);
                            totalHour = (int)(totalMin / 60);
                            formatedTC = totalHour.ToString().PadLeft(2, '0') + ":" + (totalMin - (totalHour * 60)).ToString().PadLeft(2, '0') + ":" + (totalSec - (totalMin * 60)).ToString().PadLeft(2, '0') + ":" + ((int)(this.Data.TCIn % 30)).ToString().PadLeft(2, '0');
                        }
                        break;
                    default:
                        {
                            totalSec = (int)(this.Data.TCIn / 25);
                            totalMin = (int)(totalSec / 60);
                            totalHour = (int)(totalMin / 60);
                            formatedTC = totalHour.ToString().PadLeft(2, '0') + ":" + (totalMin - (totalHour * 60)).ToString().PadLeft(2, '0') + ":" + (totalSec - (totalMin * 60)).ToString().PadLeft(2, '0') + ":" + ((int)(this.Data.TCIn % 25)).ToString().PadLeft(2, '0');
                        }
                        break;
                }
                return formatedTC;
            }
            set
            {
                string inputValue = value;
                string[] splittedValue = inputValue.Split(':');
                int totalSec = int.Parse(splittedValue[2]);
                int totalMin = int.Parse(splittedValue[1]);
                int totalHour = int.Parse(splittedValue[0]);
                int totalFrame = int.Parse(splittedValue[3]);
                switch (this.Data.FrameRate)
                {
                    case DataPikoClient.FrameRate.NTSC:
                        {
                            totalFrame += totalSec * 30;
                            totalFrame += totalMin * 60 * 30;
                            totalFrame += totalHour * 60 * 60 * 30;
                        }
                        break;
                    default:
                        {
                            totalFrame += totalSec * 25;
                            totalFrame += totalMin * 60 * 25;
                            totalFrame += totalHour * 60 * 60 * 25;
                        }
                        break;
                }
                this.Data.TCIn = (uint)totalFrame;
            }
        }
        public string DurationDisplay
        {
            get
            {
                string formatedTC = "";
                int totalSec = 0;
                int totalMin = 0;
                int totalHour = 0;
                switch (this.Data.FrameRate)
                {
                    case DataPikoClient.FrameRate.NTSC:
                        {
                            totalSec = (int)(this.Data.Duration / 30);
                            totalMin = (int)(totalSec / 60);
                            totalHour = (int)(totalMin / 60);
                            formatedTC = totalHour.ToString().PadLeft(2, '0') + ":" + (totalMin - (totalHour * 60)).ToString().PadLeft(2, '0') + ":" + (totalSec - (totalMin * 60)).ToString().PadLeft(2, '0') + ":" + ((int)(this.Data.Duration % 30)).ToString().PadLeft(2, '0');
                        }
                        break;
                    default:
                        {
                            totalSec = (int)(this.Data.Duration / 25);
                            totalMin = (int)(totalSec / 60);
                            totalHour = (int)(totalMin / 60);
                            formatedTC = totalHour.ToString().PadLeft(2, '0') + ":" + (totalMin - (totalHour * 60)).ToString().PadLeft(2, '0') + ":" + (totalSec - (totalMin * 60)).ToString().PadLeft(2, '0') + ":" + ((int)(this.Data.Duration % 25)).ToString().PadLeft(2, '0');
                        }
                        break;
                }
                return formatedTC;
            }
            set
            {
                string inputValue = value;
                string[] splittedValue = inputValue.Split(':');
                int totalSec = int.Parse(splittedValue[2]);
                int totalMin = int.Parse(splittedValue[1]);
                int totalHour = int.Parse(splittedValue[0]);
                int totalFrame = int.Parse(splittedValue[3]);
                switch (this.Data.FrameRate)
                {
                    case DataPikoClient.FrameRate.NTSC:
                        {
                            totalFrame += totalSec * 30;
                            totalFrame += totalMin * 60 * 30;
                            totalFrame += totalHour * 60 * 60 * 30;
                        }
                        break;
                    default:
                        {
                            totalFrame += totalSec * 25;
                            totalFrame += totalMin * 60 * 25;
                            totalFrame += totalHour * 60 * 60 * 25;
                        }
                        break;
                }
                this.Data.Duration = (uint)totalFrame;
            }
        }

        public string Title
        {
            get
            {
                return this.Data.Title;
            }
            set
            {
                this.Data.Title = value;
            }
        }

        public string FileName
        {
            get
            {
                return this.Data.FileName;
            }
            set
            {
                this.Data.FileName = value;
            }
        }

        public DataPikoClient.ElementType Type
        {
            get
            {
                return this.Data.ElementType;
            }
        }

        public DataPikoClient.PlaylistElementSecondaryEventData[] SecondaryEvents
        {
            get
            {
                return this.Data.SecondaryEvents;
            }
        }
        public void AddSecondaryEvent(DataPikoClient.PlaylistElementSecondaryEventData Event)
        {
            List<DataPikoClient.PlaylistElementSecondaryEventData> secEvt = new List<DataPikoClient.PlaylistElementSecondaryEventData>(this.Data.SecondaryEvents);
            secEvt.Add(Event);
            this.Data.SecondaryEvents = secEvt.ToArray();
        }
        public void RemoveSecondaryEventAt(int index)
        {
            List<DataPikoClient.PlaylistElementSecondaryEventData> secEvt = new List<DataPikoClient.PlaylistElementSecondaryEventData>(this.Data.SecondaryEvents);
            secEvt.RemoveAt(index);
            this.Data.SecondaryEvents = secEvt.ToArray();

        }
        public void ClearSecondaryEvent()
        {
            this.Data.SecondaryEvents = new DataPikoClient.PlaylistElementSecondaryEventData[0];
        }
    }

    public class Playlist
    {
        private List<PlaylistElement> _elements = new List<PlaylistElement>();
        public PlaylistElement[] Elements
        {
            get
            {
                return this._elements.ToArray();
            }
        }

        public DataPikoClient.PlaylistData Data = new DataPikoClient.PlaylistData();

        public string Name
        {
            get
            {
                return this.Data.PlaylistTitle;
            }
            set
            {
                this.Data.PlaylistTitle = value;
            }
        }

        public Playlist(DataPikoClient.PlaylistData Data)
        {
            this.Data = Data;
            this.Data.PlaylistTitle = Data.PlaylistTitle;
            foreach (DataPikoClient.PlaylistElementData dataElement in Data.Elements)
                this.AddElement(new PlaylistElement(dataElement));
        }

        public Playlist(string Title)
        {
            this.Data = new DataPikoClient.PlaylistData();
            this.Data.PlaylistTitle = Title;
        }
        public void AddElement(AElement element)
        {
            PlaylistElement newElementToAdd = new PlaylistElement(element);
            this._elements.Add(newElementToAdd);
            List<DataPikoClient.PlaylistElementData> elementsData = new List<DataPikoClient.PlaylistElementData>();
            if (this.Data.Elements != null && this.Data.Elements.Length > 0)
                elementsData.AddRange(this.Data.Elements);
            elementsData.Add(newElementToAdd.Data);
            this.Data.Elements = elementsData.ToArray();
        }

        public void AddElement(PlaylistElement element)
        {
            this._elements.Add(element);
            List<DataPikoClient.PlaylistElementData> elementsData = new List<DataPikoClient.PlaylistElementData>();
            if (this.Data.Elements != null && this.Data.Elements.Length > 0)
                elementsData.AddRange(this.Data.Elements);
            elementsData.Add(element.Data);
            this.Data.Elements = elementsData.ToArray();
        }

        public void ClearElement()
        {
            this._elements.Clear();
            this.Data.Elements = new DataPikoClient.PlaylistElementData[0];
        }
    }

}
