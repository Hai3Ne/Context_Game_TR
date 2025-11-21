# Tài liệu Tính năng VersionPanel

## Tổng quan

VersionPanel là một hệ thống quản lý phiên bản và cập nhật game, được thiết kế để thông báo cho người chơi khi có phiên bản mới và yêu cầu họ cập nhật. Hiện tại tính năng này **đang bị disable**.

## Trạng thái hiện tại

- **Vị trí**: `ClientHW2024V11/HotUpdate/UI/UI/Common/View/VersionPanel.cs`
- **Trạng thái**: DISABLED (bị vô hiệu hóa)
- **Lý do disable**: Trong `LoginCtrl.cs` (dòng 165), code kiểm tra `isUpdate = Application.version != "1.0.0"` và chỉ hiển thị VersionPanel khi điều kiện này đúng

## Kiến trúc hệ thống

### 1. VersionPanel.cs
**Đường dẫn**: `ClientHW2024V11/HotUpdate/UI/UI/Common/View/VersionPanel.cs`

#### Mục đích
Panel UI để hiển thị thông báo về phiên bản game và yêu cầu người chơi cập nhật.

#### Các chức năng chính

```csharp
// Khởi tạo panel với loại game
public void init(int type)
```

**Hai loại hiển thị**:
1. **Type = 0**: Phát hiện phiên bản mới
   - Text: "���ִ�汾����,�Ƿ�ǰ������" (bị lỗi encoding - có thể là tiếng Trung)
   - Button: "ǰ ��"
   - Hành động: Mở URL tải APK (`https://a.lywl2025.com/apk/com.dwzy.bfmx.apk`)

2. **Type = 1**: Phiên bản không tương thích
   - Text: "���ְ汾����,���˳��ؽ�" (bị lỗi encoding)
   - Button: "ȷ ��"
   - Hành động: Thoát game (`Application.Quit()`)

#### UI Components (từ VersionPanel.BindComponents.cs)
- `m_Txt_dest`: Text hiển thị thông báo
- `m_Btn_Go`: Button hành động
- `m_Txt_go`: Text trên button

#### Vấn đề hiện tại
- **Text bị lỗi encoding**: Các chuỗi tiếng Trung bị hiển thị sai
- **Hardcoded URL**: URL tải APK được hardcode trong code

---

### 2. CVersionManager.cs
**Đường dẫn**: `ClientHW2024V11/GameMain/SenSrc/Version/CVersionManager.cs`

#### Mục đích
Quản lý toàn bộ quy trình kiểm tra, tải xuống và cập nhật phiên bản game.

#### Các chức năng chính

##### a. Kiểm tra phiên bản
```csharp
public void CheckResVersion()
```
- Khởi tạo phiên bản local
- Kiểm tra kết nối internet
- Gọi `RequestReviewVersion()` để kiểm tra phiên bản server

##### b. States (EVersionState)
```csharp
public enum EVersionState
{
    Extracting,              // Giải nén đồng bộ
    ExtracSuccess,           // Giải nén thành công
    NoInternet,              // Không có mạng
    ApkUpdate,               // Phát hiện phiên bản lớn
    ResUpdating,             // Đang cập nhật tài nguyên
    ResUpdateSuccess,        // Cập nhật tài nguyên thành công
    ResUpdateFail,           // Cập nhật tài nguyên thất bại
    ResExtractSuccess,       // Giải nén tài nguyên thành công
    ResExtractFail,          // Giải nén tài nguyên thất bại
    PackageCfgFail,          // Lỗi cấu hình gói
    PackageUpdating,         // Đang cập nhật từng gói
    PackageUpdateSuccess,    // Cập nhật tất cả gói thành công
    PackageUpdateFail,       // Cập nhật gói thất bại
    AllPackageDownloaded,    // Tất cả gói đã tải về
    PackageExtracting,       // Đang giải nén gói
    PackageExtractSuccess,   // Giải nén gói thành công
    PackageExtractFail,      // Giải nén gói thất bại
    ShowPross,               // Hiển thị tiến trình
    ShowTips                 // Hiển thị tips
}
```

##### c. Events
```csharp
public event Action<VersionProgressEvent> OnVersionProgressEvent;
public event Action OnVersionErrorEvent;
```

##### d. File version tracking
- **Local version file**: `resversion.ver` - Lưu phiên bản tài nguyên local
- **Package info file**: `SubPackage/LoadedPackages.ver` - Lưu thông tin các gói đã tải

##### e. Cập nhật tài nguyên
```csharp
private IEnumerator ResFPointDown(string strURL, string strSaveFile, bool bTryPermanentHost)
```
- So sánh file MD5 giữa local và server
- Tải xuống các file đã thay đổi
- Giải nén và copy vào thư mục game

##### f. Quản lý Sub-packages
```csharp
private void DownSubPackage(int nID)
```
- Hỗ trợ tải gói theo từng phần
- Có 2 loại: Login download (type=1) và Background download (type=0)

#### Cấu hình quan trọng
- **CDN URL**: Được lấy từ `CurrentBundleVersion.ResUpdateCdnUrl`
- **App Version**: Lấy từ `Application.version`
- **Resource Version**: Phiên bản tài nguyên bắt đầu từ 1000

---

### 3. CurrentBundleVersion.cs
**Đường dẫn**: `ClientHW2024V11/GameMain/SenSrc/Version/CurrentBundleVersion.cs`

#### Mục đích
Lưu trữ các thông tin phiên bản cố định của game.

#### Các thuộc tính
```csharp
public static string AppVersion { get { return Application.version; } }
public static int ResVersion { get { return 1000; } }
public static string productName { get { return Application.identifier; } }
```

#### Lưu ý
- Các URL CDN đều trả về **empty string** `""` - có thể đã bị disable hoặc chuyển sang cấu hình khác
- Resource version mặc định là **1000**

---

### 4. LoginCtrl.cs
**Đường dẫn**: `ClientHW2024V11/HotUpdate/UI/UI/Login/LoginCtrl.cs`

#### Vai trò
Controller chính cho việc đăng nhập và quản lý VersionPanel.

#### Quản lý VersionPanel

```csharp
public GameObject VersionPanel = null;
public bool isUpdate = false;

public void StartConnectServer()
{
    isUpdate = Application.version != "1.0.0";
    if (isUpdate && VersionPanel == null)
    {
        var obj = CoreEntry.gResLoader.Load("UI/Prefabs/Version/FirstRes/VersionPanel");
        var obj1 = GameObject.Instantiate(obj);
        obj1.transform.SetParent(MainPanelMgr.Instance.uUIRootObj.transform);
        var canvas = obj1.GetComponent<Canvas>();
        canvas.worldCamera = MainPanelMgr.Instance.uiCamera;
        VersionPanel = obj;
    }
    // ...
}
```

#### Điều kiện hiển thị VersionPanel
1. `isUpdate = true` (khi `Application.version != "1.0.0"`)
2. `VersionPanel == null` (chưa được tạo)

**LÝ DO BỊ DISABLE**: Vì `Application.version` có thể đang là "1.0.0", nên `isUpdate` luôn false và VersionPanel không bao giờ được tạo.

---

### 5. WinUpdate.cs
**Đường dẫn**: `ClientHW2024V11/GameMain/SenSrc/WinUpdate.cs`

#### Mục đích
UI Panel hiển thị tiến trình cập nhật và các thông báo trong quá trình update game.

#### Các chức năng chính

##### a. UI Components
- `_updateUI`: Panel chính cho update
- `_proBar`: Progress bar
- `_updatePro`: Text hiển thị tiến độ
- `_txtTip`: Text hiển thị tips
- `_tipPanle`: Panel popup cho thông báo

##### b. Xử lý Events
```csharp
public void OnUpdateResEvent(VersionProgressEvent data)
```
Xử lý tất cả các state từ `CVersionManager`:
- **Extracting**: Hiển thị "Extraindo recursos do pacote inicial..."
- **ApkUpdate**: Hiển thị popup yêu cầu cập nhật APK
- **ResUpdating**: Hiển thị progress bar với tốc độ tải
- **PackageUpdating**: Hiển thị "Atualizando recursos..."
- Và nhiều state khác...

##### c. Popup Handlers
```csharp
private void OnOkBtnClick()      // Đồng ý cập nhật
private void OnCancleBtnClick()  // Hủy và thoát game
private void OnSureBtnClick()    // Thử lại kết nối
```

---

### 6. InitGame.cs
**Đường dẫn**: `ClientHW2024V11/GameMain/InitGame.cs`

#### Mục đích
Khởi tạo game và load các metadata cho HybridCLR (Hot Update system).

#### Quy trình khởi tạo

1. **Load ThirdPartyPanel** (dòng 33)
2. **Load UIInit** (dòng 55)
3. **Show WinUpdate** (dòng 64)
4. **Load Metadata for AOT Assemblies** (dòng 107-201)
   - Load `mscorlib.dll`, `System.dll`, `System.Core.dll`
   - Load HotUpdate DLL
5. **Load UICtrl** (dòng 194-226)
6. **Complete initialization** (dòng 254-258)

---

### 7. DownSubPack.cs
**Đường dẫn**: `ClientHW2024V11/HotUpdate/Common/DownSubPack.cs`

#### Mục đích
Quản lý việc tải xuống các sub-package (gói con) của game.

#### Các chức năng chính

##### a. Download Sub-Package
```csharp
public void downSubPack(string name)
```
- Đọc danh sách file từ `files.txt`
- Parse các file thuộc sub-package cụ thể
- Tải xuống từng file
- Broadcast progress qua Message system

##### b. File Format
```
====START<package_name>
file_path|md5|size
file_path|md5|size
====END
```

##### c. Progress Tracking
```csharp
Message.Broadcast(MessageName.REFRESH_MAINUIDOWN_PANEL + name, progress, isDone);
```

---

## Luồng hoạt động tổng thể

### 1. Game Start
```
InitGame.Start()
  └─> WinUpdate.ShowUI()
       └─> WinUpdate.onInit()
            └─> CVersionManager.CheckExtractResource()
```

### 2. Version Check
```
CVersionManager.CheckResVersion()
  └─> RequestReviewVersion() [DISABLED - yield break ở dòng 677]
       └─> LoadResVersionFile()
            ├─> [Nếu có update] DownloadResNewVersionPackage()
            └─> [Nếu không] CheckResVersionNone()
```

### 3. Resource Update
```
ResFPointDown()
  ├─> So sánh files.txt (local vs server)
  ├─> Tải các file đã thay đổi
  ├─> Giải nén vào DataPath
  └─> UpdateLocalResVersionFile()
```

### 4. Sub-Package Download
```
DownloadSubPackagesCfg()
  └─> DownloadNextPackage()
       └─> DownSubPackage(id)
            ├─> Tải gói theo thread pool
            ├─> Giải nén
            └─> UpdateSubPackageVersion()
```

### 5. Login Flow
```
LoginCtrl.StartConnectServer()
  ├─> [Nếu isUpdate] Tạo VersionPanel (DISABLED)
  ├─> Kết nối server
  └─> Hiển thị Login UI
```

---

## Vấn đề và cải tiến

### Vấn đề hiện tại

1. **VersionPanel bị disable**
   - Điều kiện `Application.version != "1.0.0"` không bao giờ đúng
   - Panel không được hiển thị

2. **Text encoding bị lỗi**
   - Các chuỗi tiếng Trung trong `VersionPanel.cs` bị hiển thị sai
   - Dòng 27, 28, 31, 32

3. **Hardcoded values**
   - URL APK được hardcode trong `VersionPanel.cs:48`
   - Resource version mặc định 1000 trong `CurrentBundleVersion.cs`

4. **RequestReviewVersion bị disable**
   - Có `yield break` ở dòng đầu tiên (CVersionManager.cs:677)
   - Logic kiểm tra phiên bản từ server không hoạt động

5. **Empty CDN URLs**
   - Tất cả URL trong `CurrentBundleVersion.cs` đều trả về `""`
   - Hệ thống update không thể hoạt động

### Đề xuất cải tiến

1. **Enable VersionPanel**
   ```csharp
   // Trong LoginCtrl.cs:165
   // Thay đổi điều kiện kiểm tra
   isUpdate = true; // Hoặc kiểm tra version từ server
   ```

2. **Sửa lỗi encoding**
   - Chuyển sang sử dụng localization system
   - Hoặc sửa các string literals thành UTF-8

3. **Cấu hình động**
   - Di chuyển APK URL và CDN URLs sang file config
   - Load từ server hoặc PlayerPrefs

4. **Enable version check**
   - Bỏ `yield break` trong `RequestReviewVersion()`
   - Implement logic kiểm tra từ server

5. **Thêm error handling**
   - Retry mechanism cho network failures
   - Fallback URLs cho CDN

---

## File locations tổng hợp

### Scripts chính
1. `ClientHW2024V11/HotUpdate/UI/UI/Common/View/VersionPanel.cs`
2. `ClientHW2024V11/HotUpdate/UI/BindComponents/VersionPanel.BindComponents.cs`
3. `ClientHW2024V11/GameMain/SenSrc/Version/CVersionManager.cs`
4. `ClientHW2024V11/GameMain/SenSrc/Version/CurrentBundleVersion.cs`
5. `ClientHW2024V11/HotUpdate/UI/UI/Login/LoginCtrl.cs`
6. `ClientHW2024V11/GameMain/SenSrc/WinUpdate.cs`
7. `ClientHW2024V11/GameMain/InitGame.cs`
8. `ClientHW2024V11/HotUpdate/Common/DownSubPack.cs`

### Prefabs
- `UI/Prefabs/Version/FirstRes/VersionPanel` (được load trong LoginCtrl.cs:169)
- `UI/Prefabs/VersionUpdate/VersionUpdatePanel` (WinUpdate panel)

### Data files
- `resversion.ver` - Local resource version
- `SubPackage/LoadedPackages.ver` - Downloaded packages info
- `files.txt` - File list với MD5 checksums

---

## Kết luận

VersionPanel là một hệ thống version management đầy đủ nhưng hiện đang bị disable do:
1. Điều kiện kiểm tra version không đúng
2. RequestReviewVersion bị yield break
3. CDN URLs trả về empty

Để enable lại, cần:
1. Sửa điều kiện `isUpdate` trong `LoginCtrl.StartConnectServer()`
2. Enable `RequestReviewVersion()` và cấu hình server URL
3. Cấu hình CDN URLs trong `CurrentBundleVersion.cs` hoặc từ remote config
4. Sửa lỗi encoding cho các text messages
