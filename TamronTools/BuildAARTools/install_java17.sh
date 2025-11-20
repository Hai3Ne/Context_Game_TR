#!/bin/bash
# Auto Install Java 17 - Simple Fix for Build Error

echo "========================================"
echo "  Auto Install Java 17 for Android SDK"
echo "========================================"
echo ""

# Detect OS
if [[ "$OSTYPE" == "linux-gnu"* ]]; then
    OS="linux"
elif [[ "$OSTYPE" == "darwin"* ]]; then
    OS="mac"
else
    echo "[ERROR] This script is for Linux/macOS only"
    echo "For Windows, download from: https://adoptium.net/temurin/releases/?version=17"
    exit 1
fi

echo "[INFO] Detected OS: $OS"
echo ""

# Linux installation
if [ "$OS" = "linux" ]; then
    echo "[STEP 1] Installing Java 17..."

    # Detect distro
    if [ -f /etc/debian_version ]; then
        echo "[INFO] Ubuntu/Debian detected"
        sudo apt update
        sudo apt install openjdk-17-jdk -y
    elif [ -f /etc/redhat-release ]; then
        echo "[INFO] Fedora/RHEL detected"
        sudo dnf install java-17-openjdk-devel -y
    elif [ -f /etc/arch-release ]; then
        echo "[INFO] Arch Linux detected"
        sudo pacman -S jdk17-openjdk --noconfirm
    else
        echo "[ERROR] Unsupported Linux distribution"
        echo "Please install Java 17 manually"
        exit 1
    fi
fi

# macOS installation
if [ "$OS" = "mac" ]; then
    echo "[STEP 1] Installing Java 17..."

    # Check if Homebrew is installed
    if ! command -v brew &> /dev/null; then
        echo "[ERROR] Homebrew not found!"
        echo "Install Homebrew first: /bin/bash -c \"\$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)\""
        exit 1
    fi

    brew install openjdk@17

    # Link Java 17
    sudo ln -sfn /usr/local/opt/openjdk@17/libexec/openjdk.jdk /Library/Java/JavaVirtualMachines/openjdk-17.jdk
fi

echo ""
echo "[STEP 2] Verifying installation..."
java -version 2>&1 | grep "17" > /dev/null

if [ $? -eq 0 ]; then
    echo "[SUCCESS] Java 17 installed successfully!"
    java -version
else
    echo "[WARNING] Java 17 installed but not set as default"
    echo ""
    echo "To set Java 17 as default:"

    if [ "$OS" = "linux" ]; then
        echo "  sudo update-alternatives --config java"
        echo "  (Then select java-17)"
    elif [ "$OS" = "mac" ]; then
        echo "  Add to ~/.zshrc or ~/.bashrc:"
        echo "  export PATH=\"/usr/local/opt/openjdk@17/bin:\$PATH\""
    fi
fi

echo ""
echo "========================================"
echo "  Next Steps:"
echo "========================================"
echo "1. Open Android Studio"
echo "2. File → Project Structure → SDK Location"
echo "3. Set JDK location to Java 17:"

if [ "$OS" = "linux" ]; then
    echo "   /usr/lib/jvm/java-17-openjdk-amd64"
elif [ "$OS" = "mac" ]; then
    echo "   /Library/Java/JavaVirtualMachines/openjdk-17.jdk/Contents/Home"
fi

echo ""
echo "4. Apply → OK"
echo "5. Build → Rebuild Project"
echo ""
echo "[DONE] Setup complete!"
echo "========================================"
