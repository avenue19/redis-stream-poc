using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stream_shared.Messages
{
    public sealed class SomethingHappened
    {
        public string EventType { get; set; }
        public int Start { get; set; }
    }
}
