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

        public PlaylistData Data = new PlaylistData();

        public Playlist(PlaylistData Data)
        {
            this.Data = Data;
            this.Data.PlaylistTitle = Data.PlaylistTitle;
            foreach (PlaylistElementData dataElement in this.Data.Elements)
                this.AddElement(new PlaylistElement(dataElement));
        }

        public Playlist(string Title)
        {
            this.Data = new PlaylistData();
            this.Data.PlaylistTitle = Title;
        }
        public void AddElement(AElement element)
        {
            this._elements.Add(new PlaylistElement(element));
        }

        public void AddElement(PlaylistElement element)
        {
            this._elements.Add(element);
        }

        public void ClearElement()
        {
            this._elements.Clear();

        }
    }
}
