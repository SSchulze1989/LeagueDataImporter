// MIT License

// Copyright (c) 2020 Simon Schulze

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using iRLeagueManager.Enums;

namespace iRLeagueManager.Timing
{
    [Serializable()]
    public class LapTime : IXmlSerializable, ILapTime, IComparable
    {
        //private DateTime _time { get => DateTime.MinValue + Time; set => Time = value.TimeOfDay; }

        public TimeSpan Time { get; set; }
        public int Minutes => Time.Minutes;
        public int Seconds => Time.Seconds;
        public int Milliseconds => Time.Milliseconds;

        public override string ToString()
        {
            //return _time.ToString("mm:ss,fff", System.Globalization.CultureInfo.GetCultureInfo("de-DE"));
            TimeSpan time = Time;
            string signString = "";

            bool negative = (Time < TimeSpan.Zero) ? true : false;
            if (negative)
            {
                time = Time.Negate();
                signString = "-";
            }

            return signString + (DateTime.MinValue + time).ToString("mm:ss,fff", System.Globalization.CultureInfo.GetCultureInfo("de-DE"));
        }

        public virtual string ToXmlString()
        {
            return XmlConvert.ToString(Time);
        }

        public bool IsValid()
        {
            if (Time > TimeSpan.Zero) return true;
            else return false;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Time = XmlConvert.ToTimeSpan(reader.ReadElementContentAsString());
            //_time = DateTime.FromBinary(long.Parse(reader.ReadElementContentAsString()));
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlConvert.ToString(Time);
            Console.WriteLine(XmlConvert.ToString(Time));
        }

        public LapTime() { }

        public LapTime(TimeSpan time)
        {
            Time = time;
        }

        public LapTime(ILapTime lapTime)
        {
            Time = lapTime.Time;
        }

        static public LapTime FromXmlString(string xmlString)
        {
            return new LapTime((string.IsNullOrEmpty(xmlString)) ? TimeSpan.Zero : XmlConvert.ToTimeSpan(xmlString));
        }

        public virtual int CompareTo(object obj)
        {
            if (obj is LapTime lapTime)
            {
                int result = Time.CompareTo(lapTime.Time);
                if (result !=0)
                {
                    if (lapTime.Time <= TimeSpan.Zero)
                    {
                        result = -1;
                    }
                    else if (Time <= TimeSpan.Zero)
                    {
                        result = 1;
                    }
                }
                return result;
            }
            throw new ArgumentException($"Object must be of type {typeof(LapTime)}.");
        }
    }
}
