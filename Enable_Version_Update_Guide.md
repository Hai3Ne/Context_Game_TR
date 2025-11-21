# Hướng dẫn Enable lại tính năng Version Update

## Tổng quan 2 loại Update

### 1. Resource Update (Hot Update - Cập nhật nóng)
**Mục đích**: Cập nhật AssetBundles mà không cần tải lại APK

**Luồng hoạt động**:
```
Game khởi động
  → Load version.json từ server
  → So sánh resVersion (server vs local)
  → Nếu server > local:
      ├─ Download files.txt từ CDN
      ├─ So sánh MD5 từng file
      ├─ Download các file đã thay đổi
      └─ Extract vào DataPath
  → Vào game bình thường
```

**Ưu điểm**:
- Người chơi không cần download lại app
- Tốc độ nhanh (chỉ tải file thay đổi)
- Không cần qua App Store/Google Play

**Hạn chế**:
- Chỉ update được AssetBundles, configs, resources
- Không update được native code (Java/Objective-C)
- Không thay đổi được Application.version

---

### 2. APK Update (Full Update - Cập nhật toàn bộ)
**Mục đích**: Bắt buộc người chơi tải APK mới khi có thay đổi lớn

**Luồng hoạt động**:
```
Game khởi động
  → Gọi API /apiarr/vestversion
  → Server trả về minVersion
  → So sánh Application.version với minVersion
  → Nếu khác nhau:
      ├─ Hiển thị VersionPanel
      ├─ Show popup "Cần update APK"
      └─ Mở URL download APK
  → Người chơi download và cài đặt APK mới
```

**Ưu điểm**:
- Có thể update mọi thứ (code, native plugin, Unity version)
- Đảm bảo tất cả người chơi dùng cùng version

**Hạn chế**:
- Người chơi phải download APK mới (100-500MB)
- Cần thời gian chờ đợi
- Có thể gặp vấn đề với Google Play/App Store review

---

## Trạng thái hiện tại

### Code đang BỊ DISABLE

#### 1. HotStart.cs (dòng 207-215)
```csharp
// BỊ COMMENT:
// if (m_urlResVersion > resVersion)
// {
//     resVersion = m_urlResVersion;
//     StartCoroutine(ResDown());
// }
// else
// {
    showNext();  // ← Luôn bỏ qua việc check version
// }
```

#### 2. CVersionManager.cs (dòng 677)
```csharp
private IEnumerator RequestReviewVersion()
{
    yield break;  // ← Dừng ngay lập tức, không check version
    /* ... code bị disable ... */
}
```

#### 3. LoginCtrl.cs (dòng 165)
```csharp
isUpdate = Application.version != "1.0.0";  // ← Luôn false
if (isUpdate && VersionPanel == null)
{
    // VersionPanel không bao giờ được tạo
}
```

---

## Cách Enable lại tính năng

### 🎯 Phương án 1: Sử dụng HotStart.cs (KHUYẾN NGHỊ)

**Ưu điểm**: Đơn giản, đã có sẵn infrastructure

#### Bước 1: Chuẩn bị Server

Tạo file `version.json` trên server với format:

```json
{
  "resVer": 1002,
  "ip": "18.162.135.99",
  "port": "8200|8201|8202",
  "backstage": "https://customer-service-url.com",
  "customer": "https://customer-service-url.com",
  "isShow": 1,
  "isShowks": 0,
  "CdnUrl": "https://cdn.yourgame.com/",
  "minAppVersion": "1.0.5"
}
```

**Các trường quan trọng**:
- `resVer`: Resource version trên server (tăng dần: 1001, 1002, 1003...)
- `CdnUrl`: URL CDN chứa AssetBundles
- `minAppVersion`: Version APK tối thiểu (để check full update)

#### Bước 2: Setup CDN Structure

Cấu trúc thư mục trên CDN:

```
https://cdn.yourgame.com/
├── files.txt                    # File list với MD5
├── UI/
│   └── *.unity3d
├── Lua/
│   └── *.unity3d
├── Audio/
│   └── *.unity3d
└── SubPackages/
    ├── PackageName1/
    │   ├── files.txt
    │   └── *.unity3d
    └── PackageName2/
        ├── files.txt
        └── *.unity3d
```

**Format files.txt**:
```
====STARTPackageName1
UI/MainUI.unity3d|abc123md5hash|524288
UI/LoginUI.unity3d|def456md5hash|1048576
====END
====STARTPackageName2
Audio/BGM.unity3d|ghi789md5hash|2097152
====END
Normal/file.unity3d|jkl012md5hash|131072
```

#### Bước 3: Enable code trong HotStart.cs

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
            GameConst.CdnUrl = node["CdnUrl"].Value;

            // 🔥 THÊM CODE CHECK MIN VERSION
            string minAppVersion = node["minAppVersion"].Value;
            if (!string.IsNullOrEmpty(minAppVersion) &&
                Application.version != minAppVersion)
            {
                // Show APK update dialog
                ShowApkUpdateDialog(minAppVersion);
                return;
            }

            StartCoroutine(loadGame());
        }, true));
    }
}
```

```csharp
IEnumerator loadGame()
{
    // ... existing code ...

    // 🔥 ENABLE LẠI CODE NÀY (dòng 207-215):
    if (m_urlResVersion > resVersion)
    {
        resVersion = m_urlResVersion;
        StartCoroutine(ResDown());  // Tải resources
    }
    else
    {
        showNext();  // Vào game bình thường
    }
}
```

#### Bước 4: Implement APK Update Dialog

Tạo method mới trong HotStart.cs:

```csharp
private void ShowApkUpdateDialog(string newVersion)
{
    // Option 1: Sử dụng VersionPanel có sẵn
    var obj = CoreEntry.gResLoader.Load("UI/Prefabs/Version/FirstRes/VersionPanel");
    if (obj != null)
    {
        var panel = GameObject.Instantiate(obj);
        var versionPanel = panel.GetComponent<HotUpdate.VersionPanel>();
        versionPanel.init(0);  // Type 0 = có update mới
    }

    // Option 2: Hiển thị Unity popup
    // Hoặc implement custom UI
}
```

#### Bước 5: Cấu hình GameConst.VesionUrl

Trong code khởi tạo (có thể là build settings):

```csharp
// File: GameConst.cs hoặc AppConst.cs
public static class GameConst
{
    public static List<string> VesionUrlArr = new List<string>
    {
        "https://version-server-1.yourgame.com/",
        "https://version-server-2.yourgame.com/",  // Backup server
        "https://version-server-3.yourgame.com/"   // Backup server
    };

    public static string VesionUrl = VesionUrlArr[0];
}
```

---

### 🎯 Phương án 2: Sử dụng CVersionManager.cs (PHỨC TẠP HƠN)

**Ưu điểm**: Tính năng đầy đủ hơn, có retry mechanism, thread pool

#### Bước 1: Setup API Server

Tạo API endpoint: `POST /apiarr/vestversion`

**Request body**:
```
versionname: <encrypted string>
```

**Encrypted string format** (trước khi encrypt):
```
packageName|appVersion|timezone|deviceId|vpnFlag|SHARE
```

Ví dụ:
```
com.dwzy.bfmx|1.0.4|America/Rio_Branco|abc123device|0|SHARE
```

**Response** (encrypted):
```json
{
  "reviewVersion": 1002,
  "minVersion": "1.0.5",
  "cdnUrl": "https://cdn.yourgame.com/",
  "serverUrl": "https://gameserver.yourgame.com/",
  "isReview": 0,
  "showAB": 0
}
```

#### Bước 2: Enable RequestReviewVersion()

Sửa file `CVersionManager.cs`:

```csharp
private IEnumerator RequestReviewVersion()
{
    // 🔥 BỎ DÒNG NÀY:
    // yield break;

    // 🔥 UNCOMMENT CODE BÊN DƯỚI và sửa lại:
    var finalUrl = "https://your-api-server.com/apiarr/vestversion";

    var zone = GetZoneCode();
    var num = isWifiProxy() ? 1 : 0;

    var desStr = Application.identifier + "|" +
                 Application.version + "|" +
                 zone + "|" +
                 SystemInfo.deviceUniqueIdentifier + "|" +
                 num;

    WWWForm wWForm = new WWWForm();
    var DES = EncryptDES(desStr);
    wWForm.AddField("versionname", DES);

    UnityEngine.Networking.UnityWebRequest www =
        UnityEngine.Networking.UnityWebRequest.Post(finalUrl, wWForm);
    www.certificateHandler = new WebRequestCertificate();
    yield return www.SendWebRequest();

    if (null != www.error)
    {
        Debug.LogError("RequestReviewVersion error: " + www.error);
        if (_tryCount-- > 0)
        {
            StartCoroutine(RequestReviewVersion());
        }
        else
        {
            // Fallback: Cho vào game
            ShowAb = true;
            RequestReviewVersionSuccess();
        }
        yield break;
    }

    // 🔥 PARSE RESPONSE:
    var responseJson = DecryptDES(www.downloadHandler.text);
    JSONNode node = JSON.Parse(responseJson);

    _reviewVerion = node["reviewVersion"].AsInt;
    _reviewMinVerion = node["minVersion"].Value;
    _cdnUrl = node["cdnUrl"].Value;
    _serverUrl = node["serverUrl"].Value;
    isArraign = node["isReview"].AsInt;
    ShowAb = node["showAB"].AsInt == 1;

    RequestReviewVersionSuccess();
}
```

#### Bước 3: Enable LoginCtrl check

Sửa file `LoginCtrl.cs`:

```csharp
public void StartConnectServer()
{
    // 🔥 SỬA ĐIỀU KIỆN:
    // Cách 1: Luôn check version
    isUpdate = true;

    // Cách 2: Check dựa trên config từ server
    // isUpdate = HotStart.ins.m_urlResVersion > 0;

    if (isUpdate && VersionPanel == null)
    {
        var obj = CoreEntry.gResLoader.Load("UI/Prefabs/Version/FirstRes/VersionPanel");
        var obj1 = GameObject.Instantiate(obj);
        obj1.transform.SetParent(MainPanelMgr.Instance.uUIRootObj.transform);
        var canvas = obj1.GetComponent<Canvas>();
        canvas.worldCamera = MainPanelMgr.Instance.uiCamera;
        VersionPanel = obj1;  // 🔥 SỬA: Lưu instance, không phải prefab
    }
    // ... rest of code
}
```

---

## Workflow hoàn chỉnh sau khi enable

### Scenario 1: Resource Update (Hot Update)

```
1. Game khởi động
2. Load version.json:
   {
     "resVer": 1003,
     "minAppVersion": "1.0.4"
   }
3. Local version: 1002
4. App version: "1.0.4" ✓
5. → Trigger Resource Update:
   - Show progress bar
   - Download files.txt
   - Compare MD5
   - Download changed files (5 files, 10MB)
   - Extract to DataPath
   - Update local version to 1003
6. Enter game
```

### Scenario 2: APK Update (Full Update)

```
1. Game khởi động
2. Load version.json:
   {
     "resVer": 1003,
     "minAppVersion": "1.0.5"
   }
3. App version: "1.0.4" ✗
4. → Trigger APK Update:
   - Show VersionPanel
   - Display message: "发现大版本更新,是否前往链接!"
   - Button: "前往" → Open APK download URL
   - Button: "取消" → Quit game
5. User clicks "前往"
6. Open URL: https://yourgame.com/apk/latest.apk
7. User downloads and installs new APK
8. Relaunch game with version 1.0.5
```

### Scenario 3: No Update needed

```
1. Game khởi động
2. Load version.json:
   {
     "resVer": 1002,
     "minAppVersion": "1.0.4"
   }
3. Local version: 1002 ✓
4. App version: "1.0.4" ✓
5. → Skip update, enter game directly
```

---

## Checklist Implementation

### Server Side
- [ ] Setup version.json file
- [ ] Configure CDN với files.txt
- [ ] Generate MD5 cho tất cả AssetBundles
- [ ] Setup backup servers
- [ ] (Optional) Implement API /apiarr/vestversion

### Client Side
- [ ] Uncomment code trong HotStart.cs (dòng 207-215)
- [ ] Thêm check minAppVersion
- [ ] Implement APK update dialog
- [ ] Cấu hình GameConst.VesionUrl
- [ ] Test local → CDN connection
- [ ] Sửa VersionPanel text encoding (tiếng Trung → tiếng Việt/Anh)

### Testing
- [ ] Test resource update: Local 1000 → Server 1001
- [ ] Test APK update: v1.0.0 → v1.0.1
- [ ] Test no update: Cùng version
- [ ] Test network error: Retry mechanism
- [ ] Test CDN fallback: Server 1 down → Server 2
- [ ] Test progress bar display
- [ ] Test sub-package download

---

## Code Examples

### 1. Generate files.txt với MD5

Python script để generate files.txt:

```python
import os
import hashlib

def calculate_md5(file_path):
    md5 = hashlib.md5()
    with open(file_path, 'rb') as f:
        for chunk in iter(lambda: f.read(4096), b""):
            md5.update(chunk)
    return md5.hexdigest()

def generate_files_txt(cdn_path, output_file):
    with open(output_file, 'w', encoding='utf-8') as f:
        for root, dirs, files in os.walk(cdn_path):
            for file in files:
                if file.endswith('.unity3d'):
                    file_path = os.path.join(root, file)
                    relative_path = os.path.relpath(file_path, cdn_path)
                    md5 = calculate_md5(file_path)
                    size = os.path.getsize(file_path)

                    # Format: path|md5|size
                    f.write(f"{relative_path}|{md5}|{size}\n")

# Usage
generate_files_txt('/path/to/cdn', 'files.txt')
```

### 2. Server API Implementation (Node.js)

```javascript
const express = require('express');
const crypto = require('crypto');
const app = express();

const DES_KEY = 'QefO3cX2';  // Phải match với Unity

function decryptDES(encrypted) {
    const decipher = crypto.createDecipheriv('des-cbc', DES_KEY,
        Buffer.from([0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF]));
    let decrypted = decipher.update(encrypted, 'base64', 'utf8');
    decrypted += decipher.final('utf8');
    return decrypted;
}

function encryptDES(text) {
    const cipher = crypto.createCipheriv('des-cbc', DES_KEY,
        Buffer.from([0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF]));
    let encrypted = cipher.update(text, 'utf8', 'base64');
    encrypted += cipher.final('base64');
    return encrypted;
}

app.post('/apiarr/vestversion', async (req, res) => {
    const encryptedData = req.body.versionname;
    const decryptedData = decryptDES(encryptedData);

    // Parse: packageName|appVersion|timezone|deviceId|vpnFlag
    const [packageName, appVersion, timezone, deviceId, vpnFlag] =
        decryptedData.split('|');

    // Logic check version
    const currentMinVersion = "1.0.5";
    const currentResVersion = 1003;

    const response = {
        reviewVersion: currentResVersion,
        minVersion: currentMinVersion,
        cdnUrl: "https://cdn.yourgame.com/",
        serverUrl: "https://gameserver.yourgame.com/",
        isReview: 0,
        showAB: 0
    };

    const encryptedResponse = encryptDES(JSON.stringify(response));
    res.send(encryptedResponse);
});

app.listen(3000);
```

### 3. Sửa VersionPanel text encoding

File: `VersionPanel.cs`

```csharp
public void init(int type)
{
    gameType = type;
    if (gameType == 1)
    {
        // 🔥 SỬA LẠI TEXT:
        m_Txt_dest.text = "Phiên bản cũ, vui lòng thoát và tải lại";
        m_Txt_go.text = "Xác nhận";
    }
    else
    {
        // 🔥 SỬA LẠI TEXT:
        m_Txt_dest.text = "Phát hiện phiên bản mới, bạn có muốn cập nhật không?";
        m_Txt_go.text = "Cập nhật";
    }
}
```

---

## Lưu ý quan trọng

### 1. Version numbering
- **App Version**: Follow Semantic Versioning (1.0.0, 1.0.1, 1.1.0)
- **Resource Version**: Số nguyên tăng dần (1000, 1001, 1002...)

### 2. CDN Caching
- Set proper cache headers
- Use version parameter: `?v=1002`
- Clear CloudFlare cache khi update

### 3. Rollback plan
- Giữ backup version cũ
- Có thể downgrade resVersion nếu cần

### 4. Testing
- Test trên mạng chậm (3G)
- Test khi CDN down
- Test với nhiều device khác nhau

### 5. Security
- Encrypt version.json nếu cần
- Validate MD5 checksum
- HTTPS cho tất cả requests

---

## Troubleshooting

### Vấn đề: Progress bar không hiển thị
**Nguyên nhân**: Event không được trigger
**Giải pháp**: Check `CVersionManager.OnVersionProgressEvent`

### Vấn đề: Download bị stuck
**Nguyên nhân**: Network timeout
**Giải pháp**: Tăng timeout, implement retry

### Vấn đề: Files.txt parse error
**Nguyên nhân**: Format không đúng
**Giải pháp**: Validate format: `path|md5|size`

### Vấn đề: VersionPanel không show
**Nguyên nhân**: `isUpdate = false`
**Giải pháp**: Set `isUpdate = true` trong LoginCtrl.cs

---

## Tổng kết

**Khuyến nghị**: Dùng **Phương án 1 (HotStart.cs)** vì:
- ✅ Đơn giản, dễ implement
- ✅ Infrastructure đã có sẵn
- ✅ Ít bug hơn
- ✅ Dễ maintain

**Phương án 2 (CVersionManager.cs)** chỉ dùng khi:
- Cần tính năng phức tạp (A/B testing, regional update)
- Cần control chi tiết hơn
- Có team backend đầy đủ

**Next steps**:
1. Setup version.json trên server
2. Test với 1 device
3. Enable code trong HotStart.cs
4. Test với nhiều scenarios
5. Deploy lên production
