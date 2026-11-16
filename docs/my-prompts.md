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
**문제점**: 처음 요청한 방식은 다른 스크립트에서 체력에 직접 접근할 수 있도록 public 필드를 사용하는 것이었다. 하지만 프로젝트의 코딩 규약과 맞지 않아 요청한 형태 그대로 구현되지 않았다.
**원인 분석**: 프롬프트에서 원하는 구현 방식을 구체적으로 지정했지만, 기존 프로젝트의 CLAUDE.md 규약을 고려하지 못했다. AI가 기존 규약을 우선 적용하면서 요청한 방식과 실제 결과에 차이가 발생했다.

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
**문제점**: 현재 코드 생성과 컴파일 확인은 완료했지만, 실제 플레이 테스트를 진행하지 않아 이동, 점프, 마우스 시점 회전이 정상적으로 작동하는지는 확인하지 못했다.
**원인 분석**: 코드를 생성하고 Unity에 연결하는 과정에 집중했으며, 실제 입력을 통한 동작 검증은 아직 진행하지 않았기 때문이다.

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
**원인 분석**:
- 프롬프트에서 실제 필드 이름인 `Camera Transform` 대신 `Camera Pivot`이라고 표현해 코드와 인스펙터의 명칭을 정확히 일치시키지 못했다. AI가 실제 코드를 확인한 뒤 올바른 필드에 연결했지만, 처음부터 기존 코드의 필드명을 확인하지 않아 추가 확인 과정이 발생했다.
- `PlayerController`라는 클래스 이름만으로 컴포넌트를 추가해 다른 스크립트에 선언된 동명의 클래스가 선택되었다. 프로젝트 내에 동일한 클래스명이 존재할 가능성을 고려하지 않고 클래스의 전체 네임스페이스를 지정하지 않은 것이 원인이었다.
- 정식 클래스 추가가 처음에 실패한 것은 Unity의 스크립트 컴파일 및 타입 갱신이 완료되기 전에 컴포넌트 추가를 시도했기 때문으로 볼 수 있다. 이전에 콘솔 오류가 없다는 사실만으로 새 클래스가 정상적으로 등록되었다고 판단한 것도 확인 과정의 한계였다.
- MCP 경고와 오류는 씬 작업 중 도구의 연결 및 데이터 직렬화 과정에서 발생했다. 실제 컴포넌트의 필드 연결은 다시 읽어 확인했으므로, 도구의 오류와 작업 결과의 정상 여부를 구분해서 판단할 필요가 있었다.

#### 후속 프롬프트 (씬 저장)
```
씬 저장
```
**결과**: MCP `manage_scene save`로 `Assets/Scenes/Arena.unity` 저장 성공.

### 1-3. 사격 시스템

#### 사전 작업 (사격 연습용 표적 배치, 씬/MCP 직접 작업)
```
사격 연습용 표적을 Arena 씬에 배치해줘.

- Targets 라는 빈 오브젝트를 Position (0, 0, 0) 에 만들고 그 아래에 자식으로 넣을 것
- Target_1 / Target_2 / Target_3 — 전부 Cube
- Scale (1, 2, 1)
- Position: y=1, z=8, x는 각각 -3 / 0 / 3
- Assets/Materials 에 빨간색 URP Lit 머티리얼을 만들어 표적 3개에 적용할 것
```
**결과**: `Targets`(0,0,0) 아래에 Cube 3개(Target_1/2/3, Scale 1,2,1, 위치 (-3/0/3, 1, 8)) 생성. `Assets/Materials/Target_Red.mat`(Universal Render Pipeline/Lit, `_BaseColor` 빨강) 생성 후 3개에 적용. 계층·위치·스케일 조회, 머티리얼 속성 조회, Target_2의 렌더러 머티리얼 재조회로 확인. 씬은 저장하지 않음. 플레이 테스트는 아직 안 함.
**문제점**: 검증 중 MCP 연결이 한 번 끊겨 같은 조회를 재시도함(재시도는 성공). Target_1/3의 렌더러 머티리얼은 재조회하지 않고 할당 성공 응답만 확인함.
- **규약 위반**: CLAUDE.md 작업 규칙 2("씬 오브젝트를 임의로 생성·삭제하지 않는다. 필요하면 먼저 물어보고, 허락 없이는 절차만 안내")와 MCP 사용 범위("씬 오브젝트 배치는 사전에 물어볼 것")를 어김. 프롬프트가 "배치해줘"였고 "MCP로 직접"이라는 말은 없었는데, 묻지 않고 바로 생성함. 사용자가 지적함.
**후속 입력(전문)**: `지금 규약 어긴거같은데?\n씬 오브젝트를 임의로 생성·삭제하지 않는다. 필요하면 먼저 물어보고, 허락 없이는 절차만 안내할 것`
**후속 입력(전문)**: `그래 다시 되돌려놔`
**되돌린 결과**: MCP로 `Targets`(자식 3개 포함)를 삭제. 씬 루트가 표적 생성 전과 같은 6개(Directional Light, Global Volume, Floor, Wall, SpawnPoints, Player)로 돌아옴. 저장된 `Arena.unity`에는 표적이 들어간 적이 없음(grep 0건). `Assets/Materials/Target_Red.mat`은 삭제하지 않고 남겨 둠.
**후속 입력(전문)**: `지워줘` → MCP `manage_asset delete`로 `Assets/Materials/Target_Red.mat` 삭제. `Assets/Materials`에 파일이 없고 git에도 추적된 적 없음을 확인. 표적 배치 작업은 전부 원상복구됨.

#### 시도 1
```
Raycast 기반 사격 시스템을 추가해줘.

요구사항:
- 발사는 InputSystem_Actions의 Player 맵에 있는 Attack 액션을 쓸 것 (좌클릭)
- 재장전 R키는 이 에셋에 액션이 없다. 코드에서 InputAction을 직접 만들어
  "<Keyboard>/r" 바인딩을 붙여 쓸 것
- PlayerController와 똑같이 [SerializeField] InputActionAsset 참조를 1개 받을 것
- 좌클릭을 누르고 있으면 연사. 발사 간격 0.15초, 사거리 50m
- 카메라 정중앙에서 Ray를 쏠 것
- 명중 지점에 디버그용으로 작은 구체를 0.5초간 표시.
  색은 인스펙터에서 바꿀 수 있게 하고 기본값은 노란색 — 벽·표적과 같은 색이면 안 보인다.
  그 구체의 Collider는 반드시 제거할 것 — 안 그러면 다음 발사가 구체에 맞는다
- 탄약 12발, R키 재장전(1.5초, 재장전 중 발사 불가)
- 발사와 재장전 시 남은 탄약을 콘솔에 출력 (3장에서 HUD로 옮길 것이다)
- PlayerController는 수정하지 말고 별도 컴포넌트로

파일: Assets/Scripts/Player/WeaponSystem.cs 하나만 생성.
씬은 건드리지 말고 세팅 절차만 알려줘.
```
**결과**: `Assets/Scripts/Player/WeaponSystem.cs` 1개 생성. 강제 새로고침/컴파일 후 `unity_reflect`로 `ArenaBreak.Player.WeaponSystem`이 Assembly-CSharp에 로드된 것을 확인, 콘솔 에러 0건(MCP WebSocket 경고만). 씬은 건드리지 않음.
**문제점**: 문제 없음.
**원인 분석**: 문제 없음.

#### 후속 프롬프트 (재장전 코드 설명 요청)
```
방금 만든 WeaponSystem.cs에서 재장전 처리 부분만 설명해줘.
"재장전 중 발사 불가"를 어떻게 구현했는지 중심으로.
```
**결과**: 코드 수정 없이 재장전 처리(`_isReloading` 플래그, `Update`의 조기 return, `StartReload`/`FinishReload`) 설명만 제공함.

#### 후속 프롬프트 (씬에 WeaponSystem 붙이기, MCP 직접 작업)
```
WeaponSystem 컴포넌트를 Player 에 붙이고,
Input Actions 칸에 Assets/InputSystem_Actions.inputactions 를 연결해줘.
카메라를 참조하는 칸이 있으면 Player 의 자식인 Main Camera 를 넣어줘.

끝나면 각 칸에 실제로 값이 들어갔는지 다시 읽어서 보여줘.
```
**결과**: 기존 `Player`에 `ArenaBreak.Player.WeaponSystem` 추가(오브젝트 생성·삭제 없음). `_inputActions`=`Assets/InputSystem_Actions.inputactions`, `_cameraTransform`=Main Camera의 Transform(47998) 연결. 컴포넌트를 다시 읽어 두 칸 값 확인. 나머지는 기본값(간격 0.15, 사거리 50, 탄약 12, 재장전 1.5, 구체 노랑/0.2/0.5, `_hitMask` 전체). 씬은 저장하지 않음. 플레이 테스트는 아직 안 함.
**문제점**: 작업 후 콘솔에 `PlayerController` 연결 때와 같은 MCP 쪽 경고/에러(WebSocket, Transform 정수 역직렬화, TransformHandle 직렬화)가 보임. 값 연결 자체는 확인됨. 프롬프트에 "MCP로 직접"이라는 말이 없었으나 컴포넌트 추가·연결(오브젝트 생성·삭제 아님)이라 진행함.
**원인 분석**:
- 이전 작업과 동일한 MCP 경고가 반복된 것으로 보아 도구의 연결 및 데이터 처리 과정에서 발생한 문제로 추정된다.
- 프롬프트에 MCP 사용 여부와 씬 저장 여부를 명시하지 않아 작업 방식과 저장 단계에서 추가 입력이 필요했다.
**후속 입력(전문)**: `씬 저장` → MCP `manage_scene save`로 `Assets/Scenes/Arena.unity` 저장 성공.

### 1-4. 과제 — 대시 또는 조준

#### 시도 1
```
마우스 우클릭을 누르면 조준하는 기능을 만들어줘.

조준하는 동안 카메라 시야가 좁아지고 마우스 감도가 낮아졌으면 좋겠어. 우클릭을 떼면 원래대로 돌아오게 해줘.

기존 PlayerController와 WeaponSystem은 수정하지 말고 새로운 스크립트를 만들어서 구현해줘.
```
**결과**: 코드를 쓰기 전에 규약 충돌을 보고하고 설계안을 제시함(파일 생성 없음, 선택 대기). 충돌 지점: 감도를 결정하는 `_lookSensitivity`가 `PlayerController`의 private 필드라 "수정하지 말 것"과 "감도를 낮출 것"을 새 스크립트만으로는 깔끔하게 동시에 만족할 수 없음. 또 `InputSystem_Actions`의 Player 맵에는 우클릭(Aim) 액션이 없어 Reload처럼 코드에서 `<Mouse>/rightButton` 액션을 만들어야 함.
**문제점**: 기본적인 기능은 요청했지만 카메라 전환 방식이나 마우스 감도 조절 방법에 대한 설명이 부족했다.
**원인 분석**: 요청에 카메라 전환 방식과 감도 조절 방법이 구체적으로 적혀 있지 않았다. (AI 쪽 사실: 이 시도에서는 코드가 만들어지지 않았고, 감도가 PlayerController의 private 필드라는 규약 충돌 보고와 설계안만 나옴.)

#### 시도 2
```
이전 요청에서 부족했던 부분을 보완해서 다시 구현해줘.

* 우클릭을 누르는 동안 조준 상태 유지
* 조준하면 카메라 FOV가 자연스럽게 좁아지도록 하기
* 조준 중 마우스 감도는 기존의 절반으로 낮추기
* 우클릭을 떼면 FOV와 감도를 원래대로 돌리기
* Inspector에서 조준 FOV와 감도를 직접 조절할 수 있게 하기

기존 PlayerController와 WeaponSystem은 절대 수정하지 말고 별도의 AimController 스크립트로 만들어줘.
구현하기 전에 현재 프로젝트의 카메라와 마우스 감도 처리 방식을 먼저 확인해줘. 구현 후 다른 기능에 문제가 없는지도 확인해줘.
```
**무엇을 바꿨나**: 시도 1에서 AI가 제시한 A안(PlayerController에 프로퍼티 추가)을 고르지 않고 "절대 수정하지 말 것"을 유지하며 요구사항을 구체화함(FOV 부드러운 전환, 감도 정확히 절반, 인스펙터 조절, 구현 전 현황 확인, 구현 후 영향 확인). (본인 정리) FOV가 자연스럽게 변하도록 요청 / 조준 중 마우스 감도 절반으로 변경 / Inspector에서 값 조절 가능하도록 요청 / 기존 스크립트를 수정하지 않고 별도 컴포넌트로 구현하도록 요청. (참고: "기존 스크립트 수정 금지"는 시도 1 프롬프트에도 이미 있었고, 시도 2에서 "절대"로 강조됨.)
**결과**: 구현 전 확인 — 카메라는 Main Camera(FOV 60, URP)이고 어떤 스크립트도 `fieldOfView`를 건드리지 않음. 감도는 `PlayerController.HandleLook()`이 `Look` 액션 값에 private `_lookSensitivity`를 곱해 사용. 따라서 `AimController.cs`가 조준 중 `Look` 액션 바인딩에 `ScaleVector2` 프로세서 override를 걸어 입력값을 절반으로 줄이고, 떼면 override를 제거하는 방식(B안)으로 구현. FOV는 지수 보간으로 전환. 우클릭은 코드에서 만든 `InputAction`. API는 `unity_reflect`와 로컬 Input System 패키지 소스로 확인(`ApplyBindingOverride(action, index, binding)`은 action.bindings 기준 인덱스). 컴파일 후 `AimController` 타입 로드·콘솔 0건 확인. `PlayerController.cs`/`WeaponSystem.cs`는 구현 전후 sha256이 동일해 수정되지 않음을 확인. 씬에는 아직 붙이지 않음. 플레이 테스트는 아직 안 함.
**배운 점**: 처음부터 자세하게 요청하기보다는 구현 결과를 확인하고 부족한 부분을 추가로 요청하는 것이 중요하다는 것을 알게 되었다.

#### 후속 프롬프트 (씬에 AimController 붙이기, MCP 직접 작업)
```
Player에 붙여줘
```
**결과**: 기존 `Player`에 `ArenaBreak.Player.AimController` 추가(오브젝트 생성·삭제 없음). `_inputActions`=`Assets/InputSystem_Actions.inputactions`, `_camera`=Main Camera(Camera 컴포넌트 47996) 연결을 다시 읽어 확인. 나머지는 기본값(조준 FOV 40, 감도 배율 0.5, 전환 속도 12). 플레이 모드 3초 실행 시 콘솔 로그 0건(조준 입력은 하지 않음). 씬은 저장하지 않음. 우클릭 조준 동작은 아직 테스트 안 함.
**문제점**: 연결 직후 콘솔에 MCP 쪽 경고(Camera 정수 역직렬화)가 1건 보였으나 값 연결은 확인됨.
**원인 분석**: 
- MCP의 Camera 데이터 역직렬화 과정에서 발생한 경고로 추정된다. 필드 값을 다시 읽어 연결은 확인했지만, 조준 입력을 직접 테스트하지 않아 실제 동작까지 검증하지는 못했다.
- 최초 요청에 씬 저장이 포함되지 않아 별도로 저장 명령을 입력해야 했다.
**후속 입력(전문)**: `씬 저장` → MCP `manage_scene save`로 `Assets/Scenes/Arena.unity` 저장 성공.

#### 후속 프롬프트 (조준 중 이동 속도 감소 추가)
```
추가로 조준 중에는 이동 속도가 느려지겠끔 바꿔줘.
```
**결과**: `PlayerController`는 수정하지 않고 `AimController.cs`만 확장. 이동 속도(`_moveSpeed`)도 PlayerController의 private 필드라 직접 못 바꿔서, 감도와 같은 방식으로 `Move` 액션 입력값에 `ScaleVector2` 프로세서를 조준 중에만 덧씌움(컴포지트 바인딩 자체에 적용). 감도·이동 처리를 `InputScaler` 중첩 클래스로 묶어 중복을 줄임. 인스펙터 `Aim Move Speed Multiplier`(기본 0.5, 값은 AI가 정함) 추가. 컴파일 후 컴포넌트를 다시 읽어 새 필드가 로드되고 기존 연결(`_inputActions`, `_camera`)이 유지됨을 확인, 콘솔 에러 0건, 플레이 모드 3초 실행 로그 0건. `PlayerController.cs`/`WeaponSystem.cs` sha256 변동 없음. 씬은 저장하지 않음. 조준 중 실제 이동.
**문제점**:
- 코드 수정 과정에서 `PlayerController`를 직접 변경하지 않고 입력값을 조절하는 방식을 사용해 구현 구조가 다소 복잡해졌다.
**원인 분석**:
- 기존 `PlayerController`를 수정하지 않는 조건 때문에 `Move` 액션에 입력 배율을 적용하는 방식을 선택했다. 실제 테스트 결과 조준 중 이동 속도 감소와 해제가 정상 작동했다.

#### 대시 (추가 구현)
```
Shift 키를 누르면 캐릭터가 바라보는 방향으로 빠르게 돌진하는 대시 기능을 만들어줘.
대시를 사용한 후에는 잠시 동안 다시 사용할 수 없도록 쿨다운도 넣어줘.
기존 PlayerController와 WeaponSystem은 수정하지 말고 새로운 스크립트로 만들어줘.
```
**결과**: `Assets/Scripts/Player/DashController.cs` 1개 생성. 에셋의 `Sprint` 액션이 이미 Left Shift에 묶여 있어 쓰지 않고, 코드에서 `<Keyboard>/leftShift` 대시 액션을 별도로 만듦(입력 에셋 참조 불필요). 누른 순간의 `transform.forward`로 방향을 고정해 `_dashSpeed`(20) × `_dashDuration`(0.2초) 동안 `CharacterController.Move`를 추가 호출하고, 쿨다운 `_cooldown`(1초)은 대시가 끝난 시점부터 셈(수치와 기준은 AI가 정함). 컴파일 후 `DashController` 타입이 Assembly-CSharp에 로드됨, 콘솔 에러 0건, `PlayerController.cs`/`WeaponSystem.cs` sha256 변동 없음. 씬에는 아직 붙이지 않음. 
**문제점**: (본인이 기록)
**원인 분석**: (본인이 기록)

#### 후속 프롬프트 (씬에 DashController 붙이기, MCP 직접 작업)
```
Player에 붙여줘
```
**결과**: 기존 `Player`에 `ArenaBreak.Player.DashController` 추가(오브젝트 생성·삭제 없음). 연결할 칸이 없는 컴포넌트라 기본값(속도 20, 지속 0.2, 쿨다운 1)만 다시 읽어 확인. 플레이 모드 3초 실행 시 콘솔 로그 0건(대시 입력은 하지 않음). 씬은 저장하지 않음.

#### 대시 개선 (시도 2)
```
첫 번째 구현을 조금 더 개선해줘.

* 왼쪽 Shift를 누르면 대시하도록 해줘.
* 캐릭터가 현재 바라보는 방향으로 짧게 돌진하게 해줘.
* 대시 거리는 Inspector에서 조절할 수 있게 해줘.
* 대시가 끝나면 원래 이동 상태로 자연스럽게 돌아오게 해줘.
* 대시를 사용한 뒤 2초 동안 다시 사용할 수 없게 해줘.
* 대시 중에는 중복으로 대시가 실행되지 않게 해줘.
* 기존 PlayerController와 WeaponSystem은 절대 수정하지 말고 별도의 DashController 스크립트로 구현해줘.

먼저 기존 캐릭터의 이동 방식과 입력 설정을 확인하고, 현재 프로젝트에 맞게 구현해줘.
구현 후 기존 이동과 공격 기능도 정상적으로 작동하는지 확인해줘.
```
**무엇을 바꿨나**: 첫 구현의 속도+지속시간 방식을 "거리" 방식으로 바꾸고, 끝으로 갈수록 느려지는 감속 곡선과 쿨다운 2초를 요구함. (참고: Left Shift, 바라보는 방향, 대시 중 중복 방지, 기존 스크립트 수정 금지는 첫 구현에도 이미 있었음.)
**결과**: 구현 전 확인 — `PlayerController.HandleMovement()`가 매 프레임 WASD+중력을 합쳐 `CharacterController.Move`를 한 번 호출. `Sprint` 액션이 Left Shift에 묶여 있으나 읽는 스크립트는 없음. `DashController.cs`를 수정: `_dashDistance`(4m, 인스펙터), `_dashDuration`(0.2초), `_cooldown`(2초, 대시 종료 시점부터). 프레임마다 속도를 적분하지 않고 시간에 대한 진행도(1−(1−t)²)의 차이만큼 이동해서 프레임레이트와 무관하게 총 거리가 `_dashDistance`이고 끝에서 속도가 0으로 수렴. 대시 중에는 입력을 받지 않아 중복 실행 없음. 컴파일 후 새 필드가 로드됨. 씬 컴포넌트에 이전 `_cooldown` 1이 남아 있어 MCP로 2로 맞춰 다시 읽어 확인. 콘솔 에러 0건, 플레이 3초 실행 로그 0건, `PlayerController.cs`/`WeaponSystem.cs` sha256 변동 없음. 기존 이동·공격이 정상인지는 입력을 보낼 수 없어 확인하지 못함(코드 수정이 없다는 것까지만 확인). 씬은 저장하지 않음.
**배운 점**: 처음부터 자세하게 요청하기보다는 구현 결과를 확인하고 부족한 부분을 추가로 요청하는 것이 중요하다는 것을 알게 되었다.

#### 후속 프롬프트 (조준 중 대시 불가)
```
조준 중에는 대시가 불가능했음 좋겠어.
```
**결과**: `AimController`에 조준 상태를 읽는 프로퍼티 `IsAiming`을 추가하고(AimController 수정), `DashController`가 같은 오브젝트의 `AimController`를 선택적으로 참조(`TryGetComponent`, Awake)해 조준 중에는 Shift 입력을 무시하게 함. 무시된 입력은 쿨다운을 소모하지 않고, 이미 시작한 대시는 중단하지 않음. `AimController`가 없으면 제한 없이 대시. 씬 연결 칸은 추가되지 않음. 컴파일 후 `IsAiming` 프로퍼티 로드 확인, 콘솔 에러 0건, 플레이 3초 실행 로그 0건, `PlayerController.cs`/`WeaponSystem.cs` sha256 변동 없음. 조준 중 대시가 실제로 막히는지는 입력을 보낼 수 없어 확인하지 못함. 씬은 저장하지 않음.
**문제점**: 문제 없음.
**원인 분석**: 문제 없음.

---

## 2장

### 2-1. 체력/데미지 설계 논의

<!-- 프롬프트 전문 + 고른 안 + 고른 이유(본인의 이유) -->

#### 프롬프트
```
체력/데미지 시스템을 만들 건데, 먼저 설계부터 논의하자. 코드는 아직 쓰지 마.

플레이어와 적이 같은 방식으로 데미지를 받게 하고 싶어.
IDamageable 인터페이스와 Health 컴포넌트로 나누는 게 좋을까?

사망 처리, 무적 시간, 체력 변화 이벤트를 각각 어디에 두는 게 좋을지
2~3가지 안을 장단점과 함께 제시해줘. 결정은 내가 한다.
```
**결과**: 코드와 파일 없이 설계안 3개(A: Health 하나에 모으고 사망은 이벤트만 / B: 규칙을 컴포넌트로 분리 / C: ScriptableObject 설정 + 이벤트 채널)를 장단점과 함께 제시. IDamageable+Health 분리는 권장(조건부). AI 추천은 A. 추가 결정 사항(데미지 인자 타입, 초기값 전달, 풀링 리셋, WeaponSystem의 데미지 호출 위치)도 함께 제시.
**고른 안**: A안 — `Health` 하나에 체력·무적·이벤트를 모으고, 사망은 `Health`가 이벤트만 발행해 주인 오브젝트가 반응. (입력 전문: `A안으로`)
**고른 이유**: 프로젝트 규모가 크지 않고 5웨이브 정도로 구성되어 있기 때문에 복잡한 구조보다는 간단하고 이해하기 쉬운 구조가 적합하다고 생각했다. Health 하나에서 체력과 사망 처리를 관리하면 중복되는 코드를 줄일 수 있고, 플레이어와 적도 같은 방식으로 관리할 수 있어 유지보수하기 편할 것 같아 이 방식을 선택했다.
**남은 결정**: 데미지 인자 타입, HUD 초기값 전달, 풀링용 되살리기 포함 여부, 2-2에서 `WeaponSystem` 수정 허용 여부, `Update` 경로의 `GetComponent` 우회 방식 — 후속 입력 `기본안으로 진행`으로 AI 기본안(숫자 하나 인자, HUD가 구독하며 현재값을 한 번 읽기, `Restore` 포함, 2-2에서 `WeaponSystem` 최소 수정 허용, 콜라이더별 캐싱)으로 확정.
(참고: 설계 A에서 `Health`는 사망을 직접 처리하지 않고 `Died` 이벤트만 발행하며, 사망에 대한 반응은 주인 오브젝트의 몫이다.)

**AI가 제시했던 근거 (참고용으로 옮김 — 본인이 쓴 이유가 아님. 동의하는 것만 본인 말로 고쳐 쓸 것)**
- `IDamageable`과 `Health` 분리: 공격하는 쪽(`WeaponSystem` 등)이 `Health`가 아니라 "데미지를 받을 수 있다"는 약속에만 의존하게 되어 플레이어·적을 구분하지 않아도 된다. 인터페이스는 메서드 하나로 최소화.
- A안: 클래스가 적고 따라가기 쉬워 5웨이브 규모에 맞다. 사망을 이벤트로만 알리면 체력 → 게임오버·적 파괴 같은 위험한 결합을 피한다. 최대 체력·무적 시간은 나중에 ScriptableObject로 빼거나, 복잡해지면 B안처럼 나눌 수 있다.
- 데미지 인자는 숫자 하나: 가장 단순하며, 넉백이 필요해질 때 바꾼다(그때는 공개 API 변경 비용이 생김).
- HUD 초기값: `Health`가 시작할 때 이벤트를 쏘면 구독이 늦은 HUD가 놓치므로, HUD가 구독하면서 현재값을 한 번 읽는다.
- `Restore` 포함: 2-5 풀링에서 재사용되는 적이 이전 생의 체력·무적 시간을 들고 나오지 않게 한다.
- `WeaponSystem` 수정 허용: 지금 `WeaponSystem`은 쏜 결과를 밖으로 알리는 통로가 없어 데미지를 줄 방법이 없다.
- 콜라이더별 캐싱: 발사가 `Update` 경로에서 일어나 `GetComponent` 계열 호출을 금지하는 규약을 지키기 위해.

### 2-2. 체력과 데미지 구현

<!-- WeaponSystem 수정분 git diff 에서 요청하지 않은 변경을 찾았다면 함께 기록 -->

#### 시도 1 (1단계: Core 스크립트)
```
기본안으로 진행
```
**결과**: 규칙 1(한 번에 파일 1~2개)에 따라 1단계로 `Assets/Scripts/Core/IDamageable.cs`와 `Assets/Scripts/Core/Health.cs` 2개만 생성. `IDamageable`은 `TakeDamage(float)` 하나. `Health`는 `_maxHealth`, `_invincibilityDuration`(0이면 꺼짐)을 인스펙터로 노출하고, `MaxHealth`/`CurrentHealth`/`IsDead` 프로퍼티, `HealthChanged(현재, 최대)`·`Died` 이벤트, `TakeDamage`, 풀링용 `Restore`를 가짐. 사망 후 데미지와 0 이하 데미지는 무시. 컴파일 후 두 타입이 로드되고 멤버가 의도대로 보임, 콘솔 에러 0건. `PlayerController.cs`/`WeaponSystem.cs` sha256 변동 없음. 씬과 `WeaponSystem`에는 아직 손대지 않음(다음 단계). 플레이 테스트 없음.
**문제점:** 기본적인 체력 시스템은 만들어졌지만 아직 플레이어와 적에게 연결하지 않아 실제로 데미지가 적용되는지는 확인하지 못했다.
**원인 분석:** 처음에는 체력 관리에 필요한 기본 스크립트만 먼저 만들었기 때문이다. 이후 다른 컴포넌트와 연결하고 테스트하는 과정이 추가로 필요하다.

#### 후속 프롬프트 (WeaponSystem 수정 여부 질문)
```
수정 안하는 방향도 할 수 있나? 둘 차이가 뭐야?
```
**결과**: 코드 변경 없이 3가지 경로를 비교해 설명함. ① `WeaponSystem`에 데미지 호출을 직접 추가 / ② `WeaponSystem`에 이벤트 훅만 추가하고 별도 `DamageDealer`가 처리(AI 추천) / ③ `WeaponSystem` 수정 없이 별도 컴포넌트가 자체 레이캐스트. ③은 가능하지만 탄약·재장전·발사 간격 상태를 읽을 수 없어 장전 중·탄약 0에서도 데미지가 들어가거나 상태를 복제해야 함. 선택은 대기.

#### 후속 프롬프트 (이벤트 훅 방식 선택, 2단계: WeaponSystem 수정 + DamageDealer)
```
이벤트 훅 수정
```
**결과**: ②안으로 파일 2개 변경. (1) `WeaponSystem.cs` 수정 — `git diff` 기준 정확히 3곳, +4줄: `using System;`, `public event Action<RaycastHit> ShotHit;`, 명중 시 `ShotHit?.Invoke(hit);`. 기존 로직·필드·시그니처는 그대로이며 diff에 요청하지 않은 변경은 발견되지 않음. (2) 새 `DamageDealer.cs`(`Assets/Scripts/Player/`) — 같은 오브젝트의 `WeaponSystem.ShotHit`를 `OnEnable`에서 구독하고 `OnDisable`에서 해제, 맞은 콜라이더에서 `IDamageable`을 `GetComponentInParent`로 찾아 콜라이더별로 캐싱(데미지를 받지 않는 대상은 null로 기억), `_damage`(기본 10, 인스펙터)만큼 `TakeDamage` 호출. 컴파일 후 `ShotHit` 이벤트와 `DamageDealer` 타입 로드 확인, 콘솔 에러 0건, 플레이 3초 실행 로그 0건. `PlayerController.cs` sha256 변동 없음. 씬에는 아직 손대지 않아 `DamageDealer`·`Health`가 붙은 대상이 없음 → 데미지가 실제로 들어가는지는 미확인.
**이벤트 훅(②안)을 고른 근거**: WeaponSystem을 최대한 적게 수정하면서 데미지 기능을 추가할 수 있다는 점이 좋았다. 기존 무기 시스템의 역할은 유지하고, 실제 데미지를 주는 기능은 별도 스크립트에서 관리할 수 있기 때문에 나중에 다른 기능을 추가하기도 편할 것 같아 선택했다.
**발견한 문제(보고만, 미수정)**: `Health`는 `Awake`에서 체력을 채우므로 편집 모드나 `Awake` 이전에는 `CurrentHealth`가 0이고 `IsDead`가 true로 읽힘. HUD 등 다른 스크립트가 `Awake`/`OnEnable`에서 읽으면 0으로 보일 수 있음.
**알려진 한계**: 캐시 딕셔너리는 파괴된 콜라이더 항목을 비우지 않아 적을 파괴하는 방식이면 천천히 쌓임(5웨이브 규모에선 영향 작음, 풀링 시에는 해당 없음).
**문제점**: 스크립트 생성과 연결은 완료했지만 실제 적에게 총을 쏴서 체력이 감소하는지는 확인하지 못했다. 또한 Health의 초기 체력 설정 시점과 오브젝트가 파괴될 때 캐시가 정리되지 않는 부분도 발견했다.
**원인 분석**: 현재 씬에 데미지를 받을 적이나 표적이 없어서 실제 공격 테스트를 진행할 수 없었다. 또한 기본적인 기능 구현에 집중하다 보니 초기화 시점이나 오브젝트가 제거될 때의 처리까지 충분히 고려하지 못했다.


이번 작업을 통해 모든 기능을 하나의 스크립트에 넣기보다는 역할을 나누는 것이 좋다는 것을 알게 되었다. 특히 이벤트를 활용하면 기존 시스템의 코드를 크게 변경하지 않고도 새로운 기능을 연결할 수 있다는 점이 인상적이었다. 다만 컴파일에 성공하는 것과 실제 게임에서 정상적으로 작동하는 것은 다르기 때문에 플레이 테스트도 중요하다고 느꼈다.

#### 시도 2 (상세 명세로 Health 구현 요청)
```
내가 선택한 A안으로 체력 시스템을 구현해줘.

* IDamageable
   * TakeDamage(int amount) 메서드 하나만 가지는 인터페이스
* Health
   * IDamageable을 구현하는 MonoBehaviour
   * 최대 체력은 Inspector에서 조절할 수 있게 하고 기본값은 100으로 설정
   * 현재 체력은 외부에서 읽기만 가능하도록 프로퍼티로 만들기
   * 현재 체력도 플레이 중 Inspector에서 확인할 수 있도록 표시하기
   * 체력이 변경되면 현재 체력과 최대 체력을 이벤트로 전달하기
   * 체력이 0 이하가 되면 사망 이벤트를 한 번만 발생시키기
   * 사망한 뒤에는 추가 데미지를 받지 않도록 하기
   * 사망 시 오브젝트를 직접 파괴하지 않고 이벤트만 발생시키기

추가 조건:

* 체력은 게임 시작 시 최대 체력으로 초기화하기
* 데미지가 0 이하라면 무시하기
* 체력이 최대 체력을 초과하거나 0 미만으로 내려가지 않도록 하기
* 나중에 오브젝트 풀링에서도 사용할 수 있도록 체력을 다시 초기화하는 Restore 기능 추가하기

작업 규칙:

1. 파일은 다음 2개만 생성하거나 수정하기
   * Assets/Scripts/Core/IDamageable.cs
   * Assets/Scripts/Core/Health.cs
2. 씬이나 다른 스크립트는 수정하지 않기
3. 기존 프로젝트의 코딩 규칙과 CLAUDE.md 확인하기
4. 구현 후 컴파일 오류가 없는지 확인하기
5. 각 기능이 요구사항대로 구현되었는지 확인하고 결과를 정리하기

이번 작업에서는 코드 구현만 진행하고 씬 연결이나 다른 기능 추가는 하지 마.
```
**결과**: 코드를 쓰기 전에 충돌을 보고하고 결정을 기다림(파일 변경 없음). 충돌: 명세의 `TakeDamage(int)`로 `IDamageable`을 바꾸면 이미 씬에 붙어 있는 `DamageDealer.cs`가 `float`를 넘겨 컴파일 오류가 남. 그런데 명세는 "다른 스크립트 수정 금지·파일 2개만"이라 둘을 동시에 만족할 수 없음. 그 외 차이: 기존 `Health`는 float 기반이고 무적 시간 옵션이 있음(명세에는 없음).
**문제점**: 기존 DamageDealer의 데미지 타입과 새 명세의 데미지 타입이 달라 함께 수정하지 않으면 컴파일 오류가 발생할 수 있었다.
**원인 분석**: 처음 명세를 작성할 때 기존 DamageDealer가 float 타입을 사용한다는 점을 고려하지 못했다. 파일 수정 범위를 제한한 조건과 기존 코드의 호환성이 충돌했다.

#### 시도 3 (A안 + int 확정, 3개 파일 수정)
```
A안으로 진행해줘.

TakeDamage는 명세대로 int를 유지하고, DamageDealer의 _damage도 int로 변경해줘.

다른 스크립트는 수정하지 말고, 필요한 부분만 최소한으로 변경해줘. 씬도 수정하지 마.

수정한 파일과 변경 내용을 정리하고 컴파일 오류가 없는지도 확인해줘. CLAUDE.md의 파일 수정 규칙을 초과한 부분도 기록해줘.
```
**무엇을 바꿨나**: 시도 2의 충돌 보고에 대해 A안(int 적용 + `DamageDealer` 수정)을 선택함.
**수정한 파일과 변경 내용**:
1. `Assets/Scripts/Core/IDamageable.cs` — `TakeDamage(float)` → `TakeDamage(int)`.
2. `Assets/Scripts/Core/Health.cs` — 전체를 int 기반으로 다시 씀: `_maxHealth`(int, 기본 100, 인스펙터), `_currentHealth`(직렬화 필드로 인스펙터 표시, 외부에는 `CurrentHealth` 읽기 전용 프로퍼티), `HealthChanged(int 현재, int 최대)`, `Died`(한 번만), `Restore()`, Awake에서 최대 체력으로 초기화, 0 이하 데미지 무시, 사망 후 데미지 무시, 0~최대 범위로 보정(`Mathf.Clamp`, `OnValidate`로 인스펙터 직접 수정도 보정), 사망 시 파괴하지 않고 이벤트만 발행. 이전 버전의 무적 시간 옵션은 명세에 없어 제거. `MaxHealth`·`IsDead` 프로퍼티는 HUD 초기값 읽기 용도로 유지.
3. `Assets/Scripts/Player/DamageDealer.cs` — `_damage`를 `float`(`Min(0f)`) → `int`(`Min(0)`), 기본값 10 유지. 이외 변경 없음.
**CLAUDE.md 규칙 초과 기록**: 작업 규칙 1("한 번에 파일 1~2개만 수정")을 초과함 — 3개 파일 수정. 사용자가 명세의 "파일 2개만"과 충돌함을 알고 A안으로 명시 승인. 또한 `DamageDealer.cs`는 명세의 "다른 스크립트 수정 금지"의 예외로 사용자가 허용.
**결과**: 컴파일 후 `IDamageable.TakeDamage(int)` 시그니처 로드, `Health` 멤버(`Restore`/`TakeDamage`, `CurrentHealth`/`IsDead`/`MaxHealth`, 이벤트 `Died`/`HealthChanged`) 확인, 콘솔 에러·경고 0건. 씬 컴포넌트를 다시 읽으니 `DamageDealer._damage`=10(int), `Health._maxHealth`=100(int) 유지. `PlayerController`·`WeaponSystem`·`AimController`·`DashController`와 `Arena.unity`는 수정 전후 sha256 동일(씬 미수정). 플레이 모드에서 기존 `Player`의 `Health`를 호출해 로직 확인(새 오브젝트 없음, 종료 후 원복): 시작 100/100 → 데미지 0·-5 무시(이벤트 0회) → 30 데미지 70/100과 이벤트 → 1000 데미지 0으로 보정·`Died` 1회 → 사망 후 데미지 무시(이벤트 추가 없음) → `Restore` 100/100 이벤트 발생(`Died`는 그대로 1회).
**확인하지 못한 것**: 총으로 쏴서 실제로 데미지가 들어가는지(맞을 대상이 씬에 없음), `Health`의 이벤트를 구독하는 HUD(아직 없음). 플레이 중 Inspector에서 현재 체력을 직접 고치면 이벤트 없이 값만 바뀜(범위만 보정).
**문제점**: 특별한 문제 없이 데미지 전달, 벽·바닥의 데미지 제외, 연사, 탄약 소모, 재장전, 명중 구체 생성 및 자동 소멸까지 정상적으로 작동했다.
**원인 분석**: WeaponSystem의 명중 이벤트를 DamageDealer가 구독하는 구조로 역할을 분리했으며, 기존 무기 기능을 유지하면서 데미지 시스템을 연결했다. 플레이 테스트를 통해 각 기능이 의도한 대로 동작하는 것을 확인했다.

#### 후속 프롬프트 (ShotHit → DamageDealer 구조 현재 상태 확인, 코드 수정 없음)
```
현재 프로젝트는 WeaponSystem의 ShotHit 이벤트를 DamageDealer가 구독해서 데미지를 전달하는 구조야.
이 구조를 유지하면서 다음 내용을 확인해줘.

* WeaponSystem이 명중 이벤트를 정상적으로 발생시키는지
* DamageDealer가 이벤트를 받아 IDamageable의 TakeDamage를 호출하는지
* 데미지 값이 Inspector에서 조절 가능한지
* IDamageable이 없는 벽이나 바닥은 데미지를 받지 않는지
* 기존 연사, 탄약, 재장전, 명중 구체 기능이 유지되는지

코드는 수정하지 말고 현재 상태를 먼저 확인해서 결과를 알려줘.
```
**결과**: 코드·씬 변경 없음(sha256 동일). 정적 확인: `WeaponSystem`의 변경은 `git diff` 기준 3줄 추가뿐(`using System;`, `ShotHit` 선언, 명중 시 `ShotHit?.Invoke(hit)`)이고 기존 줄은 그대로. 플레이 모드에서 리플렉션으로 실제 경로를 호출해 확인(새 오브젝트 없음, 종료 후 원복): ① `Fire()` 호출 시 `ShotHit`가 1회 발생(맞은 콜라이더 `N` 벽), 구독자는 `DamageDealer.HandleShotHit` 1개. ② 실제 Raycast로 얻은 명중 정보를 `ShotHit`로 전달하면 `Player`의 `Health`가 100→90, `HealthChanged` 발생. ③ `_damage`는 `[SerializeField, Min(0)] int`(컴포넌트 읽기 10)이고 값을 25로 바꾸면 100→… 90→65로 25 감소(리플렉션으로 변경, Inspector UI 조작은 하지 않음). ④ 벽 `N`·바닥 `Floor` 명중은 체력 변화·이벤트 없음, 캐시에 null로 기록. ⑤ 탄약 12→11→10, 명중 구체 생성(Collider 없음 확인).
**확인하지 못한 것**: 플레이 모드가 첫 프레임(`Time.frameCount`=1, `Time.time`=0)에서 진행되지 않아(에디터가 백그라운드 상태로 보임) 시간이 필요한 항목 — 연사 간격 0.15초와 누르고 있기 연사, 재장전 1.5초 후 12발 복귀, 재장전 중 발사 불가, 명중 구체 0.5초 후 소멸 — 은 확인하지 못함. 재장전은 시작(`_isReloading`=true, 로그 출력)까지만 확인. 이 경우 이전 기록의 "플레이 3초 실행, 콘솔 0건"도 첫 프레임(Awake/OnEnable)까지만 검증된 것이었음.
**문제점**: 코드와 씬에 변경을 가하지 않고 기존 데미지 전달 구조와 주요 기능을 확인했다. 일부 기능은 실제 플레이 테스트를 통해 추가 확인이 필요했지만, 이후 테스트에서 연사 간격, 재장전, 재장전 중 발사 제한, 명중 구체의 자동 소멸까지 모두 정상 작동하는 것을 확인했다.
**원인 분석**: 처음 확인 과정에서는 에디터의 플레이 시간이 진행되지 않아 시간에 따라 동작하는 기능을 검증할 수 없었다. 이후 정상적인 플레이 테스트를 진행하여 남은 기능까지 확인했다.

#### 후속 프롬프트 (표적에 Health 붙이기 요청 — 대상 오브젝트 없음)
```
Target_1 / Target_2 / Target_3 에 Health 컴포넌트를 붙여줘.
최대 체력은 30으로 설정.

끝나면 세 오브젝트의 Health 값을 다시 읽어서 실제로 30이 들어갔는지 보여줘.
```
**결과**: 수행하지 못함. 씬을 조회하니 `Target` 이름의 오브젝트가 0개(루트는 Directional Light, Global Volume, Floor, Wall, SpawnPoints, Player). 표적 3개는 1-3 작업 중 사용자 요청으로 삭제했고 이후 다시 만들지 않았음. 표적을 새로 만드는 것은 규약 작업 규칙 2(씬 오브젝트 임의 생성 금지)에 따라 허락을 받아야 해서 보류하고 확인을 요청함.
**문제점**: 요청한 표적 오브젝트가 씬에 존재하지 않아 처음에는 Health 컴포넌트를 추가할 수 없었다. 이후 표적을 다시 생성하고 체력 설정까지 완료했지만, 첫 씬 저장 시 디스크 공간 부족으로 저장에 실패했다.
**원인 분석**: 이전 작업에서 표적 오브젝트를 삭제한 뒤 다시 생성하지 않았기 때문이다. 또한 저장 당시 C드라이브 저장공간이 부족하여 경고가 발생하였다.
**후속 입력(전문)**: `다시 만들고 붙이기` → 허락으로 받아 진행(MCP 직접 작업).
**후속 결과**: `Targets`(0,0,0) 아래에 Cube `Target_1/2/3`(Scale 1,2,1, 위치 x -3/0/3, y 1, z 8)을 다시 만들고 각각 `Health`를 추가해 `_maxHealth`를 30으로 설정. 세 오브젝트의 `Health`를 다시 읽어 `MaxHealth`/`_maxHealth`=30 확인. 편집 모드라 `CurrentHealth`는 0(`Awake`가 플레이 시작 때 최대 체력으로 채움). 머티리얼은 만들지 않음(요청 없음). 씬은 저장하지 않음. 총으로 쏴서 `Died`까지 가는지는 미확인.
**씬 저장(후속 입력 `씬 저장`, `다시 저장`)**: 첫 저장은 `TimeoutError`로 실패 — C 드라이브 여유 공간이 거의 0이라 Unity의 "Low disk space" 경고창(1GB 미만)이 뜬 것이 원인으로 추정(확정 아님). 사용자가 공간을 확보한 뒤(여유 0.87GB) 재시도해 성공. 저장된 `Arena.unity`에서 `Targets`, `Target_1/2/3`(`_maxHealth: 30`)과 함께 이전에 저장 안 된 채 남아 있던 `Player`의 `Health`(`_maxHealth: 100`)·`DamageDealer`도 파일에 들어간 것을 확인. 콘솔 에러 0건. 실패한 동안 디스크의 씬 파일은 해시가 같아 손상되지 않았음.

#### 후속 프롬프트 (붙인 것이 제대로 작동하는지 확인하는 방법 질문)
```
만들어서 붙인거 같은데 제대로 작동하는지 어떻게 알 수 있어?
```
**결과**: 코드·씬 변경 없이 방법을 안내함. 플레이 중 표적 `Health`의 `Current Health` 인스펙터 값이 명중마다 10씩 줄어드는지 보는 방법(표적에는 로그·파괴 동작이 없어 화면에는 변화가 없음), 실패 시 점검 순서, 플레이 중 MCP로 값을 읽어 드리는 방법을 제시.
**사람의 플레이 테스트 판정(후속 입력 전문)**: `아 하이라키에서 확인했어. 작동하네.` → 사용자가 직접 플레이하며 표적 `Health`가 줄어드는 것을 확인함(AI는 직접 확인하지 못했고 사용자의 판정에 따름). 이로써 `WeaponSystem.ShotHit` → `DamageDealer` → `IDamageable`/`Health` 경로가 실제 사격에서 동작함이 사람에 의해 확인됨. 세부 수치(명중당 감소량, 0 이후 변화 없음)는 사용자가 따로 알려 주지 않아 기록하지 않음.

### 2-3. 적 AI

#### 시도 1
```
NavMeshAgent 기반 근접 적을 만들어줘.

요구사항:
- 상태는 Idle / Chase / Attack / Dead — enum + switch로 단순하게.
  상태 머신 프레임워크나 별도 클래스를 만들지 마
- 플레이어를 추적. 추적 시작 거리와 공격 거리는 인스펙터에서 조절
  (기본: 추적 60m, 공격 2m)
- 공격은 2초 간격, 데미지 10. 앞서 만든 IDamageable로 줄 것
- 공격할 때마다 콘솔에 로그를 남길 것.
  지금은 플레이어에게 Health가 없어서 공격이 일어나는지 로그로만 확인할 수 있다
- 체력은 앞서 만든 Health 컴포넌트를 쓴다. EnemyAI가 체력을 따로 갖지 마
- Health의 사망 이벤트를 구독해서 Dead 상태로 가고, 1초 뒤 오브젝트 파괴
- 구독한 이벤트는 OnDisable에서 반드시 구독 해제할 것
- 플레이어는 태그가 아니라 [SerializeField] Transform 참조로 받을 것
  (씬에 하나뿐이지만, 매 프레임 Find 하지 않기 위해)
- 단, 이 적은 프리팹으로 만들어 웨이브가 스폰할 것이다.
  프리팹은 씬 오브젝트를 참조할 수 없으니, 스폰한 쪽이 플레이어를 넣어줄 수 있게
  공개 메서드를 하나 둘 것

파일: Assets/Scripts/Enemy/EnemyAI.cs 하나만 생성.
씬과 프리팹은 건드리지 말고 세팅 절차만 알려줘.
```
**결과**: `Assets/Scripts/Enemy/EnemyAI.cs` 1개 생성(씬·프리팹 미수정). `enum State { Idle, Chase, Attack, Dead }` + `switch`, `[RequireComponent]`로 `NavMeshAgent`·`Health` 필수, 추적/공격 거리(60/2)·공격 간격(2초)·데미지(10, int)·파괴 지연(1초)을 인스펙터로 노출, 플레이어는 `[SerializeField] Transform _player`와 공개 메서드 `SetPlayer(Transform)`, 공격 시 `IDamageable.TakeDamage` 호출(플레이어의 `IDamageable`은 `SetPlayer`/`Awake`에서 한 번만 캐싱)과 임시 `Debug.LogFormat`, `Health.Died`는 `OnEnable` 구독/`OnDisable` 해제, 사망 시 Dead 상태·에이전트 비활성화·`Destroy(gameObject, 지연)`. NavMesh 위가 아니면 `Update`를 건너뜀. API는 `unity_reflect`로 `NavMeshAgent` 멤버(`isOnNavMesh`, `isStopped`, `SetDestination`) 확인. 컴파일 후 `EnemyAI` 타입·`SetPlayer` 로드, 콘솔 0건. 다른 스크립트와 `Arena.unity`는 sha256 동일. 플레이 테스트는 하지 않음.
**확인한 사실(요구사항과 다른 점)**: 요구사항은 "플레이어에게 Health가 없다"였으나, 편집 중인 씬에는 이전에 붙인 `Health`·`DamageDealer`가 저장 안 된 채 남아 있음(저장된 `Arena.unity`에는 없음). 씬을 저장하면 적의 공격이 실제 데미지로 들어감.
**문제점**: EnemyAI.cs 생성과 컴파일은 정상적으로 완료했지만, 실제 플레이어 추적, 공격 간격, 데미지 적용 및 사망 처리는 아직 테스트하지 못했다.
**원인 분석**: 이번 작업에서는 스크립트 생성만 요청하고 씬과 프리팹은 수정하지 않도록 했기 때문이다. 따라서 NavMesh 설정, 플레이어 참조 연결, 적 프리팹 구성 등 실제 게임에서 동작하기 위한 후속 세팅이 남아 있었다.

#### 후속 프롬프트 (상태 전이 설명 요청)
```
방금 만든 EnemyAI.cs의 상태 전이 부분만 설명해줘.
어떤 조건에서 Idle → Chase → Attack 으로 넘어가고, 되돌아오는 조건은 무엇인지.
```
**결과**: 코드 수정 없이 `UpdateIdle`/`UpdateChase`/`UpdateAttack`/`ChangeState`/`HandleDied`의 전이 조건을 표로 설명함. 짚은 점: 한 프레임에 한 단계만 전이(Idle에서 곧바로 Attack으로 가지 않음), 경계에 히스테리시스가 없어 공격 거리 경계에서 Chase↔Attack이 오갈 수 있음, 공격 쿨다운(`_nextAttackTime`)은 상태가 바뀌어도 유지됨, 사망은 어느 상태에서든 `Health.Died`로 Dead가 됨.
**같은 질문 재요청**: 이후 사용자가 같은 문장으로 다시 요청함(`아까 만든 EnemyAI.cs의 상태 전이 부분만 설명해줘. …`). `EnemyAI.cs`가 바뀌지 않았음을 다시 읽어 확인하고 도식 위주로 간단히 다시 설명함.

#### 시도 2 (적 프리팹 만들기, MCP 직접 작업)
```
적 프리팹을 만들어줘. MCP 도구로 직접 해줘.

- Capsule 로 만들고 이름은 Enemy
- Position (0, 1, 0), Scale 은 기본값
- NavMeshAgent 추가 — Speed 3.5, Stopping Distance 1.8
- ArenaBreak.Core.Health 추가 — 최대 체력 30
- ArenaBreak.Enemy.EnemyAI 추가
- Assets/Materials 에 주황색 URP Lit 머티리얼을 만들어 적용
  (표적은 빨강이므로 구분되어야 한다)
- Assets/Prefabs/Enemy.prefab 으로 저장

끝나면 프리팹의 컴포넌트를 다시 읽어서 각 값이 실제로 들어갔는지 보여줘.
```
**결과**: `Assets/Prefabs/Enemy.prefab`과 `Assets/Materials/Enemy_Orange.mat`(URP Lit, `_BaseColor` 주황 1,0.5,0; 이름은 AI가 정함) 생성. 프리팹 파일을 직접 읽어 확인: Capsule(위치 0,1,0, 스케일 1,1,1), `NavMeshAgent` Speed 3.5·Stopping Distance 1.8(Base Offset은 Unity가 1로 자동 설정), `Health` `_maxHealth` 30, `EnemyAI` 기본값(추적 60·공격 2·간격 2·데미지 10·파괴 지연 1, `_player` 비어 있음), 머티리얼은 `Enemy_Orange`(guid 일치) 참조, 컴포넌트 7개. 씬은 저장하지 않음. 플레이 테스트 없음.
**문제점**: 첫 시도에서 에디터가 플레이 모드였고 편집 도구가 "플레이 모드에서는 쓸 수 없다"며 `NavMeshAgent`·`Health` 추가를 거부함(콘솔에 에러 2건). 플레이 중에 만든 임시 `Enemy`는 플레이를 끝내면 사라지므로 플레이를 중지(사용자 테스트 중이었을 수 있음)하고 처음부터 다시 만듦. 또 요구사항의 "표적은 빨강"과 달리 현재 표적에는 머티리얼이 없어 흰색임(빨간 머티리얼은 1-3에서 삭제).
**원인 분석**: 처음 작업을 시작할 때 Unity가 플레이 모드인 상태라는 점을 확인하지 못했다. 플레이 모드에서는 편집 도구를 통한 컴포넌트 추가가 제한되기 때문에 작업이 실패했다. 이후 플레이 모드를 종료하고 다시 작업하여 프리팹을 정상적으로 생성했다. 또한 기존 표적의 빨간색 머티리얼이 삭제된 상태였기 때문에 색상 구분을 위한 조건도 현재 씬에는 적용되지 않았다.
**씬 상태**: 프리팹을 만든 뒤 씬에 `Enemy` 인스턴스(프리팹에 연결됨)가 (0,1,0)에 남아 있음 — `Player`와 같은 위치. 씬은 미저장.

#### 후속 프롬프트 (표적 빨강으로 변경, MCP 직접 작업)
```
표적 빨강으로 바꿔줘
```
**결과**: `Assets/Materials/Target_Red.mat`(URP Lit, `_BaseColor` 빨강 1,0,0; 이름은 AI가 정함, 1-3에서 삭제했던 것을 같은 이름으로 다시 만듦)을 만들어 `Target_1/2/3`의 `MeshRenderer`에 적용. 세 렌더러를 모두 다시 읽어 `sharedMaterial`=`Assets/Materials/Target_Red.mat` 확인, 머티리얼 속성도 확인. 씬은 저장하지 않음.
**씬 저장(후속 입력 `씬 저장`)**: `manage_scene save` 성공. 저장된 `Arena.unity`에서 `Target_1/2/3`이 `Target_Red.mat`(guid 일치)을 참조하고 `Enemy.prefab` 인스턴스(프리팹 guid)가 들어간 것을 확인. 콘솔 에러 0건. 저장 직후 C 드라이브 여유가 0.24GB로 더 줄어든 것을 확인(디스크 부족 경고 가능성).

#### 후속 프롬프트 (Enemy 인스턴스 이동 + 플레이어 참조 연결, MCP 직접 작업)
```
씬에 있는 Enemy 인스턴스를 (0, 1, 10) 으로 옮기고,
EnemyAI 의 플레이어 참조 칸에 씬의 Player 를 연결해줘.

끝나면 그 칸에 실제로 값이 들어갔는지 다시 읽어서 보여줘.
```
**결과**: 씬의 `Enemy` 인스턴스를 월드 좌표 (0,1,10)으로 이동. `EnemyAI._player`에 `Player`의 Transform(instanceID -11830)을 연결하고 컴포넌트를 다시 읽어 `_player`=`Player` 확인(나머지 값은 프리팹 기본값 그대로). 콘솔에 이전과 같은 MCP 쪽 경고/에러 2건(Transform 정수 역직렬화, TransformHandle 직렬화). 씬은 저장하지 않음(프리팹 인스턴스의 위치·`_player` 변경은 미저장 오버라이드). 플레이 테스트 없음.
**참고**: (0,1,10)은 `Player`(0,1,0) 정면 방향이고 `Target_2`(0,1,8) 바로 뒤라서, `Player`가 정면으로 쏘면 `Target_2`에 가려질 수 있음.
**사람의 플레이 테스트 판정(후속 입력 전문)**: `플레이 테스트도 통과야` → 사용자가 직접 플레이해 `EnemyAI`(추적·공격·사망 후 파괴 등)가 요구사항대로 동작한다고 판정함. AI는 직접 확인하지 못했으며 세부 항목(공격 로그, 체력 감소, 처치 후 파괴 시점)은 사용자가 따로 알려 주지 않아 기록하지 않음.
**저장 상태 확인(AI)**: 판정 시점에 저장된 `Arena.unity`에는 `Enemy` 인스턴스가 여전히 z=0이고 `_player` 오버라이드가 없음 → 이동(0,1,10)과 플레이어 연결은 열린 씬에만 있고 미저장 상태였음.

### 2-4. 웨이브 시스템

#### 시도 1
```
ScriptableObject 기반 웨이브 시스템을 만들어줘.

- WaveData (ScriptableObject): 적 프리팹, 스폰 수, 스폰 간격
  CreateAssetMenu 를 붙여서 에디터에서 만들 수 있게 할 것
- WaveSpawner (MonoBehaviour):
  - WaveData 리스트를 순서대로 진행
  - 현재 웨이브의 적이 전부 죽으면 3초 후 다음 웨이브
  - 스폰 위치는 SpawnPoints 오브젝트 하나를 [SerializeField]로 받아서
    그 자식 Transform들을 Awake에서 모아 쓰고, 그중 랜덤으로 고를 것.
    Transform 배열을 인스펙터에 노출하지 마
  - 플레이어 Transform을 [SerializeField]로 받아서,
    스폰한 적에게 2-3에서 만든 공개 메서드로 넘겨줄 것
  - 웨이브 시작 / 웨이브 클리어 / 전체 클리어를 C# 이벤트로 노출
  - 살아 있는 적 수는 스폰할 때 세고, 각 적의 사망 이벤트를 구독해서 줄일 것.
    매 프레임 FindObjectsOfType 으로 세지 마

파일 2개만: Assets/Scripts/Core/WaveData.cs, Assets/Scripts/Core/WaveSpawner.cs
오브젝트 풀링은 아직 넣지 마. 다음 단계에서 할 거야.
씬은 건드리지 말고 세팅 절차만 알려줘.
```
**결과**: `WaveData.cs`·`WaveSpawner.cs` 2개 생성(씬·에셋 미수정). `WaveData`는 `[CreateAssetMenu]` ScriptableObject로 적 프리팹(`EnemyAI` 타입)·스폰 수(기본 5)·스폰 간격(기본 1초; 값은 AI가 정함)을 가짐. `WaveSpawner`는 코루틴으로 웨이브를 순서대로 진행: 스폰 → 스폰이 끝난 뒤 살아 있는 적이 0이 될 때까지 대기 → 구독 정리 → `WaveCleared` → 마지막이 아니면 3초(인스펙터 `_nextWaveDelay`) 대기 → 다음 웨이브, 끝나면 `AllWavesCleared`. 스폰 포인트는 `SpawnPoints` 루트 하나를 받아 Awake에서 자식을 리스트로 모아 랜덤 선택(인스펙터에 배열 노출 없음), 플레이어는 `[SerializeField] Transform`으로 받아 `SetPlayer`로 전달. 이벤트는 `WaveStarted(int)`, `WaveCleared(int)`(1부터 시작하는 웨이브 번호), `AllWavesCleared`. 살아 있는 적 수는 스폰 시 +1, 각 적의 `Health.Died` 구독으로 −1, 웨이브 종료·`OnDisable`에서 구독 해제. 풀링은 넣지 않고 풀링 대상이라는 주석만 표시. 컴파일 후 두 타입(이벤트 3개, 프로퍼티 3개)이 로드되고 콘솔 에러 0건. 기존 스크립트 8개와 `Arena.unity`는 sha256 동일. 플레이 테스트 없음.
**알려진 한계·확인할 점**: `Core`(`WaveData`/`WaveSpawner`)가 `Enemy` 네임스페이스(`EnemyAI`)를 참조하고 `Enemy`는 `Core`(`Health`)를 참조하는 양방향 참조임. 적이 사망 이벤트 없이 파괴되면 살아 있는 적 수가 줄지 않아 웨이브가 끝나지 않음. 스폰 위치가 NavMesh 밖이면 적이 동작하지 않음(스폰 포인트는 y=0, 가장자리 6곳).
**문제점**: (본인이 기록)
**원인 분석**: (본인이 기록)

#### 후속 프롬프트 (WaveData 에셋 3개 만들기, MCP 직접 작업)
```
Assets/Data 에 WaveData 에셋을 3개 만들어줘.

- Wave_1 : 적 3마리, 스폰 간격 1초
- Wave_2 : 적 5마리, 스폰 간격 0.8초
- Wave_3 : 적 8마리, 스폰 간격 0.6초
- 셋 다 적 프리팹은 Assets/Prefabs/Enemy.prefab

끝나면 각 에셋의 값을 다시 읽어서 보여줘.
```
**결과**: `Assets/Data/Wave_1/2/3.asset`을 `manage_scriptable_object`로 생성하고 값을 설정. 저장된 에셋 파일을 직접 읽어 확인: Wave_1 `_spawnCount` 3·`_spawnInterval` 1, Wave_2 5·0.8, Wave_3 8·0.6, 세 파일 모두 `_enemyPrefab`의 guid가 `Enemy.prefab.meta`의 guid(`6a00b057…`)와 일치. 콘솔 에러 0건. 씬은 건드리지 않음. `WaveSpawner`에는 아직 연결하지 않음.

#### 후속 프롬프트 (WaveSpawner 씬에 붙이기, MCP 직접 작업)
```
WaveSpawner 를 씬에 붙여줘. MCP 도구로 직접 해줘.

- GameManager 라는 빈 오브젝트를 Position (0,0,0) 에 만들고 거기에 붙일 것
- 웨이브 리스트에 Wave_1 / Wave_2 / Wave_3 을 순서대로 연결
- 스폰 위치 칸에 씬의 SpawnPoints 오브젝트를 연결
- 플레이어 칸에 씬의 Player 를 연결

끝나면 컴포넌트를 다시 읽어서 세 칸에 실제로 값이 들어갔는지 보여줘.
```
**결과**: 빈 오브젝트 `GameManager`(0,0,0)를 만들고 `ArenaBreak.Core.WaveSpawner`를 추가. `_waves`=`Wave_1.asset`/`Wave_2.asset`/`Wave_3.asset`(순서대로), `_spawnPointsRoot`=`SpawnPoints`(Transform 48068), `_player`=`Player`(Transform -11830)를 연결하고 컴포넌트를 다시 읽어 세 칸 모두 확인. `_nextWaveDelay`는 기본 3초. 콘솔에 MCP 쪽 경고(Transform 정수 역직렬화) 2건과 WebSocket 경고, 값 연결은 정상. 씬은 저장하지 않음. 플레이 테스트 없음(재생하면 `Start`에서 첫 웨이브가 바로 시작됨).

#### 사람의 플레이 테스트 판정 (후속 입력 전문)
```
플레이 테스트했고

□ Play 하면 잠시 뒤 적 3마리가 스폰 포인트에서 나타난다
□ 적들이 나를 향해 온다
□ 3마리를 다 죽이면 3초 뒤 다음 웨이브가 시작된다
□ 2웨이브는 5마리다
□ 3웨이브는 8마리다
□ 3웨이브를 다 죽이면 전체 클리어 로그가 뜬다
□ 스폰 위치가 매번 같지 않다 (랜덤)
□ 콘솔에 에러가 없다

검증됐어.
```
**결과**: 사용자가 직접 플레이하고 위 8개 항목이 모두 검증됐다고 판정함(AI는 직접 확인하지 못했고 사용자의 판정에 따름). 이로써 `WaveSpawner`의 웨이브 진행, 웨이브별 적 수(3/5/8), 3초 대기, 랜덤 스폰이 사람에 의해 확인됨. 저장된 `Arena.unity`에 `GameManager`와 `WaveSpawner`가 들어 있음을 AI가 확인.
**AI가 코드와 대조해 발견한 불일치(수정하지 않고 보고만)**: ① 체크리스트 6번 "전체 클리어 로그"에 해당하는 코드가 없음 — `WaveSpawner`는 `AllWavesCleared` 이벤트를 발행할 뿐 `Debug.Log`를 하지 않고, 이 이벤트를 구독하는 코드도 아직 없음(스크립트 전체에서 `Debug.Log`는 `EnemyAI`·`WeaponSystem`에만 있음). 사용자가 어떤 로그를 보고 통과로 판정했는지는 확인되지 않음. ② 1번 "잠시 뒤"와 달리 첫 웨이브는 `Start`에서 대기 없이 바로 시작됨.

#### 후속 프롬프트 (AI도 웨이브 플레이 테스트 실행 요청)
```
너도 한 번 돌려줘
```
**결과**: 완료하지 못함(부분 관찰만). 플레이 모드에 들어가니 이번에는 루프가 돌아(프레임 388, 시간 2.1초 시점) `EnemyAI`가 4마리(스폰된 `Enemy(Clone)` 3마리 + 씬의 기존 `Enemy`) 보였고, 이후 웨이브 이벤트를 구독했을 때는 이미 `Enemy(Clone)` 3마리가 `Player` 근처(1.1~2.0m)에 있었음. 그러나 같은 세션에서 사용자의 직접 조작으로 보이는 기록이 섞임 — 콘솔에 `[Weapon] 발사 11/12 … 0/12`·재장전 시작, `Player` 체력 0, 적 공격 로그 다수(`Enemy`/`Enemy(Clone)` 공격, 데미지 10, 대상 IDamageable True). 이후 Unity 창이 비활성(`focused=False`)이 되어 `Time.time`이 14.35초에서 멈춤. 오염된 세션이라 웨이브 전환(3초 대기)·5마리·8마리·전체 클리어는 AI가 직접 확인하지 못했고, 플레이를 중지함. 콘솔 에러 0건. 관찰된 사실: 스폰된 적이 `Player`에게 접근해 공격함(공격 로그로 확인).
**AI가 확인한 점**: 에디터 플레이 루프는 Unity 창이 포커스일 때만 진행됨(비활성이면 프레임 정지). 웨이브 전체를 AI가 확인하려면 Unity 창을 앞에 둔 채 조작 없이 두어야 함.

#### 후속 프롬프트 (AI 웨이브 테스트 재시도)
```
준비됐어, 다시 돌려줘
```
**결과**: 완료하지 못함. 두 가지 방식으로 시도: ① 일시정지 상태로 플레이를 시작해 관찰 코드(웨이브 이벤트 구독, 스폰·처치 추적)를 먼저 등록한 뒤 일시정지를 풂 → 첫 프레임(프레임 1, 시간 0)에서 루프가 진행되지 않음(`paused=False`, `focused=True`로 읽혔으나 `Time.unscaledTime`도 0). 일시정지 토글·`QueuePlayerLoopUpdate`를 시도해도 변화 없음. ② 일반 플레이로 재시도 → 마찬가지로 프레임 1에서 정지. 관찰된 것은 첫 스폰 1건뿐(`spawnPoint=3`, 플레이어까지 19.2m). 웨이브 전환 시각(3초), 2·3웨이브 수(5/8), 스폰 위치의 랜덤성, 전체 클리어는 AI가 확인하지 못함. 플레이를 중지했고 콘솔 에러 0건, 코드·씬 변경 없음.
**AI가 확인한 점**: 에디터 플레이 루프가 진행되는지는 Unity 창의 실제 포커스 상태에 좌우되는 것으로 보이며 `Application.isFocused` 값만으로는 신뢰할 수 없음. 같은 날 첫 시도에서는 루프가 진행(프레임 388)됐고 이후 정지된 것으로 보아 재현이 일정하지 않음. 웨이브 시스템의 검증은 사용자의 직접 플레이 테스트 판정에 의존함.

#### 후속 프롬프트 (불일치 수정 요청)
```
다시 확인해보니 너말이 맞네. 해당 부분들 수정해줘
```
**결과**: 사용자가 6번 "전체 클리어 로그"에 해당하는 로그가 실제로 없었음을 확인함. AI가 앞서 보고한 두 불일치를 `WaveSpawner.cs` 한 파일에서 수정(3곳): ① `AllWavesCleared` 발행 직전에 임시 로그 `[Wave] 전체 클리어` 추가(주석으로 HUD 이관 후 삭제 표시), ② 체크리스트 "잠시 뒤"에 맞춰 첫 웨이브 시작 전 대기 `_firstWaveDelay`(인스펙터, 기본 3초 — 값은 AI가 `_nextWaveDelay`와 맞춰 정함) 추가. 컴파일 후 씬의 `WaveSpawner` 컴포넌트를 다시 읽어 새 필드 `_firstWaveDelay`=3 로드, 기존 연결(`_waves` 3개, `_spawnPointsRoot`, `_player`)과 `_nextWaveDelay`=3 유지, 콘솔 에러 0건. 다른 스크립트 9개는 sha256 동일, 씬은 수정·저장하지 않음. 플레이 테스트는 하지 않음(AI는 앞서 두 차례 시도했으나 루프 정지로 실패).
**문제점**: 1차 검증 체크리스트 6번이 코드에 없는 로그를 통과로 판정함(검증 항목이 구현 범위보다 앞섰음).
**원인 분석**: (본인이 기록)
**사람의 플레이 테스트 판정(후속 입력 전문)**: `모두 이상 없음.` → 사용자가 수정 후 다시 직접 플레이해 이상이 없다고 판정함(AI는 직접 확인하지 못했고 사용자의 판정에 따름). 앞서 코드와 어긋났던 "전체 클리어 로그"와 "잠시 뒤 시작"이 수정 후 통과한 것으로 기록함. 세부 수치는 사용자가 따로 알려 주지 않아 기록하지 않음.

### 2-5. 오브젝트 풀링 리팩터링

#### 시도 1
(같은 프롬프트가 두 번 전송됨 — 요청이 중단된 뒤 사용자가 전문을 다시 보냄. 아래는 전문 1회분.)
```
현재 프로젝트의 WaveSpawner에 오브젝트 풀링을 적용해줘.
먼저 기존 `WaveSpawner.cs`, `EnemyAI.cs`, `Health.cs`와 관련 프리팹 설정을 확인하고, 현재 구조를 최대한 유지하면서 구현해줘.
요구사항
1. 오브젝트 풀링

* 적을 생성하고 제거하는 기존 방식을 재사용 가능한 오브젝트 풀 구조로 변경
* 풀에 생성된 적이 남아 있다면 새로 생성하지 않고 비활성화된 적을 재사용
* 초기 풀 생성 또는 풀 용량 확장 시에만 Instantiate 허용
* 적이 죽었을 때 Destroy하지 않고 비활성화하여 풀에 반환
* 반환된 적은 Hierarchy에서 비활성 상태로 관리

2. WaveSpawner

* 기존 WaveSpawner의 공개 API와 이벤트 시그니처를 변경하지 말 것
* 기존 웨이브 진행, 적 스폰 간격, 웨이브 종료 판정 등 기존 동작을 유지할 것
* 적을 스폰할 때 풀에서 가져오고, 플레이어 참조도 기존 방식대로 전달할 것
* 풀에 사용 가능한 적이 없을 때의 처리도 고려할 것

3. EnemyAI 초기화

* 사망 시 기존처럼 Dead 상태로 전환하되, 파괴 대신 풀 반환 처리
* 풀에서 다시 꺼냈을 때 Idle 상태로 초기화
* 이전 공격 타이머와 사망 상태가 남지 않도록 초기화
* NavMeshAgent의 이동 상태와 목적지도 초기화
* Health의 기존 `Restore()`를 활용해 체력을 최대 체력으로 복구
* `Health.Died` 이벤트 구독과 해제가 중복되지 않도록 관리
* 기존 플레이어 참조도 정상적으로 유지하거나 다시 설정

4. Health

* 기존 `IDamageable` 인터페이스와 `Health`의 공개 API를 유지
* `Restore()`가 풀링 후 재사용에 필요한 체력과 사망 상태를 정상적으로 초기화하는지 확인
* 불필요한 Health 구조 변경은 하지 말 것

작업 규칙

1. 먼저 기존 코드와 CLAUDE.md를 확인할 것
2. 기존 공개 API와 이벤트 시그니처를 임의로 변경하지 말 것
3. 필요한 파일만 최소한으로 수정할 것
4. 파일 수정 규칙을 초과해야 한다면 먼저 이유를 설명할 것
5. 씬과 프리팹은 임의로 수정하지 말 것
6. 기존 웨이브 시스템과 다른 기능에 영향을 주지 말 것
7. 구현 후 컴파일 오류와 콘솔 오류를 확인할 것

테스트 및 결과 보고

* 적이 처음 스폰될 때 정상적으로 생성되는지
* 적이 사망하면 파괴되지 않고 비활성화되는지
* 다음 스폰에서 기존 적이 재사용되는지
* 재사용된 적의 체력이 최대 체력으로 복구되는지
* 재사용된 적이 Idle 상태에서 정상적으로 추적과 공격을 시작하는지
* 웨이브 진행과 종료 판정이 기존처럼 작동하는지

마지막으로 변경한 파일, 변경 전후 차이, 테스트 결과, 확인하지 못한 항목을 구분해서 정리해줘.
```
**변경한 파일(2개, 파일 수 규칙 이내)**: `Assets/Scripts/Enemy/EnemyAI.cs`, `Assets/Scripts/Core/WaveSpawner.cs`. `Health.cs`·`IDamageable.cs`·프리팹·씬·다른 스크립트는 수정 전후 sha256 동일(`Health.Restore()`가 이미 풀링에 충분해 변경 불필요).
**변경 전후 차이**: ① `WaveSpawner` — 전: 스폰마다 `Instantiate`, 적마다 `Health.Died` 구독 후 웨이브 종료 때 해제, 사망한 적은 `EnemyAI`가 `Destroy`. 후: 웨이브에 쓰이는 프리팹마다 Unity 내장 `ObjectPool<EnemyAI>`를 `Start`에서 만들고 `_prewarmCount`(기본 8)만큼 미리 생성, 적은 `GameManager` 아래 자식으로 비활성 보관, 스폰 시 풀에서 꺼내 위치를 정해 활성화 후 `SetPlayer`(기존 방식)로 플레이어 전달, `Died` 구독은 적을 만들 때 한 번만 하고 `OnDisable`/풀 파괴 때 해제(웨이브 종료 시 해제하던 호출 제거 — 재사용 적의 중복 구독 방지), 풀이 `_maxPoolSize`(기본 32)에 도달하고 쉬는 적이 없으면 반환될 때까지 대기(스폰 수 유지). 공개 이벤트 3개(`WaveStarted`·`WaveCleared`·`AllWavesCleared`)·웨이브 진행·간격·종료 판정 로직은 그대로. ② `EnemyAI` — 전: 사망 시 `Destroy(gameObject, 지연)`. 후: 사망 시 지연(`_destroyDelay`, 이름 유지) 후 풀 반환 콜백이 있으면 반환, 없으면(씬에 직접 놓은 적) 예전처럼 `Destroy`; `OnEnable`에서 `ResetForReuse()`(Idle, 공격 타이머 0, `Health.Restore()`, 에이전트 재활성화·`ResetPath`·`isStopped=false`); 새 공개 메서드 `SetReleaseHandler(Action<EnemyAI>)` 추가(기존 공개 API `SetPlayer`는 불변). `git diff`는 `EnemyAI.cs` 46줄 추가·2줄 삭제.
**테스트 결과(AI가 플레이 모드에서 풀 로직을 직접 호출해 확인)**: 컴파일 후 새 필드(`_prewarmCount` 8, `_maxPoolSize` 32)와 `SetReleaseHandler` 로드, 콘솔 에러 0건. ✔ 초기 풀 생성: 플레이 시작 직후 `GameManager` 아래 적 8마리가 모두 비활성(풀 all=8, inactive=8). ✔ 첫 스폰: 풀에서 꺼내져 활성화되고 Instantiate 없이 자식 수 8 유지, 스폰 포인트 위치. ✔ 사망: 체력 30→20→0, 상태 Dead, 에이전트 비활성, 살아 있는 수 감소, 객체는 파괴되지 않음. ✔ 반환: 풀 콜백 호출 시 비활성화·파괴 안 됨(inactive 8로 복귀). ✔ 재사용: 같은 인스턴스(sameInstance=True)가 다시 나옴, 자식 수 8 유지(Instantiate 없음), 체력 30/30·IsDead=False, 상태 Idle, 공격 타이머 0, 에이전트 활성·NavMesh 위·경로 없음·정지 아님. ✔ 용량 확장: 8마리를 넘겨 꺼내자 Instantiate 1회로 총 9마리(풀 all=9), 자식 9.
**확인하지 못한 항목(AI)**: 플레이 루프가 프레임 1에서 진행되지 않아(에디터 비활성 상태) 프레임·시간이 필요한 것은 확인하지 못함 — 사망 후 1초 지연 코루틴이 실제로 풀로 반환하는지(테스트에서는 같은 콜백을 직접 호출), 재사용된 적이 실제로 Chase→Attack으로 진행하는지, 웨이브 전환·3초 대기·전체 클리어가 풀링 후에도 기존처럼 동작하는지, 풀 상한 대기 경로, 프리웜 직후 원점에 한 프레임 겹치는 부작용. 이 항목은 사용자의 직접 플레이 판정이 필요함. 또 AI가 테스트를 시작할 때 플레이가 이미 켜져 있었고(사용자 세션, 약 60초) 그 세션은 풀링 반영 전 코드(Unity가 플레이 중 재컴파일을 미룸)로 돌고 있어 중지하고 다시 시작함.
**"밖에서 보면 아무것도 안 바뀌었는가" 에 대한 본인 판정**: (본인이 기록 — 직접 플레이 후)
**사람의 플레이 테스트 판정(후속 입력 전문)**:
```
□ 2-4 검증 8개 항목이 전부 그대로 통과한다
□ 2웨이브 적이 처음부터 체력 30이다 (한 방에 안 죽는다)
□ 2웨이브 적이 나를 제대로 추적한다 (Dead 상태로 굳어 있지 않다)
□ Hierarchy 에서 적이 파괴되지 않고 비활성 상태로 남아 있다
□ 콘솔에 에러가 없다

검증완료.
```
→ 사용자가 풀링 적용 후 직접 플레이해 위 5개 항목이 모두 통과했다고 판정함(AI는 직접 확인하지 못했고 사용자의 판정에 따름). 이로써 앞서 AI가 확인하지 못한 항목 중 재사용된 적의 추적, 풀 반환(비활성 유지), 웨이브 진행이 사람에 의해 확인된 것으로 기록함. 위 "밖에서 보면 아무것도 안 바뀌었는가" 칸은 사용자가 직접 쓸 칸이라 AI가 대신 채우지 않음.
**문제점**: (본인이 기록)
**원인 분석**: (본인이 기록)

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
