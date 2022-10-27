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
using System.Linq;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace iRLeagueManager.Locations
{
    [Serializable]
    [DataContract]
    public class RaceTrack
    {
        [DataMember]
        public int TrackId { get; set; }
        [DataMember]
        public string TrackName { get; set; }
        public string ShortName => TrackName.Substring(0, Math.Min(20, TrackName.Length));
        [DataMember]
        public ObservableCollection<TrackConfig> Configs { get; set; }
        [DataMember]
        public string Location { get; set; } = "";
        //Race track data

        public RaceTrack() 
        {
            Configs = new ObservableCollection<TrackConfig>();
        }

        public RaceTrack(int trackId, string trackName) : this()
        {
            TrackId = trackId;
            TrackName = trackName;
        }

        public void AddConfig(int configId, string configName, int lengtKm = 0)
        {
            Configs.Add(new TrackConfig(this, configId, configName, lengtKm));
        }

        public void AddConfig(TrackConfig config)
        {
            if ((Configs.Any(x => x.ConfigId == config.ConfigId) || Configs.Any(x => x.ConfigName == config.ConfigName)) == false)
            {
                var id = Configs.Count > 0 ? Configs.Max(x => x.ConfigId) + 1 : 1;
                config.ConfigId = 1;
                config.Track = this;
                Configs.Add(config);
            }
        }
    }
}
