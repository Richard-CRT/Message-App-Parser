﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MessageAppParser
{
    internal class Message
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

        private DateTime? _timestamp = null;
        [JsonPropertyName("timestamp"), JsonPropertyOrder(2)]
        public DateTime Timestamp
        {
            get { Debug.Assert(_timestamp is not null); return _timestamp.Value; }
            set
            {
                if (_timestamp != value)
                {
                    _timestamp = value;
                }
            }
        }

        private string? _textContent = null;
        [JsonPropertyName("text_content"), JsonPropertyOrder(3)]
        public string? TextContent
        {
            get { return _textContent; }
            set
            {
                if (_textContent != value)
                {
                    _textContent = value;
                }
            }
        }

        public Message()
        {
        }

        public override string ToString()
        {
            return $"[Message | Sender: {SenderParticipant}]";
        }

    }
}
