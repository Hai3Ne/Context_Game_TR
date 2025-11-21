# APK Update Flow - Tìm Code bị Comment và Cách Enable

## 🚨 QUAN TRỌNG: Phân biệt 2 loại Update

### ❌ ASSETBUNDLE UPDATE (KHÔNG PHẢI LÀM)
- Update file resources (.unity3d, textures, configs)
- Không cần download APK mới
- Code bị comment trong HotStart.cs dòng 207-215

### ✅ APK UPDATE (ĐANG TÌM)
- Update toàn bộ APK file
- Người chơi phải download và install APK mới
- Code bị comment trong CVersionManager.cs dòng 677-718

---

## 📍 Tất cả Code bị Comment

### 1. CVersionManager.cs - RequestReviewVersion() [QUAN TRỌNG NHẤT]

**File**: `ClientHW2024V11/GameMain/SenSrc/Version/CVersionManager.cs`

**Dòng 675-719**: Toàn bộ function bị DISABLE

```csharp
private IEnumerator RequestReviewVersion()
{
    yield break;  // ← DÒNG NÀY DISABLE TẤT CẢ!

    /* ========== CODE BỊ COMMENT BÊN DƯỚI ========== */
/*  var finalUrl = ClientSetting.Instance.WebDonmain() + "/apiarr/vestversion";
    Debug.Log("finalUrl:");
    Debug.Log(finalUrl);
    var zone = GetZoneCode();
    var num = isWifiProxy() ? 1 : 0;

    // Tạo request string: packageName|appVersion|zone|deviceId|vpnFlag
    var desStr = Application.identifier + "|" +
                 Application.version + "|" +
                 zone + "|" +
                 SystemInfo.deviceUniqueIdentifier + "|" +
                 num;

    if (AppConst.isShowShare)
    {
        if (!PlayerPrefs.HasKey("SHARE"))
        {
            desStr += "|SHARE";
        }
    }

    WWWForm wWForm = new WWWForm();
    var DES = EncryptDES(desStr);  // Encrypt request
    wWForm.AddField("versionname", DES);
    Debug.Log("----------identifier-------------" + desStr);

    // Gọi API
    UnityEngine.Networking.UnityWebRequest www =
        UnityEngine.Networking.UnityWebRequest.Post(finalUrl, wWForm);
    www.certificateHandler = new WebRequestCertificate();
    yield return www.SendWebRequest();

    if (null != www.error)
    {
        Debug.LogError("【RequestReviewVersion】 www.error: " + www.error);
        if (_tryCount-- > 0)
        {
            Debug.Log("【RequestReviewVersion】 重试: " + _tryCount);
            StartCoroutine(RequestReviewVersion());  // Retry
        }
        else
        {
            // Fail sau 3 lần retry → Skip update
            ShowAb = true;
            RequestReviewVersionSuccess();
        }
        yield break;
    }

    Debug.Log("【RequestReviewVersion】 www.text: " + www.downloadHandler.text);
    var str = DecryptDES(www.downloadHandler.text);  // Decrypt response
    RequestReviewVersionSuccess();
*/
}
```

**Chức năng**: Gọi API `/apiarr/vestversion` để check version từ server

**Tại sao bị comment**: Có thể không có backend API này, hoặc đang test

---

### 2. HotStart.cs - Resource Version Check [KHÔNG LIÊN QUAN APK]

**File**: `ClientHW2024V11/GameMain/HotStart.cs`

**Dòng 207-215**: Code check AssetBundle version

```csharp
txt.text = "版本: " + resVersion;

// ========== CODE BỊ COMMENT (CHO ASSETBUNDLE UPDATE) ==========
// if (m_urlResVersion > resVersion)
// {
//     resVersion = m_urlResVersion;
//     StartCoroutine(ResDown());  // Download AssetBundles
// }
// else
// {
    showNext();  // ← LUÔN GỌI showNext() (skip update)
// }
```

**⚠️ LƯU Ý**: Đây là code cho **ASSETBUNDLE UPDATE**, không phải APK Update!

---

## 🔍 Vấn đề với version.json hiện tại

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

### ❌ THIẾU Field cho APK Update:

```json
{
    "resVer": "1001",
    "resVersion": "1001",

    // ⚠️ THIẾU FIELD NÀY để trigger APK Update:
    "minAppVersion": "1.0.5",
    "apkDownloadUrl": "https://yourdomain.com/apk/latest.apk",

    "ip": "54.46.95.179",
    "port": "8200|8201|8202|8203|8204|8205",
    "backstage": "https://rechargeva4.lywl2025.com",
    "customer": "ta4.lywl2025.com",
    "CdnUrl": "https://a.lywl2025.com/tga4/",
    "isShow": "1",
    "isShowks": "1"
}
```

---

## ✅ Flow APK Update ĐÚNG (Không phải AssetBundle)

### Flow Chart

```
Game Start
    │
    ├─→ HotStart.cs: loadVersion()
    │      │
    │      └─→ Load version.json từ server
    │             │
    │             ├─ resVer: 1001 (cho AssetBundle update)
    │             └─ minAppVersion: "1.0.5" (cho APK update) ← THIẾU
    │
    ├─→ Parse version.json
    │      │
    │      └─→ Check có field minAppVersion?
    │             │
    │             ├─ CÓ: So sánh với Application.version
    │             │      │
    │             │      ├─ KHÁC NHAU → Trigger APK Update
    │             │      └─ GIỐNG NHAU → Continue game
    │             │
    │             └─ KHÔNG: Skip APK check (hiện tại)
    │
    └─→ APK Update Flow:
           │
           ├─→ Show VersionPanel
           │      ├─ Text: "Phát hiện phiên bản mới v1.0.5"
           │      ├─ Button [Cập nhật]
           │      └─ Button [Hủy]
           │
           ├─→ User click [Cập nhật]
           │      └─→ Application.OpenURL(apkDownloadUrl)
           │             └─→ Download APK mới
           │
           └─→ User click [Hủy]
                  └─→ Application.Quit()
```

---

## 🛠️ Cách Enable APK Update

### Phương án 1: Dùng version.json (KHUYẾN NGHỊ - ĐƠN GIẢN)

#### Bước 1: Update version.json trên server

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
    "isShowks": "1",

    // ========== THÊM 2 FIELDS NÀY ==========
    "minAppVersion": "1.0.5",
    "apkDownloadUrl": "https://a.lywl2025.com/apk/YourGame.apk"
}
```

#### Bước 2: Thêm code check trong HotStart.cs

**File**: `ClientHW2024V11/GameMain/HotStart.cs`

**Vị trí**: Trong function `loadVersion()` dòng 85-107

**Thêm code sau dòng 96**:

```csharp
public void loadVersion()
{
    if (GameConst.VesionUrl != "")
    {
        StartCoroutine(SaveAssetFiles(GameConst.VesionUrl + "version.json", (text) =>
        {
            PlayerPrefs.SetInt("VERSIONINDEX", sendIndex);
            PlayerPrefs.Save();
            JSONNode node = JSON.Parse(text);
            m_urlResVersion = node["resVer"].AsInt;
            m_ip = node["ip"].Value;
            m_port = node["port"].Value;
            m_backstage = node["backstage"].Value;
            m_customer = node["customer"].Value;
            m_isShow = node["isShow"].AsInt == 1;
            GameConst.CdnUrl = node["CdnUrl"].Value;

            // ========== THÊM CODE NÀY ==========
            // Check APK Update (KHÔNG PHẢI ASSETBUNDLE)
            if (node["minAppVersion"] != null)
            {
                string minAppVersion = node["minAppVersion"].Value;
                string apkDownloadUrl = node["apkDownloadUrl"] != null ?
                    node["apkDownloadUrl"].Value :
                    AppConst.PackName; // Fallback to hardcoded URL

                Debug.Log($"[APK Check] Current: {Application.version}, Required: {minAppVersion}");

                // So sánh version
                if (Application.version != minAppVersion)
                {
                    Debug.LogWarning($"[APK Update] Trigger update dialog");
                    ShowApkUpdateDialog(minAppVersion, apkDownloadUrl);
                    return; // Stop loading game
                }
            }
            // ========== KẾT THÚC CODE THÊM ==========

            if (getChannle() == 1000)
            {
                m_isShow = node["isShowks"].AsInt == 1;
            }
            if (getChannle() == 9999)
            {
                m_isShow = true;
            }

            StartCoroutine(loadGame());
        }, true));
    }
    else
    {
        m_isShow = true;
        StartCoroutine(loadGame());
    }
}
```

#### Bước 3: Thêm function hiển thị APK Update Dialog

**Thêm vào cuối HotStart.cs**:

```csharp
/// <summary>
/// Hiển thị dialog yêu cầu update APK
/// </summary>
private void ShowApkUpdateDialog(string newVersion, string apkUrl)
{
    Debug.Log($"[ShowApkUpdateDialog] New version: {newVersion}, URL: {apkUrl}");

    // Option 1: Sử dụng VersionPanel có sẵn
    var prefabPath = "UI/Prefabs/Version/FirstRes/VersionPanel";
    var obj = CoreEntry.gResLoader.Load(prefabPath);

    if (obj != null)
    {
        var panelObj = GameObject.Instantiate(obj) as GameObject;
        panelObj.transform.SetParent(_versionPanel);
        panelObj.transform.localPosition = Vector3.zero;
        panelObj.transform.localScale = Vector3.one;

        // Get VersionPanel component
        var versionPanel = panelObj.GetComponent<HotUpdate.VersionPanel>();
        if (versionPanel != null)
        {
            versionPanel.init(0); // Type 0 = có update mới

            // Override URL (sửa lại VersionPanel.cs nếu cần)
            // Hoặc dùng reflection để set URL
        }
    }
    else
    {
        Debug.LogError($"[ShowApkUpdateDialog] Cannot load prefab: {prefabPath}");

        // Option 2: Fallback - Show Unity dialog
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.DisplayDialog(
            "Update Required",
            $"Current version: {Application.version}\nRequired version: {newVersion}\n\nPlease download new version.",
            "OK"
        );
        #else
        // Hiển thị native dialog hoặc custom UI
        Debug.LogWarning("[APK Update] Show native dialog here");
        Application.OpenURL(apkUrl); // Mở URL trực tiếp
        Application.Quit(); // Thoát game
        #endif
    }
}
```

---

### Phương án 2: Enable RequestReviewVersion() (PHỨC TẠP - CẦN BACKEND API)

#### Bước 1: Bỏ yield break trong CVersionManager.cs

**File**: `ClientHW2024V11/GameMain/SenSrc/Version/CVersionManager.cs`

**Dòng 675-719**:

```csharp
private IEnumerator RequestReviewVersion()
{
    // ========== BỎ DÒNG NÀY ==========
    // yield break;

    // ========== UNCOMMENT TẤT CẢ CODE BÊN DƯỚI ==========
    var finalUrl = ClientSetting.Instance.WebDonmain() + "/apiarr/vestversion";
    Debug.Log("finalUrl:");
    Debug.Log(finalUrl);
    var zone = GetZoneCode();
    var num = isWifiProxy() ? 1 : 0;

    var desStr = Application.identifier + "|" +
                 Application.version + "|" +
                 zone + "|" +
                 SystemInfo.deviceUniqueIdentifier + "|" +
                 num;

    if (AppConst.isShowShare)
    {
        if (!PlayerPrefs.HasKey("SHARE"))
        {
            desStr += "|SHARE";
        }
    }

    WWWForm wWForm = new WWWForm();
    var DES = EncryptDES(desStr);
    wWForm.AddField("versionname", DES);
    Debug.Log("----------identifier-------------" + desStr);

    UnityEngine.Networking.UnityWebRequest www =
        UnityEngine.Networking.UnityWebRequest.Post(finalUrl, wWForm);
    www.certificateHandler = new WebRequestCertificate();
    yield return www.SendWebRequest();

    if (null != www.error)
    {
        Debug.LogError("【RequestReviewVersion】 www.error: " + www.error);
        if (_tryCount-- > 0)
        {
            Debug.Log("【RequestReviewVersion】 重试: " + _tryCount);
            StartCoroutine(RequestReviewVersion());
        }
        else
        {
            ShowAb = true;
            RequestReviewVersionSuccess();
        }
        yield break;
    }

    Debug.Log("【RequestReviewVersion】 www.text: " + www.downloadHandler.text);
    var str = DecryptDES(www.downloadHandler.text);

    // ========== THÊM CODE PARSE RESPONSE ==========
    try
    {
        JSONNode responseNode = JSON.Parse(str);
        _reviewVerion = responseNode["reviewVersion"].AsInt;
        _reviewMinVerion = responseNode["minVersion"].Value;
        _cdnUrl = responseNode["cdnUrl"].Value;
        _serverUrl = responseNode["serverUrl"].Value;
        isArraign = responseNode["isReview"].AsInt;
        ShowAb = responseNode["showAB"].AsInt == 1;

        Debug.Log($"[RequestReviewVersion] minVersion from server: {_reviewMinVerion}");
    }
    catch (System.Exception ex)
    {
        Debug.LogError($"[RequestReviewVersion] Parse error: {ex.Message}");
    }
    // ========== KẾT THÚC CODE THÊM ==========

    RequestReviewVersionSuccess();
}
```

#### Bước 2: Setup Backend API

**Endpoint**: `POST /apiarr/vestversion`

**Request body**:
```
versionname: <encrypted_string>
```

**Encrypted string format**:
```
com.slotclassic.bigwin|1.0.4|America/Rio_Branco|device123|0|SHARE
```

**Response** (encrypted):
```json
{
    "reviewVersion": 1001,
    "minVersion": "1.0.5",
    "cdnUrl": "https://a.lywl2025.com/tga4/",
    "serverUrl": "https://gameserver.com/",
    "apkDownloadUrl": "https://a.lywl2025.com/apk/latest.apk",
    "isReview": 0,
    "showAB": 0
}
```

---

## 📊 So sánh 2 Phương án

| Feature | Phương án 1 (version.json) | Phương án 2 (API) |
|---------|---------------------------|-------------------|
| **Độ phức tạp** | ⭐ Đơn giản | ⭐⭐⭐ Phức tạp |
| **Cần Backend** | ❌ Không | ✅ Cần API server |
| **Code changes** | Ít (thêm 20 dòng) | Nhiều (uncomment 50+ dòng) |
| **Flexibility** | Trung bình | Cao (dynamic logic) |
| **Security** | DES encryption (có sẵn) | DES encryption (có sẵn) |
| **Khuyến nghị** | ✅ **DÙNG NÀY** | ⚠️ Nếu cần advanced features |

---

## 🎯 Version Comparison - Cách so sánh đúng

### ❌ SAI: String comparison (hiện tại)

```csharp
if (Application.version != minAppVersion)
{
    // Trigger update
}
```

**Vấn đề**:
- "1.0.6" != "1.0.5" → Trigger update (SAI! User có version mới hơn)
- "1.0.10" < "1.0.9" (string comparison sai)

### ✅ ĐÚNG: Semantic version comparison

```csharp
/// <summary>
/// So sánh semantic version
/// </summary>
private bool IsVersionLowerThan(string currentVersion, string minVersion)
{
    try
    {
        System.Version current = new System.Version(currentVersion);
        System.Version min = new System.Version(minVersion);

        return current.CompareTo(min) < 0;
    }
    catch (System.Exception ex)
    {
        Debug.LogError($"[Version Compare] Error: {ex.Message}");
        // Fallback to string comparison
        return currentVersion != minVersion;
    }
}

// Usage:
if (IsVersionLowerThan(Application.version, minAppVersion))
{
    ShowApkUpdateDialog(minAppVersion, apkDownloadUrl);
}
```

**Kết quả**:
- Current: "1.0.4", Min: "1.0.5" → Update ✅
- Current: "1.0.5", Min: "1.0.5" → OK ✅
- Current: "1.0.6", Min: "1.0.5" → OK ✅ (đúng!)
- Current: "1.0.10", Min: "1.0.9" → OK ✅ (đúng!)

---

## 🔧 Testing Steps

### Test 1: APK Update triggered

```
1. Build app với Application.version = "1.0.4"
2. Upload to device
3. Update version.json:
   {
     "minAppVersion": "1.0.5",
     "apkDownloadUrl": "https://yourcdn.com/apk/v1.0.5.apk"
   }
4. Launch app
5. ✅ Expected: Show APK update dialog
6. Click "Cập nhật"
7. ✅ Expected: Open browser with APK download URL
```

### Test 2: Same version (no update)

```
1. Build app với Application.version = "1.0.5"
2. version.json minAppVersion = "1.0.5"
3. Launch app
4. ✅ Expected: No dialog, enter game normally
```

### Test 3: Newer client version

```
1. Build app với Application.version = "1.0.6"
2. version.json minAppVersion = "1.0.5"
3. Launch app
4. ✅ Expected: No dialog, enter game normally (user có version mới hơn)
```

### Test 4: Missing minAppVersion field

```
1. version.json không có field minAppVersion
2. Launch app
3. ✅ Expected: Skip APK check, enter game normally
```

---

## 🚨 Lưu ý quan trọng

### 1. Không nhầm lẫn với AssetBundle Update

**AssetBundle Update** (ResDown):
- Code trong HotStart.cs dòng 207-215 (BỊ COMMENT)
- Check `m_urlResVersion > resVersion`
- Download file .unity3d
- KHÔNG CẦN ENABLE cho APK Update

**APK Update**:
- Check `Application.version != minAppVersion`
- Download file .apk
- CẦN ENABLE bằng 1 trong 2 phương án trên

### 2. APK URL phải valid

Validate URL trước khi OpenURL:

```csharp
private bool IsValidApkUrl(string url)
{
    if (string.IsNullOrEmpty(url))
        return false;

    // Whitelist domains
    string[] allowedDomains = new string[]
    {
        "play.google.com",
        "lywl2025.com",
        "a.lywl2025.com"
    };

    Uri uri;
    if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
        return false;

    // Check HTTPS
    if (uri.Scheme != "https")
        return false;

    // Check domain
    foreach (var domain in allowedDomains)
    {
        if (uri.Host.EndsWith(domain))
            return true;
    }

    return false;
}
```

### 3. Google Play Store Policy

⚠️ **CHÚ Ý**: Google Play **CẤM** apps tự update bên ngoài Play Store!

Nếu app đang trên Google Play, nên dùng Play Store URL:

```csharp
string playStoreUrl = "https://play.google.com/store/apps/details?id=" +
                      Application.identifier;
Application.OpenURL(playStoreUrl);
```

### 4. iOS không cho download IPA trực tiếp

iOS chỉ cho phép update qua App Store:

```csharp
#if UNITY_IOS
string appStoreUrl = "https://apps.apple.com/app/id{YOUR_APP_ID}";
Application.OpenURL(appStoreUrl);
#elif UNITY_ANDROID
Application.OpenURL(apkDownloadUrl); // Direct APK
#endif
```

---

## 📝 Checklist Implementation

### Phương án 1 (version.json) - KHUYẾN NGHỊ

- [ ] Thêm `minAppVersion` vào version.json
- [ ] Thêm `apkDownloadUrl` vào version.json
- [ ] Thêm code check APK version trong HotStart.cs (dòng 96)
- [ ] Thêm function `ShowApkUpdateDialog()`
- [ ] Thêm function `IsVersionLowerThan()` (semantic version)
- [ ] Validate APK URL trước khi OpenURL
- [ ] Test với version khác nhau
- [ ] Sửa VersionPanel text encoding (tiếng Việt/Anh)

### Phương án 2 (API)

- [ ] Bỏ `yield break` trong RequestReviewVersion()
- [ ] Uncomment toàn bộ code (dòng 678-718)
- [ ] Thêm code parse response JSON
- [ ] Setup backend API `/apiarr/vestversion`
- [ ] Implement DES encryption server-side
- [ ] Test API endpoint
- [ ] Test với version khác nhau
- [ ] Sửa VersionPanel text encoding

---

## 🎓 Tổng kết

### Code bị comment:

1. ✅ **CVersionManager.cs:677** - `yield break` (disable RequestReviewVersion)
2. ✅ **CVersionManager.cs:678-718** - Toàn bộ API call logic
3. ❌ **HotStart.cs:207-215** - ResDown (CHO ASSETBUNDLE, không liên quan APK)

### Thiếu trong version.json:

```json
{
    // ... existing fields ...

    // ⚠️ CẦN THÊM:
    "minAppVersion": "1.0.5",
    "apkDownloadUrl": "https://a.lywl2025.com/apk/latest.apk"
}
```

### Flow APK Update đúng:

```
Load version.json
  → Check minAppVersion
    → So sánh với Application.version
      → Nếu lower: Show VersionPanel
        → Click OK: OpenURL(apkDownloadUrl)
        → Click Cancel: Quit()
```

### Khuyến nghị:

🎯 **DÙNG PHƯƠNG ÁN 1** (version.json) vì:
- Đơn giản
- Không cần backend API
- Dễ test
- Dễ maintain

**Phương án 2** chỉ dùng khi cần:
- Dynamic version control
- A/B testing
- Regional updates
- Advanced analytics
