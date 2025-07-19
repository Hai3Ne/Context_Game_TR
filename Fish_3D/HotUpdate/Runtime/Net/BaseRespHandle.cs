using System;
public class BaseRespHandle
{
    protected NetCmdPack netdata;
    public void AcceptResp(NetCmdPack netpack)
	{
		netdata = netpack;
		Handle ();
	}

	protected virtual void Handle(){ }
}