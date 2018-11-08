using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PikoTrafficManager.Data
{
    [Serializable()]
    [DataContract(Name = "FrameRate" , Namespace = "http://developer.piko.com/PikoDataService/Data/")]
    public enum FrameRate : uint
    {
        [EnumMember]
        PAL = 25000,
        [EnumMember]
        NTSC = 30000,
        [EnumMember]
        NTSC_DROP
    }

    [Serializable()]
    [DataContract(Name = "StartMode", Namespace = "http://developer.piko.com/PikoDataService/Data/")]
    public enum StartMode : uint
    {
        [EnumMember]
        Auto = 0,
        [EnumMember]
        FixedTime = 1,
        [EnumMember]
        Manual = 2
    }

    [Serializable()]
    [DataContract(Name = "PlayListType", Namespace = "http://developer.piko.com/PikoDataService/Data/")]
    public enum PlayListType : int
    {
        [EnumMember]
        Classic = 1,
        [EnumMember]
        Block = 2 
    }

    [Serializable()]
    [DataContract(Name = "TemplateFieldType", Namespace = "http://developer.piko.com/PikoDataService/Data/")]
    public enum TemplateFieldType : int
    {
        [EnumMember]
        Text = 0,
        [EnumMember]
        Picture = 1,
        [EnumMember]
        Video = 2,
    }
}
