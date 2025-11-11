using SEZSJ;
using System.Collections;
using UnityEngine;
namespace HotUpdate
{
    public class LoginTestHelper : MonoBehaviour
    {
        public bool simulateSuccessLogin = true;
        public float responseDelay = 1.5f;
        public int testResultCode = 0;
        
        [Header("Test Account")]
        public string testPhone = "12345678901";
        public string testPassword = "test1234";
        private LoginDaContaPanel loginPanel;

        private bool isInitialized = false;

 

        private void Start()

        {
            loginPanel = GetComponent<LoginDaContaPanel>();
            if (loginPanel == null)
            {
                return;

            }
            isInitialized = true;

        }
        public void TestLoginResponse()
        {
            if (!isInitialized || !simulateSuccessLogin)
            {
                return;
            }
            StartCoroutine(SimulateLoginResponse());
        }

        private IEnumerator SimulateLoginResponse()
        {
            yield return new WaitForSeconds(responseDelay);
            MsgData_sLogin fakeData = new MsgData_sLogin();
            fakeData.ResultCode = testResultCode;
            fakeData.TxCode = 0;
            fakeData.AccountGUID = 123456789;
            fakeData.GUID = 987654321;
            fakeData.ServerID = 101;
            fakeData.RoleCount = 1;
            fakeData.ServerTime = System.DateTime.Now.Ticks;
            // LoginCtrl.Instance.LoginRecevied(fakeData);
            var loginPanel = MainPanelMgr.Instance.GetPanel("LoginPanel").GetComponent<LoginPanel>();
            if (loginPanel != null)
            {
                loginPanel.OnComece1Btn(); 
            }
        }
        public void TestValidation()
        {
            Debug.Log("<color=yellow>[LoginTestHelper] Testing validation...</color>");
            TestCase[] testCases = new TestCase[]
            {
                new TestCase("12345678901", "password123", true, "Valid phone + password"),

                new TestCase("123", "password123", false, "Invalid phone (too short)"),

                new TestCase("12345678901", "pass", false, "Invalid password (too short)"),

                new TestCase("12345678901", "thispasswordistoolong123", false, "Invalid password (too long)"),

                new TestCase("", "password123", false, "Empty phone"),

                new TestCase("12345678901", "", false, "Empty password"),
            };
            foreach (var testCase in testCases)
            {
                bool result = ValidateInput(testCase.phone, testCase.password);
                string status = result == testCase.shouldPass ? "✓ PASS" : "✗ FAIL";
                Debug.Log($"<color={(result == testCase.shouldPass ? "green" : "red")}>{status}</color> - {testCase.description}");

            }
        }
        private bool ValidateInput(string phone, string password)
        {
            if (string.IsNullOrEmpty(phone)) return false;
            if (phone.Length != 11) return false;
            if (string.IsNullOrEmpty(password)) return false;
            if (password.Length < 8 || password.Length > 16) return false;
            if (ToolUtil.CheckPwd(password)) return false;
            return true;
        }
        private class TestCase
        {
            public string phone;
            public string password;
            public bool shouldPass;
            public string description;
            public TestCase(string phone, string password, bool shouldPass, string description)
            {
                this.phone = phone;
                this.password = password;
                this.shouldPass = shouldPass;
                this.description = description;
            }
        }
        
#if UNITY_EDITOR
        [ContextMenu("Test Login Success (ResultCode = 0)")]
        private void TestLoginSuccess()
        {
            testResultCode = 0;
            TestLoginResponse();
        }
        
        [ContextMenu("Test Need Create Role (ResultCode = -1)")]
        private void TestNeedCreateRole()
        {
            testResultCode = -1;
            TestLoginResponse();
        }
        [ContextMenu("Test Phone Error (ResultCode = -8)")]
        private void TestPhoneError()
        {
            testResultCode = -8;
            TestLoginResponse();
        }
        
        [ContextMenu("Test Password Error (ResultCode = -9)")]
        private void TestPasswordError()
        {
            testResultCode = -9;
            TestLoginResponse();
        }

        [ContextMenu("Test Account Not Found (ResultCode = -10)")]
        private void TestAccountNotFound()
        {
            testResultCode = -10;
            TestLoginResponse();
        }
        
        [ContextMenu("Run Validation Tests")]
        private void RunValidationTests()
        {
            TestValidation();
        }

#endif

    }

}