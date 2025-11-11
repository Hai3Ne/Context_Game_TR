@echo off
echo ×ª»»csÐ­Òé...
ProtocolBuilder.exe proto.xml -cs NetMsgData.cs
copy .\NetMsgData.cs ..\..\Assets\HotUpdate\Socket\Net /y



pause