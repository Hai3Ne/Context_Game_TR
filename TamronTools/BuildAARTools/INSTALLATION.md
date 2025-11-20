# 📘 Hướng Dẫn Cài Đặt Môi Trường - SDK AAR Build Tool

**Dành cho người mới bắt đầu** 🌱

Tài liệu này sẽ hướng dẫn bạn từng bước cài đặt môi trường cần thiết để chạy SDK AAR Build Tool.

---

## 📋 Mục Lục

1. [Tổng Quan](#tổng-quan)
2. [Cài Đặt Python](#cài-đặt-python)
   - [Windows](#cài-python-trên-windows)
   - [macOS](#cài-python-trên-macos)
   - [Linux](#cài-python-trên-linux)
3. [Cài Đặt tkinter](#cài-đặt-tkinter)
4. [Cài Đặt Java JDK](#cài-đặt-java-jdk)
5. [Kiểm Tra Môi Trường](#kiểm-tra-môi-trường)
6. [Chạy Tool Lần Đầu](#chạy-tool-lần-đầu)
7. [Troubleshooting](#troubleshooting-xử-lý-sự-cố)
8. [FAQ - Câu Hỏi Thường Gặp](#faq---câu-hỏi-thường-gặp)

---

## 🎯 Tổng Quan

### Tool này cần những gì?

SDK AAR Build Tool cần **3 thành phần chính**:

1. **Python 3.7+** - Ngôn ngữ lập trình để chạy tool
2. **tkinter** - Thư viện GUI (thường đi kèm Python)
3. **Java JDK 8+** - Để build AAR file với Gradle

### Tôi cần biết lập trình không?

**KHÔNG!** Bạn chỉ cần:
- Biết cài đặt phần mềm
- Biết mở Terminal/Command Prompt
- Biết copy/paste lệnh

Tất cả đều có hướng dẫn chi tiết bên dưới! 👇

---

## 📦 Cài Đặt Python

### Kiểm Tra Python Đã Cài Chưa

Trước khi cài, hãy kiểm tra xem máy bạn đã có Python chưa:

**Windows:**
```cmd
python --version
```
hoặc
```cmd
python3 --version
```

**macOS/Linux:**
```bash
python3 --version
```

**Kết quả mong đợi:**
```
Python 3.9.7
```
(Hoặc bất kỳ version nào >= 3.7)

**Nếu thấy lỗi:** `command not found` hoặc `không phải lệnh hợp lệ` → Bạn cần cài Python.

---

### Cài Python Trên Windows

#### **Bước 1: Download Python**

1. Truy cập: https://www.python.org/downloads/
2. Click nút **"Download Python 3.x.x"** (version mới nhất)
3. File tải về: `python-3.x.x-amd64.exe`

#### **Bước 2: Cài Đặt**

1. **Chạy file installer** (double-click)
2. ⚠️ **QUAN TRỌNG:** Tick vào ô **"Add Python to PATH"** (ở dưới cùng)
   ```
   ☑ Add Python 3.x to PATH
   ```
3. Click **"Install Now"**
4. Đợi cài đặt hoàn tất (1-2 phút)
5. Click **"Close"**

#### **Bước 3: Kiểm Tra**

Mở **Command Prompt** (cmd) và gõ:
```cmd
python --version
```

**Kết quả đúng:**
```
Python 3.11.5
```

**Nếu thấy lỗi:**
- Bạn quên tick "Add Python to PATH" → Cài lại hoặc [xem hướng dẫn thêm PATH manual](#thêm-python-vào-path-manual-windows)

#### **Bước 4: Kiểm Tra pip**

pip là công cụ cài package Python (đi kèm Python):
```cmd
pip --version
```

**Kết quả đúng:**
```
pip 23.2.1 from C:\Python311\lib\site-packages\pip (python 3.11)
```

---

### Cài Python Trên macOS

#### **Cách 1: Homebrew (Khuyên Dùng)**

**Bước 1: Cài Homebrew (nếu chưa có)**

Mở **Terminal** và chạy:
```bash
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"
```

**Bước 2: Cài Python**
```bash
brew install python3
```

**Bước 3: Kiểm Tra**
```bash
python3 --version
```

#### **Cách 2: Download Installer**

1. Truy cập: https://www.python.org/downloads/macos/
2. Download file **macOS 64-bit installer**
3. Chạy file `.pkg`
4. Follow hướng dẫn cài đặt
5. Mở Terminal và kiểm tra: `python3 --version`

---

### Cài Python Trên Linux

#### **Ubuntu/Debian:**

```bash
# Update package list
sudo apt update

# Cài Python 3
sudo apt install python3 python3-pip -y

# Kiểm tra
python3 --version
pip3 --version
```

#### **Fedora/RHEL/CentOS:**

```bash
# Cài Python 3
sudo dnf install python3 python3-pip -y

# Hoặc với yum (CentOS 7)
sudo yum install python3 python3-pip -y

# Kiểm tra
python3 --version
```

#### **Arch Linux:**

```bash
# Cài Python 3
sudo pacman -S python python-pip

# Kiểm tra
python --version
```

---

## 🖼️ Cài Đặt tkinter

### Tkinter Là Gì?

tkinter là thư viện Python để tạo giao diện đồ họa (GUI). Tool của chúng ta dùng tkinter để hiển thị cửa sổ, button, text box, v.v.

### Windows & macOS

**Tin tốt:** tkinter đã được **cài sẵn** khi bạn cài Python từ python.org!

Chỉ cần kiểm tra:
```bash
python3 -m tkinter
```

**Kết quả đúng:** Một cửa sổ nhỏ xuất hiện (click "QUIT" để đóng)

**Nếu lỗi:** Xem [Troubleshooting tkinter](#lỗi-tkinter-not-found)

### Linux

Trên Linux, tkinter thường **KHÔNG** cài sẵn. Bạn cần cài riêng:

#### **Ubuntu/Debian:**
```bash
sudo apt-get install python3-tk -y
```

#### **Fedora/RHEL:**
```bash
sudo dnf install python3-tkinter -y
```

#### **Arch Linux:**
```bash
sudo pacman -S tk
```

#### **Kiểm Tra:**
```bash
python3 -m tkinter
```

Một cửa sổ test sẽ xuất hiện → tkinter OK! ✅

---

## ☕ Cài Đặt Java JDK

### Java Dùng Để Làm Gì?

Java JDK cần thiết để chạy **Gradle** - công cụ build AAR file. Nếu không có Java, tool không thể build được AAR.

### Kiểm Tra Java Đã Cài Chưa

```bash
java -version
```

**Kết quả đúng:**
```
java version "11.0.12" 2021-07-20 LTS
Java(TM) SE Runtime Environment 18.9 (build 11.0.12+8-LTS-237)
```

**Nếu lỗi:** `command not found` → Cần cài Java

---

### Cài Java Trên Windows

#### **Cách 1: Oracle JDK (Official)**

1. Truy cập: https://www.oracle.com/java/technologies/downloads/
2. Chọn **Windows** → Download **x64 Installer**
3. Chạy file `.exe`
4. Click **Next** → **Next** → **Install**
5. Đợi cài xong

#### **Cách 2: OpenJDK (Miễn Phí)**

Download Temurin (OpenJDK distribution):
1. Truy cập: https://adoptium.net/
2. Chọn **Version:** Latest LTS (ví dụ: 17)
3. Chọn **Operating System:** Windows
4. Click **Download .msi**
5. Cài đặt file `.msi`

#### **Kiểm Tra:**
Mở **Command Prompt mới** (quan trọng!) và gõ:
```cmd
java -version
```

---

### Cài Java Trên macOS

#### **Homebrew (Khuyên Dùng):**
```bash
brew install openjdk@11

# Thêm Java vào PATH
echo 'export PATH="/usr/local/opt/openjdk@11/bin:$PATH"' >> ~/.zshrc
source ~/.zshrc

# Kiểm tra
java -version
```

#### **Download Installer:**
1. Truy cập: https://adoptium.net/
2. Download macOS installer
3. Cài đặt file `.pkg`

---

### Cài Java Trên Linux

#### **Ubuntu/Debian:**
```bash
# OpenJDK 11
sudo apt update
sudo apt install openjdk-11-jdk -y

# Kiểm tra
java -version
```

#### **Fedora/RHEL:**
```bash
sudo dnf install java-11-openjdk-devel -y
```

#### **Arch Linux:**
```bash
sudo pacman -S jdk-openjdk
```

---

## ✅ Kiểm Tra Môi Trường

Sau khi cài đặt tất cả, hãy kiểm tra lại:

### Script Kiểm Tra Nhanh

**Windows (Command Prompt):**
```cmd
echo === Checking Python ===
python --version
echo.
echo === Checking pip ===
pip --version
echo.
echo === Checking tkinter ===
python -m tkinter
echo.
echo === Checking Java ===
java -version
```

**macOS/Linux (Terminal):**
```bash
echo "=== Checking Python ==="
python3 --version

echo -e "\n=== Checking pip ==="
pip3 --version

echo -e "\n=== Checking tkinter ==="
python3 -m tkinter

echo -e "\n=== Checking Java ==="
java -version
```

### ✅ Checklist

- [ ] Python 3.7+ hiển thị version đúng
- [ ] pip hiển thị version đúng
- [ ] tkinter mở cửa sổ test (không lỗi)
- [ ] Java hiển thị version đúng

**Nếu TẤT CẢ đều OK** → Bạn đã sẵn sàng! 🎉

---

## 🚀 Chạy Tool Lần Đầu

### Bước 1: Mở Terminal/Command Prompt

**Windows:**
- Nhấn `Win + R`
- Gõ `cmd`
- Nhấn Enter

**macOS:**
- Nhấn `Cmd + Space`
- Gõ `Terminal`
- Nhấn Enter

**Linux:**
- Nhấn `Ctrl + Alt + T`

### Bước 2: Di Chuyển Vào Thư Mục Tool

```bash
# Thay đổi path này theo vị trí bạn clone repo
cd /path/to/Context_Game_TR/TamronTools/BuildAARTools
```

**Ví dụ cụ thể:**

**Windows:**
```cmd
cd C:\Users\YourName\Desktop\Context_Game_TR\TamronTools\BuildAARTools
```

**macOS/Linux:**
```bash
cd ~/Desktop/Context_Game_TR/TamronTools/BuildAARTools
```

### Bước 3: Chạy Tool

**Windows:**
```cmd
python builder.py
```
hoặc sử dụng script launcher:
```cmd
run_builder.bat
```

**macOS/Linux:**
```bash
python3 builder.py
```
hoặc sử dụng script launcher:
```bash
./run_builder.sh
```

### Bước 4: Giao Diện Xuất Hiện

Nếu mọi thứ OK, bạn sẽ thấy:
```
================================
SDK AAR Build Tool - TamronTools
================================
```

Và một cửa sổ GUI sẽ mở với các trường input! 🎉

---

## 🔧 Troubleshooting (Xử Lý Sự Cố)

### Lỗi: `python: command not found`

**Nguyên nhân:** Python chưa được cài hoặc chưa có trong PATH

**Giải pháp:**
1. Kiểm tra lại [Cài Đặt Python](#cài-đặt-python)
2. Windows: Đảm bảo đã tick "Add Python to PATH" khi cài
3. Thử gõ `python3` thay vì `python`

---

### Lỗi: `tkinter: No module named '_tkinter'`

**Nguyên nhân:** tkinter chưa được cài

**Giải pháp:**

**Linux:**
```bash
# Ubuntu/Debian
sudo apt-get install python3-tk

# Fedora
sudo dnf install python3-tkinter

# Arch
sudo pacman -S tk
```

**macOS:**
```bash
# Cài lại Python với Homebrew
brew reinstall python-tk@3.11
```

**Windows:**
- Gỡ Python
- Cài lại và tick ô **"tcl/tk and IDLE"** trong Custom Installation

---

### Lỗi: `java: command not found`

**Nguyên nhân:** Java chưa được cài hoặc chưa có trong PATH

**Giải pháp:**
1. Kiểm tra lại [Cài Đặt Java](#cài-đặt-java-jdk)
2. Sau khi cài, **đóng và mở lại Terminal/CMD**
3. Kiểm tra lại: `java -version`

---

### Lỗi: `Permission denied: './run_builder.sh'`

**Nguyên nhân:** File script chưa có quyền execute

**Giải pháp (Linux/macOS):**
```bash
chmod +x run_builder.sh
./run_builder.sh
```

---

### Lỗi: `Gradle build failed`

**Nguyên nhân:** Có thể do:
- Java chưa cài hoặc version quá cũ
- Không có internet (Gradle cần download dependencies lần đầu)
- SDK path không đúng

**Giải pháp:**
1. Kiểm tra Java: `java -version` (cần >= 8)
2. Kiểm tra internet connection
3. Đảm bảo SDK Path đúng: `/path/to/ClientHW2024V11/MySdktuiguang`
4. Xem log chi tiết trong cửa sổ tool

---

### Lỗi: `IllegalAccessError` hoặc `compileReleaseJavaWithJavac FAILED`

**Lỗi đầy đủ:**
```
java.lang.IllegalAccessError: class org.gradle.internal.compiler.java.ClassNameCollector
cannot access class com.sun.tools.javac.code.Symbol$TypeSymbol because module jdk.compiler
does not export com.sun.tools.javac.code to unnamed module
```

**Nguyên nhân:** Java version quá mới so với Gradle version

**Compatibility Matrix:**
```
Java 8-11  → Gradle 5.0+
Java 16    → Gradle 7.0+
Java 17    → Gradle 7.3+
Java 21    → Gradle 7.6+ (recommended 8.5+)
```

**Chẩn đoán:**
```bash
# Kiểm tra Java version
java -version

# Kiểm tra Gradle version
cd ClientHW2024V11/MySdktuiguang
./gradlew --version
```

**Giải pháp:**

**Option 1: Update Gradle (Khuyên Dùng)**
```bash
# Edit file gradle-wrapper.properties
# Đường dẫn: ClientHW2024V11/MySdktuiguang/gradle/wrapper/gradle-wrapper.properties

# Thay đổi dòng distributionUrl từ:
distributionUrl=https\://services.gradle.org/distributions/gradle-6.5-bin.zip

# Thành (Gradle 7.6.4 - stable, tương thích Java 21):
distributionUrl=https\://services.gradle.org/distributions/gradle-7.6.4-bin.zip

# Sau đó build lại
./gradlew clean
./gradlew :MySdkLib:assembleRelease
```

**Option 2: Downgrade Java (Nếu không muốn đổi Gradle)**
```bash
# Ubuntu/Debian - Cài Java 11
sudo apt install openjdk-11-jdk
sudo update-alternatives --config java

# macOS - Cài Java 11
brew install openjdk@11
echo 'export PATH="/usr/local/opt/openjdk@11/bin:$PATH"' >> ~/.zshrc

# Verify
java -version  # Phải hiển thị version 11
```

**Note:** Nếu bạn đang dùng tool, project đã được fix sẵn với Gradle 7.6.4!

---

### Lỗi: `No such file or directory: gradlew`

**Nguyên nhân:** SDK path không đúng

**Giải pháp:**
1. Kiểm tra SDK Path trong GUI
2. Đảm bảo path trỏ đến thư mục chứa file `gradlew`
3. Ví dụ đúng: `/home/user/Context_Game_TR/ClientHW2024V11/MySdktuiguang`

---

### Thêm Python Vào PATH Manual (Windows)

Nếu bạn quên tick "Add to PATH" khi cài:

1. Nhấn `Win + R` → Gõ `sysdm.cpl` → Enter
2. Tab **Advanced** → Click **Environment Variables**
3. Trong **User variables**, tìm **Path** → Click **Edit**
4. Click **New** → Thêm đường dẫn Python:
   ```
   C:\Users\YourName\AppData\Local\Programs\Python\Python311
   C:\Users\YourName\AppData\Local\Programs\Python\Python311\Scripts
   ```
5. Click **OK** → **OK** → **OK**
6. **Đóng và mở lại CMD**
7. Test: `python --version`

---

## 💬 FAQ - Câu Hỏi Thường Gặp

### 1. Tôi không biết lập trình, có sử dụng được tool không?

**CÓ!** Tool có giao diện đồ họa (GUI), bạn chỉ cần:
- Điền thông tin vào form
- Click "Build AAR"
- Đợi kết quả

Không cần code gì cả!

---

### 2. Tôi nên cài Python version nào?

**Khuyên dùng:** Python **3.9** hoặc **3.11** (stable)

**Tối thiểu:** Python **3.7**

**Tránh:** Python 2.x (đã lỗi thời)

---

### 3. Tôi đã có Android Studio, có cần cài Java riêng không?

**CÓ THỂ KHÔNG CẦN!**

Nếu đã cài Android Studio, bạn đã có Java. Nhưng cần thêm Java vào PATH:

**Windows:**
- Tìm thư mục: `C:\Program Files\Android\Android Studio\jre\bin`
- Thêm vào PATH (xem [hướng dẫn thêm PATH](#thêm-python-vào-path-manual-windows))

**macOS/Linux:**
```bash
# Tìm Java của Android Studio
ls ~/Library/Android/sdk/jre  # macOS
ls ~/Android/Sdk/jre          # Linux

# Thêm vào PATH trong ~/.bashrc hoặc ~/.zshrc
export JAVA_HOME=~/Library/Android/sdk/jre
export PATH=$JAVA_HOME/bin:$PATH
```

---

### 4. Tool có chạy trên máy Mac M1/M2 không?

**CÓ!** Tool hoàn toàn tương thích với Apple Silicon.

Lưu ý:
- Cài Python bằng Homebrew (khuyên dùng)
- Java khuyên dùng Azul Zulu for ARM: https://www.azul.com/downloads/?package=jdk

---

### 5. Tôi gặp lỗi "ModuleNotFoundError: No module named 'xxx'"

**Nguyên nhân:** Thư viện Python bị thiếu (hiếm gặp với tool này vì chỉ dùng built-in libraries)

**Giải pháp:**
```bash
# Thử cài lại
pip3 install --upgrade pip

# Hoặc kiểm tra đang dùng Python version nào
python3 --version
```

---

### 6. Tool có cần Internet không?

**LẦN ĐẦU: CÓ** - Gradle cần download dependencies (~100-200MB)

**LẦN SAU: KHÔNG** - Dependencies đã được cache, có thể build offline

---

### 7. File AAR được lưu ở đâu?

**Location:** `MySdkLib/build/outputs/aar/MySdkLib-release.aar`

Và nếu bạn chỉ định Unity Output Path, sẽ được copy tới đó.

---

### 8. Tôi có thể build nhiều project khác nhau không?

**CÓ!** Sử dụng chức năng **Save Config**:
1. Điền thông tin project 1
2. Click "Save Config" → Lưu `project1.json`
3. Lại điền thông tin project 2
4. Click "Save Config" → Lưu `project2.json`

Sau này chỉ cần **Load Config** để switch giữa các project!

---

### 9. Build mất bao lâu?

**Lần đầu:** 5-10 phút (download dependencies)
**Lần sau:** 2-5 phút (tùy cấu hình máy)

Tips tăng tốc:
- Đóng Android Studio khi build
- Tắt antivirus (có thể chặn Gradle)
- Dùng SSD thay vì HDD

---

### 10. Tool có an toàn không? Có virus không?

**HOÀN TOÀN AN TOÀN!**

- Tool là **open source** - bạn có thể đọc code
- Không kết nối internet (trừ Gradle download dependencies lần đầu)
- Không gửi dữ liệu đi đâu
- Chỉ thao tác local files trên máy bạn

---

## 📞 Liên Hệ & Hỗ Trợ

### Bạn vẫn gặp vấn đề?

1. **GitHub Issues:** https://github.com/Hai3Ne/Context_Game_TR/issues
   - Mô tả chi tiết vấn đề
   - Kèm screenshot lỗi
   - Nêu hệ điều hành đang dùng

2. **Email:** ndtmivn123@gmail.com

3. **Xem Log:**
   - Tool có cửa sổ log rất chi tiết
   - Copy log và gửi khi báo lỗi

---

## 🎓 Video Hướng Dẫn (Coming Soon)

Chúng tôi đang chuẩn bị video hướng dẫn:
- Cài đặt môi trường từ đầu
- Demo sử dụng tool
- Xử lý các lỗi thường gặp

Stay tuned! 📺

---

## ✅ Checklist Hoàn Thành

In ra và check từng mục:

- [ ] Đã cài Python 3.7+
- [ ] Lệnh `python --version` hoặc `python3 --version` chạy OK
- [ ] Đã cài tkinter (test bằng `python3 -m tkinter`)
- [ ] Đã cài Java JDK 8+
- [ ] Lệnh `java -version` chạy OK
- [ ] Đã clone repository về máy
- [ ] Đã cd vào thư mục `TamronTools/BuildAARTools`
- [ ] Chạy `python3 builder.py` và thấy GUI xuất hiện
- [ ] Đã đọc [README.md](README.md) để hiểu cách dùng tool

**Nếu tất cả đều check** → Bạn đã sẵn sàng build AAR! 🎉

---

## 🌟 Tips Cho Người Mới

1. **Đừng sợ Terminal/Command Prompt** - Chỉ cần copy/paste lệnh là xong!

2. **Đọc log kỹ** - Tool có log rất chi tiết, hầu hết lỗi đều giải thích trong log

3. **Test từng bước** - Đừng vội vàng, kiểm tra từng component (Python → tkinter → Java)

4. **Google là bạn** - Nếu gặp lỗi, copy error message và google, rất nhiều người đã gặp

5. **Backup trước khi build** - Tool tự động backup nhưng an toàn hơn nếu bạn backup manual

6. **Hỏi khi cần** - Đừng ngại hỏi qua GitHub Issues hoặc email!

---

## 📚 Tài Liệu Tham Khảo

- Python Documentation: https://docs.python.org/3/
- tkinter Tutorial: https://docs.python.org/3/library/tkinter.html
- Gradle User Guide: https://docs.gradle.org/current/userguide/userguide.html
- Android AAR Format: https://developer.android.com/studio/projects/android-library

---

**Cập nhật:** 2025-11-20
**Version:** 1.0.0
**Tác giả:** TamronTools Team

**Chúc bạn build thành công! 🚀**
