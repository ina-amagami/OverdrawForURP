# OverdrawForURP

[**Japanese**](README_JP.md)

![OverdrawForURP](https://amagamina.jp/wp-content/uploads/2020/09/overdraw-change.gif)

It's scene overdraw view available in Universal Render Pipeline.
It works not only in the Scene view, but also in the Game view and in the app after it has been built.

Supports Unity6 & RenderGraph.

## Information

Unity official support for overdraw began in URP12 (Unity2021.2) with a feature [**RenderingDebugger**](https://docs.unity3d.com/ja/Packages/com.unity.render-pipelines.universal@14.0/manual/whats-new/urp-whats-new.html).

## Install

To install via upm, specify `https://github.com/ina-amagami/OverdrawForURP.git`.

```manifest.json
{
  "dependencies": {
    "jp.amagamina.overdraw-for-urp": "https://github.com/ina-amagami/OverdrawForURP.git"
  }
}
```

## Setup

1. Add "OverdrawRenderer" to the list of General > RendererList in the UniversalsRenderPipelineAsset. If you don't see "OverdrawRenderer" in the list of selections when you add it, press the eye symbol on the right side of the window to show it.

![rendererlist](https://amagamina.jp/wp-content/uploads/2020/09/overdraw-renderer-show.png)

2. Add `USE_RUNTIME_OVERDRAW` to Scripting Define Symbols if you want to use it in your app after building it. [Here](https://gist.github.com/ina-amagami/84a7cb3a05d57185362e76dcebaff2de) is a gist of an example script that shows how to Overdraw the main camera.

## How to

In the upper left corner of the Scene view, if you click on the button that is normally labeled "Shaded", an "Overdraw" item will be added, which you can enable by clicking here. If you want to bring it back, click on "Shaded".

![howto](https://amagamina.jp/wp-content/uploads/2020/09/overdraw-enable.png)

## ZTesting

In the Built-in pipeline, all objects are treated as transparent, but this makes it look as if all the pixels are overdraw.
Therefore, used ZTest for opaque objects.

If you want to disable ZTest, you can change "Opaque Shader" to "Overdraw-Transparent" in the "OverdrawRenderer" asset settings as shown below.

![disableZTest](https://amagamina.jp/wp-content/uploads/2020/09/opaque-to-transparent.png)

If you installed it from upm, you can't change it, so create a new "ForwardRenderer" asset and adjust the settings. the "Name" section must be "OverdrawRendererFeature".

## Warning

Overdraw is accomplished by changing the default renderer.

The target pipeline assets cannot be saved while using Overdraw. When updating render pipeline asset, please turn off Overdraw before saving.

## Verified

Unity 6000.0.36f1 / URP 17.0.3

## License

This software is released under the MIT License.
https://opensource.org/licenses/mit-license.php

Copyright (c) 2020-2025 ina-amagami (ina@amagamina.jp)
