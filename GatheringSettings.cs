using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TymFollower
{

    public class GatheringSettings
    {

        public enum SearchOriginType
        {
            Direct, Indirect, Specified
        }
        public enum SearchGroupType
        {
            Follow, Follower
        }
        public SearchOriginType SearchOrigin;
        public int ScrapingCrawlCount = -1;
        public SearchGroupType SearchGroup;

        public GatheringSettings(SearchOriginType searchOrigin,SearchGroupType searchGroup)
        {
            SearchOrigin = searchOrigin;
            SearchGroup = searchGroup;
        }

        public static GatheringSettings CreateSettings(List<Tym> Tyms)
        {
            Console.WriteLine("探索する対象を選択してください。");
            Console.WriteLine();
            SearchOriginType SearchOrigin;
            if (Tyms.Count <= 0)
            {
                Console.WriteLine("direct\tログイン中のアカウントを起点に検索します。");
                Console.WriteLine("specified\t起点とするユーザーのIDから起点を設定します。");
                //-------------------------------------------
                Loop:
                switch (Console.ReadLine())
                {
                    case "direct":
                        SearchOrigin = SearchOriginType.Direct;
                        break;
                    case "specified":
                        SearchOrigin = SearchOriginType.Specified;
                        break;
                    default:
                        goto Loop;
                }
                //-------------------------------------------
            }
            else
            {
                Console.WriteLine("direct\tログイン中のアカウントを起点に検索します。");
                Console.WriteLine("indirect\tすでに収集済みの戸山生アカウントを起点に検索します。");
                Console.WriteLine("specified\t起点とするユーザーのIDを起点に検索します。");
                //-------------------------------------------
                Loop:
                switch (Console.ReadLine())
                {
                    case "direct":
                        SearchOrigin = SearchOriginType.Direct;
                        break;
                    case "indirect":
                        SearchOrigin = SearchOriginType.Indirect;
                        break;
                    case "specified":
                        SearchOrigin = SearchOriginType.Specified;
                        break;
                    default:
                        goto Loop;
                }
                //-------------------------------------------
            }
            Console.WriteLine();
            

            SearchGroupType SearchGroup;
            if (true)
            {
                Console.WriteLine("");
                Console.WriteLine("follow\tフォローしているアカウントから検索します。");
                Console.WriteLine("follower\tフォロワーから検索します。");

                //-------------------------------------------
                Loop:
                switch (Console.ReadLine())
                {
                    case "follow":
                        SearchGroup = SearchGroupType.Follow;
                        break;
                    case "follower":
                        SearchGroup = SearchGroupType.Follower;
                        break;
                    default:
                        goto Loop;
                }
                //-------------------------------------------
            }
            return new GatheringSettings(SearchOrigin, SearchGroup);

        }
    }
}
