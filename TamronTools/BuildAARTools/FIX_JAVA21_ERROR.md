# 🔧 Hướng Dẫn Fix Lỗi Build - Java 21 Compatibility

## ❌ Lỗi Gặp Phải

```
Execution failed for task ':MySdkLib:compileReleaseJavaWithJavac'.
java.lang.IllegalAccessError: class org.gradle.internal.compiler.java.ClassNameCollector
cannot access class com.sun.tools.javac.code.Symbol$TypeSymbol
because module jdk.compiler does not export com.sun.tools.javac.code
```

## 🔍 Nguyên Nhân

- **Java 21** cần **Gradle 8.5+** (không phải 7.6.4)
- **Android Gradle Plugin 4.1.3** cũ, không tương thích Gradle 8.x

## ✅ Giải Pháp A: Update Full Stack (Khuyên Dùng)

### Bước 1: Check Version Hiện Tại

**Trong Android Studio Terminal:**
```bash
# Check Java version
java -version

# Check Gradle version
./gradlew --version
```

### Bước 2: Update Android Gradle Plugin

**File: `build.gradle` (root)**

Thay đổi:
```gradle
dependencies {
    // Từ:
    classpath "com.android.tools.build:gradle:4.1.3"

    // Thành:
    classpath "com.android.tools.build:gradle:7.4.2"
}
```

**Tại sao 7.4.2?**
- Tương thích với Gradle 8.0+
- Tương thích với Java 21
- Stable, đã test kỹ
- Không cần thay đổi code nhiều

### Bước 3: Update Gradle Wrapper

**File: `gradle/wrapper/gradle-wrapper.properties`**

Thay đổi:
```properties
# Từ:
distributionUrl=https\://services.gradle.org/distributions/gradle-7.6.4-bin.zip

# Thành:
distributionUrl=https\://services.gradle.org/distributions/gradle-8.0-bin.zip
```

### Bước 4: Update JVM Target (nếu cần)

**File: `MySdkLib/build.gradle`**

Kiểm tra (giữ nguyên nếu đã có):
```gradle
android {
    compileOptions {
        sourceCompatibility JavaVersion.VERSION_1_8
        targetCompatibility JavaVersion.VERSION_1_8
    }
}
```

### Bước 5: Sync & Clean Build

**Trong Android Studio:**
```
1. File → Sync Project with Gradle Files
2. Build → Clean Project
3. Build → Rebuild Project
```

**Hoặc Terminal:**
```bash
./gradlew clean
./gradlew :MySdkLib:assembleRelease
```

---

## ✅ Giải Pháp B: Downgrade Java (Nếu không muốn update Gradle)

### Bước 1: Cài Java 17

**Ubuntu/Debian:**
```bash
sudo apt install openjdk-17-jdk -y
```

**macOS:**
```bash
brew install openjdk@17
```

**Windows:**
- Download từ: https://adoptium.net/temurin/releases/?version=17
- Cài file .msi

### Bước 2: Set Java 17 Trong Android Studio

```
1. File → Project Structure
2. SDK Location → JDK location
3. Browse → Chọn JDK 17 (ví dụ: /usr/lib/jvm/java-17-openjdk)
4. Apply → OK
```

### Bước 3: Set Java 17 Trong Terminal

**Linux/macOS:**
```bash
# Xem các Java version có sẵn
update-alternatives --config java  # Linux
/usr/libexec/java_home -V          # macOS

# Set JAVA_HOME
export JAVA_HOME=/usr/lib/jvm/java-17-openjdk
export PATH=$JAVA_HOME/bin:$PATH

# Thêm vào ~/.bashrc hoặc ~/.zshrc để permanent
```

**Windows:**
```
1. Win + R → sysdm.cpl
2. Advanced → Environment Variables
3. JAVA_HOME → Edit → C:\Program Files\Java\jdk-17
4. OK → OK
```

### Bước 4: Verify

```bash
java -version
# Phải hiển thị: openjdk version "17.0.x"
```

### Bước 5: Build Lại

```bash
./gradlew clean
./gradlew :MySdkLib:assembleRelease
```

---

## 📊 Compatibility Matrix Chi Tiết

| Java Version | Minimum Gradle | Recommended Gradle | Android Gradle Plugin |
|--------------|----------------|--------------------|-----------------------|
| Java 8       | 5.0            | 7.6                | 4.1.x - 8.x          |
| Java 11      | 5.0            | 7.6                | 4.2.x - 8.x          |
| Java 17      | 7.3            | 8.0                | 7.2.x - 8.x          |
| **Java 21**  | **8.5**        | **8.8**            | **8.1.x+**           |

---

## 🎯 Khuyến Nghị

### **Option 1: Update Stack (Best Long-term)**
```
Java 21 + Gradle 8.0 + Android Gradle Plugin 7.4.2
```
**Ưu điểm:**
- ✅ Modern, performant
- ✅ Future-proof
- ✅ Hỗ trợ lâu dài

**Nhược điểm:**
- ⚠️ Cần update một số config
- ⚠️ Download Gradle mới (1 lần)

### **Option 2: Downgrade Java (Quick Fix)**
```
Java 17 + Gradle 7.6.4 + Android Gradle Plugin 4.1.3
```
**Ưu điểm:**
- ✅ Không cần thay đổi build files
- ✅ Quick fix, chạy ngay

**Nhược điểm:**
- ⚠️ Java cũ hơn
- ⚠️ Thiếu features mới

---

## 🧪 Test Sau Khi Fix

### Test 1: Gradle Version
```bash
./gradlew --version

# Kết quả mong đợi (Option 1):
# Gradle 8.0
# Java: 21.0.x

# Kết quả mong đợi (Option 2):
# Gradle 7.6.4
# Java: 17.0.x
```

### Test 2: Clean Build
```bash
./gradlew clean
./gradlew :MySdkLib:assembleRelease --info
```

**Thành công khi thấy:**
```
BUILD SUCCESSFUL in Xm Ys
XX actionable tasks: XX executed
```

### Test 3: Check AAR Output
```bash
ls -lh MySdkLib/build/outputs/aar/

# Phải thấy file:
# MySdkLib-release.aar
```

---

## 🐛 Troubleshooting

### Lỗi: "Minimum supported Gradle version is X.X"

**Nguyên nhân:** Android Gradle Plugin version không tương thích Gradle version

**Fix:**
- Nếu dùng AGP 7.4.2 → Cần Gradle 7.4+
- Nếu dùng AGP 8.x → Cần Gradle 8.0+

### Lỗi: "Unsupported class file major version XX"

**Nguyên nhân:** Java version cao hơn compile target

**Fix:**
```gradle
// Trong MySdkLib/build.gradle
android {
    compileOptions {
        sourceCompatibility JavaVersion.VERSION_11  // Tăng lên
        targetCompatibility JavaVersion.VERSION_11
    }
}
```

### Lỗi: "Could not resolve com.android.tools.build:gradle:7.4.2"

**Nguyên nhân:** Repository không có plugin

**Fix:**
```gradle
// Trong build.gradle (root)
buildscript {
    repositories {
        google()        // Đảm bảo có dòng này
        mavenCentral()  // Và dòng này
    }
}
```

---

## 📞 Cần Hỗ Trợ?

Nếu vẫn gặp lỗi, hãy cung cấp:
1. Output của `java -version`
2. Output của `./gradlew --version`
3. Nội dung file `build.gradle` (root)
4. Full error log

---

**Tác giả:** TamronTools Team
**Cập nhật:** 2025-11-20
**Version:** 1.1
