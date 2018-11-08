using Piko.XML.Data;
using PikoTrafficManager.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Piko.XML.Element
{
    [DataContract(Namespace = "http://developer.piko.com/PikoDataService/Data/")]
    public class PlaylistElement
    {
        private AElement _element;
        public PlaylistElementData Data;
        public PlaylistElement(AElement Element, StartMode Mode = StartMode.Auto)
        {
            this._element = Element;
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
                    this.Data.FrameRate = FrameRate.PAL;
                    this.Data.ElementType = ElementType.Support;
                }
                else
                {
                    this.Data.Uid = new Guid().ToString();
                    this.Data.Title = "";
                    this.Data.ElementType = ElementType.Break;
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
                this.Data.FrameRate = FrameRate.PAL;
                this.Data.Duration = 0;
                this.Data.StartMode = StartMode.Auto;
                this.Data.ElementType = ElementType.Break;
            }
        }

        public PlaylistElement(PlaylistElementData Data)
        {
            this.Data = Data;
            switch(this.Data.ElementType)
            {
                case ElementType.Support:
                    Support support = new Support();
                    support.Data = new SupportData();
                    support.Data.UIdSupport = this.Data.Uid;
                    support.Data.Title = this.Data.Title;
                    support.Data.FileName = this.Data.FileName;
                    support.Data.TcStart = this.Data.TCIn;
                    support.Data.Duration = 0;
                    support.Data.FrameRate = FrameRate.PAL;
                    break;
            }
        }

        public AElement Element { get
            {
                return this._element;
            }
            set
            {
                this._element = value;
            } }
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
                    case FrameRate.NTSC:
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
                    case FrameRate.NTSC:
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
                    case FrameRate.NTSC:
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
                    case FrameRate.NTSC:
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
        public ElementType Type
        {
            get
            {
                return this.Data.ElementType;
            }
        }
        
        public PlaylistElementSecondaryEventData[] SecondaryEvents
        {
            get
            {
                return this.Data.SecondaryEvents;
            }
        }
        public void AddSecondaryEvent(PlaylistElementSecondaryEventData Event)
        {
            List<PlaylistElementSecondaryEventData> secEvt = new List<PlaylistElementSecondaryEventData>(this.Data.SecondaryEvents);
            secEvt.Add(Event);
            this.Data.SecondaryEvents = secEvt.ToArray();
        }
        public void RemoveSecondaryEventAt(int index)
        {
            List<PlaylistElementSecondaryEventData> secEvt = new List<PlaylistElementSecondaryEventData>(this.Data.SecondaryEvents);
            secEvt.RemoveAt(index);
            this.Data.SecondaryEvents = secEvt.ToArray();
            
        }
        public void ClearSecondaryEvent()
        {
            this.Data.SecondaryEvents = new PlaylistElementSecondaryEventData[0];
        }
    }
}
