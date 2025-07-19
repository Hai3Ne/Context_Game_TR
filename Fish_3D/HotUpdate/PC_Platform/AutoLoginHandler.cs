using System;
using System.IO;

#if FISH_3D_ENTRY_PC
public class AutoLoginHandler
{
    private static readonly string LOGIN_FILE_PATH = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "Low",
        "Tamron", "1378捕鱼", "login_data.txt"
    );

    private static bool mAutoLoginInProgress = false;
    private static string mCurrentAccount = "";
    private static string mCurrentPassword = "";

    public static bool TryAutoLogin()
    {
        try
        {
            if (mAutoLoginInProgress)
            {
                LogMgr.Log("Auto login already in progress");
                return false;
            }

            if (!File.Exists(LOGIN_FILE_PATH))
            {
                LogMgr.Log("Auto login file not found: " + LOGIN_FILE_PATH);
                return false;
            }

            string[] loginData = File.ReadAllLines(LOGIN_FILE_PATH);
            if (loginData.Length < 2)
            {
                LogMgr.Log("Invalid login data format");
                return false;
            }

            string account = loginData[0].Trim();
            string password = loginData[1].Trim();

            if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
            {
                LogMgr.Log("Empty account or password in login file");
                return false;
            }
            LogMgr.Log("Auto login attempt for account: " + account);
            mCurrentAccount = account;
            mCurrentPassword = password;
            mAutoLoginInProgress = true;
            PerformAutoLogin(account, password);
            return true;
        }
        catch (Exception ex)
        {
            LogMgr.Log("Auto login error: " + ex.Message);
            mAutoLoginInProgress = false;
            return false;
        }
    }

    // private static void RegisterAutoLoginEvents()
    // {
    //     NetEventManager.RegisterEvent(NetCmdType.SUB_GP_LOGON_SUCCESS, OnAutoLoginSuccess);
    //     NetEventManager.RegisterEvent(NetCmdType.SUB_GP_LOGON_FAILURE, OnAutoLoginFailure);
    //     LogMgr.Log("Auto login events registered");
    // }
    //
    // private static void UnregisterAutoLoginEvents()
    // {
    //     NetEventManager.UnRegisterEvent(NetCmdType.SUB_GP_LOGON_SUCCESS, OnAutoLoginSuccess);
    //     NetEventManager.UnRegisterEvent(NetCmdType.SUB_GP_LOGON_FAILURE, OnAutoLoginFailure);
    //     LogMgr.Log("Auto login events unregistered");
    // }

    private static void PerformAutoLogin(string account, string password)
    {
        TimeManager.DelayExec(0.1f, () => {
            HallHandle.LoginPassword = password;
            string MachineID = GameUtils.GetMachineID().ToUpper();
        
            CMD_GP_LogonAccounts logonReq = new CMD_GP_LogonAccounts();
            logonReq.SetCmdType(NetCmdType.SUB_GP_LOGON_ACCOUNTS);
            logonReq.PlazaVersion = GameConfig.PlazaVersion;
            logonReq.ClientAddress = GameConfig.ClientAddr;
            logonReq.MacID = "";
            logonReq.MachineID = MachineID;
            logonReq.MachineIDEx = GameUtils.CalMD5(MachineID);
            logonReq.Password = GameUtils.CalMD5(password);
            logonReq.Accounts = account;
            logonReq.ValidateFlags = 1;
            logonReq.CheckParam = ZJEncrypt.MapEncrypt(MachineID, 33);
            logonReq.GameID = GameConfig.ClientGameID;
            logonReq.PlatformID = GameConfig.PayPlatformID;
            HttpServer.Instance.Send(NetCmdType.SUB_GP_LOGON_ACCOUNTS, logonReq);
            LogMgr.Log("Auto login request sent for: " + account);
        });
    }

    private static void OnAutoLoginSuccess(NetCmdType type, NetCmdPack pack)
    {
        if (!mAutoLoginInProgress) return;

        try
        {
            CMD_GP_LogonSuccess resp = pack.ToObj<CMD_GP_LogonSuccess>();
            MainEntrace.Instance.HideLoad();
            
            LogMgr.Log("Auto login success for: " + resp.Accounts);
            
            ResVersionManager.Clear();
            
            string resp_account = resp.Accounts.ToUpper();
            string input_account = mCurrentAccount.ToUpper();
            if (resp_account.Equals(input_account))
            {
                HallHandle.LoginPassword = mCurrentPassword;
                SaveLoginData(mCurrentAccount, mCurrentPassword);
            }
            HallHandle.QueryIndividualInfo();
            CleanupAutoLogin();
            GoToHall();
        }
        catch (Exception ex)
        {
            LogMgr.Log("Auto login success handler error: " + ex.Message);
            HandleAutoLoginFailure("Login Failed");
        }
    }

    private static void OnAutoLoginFailure(NetCmdType type, NetCmdPack pack)
    {
        if (!mAutoLoginInProgress) return;

        try
        {
            CMD_GP_LogonFailure resp = pack.ToObj<CMD_GP_LogonFailure>();
            LogMgr.Log("Auto login failed: " + resp.DescribeString);
            
            HandleAutoLoginFailure(resp.DescribeString);
        }
        catch (Exception ex)
        {
            LogMgr.Log("Auto login failure handler error: " + ex.Message);
            HandleAutoLoginFailure("Login Fail");
        }
    }

    private static void HandleAutoLoginFailure(string errorMessage)
    {
        MainEntrace.Instance.HideLoad();
        CleanupAutoLogin();
        SystemMessageMgr.Instance.DialogShow("Login Fail: " + errorMessage, () => {
            MainEntrace.Instance.OpenLogin();
        });
    }

    private static void CleanupAutoLogin()
    {
        mAutoLoginInProgress = false;
        mCurrentAccount = "";
        mCurrentPassword = "";
    }

    private static void GoToHall()
    {

        SignManager.__init_tick = false;
        SignManager.IsSign = false;
        HttpServer.Instance.Send(NetCmdType.SUB_GP_QUERYWEEKSIGN, new CMD_GP_QueryWeekSign
        {
            UserID = HallHandle.UserID,
        });

        GameSceneManager.BackToHall(GameEnum.None);
    }

    private static void SaveLoginData(string account, string password)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LOGIN_FILE_PATH));
            File.WriteAllLines(LOGIN_FILE_PATH, new string[] { account, password });
            LogMgr.Log("Login data saved successfully");
        }
        catch (Exception ex)
        {
            LogMgr.Log("Failed to save login data: " + ex.Message);
        }
    }
}
#endif