using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADC_CDC_CONTROLLER
{
    class AdcDataStorage
    {
        private List<ulong> tmpAdcSamples;
        private Dictionary<string, List<ulong>> AdcSamplesStorage;
        public enum FileStroageExtension
        {
            Csv
        }

        public int AdcSamplesCount
        {
            get
            {
                return AdcSamplesStorage.Count;
            }
        }
        public string[] AllAdcSamplesStorageKeys
        {
            get
            {
                return AdcSamplesStorage.Select(samples => samples.Key).ToArray();
            }
        }

        public AdcDataStorage()
        {
            tmpAdcSamples = new List<ulong>();
            AdcSamplesStorage = new Dictionary<string, List<ulong>>();
        }

        public void WriteTmpAdcSamples(List<byte> byteList, int bytesPerCode)
        {
            tmpAdcSamples = ConvertBytesToCodes(byteList, bytesPerCode);
        }
        public void WriteTmpAdcSamplesToDataStorage(string name)
        {
            AdcSamplesStorage.Add(name, tmpAdcSamples);
        }
        public void WriteToDataStorage(string name, List<ulong> samples)
        {
            AdcSamplesStorage.Add(name, samples);
        }
        public List<ulong> ReadDataStorage(string key)
        {
            return AdcSamplesStorage[key];
        }
        public void StoreAllDataToFile(string path, FileStroageExtension ext)
        {
            if (ext == FileStroageExtension.Csv)
                StoreDataToCsv(path);
        }
        private void StoreDataToCsv(string path)
        {
            // Fomart
            // NAME1,1,1,1,1,
            // NAME2,1,1,1,1,1,1,
            // NAME3,1,1,1,1,1,1,1,1,
            FileStream fs = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            foreach (var kvp in AdcSamplesStorage)
            {
                sw.Write(kvp.Key + ",");
                foreach (var code in kvp.Value)
                    sw.Write(code + ",");
                sw.WriteLine();
            }
            sw.Flush();
            sw.Close();
        }
        public void LoadAllDataFromFile(string path, FileStroageExtension ext)
        {
            if (ext == FileStroageExtension.Csv)
                LoadAllDataFromCsv(path);
        }
        private void LoadAllDataFromCsv(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                while (sr.Peek() > 0)
                {
                    string line = sr.ReadLine();
                    List<string> elements = line.Split(new char[] { ',' }).Where(str => !string.IsNullOrEmpty(str)).ToList();
                    string key = elements[0];
                    elements.RemoveAt(0);
                    ulong[] value = elements.Select(code => Convert.ToUInt64(code)).ToArray();
                    AdcSamplesStorage.Add(key, value.ToList());
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
                // 2^(N-1) * [(A_IN * Gain / V_REF) + 1]
                return codeList.ConvertAll(code => Math.Pow(2, adcBits - 1) * (code * gain / vRef + 1));
            else
                // (2^N * A_IN * Gain) / V_REF
                return codeList.ConvertAll(code => (Math.Pow(2, adcBits) * code * gain) / vRef);
        }
    }
}