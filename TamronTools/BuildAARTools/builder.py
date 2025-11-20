#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
SDK AAR Build Tool - Main GUI Application
Automates SDK AAR building process for Unity projects
"""

import os
import sys
import tkinter as tk
from tkinter import ttk, filedialog, messagebox, scrolledtext
import threading
import shutil
from datetime import datetime

# Import our modules
from sdk_config import SdkConfig
from package_manager import PackageManager
from gradle_builder import GradleBuilder


class BuilderGUI:
    """Main GUI application for SDK AAR Builder"""

    def __init__(self):
        """Initialize the GUI"""
        self.window = tk.Tk()
        self.window.title("SDK AAR Build Tool - TamronTools")
        self.window.geometry("900x800")
        self.window.resizable(True, True)

        self.config = SdkConfig()
        self.is_building = False

        # Setup UI
        self.setup_ui()

        # Load default config if exists
        self.load_default_config()

    def setup_ui(self):
        """Setup the user interface"""

        # Main container with padding
        main_frame = ttk.Frame(self.window, padding="10")
        main_frame.grid(row=0, column=0, sticky=(tk.W, tk.E, tk.N, tk.S))

        # Configure grid weights
        self.window.columnconfigure(0, weight=1)
        self.window.rowconfigure(0, weight=1)
        main_frame.columnconfigure(1, weight=1)

        # Title
        title_label = ttk.Label(
            main_frame,
            text="SDK AAR Build Tool",
            font=('Arial', 16, 'bold')
        )
        title_label.grid(row=0, column=0, columnspan=3, pady=(0, 20))

        row = 1

        # SDK Path
        ttk.Label(main_frame, text="SDK Path:").grid(row=row, column=0, sticky=tk.W, pady=5)
        self.sdk_path_var = tk.StringVar(value="ClientHW2024V11/MySdktuiguang")
        ttk.Entry(main_frame, textvariable=self.sdk_path_var, width=50).grid(row=row, column=1, sticky=(tk.W, tk.E), pady=5)
        ttk.Button(main_frame, text="Browse", command=self.browse_sdk_path).grid(row=row, column=2, padx=(5, 0), pady=5)
        row += 1

        # APP ID
        ttk.Label(main_frame, text="APP ID:").grid(row=row, column=0, sticky=tk.W, pady=5)
        self.app_id_var = tk.StringVar(value="wx4f16c34621be4aff")
        ttk.Entry(main_frame, textvariable=self.app_id_var, width=50).grid(row=row, column=1, sticky=(tk.W, tk.E), pady=5)
        ttk.Label(main_frame, text="(WeChat)").grid(row=row, column=2, padx=(5, 0), pady=5)
        row += 1

        # Package Name
        ttk.Label(main_frame, text="Package Name:").grid(row=row, column=0, sticky=tk.W, pady=5)
        self.package_name_var = tk.StringVar(value="com.dwzy.xkmm")
        ttk.Entry(main_frame, textvariable=self.package_name_var, width=50).grid(row=row, column=1, sticky=(tk.W, tk.E), pady=5)
        ttk.Label(main_frame, text="(e.g., com.example.app)").grid(row=row, column=2, padx=(5, 0), pady=5)
        row += 1

        # User Agreement URL
        ttk.Label(main_frame, text="User Agreement:").grid(row=row, column=0, sticky=tk.W, pady=5)
        self.user_url_var = tk.StringVar(value="https://apiva1.lywl2025.com/user.html")
        ttk.Entry(main_frame, textvariable=self.user_url_var, width=50).grid(row=row, column=1, sticky=(tk.W, tk.E), pady=5)
        ttk.Label(main_frame, text="(urlSpan)").grid(row=row, column=2, padx=(5, 0), pady=5)
        row += 1

        # Privacy Policy URL
        ttk.Label(main_frame, text="Privacy Policy:").grid(row=row, column=0, sticky=tk.W, pady=5)
        self.privacy_url_var = tk.StringVar(value="https://apiva1.lywl2025.com/yinsi.html")
        ttk.Entry(main_frame, textvariable=self.privacy_url_var, width=50).grid(row=row, column=1, sticky=(tk.W, tk.E), pady=5)
        ttk.Label(main_frame, text="(urlSpan1)").grid(row=row, column=2, padx=(5, 0), pady=5)
        row += 1

        # Splash Image
        ttk.Label(main_frame, text="Splash Image:").grid(row=row, column=0, sticky=tk.W, pady=5)
        self.splash_image_var = tk.StringVar()
        ttk.Entry(main_frame, textvariable=self.splash_image_var, width=50).grid(row=row, column=1, sticky=(tk.W, tk.E), pady=5)
        ttk.Button(main_frame, text="Browse", command=self.browse_splash_image).grid(row=row, column=2, padx=(5, 0), pady=5)
        row += 1

        # Unity Output Path (optional)
        ttk.Label(main_frame, text="Unity Output:").grid(row=row, column=0, sticky=tk.W, pady=5)
        self.unity_output_var = tk.StringVar(value="Assets/Plugins/Android/libs")
        ttk.Entry(main_frame, textvariable=self.unity_output_var, width=50).grid(row=row, column=1, sticky=(tk.W, tk.E), pady=5)
        ttk.Button(main_frame, text="Browse", command=self.browse_unity_output).grid(row=row, column=2, padx=(5, 0), pady=5)
        row += 1

        # Separator
        ttk.Separator(main_frame, orient='horizontal').grid(row=row, column=0, columnspan=3, sticky=(tk.W, tk.E), pady=15)
        row += 1

        # Buttons frame
        button_frame = ttk.Frame(main_frame)
        button_frame.grid(row=row, column=0, columnspan=3, pady=10)

        self.build_button = ttk.Button(
            button_frame,
            text="🔨 Build AAR",
            command=self.start_build_thread,
            width=20
        )
        self.build_button.pack(side=tk.LEFT, padx=5)

        ttk.Button(
            button_frame,
            text="💾 Save Config",
            command=self.save_config,
            width=15
        ).pack(side=tk.LEFT, padx=5)

        ttk.Button(
            button_frame,
            text="📂 Load Config",
            command=self.load_config,
            width=15
        ).pack(side=tk.LEFT, padx=5)

        ttk.Button(
            button_frame,
            text="❌ Exit",
            command=self.window.quit,
            width=10
        ).pack(side=tk.LEFT, padx=5)

        row += 1

        # Progress bar
        self.progress_var = tk.DoubleVar()
        self.progress_bar = ttk.Progressbar(
            main_frame,
            variable=self.progress_var,
            maximum=100,
            mode='indeterminate'
        )
        self.progress_bar.grid(row=row, column=0, columnspan=3, sticky=(tk.W, tk.E), pady=10)
        row += 1

        # Log window
        ttk.Label(main_frame, text="Build Log:", font=('Arial', 10, 'bold')).grid(row=row, column=0, sticky=tk.W, pady=(10, 5))
        row += 1

        # Scrolled text for log
        self.log_text = scrolledtext.ScrolledText(
            main_frame,
            width=80,
            height=20,
            wrap=tk.WORD,
            font=('Courier', 9)
        )
        self.log_text.grid(row=row, column=0, columnspan=3, sticky=(tk.W, tk.E, tk.N, tk.S), pady=5)
        main_frame.rowconfigure(row, weight=1)

        # Configure text tags for colored output
        self.log_text.tag_config("INFO", foreground="blue")
        self.log_text.tag_config("SUCCESS", foreground="green", font=('Courier', 9, 'bold'))
        self.log_text.tag_config("ERROR", foreground="red", font=('Courier', 9, 'bold'))
        self.log_text.tag_config("WARNING", foreground="orange")

    def log(self, message: str, level: str = "INFO"):
        """
        Add message to log window

        Args:
            message: Message to log
            level: Log level (INFO, SUCCESS, ERROR, WARNING)
        """
        timestamp = datetime.now().strftime("%H:%M:%S")
        log_line = f"[{timestamp}] {message}\n"

        self.log_text.insert(tk.END, log_line, level)
        self.log_text.see(tk.END)
        self.window.update_idletasks()

    def browse_sdk_path(self):
        """Browse for SDK path"""
        path = filedialog.askdirectory(title="Select SDK Path")
        if path:
            self.sdk_path_var.set(path)

    def browse_splash_image(self):
        """Browse for splash image"""
        path = filedialog.askopenfilename(
            title="Select Splash Image",
            filetypes=[("JPEG files", "*.jpg *.jpeg"), ("All files", "*.*")]
        )
        if path:
            self.splash_image_var.set(path)

    def browse_unity_output(self):
        """Browse for Unity output path"""
        path = filedialog.askdirectory(title="Select Unity Output Directory")
        if path:
            self.unity_output_var.set(path)

    def load_default_config(self):
        """Load default config from template"""
        template_path = os.path.join(
            os.path.dirname(__file__),
            'config',
            'template_config.json'
        )
        if os.path.exists(template_path):
            self.config.load_from_json(template_path)
            self.update_ui_from_config()

    def update_ui_from_config(self):
        """Update UI fields from config"""
        self.sdk_path_var.set(self.config.sdk_path)
        self.app_id_var.set(self.config.app_id)
        self.package_name_var.set(self.config.package_name)
        self.user_url_var.set(self.config.user_agreement_url)
        self.privacy_url_var.set(self.config.privacy_policy_url)
        self.splash_image_var.set(self.config.splash_image_path)
        self.unity_output_var.set(self.config.unity_output_path)

    def update_config_from_ui(self):
        """Update config from UI fields"""
        self.config.sdk_path = self.sdk_path_var.get()
        self.config.app_id = self.app_id_var.get()
        self.config.package_name = self.package_name_var.get()
        self.config.user_agreement_url = self.user_url_var.get()
        self.config.privacy_policy_url = self.privacy_url_var.get()
        self.config.splash_image_path = self.splash_image_var.get()
        self.config.unity_output_path = self.unity_output_var.get()

    def save_config(self):
        """Save current config to file"""
        file_path = filedialog.asksaveasfilename(
            title="Save Config",
            defaultextension=".json",
            filetypes=[("JSON files", "*.json"), ("All files", "*.*")],
            initialdir=os.path.join(os.path.dirname(__file__), 'config')
        )
        if file_path:
            self.update_config_from_ui()
            if self.config.save_to_json(file_path):
                self.log(f"Config saved to {file_path}", "SUCCESS")
                messagebox.showinfo("Success", "Config saved successfully!")
            else:
                self.log(f"Failed to save config", "ERROR")
                messagebox.showerror("Error", "Failed to save config!")

    def load_config(self):
        """Load config from file"""
        file_path = filedialog.askopenfilename(
            title="Load Config",
            filetypes=[("JSON files", "*.json"), ("All files", "*.*")],
            initialdir=os.path.join(os.path.dirname(__file__), 'config')
        )
        if file_path:
            if self.config.load_from_json(file_path):
                self.update_ui_from_config()
                self.log(f"Config loaded from {file_path}", "SUCCESS")
                messagebox.showinfo("Success", "Config loaded successfully!")
            else:
                self.log(f"Failed to load config", "ERROR")
                messagebox.showerror("Error", "Failed to load config!")

    def start_build_thread(self):
        """Start build process in a separate thread"""
        if self.is_building:
            messagebox.showwarning("Warning", "Build is already in progress!")
            return

        # Clear log
        self.log_text.delete(1.0, tk.END)

        # Start build in thread
        build_thread = threading.Thread(target=self.build_process)
        build_thread.daemon = True
        build_thread.start()

    def build_process(self):
        """Main build process"""
        try:
            self.is_building = True
            self.build_button.config(state='disabled')
            self.progress_bar.start(10)

            self.log("=" * 80, "INFO")
            self.log("SDK AAR BUILD TOOL - Starting Build Process", "INFO")
            self.log("=" * 80, "INFO")

            # Update config from UI
            self.update_config_from_ui()

            # Step 1: Validate config
            self.log("\n[STEP 1/8] Validating configuration...", "INFO")
            is_valid, error = self.config.validate()
            if not is_valid:
                self.log(f"Validation failed: {error}", "ERROR")
                messagebox.showerror("Validation Error", error)
                return

            self.log("Configuration valid!", "SUCCESS")

            # Step 2: Create backup
            self.log("\n[STEP 2/8] Creating backup of source...", "INFO")
            backup_path = self.config.sdk_path + "_backup_" + datetime.now().strftime("%Y%m%d_%H%M%S")
            try:
                shutil.copytree(self.config.sdk_path, backup_path)
                self.log(f"Backup created at: {backup_path}", "SUCCESS")
            except Exception as e:
                self.log(f"Backup failed: {e}", "ERROR")
                messagebox.showerror("Backup Error", f"Failed to create backup: {e}")
                return

            # Step 3: Initialize PackageManager
            self.log("\n[STEP 3/8] Initializing package manager...", "INFO")
            pkg_manager = PackageManager(self.config.sdk_path)
            self.log("Package manager initialized", "SUCCESS")

            # Step 4: Update APP_ID
            self.log("\n[STEP 4/8] Updating APP_ID...", "INFO")
            success, msg = pkg_manager.update_constants_app_id(self.config.app_id)
            if not success:
                self.log(msg, "ERROR")
                self._restore_backup(backup_path)
                return
            self.log(msg, "SUCCESS")

            # Step 5: Update URLs
            self.log("\n[STEP 5/8] Updating URLs...", "INFO")
            success, msg = pkg_manager.update_launch_activity_urls(
                self.config.user_agreement_url,
                self.config.privacy_policy_url
            )
            if not success:
                self.log(msg, "ERROR")
                self._restore_backup(backup_path)
                return
            self.log(msg, "SUCCESS")

            # Step 6: Update package and WXActivity
            self.log("\n[STEP 6/8] Creating new package structure...", "INFO")
            success, wxapi_path = pkg_manager.create_package_structure(self.config.package_name)
            if not success:
                self.log(wxapi_path, "ERROR")
                self._restore_backup(backup_path)
                return
            self.log(f"Created: {wxapi_path}", "SUCCESS")

            # Find and copy WXActivity files
            self.log("Finding WXActivity template...", "INFO")
            success, template_path, template_files = pkg_manager.find_wx_activity_template()
            if not success:
                self.log(template_path, "ERROR")
                self._restore_backup(backup_path)
                return
            self.log(f"Found template at: {template_path}", "SUCCESS")

            self.log("Copying WXActivity files...", "INFO")
            success, msg = pkg_manager.copy_wx_activities(
                self.config.package_name,
                wxapi_path,
                template_files
            )
            if not success:
                self.log(msg, "ERROR")
                self._restore_backup(backup_path)
                return
            self.log(msg, "SUCCESS")

            # Step 7: Replace splash image
            self.log("\n[STEP 7/8] Replacing splash image...", "INFO")
            success, msg = pkg_manager.replace_splash_image(self.config.splash_image_path)
            if not success:
                self.log(msg, "ERROR")
                self._restore_backup(backup_path)
                return
            self.log(msg, "SUCCESS")

            # Step 8: Build AAR with Gradle
            self.log("\n[STEP 8/8] Building AAR with Gradle...", "INFO")
            gradle_builder = GradleBuilder(self.config.sdk_path)

            # Clean first
            self.log("Running Gradle clean...", "INFO")
            success, msg = gradle_builder.clean_build(log_callback=lambda m: self.log(m, "INFO"))
            if not success:
                self.log(msg, "ERROR")
                self._restore_backup(backup_path)
                return

            # Build AAR
            self.log("Building AAR (this may take a few minutes)...", "INFO")
            success, msg = gradle_builder.build_aar(log_callback=lambda m: self.log(m, "INFO"))
            if not success:
                self.log(msg, "ERROR")
                self._restore_backup(backup_path)
                return

            # Find output AAR
            found, aar_path = gradle_builder.find_output_aar()
            if not found:
                self.log(aar_path, "ERROR")
                self._restore_backup(backup_path)
                return

            self.log(f"AAR built successfully: {aar_path}", "SUCCESS")

            # Copy to Unity if path specified
            if self.config.unity_output_path:
                self.log(f"\nCopying AAR to Unity path...", "INFO")
                success, dest = gradle_builder.copy_aar_to_destination(
                    self.config.unity_output_path,
                    log_callback=lambda m: self.log(m, "INFO")
                )
                if success:
                    self.log(f"AAR copied to: {dest}", "SUCCESS")
                else:
                    self.log(f"Warning: {dest}", "WARNING")

            # Restore source
            self.log("\nRestoring original source...", "INFO")
            self._restore_backup(backup_path, delete_backup=True)

            # Success!
            self.log("\n" + "=" * 80, "SUCCESS")
            self.log("BUILD COMPLETED SUCCESSFULLY!", "SUCCESS")
            self.log("=" * 80, "SUCCESS")

            messagebox.showinfo("Success", "AAR built successfully!")

        except Exception as e:
            self.log(f"\nUnexpected error: {e}", "ERROR")
            messagebox.showerror("Error", f"Build failed: {e}")

        finally:
            self.is_building = False
            self.build_button.config(state='normal')
            self.progress_bar.stop()

    def _restore_backup(self, backup_path: str, delete_backup: bool = False):
        """Restore from backup"""
        try:
            if os.path.exists(backup_path):
                # Remove modified source
                if os.path.exists(self.config.sdk_path):
                    shutil.rmtree(self.config.sdk_path)

                # Restore from backup
                shutil.copytree(backup_path, self.config.sdk_path)
                self.log("Source restored from backup", "SUCCESS")

                # Delete backup if requested
                if delete_backup:
                    shutil.rmtree(backup_path)
                    self.log("Backup removed", "INFO")
        except Exception as e:
            self.log(f"Error restoring backup: {e}", "ERROR")

    def run(self):
        """Run the GUI application"""
        self.log("SDK AAR Build Tool loaded successfully!", "SUCCESS")
        self.log("Please configure your settings and click 'Build AAR' to start.", "INFO")
        self.window.mainloop()


def main():
    """Main entry point"""
    app = BuilderGUI()
    app.run()


if __name__ == '__main__':
    main()
