using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MessageAppParser
{
    internal class MessageBatch
    {
        private Participant? _senderParticipant = null;
        [JsonPropertyName("sender_participant"), JsonPropertyOrder(1)]
        public Participant SenderParticipant
        {
            get { Debug.Assert(_senderParticipant is not null); return _senderParticipant; }
            set
            {
                if (_senderParticipant != value)
                {
                    _senderParticipant = value;
                }
            }
        }

        private List<Message>? _messages = null;
        [JsonPropertyName("messages"), JsonPropertyOrder(2)]
        public List<Message> Messages
        {
            get { Debug.Assert(_messages is not null); return _messages; }
            set
            {
                if (_messages != value)
                {
                    _messages = value;
                }
            }
        }

        public MessageBatch()
        {
            Messages = new();
        }

        public override string ToString()
        {
            return $"[MessageBatch | Messages: {Messages.Count}]";
        }
    }
}
