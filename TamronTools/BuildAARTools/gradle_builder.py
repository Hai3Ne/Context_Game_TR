#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Gradle Builder
Handles AAR building with Gradle
"""

import os
import platform
import subprocess
import shutil
from typing import Tuple, Callable, Optional
import glob


class GradleBuilder:
    """Builds AAR using Gradle wrapper"""

    def __init__(self, project_path: str):
        """
        Initialize GradleBuilder

        Args:
            project_path: Path to Android project root (containing gradlew)
        """
        self.project_path = os.path.abspath(project_path)
        self.gradlew_path = self._get_gradlew_path()

    def _get_gradlew_path(self) -> str:
        """Get path to gradlew executable"""
        if platform.system() == 'Windows':
            gradlew = os.path.join(self.project_path, 'gradlew.bat')
        else:
            gradlew = os.path.join(self.project_path, 'gradlew')

        return gradlew

    def check_gradle_available(self) -> Tuple[bool, str]:
        """
        Check if Gradle wrapper is available

        Returns:
            Tuple[bool, str]: (is_available, error_message)
        """
        if not os.path.exists(self.gradlew_path):
            return False, f"Gradle wrapper not found at {self.gradlew_path}"

        # Make gradlew executable on Unix-like systems
        if platform.system() != 'Windows':
            try:
                os.chmod(self.gradlew_path, 0o755)
            except Exception as e:
                return False, f"Cannot make gradlew executable: {e}"

        return True, ""

    def clean_build(self, log_callback: Optional[Callable[[str], None]] = None) -> Tuple[bool, str]:
        """
        Clean build directories

        Args:
            log_callback: Optional callback function for logging

        Returns:
            Tuple[bool, str]: (success, error_message)
        """
        try:
            if log_callback:
                log_callback("[INFO] Cleaning build directories...")

            # Run ./gradlew clean
            cmd = [self.gradlew_path, 'clean']

            process = subprocess.Popen(
                cmd,
                cwd=self.project_path,
                stdout=subprocess.PIPE,
                stderr=subprocess.STDOUT,
                universal_newlines=True
            )

            # Read output line by line
            for line in process.stdout:
                if log_callback:
                    log_callback(line.rstrip())

            process.wait()

            if process.returncode != 0:
                return False, f"Clean failed with exit code {process.returncode}"

            if log_callback:
                log_callback("[SUCCESS] Clean completed")

            return True, ""

        except Exception as e:
            return False, f"Error during clean: {e}"

    def build_aar(self, log_callback: Optional[Callable[[str], None]] = None) -> Tuple[bool, str]:
        """
        Build AAR file using Gradle

        Args:
            log_callback: Optional callback function for logging output

        Returns:
            Tuple[bool, str]: (success, error_message)
        """
        try:
            # Check Gradle availability
            available, error = self.check_gradle_available()
            if not available:
                return False, error

            if log_callback:
                log_callback("[INFO] Starting Gradle build...")
                log_callback(f"[INFO] Project path: {self.project_path}")
                log_callback(f"[INFO] Using: {self.gradlew_path}")

            # Run ./gradlew :MySdkLib:assembleRelease
            cmd = [self.gradlew_path, ':MySdkLib:assembleRelease']

            if log_callback:
                log_callback(f"[INFO] Command: {' '.join(cmd)}")

            # Execute Gradle
            process = subprocess.Popen(
                cmd,
                cwd=self.project_path,
                stdout=subprocess.PIPE,
                stderr=subprocess.STDOUT,
                universal_newlines=True,
                bufsize=1
            )

            # Read output line by line and send to callback
            for line in process.stdout:
                if log_callback:
                    log_callback(line.rstrip())

            # Wait for process to complete
            process.wait()

            if process.returncode != 0:
                error_msg = f"Gradle build failed with exit code {process.returncode}"
                if log_callback:
                    log_callback(f"[ERROR] {error_msg}")
                return False, error_msg

            if log_callback:
                log_callback("[SUCCESS] Gradle build completed successfully!")

            return True, ""

        except Exception as e:
            error_msg = f"Error executing Gradle: {e}"
            if log_callback:
                log_callback(f"[ERROR] {error_msg}")
            return False, error_msg

    def find_output_aar(self) -> Tuple[bool, str]:
        """
        Find the generated AAR file

        Returns:
            Tuple[bool, str]: (found, aar_file_path or error_message)
        """
        try:
            # Search for AAR in build/outputs/aar/
            aar_dir = os.path.join(self.project_path, 'MySdkLib', 'build', 'outputs', 'aar')

            if not os.path.exists(aar_dir):
                return False, f"AAR output directory not found: {aar_dir}"

            # Find AAR files
            aar_files = glob.glob(os.path.join(aar_dir, '*.aar'))

            if not aar_files:
                return False, f"No AAR files found in {aar_dir}"

            # Return the first AAR file (usually MySdkLib-release.aar)
            # Prefer -release.aar over -debug.aar
            release_aars = [f for f in aar_files if 'release' in os.path.basename(f).lower()]
            if release_aars:
                return True, release_aars[0]

            return True, aar_files[0]

        except Exception as e:
            return False, f"Error finding AAR file: {e}"

    def copy_aar_to_destination(self, destination: str, log_callback: Optional[Callable[[str], None]] = None) -> Tuple[bool, str]:
        """
        Copy generated AAR to destination path

        Args:
            destination: Destination path (can be directory or file path)
            log_callback: Optional callback for logging

        Returns:
            Tuple[bool, str]: (success, error_message or destination_path)
        """
        try:
            # Find AAR file
            found, aar_path = self.find_output_aar()
            if not found:
                return False, aar_path

            if log_callback:
                log_callback(f"[INFO] Found AAR: {aar_path}")

            # Determine destination file path
            if os.path.isdir(destination):
                dest_file = os.path.join(destination, os.path.basename(aar_path))
            else:
                dest_file = destination

            # Create destination directory if not exists
            os.makedirs(os.path.dirname(dest_file), exist_ok=True)

            # Copy AAR file
            shutil.copy2(aar_path, dest_file)

            if log_callback:
                log_callback(f"[SUCCESS] Copied AAR to: {dest_file}")

            return True, dest_file

        except Exception as e:
            error_msg = f"Error copying AAR: {e}"
            if log_callback:
                log_callback(f"[ERROR] {error_msg}")
            return False, error_msg


if __name__ == '__main__':
    # Test the gradle builder module
    builder = GradleBuilder('/path/to/project')

    # Check Gradle
    available, error = builder.check_gradle_available()
    print(f"Gradle available: {available}, Error: {error}")

    # Build AAR (test without actually running)
    print(f"Gradle path: {builder.gradlew_path}")
