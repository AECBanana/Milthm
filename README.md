# Milthm

**简体中文** | [English](https://github.com/buger404/Milthm/blob/master/README_EN.md)

<img src="Icon.png" alt="Milthm" style="zoom:50%;" />

**Mil**k Rhy**thm**，一款PC/移动端~~自娱自乐~~节奏音游(无轨)。

基础玩法：

* Tap，Hold
* Windows：A-Z任意键击打
* Android：全屏/垂直判定

谱面：

* 原创谱面格式

* 也支持导入Osu!Mania，Malody谱面(仅供娱乐，游戏内将明确标注源谱师、来自Osu!、Malody等信息，侵删)

# 项目进度

- [x] 标题画面
- [x] 选曲界面
- [x] 结算界面
- [x] 谱面读取、游玩
- [ ] 制谱器
- [ ] 谱面联机交流
- [ ] 账号系统
- [ ] 更多动画
- [ ] 谱面演出播放
- [x] 输入延迟调节

# 判定机制

分为“宽松”、“普通”、“严格”三种判定。

普通：

* ±30ms：Perfect+
* ±60ms：Perfect
* ±120ms：Good
* ±135ms：Bad

分数 = (最大连击数 / 总音符数) * 110000 + ((大P * 1.1 + 小P) / 总音符数 + Good / 总音符数 * 0.6 + Bad / 总音符数 * 0.1) * 900000

准度 = ((大P + 小P) / 总音符数 + Good  / 总音符数 * 0.75 + Bad  / 总音符数 * 0.5) * 100%

其余细节待补充。

# 制作人员名单

策划/制作/程序/界面设计：Buger404

部分美术：AI技术

# Milthm资源库声明

《Milthm》的出版商、作者和发布者希望向包括但不限于导演、插画师、摄影师、音乐制作人、用户界面设计师、游戏脚本作者和校对在内的个人表示诚挚的感谢，他们为这款游戏的创作贡献了时间、精力和专业知识。我们感谢他们的宝贵资源和建议，这对这个游戏的发展至关重要。

Copyright (C) 2023，Milthm制作组。保留所有权利。未经 "Milthm制作组"事先书面许可，不得以任何形式或通过任何与 "Milthm制作组"没有直接关系的方式使用本资源库中的资源，或将其存储在与 "Milthm制作组"没有直接关系的数据库或检索系统中。任何关于复制或使用本出版物的请求，应以书面形式提交给 "Milthm制作组"。未经原创作者或制作者的许可，任何人不得复制或使用本资源库中的资源，并以原始或修改的形式重新传播，用于商业目的。

# 第三方

* 游戏引擎

  Unity Personal

* 包(Unity Assets Store 正规购买)

  [Digital Ruby (Jeff Johnson)] Rain Maker - 2D and 3D Rain Particle System for Unity

  [Nectar Sonic Lab.] UI Sound Effects Collection Pack 2: Buttons

  [Tai's Assets] Translucent Image - Fast UI Background Blur

  [yasirkula] Native File Picker for Android & iOS

  许可证： [Standard Unity Asset Store EULA](https://unity3d.com/legal/as_terms)

* 开源项目

  [Bunny83/UnityWindowsFileDrag-Drop](https://github.com/Bunny83/UnityWindowsFileDrag-Drop)

  MIT License

  Copyright (c) 2018 Markus Göbel (Bunny83)

  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files (the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all
  copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
  SOFTWARE.

  [mob-sakai/SoftMaskForUGUI](https://github.com/mob-sakai/SoftMaskForUGUI)

  Copyright 2018-2020 mob-sakai

  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

* 图标(来自Flaticon)

  Sandglass  By Freepik

  Back  By Freepik

  Reload  By Freepik

  Play button  By Freepik

  Maximize  By kendis lasman

  Musical note  By Freepik

  Music note  By Freepik

  World wide web  By Freepik

  Editing  By Freepik

  Christmas stars  By Freepik

  Skip button  By Ranah Pixel Studio

  Trash  By Phoenix Group

  Add  By dmitri13

  Magic wand  By Freepik

  Rotate  By Pixel perfect

  许可证：[Flaticon License Certificate](https://media.flaticon.com/license/license.pdf)

* 音乐

  主界面：Sadness in the Rain - Jim Paterson(编辑过)