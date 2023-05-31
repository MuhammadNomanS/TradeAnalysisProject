namespace TradeAnalysisProject
{
    internal static class CommonMethods
    {

        public static string GetField(string stField, string stMessage, char separator)
        {
            try
            {
                string str1 = "";
                stField += "=";
                string str2 = separator.ToString() + stField;
                int num1 = !(stField == "8=") ? stMessage.IndexOf(str2, 0,StringComparison.Ordinal) : 0;
                if (num1 >= 0)
                {
                    int num2 = stMessage.IndexOf(separator.ToString(), num1 + stField.Length, StringComparison.Ordinal);
                    if (num2 >= 0)
                    {
                        int index = num1 + stField.Length + 1;
                        int length = num2 - num1 - stField.Length - 1;

                        str1 = stMessage.Substring(index, length);
                    }
                }
                return str1;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        public static string GetField(int nField, string stMessage, char separator)
        {
            return GetField(nField.ToString(), stMessage, separator);
        }
    }
}