# OverdrawForURP

[**English**](README.md)

![OverdrawForURP](https://amagamina.jp/wp-content/uploads/2020/09/overdraw-change.gif)

UnityのBuilt-InレンダリングパイプラインにあってURPで使えなくなってしまったOverdraw表示を導入するものです。
SceneビューだけでなくGameビュー、ビルド後のアプリでも動作します。

Unity6以降のRenderGraphもサポートしています。

詳しい解説は[**こちら**](https://amagamina.jp/overdraw-for-urp/)

※Unity公式でもOverdraw表示はURP12（Unity2021.2）より[**RenderingDebugger**](https://docs.unity3d.com/ja/Packages/com.unity.render-pipelines.universal@14.0/manual/whats-new/urp-whats-new.html)という機能でサポートされるようになりました。

## インストール

upm経由でインストールする場合は `https://github.com/ina-amagami/OverdrawForURP.git` を指定して下さい。

```manifest.json
{
  "dependencies": {
    "jp.amagamina.overdraw-for-urp": "https://github.com/ina-amagami/OverdrawForURP.git"
  }
}
```

## セットアップ

1. UniversalRenderPipelineAssetにあるGeneral > RendererListの一覧に「OverdrawRenderer」を追加して下さい。

2. ビルド後のアプリで使用する場合はScripting Define Symbolsに`USE_RUNTIME_OVERDRAW`を追加して下さい。メインカメラをOverdraw表示に変更する[コード例](https://gist.github.com/ina-amagami/2f4a3b493d58333fdfcaa1baffbc066b)

## 動作確認バージョン

Unity 6000.0.36f1 / URP 17.0.3

## ライセンス条項

MITライセンス
https://opensource.org/licenses/mit-license.php

LICENSE.txtを残して頂ければ改変、再配布、商用利用等、自由にご使用頂けます。別途ライセンス表記は不要です。

Copyright (c) 2020-2025 ina-amagami (ina@amagamina.jp)
