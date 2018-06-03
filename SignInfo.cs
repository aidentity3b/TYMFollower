using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TymFollower
{
    [MessagePackObject]
    public class SignInfo
    {
        [Key(0)]
        public string ACCESS_TOKEN { get; set; }
        [Key(1)]
        public string ACCESS_TOKEN_SECRET { get; set; }
        [Key(2)]
        public string CONSUMER_KEY { get; set; }
        [Key(3)]
        public string CONSUMER_SECRET { get; set; }
        public SignInfo() { }
        public SignInfo(string aCCESS_TOKEN, string aCCESS_TOKEN_SECRET, string cONSUMER_KEY, string cONSUMER_SECRET)
        {
            ACCESS_TOKEN = aCCESS_TOKEN;
            ACCESS_TOKEN_SECRET = aCCESS_TOKEN_SECRET;
            CONSUMER_KEY = cONSUMER_KEY;
            CONSUMER_SECRET = cONSUMER_SECRET;
        }

        public static SignInfo GetSignInfo(string path)
        {
            SignInfo si;
            try
            {
                si = MessagePackSerializer.Deserialize<SignInfo>(File.ReadAllBytes(path));
            }
            catch (Exception e)
            {
                Console.WriteLine($"トークン「{path}」を読み込めませんでした：");
                Console.WriteLine(e.Message);
                throw e;
            }
            return si;
        }
        public static void SaveSignInfo(string path, SignInfo si)
        {

            try
            {
                if (!File.Exists(path)) File.Create(path);
                File.WriteAllBytes(path, MessagePackSerializer.Serialize(si));
            }
            catch (IOException e)
            {
                Console.WriteLine($"トークン「{path}」を書き込めませんでした：");
                Console.WriteLine(e.Message);
                throw e;
            }
            return;
        }
    }
}
