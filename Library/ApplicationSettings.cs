using Library;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Library
{
    public class ApplicationSettings : PropertyChangedBase
    {
        private string switch1;
        private string switch2;
        private string priority;
        private int playerTimeInMinutes;

        [XmlIgnore]
        public string BoardXmlPathSwitch1 => @"..\..\..\..\Library\Txt\" + Switch1 + ".txt";

        [XmlIgnore]
        public string BoardXmlPathSwitch2 => @"..\..\..\..\Library\Txt\" + Switch2 + ".txt";

        [XmlIgnore]
        public string BoardXmlPathPriority => @"..\..\..\..\Library\Txt\" + Priority + ".txt";

        public string Switch1
        {
            get { return switch1; }
            set { RaisePropertyChanged(ref switch1, value); }
        }

        public string Switch2
        {
            get { return switch2; }
            set { RaisePropertyChanged(ref switch2, value); }
        }

        public string Priority
        {
            get { return priority; }
            set { RaisePropertyChanged(ref priority, value); }
        }

        public int PlayerTimeInMinutes
        {
            get { return playerTimeInMinutes; }
            set { RaisePropertyChanged(ref playerTimeInMinutes, value); }
        }
    }
}
