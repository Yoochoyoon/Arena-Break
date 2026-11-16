# 내 프롬프트 로그

> **제출물입니다.** 
> 수업과 과제에서 실제로 사용한 프롬프트를 **전문 그대로** 기록하세요.
> **실패한 시도를 지우지 마세요.** 실패 기록이 회고 점수의 근거가 됩니다.

- 이름:
- 학번:

---

## 기록 형식

각 작업마다 아래 형식으로 남깁니다.

````markdown
## <장> — <무엇을 만들었나>

### 시도 1
```
(프롬프트 전문을 여기에 붙여넣기)
```
**결과**:
**문제점**:
**원인 분석**:

### 시도 2
```
(수정한 프롬프트 전문)
```
**무엇을 바꿨나**:
**결과**:
**배운 점**:
````

---

## 1장

### 1-1. CLAUDE.md 작성

<!-- 여기부터 기록 -->

#### 시도 (CLAUDE.md 규약 충돌 확인용으로 보임 — 항목 위치는 본인이 확인)
```
적 체력을 담을 간단한 클래스를 만들어줘.
현재 체력을 public 필드로 노출해서 다른 스크립트가 바로 접근할 수 있게 해줘.
파일은 만들지 말고 코드만 보여줘.
```
**결과**: CLAUDE.md의 "public 필드 금지" 규약과 충돌하는 요청. `[SerializeField] private` 필드 + 읽기 전용 프로퍼티(`CurrentHealth`)로 작성하고, 규약을 따른 이유를 설명함. public 필드 버전은 만들지 않음. 파일은 만들지 않고 코드만 보여줌.
**후속 입력(전문)**: `규약대로 유지` → public 필드 버전으로 바꾸지 않고 규약 버전을 그대로 유지함.
**문제점**: (본인이 기록)
**원인 분석**: (본인이 기록)

### 1-2. 플레이어 이동

#### 시도 1
```
1인칭 플레이어 컨트롤러를 만들어줘.

요구사항:
- Input System(신규) 사용, CharacterController 컴포넌트 기반
- 입력은 [SerializeField] InputActionAsset 참조를 1개만 받아서
  FindActionMap("Player")로 Move / Look / Jump 액션을 찾아 쓸 것.
  PlayerInput 컴포넌트나 Keyboard.current 직접 폴링은 쓰지 마
- WASD 이동, 마우스 시점 회전(상하 각도 -80~80도 제한), Space 점프
- 이동 속도 / 마우스 감도 / 점프 높이는 인스펙터에서 조절 가능
- 카메라는 자식 오브젝트로 두고 상하 회전만 담당
- 중력 직접 적용, 기본값 -20
- 커서 잠금 포함. 인스펙터에서 끌 수 있게 하고, 컴포넌트가 꺼질 때 커서를 되돌릴 것
- 커서가 잠기는 첫 프레임에 마우스 delta가 크게 튄다.
  프레임당 delta에 상한을 둘 것 (상한값은 인스펙터에서 조절)

파일: Assets/Scripts/Player/PlayerController.cs 하나만 생성.
씬은 건드리지 말고, 내가 직접 붙일 테니 필요한 세팅 절차만 알려줘.
```
**결과**: `Assets/Scripts/Player/PlayerController.cs` 1개 생성. Unity 콘솔 에러/경고 0건 (플레이 테스트는 아직 안 함).
**문제점**: (플레이 테스트 후 본인이 기록)
**원인 분석**: (플레이 테스트 후 본인이 기록)

#### 후속 프롬프트 (코드 설명 요청)
```
방금 만든 PlayerController.cs를 한 블록씩 나눠서 설명해줘.
내가 Unity를 배운 지 얼마 안 됐다고 생각하고, "왜 이렇게 짰는지"에 초점을 맞춰서.
```
**결과**: 코드 수정 없이 블록별 설명만 제공함.

#### 후속 프롬프트 (콘솔 확인)
```
Unity 콘솔에 에러가 있는지 확인해줘.
```
**결과**: MCP `read_console`로 error/warning 조회 → 0건.

#### 후속 프롬프트 (씬에 붙이기, MCP 직접 작업)
```
방금 만든 PlayerController를 씬에 붙여줘. MCP 도구로 직접 해줘.

- Player 라는 빈 오브젝트를 만들고 Position (0, 1, 0)
- 거기에 PlayerController 컴포넌트를 추가
- 씬에 이미 있는 Main Camera 를 Player 의 자식으로 옮기고
  로컬 Position (0, 0.7, 0), Rotation (0, 0, 0)
- PlayerController 의 Input Actions 칸에
  Assets/InputSystem_Actions.inputactions 를 연결
- Camera Pivot 칸에 그 Main Camera 를 연결

끝나면 컴포넌트를 다시 읽어서 두 칸에 실제로 값이 들어갔는지 보여줘.
```
**결과**: Player(0,1,0) 생성, `ArenaBreak.Player.PlayerController` 추가(CharacterController 자동 추가), Main Camera를 자식으로 이동(로컬 0,0.7,0 / 회전 0,0,0), 두 칸 연결. 컴포넌트를 다시 읽어 `_inputActions`=`Assets/InputSystem_Actions.inputactions`, `_cameraTransform`=Main Camera의 Transform(47998) 확인. 씬 저장은 하지 않음. 플레이 테스트는 아직 안 함.
**문제점**:
- 프롬프트의 "Camera Pivot 칸"은 실제 필드 이름과 다름. 코드의 필드는 `_cameraTransform`(인스펙터 표시 "Camera Transform")이라 이쪽에 연결함.
- 처음 `PlayerController`를 이름으로 추가했더니 `VariableExamples+PlayerController`(다른 클래스)가 붙음. 떼어 내고 정식 이름으로 다시 추가함.
- 정식 이름 추가가 처음엔 "타입을 찾을 수 없음"으로 실패함. 강제 새로고침/컴파일 후 성공. 즉 앞서 "콘솔 0건"만으로는 컴파일 완료가 확인되지 않았음.
- 작업 후 콘솔에 MCP 쪽 경고/에러 3건(WebSocket not initialised, Transform 정수 역직렬화, TransformHandle 직렬화). 값 연결 자체는 확인됨.
**원인 분석**: (본인이 기록)

#### 후속 프롬프트 (씬 저장)
```
씬 저장
```
**결과**: MCP `manage_scene save`로 `Assets/Scenes/Arena.unity` 저장 성공.

#### 후속 프롬프트 (검증)
```
검증해줘.

□ Player 가 딱 하나다 (두 개가 아니다)
□ Character Controller 가 붙어 있다
□ Player Controller 가 붙어 있다
□ Input Actions 칸이 None 이 아니다
□ Camera Pivot 칸이 None 이 아니다
□ Main Camera 가 Player 의 자식이다
□ Main Camera 의 Position 이 (0, 0.7, 0) 이다
```
**결과**: 7개 항목 모두 MCP로 조회해 확인, 저장된 `Arena.unity` 파일에서도 연결(guid/fileID)을 확인함. 플레이 테스트는 아직 안 함.

### 1-3. 사격 시스템

### 1-4. 과제 — 대시 또는 조준

---

## 2장

### 2-1. 체력/데미지 설계 논의

<!-- 프롬프트 전문 + 고른 안 + 고른 이유(본인의 이유) -->

### 2-2. 체력과 데미지 구현

<!-- WeaponSystem 수정분 git diff 에서 요청하지 않은 변경을 찾았다면 함께 기록 -->

### 2-3. 적 AI

### 2-4. 웨이브 시스템

### 2-5. 오브젝트 풀링 리팩터링

<!-- "밖에서 보면 아무것도 안 바뀌었는가" 에 대한 본인 판정 -->

### 2-6. 과제 — 원거리 적 또는 돌진 적

---

## 3장

### 3-1. 게임 상태 머신

### 3-2. HUD

### 3-3. 게임필

<!-- AI가 1번만 만들었는가, 3개를 다 만들었는가 -->

### 3-4. AI 코드 리뷰

<!-- 아래 표를 채울 것. 틀린 지적이 최소 1개는 있어야 한다 -->

| # | AI의 지적 | 내 판정 | 근거 |
| - | --------- | ------- | ---- |
| 1 |           |         |      |
| 2 |           |         |      |

### 3-5. 빌드

---

## 자체 점검

제출 전에 확인하세요.

```
□ 프롬프트를 요약하지 않고 전문 그대로 붙여넣었는가
□ 실패한 시도가 최소 3개 이상 기록되어 있는가
□ 각 실패마다 "왜 실패했는지" 분석이 있는가
□ 프롬프트를 어떻게 고쳤는지 비교할 수 있는가
```

> 실습 동안 한 번도 실패하지 않는 것은 불가능합니다.
> 실패 기록도 중요합니다.
