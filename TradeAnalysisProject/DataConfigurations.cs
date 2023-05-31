using System;
using System.Xml;
using System.Configuration;
using System.Xml.Serialization;

namespace TradeAnalysisProject
{
    [Serializable]
    public class DataConfigurations
    {
        [XmlElement("DataConfiguration")]
        public DataConfiguration[] DataConfigurationCollection { get; set; }

        public bool LoadConfigs(out DataConfigurations dataConfigurations)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(DataConfigurations));
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath);
                XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName(nameof(DataConfigurations));
                dataConfigurations = (DataConfigurations)xmlSerializer.Deserialize(new XmlNodeReader(elementsByTagName[0]));
                //DataConfigurationCollection = dataConfigurations.DataConfigurationCollection;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
    }

    
}
