using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace TradeAnalysisProject
{
    [Serializable]

    public class DataConfiguration
    {
        [XmlAttribute("Key")]
        public string Key { get; set; }

        [XmlElement("degreeOfParellelism")]
        public int degreeOfParellelism { get; set; }
        [XmlElement("fixInFiles")]
        public FixInFile[] fixInFiles { get; set; }

        [XmlElement("fixInFileDateFormat")]
        public string fixInFileDateFormat { get; set; }

        [XmlElement("fixInFileSeperator")]
        public string fixInFileSeperator { get; set; }

        [XmlElement("fixOutFile")]
        public string fixOutFile { get; set; }

        [XmlElement("fixOutFileDateFormat")]
        public string fixOutFileDateFormat { get; set; }

        [XmlElement("fixOutFileCreate")]
        public string fixOutFileCreate { get; set; }

        [XmlElement("outFile")]
        public string outFile { get; set; }

        [XmlElement("outFileDateFormat")]
        public string outFileDateFormat { get; set; }

        [XmlElement("connectionString")]
        public string connectionString { get; set; }

        [XmlElement("msgSeqNumTag")]
        public string msgSeqNumTag { get; set; }

        [XmlElement("possDupFlagTag")]
        public string possDupFlagTag { get; set; }

        [XmlElement("tagSeperator")]
        public string tagSeperator { get; set; }

        [XmlElement("mpidTAG")]
        public string mpidTAG { get; set; }

        [XmlElement("mpidToCheck")]
        public string mpidToCheck { get; set; }

        [XmlElement("mpidSeperator")]
        public string mpidSeperator { get; set; }

        [XmlElement("sortTAG")]
        public string sortTAG { get; set; }

        [XmlElement("clOrdIdTag")]
        public string clOrdIdTag { get; set; }

        [XmlElement("clParentOrdIdTag")]
        public string clParentOrdIdTag { get; set; }

        [XmlElement("mpidSortSeperator")]
        public string mpidSortSeperator { get; set; }

        [XmlElement("NewOrderTag")]
        public string NewOrderTag { get; set; }

        [XmlElement("NewOrderTagValues")]
        public string NewOrderTagValues { get; set; }

        [XmlElement("separateLegs")]
        public string separateLegs { get; set; }

        [XmlElement("legIdTag")]
        public string legIdTag { get; set; }

        [XmlElement("currentDate")]
        public string currentDate { get; set; }

        [XmlElement("sourceFileName")]
        public string sourceFileName { get; set; }

        [XmlElement("checkDupes")]
        public bool checkDupes { get; set; }

        [XmlElement("addDay")]
        public string addDay { get; set; }

        [XmlElement("addHour")]
        public string addHour { get; set; }

        [XmlElement("addMinutes")]
        public string addMinutes { get; set; }

        [XmlElement("addSeconds")]
        public string addSeconds { get; set; }

        [XmlElement("preSortingTags")]
        public string preSortingTags { get; set; }

        [XmlElement("postSortingTags")]
        public string postSortingTags { get; set; }

        [XmlElement("mpids")]
        public string mpids { get; set; }

        [XmlElement("mpidTags")]
        public string mpidTags { get; set; }

    }

    public class FixInFile
    {
        [XmlAttribute("Key")]
        public string Key { get; set; }
        [XmlAttribute("Path")]
        public string Path { get; set; }
        [XmlAttribute("DateFormat")]
        public string DateFormat { get; set; }
    }
}
