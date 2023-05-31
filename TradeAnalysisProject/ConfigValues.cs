using System;
using System.Collections.Generic;
using TradeAnalysisProject;

namespace TradeAnalysisProject
{
    public class ConfigValues
    {
        public FixInFile[] fixInFile { get; }
        public string fixInFileDateFormat { get; }
        public char fixInFileSeperator { get; }
        public string fixOutFile { get; }
        public string fixOutFileDateFormat { get; }
        public string fixOutFileCreate { get; }
        public string outFile { get; }
        public string outFileDateFormat { get; }
        public string connectionString { get; }
        public string msgSeqNumTag { get; }
        public string possDupFlagTag { get; }
        public string tagSeperator { get; }
        public string mpidTAG { get; }
        public string mpidToCheck { get; }
        public string mpidSeperator { get; }
        public string sortTAG { get; }
        public string clOrdIdTag { get; }
        public string clParentOrdIdTag { get; }
        public string mpidSortSeperator { get; }
        public string NewOrderTag { get; }
        public string NewOrderTagValues { get; }
        public string separateLegs { get; }
        public string addDay { get; }
        public string addHour { get; }
        public string addMinutes { get; }
        public string addSeconds { get; }
        public string legIdTag { get; }
        public string currentDate { get; set; }
        public string sourceFileName { get; }
        public bool checkDupes { get; }
        public string[] preSortingTags { get; }
        public string[] postSortingTags { get; }
        public List<string> mpids { get; }
        public List<string> mpidTags { get; }

        public ConfigValues(FixInFile[] fixInFile, string fixInFileDateFormat, char fixInFileSeperator, string NewOrderTag, string NewOrderTagValues, string fixOutFile, string fixOutFileDateFormat, string fixOutFileCreate, string outFile, string outFileDateFormat, string msgSeqNumTag, string possDupFlagTag, string mpidTAG, string TAGSeperator, string mpidToCheck, string mpidSeperator, string sortTAG, string clOrdIdTag, string addDay, string addHour, string addMinutes, string addSeconds, string clParentOrdIdTag, string mpidSortSeperator, string separateLegs, string legIdTag, bool checkDupes, string[] preSortingTags, string[] postSortingTags, string connectionString, DateOnly currentDate)
        {
            this.fixInFile = fixInFile;
            this.fixInFileDateFormat = fixInFileDateFormat;
            this.fixInFileSeperator = fixInFileSeperator;
            this.fixOutFile = fixOutFile;
            this.fixOutFileDateFormat = fixOutFileDateFormat;
            this.fixOutFileCreate = fixOutFileCreate;
            this.connectionString = connectionString;
            this.outFile = outFile;
            this.outFileDateFormat = outFileDateFormat;
            this.msgSeqNumTag = msgSeqNumTag;
            this.possDupFlagTag = possDupFlagTag;
            this.mpidTAG = mpidTAG;
            this.tagSeperator = TAGSeperator;
            this.mpidToCheck = mpidToCheck;
            this.mpidSeperator = mpidSeperator;
            this.sortTAG = sortTAG;
            this.clOrdIdTag = clOrdIdTag;
            this.addDay = addDay;
            this.addHour = addHour;
            this.addMinutes = addMinutes;
            this.addSeconds = addSeconds;
            this.clParentOrdIdTag = clParentOrdIdTag;
            this.mpidSortSeperator = mpidSortSeperator;
            this.NewOrderTag = NewOrderTag;
            this.NewOrderTagValues = NewOrderTagValues;
            this.separateLegs = separateLegs;
            this.legIdTag = legIdTag;
            this.checkDupes = checkDupes;
            this.preSortingTags = preSortingTags;
            this.postSortingTags = postSortingTags;
            this.currentDate = currentDate.ToString(fixInFileDateFormat);
            //this.sourceFileName = fixInFile.Replace("%DATE%", this.currentDate);

            this.mpids = new List<string>();
            this.mpidTags = new List<string>();
        }
    } 
}
