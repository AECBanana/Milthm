# Milthm

[简体中文](https://github.com/buger404/Milthm/blob/master/README.md) | **English**

<img src="Icon.png" alt="Milthm" style="zoom:50%;" />

**Mil**k Rhy**thm**，a simple rhythm game ~~made for fun~~ for PC.

Base Game Mechanics：

* Tap，Hold
* Input：Arrow Keys/Number Pad(Considering one of them may be unavaliable on most PC)
* Specific Input：A~Z

Beatmap：

* Support importing beatmaps from Osu!Mania, Osu!Taiko(Just for fun, we will include their original information including creator and note they are from Osu! clearly, **we will delete this function if this hurt Osu!'s rights**) 

# Project Progress

- [x] Title UI
- [ ] Song Select UI
- [ ] Game Settlement UI
- [ ] Beatmap Loader and Playing
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
* ±150ms：Bad

The visual effects of `Perfect+` and `Perfect` are the same.

Score = (`Max Combo` / `Full Combo`) * 100000 + ((`Perfect+` * 1.1 + `Perfect`) / `Full Combo` + `Good` / `Full Combo` * 0.6 + `Bad` / `Full Combo` * 0.3) * 900000

Accuracy = ((`Perfect+` + `Perfect`) / `Full Combo` + `Good`  / `Full Combo` * 0.6 + `Bad`  / `Full Combo` * 0.3) * 100%

More details are still considering.

# Third Party

* Game Engine

  Unity Personal

* Packages(Purchase from Unity Assets Store)

  [Digital Ruby (Jeff Johnson)] Rain Maker - 2D and 3D Rain Particle System for Unity

  [Nectar Sonic Lab.] UI Sound Effects Collection Pack 2: Buttons

  [Tai's Assets] Translucent Image - Fast UI Background Blur

  License： [Standard Unity Asset Store EULA](https://unity3d.com/legal/as_terms)

* Icons(from Flaticon)

  Sandglass By Freepik

  License：[Flaticon License Certificate](https://media.flaticon.com/license/license.pdf)

* Music

  Title UI BGM：Sadness in the Rain - Jim Paterson(Edited)

# Credits

Design/Produce/Programming/UI Design：Buger404

Some Art：AI Technology