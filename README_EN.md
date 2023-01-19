# Milthm

[简体中文](https://github.com/buger404/Milthm/blob/master/README.md) | **English**

<img src="Icon.png" alt="Milthm" style="zoom:50%;" />

**Mil**k Rhy**thm**，a simple rhythm game ~~made for fun~~ for PC/Mobile.

Base Game Mechanics：

* Tap，Hold
* Windows：A-Z Anykey to hit
* Android：Touch anywhere to hit

Beatmap：

* Original beatmap format

* Also support importing beatmaps from Osu!Mania, Osu!Taiko(Just for fun, we will include their original information including creator and note they are from Osu! clearly, **we will delete this function if this hurt Osu!'s rights**) 

# Project Progress

- [x] Title UI
- [x] Song Select UI
- [x] Game Settlement UI
- [x] Beatmap Loader and Game Play
- [ ] Beatmap Editor
- [ ] Beatmap Online Disscussion
- [ ] Account System
- [ ] More Animations
- [ ] Beatmap Show
- [x] Input Delay Settings

# Judge Mechanics

* ±30ms：Perfect+
* ±60ms：Perfect
* ±120ms：Good
* ±135ms：Bad

Score = (`Max Combo` / `Full Combo`) * 110000 + ((`Perfect+` * 1.1 + `Perfect`) / `Full Combo` + `Good` / `Full Combo` * 0.6 + `Bad` / `Full Combo` * 0.1) * 900000

Accuracy = ((`Perfect+` + `Perfect`) / `Full Combo` + `Good`  / `Full Combo` * 0.75 + `Bad`  / `Full Combo` * 0.5) * 100%

More details are still considering.

# Credits

Design/Produce/Programming/UI Design：Buger404

Some Art：AI Technology

# Third Party

* Game Engine

  Unity Personal

* Packages(Purchase from Unity Assets Store)

  [Digital Ruby (Jeff Johnson)] Rain Maker - 2D and 3D Rain Particle System for Unity

  [Nectar Sonic Lab.] UI Sound Effects Collection Pack 2: Buttons

  [Tai's Assets] Translucent Image - Fast UI Background Blur

  [yasirkula] Native File Picker for Android & iOS

  License： [Standard Unity Asset Store EULA](https://unity3d.com/legal/as_terms)

* Opensource Projects

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

* Icons(from Flaticon)

  Sandglass By Freepik

  Back  By Freepik

  Reload  By Freepik

  Play button  By Freepik

  Maximize  By kendis lasman

  Musical note  By Freepik

  Pause  By Kiranshastry

  Setting  By Freepik

  License：[Flaticon License Certificate](https://media.flaticon.com/license/license.pdf)

* Music

  Title UI BGM：Sadness in the Rain - Jim Paterson(Edited)
