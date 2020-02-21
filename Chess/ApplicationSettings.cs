using Library;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Chess
{
    public class ApplicationSettings : PropertyChangedBase
    {
        private string boardXmlName;
        private int playerTimeInMinutes;

        [XmlIgnore]
        public string BoardXmlPath => @"..\..\..\..\Library\Xml\" + BoardXmlName + ".xml";       

        public string BoardXmlName
        {
            get { return boardXmlName; }
            set { RaisePropertyChanged(ref boardXmlName, value); }
        }

        public int PlayerTimeInMinutes
        {
            get { return playerTimeInMinutes; }
            set { RaisePropertyChanged(ref playerTimeInMinutes, value); }
        }
    }
}
