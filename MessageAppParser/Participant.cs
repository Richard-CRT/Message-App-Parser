using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MessageAppParser
{
    internal class Participant
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

        public Participant()
        {
        }

        public override string ToString()
        {
            return $"[Participant | Name: {Name}]";
        }
    }
}
