# APK Update - Phân tích chuyên sâu

## 📱 Tổng quan APK Update System

APK Update là cơ chế bắt buộc người chơi phải tải và cài đặt bản APK mới khi có thay đổi lớn về:
- Native code (Java/Kotlin/Objective-C)
- Unity Engine version
- Core framework changes
- Application version (major/minor updates)

---

## 🔄 Luồng hoạt động chi tiết

### Flow Chart

```
Game Start
    │
    ├─→ HotStart.cs: loadVersion()
    │      │
    │      └─→ Load version.json từ server
    │             ├─ minAppVersion: "1.0.5"
    │             └─ (other configs...)
    │
    ├─→ So sánh Application.version với minAppVersion
    │      │
    │      ├─ MATCH (1.0.5 == 1.0.5) ✓
    │      │    └─→ Continue normal flow
    │      │
    │      └─ MISMATCH (1.0.4 != 1.0.5) ✗
    │           └─→ Trigger APK Update Flow
    │
    └─→ APK Update Flow:
           │
           ├─→ CVersionManager.cs
           │      │
           │      └─→ LoadResVersionFile() (line 746-771)
           │             │
           │             └─→ if (Application.version != _reviewMinVerion)
           │                    │
           │                    └─→ Trigger EVersionState.ApkUpdate
           │                           │
           │                           ├─ m_eventVersion.state = EVersionState.ApkUpdate
           │                           ├─ m_eventVersion.info = AppConst.PackName
           │                           └─ TriggerVersionProgressEvent()
           │
           ├─→ WinUpdate.cs
           │      │
           │      └─→ OnUpdateResEvent() (line 366-479)
           │             │
           │             └─→ case EVersionState.ApkUpdate: (line 398-409)
           │                    │
           │                    ├─ apkUrl = data.info (AppConst.PackName)
           │                    ├─ Show TipPanel với 2 buttons:
           │                    │    ├─ [OK] → Open APK URL
           │                    │    └─ [Cancel] → Quit game
           │                    └─ _okBtn.gameObject.SetActive(true)
           │
           └─→ User Action
                  │
                  ├─ Click [OK] → OnOkBtnClick() (line 487-498)
                  │    │
                  │    └─→ Application.OpenURL(apkUrl)
                  │           │
                  │           └─→ Mở browser/download manager
                  │                  │
                  │                  └─→ Download APK
                  │
                  └─ Click [Cancel] → OnCancleBtnClick() (line 500-503)
                       │
                       └─→ Application.Quit()
```

---

## 📂 Các file liên quan

### 1. CVersionManager.cs

**Đường dẫn**: `ClientHW2024V11/GameMain/SenSrc/Version/CVersionManager.cs`

#### Phần quan trọng nhất

##### a. Định nghĩa URL pattern (dòng 152)

```csharp
private const string ApkUrlFile = "update/pak/{AppOS}/{AppName}/{AppVersion}/apkUrl.txt";
```

**Giải thích**:
- `{AppOS}`: Android/iOS/Windows
- `{AppName}`: Package name (vd: com.slotclassic.bigwin)
- `{AppVersion}`: App version (vd: 1.0.4)

**Ví dụ URL thực tế**:
```
https://cdn.yourgame.com/update/pak/Android/com.slotclassic.bigwin/1.0.4/apkUrl.txt
```

##### b. Replace placeholders (dòng 155-164)

```csharp
private string ReplaceEscapeCharacter(string strValue)
{
    string strValueNew = strValue;

    strValueNew = strValueNew.Replace(PRE_STRING_APPNAME, Application.identifier);
    strValueNew = strValueNew.Replace(PRE_STRING_APPOS, Util.GetOS());
    strValueNew = strValueNew.Replace(PRE_STRING_APPVERSION, GetAppVersion());

    return strValueNew;
}
```

**Chức năng**: Thay thế placeholders động
- `{AppName}` → `Application.identifier` (vd: com.slotclassic.bigwin)
- `{AppOS}` → `Util.GetOS()` (Android/iOS/Windows)
- `{AppVersion}` → `GetAppVersion()` (vd: 1.0.4)

##### c. Version comparison logic (dòng 746-771)

```csharp
private IEnumerator LoadResVersionFile()
{
    m_urlResVersion = _reviewVerion;

    // ĐIỂM QUAN TRỌNG: So sánh Application.version với minVersion từ server
    if (Application.version != _reviewMinVerion)
    {
        // Trigger APK Update
        m_eventVersion.state = EVersionState.ApkUpdate;
        m_eventVersion.info = AppConst.PackName;  // ← APK download URL
        TriggerVersionProgressEvent();
    }
    else
    {
        // Version match → Continue với resource update
        if (_reviewVerion > m_localResVersion)
        {
            DownloadResNewVersionPackage();
        }
        else
        {
            CheckResVersionNone();
        }
    }

    yield break;
}
```

**Quan sát quan trọng**:

1. **So sánh STRING, không phải số**:
   ```csharp
   if (Application.version != _reviewMinVerion)
   ```
   - `Application.version`: "1.0.4"
   - `_reviewMinVerion`: "1.0.5"
   - So sánh exact match, không parse version numbers

2. **APK URL source**:
   ```csharp
   m_eventVersion.info = AppConst.PackName;
   ```
   - Không load từ `apkUrl.txt`
   - Lấy trực tiếp từ `AppConst.PackName` (hardcoded)

##### d. Variables tracking (dòng 1336-1339)

```csharp
public int _reviewVerion;           // Resource version từ server
private string _reviewMinVerion;    // Minimum App version từ server
public string _serverUrl;           // Game server URL
public string _cdnUrl;              // CDN URL
```

---

### 2. WinUpdate.cs

**Đường dẫn**: `ClientHW2024V11/GameMain/SenSrc/WinUpdate.cs`

#### Hiển thị popup và xử lý user action

##### a. Biến lưu APK URL (dòng 38)

```csharp
string apkUrl = string.Empty;
```

##### b. Nhận event từ CVersionManager (dòng 398-409)

```csharp
case EVersionState.ApkUpdate:   // 发现大版本
    _proBar.gameObject.SetActive(false);  // Ẩn progress bar
    _txtTip.text = string.Empty;           // Clear tip text

    apkUrl = data.info;  // ← LƯU APK URL (từ AppConst.PackName)

    // Hiển thị 2 buttons
    _okBtn.gameObject.SetActive(true);      // "OK" button
    _cancleBtn.gameObject.SetActive(true);  // "Cancel" button
    _sureBtn.gameObject.SetActive(false);   // Ẩn "Sure" button

    // Hiển thị popup (bị comment)
    // ShowTipPanel(MyLoc.Get("CS.WinUpdate.226.0"));
    // Tạm dịch: "发现大版本更新,是否前往链接!"
    // "Phát hiện phiên bản lớn, có muốn cập nhật không?"
    break;
```

##### c. User clicks OK (dòng 487-498)

```csharp
private void OnOkBtnClick()
{
    if (string.IsNullOrEmpty(apkUrl))
    {
        Application.Quit();  // Không có URL → Thoát game
    }
    else
    {
        Util.Log("apk更新地址： " + apkUrl);
        Application.OpenURL(apkUrl);  // Mở URL download APK
    }
}
```

**Hành vi**:
- Nếu có `apkUrl` → Mở browser với URL
- Nếu không có URL → Thoát game luôn

##### d. User clicks Cancel (dòng 500-503)

```csharp
private void OnCancleBtnClick()
{
    Application.Quit();  // Thoát game ngay
}
```

**Kết quả**: Không có cách nào để vào game với version cũ!

---

### 3. AppConst.cs

**Đường dẫn**: `ClientHW2024V11/GameMain/SenSrc/AppConst.cs`

#### APK URL được hardcode (dòng 33)

```csharp
public static string PackName = "https://play.google.com/store/apps/details?id=com.slotclassic.bigwin";
```

**Quan sát**:
- Đây là Google Play Store link
- Không phải direct APK download
- Hardcoded trong code

#### Các constants khác

```csharp
public const string CdnUrl = "https://a.lywl2025.com/tga2/";
public const string customer = "ta1.lywl2025.com";
public const string backstage = "https://rechargeva1.lywl2025.com";
```

---

### 4. VersionPanel.cs

**Đường dẫn**: `ClientHW2024V11/HotUpdate/UI/UI/Common/View/VersionPanel.cs`

#### UI Component cho APK Update

##### a. Initialize với 2 modes (dòng 22-35)

```csharp
public void init(int type)
{
    gameType = type;
    if (gameType == 1)
    {
        // Mode 1: Version không tương thích → Bắt buộc thoát
        m_Txt_dest.text = "���ְ汾����,���˳��ؽ�";
        // Dịch: "当前版本过旧,请退出重进"
        // "Phiên bản quá cũ, vui lòng thoát và vào lại"

        m_Txt_go.text = "ȷ ��";
        // Dịch: "确定"
        // "OK"
    }
    else
    {
        // Mode 0: Có phiên bản mới → Cho phép update
        m_Txt_dest.text = "���ִ�汾����,�Ƿ�ǰ������";
        // Dịch: "发现新版本更新,是否前往更新"
        // "Phát hiện phiên bản mới, có muốn cập nhật không?"

        m_Txt_go.text = "ǰ ��";
        // Dịch: "前往"
        // "Cập nhật"
    }
}
```

**2 modes**:
- **Type 0**: Có update mới (optional)
- **Type 1**: Version quá cũ (bắt buộc)

##### b. Button action (dòng 44-54)

```csharp
private void onClickGo()
{
    if (gameType == 0)
    {
        // Mode 0: Mở URL download APK
        Application.OpenURL("https://a.lywl2025.com/apk/com.dwzy.bfmx.apk");
    }
    else
    {
        // Mode 1: Thoát game
        Application.Quit();
    }
}
```

**Quan sát**:
- APK URL **hardcoded** trong code
- URL khác với `AppConst.PackName`
- Có 2 URLs khác nhau cho APK:
  1. `https://play.google.com/store/apps/details?id=com.slotclassic.bigwin` (WinUpdate)
  2. `https://a.lywl2025.com/apk/com.dwzy.bfmx.apk` (VersionPanel)

---

### 5. HotStart.cs

**Đường dẫn**: `ClientHW2024V11/GameMain/HotStart.cs`

#### Load version.json và check minAppVersion

##### a. Load version config (dòng 79-115)

```csharp
public void loadVersion()
{
    if (GameConst.VesionUrl != "")
    {
        StartCoroutine(SaveAssetFiles(GameConst.VesionUrl + "version.json", (text) =>
        {
            JSONNode node = JSON.Parse(text);
            m_urlResVersion = node["resVer"].AsInt;
            m_ip = node["ip"].Value;
            m_port = node["port"].Value;
            m_backstage = node["backstage"].Value;
            m_customer = node["customer"].Value;
            m_isShow = node["isShow"].AsInt == 1;
            GameConst.CdnUrl = node["CdnUrl"].Value;

            // CHƯA CÓ CHECK minAppVersion Ở ĐÂY!
            // Cần thêm:
            // string minAppVersion = node["minAppVersion"].Value;
            // if (Application.version != minAppVersion) { ... }

            StartCoroutine(loadGame());
        }, true));
    }
}
```

**Vấn đề**: Chưa implement check `minAppVersion` trong `version.json`

##### b. Retry mechanism (dòng 476-539)

```csharp
IEnumerator SaveAssetFiles(string path, Action<string> DownLoad = null, bool isReplace = false)
{
    WWW request = new WWW(originPath);
    yield return request;

    if (request.error != null)
    {
        if (isReplace)
        {
            sendNum++;
            if (sendNum >= 3)
            {
                sendNum = 0;
                sendIndex++;
                if (sendIndex > GameConst.VesionUrlArr.Count - 1)
                {
                    sendIndex = 0;
                }
                GameConst.VesionUrl = GameConst.VesionUrlArr[sendIndex];
                loadVersion();  // Retry với server khác
            }
            else
            {
                StartCoroutine(SaveAssetFiles(path, DownLoad, true));
            }
        }
    }
}
```

**Retry logic**:
1. Thử 3 lần với cùng 1 server
2. Nếu fail → Chuyển sang server khác
3. Rotate qua tất cả servers trong `VesionUrlArr`

---

## 🔍 Cơ chế hoạt động chi tiết

### Scenario 1: APK Update từ CVersionManager

**Điều kiện trigger**:
```csharp
Application.version != _reviewMinVerion
```

**Flow**:

```
1. Game khởi động
2. CVersionManager.CheckResVersion()
3. RequestReviewVersion() [BỊ DISABLE - yield break]
4. Nếu enabled:
   a. Gọi API: POST /apiarr/vestversion
   b. Response encrypted DES:
      {
        "reviewVersion": 1002,
        "minVersion": "1.0.5",
        "cdnUrl": "...",
        "serverUrl": "...",
        "isReview": 0
      }
   c. Decrypt và lưu vào _reviewMinVerion
5. LoadResVersionFile()
6. So sánh Application.version vs _reviewMinVerion
7. Nếu khác:
   → m_eventVersion.state = EVersionState.ApkUpdate
   → m_eventVersion.info = AppConst.PackName
   → TriggerVersionProgressEvent()
8. WinUpdate nhận event
9. Hiển thị popup với apkUrl
10. User click OK → Application.OpenURL(apkUrl)
```

---

### Scenario 2: APK Update từ HotStart (CHƯA IMPLEMENT)

**Điều kiện trigger** (cần implement):
```csharp
string minAppVersion = node["minAppVersion"].Value;
if (Application.version != minAppVersion)
{
    ShowApkUpdateDialog();
}
```

**Flow** (nếu implement):

```
1. Game khởi động
2. HotStart.loadVersion()
3. Load version.json:
   {
     "resVer": 1002,
     "minAppVersion": "1.0.5",
     "CdnUrl": "...",
     ...
   }
4. Check Application.version vs minAppVersion
5. Nếu khác:
   a. Tạo VersionPanel
   b. Init với type = 0 (có update mới)
   c. Show popup
6. User click "Cập nhật"
7. Application.OpenURL("hardcoded URL")
```

---

## 🎯 Các cách triển khai APK URL

### Cách 1: Hardcode trong AppConst (ĐANG DÙNG)

```csharp
// AppConst.cs
public static string PackName = "https://play.google.com/store/apps/details?id=com.slotclassic.bigwin";
```

**Ưu điểm**:
- Đơn giản
- Không cần server config

**Nhược điểm**:
- Phải build lại app để thay đổi URL
- Không linh hoạt
- Có nhiều URLs hardcoded khác nhau (confusing)

---

### Cách 2: Load từ version.json (KHUYẾN NGHỊ)

```json
{
  "resVer": 1002,
  "minAppVersion": "1.0.5",
  "apkDownloadUrl": "https://yourdomain.com/download/latest.apk",
  "apkStoreUrl": "https://play.google.com/store/apps/details?id=com.your.game"
}
```

**Implementation**:

```csharp
// HotStart.cs - Thêm vào loadVersion()
string minAppVersion = node["minAppVersion"].Value;
string apkUrl = node["apkDownloadUrl"].Value;

if (Application.version != minAppVersion)
{
    ShowApkUpdateDialog(apkUrl);
}
```

**Ưu điểm**:
- Linh hoạt, thay đổi URL không cần build lại
- Quản lý tập trung
- Có thể A/B test URLs

**Nhược điểm**:
- Cần maintain version.json trên server

---

### Cách 3: Load từ apkUrl.txt (DESIGNED NHƯNG CHƯA DÙNG)

**URL pattern** (CVersionManager.cs dòng 152):
```
update/pak/{AppOS}/{AppName}/{AppVersion}/apkUrl.txt
```

**Ví dụ**:
```
https://cdn.yourgame.com/update/pak/Android/com.slotclassic.bigwin/1.0.4/apkUrl.txt
```

**Nội dung apkUrl.txt**:
```
https://yourdomain.com/apk/YourGame_v1.0.5.apk
```

**Implementation** (cần thêm code):

```csharp
// CVersionManager.cs
private IEnumerator LoadApkUrlFromFile()
{
    string apkUrlFilePath = GetApkUrlFile();

    WWW www = new WWW(apkUrlFilePath);
    yield return www;

    if (www.error == null)
    {
        string apkUrl = www.text.Trim();
        m_eventVersion.info = apkUrl;  // Thay vì AppConst.PackName
    }
}
```

**Ưu điểm**:
- Version-specific URLs
- Có thể rollback về version cũ
- Organize theo folder structure

**Nhược điểm**:
- Phức tạp hơn
- Cần maintain nhiều files

---

### Cách 4: Load từ API response (DESIGNED NHƯNG BỊ DISABLE)

**API**: `POST /apiarr/vestversion`

**Response có thể thêm field**:
```json
{
  "reviewVersion": 1002,
  "minVersion": "1.0.5",
  "apkDownloadUrl": "https://yourdomain.com/apk/latest.apk",
  "forceUpdate": true
}
```

**Implementation** (trong RequestReviewVersion):

```csharp
var responseJson = DecryptDES(www.downloadHandler.text);
JSONNode node = JSON.Parse(responseJson);

_reviewMinVerion = node["minVersion"].Value;
string apkUrl = node["apkDownloadUrl"].Value;
bool forceUpdate = node["forceUpdate"].AsBool;

// Lưu vào variable để dùng sau
m_apkDownloadUrl = apkUrl;
```

**Ưu điểm**:
- Tích hợp với version check
- Có thể return dynamic URLs (CDN, geo-location)
- Control flags (forceUpdate, etc.)

**Nhược điểm**:
- Cần backend API
- Phức tạp hơn

---

## 🔐 Security considerations

### 1. APK URL validation

**Vấn đề hiện tại**: Không validate URL

```csharp
Application.OpenURL(apkUrl);  // Mở bất kỳ URL nào!
```

**Risk**: Nếu attacker compromise server → có thể redirect người chơi đến malicious APK

**Giải pháp**:

```csharp
private bool IsValidApkUrl(string url)
{
    // Whitelist domains
    string[] allowedDomains = new string[]
    {
        "play.google.com",
        "yourdomain.com",
        "cdn.yourdomain.com"
    };

    Uri uri;
    if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
        return false;

    foreach (var domain in allowedDomains)
    {
        if (uri.Host.EndsWith(domain))
            return true;
    }

    return false;
}

private void OnOkBtnClick()
{
    if (string.IsNullOrEmpty(apkUrl))
    {
        Application.Quit();
    }
    else if (IsValidApkUrl(apkUrl))
    {
        Application.OpenURL(apkUrl);
    }
    else
    {
        Debug.LogError("Invalid APK URL: " + apkUrl);
        Application.Quit();
    }
}
```

---

### 2. HTTPS enforcement

```csharp
private bool IsValidApkUrl(string url)
{
    Uri uri;
    if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
        return false;

    // Chỉ chấp nhận HTTPS
    if (uri.Scheme != "https")
        return false;

    // ... domain validation
}
```

---

### 3. Version string validation

**Vấn đề**: So sánh string exact match

```csharp
if (Application.version != _reviewMinVerion)
```

**Risk**:
- "1.0.10" < "1.0.9" (string comparison)
- "1.0.0" != "1.0" (should be equal)

**Giải pháp**: Parse semantic version

```csharp
private bool IsVersionLowerThan(string currentVersion, string minVersion)
{
    var current = ParseVersion(currentVersion);
    var min = ParseVersion(minVersion);

    return current.CompareTo(min) < 0;
}

private Version ParseVersion(string versionString)
{
    try
    {
        return new Version(versionString);
    }
    catch
    {
        return new Version(0, 0, 0);
    }
}

// Usage:
if (IsVersionLowerThan(Application.version, _reviewMinVerion))
{
    // Trigger APK update
}
```

---

## 📊 Version comparison strategies

### Strategy 1: Exact Match (ĐANG DÙNG)

```csharp
if (Application.version != _reviewMinVerion)
```

**Khi trigger**:
- Local: "1.0.4" vs Server: "1.0.5" → UPDATE ✓
- Local: "1.0.5" vs Server: "1.0.5" → OK ✓
- Local: "1.0.6" vs Server: "1.0.5" → UPDATE ✗ (sai!)

**Vấn đề**: Nếu user có version mới hơn → vẫn bắt update!

---

### Strategy 2: Minimum Version Check (KHUYẾN NGHỊ)

```csharp
if (IsVersionLowerThan(Application.version, _reviewMinVerion))
```

**Khi trigger**:
- Local: "1.0.4" vs Server: "1.0.5" → UPDATE ✓
- Local: "1.0.5" vs Server: "1.0.5" → OK ✓
- Local: "1.0.6" vs Server: "1.0.5" → OK ✓ (đúng!)

**Ưu điểm**: Cho phép user có version mới hơn

---

### Strategy 3: Version Range

```json
{
  "minAppVersion": "1.0.5",
  "maxAppVersion": "1.1.0"
}
```

```csharp
bool needUpdate = IsVersionLowerThan(Application.version, minVersion) ||
                  IsVersionHigherThan(Application.version, maxVersion);
```

**Use case**: Deprecate old clients và beta clients

---

## 🎨 UI/UX Best Practices

### 1. Clear messaging

**BAD** (hiện tại - text bị encoding):
```
���ִ�汾����,�Ƿ�ǰ������
```

**GOOD**:
```
"Phát hiện phiên bản mới v1.0.5"
"Vui lòng cập nhật để tiếp tục chơi"
```

---

### 2. Show version numbers

```csharp
m_Txt_dest.text = string.Format(
    "Phiên bản hiện tại: {0}\n" +
    "Phiên bản yêu cầu: {1}\n" +
    "Vui lòng cập nhật để tiếp tục",
    Application.version,
    _reviewMinVerion
);
```

---

### 3. Show download size (nếu có)

```json
{
  "minAppVersion": "1.0.5",
  "apkDownloadUrl": "...",
  "apkSize": 125829120  // bytes
}
```

```csharp
m_Txt_dest.text = string.Format(
    "Cập nhật mới: {0} MB\n" +
    "Có muốn tải về không?",
    apkSize / 1024f / 1024f
);
```

---

### 4. Differentiate update types

```csharp
public enum UpdateType
{
    Optional,      // Có thể skip
    Recommended,   // Nên update
    Required       // Bắt buộc
}
```

**UI for each type**:

- **Optional**: [Update] [Later]
- **Recommended**: [Update] [Remind me later]
- **Required**: [Update] (no cancel button)

---

## 📱 Platform-specific behaviors

### Android

#### Google Play Store

```csharp
string playStoreUrl = "https://play.google.com/store/apps/details?id=" + Application.identifier;
Application.OpenURL(playStoreUrl);
```

**Behavior**:
- Mở Google Play app nếu có
- Fallback to browser nếu không

#### Direct APK Download

```csharp
string apkUrl = "https://yourdomain.com/apk/YourGame.apk";
Application.OpenURL(apkUrl);
```

**Behavior**:
- Download APK về device
- User phải manually install
- Cần enable "Unknown sources"

**Cảnh báo**: Google Play **CẤM** apps update outside của store!

---

### iOS

#### App Store

```csharp
string appStoreUrl = "https://apps.apple.com/app/id{YOUR_APP_ID}";
Application.OpenURL(appStoreUrl);
```

**Behavior**:
- Mở App Store app
- User phải manually update

**Lưu ý**: iOS **KHÔNG CHO PHÉP** download IPA directly

---

## 🧪 Testing scenarios

### Test Case 1: Normal update flow

```
1. Build app với version 1.0.4
2. Upload to device
3. Setup server với minVersion = "1.0.5"
4. Launch app
5. Expected: Show APK update dialog
6. Click "Update"
7. Expected: Open browser with APK URL
```

---

### Test Case 2: Same version

```
1. Build app với version 1.0.5
2. Server minVersion = "1.0.5"
3. Launch app
4. Expected: No update dialog, enter game normally
```

---

### Test Case 3: Newer client version

```
1. Build app với version 1.0.6
2. Server minVersion = "1.0.5"
3. Launch app
4. Expected: ???
   - Current behavior: Show update (BUG!)
   - Expected behavior: Enter game normally
```

---

### Test Case 4: Network error

```
1. Launch app offline
2. Expected: Show "No internet" dialog
3. Retry button → Check version again
```

---

### Test Case 5: Server down

```
1. Launch app
2. Version server không response
3. Expected: ???
   - Current: Retry 3 lần → Fallback server
   - Nếu tất cả servers down → ???
```

---

## 🚀 Deployment workflow

### Step 1: Prepare new APK

```bash
# Build new APK
unity-build --version 1.0.5

# Sign APK
jarsigner -keystore release.keystore app-release.apk

# Upload to hosting
aws s3 cp app-release.apk s3://your-bucket/apk/v1.0.5.apk
```

---

### Step 2: Update server config

**Option A: version.json**

```json
{
  "resVer": 1002,
  "minAppVersion": "1.0.5",
  "apkDownloadUrl": "https://yourdomain.com/apk/v1.0.5.apk"
}
```

**Option B: API response**

Update database:
```sql
UPDATE version_config
SET min_app_version = '1.0.5',
    apk_download_url = 'https://yourdomain.com/apk/v1.0.5.apk'
WHERE platform = 'android';
```

---

### Step 3: Gradual rollout (RECOMMENDED)

```json
{
  "minAppVersion": "1.0.5",
  "rolloutPercentage": 10  // 10% users first
}
```

Backend logic:
```python
user_hash = hash(user_id) % 100
if user_hash < rollout_percentage:
    return min_version = "1.0.5"
else:
    return min_version = "1.0.4"  # Old version
```

---

### Step 4: Monitor

Track metrics:
- Update completion rate
- Download failures
- Crash reports on new version
- Rollback nếu cần

---

## ⚠️ Common pitfalls

### 1. Hardcoded URLs ở nhiều nơi

**Vấn đề hiện tại**:
- `AppConst.PackName`: Google Play URL
- `VersionPanel.onClickGo()`: Direct APK URL
- Khác nhau → Confusing!

**Giải pháp**: Single source of truth

---

### 2. String comparison cho versions

```csharp
"1.0.10" != "1.0.9"  // FALSE (string comparison)
"1.0.10" < "1.0.9"   // TRUE (wrong!)
```

**Giải pháp**: Use `System.Version` class

---

### 3. Không có fallback mechanism

Nếu version server down → User không vào được game!

**Giải pháp**: Timeout và fallback

```csharp
// Sau 10s không load được version → Cho vào game
yield return new WaitForSecondsRealtime(10f);
if (!versionLoaded)
{
    Debug.LogWarning("Version check timeout, entering game");
    EnterGameWithoutVersionCheck();
}
```

---

### 4. Không thông báo reason

User không biết tại sao phải update:
- Bug fixes?
- New features?
- Security patches?

**Giải pháp**: Include release notes

```json
{
  "minAppVersion": "1.0.5",
  "releaseNotes": "- Fixed login issue\n- Added new slot game\n- Performance improvements"
}
```

---

## 📝 Tổng kết

### Điểm mạnh của hệ thống hiện tại

✅ Có infrastructure cơ bản
✅ Có fallback servers (VesionUrlArr)
✅ Retry mechanism
✅ Encryption cho communication

### Điểm yếu

❌ APK URL hardcoded ở nhiều nơi
❌ String comparison cho versions (sai logic)
❌ RequestReviewVersion bị disable
❌ Không validate APK URLs
❌ Text encoding bị lỗi
❌ Không có version mới hơn min → Vẫn bắt update

### Khuyến nghị

1. **Enable RequestReviewVersion** với proper API
2. **Implement semantic version comparison**
3. **Single source cho APK URL** (từ server)
4. **Validate URLs** trước khi OpenURL
5. **Sửa UI text encoding**
6. **Thêm release notes** vào update dialog
7. **Implement gradual rollout**
8. **Add telemetry** để track update success rate

---

## 🔗 Related files

- `CVersionManager.cs:746-771` - Version comparison logic
- `WinUpdate.cs:398-409` - APK update UI
- `WinUpdate.cs:487-503` - Button handlers
- `AppConst.cs:33` - Hardcoded APK URL
- `VersionPanel.cs:22-54` - Update panel UI
- `HotStart.cs:79-115` - Version.json loading

---

**Kết luận**: Hệ thống APK Update có design tốt nhưng implementation chưa hoàn chỉnh. Cần enable lại các phần bị disable và fix các bugs về version comparison.
