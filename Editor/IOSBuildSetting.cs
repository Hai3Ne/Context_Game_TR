using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
#if UNITY_IOS
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

public class IOSBuildSetting : MonoBehaviour {
    public const string PROJECT_ROOT = "$(PROJECT_DIR)/";
    public const string IMAGE_XCASSETS_DIRECTORY_NAME = "Unity-iPhone";
    public const string LINKER_FLAG_KEY = "OTHER_LDFLAGS";
    public const string FRAMEWORK_SEARCH_PATHS_KEY = "FRAMEWORK_SEARCH_PATHS";
    public const string LIBRARY_SEARCH_PATHS_KEY = "LIBRARY_SEARCH_PATHS";
    public const string ENABLE_BITCODE_KEY = "ENABLE_BITCODE";
    public const string DEVELOPMENT_TEAM = "DEVELOPMENT_TEAM";
    public const string GCC_ENABLE_CPP_EXCEPTIONS = "GCC_ENABLE_CPP_EXCEPTIONS";
    public const string GCC_ENABLE_CPP_RTTI = "GCC_ENABLE_CPP_RTTI";
    public const string GCC_ENABLE_OBJC_EXCEPTIONS = "GCC_ENABLE_OBJC_EXCEPTIONS";
    public const string INFO_PLIST_NAME = "Info.plist";

    public const string URL_TYPES_KEY = "CFBundleURLTypes";
    public const string URL_TYPE_ROLE_KEY = "CFBundleTypeRole";
    public const string URL_IDENTIFIER_KEY = "CFBundleURLName";
    public const string URL_SCHEMES_KEY = "CFBundleURLSchemes";
    public const string APPLICATION_QUERIES_SCHEMES_KEY = "LSApplicationQueriesSchemes";
    public const string AliPayIdentifierKey = "alipay";
    public const string AliPaySchemesKey = "touchmindfish3d";
    [PostProcessBuild]
    private static void OnPostprocessBuild(BuildTarget buildTarget, string buildPath) {
        string pbxProjPath = PBXProject.GetPBXProjectPath(buildPath);
        PBXProject pbxProject = new PBXProject();
        pbxProject.ReadFromString(File.ReadAllText(pbxProjPath));
        string targetGuid = pbxProject.TargetGuidByName(PBXProject.GetUnityTargetName());

        pbxProject.SetBuildProperty(targetGuid, ENABLE_BITCODE_KEY, "NO");
        //pbxProject.SetBuildProperty(targetGuid, DEVELOPMENT_TEAM, setting.DevelopmentTeam);
        //pbxProject.SetBuildProperty(targetGuid, GCC_ENABLE_CPP_EXCEPTIONS, setting.EnableCppEcceptions ? "YES" : "NO");
        //pbxProject.SetBuildProperty(targetGuid, GCC_ENABLE_CPP_RTTI, setting.EnableCppRtti ? "YES" : "NO");
        //pbxProject.SetBuildProperty(targetGuid, GCC_ENABLE_OBJC_EXCEPTIONS, setting.EnableObjcExceptions ? "YES" : "NO");

        List<string> list = new List<string>();
        list.Add("Libraries/Plugins/IOS/2.iOSGW/Weixin.mm");
        list.Add("Libraries/Plugins/IOS/2.iOSGW/ApiXml.mm");
        list.Add("Libraries/Plugins/IOS/2.iOSGW/payRequsestHandler.mm");
        list.Add("Libraries/Plugins/IOS/2.iOSGW/SKProduct+LocalizedPrice.m");
        list.Add("Libraries/Plugins/IOS/2.iOSGW/InAppPurchaseManager.m");
        foreach (var item in list) {
            string file = pbxProject.FindFileGuidByProjectPath(item);
            if (string.IsNullOrEmpty(file)) {
                continue;
            }
            var flags = pbxProject.GetCompileFlagsForFile(targetGuid, file);
            flags.Add("-fno-objc-arc");
            pbxProject.SetCompileFlagsForFile(targetGuid, file, flags);
        }

        pbxProject.AddFrameworkToProject(targetGuid, "StoreKit.framework", false);
        pbxProject.AddFrameworkToProject(targetGuid, "CoreTelephony.framework", false);
        pbxProject.AddFrameworkToProject(targetGuid, "Security.framework", false);
        //添加lib
        AddLibToProject(pbxProject, targetGuid, "libz.1.2.5.tbd");
        AddLibToProject(pbxProject, targetGuid, "libsqlite3.0.tbd");

        list.Clear();
        list.Add("-ObjC");
        pbxProject.UpdateBuildProperty(targetGuid, LINKER_FLAG_KEY, list, null);
        File.WriteAllText(pbxProjPath, pbxProject.WriteToString());


        string plistPath = Path.Combine(buildPath, INFO_PLIST_NAME);
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        //<key>CFBundleURLTypes</key>
        //<array>
        //    <dict>
        //        <key>CFBundleTypeRole</key>
        //        <string>Editor</string>
        //        <key>CFBundleURLName</key>
        //        <string>com.touchmind.fish3d</string>
        //        <key>CFBundleURLSchemes</key>
        //        <array>
        //            <string>wxcd7adcbc7a735069</string>
        //        </array>
        //    </dict>
        //</array>
        PlistElementArray urlTypes;
        if (plist.root.values.ContainsKey(URL_TYPES_KEY)) {
            urlTypes = plist.root[URL_TYPES_KEY].AsArray();
        } else {
            urlTypes = plist.root.CreateArray(URL_TYPES_KEY);
        }
        PlistElementDict itmeDict = urlTypes.AddDict();
        itmeDict.SetString(URL_TYPE_ROLE_KEY, "Editor");
        itmeDict.SetString(URL_IDENTIFIER_KEY, PlayerSettings.applicationIdentifier);// "com.touchmind.fish3d");
        PlistElementArray schemesArray = itmeDict.CreateArray(URL_SCHEMES_KEY);
        if (itmeDict.values.ContainsKey(URL_SCHEMES_KEY)) {
            schemesArray = itmeDict[URL_SCHEMES_KEY].AsArray();
        } else {
            schemesArray = itmeDict.CreateArray(URL_SCHEMES_KEY);
        }
        schemesArray.AddString("wxcd7adcbc7a735069");

        PlistElementDict itmeDict_AliPay = urlTypes.AddDict();
        itmeDict_AliPay.SetString(URL_TYPE_ROLE_KEY, "Editor");
        itmeDict_AliPay.SetString(URL_IDENTIFIER_KEY, AliPayIdentifierKey);
        PlistElementArray schemesArray_AliPay = itmeDict_AliPay.CreateArray(URL_SCHEMES_KEY);
        if (itmeDict_AliPay.values.ContainsKey(URL_SCHEMES_KEY))
            schemesArray_AliPay = itmeDict_AliPay[URL_SCHEMES_KEY].AsArray();
        else
            schemesArray_AliPay = itmeDict_AliPay.CreateArray(URL_SCHEMES_KEY);

        schemesArray_AliPay.AddString(AliPaySchemesKey);

        plist.WriteToFile(plistPath);


        Debug.LogError("打包完成：" + PlayerSettings.applicationIdentifier);
    }//添加lib方法
    static void AddLibToProject(PBXProject inst, string targetGuid, string lib)
    {
        string fileGuid = inst.AddFile("usr/lib/" + lib, "Frameworks/" + lib, PBXSourceTree.Sdk);
        inst.AddFileToBuild(targetGuid, fileGuid);
    }
}
#endif