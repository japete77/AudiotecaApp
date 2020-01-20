using audioteca.Models.Daisy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;
using System.IO;

namespace audioteca.Services
{
    public class DaisyBook
    {
        public static int NAV_LEVEL_1 = 1;
        public static int NAV_LEVEL_2 = 2;
        public static int NAV_LEVEL_3 = 3;
        public static int NAV_LEVEL_4 = 4;
        public static int NAV_LEVEL_5 = 5;
        public static int NAV_LEVEL_6 = 6;
        public static int NAV_LEVEL_PHRASE = 7;
        public static int NAV_LEVEL_PAGE = 8;

        // Metadata info
        private string _id;
        private string _creator;
        private string _date;
        private string _format;
        private string _identifier;
        private string _publisher;
        private string _subject;
        private string _source;
        private string _title;
        private string _charset;
        private string _generator;
        private string _narrator;
        private string _producer;
        private string _totalTime;

        private int _maxLevels = 0;
        private bool _hasPages = false;

        // body smil info
        private List<SmilInfo> _body = new List<SmilInfo>();
        public List<SmilInfo> Body { get { return _body; } }

        // audio navigation helper
        private List<Sequence> _sequence = new List<Sequence>();
        private List<Sequence> Sequence { get { return _sequence; } }

        public DaisyBook(string filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            // Read header metadata
            var meta = doc.GetElementsByTagName("meta");
            for (var i = 0; i < meta.Count; i++)
            {
                if (meta.Item(i).Attributes.GetNamedItem("name") != null)
                {
                    if (meta.Item(i).Attributes.GetNamedItem("name").Value == "dc:creator")
                        this._creator = meta.Item(i).Attributes.GetNamedItem("content").Value;
                    else if (meta.Item(i).Attributes.GetNamedItem("name").Value == "dc:date")
                        this._date = meta.Item(i).Attributes.GetNamedItem("content").Value;
                    else if (meta.Item(i).Attributes.GetNamedItem("name").Value == "dc:format")
                        this._format = meta.Item(i).Attributes.GetNamedItem("content").Value;
                    else if (meta.Item(i).Attributes.GetNamedItem("name").Value == "dc:identifier")
                        this._identifier = meta.Item(i).Attributes.GetNamedItem("content").Value;
                    else if (meta.Item(i).Attributes.GetNamedItem("name").Value == "dc:publisher")
                        this._publisher = meta.Item(i).Attributes.GetNamedItem("content").Value;
                    else if (meta.Item(i).Attributes.GetNamedItem("name").Value == "dc:subject")
                        this._subject = meta.Item(i).Attributes.GetNamedItem("content").Value;
                    else if (meta.Item(i).Attributes.GetNamedItem("name").Value == "dc:source")
                        this._source = meta.Item(i).Attributes.GetNamedItem("content").Value;
                    else if (meta.Item(i).Attributes.GetNamedItem("name").Value == "dc:title")
                        this._title = meta.Item(i).Attributes.GetNamedItem("content").Value;
                    else if (meta.Item(i).Attributes.GetNamedItem("name").Value == "ncc:charset")
                        this._charset = meta.Item(i).Attributes.GetNamedItem("content").Value;
                    else if (meta.Item(i).Attributes.GetNamedItem("name").Value == "ncc:generator")
                        this._generator = meta.Item(i).Attributes.GetNamedItem("content").Value;
                    else if (meta.Item(i).Attributes.GetNamedItem("name").Value == "ncc:narrator")
                        this._narrator = meta.Item(i).Attributes.GetNamedItem("content").Value;
                    else if (meta.Item(i).Attributes.GetNamedItem("name").Value == "ncc:producer")
                        this._producer = meta.Item(i).Attributes.GetNamedItem("content").Value;
                    else if (meta.Item(i).Attributes.GetNamedItem("name").Value == "ncc:totalTime")
                        this._totalTime = meta.Item(i).Attributes.GetNamedItem("content").Value;
                }
            }

            // Read body
            var bodyElements = doc.GetElementsByTagName("body").Item(0).ChildNodes;
            for (var i = 0; i < bodyElements.Count; i++)
            {
                var href = bodyElements.Item(i).FirstChild.Attributes.GetNamedItem("href").Value.Split('#');
                int level = -1;
                switch (bodyElements.Item(i).Name)
                {
                    case "h1":
                        level = NAV_LEVEL_1;
                        break;
                    case "h2":
                        level = NAV_LEVEL_2;
                        break;
                    case "h3":
                        level = NAV_LEVEL_3;
                        break;
                    case "h4":
                        level = NAV_LEVEL_4;
                        break;
                    case "h5":
                        level = NAV_LEVEL_5;
                        break;
                    case "h6":
                        level = NAV_LEVEL_6;
                        break;
                    case "span":
                        level = NAV_LEVEL_PAGE;
                        this._hasPages = true;
                        break;
                    case "div":
                        level = NAV_LEVEL_PAGE;
                        this._hasPages = true;
                        break;
                }

                if (level <= NAV_LEVEL_6 && this._maxLevels < level)
                {
                    this._maxLevels = level;
                }

                var tmpSmil = new SmilInfo
                {
                    Id = href[1],
                    Title = bodyElements.Item(i).FirstChild.InnerText,
                    Filename = href[0],
                    Level = level
                };

                this._body.Add(tmpSmil);

                string smilPath = $"{Path.GetDirectoryName(filename)}/{tmpSmil.Filename}";
                ParseSmils(smilPath, tmpSmil.Id, tmpSmil.Title, tmpSmil.Level);
            }
        }

        // Read smil file....
        public void ParseSmils(string filename, string id, string title, int level)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            // Read SOM
            var somValue = doc.SelectSingleNode("//meta[@name='ncc:totalElapsedTime']").Attributes.GetNamedItem("content").Value;
            var som = this.Tc2Secs(somValue);

            // Read audio sequences
            var audioElements = doc.SelectSingleNode($"//text[@id='{id}']").ParentNode.SelectNodes("//audio");
            for (var i = 0; i < audioElements.Count; i++)
            {
                var tcin = Ntp2Number(audioElements.Item(i).Attributes.GetNamedItem("clip-begin").Value);
                var tcout = Ntp2Number(audioElements.Item(i).Attributes.GetNamedItem("clip-end").Value);
                _sequence.Add(new Sequence
                {
                    Id = id,
                    Filename = audioElements.Item(i).Attributes.GetNamedItem("src").Value,
                    Title = title,
                    Level = i == 0 ? level : NAV_LEVEL_PHRASE,
                    SOM = som,
                    TCIn = tcin,
                    TCOut = tcout
                });
            }
        }

        public float Ntp2Number(string value)
        {
            value = value.Replace("npt=", "");
            value = value.Replace("s", "");
            return float.Parse(value);
        }

        private float Tc2Secs(string value)
        {
            var parts = value.Split(':');
            return float.Parse(parts[0]) * 3600 +
                    float.Parse(parts[1]) * 60 +
                    float.Parse(parts[2]);
        }
    }
}
