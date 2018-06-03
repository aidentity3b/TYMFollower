# TYMFollower
TYMFollowerは戸山生っぽいTwitterアカウントをリスト化し、一括フォローする機能を提供します。
### 原理
ある特定の戸山生のTwitterアカウントを起点に、そのフォロー・フォロワーの中から戸山生っぽいアカウントを抽出します。(Direct/Specified Gathering)

戸山生かどうかの判断基準は、プロフィール内に以下のいずれかのキーワードが使用されているかどうかです。
```csharp:
string[] keywords = { "戸山", "とやま", "tym", "Tym", "TYM", "Toyama", "TOYAMA", "toyama", "めめち", "めめ物", "めめ化", "めめ生", "めめ地学", "めめ地", "めめ数", "めめぶつ", "めめか", "めめなま", "めめちがく" };
```
このコードを改変すれば戸山生以外の何らかのアカウントの判別に応用することもできます。

また、このようにして取得できた戸山生ユーザーを起点として、そのフォロー・フォロワーを検索することも可能です。(Indirect Gathering)

### 機能
- 戸山生アカウントの抽出と保存。
- リスト内の戸山生の一括フォロー。
  - 一か月以上ツイートしていないユーザーのフォロー回避。
  - 鍵アカのフォロー回避。
- リストのエクスポート。(JSON)

## 初心者向け
[ここ](https://github.com/aidentity3b/TYMFollower/releases)から最新版を落とせます。
落としたら展開して、中の「TYMFollower.exe」を実行してください。

このプログラムではTwitterのAPIを操作するので、事前に連携アプリの作成(APIキー)が必要です。

[https://apps.twitter.com](https://apps.twitter.com) にアクセスし、「Create New App」から新しく連携アプリを作成します。

Name(連携アプリの名前)、Description(連携アプリの説明)、 Website(あなたのサイトのURL)を適当に決めて作成します。(Callback URLは要りません)

作成できたら管理画面から「Keys and Access Tokens」に移動し、「Consumer Key (API Key)」と「Consumer Secret (API Secret)」をコピーします。TYMFollower側で入力を求められたらペーストしてください。
