using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PikoDataService.DB
{
    [DataContractAttribute]
    public class Channel
    {
        [DataMemberAttribute]
        public String ChannelName
        {
            get;
            set;
        }
        [DataMemberAttribute]
        public DateTime LastHasRunDate
        {
            get;
            set;
        }
    }
    public class Config
    {
        [DataMemberAttribute]
        public ulong Version
        {
            get;
            set;
        }
        [DataMemberAttribute]
        public string SupportsId
        {
            get;
            set;
        }
        [DataMemberAttribute]
        public string HasRunsId
        {
            get;
            set;
        }
    }
    [DataContractAttribute]
    public class Devices
    {
        [DataMemberAttribute]
        public string Device
        {
            get;
            set;
        }
        [DataMemberAttribute]
        public uint DeviceType
        {
            get;
            set;
        }
    }
    [DataContractAttribute]
    public class HasRun
    {
        [DataMemberAttribute]
        public string HasRunId { get; set; }
        [DataMemberAttribute]
        public string Channel { get; set; }
        [DataMemberAttribute]
        public string DeviceSource { get; set; }
        [DataMemberAttribute]
        public DateTime IdDate { get; set; }
        [DataMemberAttribute]
        public TimeSpan IdTime { get; set; }
        [DataMemberAttribute]
        public string SupportId { get; set; }
        [DataMemberAttribute]
        public uint SegmentId { get; set; }
        [DataMemberAttribute]
        public string Title { get; set; }
        [DataMemberAttribute]
        public string TrafficKey { get; set; }
        [DataMemberAttribute]
        public ulong Som { get; set; }
        [DataMemberAttribute]
        public ulong Duration { get; set; }
        [DataMemberAttribute]
        public DateTime EventDate { get; set; }
        [DataMemberAttribute]
        public TimeSpan EventTime { get; set; }
        [DataMemberAttribute]
        public uint EventPlayMode { get; set; }
        [DataMemberAttribute]
        public string EventFileName { get; set; }
    }
    [DataContractAttribute]
    public class HasRunSec
    {
        [DataMemberAttribute]
        public string HasRunId { get; set; }
        [DataMemberAttribute]
        public uint EventType { get; set; }
        [DataMemberAttribute]
        public string EventId { get; set; }
        [DataMemberAttribute]
        public string OffSetFrom { get; set; }
        [DataMemberAttribute]
        public string OffSet { get; set; }
        [DataMemberAttribute]
        public string EventParam1 { get; set; }
        [DataMemberAttribute]
        public string EventParam2 { get; set; }

    }
    [DataContractAttribute]
    public class Segment
    {
        [DataMemberAttribute]
        public string SupportId { get; set; }
        [DataMemberAttribute]
        public uint SegmentId { get; set; }
        [DataMemberAttribute]
        public ulong Som { get; set; }
        [DataMemberAttribute]
        public ulong Eom { get; set; }
        [DataMemberAttribute]
        public ulong Duration { get; set; }
    }

    [DataContractAttribute]
    public class Video
    {
        [DataMemberAttribute]
        public string Id { get; set; }
        [DataMemberAttribute]
        public string Extension { get; set; }
        [DataMemberAttribute]
        public ulong TCStart { get; set; }
        [DataMemberAttribute]
        public ulong Eom { get; set; }
        [DataMemberAttribute]
        public ulong Duration { get; set; }
        [DataMemberAttribute]
        public ulong Width { get; set; }
        [DataMemberAttribute]
        public ulong Height { get; set; }
        [DataMemberAttribute]
        public ulong FrameRate { get; set; }
        [DataMemberAttribute]
        public ulong AudioCount { get; set; }
        [DataMemberAttribute]
        public string FileSize { get; set; }
        [DataMemberAttribute]
        public string FileName { get; set; }
        [DataMemberAttribute]
        public ulong State { get; set; }
        [DataMemberAttribute]
        public string SupportId { get; set; }
    }



}
