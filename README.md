# SimpleUnity
    简易的unity2022.3项目，用于调试各种小功能，例如调试微信sdk登录，调试某个平台接口等。
## LearnCrash
    用于调试unity崩溃。借助Unity内建脚本，触发一个crash，然后拿到bmp文件进行调试。
---
## LearnGitHubWebsite
    调试github的静态网页项目，点击一个按钮后开始请求静态网页地址
[调试静态网页](https://jyddtopen.github.io/SimpleUnity.github.io/?message=Hello)
---
## LearnWebGL
    用于构建出一个简易的webgl项目，构建时使用自定义的编辑器入口，而非默认的入口。     
[webgl主页](https://jyddtopen.github.io/SimpleUnity.github.io/WebGlGame)
---
## LearnWxGame
    用于调试微信小游戏，例如获取用户协议，交换code码等
[微信小游戏webgl主页](https://jyddtopen.github.io/SimpleUnity.github.io/webgl)
---

1.调试本机产生的崩溃文件
     可以认为是本地的Unity生成的player包触发的崩溃文件，这种情况最为简单，直接右键调试即可，无需过多设置。当然，这仅限制于本地调试用，真正到了发布阶段，肯定不会有这种情况。
接下来是详细说明
1.1 拿到崩溃日志
