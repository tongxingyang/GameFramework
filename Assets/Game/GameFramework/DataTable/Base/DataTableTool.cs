using System.Linq;

namespace GameFramework.DataTable.Base
{
    public class DataTableTool
    {
        public static char[] eof = {'\r', '\n'};
        
        public static string[] SplitLine(string line)
        {
            line = line.TrimEnd(eof);
            return SplitCsvLine(line);
        }

        private static string CsvLineSeperator = @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)";

        private static string[] SplitCsvLine(string line)
        {
            var value = (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(
                    line,
                    CsvLineSeperator, System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
                select m.Groups[1].Value);
            return value.ToArray();
        }
    }
}