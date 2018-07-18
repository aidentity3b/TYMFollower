using CoreTweet;
using CoreTweet.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TymFollower
{
    class Program
    {
        private static Tokens Login()
        {
            Tokens tokens;

            //Create SignInfo
            if (!File.Exists("sign.dat"))
            {
                Console.WriteLine("ログイン情報ファイルを作成しています...");
                try
                {
                    File.Create("sign.dat").Close();
                    SignInfo.SaveSignInfo("sign.dat", new SignInfo());
                }
                catch (Exception)
                {
                    Console.WriteLine("ログイン情報ファイルの作成に失敗しました。");
                    throw;
                }
            }
            if (!File.Exists("sign.dat"))
            {
                Console.WriteLine("ログイン情報ファイルの作成に失敗しました。");
                throw new Exception();
            }

            //Read SignInfo
            SignInfo si;
            try
            {
                si = SignInfo.GetSignInfo("sign.dat");
            }
            catch (Exception)
            {
                Console.WriteLine("ログイン情報ファイルを作成しています...");
                try
                {
                    SignInfo.SaveSignInfo("sign.dat", new SignInfo());
                    si = new SignInfo();
                }
                catch (Exception)
                {
                    Console.WriteLine("ログイン情報ファイルの作成に失敗しました。");
                    throw;
                }
            }
            if (!string.IsNullOrEmpty(si.CONSUMER_KEY) && !string.IsNullOrEmpty(si.CONSUMER_SECRET))
            {
                Console.WriteLine("以前使用したAPIキーが見つかりました。前回と同じ連携アプリを使用してログインしますか？[y/n]");
                switch (Console.ReadLine())
                {
                    case "n":
                        si.CONSUMER_KEY = null;
                        si.CONSUMER_SECRET = null;
                        break;
                    default:
                        break;
                }
            }
            if (string.IsNullOrEmpty(si.CONSUMER_KEY) || string.IsNullOrEmpty(si.CONSUMER_SECRET))
            {
                Console.WriteLine("APIキーの入力が必要です。");
                Console.Write("まず ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("https://apps.twitter.com/");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(" にアクセスし、新しい連携アプリを作成してください。");
                Console.WriteLine("次に「Keys and Access Tokens」から必要な情報を参照して以下に入力してください：");
                string key;
                do
                {
                    Console.Write("Consumer Key (API Key): ");
                } while (string.IsNullOrEmpty(key = Console.ReadLine()));
                key = key.Trim(' ');
                string secret;
                do
                {
                    Console.Write("Consumer Secret (API Secret): ");
                } while (string.IsNullOrEmpty(secret = Console.ReadLine()));
                secret = secret.Trim(' ');
                si.CONSUMER_KEY = key;
                si.CONSUMER_SECRET = secret;
                SignInfo.SaveSignInfo("sign.dat", si);
            }
            OAuth.OAuthSession session;
            try
            {
                session = OAuth.Authorize(si.CONSUMER_KEY, si.CONSUMER_SECRET);
            }
            catch (Exception e)
            {
                Console.WriteLine("APIキーの有効性を確認できませんでした: " + e.Message);
                throw;
            }

            if (!string.IsNullOrEmpty(si.ACCESS_TOKEN) && !string.IsNullOrEmpty(si.ACCESS_TOKEN_SECRET))
            {
                Console.WriteLine("以前使用したアクセストークンが見つかりました。前回のログイン情報を使用してログインしますか？[y/n]");
                switch (Console.ReadLine())
                {
                    case "n":
                        si.ACCESS_TOKEN = null;
                        si.ACCESS_TOKEN_SECRET = null;
                        break;
                    default:
                        break;
                }
            }
            if (string.IsNullOrEmpty(si.ACCESS_TOKEN) || string.IsNullOrEmpty(si.ACCESS_TOKEN_SECRET))
            {
                Console.WriteLine("新しいアクセストークンを取得します。");
                System.Diagnostics.Process.Start(session.AuthorizeUri.AbsoluteUri);
                Console.WriteLine("表示されたPINを入力してください:");
                string pin = Console.ReadLine();
                try
                {
                    tokens = OAuth.GetTokens(session, pin);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("アクセストークンの取得に失敗しました： " + e.Message);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    throw;
                }
                si.ACCESS_TOKEN = tokens.AccessToken;
                si.ACCESS_TOKEN_SECRET = tokens.AccessTokenSecret;
                SignInfo.SaveSignInfo("sign.dat", si);
            }
            else
            {
                tokens = Tokens.Create(si.CONSUMER_KEY, si.CONSUMER_SECRET, si.ACCESS_TOKEN, si.ACCESS_TOKEN_SECRET);
            }

            Console.WriteLine("ログインしたユーザー情報を取得しています……");
            try
            {
                currentUser = tokens.Account.VerifyCredentials();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ログインに失敗しました： " + e.Message);
                Console.ForegroundColor = ConsoleColor.Gray;
                throw;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0} @{1}", currentUser.Name, currentUser.ScreenName);
            Console.ForegroundColor = ConsoleColor.Gray;
            return tokens;
        }
        static UserResponse currentUser;
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("TYMFollower by @ruccho_vector (C)2018 ruccho");
            Console.WriteLine("このプログラムは、Twitterから戸山生とみられるアカウントを判別し、一括フォローします。");
            Console.ForegroundColor = ConsoleColor.Gray;
            Tokens tokens;
            try
            {
                tokens = Login();
            }
            catch (Exception)
            {
                Console.Write("何かキーを押して終了します。");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("収集済みの戸山生リストを読み込んでいます……");

            if (!File.Exists("tyms.dat"))
            {
                Console.WriteLine("戸山生リストが見つかりませんでした。新しく作成しますか？(y/n)");
                string c = Console.ReadLine();
                switch (c)
                {
                    case "y":
                        File.Create("tyms.dat").Close();
                        Tym.SaveTymList("tyms.dat", new List<Tym>(), false);
                        break;
                    default:
                        return;
                }
            }
            List<Tym> GatheredTyms = Tym.GetTymList("tyms.dat");
            Console.WriteLine($"{GatheredTyms.Count.ToString()}人の戸山生をリストから読み込みました");

            while (true)
            {
                Console.WriteLine($"なんの操作を行いますか？ヘルプを表示するには「help」と入力してください。");
                string command = Console.ReadLine();
                switch (command)
                {
                    case "help":
                        ShowHelp();
                        break;
                    case "gather":
                        Gather(tokens, GatheredTyms);
                        break;
                    case "show":
                        Show(GatheredTyms);
                        break;
                    case "update":
                        Update(tokens, GatheredTyms);
                        break;
                    case "clear":
                        Clear(GatheredTyms);
                        break;
                    case "export":
                        Export(GatheredTyms);
                        break;
                    case "follow":
                        Follow(tokens, GatheredTyms);
                        break;
                    case "backup":
                        Backup(GatheredTyms);
                        break;
                    case "exit":
                        return;
                    default:
                        break;

                }
            }

        }

        private static void Export(List<Tym> gatheredTyms)
        {
            Console.WriteLine("JSONに変換しています……");
            string path = "tyms_" + Guid.NewGuid().ToString() + ".json";
            if (Tym.SaveJson(path, gatheredTyms))
            {
                Console.WriteLine("「" + path + "」に保存しました。");
            }
        }

        private static void Backup(List<Tym> gatheredTyms)
        {
            Tym.SaveTymList("tyms.dat", gatheredTyms, true);
        }
        /// <summary>
        /// あるアカウントが戸山生がどうか判定する。
        /// </summary>
        /// <param name="user">判定対象のユーザー。</param>
        /// <param name="rateSearch">そのユーザーのフォロワーに占める戸山生の割合を検索に使用する。</param>
        /// <returns></returns>
        static bool IsTym(User user, Tokens tokens = null,List<Tym> GatheredTym = null, bool rateSearch = false)
        {
            string desc = user.Description;
            string[] keywords = { "戸山", "とやま", "tym", "Tym", "TYM", "Toyama", "TOYAMA", "toyama", "めめち", "めめ物", "めめ化", "めめ生", "めめ地学", "めめ地", "めめ数", "めめぶつ", "めめか", "めめなま", "めめちがく" };
            bool isTym = false;
            foreach (string keyword in keywords)
            {
                if (desc.Contains(keyword)) isTym = true;
            }
            if (isTym)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("■プロフィールから戸山生であると推測されます。");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("□プロフィールから戸山生かどうか判別できませんでした。");
            }

            if(!isTym && rateSearch)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("□フォロワーに占める既知の戸山生の割合を調査します。");
                if (user.IsProtected)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("□鍵アカのフォロワーは調査できません。スキップします。");
                    return isTym;
                }
                //フォロワーにしめる戸山生の割合を調べる。
                if (tokens == null || GatheredTym == null)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    throw new NullReferenceException("引数が不正（null）です。");
                }
                Cursored<User> followers = tokens.Followers.List(user_id: user.Id.Value, count: 200);
                int tymCount = 0;
                foreach (User follower in followers)
                {
                    if (GatheredTym.Contains(new Tym(follower, false))) tymCount++;
                }
                float rate = (float)tymCount / followers.Count;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("□フォロワーに占める既知の戸山生の割合：　" + (rate*100).ToString());
                if (rate > 0.4f)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("■フォロワーに占める既知の戸山生の割合から戸山生であると推測されます。");
                    isTym = true;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("□フォロワーに占める既知の戸山生の割から戸山生かどうか判別できませんでした。");
                }

            }
            Console.ForegroundColor = ConsoleColor.Gray;
            return isTym;
        }

        static void ShowHelp()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("TwitterのAPI制限によりリクエストは15回/15分に制限されます");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n--------アカウント表示の見方--------");
            Console.WriteLine("●<ACTIVE>3B「アイデンティティ」@戸山祭 (@AIdentity_3B) (907235856020148225) ☆戸山高校 3B制作の映画 #AIdentity に関する情報をお届けします。脚本はオリジナル。2018.9戸山祭にて上映。フォローよろしくお願いします！");
            Console.WriteLine("[アカウントの状態] [   アカウント名   ] (@    [ID]    ) ([     数値ID     ]) ☆[説明文]");
            Console.WriteLine("\n--------アカウントの状態について--------");
            Console.WriteLine("●<ACTIVE>\t過去30日以内に更新のあるアカウントです。");
            Console.WriteLine("○<INACTIVE>\t過去30日以内に更新のない非活動的なアカウントです。");
            Console.WriteLine("□<LOCKED>\t鍵のついたアカウントです。");
            Console.WriteLine("※アカウントの最終活動日時はupdateコマンドで更新できます。");
            Console.WriteLine("\n--------アカウント表示の色について--------");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("黄色で表示されたアカウント\tまだフォローしていません。");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("緑色で表示されたアカウント\tすでにフォローしています。");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("------------------------------------\n");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("使用できるコマンド：\n");
            Console.WriteLine("gather\t新しく戸山生を収集しリストに追加します。最後に検索したところから再探索できます。");
            Console.WriteLine("show\tリスト内容を表示します。");
            Console.WriteLine("update\tリストに登録された戸山生のフォロー状態の情報を更新します。");
            Console.WriteLine("clear\tリストを消去します。");
            Console.WriteLine("backup\t読み込んでいる戸山生リストを保存し、コピーを出力します。");
            Console.WriteLine("export\t読み込んでいる戸山生リストをJSONに保存します。");
            Console.WriteLine("follow\tリストに追加された戸山生をフォローします。");
            Console.WriteLine("exit\t終了します。");
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        static void Gather(Tokens tokens, List<Tym> Tyms)
        {
            Console.WriteLine("Gather: 戸山生を収集します。");
            Console.WriteLine();
            GatheringSettings settings = GatheringSettings.CreateSettings(Tyms);
            GatheringHistory History = GatheringHistory.GetHistory("history.dat");
            switch (settings.SearchOrigin)
            {
                case GatheringSettings.SearchOriginType.Specified:
                case GatheringSettings.SearchOriginType.Direct:
                    SingleGathering(tokens, Tyms, settings, History);
                    break;
                case GatheringSettings.SearchOriginType.Indirect:
                    IndirectGathering(tokens, Tyms, settings, History);
                    break;
                default:
                    break;
            }
            Tym.SaveTymList("tyms.dat", Tyms, true);

        }
        static void SingleGathering(Tokens tokens, List<Tym> Tyms, GatheringSettings settings, GatheringHistory History)
        {
            GatheringHistory.SingleGatheringState State = History.GetState(settings) as GatheringHistory.SingleGatheringState;

            if (State != null)
            {
                Console.WriteLine("以前にこの設定で途中まで検索を行った履歴があります。途中から再開しますか？[y/n]");
                switch (settings.SearchOrigin)
                {
                    case GatheringSettings.SearchOriginType.Direct:
                        Console.WriteLine($"（{State.CurrentTym.Name}, @{State.CurrentTym.Screen_name}）");
                        break;
                    case GatheringSettings.SearchOriginType.Specified:
                        Console.WriteLine($"（@{State.SpecifiedScreenName}）");
                        break;

                }
                switch (Console.ReadLine())
                {
                    case "n":
                        switch (settings.SearchOrigin)
                        {
                            case GatheringSettings.SearchOriginType.Direct:
                                State = new GatheringHistory.SingleGatheringState(null, new Tym(currentUser, true));
                                break;
                            case GatheringSettings.SearchOriginType.Specified:
                                Console.WriteLine("検索の起点となるユーザーのIDを入力してください");
                                Console.Write("@");
                                State = new GatheringHistory.SingleGatheringState(null, Console.ReadLine());
                                break;
                            default:
                                State = new GatheringHistory.SingleGatheringState(null, new Tym(currentUser, true));
                                break;

                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (settings.SearchOrigin)
                {
                    case GatheringSettings.SearchOriginType.Direct:
                        State = new GatheringHistory.SingleGatheringState(null, new Tym(currentUser, true));
                        break;
                    case GatheringSettings.SearchOriginType.Specified:
                        Console.WriteLine("検索の起点となるユーザーのIDを入力してください");
                        Console.Write("@");
                        State = new GatheringHistory.SingleGatheringState(null, Console.ReadLine());
                        break;
                    default:
                        State = new GatheringHistory.SingleGatheringState(null, new Tym(currentUser, true));
                        break;

                }
            }
            History.SetState(settings, State);
            GatheringHistory.SaveHistory("history.dat", History);
            int TymCounter = 0;
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("累計収集戸山生数:" + TymCounter.ToString());
                Cursored<User> gathereds = null;
                try
                {
                    switch (settings.SearchOrigin)
                    {
                        case GatheringSettings.SearchOriginType.Direct:
                            gathereds = InternalGather(tokens, State.Cursor, State.CurrentTym, settings.SearchGroup);
                            break;
                        case GatheringSettings.SearchOriginType.Specified:
                            gathereds = InternalGather(tokens, State.Cursor, State.SpecifiedScreenName, settings.SearchGroup);
                            break;
                    }
                }
                catch (Exception)
                {
                    return;
                }
                if (gathereds == null)
                {
                    continue;
                }
                foreach (User gathered in gathereds)
                {
                    if (IsTym(gathered))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("●戸山生っぽい！");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine($"{gathered.Name} ({gathered.ScreenName})");
                        Console.WriteLine(gathered.Description);
                        Tym tym;
                        switch (settings.SearchOrigin)
                        {
                            case GatheringSettings.SearchOriginType.Direct:
                                tym = new Tym(gathered, (settings.SearchGroup == GatheringSettings.SearchGroupType.Follow ? true : false));
                                break;
                            case GatheringSettings.SearchOriginType.Specified:
                            default:
                                tym = new Tym(gathered, false);
                                break;
                        }

                        if (!Tyms.Contains(tym))
                        {
                            TymCounter++;
                            Tyms.Add(tym);
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("●戸山生じゃないかも……");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine($"{gathered.Name} ({gathered.ScreenName})");
                        Console.WriteLine(gathered.Description);
                    }
                }
                if (gathereds.NextCursor == 0)
                {
                    //Last Page
                    State = null;
                    History.SetState(settings, State);
                    GatheringHistory.SaveHistory("history.dat", History);
                    break;
                }
                else
                {
                    State.Cursor = gathereds.NextCursor;
                    History.SetState(settings, State);
                    GatheringHistory.SaveHistory("history.dat", History);
                }
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("収集終了");
            Console.WriteLine("累計収集戸山生数:" + TymCounter.ToString());
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        static Cursored<User> InternalGather(Tokens tokens, long? Cursor, Tym userToProcess, GatheringSettings.SearchGroupType SearchGroup)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("START REQUEST");
            Console.ForegroundColor = ConsoleColor.Gray;
            Cursored<User> gathereds = new Cursored<User>();
            try
            {
                switch (SearchGroup)
                {
                    case GatheringSettings.SearchGroupType.Follow:
                        return tokens.Friends.List(user_id => userToProcess.Id, cursor => Cursor, count => 200);
                    case GatheringSettings.SearchGroupType.Follower:
                        return tokens.Followers.List(user_id => userToProcess.Id, cursor => Cursor, count => 200);
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("リクエストに失敗しました。: " + e.Message);
                Console.ForegroundColor = ConsoleColor.Gray;
                if (e.Message == "Rate limit exceeded")
                {
                    Console.WriteLine("API制限は15分後に解除されます。15分後に再開します。");
                    Thread.Sleep(910000);
                }
                else
                {
                    throw e;
                }
            }
            return null;
        }
        static Cursored<User> InternalGather(Tokens tokens, long? Cursor, string screenNameToProcess, GatheringSettings.SearchGroupType SearchGroup)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("START REQUEST");
            Console.ForegroundColor = ConsoleColor.Gray;
            try
            {
                switch (SearchGroup)
                {
                    case GatheringSettings.SearchGroupType.Follow:
                        return tokens.Friends.List(screen_name => screenNameToProcess, cursor => Cursor, count => 200);
                    case GatheringSettings.SearchGroupType.Follower:
                        return tokens.Followers.List(screen_name => screenNameToProcess, cursor => Cursor, count => 200);
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("リクエストに失敗しました。: " + e.Message);
                Console.ForegroundColor = ConsoleColor.Gray;
                if (e.Message == "Rate limit exceeded")
                {
                    Console.WriteLine("API制限は15分後に解除されます。15分後に再開します。");
                    Thread.Sleep(910000);
                }
                else
                {
                    throw e;
                }
            }
            return null;
        }
        static void IndirectGathering(Tokens tokens, List<Tym> Tyms, GatheringSettings settings, GatheringHistory History)
        {
            GatheringHistory.IndirectGatheringState State = History.GetState(settings) as GatheringHistory.IndirectGatheringState;
            int startIndex = 0;
            if (State != null)
            {
                Console.WriteLine("以前にこの設定で途中まで検索を行った履歴があります。途中から再開しますか？[y/n]");
                switch (Console.ReadLine())
                {
                    case "n":
                        State = new GatheringHistory.IndirectGatheringState(null, null);
                        break;
                    default:
                        int index = Tyms.IndexOf(State.CurrentTym);
                        if (index == -1)
                        {
                            startIndex = 0;
                        }
                        else
                        {
                            startIndex = index;
                        }
                        break;
                }
            }
            else
            {
                State = new GatheringHistory.IndirectGatheringState(null, null);
            }
            History.SetState(settings, State);
            GatheringHistory.SaveHistory("history.dat", History);
            int TymCounter = 0;
            for (int i = 0; i < Tyms.Count; i++)
            {
                if (startIndex <= i)
                {
                    //Process
                    State.CurrentTym = Tyms[i];
                    History.SetState(settings, State);
                    GatheringHistory.SaveHistory("history.dat", History);
                    while (true)
                    {
                        Tym.SaveTymList("tyms.dat", Tyms, false);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("累計収集戸山生数:" + TymCounter.ToString());
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"これから処理する戸山生:{State.CurrentTym.Name} (@{State.CurrentTym.Screen_name})");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Cursored<User> gathereds = null;
                        try
                        {
                            gathereds = InternalGather(tokens, State.Cursor, State.CurrentTym, settings.SearchGroup);
                        }
                        catch (Exception)
                        {
                            return;
                        }
                        if (gathereds == null)
                        {
                            continue;
                        }
                        foreach (User gathered in gathereds)
                        {
                            if (IsTym(gathered))
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("●戸山生っぽい！");
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Console.WriteLine($"{gathered.Name} ({gathered.ScreenName})");
                                Console.WriteLine(gathered.Description);
                                Tym tempTym = new Tym(gathered, false);
                                if (!Tyms.Contains(tempTym))
                                {
                                    TymCounter++;
                                    Tyms.Add(tempTym);
                                }
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Magenta;
                                Console.WriteLine("●戸山生じゃないかも……");
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Console.WriteLine($"{gathered.Name} ({gathered.ScreenName})");
                                Console.WriteLine(gathered.Description);
                            }
                        }
                        if (gathereds.NextCursor == 0)
                        {
                            //Last Page
                            State.Cursor = null;
                            History.SetState(settings, State);
                            GatheringHistory.SaveHistory("history.dat", History);
                            break;
                        }
                        else
                        {
                            State.Cursor = gathereds.NextCursor;
                            History.SetState(settings, State);
                            GatheringHistory.SaveHistory("history.dat", History);
                        }
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("収集終了");
            Console.WriteLine("累計収集戸山生数:" + TymCounter.ToString());
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        static void Show(List<Tym> Tyms)
        {
            foreach (Tym tym in Tyms)
            {
                if (tym.Followed) Console.ForegroundColor = ConsoleColor.Green;
                else Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(tym.ToString());
            }
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"現在{Tyms.Count.ToString()}の戸山生を収集済み");
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        static void Update(Tokens tokens, List<Tym> Tyms)
        {
            Console.WriteLine("Update: リスト内のユーザー情報を最新に更新します。");
            bool updateSecret = false;
            Console.WriteLine("鍵アカの情報も更新しますか？[y/n]");
            if (Console.ReadLine() == "y")
            {
                updateSecret = true;
            }
            StringBuilder ids = new StringBuilder();
            int gatheredCount = 0;
            for (int i = 0; i < Tyms.Count; i++)
            {
                Tym tym = Tyms[i];
                if (!tym.IsSecret || (tym.IsSecret && updateSecret))
                {
                    if (gatheredCount != 0)
                    {
                        ids.Append("," + tym.Id.ToString());
                    }
                    else
                    {
                        ids.Append(tym.Id.ToString());
                    }
                    gatheredCount++;
                    if (gatheredCount == 100)
                    {
                        gatheredCount = 0;
                        try
                        {
                            Tym[] response = InternalUpdate(tokens, ids.ToString());
                            foreach (Tym user in response)
                            {
                                if (user == null) continue;
                                int index = Tyms.IndexOf(user);
                                if (index != -1)
                                {
                                    if (user.Followed) Console.ForegroundColor = ConsoleColor.Green;
                                    else Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine(user.ToString());
                                    Tyms[index] = user;
                                }
                            }
                        }
                        catch (Exception)
                        {

                        }
                        ids = new StringBuilder();
                    }
                }
            }
            if (gatheredCount > 0)
            {
                try
                {
                    Tym[] response = InternalUpdate(tokens, ids.ToString());
                    foreach (Tym user in response)
                    {
                        if (user == null) continue;
                        int index = Tyms.IndexOf(user);
                        if (index != -1)
                        {
                            if (user.Followed) Console.ForegroundColor = ConsoleColor.Green;
                            else Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(user.ToString());
                            Tyms[index] = user;
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
            Tym.SaveTymList("tyms.dat", Tyms, true);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
        static Tym[] InternalUpdate(Tokens tokens, string ids)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("START REQUEST");
            Console.ForegroundColor = ConsoleColor.Gray;
            try
            {
                ListedResponse<User> users = tokens.Users.Lookup(user_id => ids);
                ListedResponse<Friendship> friendships = tokens.Friendships.Lookup(user_id => ids);
                Tym[] response = new Tym[users.Count];
                int i = 0;
                foreach (User user in users)
                {
                    Friendship fs = friendships.Where((x) => x.Id == user.Id).FirstOrDefault();
                    if (fs == null) continue;
                    response[i] = new Tym(user, fs.Connections.Contains("following"));
                    i++;
                }
                return response;
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("リクエストに失敗しました。: " + e.Message);
                Console.ForegroundColor = ConsoleColor.Gray;
                throw e;
            }
        }
        static void Clear(List<Tym> Tyms)
        {
            Console.WriteLine("Clear: 本当に削除しますか？[y/n]");
            if (Console.ReadLine() == "y")
            {
                Tyms.Clear();
                Tym.SaveTymList("tyms.dat", Tyms, true);
                Console.WriteLine("削除しました。");
            }
        }
        static void Follow(Tokens tokens, List<Tym> Tyms)
        {
            Console.WriteLine("Follow: フォローを開始します。");
            Console.WriteLine("Twitterルールに違反するフォロー行為はアカウントのロックの対象となる場合があります。");
            bool followSecret = false;
            Console.WriteLine("鍵アカをフォローしますか？[y/n]");
            if (Console.ReadLine() == "y")
            {
                followSecret = true;
            }
            bool followInactive = false;
            Console.WriteLine("活動していないユーザーをフォローしますか？[y/n]");
            if (Console.ReadLine() == "y")
            {
                followSecret = true;
            }
            Console.WriteLine("一度にフォローを行う最大数を設定しますか？[y/n]");
            int maxFollow = -1;
            if (Console.ReadLine() == "y")
            {
                Console.WriteLine("数値を入力してください：");
                Loop:
                if (!int.TryParse(Console.ReadLine(), out maxFollow) || maxFollow < 1)
                {
                    Console.WriteLine("有効な数値を入力してください：");
                    goto Loop;
                }
            }
            else
            {
                maxFollow = int.MaxValue;
            }
            //count
            int usersToFollow = Tyms.Where(tym => (((tym.IsSecret && followSecret) || (!tym.IsSecret && !followSecret)) && ((tym.isActive(30) && !followInactive) || (!tym.isActive(30) && followInactive)) && !tym.Followed)).Count();
            Console.WriteLine("これからフォローするユーザー数：" + (maxFollow < usersToFollow ? maxFollow.ToString() : usersToFollow.ToString()));

            Console.WriteLine("何かキーを押してフォローを開始します。");
            Tym.SaveTymList("tyms.dat", Tyms, true);
            Console.ReadKey();
            int counter = 0;
            for (int index = 0; index < Tyms.Count; index++)
            {
                Tym tym = Tyms[index];
                if (counter >= maxFollow)
                {
                    break;
                }
                if (((tym.IsSecret && followSecret) || (!tym.IsSecret && !followSecret)) && ((tym.isActive(30) && !followInactive) || (!tym.isActive(30) && followInactive)) && !tym.Followed)
                {
                    Console.WriteLine(tym.ToString());
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("フォロー中……");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    UserResponse followedUser;
                    try
                    {
                        followedUser = tokens.Friendships.Create(screen_name => tym.Screen_name);
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("エラーが発生しました：" + e.Message);
                        Console.ForegroundColor = ConsoleColor.Gray;
                        continue;
                    }
                    Tyms[index] = new Tym(followedUser, true);
                    Tym.SaveTymList("tyms.dat", Tyms, false);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(" => フォロー完了！");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                counter++;
            }
            Console.WriteLine("フォローが終了しました。");
        }
    }
}
