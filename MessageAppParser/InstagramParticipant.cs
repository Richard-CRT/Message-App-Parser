using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MessageAppParser
{
    internal class InstagramParticipant
    {
        private string? _name = null;
        [JsonPropertyName("name"), JsonPropertyOrder(1)]
        public string Name
        {
            get { Debug.Assert(_name is not null); return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                }
            }
        }

        public InstagramParticipant()
        {
        }

        public override string ToString()
        {
            return $"[Instagram Participant | Name: {Name}]";
        }
    }
}
