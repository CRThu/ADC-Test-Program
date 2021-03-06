﻿using System;
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
        // bugs in tmpAdcSample
        public const string tmpAdcSample = "tmpAdcSample";

        // Replace string to settings
        public Dictionary<string, string> AdcSamplesSettingInfo;
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

        private void TmpAdcSampleInit()
        {
            AdcSamplesSettingInfo.Add(tmpAdcSample, "");
            AdcSamplesStorage.Add(tmpAdcSample, new List<ulong>());
        }

        public AdcDataStorage()
        {
            AdcSamplesSettingInfo = new Dictionary<string, string>();
            AdcSamplesStorage = new Dictionary<string, List<ulong>>();
            TmpAdcSampleInit();
        }

        public void WriteTmpAdcSamples(string info, List<byte> byteList, int bytesPerCode)
        {
            WriteToDataStorage(tmpAdcSample, info, ConvertBytesToCodes(byteList, bytesPerCode));
            //AdcSamplesSettingInfo[tmpAdcSample] = info;
            //AdcSamplesStorage[tmpAdcSample] = ConvertBytesToCodes(byteList, bytesPerCode);
        }

        public int WriteTmpAdcSamplesToDataStorage(string name)
        {
            return WriteToDataStorage(name, AdcSamplesSettingInfo[tmpAdcSample], AdcSamplesStorage[tmpAdcSample]);
        }

        public int WriteToDataStorage(string name, string info, List<ulong> samples)
        {
            //if (name.Equals(tmpAdcSample))
            //    throw new AccessViolationException();

            if (!AdcSamplesSettingInfo.ContainsKey(name))
                AdcSamplesSettingInfo.Add(name, info);
            else
                AdcSamplesSettingInfo[name] = info;

            if (!AdcSamplesStorage.ContainsKey(name))
                AdcSamplesStorage.Add(name, samples);
            else
                AdcSamplesStorage[name] = samples;

            return samples.Count;
        }
        public List<ulong> ReadTmpAdcSamples()
        {
            return AdcSamplesStorage[tmpAdcSample];
        }
        public List<ulong> ReadDataStorage(string key)
        {
            return AdcSamplesStorage[key];
        }

        public void StoretmpAdcSamplesToFile(string path, DataStroageExtension ext)
        {
            KeyValuePair<string, string> tmpAdcSampleSettingInfoKvp = AdcSamplesSettingInfo.Where(info => info.Key.Equals(tmpAdcSample)).First();
            Dictionary<string, string> tmpAdcSampleSettingInfoDic = new Dictionary<string, string>
            {
                {tmpAdcSampleSettingInfoKvp.Key,tmpAdcSampleSettingInfoKvp.Value }
            };
            KeyValuePair<string, List<ulong>> tmpAdcSampleStorageKvp = AdcSamplesStorage.Where(info => info.Key.Equals(tmpAdcSample)).First();
            Dictionary<string, List<ulong>> tmpAdcSampleStorageDic = new Dictionary<string, List<ulong>>
            {
                {tmpAdcSampleStorageKvp.Key,tmpAdcSampleStorageKvp.Value }
            };

            if (ext == DataStroageExtension.Csv)
                StoreDataToCsv(path, tmpAdcSampleSettingInfoDic, tmpAdcSampleStorageDic);
        }
        public void StoreAllDataToFile(string path, DataStroageExtension ext)
        {
            if (ext == DataStroageExtension.Csv)
                StoreDataToCsv(path, AdcSamplesSettingInfo, AdcSamplesStorage);
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
            AdcSamplesSettingInfo.Clear();
            using (StreamReader sr = new StreamReader(path))
            {
                while (sr.Peek() > 0)
                {
                    string line = sr.ReadLine();
                    List<string> elements = line.Split(new char[] { ',' }).ToList();
                    string key = elements[0];
                    string info = elements[1];
                    elements.RemoveRange(0, 2);
                    elements = elements.Where(element => !string.IsNullOrEmpty(element)).ToList();
                    ulong[] value = elements.Select(code => Convert.ToUInt64(code)).ToArray();
                    AdcSamplesStorage.Add(key, value.ToList());
                    AdcSamplesSettingInfo.Add(key, info);
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
    }
}