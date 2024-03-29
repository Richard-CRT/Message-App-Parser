using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MessageAppParser.Apps.Instagram
{
    internal class InstagramConversation
    {
        public static InstagramConversation FromFile(string filePath)
        {
            InstagramConversation instagramConversation;
            try
            {
                var jsonString = File.ReadAllText(filePath);

                InstagramConversation? o;
                o = (InstagramConversation?)JsonSerializer.Deserialize(jsonString, typeof(InstagramConversation), new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve });

                if (o is null) throw new ArgumentNullException("InstagramConversation is null");

                instagramConversation = o!;
            }
            catch (FileNotFoundException)
            {
                throw;
            }

            return instagramConversation;
        }

        // ============================================================================================
        // ============================================================================================

        private List<InstagramParticipant>? _participants = null;
        [JsonPropertyName("participants"), JsonPropertyOrder(1)]
        public List<InstagramParticipant> Participants
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

        private List<InstagramMessage>? _messages = null;
        [JsonPropertyName("messages"), JsonPropertyOrder(2)]
        public List<InstagramMessage> Messages
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

        public InstagramConversation()
        {
        }

        public override string ToString()
        {
            return $"[Instagram Conversation | Participants: {Participants.Count} | Messages: {Messages.Count}]";
        }

        public Conversation ToGenericConversation()
        {
            Conversation conversation = new();
            conversation.Participants = this.Participants.Select(p => p.ToGenericParticipant()).ToList();
            conversation.Messages = this.Messages.Select(p => p.ToGenericMessage(conversation.Participants)).ToList();
            return conversation;
        }
    }
}
