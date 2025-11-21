# CheckResVersion() - Phân Tích 4 Điểm Gọi

## 🎯 Câu hỏi: "4 chỗ gọi CheckResVersion(), đâu là đúng?"

**Đáp án**: CẢ 4 ĐỀU ĐÚNG - mỗi chỗ phục vụ 1 mục đích khác nhau!

---

## 🔄 Flow Đầy Đủ Từ Khi Game Khởi Động

```
┌────────────────────────────────────────────────────────────┐
│                    GAME START FLOW                         │
└────────────────────────────────────────────────────────────┘

1. Unity Engine Start
   │
   ├─→ InitGame.cs:51 - Start()
   │      │
   │      └─→ Line 64: WinUpdate._instance.ShowInit()
   │
2. WinUpdate Setup
   │
   ├─→ WinUpdate.ShowUI() creates WinUpdate instance
   │      │
   │      └─→ WinUpdate.onInit() - Line 128
   │             │
   │             └─→ Registers event listeners
   │
3. CVersionManager Init
   │
   ├─→ CVersionManager.Instance (Singleton creation)
   │      │
   │      └─→ Awake() - Line 235
   │             │
   │             └─→ WinUpdate.OnSplashOver += CheckExtractResource
   │                 (Subscribes to splash screen end event)
   │
4. Splash Screen Animation
   │
   ├─→ WinUpdate.Init() - Line 312
   │      │
   │      └─→ StartCoroutine(SplashAct())
   │             │
   │             └─→ Line 327: OnSplashOver()  ← EVENT TRIGGERED
   │
5. Version Check Triggered
   │
   └─→ CVersionManager.CheckExtractResource()  ← ENTRY POINT
          │
          ├─→ [SCENARIO 1] Line 314: if (GameConst.isEditor)
          │      └─→ CheckResVersion() ✅ CALL #1
          │
          ├─→ [SCENARIO 2] Line 333: if (extracted || AppConst.DebugMode)
          │      └─→ CheckResVersion() ✅ CALL #2
          │
          └─→ [SCENARIO 3] Line 357: After OnExtractResource() complete
                 └─→ CheckResVersion() ✅ CALL #3

[SCENARIO 4] Error Retry Flow (separate):
          │
          └─→ WinUpdate.cs:514 - DelayCheckInternet()
                 └─→ CheckResVersion() ✅ CALL #4
```

---

## 📍 Chi Tiết 4 Điểm Gọi CheckResVersion()

### ✅ CALL #1: Editor Mode Bypass

**Location**: `CVersionManager.cs:314`

**Context**:
```csharp
private void CheckExtractResource()
{
    if (GameConst.isEditor)
    {
        CheckResVersion();  // ← CALL #1
    }
    else
    {
        CheckExtractResource1();
    }
}
```

**Mục đích**:
- Skip extraction process trong Unity Editor
- Developers không cần giải nén files từ APK mỗi lần test

**Khi nào chạy**:
- Chỉ khi `GameConst.isEditor == true`
- Development/Testing mode

**Flow**:
```
Game Start
  → OnSplashOver event
    → CheckExtractResource()
      → isEditor? YES
        → CheckResVersion() [CALL #1]
          → Skip extraction
          → Go directly to version check
```

---

### ✅ CALL #2: Already Extracted

**Location**: `CVersionManager.cs:333`

**Context**:
```csharp
private void CheckExtractResource1()
{
    Debug.LogWarning("##############CheckExtractResource1111111");
    string strVersionFilePath = GameConst.DataPath + "resversion.ver";
    bool extracted = File.Exists(strVersionFilePath);
    if (extracted || AppConst.DebugMode)
    {
        CheckResVersion();  // ← CALL #2
        return;   //文件已经解压过了
    }
    StartCoroutine(OnExtractResource());    //启动释放协成
}
```

**Mục đích**:
- Skip extraction nếu files đã được giải nén trước đó
- Check version immediately khi app không phải lần đầu chạy

**Khi nào chạy**:
- Khi file `resversion.ver` tồn tại (đã extract trước đó)
- HOẶC `AppConst.DebugMode == true`

**Flow**:
```
Game Start (2nd+ launch)
  → OnSplashOver event
    → CheckExtractResource()
      → isEditor? NO
        → CheckExtractResource1()
          → File.Exists("resversion.ver")? YES
            → CheckResVersion() [CALL #2]
              → Skip extraction
              → Go to version check
```

**Evidence**:
```csharp
string strVersionFilePath = GameConst.DataPath + "resversion.ver";
bool extracted = File.Exists(strVersionFilePath);
```

Nếu file này tồn tại = đã extract rồi = không cần extract lại!

---

### ✅ CALL #3: After Extraction Complete

**Location**: `CVersionManager.cs:357`

**Context**:
```csharp
IEnumerator OnExtractResource()
{
    string dataPath = GameConst.DataPath;
    string streamPath = Util.AppContentPath();
    Debug.LogWarning("--------------------解压-------------------------" + GameConst.DataPath);

    // ... extraction code (commented out) ...

    yield return m_wait;
    m_eventVersion.state = EVersionState.ExtracSuccess;
    TriggerVersionProgressEvent();

    // Write resource version file
    UpdateLocalResVersionFile(CurrentBundleVersion.ResVersion);

    //释放完成，开始启动更新资源
    CheckResVersion();  // ← CALL #3
}
```

**Mục đích**:
- Version check SAU KHI extraction hoàn tất
- Normal flow cho first launch

**Khi nào chạy**:
- Lần đầu tiên chạy app (first install)
- Sau khi extract resources từ APK StreamingAssets

**Flow**:
```
Game Start (FIRST launch)
  → OnSplashOver event
    → CheckExtractResource()
      → isEditor? NO
        → CheckExtractResource1()
          → File.Exists("resversion.ver")? NO
            → StartCoroutine(OnExtractResource())
              │
              ├─→ Extract files from StreamingAssets
              ├─→ UpdateLocalResVersionFile()
              │      └─→ Creates "resversion.ver" file
              │
              └─→ CheckResVersion() [CALL #3]
                    → Now check for updates
```

**Sequence**:
1. Extract resources from APK
2. Write `resversion.ver` (marks extraction complete)
3. THEN check version

---

### ✅ CALL #4: Network Error Retry

**Location**: `WinUpdate.cs:514`

**Context**:
```csharp
private IEnumerator DelayCheckInternet()
{
    yield return new WaitForSeconds(1f);
    CVersionManager.Instance.CheckResVersion();  // ← CALL #4
}
```

**Mục đích**:
- Retry version check sau khi có lỗi network
- User clicks "Retry" button

**Khi nào chạy**:
- Khi `CheckInternet()` fail (no internet connection)
- User clicks retry button

**Flow**:
```
Version Check
  → CheckInternet() → FAIL
    → Show "No Internet" dialog
      → User clicks [Retry]
        → DelayCheckInternet()
          → Wait 1 second
            → CheckResVersion() [CALL #4]
              → Retry entire flow
```

**Where it's triggered**:
```csharp
// CVersionManager.cs:641 - CheckResVersion()
if (!CheckInternet())
{
    // No internet handling - shows retry dialog
    return;
}
```

Then in WinUpdate's cancel button handler, it calls `DelayCheckInternet()`.

---

## 🎯 Tóm Tắt: Đâu Là "Đúng"?

### CẢ 4 ĐỀU ĐÚNG!

| Call | Location | Scenario | Purpose |
|------|----------|----------|---------|
| **#1** | CVersionManager:314 | Editor Mode | Skip extraction, direct to version check |
| **#2** | CVersionManager:333 | Already Extracted | 2nd+ launch, files exist, skip extraction |
| **#3** | CVersionManager:357 | After Extraction | 1st launch, after extract complete |
| **#4** | WinUpdate:514 | Network Retry | Retry after connection error |

### Entry Point Chính

**PRIMARY ENTRY POINT**:
```
WinUpdate.OnSplashOver event → CheckExtractResource()
```

Từ đây flow sẽ split thành 3 con đường:
- **Path 1**: Editor Mode → CALL #1
- **Path 2**: Already Extracted → CALL #2
- **Path 3**: First Launch → Extract → CALL #3

**SECONDARY ENTRY POINT**:
```
Network Error → Retry → CALL #4
```

---

## 🔍 Code Evidence

### Entry Point Registration

**File**: `CVersionManager.cs:235`
```csharp
WinUpdate.OnSplashOver += CheckExtractResource;
```

### Event Trigger

**File**: `WinUpdate.cs:315-328`
```csharp
IEnumerator SplashAct()
{
    yield return new WaitForEndOfFrame();

    _splashUI.SetActive(false);

    _version.text = string.Format("{0}.{1}", Application.version,
        CVersionManager.Instance.GetLocalResVersion());
    _version1.text = string.Format("{0}.{1}", Application.version,
        CVersionManager.Instance.GetLocalResVersion());
    _updateUI.SetActive(true);

    if(null != OnSplashOver)
    {
        OnSplashOver();  // ← TRIGGERS CheckExtractResource()
    }
}
```

### CheckExtractResource Decision Tree

**File**: `CVersionManager.cs:309-337`
```csharp
private void CheckExtractResource()
{
    if (GameConst.isEditor)
    {
        CheckResVersion();  // ← CALL #1: Editor bypass
    }
    else
    {
        CheckExtractResource1();
    }
}

private void CheckExtractResource1()
{
    Debug.LogWarning("##############CheckExtractResource1111111");
    string strVersionFilePath = GameConst.DataPath + "resversion.ver";
    bool extracted = File.Exists(strVersionFilePath);

    if (extracted || AppConst.DebugMode)
    {
        CheckResVersion();  // ← CALL #2: Already extracted
        return;
    }

    StartCoroutine(OnExtractResource());  // → Leads to CALL #3
}
```

---

## 📊 Decision Flow Diagram

```
                    ┌─────────────────┐
                    │  Game Startup   │
                    └────────┬────────┘
                             │
                    ┌────────▼────────┐
                    │  OnSplashOver   │
                    │  Event Fired    │
                    └────────┬────────┘
                             │
                ┌────────────▼─────────────┐
                │ CheckExtractResource()   │
                └────────────┬─────────────┘
                             │
                ┌────────────▼──────────────┐
                │  GameConst.isEditor?      │
                └─────┬──────────────┬──────┘
                      │              │
                  YES │              │ NO
                      │              │
          ┌───────────▼───┐   ┌──────▼──────────────┐
          │ CheckResVersion│   │CheckExtractResource1│
          │   [CALL #1]    │   └──────┬──────────────┘
          └───────┬────────┘          │
                  │            ┌──────▼───────┐
                  │            │File.Exists(  │
                  │            │resversion.ver│
                  │            │)?            │
                  │            └──┬────────┬──┘
                  │               │        │
                  │           YES │        │ NO
                  │               │        │
                  │    ┌──────────▼─┐   ┌─▼────────────┐
                  │    │CheckResVer  │   │OnExtractRes  │
                  │    │[CALL #2]    │   │→ Extract     │
                  │    └──────┬──────┘   └─┬────────────┘
                  │           │             │
                  │           │      ┌──────▼──────────┐
                  │           │      │UpdateLocalRes   │
                  │           │      │VersionFile()    │
                  │           │      └──────┬──────────┘
                  │           │             │
                  │           │      ┌──────▼──────────┐
                  │           │      │CheckResVersion  │
                  │           │      │  [CALL #3]      │
                  │           │      └──────┬──────────┘
                  │           │             │
                  └───────────┴─────────────┴──────────┐
                                                       │
                              ┌────────────────────────▼────┐
                              │  CheckInternet()?           │
                              └────────┬────────────────┬───┘
                                       │                │
                                   YES │                │ NO
                                       │                │
                      ┌────────────────▼──┐    ┌────────▼──────┐
                      │RequestReviewVer   │    │Show "No Net"  │
                      │(API call)         │    │Dialog         │
                      └────────────────┬──┘    └────────┬──────┘
                                       │                │
                                       │         ┌──────▼──────────┐
                                       │         │User clicks      │
                                       │         │[Retry]          │
                                       │         └──────┬──────────┘
                                       │                │
                                       │         ┌──────▼──────────┐
                                       │         │DelayCheckInternet│
                                       │         └──────┬──────────┘
                                       │                │
                                       │         ┌──────▼──────────┐
                                       │         │CheckResVersion  │
                                       │         │  [CALL #4]      │
                                       │         └──────┬──────────┘
                                       │                │
                                       └────────────────┴──────────┐
                                                                   │
                                              ┌────────────────────▼────┐
                                              │ Continue Version Check  │
                                              │ Flow...                 │
                                              └─────────────────────────┘
```

---

## 🎪 Real-World Scenarios

### Scenario 1: First Install (Lần đầu cài app)

```
User installs APK
  → Launches app
    → Unity loads
      → InitGame.Start()
        → WinUpdate splash animation
          → OnSplashOver fired
            → CheckExtractResource()
              → isEditor? NO
                → CheckExtractResource1()
                  → resversion.ver exists? NO (first time!)
                    → OnExtractResource()
                      → Extract files from StreamingAssets to DataPath
                      → Create resversion.ver
                      → CheckResVersion() [CALL #3] ✅
                        → Check internet
                          → Call API /apiarr/vestversion (disabled)
                          → Load version.json
                          → Check for updates
```

### Scenario 2: Second Launch (Lần mở thứ 2)

```
User launches app again
  → Unity loads
    → InitGame.Start()
      → WinUpdate splash animation
        → OnSplashOver fired
          → CheckExtractResource()
            → isEditor? NO
              → CheckExtractResource1()
                → resversion.ver exists? YES ✅
                  → CheckResVersion() [CALL #2] ✅
                    → Skip extraction
                    → Check internet
                      → Load version.json
                      → Check for updates
```

### Scenario 3: Editor Development

```
Developer presses Play in Unity Editor
  → InitGame.Start()
    → WinUpdate splash animation
      → OnSplashOver fired
        → CheckExtractResource()
          → isEditor? YES ✅
            → CheckResVersion() [CALL #1] ✅
              → Skip extraction entirely
              → Load version.json from project
              → Check for updates
```

### Scenario 4: Network Error

```
User launches app (no internet)
  → ... normal flow ...
    → CheckResVersion()
      → CheckInternet() → FAIL ❌
        → Show "No Internet Connection" dialog
          → User clicks [Retry] button
            → DelayCheckInternet()
              → Wait 1 second
                → CheckResVersion() [CALL #4] ✅
                  → Retry internet check
                  → If success, continue version check
```

---

## 🔑 Kết Luận

### Câu trả lời cho "đâu mới là đúng?"

**TẤT CẢ 4 CALLS ĐỀU ĐÚNG** - không có call nào "sai":

1. **CALL #1** (line 314): Đúng cho Editor mode
2. **CALL #2** (line 333): Đúng cho subsequent launches
3. **CALL #3** (line 357): Đúng cho first launch
4. **CALL #4** (WinUpdate:514): Đúng cho retry flow

### Entry Point Chính Thức

**PRIMARY**: `CheckExtractResource()` được trigger bởi `WinUpdate.OnSplashOver` event

**Flow Path**:
```
OnSplashOver
  → CheckExtractResource()
    → CheckResVersion() (via 1 of 3 paths)
```

**SECONDARY**: `DelayCheckInternet()` cho retry mechanism

### Design Pattern

Đây là **State-Based Branching Pattern**:
- 1 entry point (`CheckExtractResource`)
- Multiple exit paths based on state:
  - Editor state → CALL #1
  - Extracted state → CALL #2
  - Not extracted state → Extract → CALL #3
- Error recovery path → CALL #4

Tất cả đều converge về cùng 1 function: `CheckResVersion()`

---

## 📖 Related Documentation

Để hiểu CheckResVersion() làm gì sau khi được gọi, xem:
- `APK_Update_How_It_Really_Works.md` - Chi tiết flow trong CheckResVersion()
- `VersionPanel_Documentation.md` - UI handling cho update dialogs
- `APK_Update_Implementation_Guide.md` - Complete implementation guide
