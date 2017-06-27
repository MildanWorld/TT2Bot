﻿using LiteDB;
using Newtonsoft.Json.Linq;
using System;

namespace Conversion.OldModels
{
    class Timer
    {
        [BsonId]
        public int Id { get; set; }
        public bool Complete { get; set; }
        public bool Cancelled { get; set; }
        public ulong? GuildId { get; set; }
        public ulong UserId { get; set; }
        public ulong MessageId { get; set; }
        public ulong ChannelId { get; set; }
        public DateTime From { get; set; }
        public DateTime? To { get; set; }
        public int SecondInterval { get; set; }
        public object Callback { get; set; }

        public string _custArgs { get; set; }

        [BsonIgnore]
        public JObject CustArgs
        {
            get
            {
                return JObject.Parse(_custArgs ?? "{}");
            }
            set
            {
                _custArgs = value.ToString();
            }
        }


        public bool Active { get { return !(Complete || Cancelled); } }
    }
}
