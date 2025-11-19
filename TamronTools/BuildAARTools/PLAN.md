# Kế Hoạch Build AAR Tool - Tài Liệu Phân Tích & Thiết Kế

## 📋 Tổng Quan Dự Án

Tool tự động hóa quy trình build SDK AAR cho Unity, giảm thiểu thời gian và sai sót trong quá trình thay đổi cấu hình và build.

---

## 🔍 1. PHÂN TÍCH CẤU TRÚC SOURCE HIỆN TẠI

### 1.1. Cấu Trúc Dự Án
```
ClientHW2024V11/MySdktuiguang/
├── MySdkLib/                          # Module SDK chính
│   ├── src/main/
│   │   ├── java/
│   │   │   ├── com/game/gold/
│   │   │   │   ├── Constants.java           # [CẦN THAY ĐỔI] APP_ID
│   │   │   │   └── LaunchMainActivity.java  # [CẦN THAY ĐỔI] URLs
│   │   │   └── com/dwzy/xkmm/wxapi/        # [CẦN THAY ĐỔI] Package name
│   │   │       ├── WXEntryActivity.java
│   │   │       └── WXPayEntryActivity.java
│   │   ├── res/drawable/
│   │   │   └── splash.jpg                  # [CẦN THAY ĐỔI] Splash image
│   │   └── AndroidManifest.xml
│   ├── libs/                          # Thư viện JAR
│   └── build.gradle                   # Build configuration
└── build.gradle                        # Root build config
```

### 1.2. Các File Cần Thay Đổi Theo Dự Án

#### File 1: `Constants.java`
**Đường dẫn:** `MySdkLib/src/main/java/com/game/gold/Constants.java`
```java
public class Constants {
    public static final String APP_ID ="wx4f16c34621be4aff";  // ← CẦN THAY ĐỔI
}
```
**Thay đổi:** Biến `APP_ID` theo từng dự án (WeChat App ID)

#### File 2: `LaunchMainActivity.java`
**Đường dẫn:** `MySdkLib/src/main/java/com/game/gold/LaunchMainActivity.java`
```java
// Dòng 39-40
URLSpan urlSpan = new URLSpan("https://apiva1.lywl2025.com/user.html");    // ← CẦN THAY ĐỔI
URLSpan urlSpan1 = new URLSpan("https://apiva1.lywl2025.com/yinsi.html");  // ← CẦN THAY ĐỔI
```
**Thay đổi:**
- `urlSpan`: URL User Agreement (Thỏa thuận người dùng)
- `urlSpan1`: URL Privacy Policy (Chính sách bảo mật)

#### File 3 & 4: `WXEntryActivity.java` và `WXPayEntryActivity.java`
**Đường dẫn hiện tại:** `MySdkLib/src/main/java/com/dwzy/xkmm/wxapi/`
```java
package com.dwzy.xkmm.wxapi;  // ← CẦN THAY ĐỔI package name
```
**Thay đổi:**
- Tạo folder mới theo package name (ví dụ: `com/abc/xyz/wxapi/`)
- Copy 2 file vào folder mới
- Thay đổi package declaration trong file

#### File 5: `splash.jpg`
**Đường dẫn:** `MySdkLib/src/main/res/drawable/splash.jpg`
**Thay đổi:** Thay thế file hình ảnh splash screen (định dạng JPG)

---

## ⚠️ 2. VẤN ĐỀ NATIVE LIBRARIES (.so files)

### 2.1. Vấn Đề Hiện Tại
- Sau khi build AAR, phải manually:
  1. Đổi `.aar` → `.zip`
  2. Extract file ZIP
  3. Copy `classes.jar` vào `libs/`
  4. Copy folder `jni/` (chứa arm64-v8a và armeabi-v7a) từ nguồn khác
  5. Đóng gói lại thành ZIP
  6. Đổi `.zip` → `.aar`

### 2.2. Giải Pháp Đúng - Setup JNI Libraries

**Cấu trúc cần tạo:**
```
MySdkLib/src/main/jniLibs/
├── arm64-v8a/
│   └── (các file .so)
└── armeabi-v7a/
    └── (các file .so)
```

**Lợi ích:**
- Gradle tự động đóng gói `.so` vào AAR khi build
- Không cần thao tác manual sau khi build
- AAR output sẵn sàng sử dụng luôn

**Câu hỏi cần trả lời:**
> Bạn đang copy các file `.so` từ đâu? Có sẵn file mẫu không?

---

## 🛠️ 3. KẾ HOẠCH BUILD AAR TOOL

### 3.1. Cấu Trúc Tool

```
TamronTools/BuildAARTools/
├── builder.py                      # Script chính với GUI
├── sdk_config.py                   # Module xử lý config
├── package_manager.py              # Module quản lý package name
├── gradle_builder.py               # Module build AAR với Gradle
├── requirements.txt                # Python dependencies
├── config/
│   └── template_config.json        # Template cấu hình mẫu
├── PLAN.md                         # Tài liệu này
└── README.md                       # Hướng dẫn sử dụng
```

### 3.2. Công Nghệ Sử Dụng

- **Ngôn ngữ:** Python 3.7+
- **GUI Framework:** Tkinter (built-in Python)
- **Build Tool:** Gradle (qua subprocess)
- **File Processing:** shutil, zipfile, re (regex)

### 3.3. Chức Năng Chi Tiết

#### 3.3.1. Giao Diện GUI (Tkinter)

**Input Fields:**
```
┌─────────────────────────────────────────────────┐
│  SDK AAR Build Tool                             │
├─────────────────────────────────────────────────┤
│                                                 │
│  APP ID:            [wx4f16c34621be4aff    ]   │
│  Package Name:      [com.dwzy.xkmm          ]   │
│  User Agreement:    [https://...            ]   │
│  Privacy Policy:    [https://...            ]   │
│  Splash Image:      [Browse...] [path/to.jpg]  │
│                                                 │
│  Output Path:       [Browse...] [Assets/...]   │
│                                                 │
│  [  Build AAR  ]  [  Save Config  ]  [  Exit  ]│
│                                                 │
├─────────────────────────────────────────────────┤
│  Build Log:                                     │
│  ┌─────────────────────────────────────────┐   │
│  │ [INFO] Starting build process...        │   │
│  │ [INFO] Modifying Constants.java...      │   │
│  │ ...                                     │   │
│  └─────────────────────────────────────────┘   │
└─────────────────────────────────────────────────┘
```

#### 3.3.2. Quy Trình Build Tự Động

**Workflow:**
```
START
  ↓
1. Validate Input
   - Kiểm tra APP_ID không rỗng
   - Kiểm tra package name hợp lệ (regex)
   - Kiểm tra URLs hợp lệ
   - Kiểm tra splash.jpg tồn tại
  ↓
2. Create Backup
   - Copy toàn bộ MySdkLib sang temporary folder
   - Làm việc trên backup để không ảnh hưởng source gốc
  ↓
3. Modify Constants.java
   - Regex replace APP_ID
   - Verify thay đổi thành công
  ↓
4. Modify LaunchMainActivity.java
   - Regex replace urlSpan (User Agreement)
   - Regex replace urlSpan1 (Privacy Policy)
   - Verify thay đổi thành công
  ↓
5. Create New WXActivity Package
   - Parse package name (com.abc.xyz → com/abc/xyz/)
   - Tạo folder structure: src/main/java/com/abc/xyz/wxapi/
   - Copy WXEntryActivity.java
   - Copy WXPayEntryActivity.java
   - Replace package declaration trong 2 file
  ↓
6. Replace Splash Image
   - Copy splash.jpg vào res/drawable/
   - Overwrite file cũ
  ↓
7. Build AAR with Gradle
   - Command: ./gradlew :MySdkLib:assembleRelease
   - Capture output log
   - Wait for completion
  ↓
8. Copy AAR to Output Path
   - Tìm file AAR: MySdkLib/build/outputs/aar/*.aar
   - Copy vào Unity path (nếu được chỉ định)
  ↓
9. Cleanup
   - Xóa temporary backup
   - Log completion message
  ↓
END
```

### 3.4. Module Chi Tiết

#### Module 1: `sdk_config.py`
**Chức năng:**
- Load/Save cấu hình từ JSON
- Validate input data
- Quản lý template config

**Methods:**
```python
class SdkConfig:
    def __init__(self):
        self.app_id = ""
        self.package_name = ""
        self.user_agreement_url = ""
        self.privacy_policy_url = ""
        self.splash_image_path = ""
        self.output_path = ""

    def validate(self) -> tuple[bool, str]:
        # Validate all fields
        pass

    def load_from_json(self, path: str):
        # Load config from JSON file
        pass

    def save_to_json(self, path: str):
        # Save config to JSON file
        pass
```

#### Module 2: `package_manager.py`
**Chức năng:**
- Xử lý package name (com.abc.xyz → com/abc/xyz/)
- Tạo folder structure
- Copy và modify WXActivity files

**Methods:**
```python
class PackageManager:
    def __init__(self, sdk_path: str):
        self.sdk_path = sdk_path

    def create_package_structure(self, package_name: str):
        # Tạo folder theo package name
        pass

    def copy_wx_activities(self, package_name: str):
        # Copy 2 file WXActivity
        pass

    def replace_package_declaration(self, file_path: str, new_package: str):
        # Replace package name trong file
        pass
```

#### Module 3: `gradle_builder.py`
**Chức năng:**
- Execute Gradle build
- Capture build output
- Handle errors

**Methods:**
```python
class GradleBuilder:
    def __init__(self, project_path: str):
        self.project_path = project_path

    def build_aar(self, callback=None) -> tuple[bool, str]:
        # Execute: ./gradlew :MySdkLib:assembleRelease
        # callback(log_line) để update GUI
        pass

    def find_output_aar(self) -> str:
        # Tìm file AAR trong build/outputs/aar/
        pass
```

#### Module 4: `builder.py` (Main GUI)
**Chức năng:**
- Tạo GUI với Tkinter
- Orchestrate toàn bộ quy trình
- Update log real-time

**Main Flow:**
```python
class BuilderGUI:
    def __init__(self):
        self.window = tk.Tk()
        self.config = SdkConfig()
        self.setup_ui()

    def setup_ui(self):
        # Create all input fields, buttons
        pass

    def on_build_clicked(self):
        # Execute full build workflow
        self.validate_input()
        self.backup_source()
        self.modify_files()
        self.build_gradle()
        self.copy_output()
        self.cleanup()
        pass

    def log(self, message: str, level="INFO"):
        # Update log text widget
        pass
```

---

## ✅ 4. ĐÁNH GIÁ TÍNH KHẢ THI

### 4.1. Các Tính Năng KHẢ THI (✅)

| Tính Năng | Độ Khó | Phương Pháp | Thời Gian Ước Tính |
|-----------|--------|-------------|-------------------|
| Thay đổi APP_ID | Dễ | Regex replace | 30 phút |
| Thay đổi URLs | Dễ | Regex replace | 30 phút |
| Thay splash.jpg | Dễ | File copy | 15 phút |
| Thay package + WXActivity | Trung bình | Directory creation + File copy + Regex | 2 giờ |
| Build AAR với Gradle | Trung bình | Subprocess call | 1 giờ |
| GUI với Tkinter | Trung bình | Tkinter widgets | 3 giờ |
| Config Management | Dễ | JSON load/save | 1 giờ |

**Tổng thời gian ước tính:** ~8-10 giờ

### 4.2. Yêu Cầu Hệ Thống

**Để tool hoạt động cần:**
1. ✅ Python 3.7+ (có sẵn tkinter)
2. ✅ Java JDK 8+ (để chạy Gradle)
3. ✅ Gradle wrapper (có sẵn trong project: `gradlew`)
4. ✅ Android SDK (đã cài với Android Studio Flamingo)

### 4.3. Ưu Điểm Của Tool

1. **Tự động hóa hoàn toàn:** Từ input → AAR output chỉ 1 click
2. **An toàn:** Làm việc trên backup, không ảnh hưởng source gốc
3. **Có validation:** Kiểm tra input trước khi build
4. **Log chi tiết:** Developer biết chính xác quy trình đang ở bước nào
5. **Save config:** Lưu cấu hình để tái sử dụng
6. **Cross-platform:** Chạy trên Windows/Linux/Mac

### 4.4. Lưu Ý & Giới Hạn

⚠️ **Lưu ý:**
- Tool KHÔNG thay đổi source gốc (làm việc trên temporary copy)
- Cần Gradle và Java đã được cài đặt và config trong PATH
- Build time phụ thuộc vào cấu hình máy (2-5 phút/build)

⚠️ **Giới hạn:**
- Tool không tự động tạo `.so` files (cần setup trước trong `jniLibs/`)
- Chỉ hỗ trợ Android Studio project với Gradle
- Cần internet để Gradle download dependencies (lần đầu)

---

## 🚀 5. ROADMAP TRIỂN KHAI

### Phase 1: Setup & Core Modules (2-3 giờ)
- [ ] Tạo cấu trúc thư mục TamronTools/BuildAARTools/
- [ ] Implement `sdk_config.py`
- [ ] Implement validation logic
- [ ] Test config load/save

### Phase 2: File Modification Modules (3-4 giờ)
- [ ] Implement regex replace cho APP_ID
- [ ] Implement regex replace cho URLs
- [ ] Implement `package_manager.py`
- [ ] Test package creation và file copy
- [ ] Implement splash.jpg replacement

### Phase 3: Gradle Build Module (1-2 giờ)
- [ ] Implement `gradle_builder.py`
- [ ] Test Gradle execution
- [ ] Handle build errors
- [ ] Test AAR output detection

### Phase 4: GUI Development (3-4 giờ)
- [ ] Design GUI layout
- [ ] Implement input fields
- [ ] Implement file browsers
- [ ] Implement log window
- [ ] Connect GUI với backend modules

### Phase 5: Integration & Testing (2-3 giờ)
- [ ] Integrate tất cả modules
- [ ] Test với các bộ parameters khác nhau
- [ ] Test error handling
- [ ] Test trên Windows (Android Studio environment)

### Phase 6: Documentation (1 giờ)
- [ ] Viết README.md chi tiết
- [ ] Tạo video demo (optional)
- [ ] Document troubleshooting

**Tổng thời gian:** ~12-17 giờ (1.5-2 ngày làm việc)

---

## 📝 6. CÂU HỎI CẦN GIẢI ĐÁP

### Q1: Native Libraries (.so files)
**Hiện trạng:** Bạn đang copy `.so` files từ đâu sau khi build AAR?

**Cần xác định:**
- Có file `.so` mẫu không?
- Các file `.so` này là của thư viện nào? (Unity, WeChat SDK, hay custom?)
- Có thể setup sẵn trong `jniLibs/` để Gradle tự động đóng gói không?

**Nếu có sẵn file `.so`:**
→ Tool có thể tự động copy vào `jniLibs/` trước khi build
→ Không cần manual extract/repack AAR nữa

### Q2: Output Path
**Unity project structure?**
- Path đầy đủ: `Assets/Plugins/Android/libs` hay khác?
- Tool có cần auto-copy AAR vào Unity project không?

### Q3: Multiple WXActivity Packages
**Hiện tại có bao nhiêu bộ WXActivity?**
- Thấy có: `com.dzzy.xcfb.wxapi`, `com.dzzy.csly.wxapi`, `com.dwzy.xkmm.wxapi`
- Tool có cần xóa các package cũ không? Hay chỉ thêm mới?

---

## 🎯 7. KẾT LUẬN

### Tính Khả Thi: ✅ HOÀN TOÀN KHẢ THI

**Tool này sẽ giúp:**
- ✅ Tiết kiệm 10-15 phút mỗi lần build
- ✅ Giảm thiểu sai sót do thao tác manual
- ✅ Chuẩn hóa quy trình build
- ✅ Dễ dàng chia sẻ config giữa các dev

**Bước tiếp theo:**
1. Xác nhận các câu hỏi ở mục 6
2. Setup `jniLibs/` với file `.so` mẫu (nếu có)
3. Bắt đầu implement tool theo roadmap

---

**Tài liệu này được tạo bởi Claude - Build AAR Tool Planning**
**Ngày tạo:** 2025-11-19
**Phiên bản:** 1.0
**Trạng thái:** ✅ Sẵn sàng triển khai
