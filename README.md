<div align="center">
  <h1>🏰 Tower Defense Game</h1>
  <p>전략과 성장의 재미를 담은, 타워 디펜스 - 시스템 추가 중 - 버그 픽스</p>
  <img src="https://img.shields.io/badge/Unity-6000.0.10f1-black?logo=unity&style=flat-square" alt="Unity Version" />
</div>

---

## 🧩 프로젝트 소개

이 프로젝트는 Unity 기반의 타워 디펜스 게임으로,  
다양한 **디자인 패턴**과 **모듈형 아키텍처**를 적용해 개발되었습니다.  
**유니티 6**의 **GPU Resident Drawer**를 이용하여 많은 오브젝트가 렌더링이 되더라도 프레임 방어가 가능합니다.<br>
클린 코드와 유지보수에 초점을 맞춰, 개발자와 유저 모두를 고려한 구조를 구현했습니다.

---

## 🔥 주요 기능

| **범주**            | **내용**                                                                 |
|---------------------|--------------------------------------------------------------------------|
| **아키텍처**         | MVC, Singleton, ScriptableObject, Object Pooling 등                      |
| **카메라**           | 아이소메트릭 뷰 카메라 구현                                              |
| **타워 시스템**      | 타워 설치, 실시간 업그레이드, 철거 및 그리드 배치 관리                    |
| **전투 시스템**      | 다양한 타워 유형, 공격 패턴 및 범위 기반 전투 구현                         |
| **상점 시스템**      | 총기 및 스킬 상점 UI와 로직 구현                                          |
| **도감 시스템**      | 적과 타워 정보를 열람할 수 있는 도감 기능                                  |
| **퀘스트 시스템**    | 간단한 퀘스트 및 보상 시스템 구현                                       |
| **외부 API**         | NativeGallery API를 통한 프로필 세팅 및 저장 기능                           |
| **이미지 편집 도구**  | API를 통해 불러온 이미지를 편집할 수 있는 툴 제공                           |
| **유저 시스템**      | **뒤끝**을 활용한 로그인 및 회원가입 기능 구현                   |
| **랭킹 시스템**      | **뒤끝 랭킹 서버**와 연동하여 유저별 점수 저장 및 실시간 랭킹 제공        |
| **그래픽**           | 심플하면서도 깔끔한 그래픽 스타일링                                     |

---

## 📸 스크린샷 & 영상

> 플레이 장면, UI, 상점, 도감 등 다양한 스크린샷을 통해 게임의 분위기와 기능을 확인할 수 있습니다.

<table align="center">
  <tr>
    <td align="center">
      🎮 인게임 화면<br/>
      <img src="https://postfiles.pstatic.net/MjAyNTA0MDhfMTc0/MDAxNzQ0MDkzMDc4OTI0.82b7dQJBAPHoRskCLY2TxE-mPKUTV0sMtF5i34Je8IIg.oXFvbJ8__nVuCXhEnuKDqqZGQUqoWGGxGnAD2X-ZoJUg.PNG/Image_Sequence_020_0000.png?type=w966" width="300px"/>
    </td>
    <td align="center">
      ⚙️ 게임 정보 수정<br/>
      <img src="https://postfiles.pstatic.net/MjAyNTA0MDhfMjQx/MDAxNzQ0MDkzMDY5NDg3.Lv7-eZrhE2t0JdBA0Ofij5BOSwqnX1TUh2jCpTazlyYg.pWgF0175HxkTk1VoM37iAy1cj5x8UEHMhPNWY9_uGTgg.PNG/Image_Sequence_013_0000.png?type=w966" width="300px"/>
    </td>
  </tr>
  <tr>
    <td align="center">
      🎯 게임 난이도 설정<br/>
      <img src="https://postfiles.pstatic.net/MjAyNTA0MDhfOTAg/MDAxNzQ0MDkzMTA3ODI4.2rNbQ2Fo8JJKxuD9q9Gc8LHvopptcp7elWTj0TUKJO0g.hXuhXZJfbSrMYfnluLfs5zODs0wHfy29nkkX3mQqTgsg.PNG/Image_Sequence_010_0000.png?type=w966" width="300px"/>
    </td>
    <td align="center">
      🛡️ 전투 화면<br/>
      <img src="https://postfiles.pstatic.net/MjAyNTA0MDhfMjY2/MDAxNzQ0MDk0MjEyNjkz.9rKDlpMWLnhZM9enTygkXMYydt_GfWy1ULt6pKw9Cdkg.Dyz-m32oGOTeEdNDfL78VzTy-Vz2F1V7vGn7IiyxiGEg.PNG/Image_Sequence_032_0000.png?type=w966" width="300px"/>
    </td>
  </tr>
  <tr>
    <td align="center">
      ⚙️ 게임 세팅 화면<br/>
      <img src="https://postfiles.pstatic.net/MjAyNTA0MDhfMjM0/MDAxNzQ0MDkzMTI4NDYw.w5ZCWJpWWX1j1-UflAhA9F1W3zGN3ffh52nEpRjEtmcg.ixlaWMhPhl8dfaV0iJOKlEnmGhRP1_EZwJ8fS2J8nw8g.PNG/Image_Sequence_003_0000.png?type=w966" width="300px"/>
    </td>
    <td align="center">
      🎒 인벤토리<br/>
      <img src="https://postfiles.pstatic.net/MjAyNTA0MDhfMjY1/MDAxNzQ0MDkzMTIwMzU4.H3_K2aCQhPf9YY5_1tdGGkctwBcM0BOvFA5ZoJsmJPIg.NTvfmMqBc476ZUu-xP3X6gHDGaoW4ge2FcFb9BLPrSMg.PNG/Image_Sequence_009_0000.png?type=w966" width="300px"/>
    </td>
  </tr>
  <tr>
    <td align="center">
      📚 도감 시스템<br/>
      <img src="https://postfiles.pstatic.net/MjAyNTA0MDhfMTA4/MDAxNzQ0MDkzMTE4MDU1.AonPK1Bj0fIh1ihgTy3pABrcF6WBlhjGufpLH-aHr-wg.mhO4DCLW1VTqv4NaQbyM_EOVJoDOfd7Y-8dgRZt8PCIg.PNG/Image_Sequence_008_0000.png?type=w966" width="300px"/>
    </td>
    <td align="center">
      💥 스킬 강화<br/>
      <img src="https://postfiles.pstatic.net/MjAyNTA0MDhfMjI2/MDAxNzQ0MDkzMTE2Mzgx.LF97tWlgd1pt8dLjANXZd83ZlaNAy_rcK66zkKurkc0g.t4RsKbdlAcdtfDT0S2vrvgRl3wYafPeMVEZa1gYXgTIg.PNG/Image_Sequence_007_0000.png?type=w966" width="300px"/>
    </td>
  </tr>
  <tr>
    <td align="center">
      🔫 무기 구매 및 강화<br/>
      <img src="https://postfiles.pstatic.net/MjAyNTA0MDhfMTc2/MDAxNzQ0MDkzMTE0Njc2.BPb9ziGRaMmmQx1w7j5_FiyDbDdeFHpCCmqlTNn8Qt4g.U_G88z-ru2LlEJSvwPFiKL3DXfv6Pu9wlB0lEOFzgFQg.PNG/Image_Sequence_006_0000.png?type=w966" width="300px"/>
    </td>
    <td align="center">
      🪙 코인 교환 시스템<br/>
      <img src="https://postfiles.pstatic.net/MjAyNTA0MDhfMjIg/MDAxNzQ0MDkzNTg1NjE0.GFwlWDVWZWzLWB-O7kqaF0H5u1q-mYaI-kODfe5yXjog.zpf1GvbaqWeRSOpvuu8NXGNUAO166exuS-4T0l8Tl1Ug.PNG/Image_Sequence_015_0000.png?type=w966" width="300px"/>
    </td>
  </tr>
</table>

---

## 🧠 기술 스택

- **Unity** 6000.0.10f1  
- **C#**  
- **ScriptableObject**, **Singleton**, **Object Pooling**  
- **MVC 아키텍처**  
- **NativeGallery** (프로필 설정 및 저장 외부 API)  
- 기타 커스텀 에디터 활용  

---

### ✅ 1. 전체 조건
- **Unity Editor**: 최소 6000.0.10f1 버전 이상의 Unity 환경이 필요합니다.  
- **플랫폼**: PC, 모바일 등 대상 플랫폼에 맞게 빌드 후 실행.

### 🛠️ 2. 설치 단계
1. **프로젝트 클론 및 다운로드**  
터미널 또는 Git 클라이언트를 사용하여 프로젝트 저장소를 클론합니다.
```bash
git clone https://github.com/your_username/tower-defense-game.git
```

2. **Unity에서 프로젝트 열기**  
Unity Hub를 실행한 뒤, 클론 받은 프로젝트 폴더를 선택하여 프로젝트를 엽니다.

4. **씬 실행**  
메인 씬 (예: Menu_Screen.unity)을 열고 재생 버튼을 누르면 게임이 실행됩니다.

4. **게임 씬 사이즈 조절**  
Fixed Resolution(1080x1920) 사이즈로 게임 씬 사이즈를 조절합니다. 

---

## 📝 개발 가이드

### 코드 구조 및 작성 규칙
- **클린 코드**를 지향(?)하며, 각 클래스와 메소드는 역할에 맞게 분리되어 작성되었습니다.  
- **주석**은 필수적인 부분에 추가하여 코드 가독성을 높였습니다.  
- **모듈화**: 타워, 적, UI 등 각각의 기능은 독립적인 모듈로 관리되며, 서로 간의 의존성을 최소화했습니다.

### 커스텀 에디터 확장
- **에디터 도구**: 게임 디자이너들이 더 쉽게 타워 배치 및 속성 조절을 할 수 있도록 커스텀 에디터를 개발했습니다.  
- **ScriptableObject 활용**: 다양한 게임 데이터를 에셋으로 관리하여, 인스펙터에서 쉽게 수정할 수 있도록 구성했습니다.

---

## 🤝 기여 방법

### 1. 이슈 제출
프로젝트에 버그나 개선점이 있다면, GitHub Issue에 이슈를 등록해주시면 감사하겠습니다.

### 2. 포크 및 풀 리퀘스트
- 프로젝트를 포크한 후, 본인의 로컬 저장소에서 작업합니다.  
- 기능 추가 또는 버그 픽스 후, 변경 사항을 커밋합니다.  
- GitHub를 통해 풀 리퀘스트(PR)를 제출하면, 리뷰 후 프로젝트에 반영합니다.

---

## 🔮 향후 계획
- **게임 시스템 완벽 구현**: 게임 내 미완성 된 시스템을 완전하게 개발 
- **추가 타워 및 스킬**: 플레이 스타일에 맞춘 다양한 타워와 스킬 추가  
- **멀티플레이어 모드**: 온라인 대전 기능 도입 검토  
- **AI 개선**: 적 AI 알고리즘의 고도화 및 새로운 전투 패턴 도입  
- **UI/UX 개선**: 사용자 피드백을 반영한 UI 리디자인 및 인터랙션 강화  
- **새로운 모드 추가**: 서바이벌, 타임 어택 등 다양한 게임 모드 추가

---

## 📄 라이선스
이 프로젝트는 MIT 라이선스를 따릅니다.  
자세한 내용은 LICENSE 파일을 참고하세요.
