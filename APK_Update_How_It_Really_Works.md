# APK Update - Cách Hệ Thống THỰC SỰ Hoạt Động

## 🔍 Phát hiện quan trọng về resVer vs resVersion

Bạn hỏi đúng! Có 2 fields trong version.json:

```json
{
    "resVer": "1001",       // ← Field 1
    "resVersion": "1001",   // ← Field 2
}
```

## 📊 Phân tích sử dụng của từng field

### 1. `resVer` - ĐANG ĐƯỢC DÙNG

**Được parse ở**: `HotStart.cs:90`

```csharp
JSONNode node = JSON.Parse(text);
m_urlResVersion = node["resVer"].AsInt;  // ← Parse "resVer"
```

**Mục đích**: Resource version từ SERVER (cho AssetBundle update)

**Sử dụng**: So sánh với local version (code bị comment dòng 207):

```csharp
// if (m_urlResVersion > resVersion)
// {
//     resVersion = m_urlResVersion;
//     StartCoroutine(ResDown());  // AssetBundle update
// }
```

**Kết luận**: `resVer` dùng cho **ASSETBUNDLE UPDATE**

---

### 2. `resVersion` - KHÔNG ĐƯỢC DÙNG! 🚨

**Tìm kiếm trong code**: Không có nơi nào parse field này từ JSON!

```csharp
// KHÔNG TỒN TẠI:
// node["resVersion"]
```

**Được generate ở**: `Packager.cs:1524-1525`

```csharp
versionJson["resVer"].AsInt = AppConst.ResVersion;
versionJson["resVersion"].AsInt = AppConst.ResVersion;  // ← Duplicate!
```

**Kết luận**: `resVersion` là **DUPLICATE** của `resVer` - KHÔNG ĐƯỢC DÙNG trong runtime!

---

## 🔍 Tìm field thực sự cho APK Update

### Trong CVersionManager.cs có 2 variables quan trọng:

```csharp
public int _reviewVerion;           // Line 1336 - Resource version
private string _reviewMinVerion;    // Line 1337 - MINIMUM APP VERSION (cho APK check)
```

### Flow thực sự (khi enable):

#### Bước 1: RequestReviewVersion() gọi API

**File**: `CVersionManager.cs:675-719`

```csharp
private IEnumerator RequestReviewVersion()
{
    yield break;  // ← DISABLED!

    /* CODE BỊ COMMENT:

    // Gọi API
    var finalUrl = ClientSetting.Instance.WebDonmain() + "/apiarr/vestversion";

    // Request body (encrypted):
    // "packageName|appVersion|zone|deviceId|vpnFlag|SHARE"

    UnityEngine.Networking.UnityWebRequest www =
        UnityEngine.Networking.UnityWebRequest.Post(finalUrl, wWForm);
    yield return www.SendWebRequest();

    if (null != www.error) {
        // Retry 3 times
        if (_tryCount-- > 0) {
            StartCoroutine(RequestReviewVersion());
        }
        yield break;
    }

    // ⚠️ ĐIỂM QUAN TRỌNG:
    Debug.Log("www.text: " + www.downloadHandler.text);
    var str = DecryptDES(www.downloadHandler.text);  // ← DECRYPT

    RequestReviewVersionSuccess();  // ← NHƯNG KHÔNG PARSE!
    */
}
```

**🚨 VẤN ĐỀ**: Response được decrypt nhưng **KHÔNG PARSE** để lấy giá trị!

#### Bước 2: Parse response (BỊ THIẾU!)

Code THIẾU đoạn này sau dòng 717:

```csharp
var str = DecryptDES(www.downloadHandler.text);

// ⚠️ THIẾU CODE NÀY:
// JSONNode responseNode = JSON.Parse(str);
// _reviewVerion = responseNode["reviewVersion"].AsInt;
// _reviewMinVerion = responseNode["minVersion"].Value;
// _cdnUrl = responseNode["cdnUrl"].Value;
// _serverUrl = responseNode["serverUrl"].Value;
// isArraign = responseNode["isReview"].AsInt;

RequestReviewVersionSuccess();
```

#### Bước 3: LoadResVersionFile() check version

**File**: `CVersionManager.cs:746-771`

```csharp
private IEnumerator LoadResVersionFile()
{
    m_urlResVersion = _reviewVerion;  // ← Từ API response

    // CHECK APK VERSION:
    if (Application.version != _reviewMinVerion)  // ← Từ API response
    {
        // Trigger APK Update
        m_eventVersion.state = EVersionState.ApkUpdate;
        m_eventVersion.info = AppConst.PackName;
        TriggerVersionProgressEvent();
    }
    else
    {
        // CHECK RESOURCE VERSION:
        if (_reviewVerion > m_localResVersion)
        {
            DownloadResNewVersionPackage();  // AssetBundle update
        }
        else
        {
            CheckResVersionNone();  // No update
        }
    }

    yield break;
}
```

---

## 🎯 API Response Format (Expected)

### Request

**URL**: `POST /apiarr/vestversion`

**Body**:
```
versionname: <encrypted_string>
```

**Decrypted string**:
```
com.slotclassic.bigwin|1.0.4|America/Rio_Branco|device123abc|0|SHARE
```

Format: `packageName|appVersion|timezone|deviceId|vpnFlag|SHARE`

### Response (Encrypted với DES)

**Sau khi decrypt**, response nên là JSON:

```json
{
    "reviewVersion": 1001,
    "minVersion": "1.0.5",
    "cdnUrl": "https://a.lywl2025.com/tga4/",
    "serverUrl": "https://gameserver.com/",
    "isReview": 0,
    "showAB": 0
}
```

**Giải thích các fields**:

| Field | Type | Mục đích |
|-------|------|----------|
| `reviewVersion` | int | Resource version (cho AssetBundle update) |
| `minVersion` | string | **Minimum App Version** (cho APK update) |
| `cdnUrl` | string | CDN URL cho download resources |
| `serverUrl` | string | Game server URL |
| `isReview` | int | Review mode flag (1 = đang review, skip update) |
| `showAB` | int | Show AB test flag |

---

## 🔄 Flow đầy đủ (khi enable)

```
Game Start
    │
    ├─→ CVersionManager.CheckResVersion()
    │      │
    │      └─→ RequestReviewVersion()
    │             │
    │             ├─→ POST /apiarr/vestversion
    │             │      Request: "com.game|1.0.4|zone|device|0|SHARE"
    │             │
    │             ├─→ Response (encrypted):
    │             │      "{reviewVersion:1001,minVersion:'1.0.5',...}"
    │             │
    │             ├─→ Decrypt response
    │             │
    │             ├─→ Parse JSON (⚠️ BỊ THIẾU!)
    │             │      _reviewVerion = 1001
    │             │      _reviewMinVerion = "1.0.5"
    │             │
    │             └─→ RequestReviewVersionSuccess()
    │
    ├─→ LoadResVersionFile()
    │      │
    │      ├─→ CHECK 1: APK Version
    │      │      if (Application.version != _reviewMinVerion)
    │      │         "1.0.4" != "1.0.5" → TRUE
    │      │         → Trigger EVersionState.ApkUpdate
    │      │         → Stop (không check resource)
    │      │
    │      └─→ CHECK 2: Resource Version (nếu APP version OK)
    │             if (_reviewVerion > m_localResVersion)
    │                1001 > 1000 → TRUE
    │                → DownloadResNewVersionPackage()
    │
    └─→ WinUpdate.OnUpdateResEvent()
           │
           └─→ case EVersionState.ApkUpdate:
                  │
                  ├─→ apkUrl = AppConst.PackName
                  ├─→ Show popup
                  │      [OK] → Application.OpenURL(apkUrl)
                  │      [Cancel] → Application.Quit()
                  │
                  └─→ END
```

---

## 📍 Vấn đề với version.json hiện tại

### version.json của bạn:

```json
{
    "resVer": "1001",
    "resVersion": "1001",
    "ip": "54.46.95.179",
    "port": "8200|8201|8202|8203|8204|8205",
    "backstage": "https://rechargeva4.lywl2025.com",
    "customer": "ta4.lywl2025.com",
    "CdnUrl": "https://a.lywl2025.com/tga4/",
    "isShow": "1",
    "isShowks": "1"
}
```

### ❌ Thiếu hoàn toàn mechanism cho APK Update!

**Lý do**:
1. `version.json` KHÔNG PHẢI là nơi check APK version
2. APK version check được thực hiện qua **API `/apiarr/vestversion`**
3. API này BỊ DISABLE (`yield break` dòng 677)

### ✅ Để APK Update hoạt động:

**Option 1: Enable API** (Design gốc)
- Bỏ `yield break` trong RequestReviewVersion()
- Thêm code parse JSON response
- Setup backend API `/apiarr/vestversion`

**Option 2: Dùng version.json** (Hack đơn giản)
- Thêm field `minAppVersion` vào version.json
- Parse nó trong HotStart.cs
- Bypass API check

---

## 🧩 Giải thích tại sao có 2 fields giống nhau

### Code generate version.json (Packager.cs:1524-1525)

```csharp
JSONNode versionJson = new JSONClass();

versionJson["resVer"].AsInt = AppConst.ResVersion;
versionJson["resVersion"].AsInt = AppConst.ResVersion;  // Duplicate!
```

**Lý do có thể**:
1. **Legacy code**: `resVersion` là field cũ, `resVer` là field mới
2. **Backward compatibility**: Giữ cả 2 để support client cũ
3. **Mistake**: Dev duplicate nhầm và quên xóa

**Thực tế**: Chỉ `resVer` được dùng, `resVersion` bị ignore!

---

## 🎯 Kết luận

### Hệ thống thiết kế ban đầu:

```
┌─────────────────────────────────────────────────┐
│          VERSION CHECK FLOW                      │
├─────────────────────────────────────────────────┤
│                                                  │
│  1. Load version.json                            │
│     └─→ resVer: 1001 (AssetBundle version)      │
│                                                  │
│  2. Call API /apiarr/vestversion                 │
│     └─→ Response:                                │
│         ├─ reviewVersion: 1001 (Resource)        │
│         └─ minVersion: "1.0.5" (APK)             │
│                                                  │
│  3. Check APP version                            │
│     if (Application.version != minVersion)       │
│        → Trigger APK Update                      │
│                                                  │
│  4. Check Resource version                       │
│     if (reviewVersion > localVersion)            │
│        → Trigger AssetBundle Update              │
│                                                  │
└─────────────────────────────────────────────────┘
```

### Trạng thái hiện tại:

```
┌─────────────────────────────────────────────────┐
│          CURRENT STATE (BROKEN)                  │
├─────────────────────────────────────────────────┤
│                                                  │
│  1. Load version.json ✅                         │
│     └─→ resVer: parsed                           │
│                                                  │
│  2. Call API ❌ DISABLED                         │
│     └─→ yield break;                             │
│                                                  │
│  3. Parse response ❌ MISSING                    │
│     └─→ No code to parse JSON                    │
│                                                  │
│  4. Check APP version ❌ NEVER RUNS              │
│     └─→ _reviewMinVerion = null/empty            │
│                                                  │
│  5. AssetBundle Update ❌ COMMENTED              │
│     └─→ if (m_urlResVersion > resVersion) //    │
│                                                  │
└─────────────────────────────────────────────────┘
```

---

## 📝 Câu trả lời cho câu hỏi của bạn

### "resVer và resVersion phải khác nhau gì đó chứ?"

**Đáp án**:
- `resVer` và `resVersion` **KHÔNG KHÁC NHAU** về giá trị
- Cả 2 đều được set = `AppConst.ResVersion` (1000)
- Chỉ `resVer` được parse và dùng
- `resVersion` là **DEAD CODE** - bị bỏ quên

**Evidence**:
```csharp
// Generate (Packager.cs:1524-1525)
versionJson["resVer"].AsInt = AppConst.ResVersion;      // 1000
versionJson["resVersion"].AsInt = AppConst.ResVersion;  // 1000 (giống hệt)

// Parse (HotStart.cs:90)
m_urlResVersion = node["resVer"].AsInt;  // ← Chỉ parse "resVer"
// node["resVersion"] KHÔNG BAO GIỜ được parse!
```

---

## 🛠️ Để enable APK Update theo DESIGN GỐC

### Bước 1: Enable RequestReviewVersion()

**File**: `CVersionManager.cs:675-719`

**Sửa**:
```csharp
private IEnumerator RequestReviewVersion()
{
    // yield break;  // ← XÓA DÒNG NÀY

    var finalUrl = ClientSetting.Instance.WebDonmain() + "/apiarr/vestversion";
    // ... existing code ...

    var str = DecryptDES(www.downloadHandler.text);

    // ========== THÊM CODE PARSE ==========
    try
    {
        JSONNode responseNode = JSON.Parse(str);
        _reviewVerion = responseNode["reviewVersion"].AsInt;
        _reviewMinVerion = responseNode["minVersion"].Value;
        _cdnUrl = responseNode["cdnUrl"].Value;
        _serverUrl = responseNode["serverUrl"].Value;
        isArraign = responseNode["isReview"].AsInt;
        ShowAb = responseNode["showAB"].AsInt == 1;

        Debug.Log($"[APK Check] minVersion from API: {_reviewMinVerion}");
    }
    catch (System.Exception ex)
    {
        Debug.LogError($"Parse response error: {ex.Message}");
    }
    // ========== KẾT THÚC ==========

    RequestReviewVersionSuccess();
}
```

### Bước 2: Setup Backend API

**Endpoint**: `POST /apiarr/vestversion`

**Response format** (sau encrypt):
```json
{
    "reviewVersion": 1001,
    "minVersion": "1.0.5",
    "cdnUrl": "https://a.lywl2025.com/tga4/",
    "serverUrl": "https://your-game-server.com/",
    "isReview": 0,
    "showAB": 0
}
```

### Bước 3: Xong!

Khi enable xong, flow sẽ hoạt động:

```
RequestReviewVersion()
  → API response with minVersion
    → LoadResVersionFile()
      → Application.version != _reviewMinVerion?
        → YES: Show APK Update dialog
        → NO: Check resource version
```

---

## 🔍 Tóm tắt

1. **`resVer`**: Dùng cho AssetBundle update (code bị comment)
2. **`resVersion`**: KHÔNG được dùng (dead code)
3. **`minVersion`**: Từ API response - dùng cho APK update (bị disable)
4. **version.json**: KHÔNG DÙNG cho APK check - chỉ dùng cho resource
5. **APK Update**: Phụ thuộc hoàn toàn vào API `/apiarr/vestversion`

**Vấn đề lớn nhất**: API bị disable + thiếu code parse response → APK Update không thể hoạt động!
