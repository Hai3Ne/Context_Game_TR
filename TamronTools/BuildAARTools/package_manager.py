#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Package Manager
Handles package name changes and WXActivity files management
"""

import os
import re
import shutil
from typing import Tuple, List


class PackageManager:
    """Manages package structure and WXActivity files"""

    def __init__(self, sdk_path: str):
        """
        Initialize PackageManager

        Args:
            sdk_path: Path to SDK root directory
        """
        self.sdk_path = sdk_path
        self.java_src_path = os.path.join(sdk_path, 'MySdkLib', 'src', 'main', 'java')

    def get_package_path(self, package_name: str) -> str:
        """
        Convert package name to file path

        Args:
            package_name: Package name (e.g., com.example.app)

        Returns:
            str: File path (e.g., com/example/app)
        """
        return package_name.replace('.', os.sep)

    def create_package_structure(self, package_name: str) -> Tuple[bool, str]:
        """
        Create package directory structure

        Args:
            package_name: Package name (e.g., com.example.app)

        Returns:
            Tuple[bool, str]: (success, error_message or created_path)
        """
        try:
            # Create wxapi subfolder
            package_path = self.get_package_path(package_name)
            wxapi_path = os.path.join(self.java_src_path, package_path, 'wxapi')

            # Create directories
            os.makedirs(wxapi_path, exist_ok=True)

            return True, wxapi_path
        except Exception as e:
            return False, f"Error creating package structure: {e}"

    def find_wx_activity_template(self) -> Tuple[bool, str, List[str]]:
        """
        Find existing WXActivity template files

        Returns:
            Tuple[bool, str, List[str]]: (success, error_message, list of template files)
        """
        try:
            # Search for any existing wxapi folder as template
            for root, dirs, files in os.walk(self.java_src_path):
                if 'wxapi' in dirs:
                    wxapi_dir = os.path.join(root, 'wxapi')
                    entry_file = os.path.join(wxapi_dir, 'WXEntryActivity.java')
                    pay_file = os.path.join(wxapi_dir, 'WXPayEntryActivity.java')

                    if os.path.exists(entry_file) and os.path.exists(pay_file):
                        return True, wxapi_dir, [entry_file, pay_file]

            return False, "No WXActivity template found", []
        except Exception as e:
            return False, f"Error finding template: {e}", []

    def copy_wx_activities(self, package_name: str, target_path: str, template_files: List[str]) -> Tuple[bool, str]:
        """
        Copy WXActivity files and update package declaration

        Args:
            package_name: New package name
            target_path: Target wxapi directory path
            template_files: List of template file paths

        Returns:
            Tuple[bool, str]: (success, error_message)
        """
        try:
            for template_file in template_files:
                filename = os.path.basename(template_file)
                target_file = os.path.join(target_path, filename)

                # Read template file
                with open(template_file, 'r', encoding='utf-8') as f:
                    content = f.read()

                # Replace package declaration
                # Pattern: package com.xxx.yyy.wxapi;
                new_content = re.sub(
                    r'^package\s+[\w.]+\.wxapi;',
                    f'package {package_name}.wxapi;',
                    content,
                    flags=re.MULTILINE
                )

                # Write to target file
                with open(target_file, 'w', encoding='utf-8') as f:
                    f.write(new_content)

            return True, f"Copied {len(template_files)} WXActivity files"
        except Exception as e:
            return False, f"Error copying WXActivity files: {e}"

    def update_constants_app_id(self, app_id: str) -> Tuple[bool, str]:
        """
        Update APP_ID in Constants.java

        Args:
            app_id: New APP_ID value

        Returns:
            Tuple[bool, str]: (success, error_message)
        """
        try:
            constants_file = os.path.join(
                self.java_src_path,
                'com', 'game', 'gold', 'Constants.java'
            )

            if not os.path.exists(constants_file):
                return False, f"Constants.java not found at {constants_file}"

            # Read file
            with open(constants_file, 'r', encoding='utf-8') as f:
                content = f.read()

            # Replace APP_ID
            # Pattern: public static final String APP_ID ="wx...";
            new_content = re.sub(
                r'(public\s+static\s+final\s+String\s+APP_ID\s*=\s*")[^"]*(")',
                rf'\g<1>{app_id}\g<2>',
                content
            )

            # Write back
            with open(constants_file, 'w', encoding='utf-8') as f:
                f.write(new_content)

            return True, f"Updated APP_ID to {app_id}"
        except Exception as e:
            return False, f"Error updating Constants.java: {e}"

    def update_launch_activity_urls(self, user_url: str, privacy_url: str) -> Tuple[bool, str]:
        """
        Update URLs in LaunchMainActivity.java

        Args:
            user_url: User Agreement URL
            privacy_url: Privacy Policy URL

        Returns:
            Tuple[bool, str]: (success, error_message)
        """
        try:
            launch_file = os.path.join(
                self.java_src_path,
                'com', 'game', 'gold', 'LaunchMainActivity.java'
            )

            if not os.path.exists(launch_file):
                return False, f"LaunchMainActivity.java not found at {launch_file}"

            # Read file
            with open(launch_file, 'r', encoding='utf-8') as f:
                content = f.read()

            # Replace first URLSpan (User Agreement)
            # Pattern: URLSpan urlSpan = new URLSpan("https://...");
            content = re.sub(
                r'(URLSpan\s+urlSpan\s*=\s*new\s+URLSpan\s*\(\s*")[^"]*(")',
                rf'\g<1>{user_url}\g<2>',
                content,
                count=1
            )

            # Replace second URLSpan (Privacy Policy)
            # Pattern: URLSpan urlSpan1 = new URLSpan("https://...");
            content = re.sub(
                r'(URLSpan\s+urlSpan1\s*=\s*new\s+URLSpan\s*\(\s*")[^"]*(")',
                rf'\g<1>{privacy_url}\g<2>',
                content,
                count=1
            )

            # Write back
            with open(launch_file, 'w', encoding='utf-8') as f:
                f.write(content)

            return True, f"Updated URLs in LaunchMainActivity"
        except Exception as e:
            return False, f"Error updating LaunchMainActivity.java: {e}"

    def replace_splash_image(self, new_splash_path: str) -> Tuple[bool, str]:
        """
        Replace splash.jpg in drawable folder

        Args:
            new_splash_path: Path to new splash.jpg file

        Returns:
            Tuple[bool, str]: (success, error_message)
        """
        try:
            target_splash = os.path.join(
                self.sdk_path,
                'MySdkLib', 'src', 'main', 'res', 'drawable', 'splash.jpg'
            )

            # Create directory if not exists
            os.makedirs(os.path.dirname(target_splash), exist_ok=True)

            # Copy new splash image
            shutil.copy2(new_splash_path, target_splash)

            return True, f"Replaced splash.jpg"
        except Exception as e:
            return False, f"Error replacing splash image: {e}"


if __name__ == '__main__':
    # Test the package manager module
    manager = PackageManager('/path/to/sdk')

    # Test package path conversion
    print("Package path:", manager.get_package_path('com.example.test'))

    # Test create structure
    success, result = manager.create_package_structure('com.test.app')
    print(f"Create structure: {success}, {result}")
