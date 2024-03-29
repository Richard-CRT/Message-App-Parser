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

namespace MessageAppParser
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

                instagramConversation = (InstagramConversation)o!;
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

        public InstagramConversation()
        {
            Participants = new();
        }

        public override string ToString()
        {
            return $"[Instagram Conversation | Participants: {Participants.Count}]";
        }
    }
}
