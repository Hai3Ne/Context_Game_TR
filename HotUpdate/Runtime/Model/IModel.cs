using System;

public interface IModel
{
	void Init();
	void HandleCmd(NetCmdBase pack);
}

