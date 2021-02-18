using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADC_CDC_CONTROLLER
{
    static class AdcPerfCalcUtil
    {
        public static double RmsNoise(List<ulong> sample, double lsbVoltage)
        {
            List<decimal> decSample = sample.ConvertAll(i => (decimal)i);
            return (double)Std(decSample) * lsbVoltage;
        }
        public static double PeakNoiseCalc(List<ulong> sample, double lsbVoltage)
        {
            return RmsNoise(sample, lsbVoltage) * 6.6;
        }
        public static double PeakNoise(List<ulong> sample, double lsbVoltage)
        {
            return (sample.Max() - sample.Min())* lsbVoltage;
        }
        public static double EffResolution(List<ulong> sample, int adcBit)
        {
            return Math.Log(Math.Pow(2, adcBit) / RmsNoise(sample, 1), 2);
        }
        public static double NoiseFreeResolutionCalc(List<ulong> sample, int adcBit)
        {
            return Math.Log(Math.Pow(2, adcBit) / PeakNoiseCalc(sample, 1), 2);
        }
        public static double NoiseFreeResolution(List<ulong> sample, int adcBit)
        {
            return Math.Log(Math.Pow(2, adcBit) / PeakNoise(sample, 1), 2);
        }
        public static decimal Std(List<decimal> xList)
        {
            decimal avg = xList.Average();
            decimal sigma = 0m;
            foreach (var x in xList)
                sigma += (x - avg) * (x - avg);
            sigma /= xList.Count;
            sigma = (decimal)Math.Sqrt((double)sigma);
            return sigma;
        }
    }
}
