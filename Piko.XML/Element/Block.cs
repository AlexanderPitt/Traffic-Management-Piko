using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PikoTrafficManager.Data;

namespace Piko.XML.Element
{
    public class Block 
    {
        public Block(String Name = "")
        {
            this.Name = Name;
        }

        public string Name { get; set; }

        public BlockData Data { get; set; }

        private List<PlaylistElement> _elements = new List<PlaylistElement>();
        public PlaylistElement[] Elements
        {
            get
            {
                return this._elements.ToArray();
            }
        }

        public uint GetBlockDuration()
        {
            //here return the total duration of the block ...
            return 0;
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
