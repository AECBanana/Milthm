# Milthm

**简体中文** | [English](https://github.com/buger404/Milthm/blob/master/README_EN.md)

<img src="Icon.png" alt="Milthm" style="zoom:50%;" />

**Mil**k Rhy**thm**，一款PC端~~自娱自乐~~节奏音游(无轨)。

基础玩法：

* Tap，Hold
* 按键：方向键/小键盘(考虑到这两个在不同电脑基本上残缺一个)
* 指定按键：A~Z

谱面：

* 将支持导入Osu!Mania，Osu!Taiko谱面(仅供娱乐，游戏内将明确标注源谱师、来自Osu!等信息，侵删)

# 项目进度

- [x] 标题画面
- [ ] 选曲界面
- [ ] 结算界面
- [ ] 谱面读取、游玩
- [ ] 制谱器
- [ ] 谱面联机交流
- [ ] 账号系统
- [ ] 更多动画
- [ ] 谱面演出播放
- [x] 输入延迟调节

# 判定机制

* ±30ms：Perfect+
* ±60ms：Perfect
* ±120ms：Good
* ±150ms：Bad

Perfect+与Perfect的视觉效果一致。

分数 = (最大连击数 / 总音符数) * 100000 + ((大P * 1.1 + 小P) / 总音符数 + Good / 总音符数 * 0.6 + Bad / 总音符数 * 0.3) * 900000

准度 = ((大P + 小P) / 总音符数 + Good  / 总音符数 * 0.6 + Bad  / 总音符数 * 0.3) * 100%

其余细节待补充。

# 第三方

* 游戏引擎

  Unity Personal

* 包(Unity Assets Store 正规购买)

  [Digital Ruby (Jeff Johnson)] Rain Maker - 2D and 3D Rain Particle System for Unity

  [Nectar Sonic Lab.] UI Sound Effects Collection Pack 2: Buttons

  [Tai's Assets] Translucent Image - Fast UI Background Blur

  许可证： [Standard Unity Asset Store EULA](https://unity3d.com/legal/as_terms)

* 图标(来自Flaticon)

  Sandglass By Freepik

  许可证：[Flaticon License Certificate](https://media.flaticon.com/license/license.pdf)

* 音乐

  主界面：Sadness in the Rain - Jim Paterson(改编)

# 制作人员名单

策划/制作/程序/界面设计：Buger404

部分美术：AI技术