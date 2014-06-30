using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Nop.Plugin.Widgets.LiveChat.Models
{
    [XmlType("LiveChatMessage")]
    public class LiveChatMessage : ISerializable
    {
        [XmlAttribute("AgentName")]
        public string AgentName { get; set; }

        [XmlAttribute("ClientName")]
        public string ClientName { get; set; }

        [XmlAttribute("Message")]
        public string Message { get; set; }

        [XmlAttribute("MessageTime")]
        public DateTime MessageTime { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }
    }
}
