using Piko.XML.Data;
using Piko.XML.Element;
using PikoTrafficManager.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Piko.XML
{
    public static class PikoXML
    {
        public static PlaylistData LoadXML(string xmlPath)
        {
            XElement result = XElement.Load(xmlPath);
            PlaylistData data = new PlaylistData();
            XElement generateTag = result.Element("Generate"); //new XElement("Generate");
            data.PlaylistTitle = generateTag.Attribute("play_list_title").Value;
            data.PlaylistType = (generateTag.Attribute("play_list_type") != null && !string.IsNullOrEmpty(generateTag.Attribute("play_list_type").Value) ? (PlayListType)int.Parse(generateTag.Attribute("play_list_type").Value) : PlayListType.Classic);

            //generateTag.SetAttributeValue("auto_link", "No");
            //generateTag.SetAttributeValue("comment", "");
            //generateTag.SetAttributeValue("file_date", DateTime.Now.ToString("ddMMyyyy"));
            //generateTag.SetAttributeValue("origin", "Piko Traffic Manager");
            //generateTag.SetAttributeValue("play_list_title", block.Data.Title);
            //generateTag.SetAttributeValue("play_list_type", "block");
            List<PlaylistElementData> elementsData = new List<PlaylistElementData>();
            foreach (XElement playListElement in result.Elements("Event"))
            {
                PlaylistElementData elementData = new PlaylistElementData();
                elementData.Duration = playListElement.Attribute("event_duration") != null
                                       && !string.IsNullOrEmpty(playListElement.Attribute("event_duration").Value) ? (uint)int.Parse(playListElement.Attribute("event_duration").Value) : (uint)0;
                elementData.FileName = playListElement.Attribute("event_file_name_id") != null
                                       && !string.IsNullOrEmpty(playListElement.Attribute("event_file_name_id").Value) ? playListElement.Attribute("event_file_name_id").Value : "";
                elementData.Repeat = playListElement.Attribute("event_repeat") != null
                                       && !string.IsNullOrEmpty(playListElement.Attribute("event_repeat").Value) ? bool.Parse(playListElement.Attribute("event_repeat").Value) : false;
                elementData.Title = playListElement.Attribute("event_title") != null
                                       && !string.IsNullOrEmpty(playListElement.Attribute("event_title").Value) ? playListElement.Attribute("event_title").Value : "";
                List<PlaylistElementSecondaryEventData> listSecData = new List<PlaylistElementSecondaryEventData>();
                if (playListElement.Elements("Event").Count() > 0)
                {                    
                    foreach (XElement secondaryElement in playListElement.Elements("Event"))
                    {
                        PlaylistElementSecondaryEventData secData = new PlaylistElementSecondaryEventData();
                        secData.Duration = secondaryElement.Attribute("sec_evt_duration") != null
                                       && !string.IsNullOrEmpty(secondaryElement.Attribute("sec_evt_duration").Value) ? (uint)int.Parse(secondaryElement.Attribute("sec_evt_duration").Value) : (uint)0;
                        secData.ExtendedParam = secondaryElement.Attribute("sec_evt_ext_param") != null
                                       && !string.IsNullOrEmpty(secondaryElement.Attribute("sec_evt_ext_param").Value) ? secondaryElement.Attribute("sec_evt_ext_param").Value : "";
                        secData.Id = secondaryElement.Attribute("sec_evt_id") != null
                                       && !string.IsNullOrEmpty(secondaryElement.Attribute("sec_evt_id").Value) ? int.Parse(secondaryElement.Attribute("sec_evt_id").Value) : (int)0;
                        secData.Param = secondaryElement.Attribute("sec_evt_ext_param") != null
                                       && !string.IsNullOrEmpty(secondaryElement.Attribute("sec_evt_ext_param").Value) ? secondaryElement.Attribute("sec_evt_ext_param").Value : "";
                        secData.TcOffsetType = secondaryElement.Attribute("sec_evt_tc_offset_type") != null
                                       && !string.IsNullOrEmpty(secondaryElement.Attribute("sec_evt_tc_offset_type").Value) ? (uint)int.Parse(secondaryElement.Attribute("sec_evt_tc_offset_type").Value) : (uint)0;
                        secData.SecondaryEventType = secondaryElement.Attribute("sec_evt_type") != null
                                       && !string.IsNullOrEmpty(secondaryElement.Attribute("sec_evt_type").Value) ? secondaryElement.Attribute("sec_evt_type").Value : "";
                        listSecData.Add(secData);
                    }
                }
                elementData.SecondaryEvents = listSecData.ToArray();
                elementsData.Add(elementData);
            }
            data.Elements = elementsData.ToArray();
            return data;
        }

        public static void SaveXML(Playlist playlist , string xmlSavePath)
        {
            XElement xmlPlaylist = BuildXMLObject(playlist);
            xmlPlaylist.Save(xmlSavePath);
        }

        public static string BuildXML(Playlist playlist)
        {
            XElement xmlPlaylist = BuildXMLObject(playlist);
            return xmlPlaylist.ToString();
        }

        //private static void SaveBlockXML(Playlist block, string xmlSavePath)
        //{
        //    XElement xmlPlaylist = new XElement("Exchange-Piko-Traffic");
        //    XElement generateTag = new XElement("Generate");
        //    generateTag.SetAttributeValue("auto_link", "No");
        //    generateTag.SetAttributeValue("comment", "");
        //    generateTag.SetAttributeValue("file_date", DateTime.Now.ToString("ddMMyyyy"));
        //    generateTag.SetAttributeValue("origin", "Piko Traffic Manager");
        //    generateTag.SetAttributeValue("play_list_title", block.Data.Title);
        //    generateTag.SetAttributeValue("play_list_type", "block");
        //    xmlPlaylist.Add(generateTag);
        //    foreach (PlaylistElement blockLine in block.Elements)
        //    {
        //        XElement eventTag = new XElement("Event");
        //        eventTag.SetAttributeValue("event_duration", blockLine.Data.Duration);
        //        eventTag.SetAttributeValue("event_file_name_id", blockLine.Data.FileName);
        //        eventTag.SetAttributeValue("event_file_tc_start_offset", null);
        //        eventTag.SetAttributeValue("event_gender", null);
        //        eventTag.SetAttributeValue("event_repeat", blockLine.Data.Repeat);
        //        eventTag.SetAttributeValue("event_source", null);
        //        eventTag.SetAttributeValue("event_start_date", null);
        //        eventTag.SetAttributeValue("event_start_type_param", null);
        //        eventTag.SetAttributeValue("event_tech_id", null);
        //        eventTag.SetAttributeValue("event_title", blockLine.Data.Title);
        //        eventTag.SetAttributeValue("event_traffic_key", null);
        //        eventTag.SetAttributeValue("event_transition", null);
        //        eventTag.SetAttributeValue("event_transition_duration", null);
        //        eventTag.SetAttributeValue("event_type", null);
        //        eventTag.SetAttributeValue("Event_fade_in_duration", null);
        //        eventTag.SetAttributeValue("Event_fade_in_Type", null);
        //        eventTag.SetAttributeValue("Event_fade_out_duration", null);
        //        eventTag.SetAttributeValue("Event_fade_out_Type", null);
        //        if (blockLine.SecondaryEvents.Length > 0)
        //        {
        //            foreach (PlaylistElementSecondaryEventData secEvent in blockLine.SecondaryEvents)
        //            {
        //                XElement evtSecTag = new XElement("Event");
        //                eventTag.SetAttributeValue("sec_evt_duration", secEvent.Duration);
        //                eventTag.SetAttributeValue("sec_evt_ext_param", secEvent.ExtendedParam);
        //                eventTag.SetAttributeValue("sec_evt_id", secEvent.Id);
        //                eventTag.SetAttributeValue("sec_evt_param", secEvent.Param);
        //                eventTag.SetAttributeValue("sec_evt_tc_offset_type", secEvent.TcOffsetType);
        //                eventTag.SetAttributeValue("sec_evt_type", secEvent.SecondaryEventType);
        //                eventTag.Add(evtSecTag);
        //            }
        //        }
        //        xmlPlaylist.Add(eventTag);



        //    }
        //    xmlPlaylist.Save(xmlSavePath);            
        //}

        //private static void SavePlaylistXML(Playlist playlist, string xmlSavePath)
        //{
        //    XElement xmlPlaylist = BuildXMLObject(playlist);
        //    xmlPlaylist.Save(xmlSavePath);
        //}

        private static XElement BuildXMLObject(Playlist playlist)
        {
            XElement xmlPlaylist = new XElement("Exchange-Piko-Traffic");
            XElement generateTag = new XElement("Generate");
            generateTag.SetAttributeValue("auto_link", "No");
            generateTag.SetAttributeValue("comment", playlist.Data.Comment);
            generateTag.SetAttributeValue("file_date", DateTime.Now.ToString("ddMMyyyy"));
            generateTag.SetAttributeValue("origin", "Piko Traffic Manager");
            generateTag.SetAttributeValue("play_list_title", playlist.Data.PlaylistTitle);
            generateTag.SetAttributeValue("play_list_type", playlist.Data.PlaylistType == PlayListType.Block ? "block" : "");
            xmlPlaylist.Add(generateTag);
            foreach (PlaylistElement playlistLine in playlist.Elements)
            {
                XElement eventTag = new XElement("Event");
                eventTag.SetAttributeValue("event_duration", playlistLine.Data.Duration);
                eventTag.SetAttributeValue("event_file_name_id", playlistLine.Data.FileName);
                eventTag.SetAttributeValue("event_file_tc_start_offset", null);
                eventTag.SetAttributeValue("event_gender", null);
                eventTag.SetAttributeValue("event_repeat", playlistLine.Data.Repeat);
                eventTag.SetAttributeValue("event_source", null);
                eventTag.SetAttributeValue("event_start_date", null);
                eventTag.SetAttributeValue("event_start_type_param", null);
                eventTag.SetAttributeValue("event_tech_id", null);
                eventTag.SetAttributeValue("event_title", playlistLine.Data.Title);
                eventTag.SetAttributeValue("event_traffic_key", null);
                eventTag.SetAttributeValue("event_transition", null);
                eventTag.SetAttributeValue("event_transition_duration", null);
                eventTag.SetAttributeValue("event_type", null);
                eventTag.SetAttributeValue("Event_fade_in_duration", null);
                eventTag.SetAttributeValue("Event_fade_in_Type", null);
                eventTag.SetAttributeValue("Event_fade_out_duration", null);
                eventTag.SetAttributeValue("Event_fade_out_Type", null);
                if (playlistLine.SecondaryEvents!= null && playlistLine.SecondaryEvents.Length > 0)
                {
                    foreach (PlaylistElementSecondaryEventData secEvent in playlistLine.SecondaryEvents)
                    {
                        XElement evtSecTag = new XElement("Event");
                        eventTag.SetAttributeValue("sec_evt_duration", secEvent.Duration);
                        eventTag.SetAttributeValue("sec_evt_ext_param", secEvent.ExtendedParam);
                        eventTag.SetAttributeValue("sec_evt_id", secEvent.Id);
                        eventTag.SetAttributeValue("sec_evt_param", secEvent.Param);
                        eventTag.SetAttributeValue("sec_evt_tc_offset_type", secEvent.TcOffsetType);
                        eventTag.SetAttributeValue("sec_evt_type", secEvent.SecondaryEventType);
                        eventTag.Add(evtSecTag);
                    }
                }
                xmlPlaylist.Add(eventTag);
            }

            return xmlPlaylist;
        }

    }
}
