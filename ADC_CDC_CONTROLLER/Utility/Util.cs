using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ADC_CDC_CONTROLLER
{
    static class Util
    {

        public static string MidStrEx(string sourse, string startstr, string endstr)
        {
            string result = string.Empty;
            int startindex, endindex;
            try
            {
                startindex = sourse.IndexOf(startstr);
                if (startindex == -1)
                    return result;
                string tmpstr = sourse.Substring(startindex + startstr.Length);
                endindex = tmpstr.IndexOf(endstr);
                if (endindex == -1)
                    return result;
                result = tmpstr.Remove(endindex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return result;
        }
        public static List<string> MidStrExAll(string sourse, string startstr, string endstr)
        {
            List<string> result = new List<string>();
            int startindex, endindex;
            int offsetIndex = 0;
            try
            {
                while (offsetIndex < sourse.Length)
                {
                    startindex = sourse.IndexOf(startstr, offsetIndex);
                    if (startindex == -1)
                        return result;
                    string tmpstr = sourse.Substring(startindex + startstr.Length);
                    endindex = tmpstr.IndexOf(endstr);
                    if (endindex == -1)
                        return result;
                    result.Add(tmpstr.Remove(endindex));
                    offsetIndex = endindex;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return result;
        }
        public static string ToHexStrFromByte(byte[] byteDatas)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < byteDatas.Length; i++)
            {
                builder.Append(string.Format("{0:X2} ", byteDatas[i]));
            }
            return builder.ToString().Trim();
        }

        public static int GetIndexOf(byte[] b, byte[] bb)
        {
            if (b == null || bb == null || b.Length == 0 || bb.Length == 0 || b.Length < bb.Length)
                return -1;
            int i, j;
            for (i = 0; i < b.Length - bb.Length + 1; i++)
            {
                if (b[i] == bb[0])
                {
                    for (j = 1; j < bb.Length; j++)
                    {
                        if (b[i + j] != bb[j])
                            break;
                    }
                    if (j == bb.Length)
                        return i;
                }
            }
            return -1;
        }
    }
}
