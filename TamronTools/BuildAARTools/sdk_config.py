#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
SDK Configuration Manager
Manages configuration for SDK AAR Build Tool
"""

import json
import os
import re
from typing import Tuple


class SdkConfig:
    """Manages SDK build configuration"""

    def __init__(self):
        """Initialize with default values"""
        self.app_id = ""
        self.package_name = ""
        self.user_agreement_url = ""
        self.privacy_policy_url = ""
        self.splash_image_path = ""
        self.unity_output_path = ""
        self.sdk_path = ""

    def validate(self) -> Tuple[bool, str]:
        """
        Validate all configuration fields

        Returns:
            Tuple[bool, str]: (is_valid, error_message)
        """
        # Validate APP_ID
        if not self.app_id or len(self.app_id.strip()) == 0:
            return False, "APP_ID không được để trống"

        if not self.app_id.startswith("wx"):
            return False, "APP_ID phải bắt đầu bằng 'wx' (WeChat App ID)"

        # Validate Package Name
        if not self.package_name or len(self.package_name.strip()) == 0:
            return False, "Package Name không được để trống"

        # Regex for valid Java package name
        package_pattern = r'^[a-z][a-z0-9_]*(\.[a-z][a-z0-9_]*)+$'
        if not re.match(package_pattern, self.package_name):
            return False, "Package Name không hợp lệ (ví dụ: com.example.app)"

        # Validate URLs
        url_pattern = r'^https?://.+'
        if not re.match(url_pattern, self.user_agreement_url):
            return False, "User Agreement URL không hợp lệ (phải bắt đầu http:// hoặc https://)"

        if not re.match(url_pattern, self.privacy_policy_url):
            return False, "Privacy Policy URL không hợp lệ (phải bắt đầu http:// hoặc https://)"

        # Validate Splash Image
        if not self.splash_image_path or len(self.splash_image_path.strip()) == 0:
            return False, "Splash Image path không được để trống"

        if not os.path.exists(self.splash_image_path):
            return False, f"Splash Image không tồn tại: {self.splash_image_path}"

        if not self.splash_image_path.lower().endswith(('.jpg', '.jpeg')):
            return False, "Splash Image phải là file .jpg hoặc .jpeg"

        # Validate SDK Path
        if not self.sdk_path or len(self.sdk_path.strip()) == 0:
            return False, "SDK Path không được để trống"

        if not os.path.exists(self.sdk_path):
            return False, f"SDK Path không tồn tại: {self.sdk_path}"

        # Validate Unity Output Path (optional - có thể để trống)
        # if self.unity_output_path and not os.path.exists(os.path.dirname(self.unity_output_path)):
        #     return False, f"Unity Output Path parent directory không tồn tại"

        return True, ""

    def load_from_json(self, file_path: str) -> bool:
        """
        Load configuration from JSON file

        Args:
            file_path: Path to JSON config file

        Returns:
            bool: True if successful, False otherwise
        """
        try:
            with open(file_path, 'r', encoding='utf-8') as f:
                data = json.load(f)

            self.app_id = data.get('app_id', '')
            self.package_name = data.get('package_name', '')
            self.user_agreement_url = data.get('user_agreement_url', '')
            self.privacy_policy_url = data.get('privacy_policy_url', '')
            self.splash_image_path = data.get('splash_image_path', '')
            self.unity_output_path = data.get('unity_output_path', '')
            self.sdk_path = data.get('sdk_path', '')

            return True
        except Exception as e:
            print(f"Error loading config: {e}")
            return False

    def save_to_json(self, file_path: str) -> bool:
        """
        Save configuration to JSON file

        Args:
            file_path: Path to save JSON config file

        Returns:
            bool: True if successful, False otherwise
        """
        try:
            data = {
                'app_id': self.app_id,
                'package_name': self.package_name,
                'user_agreement_url': self.user_agreement_url,
                'privacy_policy_url': self.privacy_policy_url,
                'splash_image_path': self.splash_image_path,
                'unity_output_path': self.unity_output_path,
                'sdk_path': self.sdk_path
            }

            # Create directory if not exists
            os.makedirs(os.path.dirname(file_path), exist_ok=True)

            with open(file_path, 'w', encoding='utf-8') as f:
                json.dump(data, f, indent=2, ensure_ascii=False)

            return True
        except Exception as e:
            print(f"Error saving config: {e}")
            return False

    def to_dict(self) -> dict:
        """Convert config to dictionary"""
        return {
            'app_id': self.app_id,
            'package_name': self.package_name,
            'user_agreement_url': self.user_agreement_url,
            'privacy_policy_url': self.privacy_policy_url,
            'splash_image_path': self.splash_image_path,
            'unity_output_path': self.unity_output_path,
            'sdk_path': self.sdk_path
        }

    def __str__(self) -> str:
        """String representation"""
        return f"""SDK Config:
  APP_ID: {self.app_id}
  Package: {self.package_name}
  User Agreement URL: {self.user_agreement_url}
  Privacy Policy URL: {self.privacy_policy_url}
  Splash Image: {self.splash_image_path}
  Unity Output: {self.unity_output_path}
  SDK Path: {self.sdk_path}
"""


if __name__ == '__main__':
    # Test the config module
    config = SdkConfig()
    config.app_id = "wx4f16c34621be4aff"
    config.package_name = "com.example.test"
    config.user_agreement_url = "https://example.com/user.html"
    config.privacy_policy_url = "https://example.com/privacy.html"
    config.splash_image_path = "/path/to/splash.jpg"
    config.sdk_path = "/path/to/sdk"

    print(config)
    is_valid, error = config.validate()
    print(f"Valid: {is_valid}, Error: {error}")
