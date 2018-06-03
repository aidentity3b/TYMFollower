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
    public class GatheringHistory
    {
        public GatheringHistory(IndirectGatheringState indirectStateFollow, IndirectGatheringState indirectStateFollower, SingleGatheringState directStateFollow, SingleGatheringState directStateFollower, SingleGatheringState specifiedStateFollow, SingleGatheringState specifiedStateFollower)
        {
            IndirectStateFollow = indirectStateFollow;
            IndirectStateFollower = indirectStateFollower;
            DirectStateFollow = directStateFollow;
            DirectStateFollower = directStateFollower;
            SpecifiedStateFollow = specifiedStateFollow;
            SpecifiedStateFollower = specifiedStateFollower;
        }
        public GatheringHistory()
        {

        }

        [Key(0)]
        public IndirectGatheringState IndirectStateFollow { get; set; }
        [Key(1)]
        public IndirectGatheringState IndirectStateFollower { get; set; }
        [Key(2)]
        public SingleGatheringState DirectStateFollow { get; set; }
        [Key(3)]
        public SingleGatheringState DirectStateFollower { get; set; }
        [Key(4)]
        public SingleGatheringState SpecifiedStateFollow { get; set; }
        [Key(5)]
        public SingleGatheringState SpecifiedStateFollower { get; set; }
        public static GatheringHistory GetHistory(string path)
        {
            GatheringHistory History;
            if (!File.Exists(path))
            {
                File.Create(path).Close();
                File.WriteAllBytes(path,MessagePackSerializer.Serialize(new GatheringHistory()));
            }
            try
            {
                History = MessagePackSerializer.Deserialize<GatheringHistory>(File.ReadAllBytes(path));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Gather履歴「{path}」を読み込めませんでした：");
                Console.WriteLine(e.Message);
                return null;
            }
            return History;
        }
        public static bool SaveHistory(string path, GatheringHistory history)
        {

            try
            {
                if (!File.Exists(path)) File.Create(path);
                File.WriteAllBytes(path, MessagePackSerializer.Serialize(history));
            }
            catch (IOException e)
            {
                Console.WriteLine($"Gather履歴「{path}」を書き込めませんでした：");
                Console.WriteLine(e.Message);
                return false;
            }
            return true;
        }
        public Object GetState(GatheringSettings settings)
        {
            switch (settings.SearchOrigin)
            {
                case GatheringSettings.SearchOriginType.Direct:
                    switch (settings.SearchGroup)
                    {
                        case GatheringSettings.SearchGroupType.Follow:
                            return DirectStateFollow;
                        case GatheringSettings.SearchGroupType.Follower:
                            return DirectStateFollower;
                        default:
                            break;
                    }
                    break;
                case GatheringSettings.SearchOriginType.Indirect:
                    switch (settings.SearchGroup)
                    {
                        case GatheringSettings.SearchGroupType.Follow:
                            return IndirectStateFollow;
                        case GatheringSettings.SearchGroupType.Follower:
                            return IndirectStateFollower;
                        default:
                            break;
                    }
                    break;
                case GatheringSettings.SearchOriginType.Specified:
                    switch (settings.SearchGroup)
                    {
                        case GatheringSettings.SearchGroupType.Follow:
                            return SpecifiedStateFollow;
                        case GatheringSettings.SearchGroupType.Follower:
                            return SpecifiedStateFollower;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            return null;
        }
        public void SetState(GatheringSettings settings, Object state)
        {
            switch (settings.SearchOrigin)
            {
                case GatheringSettings.SearchOriginType.Direct:
                    switch (settings.SearchGroup)
                    {
                        case GatheringSettings.SearchGroupType.Follow:
                            DirectStateFollow = state as SingleGatheringState;
                            break;
                        case GatheringSettings.SearchGroupType.Follower:
                            DirectStateFollower = state as SingleGatheringState;
                            break;
                        default:
                            break;
                    }
                    break;
                case GatheringSettings.SearchOriginType.Indirect:
                    switch (settings.SearchGroup)
                    {
                        case GatheringSettings.SearchGroupType.Follow:
                            IndirectStateFollow = state as IndirectGatheringState;
                            break;
                        case GatheringSettings.SearchGroupType.Follower:
                            IndirectStateFollower = state as IndirectGatheringState;
                            break;
                        default:
                            break;
                    }
                    break;
                case GatheringSettings.SearchOriginType.Specified:
                    switch (settings.SearchGroup)
                    {
                        case GatheringSettings.SearchGroupType.Follow:
                            SpecifiedStateFollow = state as SingleGatheringState;
                            break;
                        case GatheringSettings.SearchGroupType.Follower:
                            SpecifiedStateFollower = state as SingleGatheringState;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            return;
        }

        [MessagePackObject]
        public class IndirectGatheringState
        {
            [Key(0)]
            public long? Cursor { get; set; }
            [Key(1)]
            public Tym CurrentTym { get; set; }

            public IndirectGatheringState(long? cursor, Tym currentTym)
            {
                Cursor = cursor;
                CurrentTym = currentTym;
            }
        }
        [MessagePackObject]
        public class SingleGatheringState 
        {
            [Key(0)]
            public long? Cursor { get; set; }
            [Key(1)]
            public Tym CurrentTym { get; set; }
            [Key(2)]
            public string SpecifiedScreenName { get; set; }

            public SingleGatheringState(long? cursor, Tym currentTym)
            {
                Cursor = cursor;
                CurrentTym = currentTym;
            }
            public SingleGatheringState(long? cursor, string specifiedScreenName)
            {
                Cursor = cursor;
                SpecifiedScreenName = specifiedScreenName;
            }
        }
    }
}
