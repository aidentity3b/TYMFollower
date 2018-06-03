using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TymFollower
{
    [MessagePackObject]
    public class LabeledTym
    {
        [Key("id")]
        public long? Id { get; set; }
        [Key("screen_name")]
        public string Screen_name { get; set; }
        [Key("name")]
        public string Name { get; set; }
        [Key("description")]
        public string Description { get; set; }
        [Key("followed")]
        public bool Followed { get; set; }
        [Key("protected")]
        public bool IsSecret { get; set; }
        [Key("last_updated")]
        public DateTimeOffset? LastUpdated { get; set; }
        public LabeledTym() { }
        public LabeledTym(Tym tym)
        {
            Id = tym.Id;
            Screen_name = tym.Screen_name;
            Name = tym.Name;
            Description = tym.Description;
            Followed = tym.Followed;
            IsSecret = tym.IsSecret;
            LastUpdated = tym.LastUpdated;
        }
        
    }
}
