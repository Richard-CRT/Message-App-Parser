using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageAppParser
{
    internal class Conversation
    {
        public List<Participant> Participants;

        public Conversation()
        {
            Participants = new();
        }
    }
}
