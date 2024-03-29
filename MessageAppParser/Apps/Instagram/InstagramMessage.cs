using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MessageAppParser.Apps.Instagram
{
    internal class InstagramMessage
    {
        private string? _senderName = null;
        [JsonPropertyName("sender_name"), JsonPropertyOrder(1)]
        public string SenderName
        {
            get { Debug.Assert(_senderName is not null); return _senderName; }
            set
            {
                if (_senderName != value)
                {
                    _senderName = value;
                }
            }
        }

        private Int64? _timestampMs = null;
        [JsonPropertyName("timestamp_ms"), JsonPropertyOrder(2)]
        public Int64 TimestampMs
        {
            get { Debug.Assert(_timestampMs is not null); return _timestampMs.Value; }
            set
            {
                if (_timestampMs != value)
                {
                    _timestampMs = value;
                }
            }
        }

        private string? _content = null;
        [JsonPropertyName("content"), JsonPropertyOrder(3)]
        public string? Content
        {
            get { return _content; }
            set
            {
                if (_content != value)
                {
                    _content = value;
                }
            }
        }

        private bool? _isGeoblockedForViewer = null;
        [JsonPropertyName("is_geoblocked_for_viewer"), JsonPropertyOrder(4)]
        public bool IsGeoblockedForViewer
        {
            get { Debug.Assert(_isGeoblockedForViewer is not null); return _isGeoblockedForViewer.Value; }
            set
            {
                if (_isGeoblockedForViewer != value)
                {
                    _isGeoblockedForViewer = value;
                }
            }
        }

        public InstagramMessage()
        {
        }

        public override string ToString()
        {
            return $"[Instagram Message | Sender: {SenderName} | Content: {Content}]";
        }

        public Message ToGenericMessage(IEnumerable<Participant> genericParticipants)
        {
            Message message = new();
            message.SenderParticipant = genericParticipants.First(gP => gP.Name == this.SenderName);
            message.Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(this.TimestampMs).UtcDateTime;
            message.TextContent = this.Content;
            return message;
        }
    }
}
