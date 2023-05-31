using System.Collections.Generic;

namespace TradeAnalysisProject
{
    public class SorterClass
    {
        private List<dynamic> m_sortTagValues;
        private string m_stMessage;

        public List<dynamic> SortTagValues
        {
            get
            {
                return this.m_sortTagValues;
            }
        }

        public string StMessage { 
            get
            { 
                return this.m_stMessage; 
            }
        }

        public SorterClass ()
        {
            m_sortTagValues = new List<dynamic>();
            m_stMessage = "";
        }
        public SorterClass (List<dynamic> sortTagValues, string stMessage)
        {
            m_sortTagValues = sortTagValues;
            m_stMessage = stMessage;
        }


    }
}
