using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficLight
{
    public class TrafficEventArgs : EventArgs
    {
        public string Id { get; }
        public int QueueSize { get; }
        public string State { get; }
        public int GreenLightDuration { get; }

        public TrafficEventArgs(string id, int queueSize, string state, int greenLightDuration)
        {
            Id = id;
            QueueSize = queueSize;
            State = state;
            GreenLightDuration = greenLightDuration;
        }
    }
}
