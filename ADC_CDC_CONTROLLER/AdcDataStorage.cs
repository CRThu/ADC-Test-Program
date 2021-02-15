using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADC_CDC_CONTROLLER
{
    public enum DataStroageExtension
    {
        Csv
    }

    class AdcDataStorage
    {
        // TODO ADD INFO
        public string tmpAdcSampleInfo;
        public List<ulong> tmpAdcSample;
        // Replace string to settings
        public Dictionary<string, string> AdcSamplesInfo;
        public Dictionary<string, List<ulong>> AdcSamplesStorage;

        public int AdcSamplesCount
        {
            get
            {
                return AdcSamplesStorage.Count;
            }
        }
        public string[] AdcSamplesStorageKeys
        {
            get
            {
                return AdcSamplesStorage.Select(samples => samples.Key).ToArray();
            }
        }

        public AdcDataStorage()
        {
            tmpAdcSampleInfo = "";
            tmpAdcSample = new List<ulong>();
            AdcSamplesInfo = new Dictionary<string, string>();
            AdcSamplesStorage = new Dictionary<string, List<ulong>>();
        }

        public void WriteTmpAdcSamples(string info, List<byte> byteList, int bytesPerCode)
        {
            tmpAdcSampleInfo = info;
            tmpAdcSample = ConvertBytesToCodes(byteList, bytesPerCode);
        }
        public int WriteTmpAdcSamplesToDataStorage(string name)
        {
            return WriteToDataStorage(name, tmpAdcSampleInfo, tmpAdcSample);
        }
        public int WriteToDataStorage(string name, string info, List<ulong> samples)
        {

            if (!AdcSamplesInfo.ContainsKey(name))
                AdcSamplesInfo.Add(name, info);
            else
                AdcSamplesInfo[name] = info;

            if (!AdcSamplesStorage.ContainsKey(name))
                AdcSamplesStorage.Add(name, samples);
            else
                AdcSamplesStorage[name] = samples;

            return samples.Count;
        }
        public List<ulong> ReadTmpAdcSamples()
        {
            return tmpAdcSample;
        }
        public List<ulong> ReadDataStorage(string key)
        {
            return AdcSamplesStorage[key];
        }
        public void StoretmpAdcSamplesToFile(string path, DataStroageExtension ext)
        {
            Dictionary<string, string> tmpSamplesInfoKvp = new Dictionary<string, string>
            {
                {"tmpAdcSamples",tmpAdcSampleInfo }
            };
            Dictionary<string, List<ulong>> tmpAdcSamplesDataKvp = new Dictionary<string, List<ulong>>
            {
                { "tmpAdcSamples", tmpAdcSample }
            };

            if (ext == DataStroageExtension.Csv)
                StoreDataToCsv(path, tmpSamplesInfoKvp, tmpAdcSamplesDataKvp);
        }
        public void StoreAllDataToFile(string path, DataStroageExtension ext)
        {
            if (ext == DataStroageExtension.Csv)
                StoreDataToCsv(path,AdcSamplesInfo, AdcSamplesStorage);
        }
        private void StoreDataToCsv(string path, Dictionary<string, string> info, Dictionary<string, List<ulong>> data)
        {
            // Fomart
            // NAME,INFO must not contain ','
            // NAME1,INFO1,1,1,1,1,
            // NAME2,INFO2,1,1,1,1,1,1,
            // NAME3,INFO3,1,1,1,1,1,1,1,1,
            
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            foreach (var kvp in data)
            {
                sw.Write(kvp.Key + ",");
                sw.Write(info[kvp.Key] + ",");
                foreach (var code in kvp.Value)
                    sw.Write(code + ",");
                sw.WriteLine();
            }
            sw.Flush();
            sw.Close();
        }
        public void LoadAllDataFromFile(string path, DataStroageExtension ext)
        {
            if (ext == DataStroageExtension.Csv)
                LoadAllDataFromCsv(path);
        }
        private void LoadAllDataFromCsv(string path)
        {
            AdcSamplesStorage.Clear();
            AdcSamplesInfo.Clear();
            using (StreamReader sr = new StreamReader(path))
            {
                while (sr.Peek() > 0)
                {
                    string line = sr.ReadLine();
                    List<string> elements = line.Split(new char[] { ',' }).Where(str => !string.IsNullOrEmpty(str)).ToList();
                    string key = elements[0];
                    string info = elements[1];
                    elements.RemoveRange(0,2);
                    ulong[] value = elements.Select(code => Convert.ToUInt64(code)).ToArray();
                    AdcSamplesStorage.Add(key, value.ToList());
                    AdcSamplesInfo.Add(key, info);
                }
            }
        }

        private List<ulong> ConvertBytesToCodes(List<byte> byteList, int bytesPerCode)
        {
            List<ulong> convertList = new List<ulong>();

            if (byteList.Count % bytesPerCode != 0)
                throw new ArgumentOutOfRangeException();

            for (int i = 0; i < byteList.Count / bytesPerCode; i++)
                if (bytesPerCode == 2)
                    convertList.Add(BitConverter.ToUInt16(byteList.ToArray(), i * bytesPerCode));
                else if (bytesPerCode == 4)
                    convertList.Add(BitConverter.ToUInt32(byteList.ToArray(), i * bytesPerCode));
                else if (bytesPerCode == 8)
                    convertList.Add(BitConverter.ToUInt64(byteList.ToArray(), i * bytesPerCode));
                else
                    throw new ArgumentOutOfRangeException();

            return convertList;
        }

        public List<double> ConvertVoltages(List<ulong> codeList, bool isBipolar, double vRef, double gain, int adcBits)
        {
            if (isBipolar)
                // Code = 2^(N-1) * [(A_IN * Gain / V_REF) + 1]
                // A_IN = [Code / 2^(N-1) - 1] * V_REF / Gain
                return codeList.ConvertAll(code => (code / Math.Pow(2, adcBits - 1) - 1) * vRef / gain);
            else
                // Code = (2^N * A_IN * Gain) / V_REF
                // A_IN = Code * V_REF / 2^N / Gain
                return codeList.ConvertAll(code => (code * vRef / Math.Pow(2, adcBits) / gain));
        }
    }
}