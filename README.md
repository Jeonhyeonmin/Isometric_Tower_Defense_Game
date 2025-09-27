<div align="center">
  <h1>🏰 Tower Defense Game</h1>
  <p>A Tower Defense Game with Strategy and Growth - System Expansion in Progress - Bug Fixes</p>
  <img src="https://img.shields.io/badge/Unity-6000.0.10f1-black?logo=unity&style=flat-square" alt="Unity Version" />
</div>

---

## 🧩 Project Introduction

This project is a Unity-based Tower Defense game, developed with various **design patterns** and a **modular architecture**. It leverages **Unity 6's GPU Resident Drawer** to maintain frame rates even with many objects being rendered. The focus was on clean code and maintainability, implementing a structure that considers both developers and users.

---

## 🔥 Key Features

| **Category**          | **Description**                                                          |
|-----------------------|--------------------------------------------------------------------------|
| **Architecture**      | MVC, Singleton, ScriptableObject, Object Pooling, etc.                   |
| **Camera**            | Isometric view camera implementation                                     |
| **Tower System**      | Tower placement, real-time upgrades, demolition, and grid management     |
| **Combat System**     | Various tower types, attack patterns, and range-based combat             |
| **Shop System**       | UI and logic for weapon and skill shops                                  |
| **Compendium System** | Encyclopedia feature to view information about enemies and towers        |
| **Quest System**      | Simple quest and reward system implementation                            |
| **External API**      | Profile setting and saving features using NativeGallery API              |
| **Image Editing Tool**| A tool to edit images loaded via API                                     |
| **User System**       | Login and registration features using **Back-end**                       |
| **Ranking System**    | Integration with **Back-end Ranking Server** to save user scores and provide real-time rankings |
| **Graphics**          | Simple yet clean graphic styling                                         |

---

## 📸 Screenshots & Videos

> Explore the game's atmosphere and features through various screenshots of gameplay, UI, shop, and compendium.

<table align="center">
  <tr>
    <td align="center">
      🎮 In-game Screen<br/>
      <img src="https://postfiles.pstatic.net/MjAyNTA0MDhfMTc0/MDAxNzQ0MDkzMDc4OTI0.82b7dQJBAPHoRskCLY2TxE-mPKUTV0sMtF5i34Je8IIg.oXFvbJ8__nVuCXhEnuKDqqZGQUqoWGGxGnAD2X-ZoJUg.PNG/Image_Sequence_020_0000.png?type=w966" width="300px"/>
    </td>
    <td align="center">
      ⚙️ Game Info Modification<br/>
      <img src="https://postfiles.pstatic.net/MjAyNTA0MDhfMjQx/MDAxNzQ0MDkzMDY5NDg3.Lv7-eZrhE2t0JdBA0Ofij5BOSwqnX1TUh2jCpTazlyYg.pWgF0175HxkTk1VoM37iAy1cj5x8UEHMHPNWY9_uGTgg.PNG/Image_Sequence_013_0000.png?type=w966" width="300px"/>
    </td>
  </tr>
  <tr>
    <td align="center">
      🎯 Game Difficulty Settings<br/>
      <img src="https://postfiles.pstatic.net/MjAyNTA0MDhfOTAg/MDAxNzQ0MDkzMTA3ODI4.2rNbQ2Fo8JJKxuD9q9Gc8LHvopptcp7elWTj0TUKJO0g.hXuhXZJfbSrMYfnluLfs5zODs0wHfy29nkkX3mQqTgsg.PNG/Image_Sequence_010_0000.png?type=w966" width="300px"/>
    </td>
    <td align="center">
      🛡️ Combat Screen<br/>
      <img src="https://postfiles.pstatic.net/MjAyNTA0MDhfMjY2/MDAxNzQ0MDk0MjEyNjkz.9rKDlpMWLnhZM9enTygkXMYydt_GfWy1ULt6pKw9Cdkg.Dyz-m32oGOTeEdNDfL78VzTy-Vz2F1V7vGn7IiyxiGEg.PNG/Image_Sequence_032_0000.png?type=w966" width="300px"/>
    </td>
  </tr>
  <tr>
    <td align="center">
      ⚙️ Game Settings Screen<br/>
      <img src="https://postfiles.pstatic.net/MjAyNTA0MDhfMjM0/MDAxNzQ0MDkzMTI4NDYw.w5ZCWJpWWX1j1-UflAhA9F1W3zGN3ffh52nEpRjEtmcg.ixlaWMhPhl8dfaV0iJOKlEnmGhRP1_EZJ8fS2J8nw8g.PNG/Image_Sequence_003_0000.png?type=w966" width="300px"/>
    </td>
    <td align="center">
      🎒 Inventory<br/>
      <img src="https://postfiles.pstatic.net/MjAyNTA0MDhfMjY1/MDAxNzQ0MDkzMTIwMzU4.H3_K2aCQhPf9YYN1tdGGkctwBcM0BOvFA5ZoJsmJPIg.NTvfmMqBc476ZUu-xP3X6gHDGaoW4ge2FcFb9BLPrSMg.PNG/Image_Sequence_009_0000.png?type=w966" width="300px"/>
    </td>
  </tr>
  <tr>
    <td align="center">
      📚 Compendium System<br/>
      <img src="https://postfiles.pstatic.net/MjAyNTA0MDhfMTA4/MDAxNzQ0MDkzMTE4MDU1.AonPK1Bj0fIh1ihgTy3pABrcF6WBlhjGufpLH-aHr-wg.mhO4DCLW1VTqv4NaQbyM_EOVJoDOfd7Y-8dgRZt8PCIg.PNG/Image_Sequence_008_0000.png?type=w966" width="300px"/>
    </td>
    <td align="center">
      💥 Skill Enhancement<br/>
      <img src="https://postfiles.pstatic.net/MjAyNTA0MDhfMjI2/MDAxNzQ0MDkzMTE2Mzgx.LF97tWlgd1pt8dLjANXZd83ZlaNAy_rcK66zkKurkc0g.t4RsKbdlAcdtfDT0S2vrvgRl3wYafPeMVEZa1gYXgTIg.PNG/Image_Sequence_007_0000.png?type=w966" width="300px"/>
    </td>
  </tr>
  <tr>
    <td align="center">
      🔫 Weapon Purchase and Enhancement<br/>
      <img src="https://postfiles.pstatic.net/MjAyNTA0MDhfMTc2/MDAxNzQ0MDkzMTE0Njc2.BPb9ziGRaMmmQx1w7j5_FiyDbDdeFHpCCmqlTNn8Qt4g.U_G88z-ru2LlEJSvwPFiKL3DXfv6Pu9wlB0lEOFzgFQg.PNG/Image_Sequence_006_0000.png?type=w966" width="300px"/>
    </td>
    <td align="center">
      🪙 Coin Exchange System<br/>
      <img src="https://postfiles.pstatic.net/MjAyNTA0MDhfMjIg/MDAxNzQ0MDkzNTg1NjE0.GFwlWDVWZWzLWB-O7kqaF0H5u1q-mYaI-kODfe5yXjog.zpf1GvbaqWeRSOpvuu8NXGNUAO166exuS-4T0l8Tl1Ug.PNG/Image_Sequence_015_0000.png?type=w966" width="300px"/>
    </td>
  </tr>
</table>

---

## 🧠 Tech Stack

- **Unity** 6000.0.10f1
- **C#**
- **ScriptableObject**, **Singleton**, **Object Pooling**
- **MVC Architecture**
- **NativeGallery** (External API for profile settings and saving)
- Other custom editor utilization

---
