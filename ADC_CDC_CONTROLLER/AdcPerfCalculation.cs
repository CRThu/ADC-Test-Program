using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADC_CDC_CONTROLLER
{
    static class AdcPerfCalculation
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
            return (sample.Max() - sample.Min() + 1) * lsbVoltage;
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

        public static double OffsetErrorVoltage(List<ulong> sample, bool isBipolar, double vRef, double gain, int adcBits)
        {
            return ConvertVoltage((ulong)AvgCode(sample), isBipolar, vRef, gain, adcBits);
        }

        public static ulong MinCode(List<ulong> sample)
        {
            return sample.Min();
        }

        public static ulong MaxCode(List<ulong> sample)
        {
            return sample.Max();
        }

        public static double AvgCode(List<ulong> sample)
        {
            List<decimal> decSample = sample.ConvertAll(i => (decimal)i);
            return (double)decSample.Average();
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

        static public double LsbVoltage(bool isBipolar, double vRef, double gain, int adcBits)
        {
            return vRef / gain * (isBipolar ? 2 : 1) / Math.Pow(2, adcBits);
        }

        static public double ConvertVoltage(ulong code, bool isBipolar, double vRef, double gain, int adcBits)
        {
            if (isBipolar)
                // Code = 2^(N-1) * [(A_IN * Gain / V_REF) + 1]
                // A_IN = [Code / 2^(N-1) - 1] * V_REF / Gain
                return (code / Math.Pow(2, adcBits - 1) - 1) * vRef / gain;
            else
                // Code = (2^N * A_IN * Gain) / V_REF
                // A_IN = Code * V_REF / 2^N / Gain
                return code * vRef / Math.Pow(2, adcBits) / gain;
        }

        static public List<double> ConvertVoltages(List<ulong> codeList, bool isBipolar, double vRef, double gain, int adcBits)
        {
            return codeList.ConvertAll(code => ConvertVoltage(code, isBipolar, vRef, gain, adcBits));
        }
    }
}
