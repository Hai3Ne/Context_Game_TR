# SDK AAR Build Tool

Công cụ tự động hóa quy trình build SDK AAR cho Unity projects. Tiết kiệm thời gian và giảm thiểu sai sót khi thay đổi cấu hình và build SDK.

## Tính năng

- ✅ Giao diện GUI thân thiện với Python Tkinter
- ✅ Tự động thay đổi APP_ID (WeChat)
- ✅ Tự động thay đổi Package Name và tạo WXActivity files
- ✅ Tự động thay đổi URLs (User Agreement, Privacy Policy)
- ✅ Tự động thay thế Splash Image
- ✅ Tự động build AAR với Gradle
- ✅ Tự động copy AAR vào Unity project
- ✅ Log chi tiết real-time
- ✅ Save/Load configuration
- ✅ Backup và restore source tự động

## 🚀 Bắt đầu nhanh

**Người mới bắt đầu?** Đọc [**INSTALLATION.md**](INSTALLATION.md) - Hướng dẫn chi tiết cài đặt môi trường từ đầu!

## Yêu cầu hệ thống

### Bắt buộc:
- **Python 3.7+** (với tkinter)
- **Java JDK 8+** (để chạy Gradle)
- **Android Studio Flamingo | 2022.2.1** (hoặc tương đương)
- **Gradle wrapper** (có sẵn trong project)

### Kiểm tra:
```bash
# Kiểm tra Python
python3 --version

# Kiểm tra tkinter
python3 -m tkinter

# Kiểm tra Java
java -version
```

## Cấu trúc thư mục

```
TamronTools/BuildAARTools/
├── builder.py                    # Main GUI application
├── sdk_config.py                 # Configuration management
├── package_manager.py            # Package & file operations
├── gradle_builder.py             # Gradle build automation
├── requirements.txt              # Python dependencies
├── config/
│   └── template_config.json      # Default configuration template
├── PLAN.md                       # Design document
└── README.md                     # This file
```

## Cài đặt

### 1. Clone repository
```bash
git clone https://github.com/Hai3Ne/Context_Game_TR.git
cd Context_Game_TR/TamronTools/BuildAARTools
```

### 2. Cài đặt Python tkinter (nếu chưa có)

**Ubuntu/Debian:**
```bash
sudo apt-get install python3-tk
```

**Fedora/RHEL:**
```bash
sudo dnf install python3-tkinter
```

**Windows:**
Tkinter đã được cài sẵn với Python installer từ python.org

**macOS:**
```bash
brew install python-tk
```

### 3. Đảm bảo Java và Gradle đã được cài đặt
Tool sử dụng Gradle wrapper của project, không cần cài Gradle riêng.

## Sử dụng

### Khởi chạy Tool

```bash
cd TamronTools/BuildAARTools
python3 builder.py
```

### Giao diện

Tool sẽ hiển thị giao diện GUI với các trường:

1. **SDK Path**: Đường dẫn tới SDK project (mặc định: `ClientHW2024V11/MySdktuiguang`)
2. **APP ID**: WeChat App ID (ví dụ: `wx4f16c34621be4aff`)
3. **Package Name**: Package name cho WXActivity (ví dụ: `com.dwzy.xkmm`)
4. **User Agreement**: URL tới User Agreement page
5. **Privacy Policy**: URL tới Privacy Policy page
6. **Splash Image**: Đường dẫn tới file splash.jpg mới
7. **Unity Output**: Đường dẫn output AAR vào Unity (ví dụ: `Assets/Plugins/Android/libs`)

### Quy trình Build

1. **Điền thông tin** vào các trường
2. **Chọn Splash Image** bằng nút Browse
3. **Click "Build AAR"**
4. Theo dõi log trong cửa sổ bên dưới
5. Đợi build hoàn tất (2-5 phút tùy máy)

### Quy trình tự động

Tool sẽ tự động thực hiện:

```
[STEP 1/8] Validate configuration
[STEP 2/8] Create backup of source
[STEP 3/8] Initialize package manager
[STEP 4/8] Update APP_ID in Constants.java
[STEP 5/8] Update URLs in LaunchMainActivity.java
[STEP 6/8] Create new package structure & copy WXActivity files
[STEP 7/8] Replace splash.jpg
[STEP 8/8] Build AAR with Gradle
          ├── Clean build
          ├── Build AAR
          ├── Copy to Unity (if specified)
          └── Restore original source
```

### Save/Load Config

**Save Config:**
- Click "Save Config"
- Chọn vị trí lưu file .json
- Config được lưu để tái sử dụng

**Load Config:**
- Click "Load Config"
- Chọn file config .json đã lưu
- Tất cả trường sẽ được điền tự động

## Output

### AAR File Location

**Sau khi build thành công:**
- File AAR gốc: `MySdkLib/build/outputs/aar/MySdkLib-release.aar`
- File AAR trong Unity: `<Unity_Output_Path>/MySdkLib-release.aar`

### AAR Structure

File AAR đã được Gradle tự động đóng gói:
```
MySdkLib-release.aar
├── classes.jar                    # Compiled Java classes
├── AndroidManifest.xml            # Manifest file
├── res/                           # Resources
│   └── drawable/splash.jpg        # Your custom splash
├── jni/
│   ├── arm64-v8a/
│   │   ├── libmsaoaidauth.so
│   │   └── libmsaoaidsec.so
│   └── armeabi-v7a/
│       ├── libmsaoaidauth.so
│       └── libmsaoaidsec.so
└── libs/                          # JAR dependencies
```

## Troubleshooting

### Python tkinter không tìm thấy

**Linux:**
```bash
sudo apt-get install python3-tk  # Ubuntu/Debian
sudo dnf install python3-tkinter   # Fedora/RHEL
```

**macOS:**
```bash
brew install python-tk
```

### Gradle build failed

1. Kiểm tra Java đã cài đặt: `java -version`
2. Kiểm tra internet connection (Gradle cần download dependencies lần đầu)
3. Xem log chi tiết trong cửa sổ tool
4. Thử build manual trong Android Studio để xem lỗi chi tiết

### Permission denied on gradlew

**Linux/macOS:**
```bash
cd ClientHW2024V11/MySdktuiguang
chmod +x gradlew
```

### AAR không chứa .so files

Đảm bảo folder `jniLibs` đã được setup đúng:
```
MySdkLib/src/main/jniLibs/
├── arm64-v8a/
│   └── *.so files
└── armeabi-v7a/
    └── *.so files
```

### Source bị thay đổi sau khi build

Tool TỰ ĐỘNG restore source gốc sau khi build. Nếu gặp lỗi giữa chừng, tool sẽ restore từ backup.

Nếu cần restore manual:
```bash
# Tìm backup folder
ls -la ClientHW2024V11/ | grep backup

# Restore
rm -rf ClientHW2024V11/MySdktuiguang
mv ClientHW2024V11/MySdktuiguang_backup_YYYYMMDD_HHMMSS ClientHW2024V11/MySdktuiguang
```

## FAQ

### Q: Tool có thay đổi source gốc không?
**A:** KHÔNG. Tool làm việc trên backup copy và tự động restore sau khi build.

### Q: Có cần Android Studio mở không?
**A:** KHÔNG cần. Tool chạy Gradle độc lập.

### Q: Build mất bao lâu?
**A:** 2-5 phút tùy cấu hình máy. Lần đầu có thể lâu hơn (Gradle download dependencies).

### Q: Có thể build nhiều config khác nhau không?
**A:** CÓ. Dùng Save/Load Config để lưu nhiều bộ config khác nhau.

### Q: Tool có hoạt động trên Windows không?
**A:** CÓ. Tool cross-platform (Windows/Linux/macOS).

### Q: Gradle version được quản lý thế nào?
**A:** Gradle wrapper (`gradlew`) đảm bảo dùng đúng version được định nghĩa trong project.

## Thay đổi Version

Để thay đổi version của SDK, edit file:
```
ClientHW2024V11/MySdktuiguang/MySdkLib/build.gradle
```

Sửa:
```gradle
defaultConfig {
    versionCode 2         // Increment this
    versionName "2.0"     // Update version string
}
```

Sau đó chạy tool build như bình thường.

## So sánh: Manual vs Tool

### Manual Process (Cũ - 15-20 phút):
1. Mở Android Studio
2. Sửa Constants.java → APP_ID
3. Sửa LaunchMainActivity.java → 2 URLs
4. Tạo folder package mới
5. Copy 2 file WXActivity
6. Sửa package declaration trong 2 file
7. Copy splash.jpg mới
8. Click Build → Build Bundle/APK → Build AAR
9. Chờ build xong
10. Đổi .aar → .zip
11. Extract ZIP
12. Copy classes.jar vào libs/
13. Copy folder jni/
14. Zip lại
15. Đổi .zip → .aar
16. Copy vào Unity

### With Tool (Mới - 2-5 phút):
1. Điền config vào GUI
2. Click "Build AAR"
3. Đợi
4. XONG!

## Liên hệ & Báo lỗi

- GitHub Issues: https://github.com/Hai3Ne/Context_Game_TR/issues
- Email: ndtmivn123@gmail.com

## License

MIT License - Free to use and modify

## Credits

**Developed by:** TamronTools Team
**Version:** 1.0.0
**Date:** 2025-11-20

---

**Happy Building! 🚀**
