using System.Data.SqlClient;
using Newtonsoft.Json;

namespace DataDumper
{
    public class DataDumper
    {
        private string connectionString;

        public DataDumper (string connString)
        {
            connectionString = connString;
        }

        public object DumpDataIntoSQL(TradeData tradeData)
        {
            bool success = false;
            //string connString = "Server=DEV-HassanREHMA\\SQLEXPRESS;Database=TRADE_ANALYSIS;Trusted_Connection=True;";
            var json = JsonConvert.SerializeObject(tradeData);
            Console.WriteLine($"Inserting Data for Record {json}");
            string cmdString = "INSERT INTO TradeData (TradeDate,TotalOrders,TotalShares,TotalExecutedShares,MarketOrders,MarketShares,MarketExecutedShares,LimitOrders,LimitShares,LimitExecutedShares, TotalNotHeldOrders,NotHeldShares,NotHeldExecutedShares,PeggedOrders,PeggedShares,PeggedExecutedShares,SessionKey) VALUES (@TradeDate, @TotalOrders, @TotalShares,@TotalExecutedShares,@MarketOrders,@MarketShares,@MarketExecutedShares,@LimitOrders,@LimitShares, @LimitExecutedShares,@NotHeldOrders,@NotHeldShares,@NotHeldExecutedShares,@TotalPeggedOrders,@TotalPeggedShares,@TotalExecutedPeggedShares,@SessionKey)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = cmdString;

                    command.Parameters.AddWithValue("@TradeDate", tradeData.TradeDate);
                    command.Parameters.AddWithValue("@TotalOrders", tradeData.TotalOrders);
                    command.Parameters.AddWithValue("@TotalShares", tradeData.TotalShares);
                    command.Parameters.AddWithValue("@TotalExecutedShares", tradeData.TotalExecutedShares);
                    command.Parameters.AddWithValue("@MarketOrders", tradeData.TotalMarketOrders);
                    command.Parameters.AddWithValue("@MarketShares", tradeData.TotalMarketShares);
                    command.Parameters.AddWithValue("@MarketExecutedShares", tradeData.TotalExecutedMarketShares);
                    command.Parameters.AddWithValue("@LimitOrders", tradeData.TotalLimitOrders);
                    command.Parameters.AddWithValue("@LimitShares", tradeData.TotalLimitShares);
                    command.Parameters.AddWithValue("@LimitExecutedShares", tradeData.TotalExecutedLimitShares);
                    command.Parameters.AddWithValue("@NotHeldOrders", tradeData.TotalNotHeldOrders);
                    command.Parameters.AddWithValue("@NotHeldShares", tradeData.TotalNotHeldShares);
                    command.Parameters.AddWithValue("@NotHeldExecutedShares", tradeData.TotalExecutedNotHeldShares);
                    command.Parameters.AddWithValue("@TotalPeggedOrders", tradeData.TotalPeggedOrders);
                    command.Parameters.AddWithValue("@TotalPeggedShares", tradeData.TotalPeggedShares);
                    command.Parameters.AddWithValue("@TotalExecutedPeggedShares", tradeData.TotalExecutedPeggedShares);
                    command.Parameters.AddWithValue("@SessionKey", tradeData.SessionKey);
                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                return success;
            }
        }
    }
}
