using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using DataDumper;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TradeAnalysisProject
{
    public class ConcreteKeyedCollection<TKey, TValue> : KeyedCollection<TKey, TValue>
    {
        private Func<TValue, TKey> keySelector;

        public ConcreteKeyedCollection(Func<TValue, TKey> keySelector)
        {
            this.keySelector = keySelector;
        }
        protected override TKey GetKeyForItem(TValue item)
        {
            return keySelector(item);
        }
    }

    internal class FIXLogFileTransformer
    {
        private static AppSettingsReader reader = new AppSettingsReader();

        private static string getAppConfigValue(string stKey, string stDefaultValue)
        {
            try
            {
                if (string.IsNullOrEmpty(stKey))
                    return stDefaultValue;
                return stKey;
            }
            catch (Exception ex)
            {
                return stDefaultValue;
            }
        }
        private static char getAppConfigValue(string stKey, char stDefaultValue)
        {
            try
            {
                if (string.IsNullOrEmpty(stKey))
                    return stDefaultValue;
                return char.Parse(stKey);
            }
            catch (Exception ex)
            {
                return stDefaultValue;
            }
        }

        public static int FIXCleanup(DataConfiguration _dataConfig, DateOnly currentDate, int index)
        {
            ConfigValues configValues = GetConfigValues(_dataConfig, currentDate);
            List<string> FixMessages = GetMessages(index, configValues);
            if (FixMessages == null)
                return 0;

            List<List<string>> OrderLifeCycles = new List<List<string>>();
            Dictionary<string, List<string>> ClOrderIDAndLifeCycleMapping = new Dictionary<string, List<string>>();

            string newValue3 = currentDate.ToString(configValues.outFileDateFormat);
            string path = configValues.outFile.Replace("%DATE%", newValue3);
            string file = Path.GetFileNameWithoutExtension(path);
            string path2 = path.Replace(file, $"Non-Reportable_{_dataConfig.fixInFiles[index].Key}_{file}");
            using (StreamWriter sw = new StreamWriter(path2))
            {

                foreach (var str2 in FixMessages)
                {
                    string ClOrdID = CommonMethods.GetField(configValues.clOrdIdTag, str2, configValues.fixInFileSeperator);
                    string ClParentOrdID = CommonMethods.GetField(configValues.clParentOrdIdTag, str2, configValues.fixInFileSeperator);
                    string OrdID = CommonMethods.GetField(37, str2, configValues.fixInFileSeperator);
                    string execType = CommonMethods.GetField(150, str2, configValues.fixInFileSeperator);

                    if (execType == "8")
                        continue;

                    if (configValues.separateLegs == "true")
                    {
                        string legId = CommonMethods.GetField(configValues.legIdTag, str2, configValues.fixInFileSeperator);
                        ClOrdID = string.IsNullOrEmpty(ClOrdID) ? ClOrdID : ClOrdID + "-" + legId;
                        ClParentOrdID = string.IsNullOrEmpty(ClParentOrdID) ? ClParentOrdID : ClParentOrdID + "-" + legId;
                    }

                    List<string> lifeCycle;
                    if (!string.IsNullOrEmpty(ClOrdID) && ClOrderIDAndLifeCycleMapping.ContainsKey(ClOrdID))
                    {
                        lifeCycle = ClOrderIDAndLifeCycleMapping[ClOrdID];
                    }
                    else if (!string.IsNullOrEmpty(ClParentOrdID) && ClOrderIDAndLifeCycleMapping.ContainsKey(ClParentOrdID))
                    {
                        ClOrderIDAndLifeCycleMapping[ClOrdID] = ClOrderIDAndLifeCycleMapping[ClParentOrdID];
                        lifeCycle = ClOrderIDAndLifeCycleMapping[ClOrdID];
                    }
                    else if (!string.IsNullOrEmpty(OrdID) && ClOrderIDAndLifeCycleMapping.ContainsKey(OrdID))
                    {
                        ClOrderIDAndLifeCycleMapping[OrdID] = ClOrderIDAndLifeCycleMapping[OrdID];
                        lifeCycle = ClOrderIDAndLifeCycleMapping[OrdID];
                    }
                    else
                    {
                        //if (!string.IsNullOrEmpty(configValues.NewOrderTag))
                        //{
                        if (IsValidLifeCycle(configValues.NewOrderTag, str2, configValues.fixInFileSeperator, configValues.NewOrderTagValues, configValues.tagSeperator))
                        {
                            lifeCycle = new List<string>();
                            ClOrderIDAndLifeCycleMapping.Add(ClOrdID, lifeCycle);

                            OrderLifeCycles.Add(lifeCycle);
                        }
                        else if (!String.IsNullOrEmpty(OrdID))
                        {
                            lifeCycle = new List<string>();
                            ClOrderIDAndLifeCycleMapping.Add(OrdID, lifeCycle);
                            OrderLifeCycles.Add(lifeCycle);
                        }
                        else
                        {
                            string lastShares = CommonMethods.GetField(32, str2, configValues.fixInFileSeperator);
                            string orderQty = CommonMethods.GetField(38, str2, configValues.fixInFileSeperator);
                            if (lastShares == orderQty)
                            {
                                lifeCycle = new List<string>();
                                ClOrderIDAndLifeCycleMapping.Add(Guid.NewGuid().ToString(), lifeCycle);
                                OrderLifeCycles.Add(lifeCycle);
                            }
                            else
                            {
                                lifeCycle = null;
                                sw.WriteLine(str2);
                            }
                        }
                        //}
                    }

                    lifeCycle?.Add(str2);
                }
            }
            
            List<List<string>> ls_fullOrderLC = new List<List<string>>();
            List<List<string>> ls_onlyFillsLC = new List<List<string>>();
            Dictionary<string, List<string>> dc_onlyFills = new Dictionary<string, List<string>>();

            foreach (var lifecycle in OrderLifeCycles)
            {
                List<string> tempLifeCycle = lifecycle;
                if (configValues.postSortingTags != null)
                {
                    tempLifeCycle = MessagesSorter(lifecycle, configValues.postSortingTags, configValues.fixInFileSeperator, '=');
                }
                //if (!string.IsNullOrEmpty(configValues.NewOrderTag))
                //{
                //    if (IsValidLifeCycle(configValues.NewOrderTag, tempLifeCycle[0], configValues.fixInFileSeperator, configValues.NewOrderTagValues, configValues.tagSeperator))
                //    {
                        //writeOrderLifeCycles(streamWriter, tempLifeCycle);
                        ls_fullOrderLC.Add(tempLifeCycle);
                    //}
                    //else
                    //{
                    //    //File.AppendAllLines(path2, tempLifeCycle);

                    //    ls_onlyFillsLC.Add(tempLifeCycle);
                    //}
                //}
                //else
                //{
                //    ls_fullOrderLC.Add(tempLifeCycle);
                //}
            }

            DataParser dataParser = new DataParser();

            var tradeData = dataParser.GetAggregatedDataFromFullOrderLC(ls_fullOrderLC, currentDate, _dataConfig.fixInFiles[index].Key);

            DataDumper.DataDumper dataDumper = new DataDumper.DataDumper(_dataConfig.connectionString);
            dataDumper.DumpDataIntoSQL(tradeData);

            return 0;
        }

        private static ConfigValues GetConfigValues (DataConfiguration _dataConfig, DateOnly _currentDate)
        {
            var configValues = new ConfigValues(
                fixInFile: _dataConfig.fixInFiles,
                fixInFileDateFormat: getAppConfigValue(_dataConfig.fixInFileDateFormat, ""),
                fixInFileSeperator: getAppConfigValue(_dataConfig.fixInFileSeperator , '\x0001'),
                fixOutFile: getAppConfigValue(_dataConfig.fixOutFile, ""),
                fixOutFileDateFormat: getAppConfigValue(_dataConfig.fixOutFileDateFormat, ""),
                fixOutFileCreate: getAppConfigValue(_dataConfig.fixOutFileCreate, "false"),
                connectionString: getAppConfigValue(_dataConfig.connectionString, ""),
                outFile: getAppConfigValue(_dataConfig.outFile, ""),
                outFileDateFormat: getAppConfigValue(_dataConfig.outFileDateFormat, ""),
                msgSeqNumTag: getAppConfigValue(_dataConfig.msgSeqNumTag, "34"),
                possDupFlagTag: getAppConfigValue(_dataConfig.possDupFlagTag, "43"),
                TAGSeperator: getAppConfigValue(_dataConfig.tagSeperator, ","),
                mpidTAG: getAppConfigValue(_dataConfig.mpidTAG, ""),
                mpidToCheck: getAppConfigValue(_dataConfig.mpidToCheck, ""),
                mpidSeperator: getAppConfigValue(_dataConfig.mpidSeperator, ","),
                sortTAG: getAppConfigValue(_dataConfig.sortTAG, ""),
                clOrdIdTag: getAppConfigValue(_dataConfig.clOrdIdTag, "11"),
                addDay: getAppConfigValue(_dataConfig.addDay, "0"),
                addHour: getAppConfigValue(_dataConfig.addHour, "0"),
                addMinutes: getAppConfigValue(_dataConfig.addMinutes, "0"),
                addSeconds: getAppConfigValue(_dataConfig.addSeconds, "0"),
                clParentOrdIdTag: getAppConfigValue(_dataConfig.clParentOrdIdTag, "41"),
                mpidSortSeperator: getAppConfigValue(_dataConfig.mpidSortSeperator, ","),
                NewOrderTag: getAppConfigValue(_dataConfig.NewOrderTag, "35"),
                NewOrderTagValues: getAppConfigValue(_dataConfig.NewOrderTagValues, "D"),
                separateLegs: getAppConfigValue(_dataConfig.separateLegs, "false"),
                legIdTag: getAppConfigValue(_dataConfig.legIdTag, "654"),
                checkDupes: bool.Parse(getAppConfigValue(_dataConfig.checkDupes.ToString(), "true")),
                preSortingTags: GetSperatedTags(_dataConfig.preSortingTags, _dataConfig.tagSeperator),
                postSortingTags: GetSperatedTags(_dataConfig.postSortingTags, _dataConfig.tagSeperator),
                currentDate: _currentDate
            );
            if (configValues.mpidTAG.Contains(configValues.tagSeperator))
            {
                configValues.mpidTags.AddRange(GetSperatedTags(configValues.mpidTAG, configValues.tagSeperator));
            }
            else
            {
                configValues.mpidTags.Add(configValues.mpidTAG);
            }

            if (configValues.mpidToCheck.Contains(configValues.tagSeperator))
            {
                configValues.mpids.AddRange(GetSperatedTags(configValues.mpidToCheck, configValues.tagSeperator));
            }
            else
            {
                configValues.mpids.Add(configValues.mpidToCheck);
            }
            return configValues;
        }
        private static List<string> GetMessages (int index, ConfigValues configValues)
        {
            string str1;
            bool hasDupes = false;
            List<string> messages = new List<string>();
            if (configValues.fixOutFileCreate.Length > 0 && configValues.fixOutFileCreate.ToLower() == "true")
            {
                System.TimeSpan duration = new System.TimeSpan(int.Parse(configValues.addDay), int.Parse(configValues.addHour), int.Parse(configValues.addMinutes), int.Parse(configValues.addSeconds));
                string newValue2 = DateTime.Now.Add(duration).ToString(configValues.fixOutFileDateFormat);
                str1 = configValues.fixOutFile.Replace("%DATE%", newValue2);
                var offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
                File.Copy(configValues.sourceFileName, str1);
            }
            else
                str1 = configValues.fixInFile[index].Path.Replace("%DATE%",configValues.currentDate);

            HashSet<string> MsgSeqNumsSet = new HashSet<string>();

            try
            {
                StreamReader streamReader = new StreamReader(File.Open(str1, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                Console.WriteLine($"Reading Data from File {str1}");
                while (streamReader.Peek() >= 0)
                {
                    string stMessage = streamReader.ReadLine();
                    if (IsValidMessage(stMessage, configValues.mpidTags, configValues.mpids, configValues))
                    {
                        string curMsgSeqNum = CommonMethods.GetField(configValues.msgSeqNumTag, stMessage, configValues.fixInFileSeperator);
                        if (configValues.checkDupes)
                        {
                            if (MsgSeqNumsSet.Contains(curMsgSeqNum))
                            {
                                hasDupes = true;
                                continue;
                            }
                        }
                        MsgSeqNumsSet.Add(curMsgSeqNum);
                        string str2 = stMessage.Substring(stMessage.IndexOf("8=FIX"));
                        messages.Add(str2);
                    }
                }

                if (hasDupes)
                    return MessagesSorter(messages, configValues.preSortingTags, configValues.fixInFileSeperator, '=');

                return messages;
            }
            catch (Exception ex)
            {

                Console.WriteLine($"File Missing for path {str1}");
                return null;
            }
        }
        private static Dictionary<string, dynamic> GetKeyValuePairs(string stMessage, char seperator, char keyValueSeperator)
        {
            Dictionary<string, dynamic> keyValuePairs = new Dictionary<string, dynamic>();
            try
            {
                var kvPairs = stMessage.Split(seperator);
                foreach (string kvp in kvPairs)
                {
                    if (!string.IsNullOrEmpty(kvp))
                    {
                        int indxOfsep = kvp.IndexOf(keyValueSeperator);
                        int intVal;
                        bool b = int.TryParse(kvp.Substring(indxOfsep + 1), out intVal);
                        if (b)
                            keyValuePairs.Add(kvp.Substring(0, indxOfsep), intVal);
                        if (!b)
                            keyValuePairs.Add(kvp.Substring(0, indxOfsep), kvp.Substring(indxOfsep + 1));
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return keyValuePairs;
        }

        /// <summary>
        /// Checks if message is valid
        /// </summary>
        /// <param name="stMessage"></param>
        /// <param name="mpidTags"></param>
        /// <param name="mpids"></param>
        /// <returns></returns>
        private static bool IsValidMessage(string stMessage, List<string> mpidTags, List<string> mpids, ConfigValues configValues)
        {
            if (stMessage.Length != 0 && stMessage.IndexOf("8=FIX") >= 0)
            {
                string msgType = CommonMethods.GetField(35, stMessage, configValues.fixInFileSeperator);
                if (!(msgType == "5") && !(msgType == "A") &&
                    (!(msgType == "0") && !(msgType == "2")) &&
                    (!(msgType == "1") && !(msgType == "4")))
                {
                    foreach (var mpidTag in mpidTags)
                    {
                        string tagValue = CommonMethods.GetField(mpidTag, stMessage, configValues.fixInFileSeperator);
                        foreach (var mpid in mpids)
                        {
                            if (mpid == tagValue)
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        private static string[] GetSperatedTags (string tagString, string seperator)
        {
            if (string.IsNullOrEmpty(tagString))
                return null;
            return tagString.Split(seperator.ToCharArray());
        }

        private static bool IsValidLifeCycle(string stField, string stMessage, char separator, string stValue, string tagSeperator)
        {
            try
            {
                string[] newOrderTagValues = GetSperatedTags(stValue, tagSeperator);
                if (newOrderTagValues.Contains(CommonMethods.GetField(stField, stMessage, separator)))
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Sorts a list of messages according to certain 
        /// </summary>
        /// <param name="lifecycle"></param>
        /// <param name="sortTag"></param>
        /// <param name="seperator"></param>
        /// <param name="kvpSeperator"></param>
        /// <returns></returns>
        private static List<string> MessagesSorter (List<string> lifecycle, string[] sortTag, char seperator, char kvpSeperator)
        {
            List<SorterClass> sorters = new List<SorterClass>();
            foreach (string stMessage in lifecycle)
            {
                List<dynamic> tagValues = new List<dynamic>();

                foreach (string tag in sortTag)
                {
                    var val = CommonMethods.GetField(tag, stMessage, seperator);
                    bool isInt = int.TryParse(val, out int intTemp);

                    switch (isInt)
                    {
                        case true:
                            tagValues.Add(intTemp);
                            break;

                        case false:
                            tagValues.Add(val);
                            break;

                        default:
                            tagValues.Add(val);
                            break;
                    }
                }

                sorters.Add(new SorterClass(tagValues, stMessage));
            }

            var sorted = sorters.OrderBy(x => x.SortTagValues[0]);
            for (int i = 1; i < sortTag.Length; i++)
            {
                var index = i;
                sorted = sorted.ThenBy(x => x.SortTagValues[index]);
            }
            return (sorted.ToList()).Select(x => x.StMessage).ToList();
        }

        
    }
}
