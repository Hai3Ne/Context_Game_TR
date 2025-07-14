using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class ReportException:SingleTon<ReportException>
{
	string ReporterIP;
	int    ReporterPort;

    const uint MAGIC_NUMBER = 0xFEDCABCD;
    SafeList<string> m_ErrList = new SafeList<string>();
    string m_LastErr    = "";
    string m_ErrPrefix  = "";
	public void GlobalInit()
    {
		m_ErrPrefix = string.Format ("<Plateform:{0}, Package:{1}, Version:{2}", 
			KApplication.GetPlatfomrName(),
            PlatormConfig.AppIdentifier,
            //ConstValue.DefPackageName, 
			Utility.VerToDotStr (GameUtils.ClientVer));
		
		ReporterIP = GameParams.Instance.ReporterIP;
		ReporterPort = GameParams.Instance.ReporterPort;
        Thread th = new Thread(new ParameterizedThreadStart(SendThread));
        th.Start(null);
    }

    public void Clear()
    {
        m_ErrList = new SafeList<string>();
    }

    public void SendThread(object obj)
    {
        byte[] magic = System.BitConverter.GetBytes(MAGIC_NUMBER);
        while(true)
        {
            try
            {
                if (ReporterIP != null)
                {
                    if (m_ErrList.HasItem())
                    {
                        string str = m_ErrList.GetItem();
                        TCPClient tt = new TCPClient(null);
                        if (tt.Connect(ReporterIP, ReporterPort, 0, 0, false))
                        {

                            byte[] txt = System.Text.Encoding.ASCII.GetBytes(str);
                            byte[] size = System.BitConverter.GetBytes((ushort)txt.Length);
                            byte[] sendData = new byte[txt.Length + 6];
                            System.Array.Copy(txt, 0, sendData, 6, txt.Length);
                            System.Array.Copy(size, 0, sendData, 4, size.Length);
                            System.Array.Copy(magic, 0, sendData, 0, magic.Length);
                            tt.Send(sendData);
                            tt.Disconnect();
                        }
                    }
                }
            }
            catch
            {

            }
            Thread.Sleep(500);
        }
    }
    public void AddException(string str)
    {
        if (str != m_LastErr)
        {
            m_LastErr = str;
            m_ErrList.AddItem(m_ErrPrefix + str);
        }
    }

}

