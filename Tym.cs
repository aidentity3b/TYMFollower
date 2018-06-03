using CoreTweet;
using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TymFollower
{
    [MessagePackObject]
    public class Tym
    {
        [Key(0)]
        public long? Id { get; set; }
        [Key(1)]
        public string Screen_name { get; set; }
        [Key(2)]
        public string Name { get; set; }
        [Key(3)]
        public string Description { get; set; }
        [Key(4)]
        public bool Followed { get; set; }
        [Key(5)]
        public bool IsSecret { get; set; }
        [Key(6)]
        public DateTimeOffset? LastUpdated { get; set; }


        public bool isActive(int activeDay)
        {
            if (this.LastUpdated.HasValue)
            {
                return Math.Abs((this.LastUpdated.Value - DateTimeOffset.Now).TotalDays) <= activeDay ? true : false;
            }
            else
            {
                return true;
            }
        }
        public Tym() { }
        public override string ToString()
        {
            return $"{(this.IsSecret ? "□<LOCKED>" : (this.LastUpdated.HasValue?(Math.Abs((this.LastUpdated.Value - DateTimeOffset.Now).TotalDays) <= 30?"●<ACTIVE>":"○<INACTIVE>"):"●<UNKNOWN>"))}{this.Name} (@{this.Screen_name}) ({this.Id.ToString()}) ☆{this.Description}";
        }
        public Tym(User user, bool followed)
        {
            this.Id = user.Id;
            this.Screen_name = user.ScreenName;
            this.Name = user.Name;
            this.Description = user.Description;
            this.Followed = followed;
            this.IsSecret = user.IsProtected;
            if(user.Status != null)
            this.LastUpdated = user.Status.CreatedAt;
        }

        public Tym(long? id, string screen_name, string name, string description, bool followed, bool isSecret)
        {
            this.Id = id;
            this.Screen_name = screen_name;
            this.Name = name;
            this.Description = description;
            this.Followed = followed;
            this.IsSecret = isSecret;
        }

        public static List<Tym> GetTymList(string path)
        {
            List<Tym> Tyms;
            try
            {
                Tyms = MessagePackSerializer.Deserialize<List<Tym>>(File.ReadAllBytes(path));
            }catch(Exception e)
            {
                Console.WriteLine($"リスト「{path}」を読み込めませんでした：");
                Console.WriteLine(e.Message);
                return null;
            }
            return Tyms;
        }
        public static bool SaveTymList(string path, List<Tym> tyms, bool backup)
        {
            
            try
            {
                if (File.Exists(path) && backup)
                {
                    //Backup
                    File.Copy(path, "tyms_" + Guid.NewGuid().ToString() + ".dat");
                }
                if (!File.Exists(path)) File.Create(path);
                File.WriteAllBytes(path,MessagePackSerializer.Serialize(tyms));
            }
            catch (IOException e)
            {
                Console.WriteLine($"リスト「{path}」を書き込めませんでした：");
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        public static bool SaveJson(string path, List<Tym> tyms)
        {

            try
            {
                if (!File.Exists(path)) File.Create(path).Close();
                List<LabeledTym> labeled = new List<LabeledTym>();
                foreach (Tym tym in tyms)
                {
                    labeled.Add(new LabeledTym(tym));
                }
                File.WriteAllText(path, MessagePackSerializer.ToJson(labeled));
            }
            catch (IOException e)
            {
                Console.WriteLine($"リスト「{path}」を書き込めませんでした：");
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            var tym = obj as Tym;
            return tym != null &&
                   Id == tym.Id;
        }

        public override int GetHashCode()
        {
            return 2108858624 + EqualityComparer<long?>.Default.GetHashCode(Id);
        }

        public static bool operator ==(Tym a, Tym b)
        {
            if((object)a == null || (object)b == null)
            {
                return false;
            }
            return (a.Id == b.Id);
        }
        public static bool operator !=(Tym a, Tym b)
        {
            return !(a == b);
        }
    }
}
