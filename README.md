<div align="center">
  <h1>🏰 Tower Defense Game</h1>
  <p>전략과 성장의 재미를 담은, 심플하지만 강력한 타워 디펜스</p>
  <img src="https://img.shields.io/badge/Unity-6000.0.10f1-black?logo=unity&style=flat-square" alt="Unity Version" />
  <img src="https://img.shields.io/badge/Pattern-MVC%20%7C%20Singleton%20%7C%20Object%20Pooling-green?style=flat-square" alt="Design Patterns" />
</div>

---

## 🧩 프로젝트 소개

이 프로젝트는 Unity 기반의 타워 디펜스 게임으로,  
다양한 **디자인 패턴**과 **모듈형 아키텍처**를 적용해 개발되었습니다.  
클린 코드와 유지보수에 초점을 맞춰, 개발자와 유저 모두를 고려한 구조를 구현했습니다.

---

## 🔥 주요 기능

| 범주            | 내용                                                         |
|-----------------|--------------------------------------------------------------|
| **아키텍처**      | MVC, Singleton, ScriptableObject, Object Pooling             |
| **카메라**        | 아이소메트릭 뷰 카메라 구현                                  |
| **타워 시스템**   | 타워 설치, 실시간 업그레이드, 철거 및 그리드 배치 관리          |
| **전투 시스템**   | 다양한 타워 유형, 공격 패턴 및 범위 기반 전투 구현             |
| **상점 시스템**   | 총기 및 스킬 상점 UI와 로직 구현                              |
| **도감 시스템**   | 적과 타워 정보를 열람할 수 있는 도감 기능                      |
| **퀘스트 시스템** | 간단한 퀘스트 및 보상 시스템 구현                           |
| **외부 API**      | NativeGallery API를 통한 스크린샷 저장 기능                    |
| **그래픽**        | 심플하면서도 깔끔한 그래픽 스타일링                           |

---

## 📸 스크린샷 & 영상

> 플레이 장면, UI, 상점, 도감 등 다양한 스크린샷을 통해 게임의 분위기와 기능을 확인할 수 있습니다.

<div align="center">
  <img src="Screenshots/gameplay.png" width="600px" alt="타워 설치 장면 예시" />
  <p>📸 <i>타워 설치 장면 (예시)</i></p>
</div>

---

## 🧠 기술 스택

- **Unity** 6000.0.10f1
- **C#**
- **ScriptableObject**, **Singleton**, **Object Pooling**
- **MVC 아키텍처**
- **NativeGallery** (스크린샷 저장 외부 API)
- 기타 커스텀 에디터 활용

---

## 🔄 아키텍처 다이어그램

```mermaid
flowchart TD
    A[플레이어 입력] --> B[컨트롤러 계층]
    B --> C[모델 (ScriptableObject)]
    C --> D[뷰 (타워, UI)]
    B --> E[시스템 로직 (타워/상점/퀘스트)]
