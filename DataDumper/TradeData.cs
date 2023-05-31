namespace DataDumper
{
    public class TradeData
    {
        public DateTime TradeDate { get; set; }
        public Int64 TotalOrders { get; set; }
        public Int64 TotalShares { get; set; }
        public Int64 TotalExecutedShares { get; set; }
        public Int64 TotalMarketOrders { get; set; }
        public Int64 TotalMarketShares { get; set; }
        public Int64 TotalExecutedMarketShares { get; set; }
        public Int64 TotalLimitOrders { get; set; }
        public Int64 TotalLimitShares { get; set; }
        public Int64 TotalExecutedLimitShares { get; set; }
        public Int64 TotalNotHeldOrders { get; set; }
        public Int64 TotalNotHeldShares { get; set; }
        public Int64 TotalExecutedNotHeldShares { get; set; }
        public Int64 TotalPeggedOrders { get; set; }
        public Int64 TotalPeggedShares { get; set; }
        public Int64 TotalExecutedPeggedShares { get; set; }
        public string SessionKey { get; set; }

    }
}
