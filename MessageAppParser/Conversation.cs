using MessageAppParser.Apps.Instagram;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MessageAppParser
{
    internal class Conversation
    {
        private List<Participant>? _participants = null;
        [JsonPropertyName("participants"), JsonPropertyOrder(1)]
        public List<Participant> Participants
        {
            get { Debug.Assert(_participants is not null); return _participants; }
            set
            {
                if (_participants != value)
                {
                    _participants = value;
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

        private List<MessageBatch>? _messageBatches = null;
        [JsonIgnore]
        public List<MessageBatch>? MessageBatches
        {
            get { return _messageBatches; }
            set
            {
                if (_messageBatches != value)
                {
                    _messageBatches = value;
                }
            }
        }

        private Dictionary<MessageBatch, TimeSpan>? _responseTimesBeforeMessageBatch = null;
        [JsonIgnore]
        public Dictionary<MessageBatch, TimeSpan>? ResponseTimeBeforeMessageBatches
        {
            get { return _responseTimesBeforeMessageBatch; }
            set
            {
                if (_responseTimesBeforeMessageBatch != value)
                {
                    _responseTimesBeforeMessageBatch = value;
                }
            }
        }

        public Conversation()
        {
        }

        public void AnalyseMessageBatches()
        {
            if (MessageBatches is null)
            {
                this.MessageBatches = new();
                var orderedMessages = this.Messages.OrderBy(m => m.Timestamp);
                MessageBatch? messageBatch = null;
                foreach (Message message in orderedMessages)
                {
                    if (messageBatch is null || message.SenderParticipant != messageBatch.Messages.First().SenderParticipant)
                    {
                        messageBatch = new();
                        messageBatch.SenderParticipant = message.SenderParticipant;
                        this.MessageBatches.Add(messageBatch);
                    }
                    messageBatch.Messages.Add(message);
                }
            }
        }

        public void AnalyseTimesBetweenMessageBatches()
        {
            if (ResponseTimeBeforeMessageBatches is null)
            {
                ResponseTimeBeforeMessageBatches = new();
                Debug.Assert(this.MessageBatches is not null);
                for (int i = 1; i < this.MessageBatches.Count; i++)
                {
                    ResponseTimeBeforeMessageBatches[this.MessageBatches[i]] = this.MessageBatches[i].Messages.First().Timestamp - this.MessageBatches[i - 1].Messages.First().Timestamp;
                }
            }
        }

        public override string ToString()
        {
            return $"[Conversation | Participants: {Participants.Count} | Messages: {Messages.Count}]";
        }
    }
}
