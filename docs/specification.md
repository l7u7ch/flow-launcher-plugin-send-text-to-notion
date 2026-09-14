# 機能仕様

## 目的

Flow Launcher に入力されたメモを、設定済みの Notion データベースへ新しいページとして送信します。

## クエリの動作

| 状態 | 表示と動作 |
| --- | --- |
| `ApiToken` または `DatabaseId` が未設定 | 設定が必要であることを表示し、選択時に設定画面を開く |
| 前後の空白を除いた入力が空 | 入力を促し、選択しても送信しない |
| テキスト入力済み | メモと Database ID の先頭8文字を表示し、選択時に送信する |
| 送信成功 | 完了通知を表示し、Flow Launcher を閉じる |
| 送信失敗 | エラーを表示し、Flow Launcher を開いたままにする |

## 設定

| 設定値 | 必須 | 既定値 | 取り扱い |
| --- | --- | --- | --- |
| `ApiToken` | はい | 空文字 | Notion Internal Integration のトークン |
| `DatabaseId` | はい | 空文字 | 前後の空白を除去して保存し、送信前にハイフンを除去する |
| `TitlePropertyName` | いいえ | `Name` | 空欄の場合は `Name` を保存する |

設定は Flow Launcher の設定ストレージを通じて保存されます。

## Notion API 契約

次の Pages API を呼び出します。

```http
POST https://api.notion.com/v1/pages
Authorization: Bearer <ApiToken>
Notion-Version: 2022-06-28
Content-Type: application/json
```

```json
{
  "parent": {
    "database_id": "<ハイフンを除去したDatabaseId>"
  },
  "properties": {
    "<TitlePropertyName>": {
      "title": [
        {
          "text": {
            "content": "<前後の空白を除去したメモ>"
          }
        }
      ]
    }
  }
}
```

2xx レスポンスを成功として扱います。失敗時の表示内容は次のとおりです。

| 状態 | エラー文字列 |
| --- | --- |
| 2xx 以外 | `HTTP {ステータスコード}: {レスポンス本文}` |
| キャンセル | `Request cancelled.` |
| HTTP 通信例外 | `HttpRequestException.Message` |

## 制限

- 送信先は1データベースに固定されます。
- タイトルプロパティ以外には書き込みません。
- オフライン時の下書き保存には対応しません。
- Notion OAuth には対応せず、Internal Integration を使用します。
