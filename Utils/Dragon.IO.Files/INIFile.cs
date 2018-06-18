using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Dragon.IO.Files.INI
{
    [DataContractAttribute]
    public class INIFile
    {
        
        public static INIFile Parse(string INIFileName)
        {
            INIFile result = new INIFile(INIFileName);
            return result;
        }

        public static INIFile ParseINIString(String INIFileString)
        {
            INIFile result = new INIFile();
            System.IO.MemoryStream mStream = new System.IO.MemoryStream();
            System.IO.StreamWriter sw = new System.IO.StreamWriter(mStream);
            sw.Write(INIFileString);
            sw.Flush();
            mStream.Position = 0;
            //Parse IniFile
            using (System.IO.StreamReader sr = new System.IO.StreamReader(mStream))
            {
                string sTemp = "";
                StringBuilder SBuilder = new StringBuilder();
                string currentSection = null;
                while ((sTemp = sr.ReadLine()) != null)
                {
                    string trimLine = sTemp.Trim();
                    if (!String.IsNullOrEmpty(trimLine))
                    {
                        if (trimLine.First() == '[')
                        { // il s'agit d'une section
                            currentSection = trimLine.Replace("[", "").Replace("]", "").Trim();
                            result.CreateSection(currentSection);

                        }
                        else
                        { // il s'agit d'un élement de la section en cours
                            string[] asTmp = trimLine.Split('=');
                            result.Sections.Where(s => s.Name == currentSection).FirstOrDefault().AddElement(asTmp[0].Trim(), asTmp[1].Trim());
                        }
                    }
                }
            }
            return result;
        }
        public INIFile()
        { }
        public INIFile(String INIFileName)
        {
            if(!System.IO.File.Exists(INIFileName))
                throw new Exception("File not exist !");

            this.Filename = System.IO.Path.GetFileName(INIFileName);
            this.Path = System.IO.Path.GetFullPath(INIFileName).Replace(this.Filename,"");

            //Parse IniFile
            using (System.IO.StreamReader sr = System.IO.File.OpenText(INIFileName))
            {
                string sTemp = "";
                StringBuilder SBuilder = new StringBuilder();
                INISection currentSection = null;
                while(  (sTemp = sr.ReadLine()) != null  )
                {
                    string trimLine = sTemp.Trim();
                    if(trimLine.First() == '[')
                    { // il s'agit d'une section
                        if (currentSection != null)
                            this._sections.Add(currentSection);
                        currentSection = new INISection(trimLine.Replace("[","").Replace("]","").Trim());
                    }
                    else
                    { // il s'agit d'un élement de la section en cours
                        string[] asTmp = trimLine.Split('='); 
                        currentSection.AddElement(asTmp[0].Trim(), asTmp[1].Trim());
                    }
                }
                if(currentSection != null)
                    this._sections.Add(currentSection);            
            }

        }
        [DataMemberAttribute]
        public string Filename
        {
            get;
            private set;
        }
        [DataMemberAttribute]
        public string Path
        {
            get;
            private set;
        }

        private List<INISection> _sections = new List<INISection>();
        [DataMemberAttribute]
        public INISection[] Sections
        {
            get
            {
                return this._sections.ToArray();
            }
        }

        public void CreateSection(String Name)
        {
            if (!this._sections.Any(e => e.Name == Name))
                this._sections.Add(new INISection(Name));
            else
                throw new Exception("Section with name " + Name + " already exist!");
        }

        public void RemoveSection(String Name)
        {
            INISection sectionsToRemove = this._sections.Where(e => e.Name == Name).FirstOrDefault();
            if (sectionsToRemove != null)
                this._sections.Remove(sectionsToRemove);
        }
    }
    [DataContractAttribute]
    public class INISection
    {
        [DataMemberAttribute]
        public String Name
        {
            get;
            private set;
        }

        private List<INISectionElement> _Elements = new List<INISectionElement>();

        public INISection(String SectionName)
        {
            this.Name = SectionName;
        }
        [DataMemberAttribute]
        public INISectionElement[] Items
        {
            get
            {
                return this._Elements.ToArray();
            }
        }

        public INISectionElement Element(String Name)
        {
            return this._Elements.Where(e => e.Name == Name).FirstOrDefault();
        }

        public void AddElement(String Name, String Value)
        {
            if (!this._Elements.Any(e => e.Name == Name))
                this._Elements.Add(new INISectionElement(Name, Value));
            else
                throw new Exception("Element with name " + Name + " already exist!");
        }

        public void RemoveElement(String Name)
        {
            INISectionElement elementToRemove = this._Elements.Where(e => e.Name == Name).FirstOrDefault();
            if (elementToRemove != null)
                this._Elements.Remove(elementToRemove);
        }


    }
    [DataContractAttribute]
    public class INISectionElement
    {
        [DataMemberAttribute]
        public String Name
        {
            get;
            private set;
        }

        [DataMemberAttribute]
        public string Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }

        private string _value;

        public INISectionElement(String ElementName, string value = "")
        {
            this.Name = Name;
            this._value = value;
        }

        public int AsInteger()
        {
            return this._value != "" ? int.Parse(this._value) : int.MinValue;
        }

        public string AsString()
        {
            return this._value;
        }

    }


}
