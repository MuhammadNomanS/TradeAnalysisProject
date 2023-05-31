using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using DataDumper;

namespace TradeAnalysisProject
{

    public class DataParser
    {
        public TradeData GetAggregatedDataFromFullOrderLC (List<List<string>> ls_fullOrderLC, DateOnly tradeDate, string Key)
        {
            TradeData aggTradeData = new TradeData ();
            for (int i = 0; i < ls_fullOrderLC.Count; i++) {
                var record = ParseDataForFullOrderLC(ls_fullOrderLC[i]);
                if (i == 0)
                {
                    aggTradeData.TradeDate = tradeDate.ToDateTime(new TimeOnly()).Date;
                    aggTradeData.SessionKey = Key;
                }

                AggregateTradesFromOrders(ref aggTradeData, record);
            }
            return aggTradeData;
            
        }

        private void AggregateTradesFromOrders(ref TradeData aggTradeData, TradeData record)
        {
            aggTradeData.TotalOrders += record.TotalOrders;
            aggTradeData.TotalShares += record.TotalShares;
            aggTradeData.TotalExecutedShares += record.TotalExecutedShares;
            aggTradeData.TotalLimitOrders += record.TotalLimitOrders;
            aggTradeData.TotalLimitShares += record.TotalLimitShares;
            aggTradeData.TotalExecutedLimitShares += record.TotalExecutedLimitShares;
            aggTradeData.TotalMarketOrders += record.TotalMarketOrders;
            aggTradeData.TotalMarketShares += record.TotalMarketShares;
            aggTradeData.TotalExecutedMarketShares += record.TotalExecutedMarketShares;
            aggTradeData.TotalNotHeldOrders += record.TotalNotHeldOrders;
            aggTradeData.TotalNotHeldShares += record.TotalNotHeldShares;
            aggTradeData.TotalExecutedNotHeldShares += record.TotalExecutedNotHeldShares;
            aggTradeData.TotalPeggedOrders += record.TotalPeggedOrders;
            aggTradeData.TotalPeggedShares += record.TotalPeggedShares;
            aggTradeData.TotalExecutedPeggedShares += record.TotalExecutedPeggedShares;
        }

        public TradeData ParseDataForFullOrderLC (List<string> records)
        {
            TradeData data = new TradeData();
            //var lastExec = records.Last();
            //var lastExecType = CommonMethods.GetField(150, lastExec, '\x0001');
            TraderDataFromExec(data, records);
            return data;
        }

        private static void TraderDataFromExec(TradeData data, List<string> records)
        {
            try
            {
                var lastExec = records.Last();
                data.TotalOrders = 1;
                data.TotalShares = (long)double.Parse(CommonMethods.GetField(38, lastExec, '\x0001'));
                var leaves= (long)double.Parse(CommonMethods.GetField(151, lastExec, '\x0001'));
                data.TotalExecutedShares = records
                    .Where(x =>
                    {
                        var execType = CommonMethods.GetField(150, x, '\x0001');
                        return execType == "1" || execType == "2";
                    })
                    .Sum(x => (long)double.Parse(CommonMethods.GetField(32, x, '\x0001')));

                var tempExecutedShares = data.TotalShares - leaves;

                var OrdType = CommonMethods.GetField(40, lastExec, '\x0001');
                if (OrdType == "1" || OrdType == "4")
                {
                    data.TotalMarketOrders = data.TotalOrders;
                    data.TotalMarketShares = data.TotalShares;
                    data.TotalExecutedMarketShares = data.TotalExecutedShares;
                }
                else if (OrdType == "2" || OrdType == "3")
                {
                    data.TotalLimitOrders = data.TotalOrders;
                    data.TotalLimitShares = data.TotalShares;
                    data.TotalExecutedLimitShares = data.TotalExecutedShares;
                }
                else if (OrdType == "P")
                {
                    data.TotalPeggedOrders = data.TotalOrders;
                    data.TotalPeggedShares = data.TotalShares;
                    data.TotalExecutedPeggedShares = data.TotalExecutedShares;
                }
                else
                {
                    var price = CommonMethods.GetField(44, lastExec, '\x0001');
                    if (string.IsNullOrEmpty(price) || price == "0")
                    {
                        data.TotalMarketOrders = data.TotalOrders;
                        data.TotalMarketShares = data.TotalShares;
                        data.TotalExecutedMarketShares = data.TotalExecutedShares;
                    }
                    else
                    {
                        data.TotalLimitOrders = data.TotalOrders;
                        data.TotalLimitShares = data.TotalShares;
                        data.TotalExecutedLimitShares = data.TotalExecutedShares;
                    }
                }
                if (CommonMethods.GetField(18, lastExec, '\x0001').Split(" ").Contains("1"))
                {
                    data.TotalNotHeldOrders = data.TotalOrders;
                    data.TotalNotHeldShares = data.TotalShares;
                    data.TotalExecutedNotHeldShares = data.TotalExecutedShares;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
