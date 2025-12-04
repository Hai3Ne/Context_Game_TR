/********************************************************************
生成日期:	15:8:2025   11:31
作    者:	自动生成
描    述:
*********************************************************************/
using System;
using UnityEngine;
namespace SEZSJ
{
	public static class PacketBuilder
	{
		public static MsgData newBuilder(int code, NetReadBuffer pDatas)
		{
			MsgData msgdata = null;
			switch(code)
			{
				case 6001:
				{
					msgdata = Activator.CreateInstance(typeof(MsgData_sLogin)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 6002:
				{
					msgdata = Activator.CreateInstance(typeof(MsgData_sCreateRole)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 6003:
				{
					msgdata = Activator.CreateInstance(typeof(MsgData_sRoleInfo)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 6005:
				{
					msgdata = Activator.CreateInstance(typeof(MsgData_sCookieUpdate)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 6006:
				{
					msgdata = Activator.CreateInstance(typeof(MsgData_sReconnect)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 6010:
				{
					msgdata = Activator.CreateInstance(typeof(LC_RegisterAccountSmsgRet)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 6011:
				{
					msgdata = Activator.CreateInstance(typeof(LC_RegisterAccountRet)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 6012:
				{
					msgdata = Activator.CreateInstance(typeof(LC_ChangePwdForAccountSmsgRet)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 6013:
				{
					msgdata = Activator.CreateInstance(typeof(LC_ChangePwdForAccountRet)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 7001:
				{
					msgdata = Activator.CreateInstance(typeof(MsgData_sEnterGame)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 7006:
				{
					msgdata = Activator.CreateInstance(typeof(WC_SysNotice)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 7010:
				{
					msgdata = Activator.CreateInstance(typeof(WC_SyncTime)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 7032:
				{
					msgdata = Activator.CreateInstance(typeof(WC_GetMailResult)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 7033:
				{
					msgdata = Activator.CreateInstance(typeof(WC_OpenMailResult)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 7034:
				{
					msgdata = Activator.CreateInstance(typeof(WC_GetMailItemResult)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 7035:
				{
					msgdata = Activator.CreateInstance(typeof(WC_DelMail)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 7036:
				{
					msgdata = Activator.CreateInstance(typeof(WC_NotifyMail)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 7100:
				{
					msgdata = Activator.CreateInstance(typeof(WC_BindAccountSmsgRet)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 7101:
				{
					msgdata = Activator.CreateInstance(typeof(WC_BindAccountRet)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 7102:
				{
					msgdata = Activator.CreateInstance(typeof(WC_ChangeAccountSmsgRet)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 7103:
				{
					msgdata = Activator.CreateInstance(typeof(WC_ChangeAccountPswdRet)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 7104:
				{
					msgdata = Activator.CreateInstance(typeof(WC_Human_Real_Name_Authentication_Ret)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 7136:
				{
					msgdata = Activator.CreateInstance(typeof(MsgData_sHeartBeat)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 7906:
				{
					msgdata = Activator.CreateInstance(typeof(MsgData_sLeaveGame)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 8002:
				{
					msgdata = Activator.CreateInstance(typeof(SC_SCENE_SHOW_ME_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 8004:
				{
					msgdata = Activator.CreateInstance(typeof(SC_SCENE_SHOW_ME_INFO_EXTEND)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 8005:
				{
					msgdata = Activator.CreateInstance(typeof(SC_SCENE_VARIABLE_VALUE_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 8011:
				{
					msgdata = Activator.CreateInstance(typeof(SC_OBJ_ATTR_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 8201:
				{
					msgdata = Activator.CreateInstance(typeof(SC_HUMAN_SIGNIN_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 8202:
				{
					msgdata = Activator.CreateInstance(typeof(SC_HUMAN_GETJJJ_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 8203:
				{
					msgdata = Activator.CreateInstance(typeof(SC_HUMAN_GETZCCAT_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 8204:
				{
					msgdata = Activator.CreateInstance(typeof(SC_HUMAN_SET_ICONID_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 8205:
				{
					msgdata = Activator.CreateInstance(typeof(SC_RECHARGE_PIX_BIND_INFO_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 8206:
				{
					msgdata = Activator.CreateInstance(typeof(SC_CASHOUT_PIX_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 8207:
				{
					msgdata = Activator.CreateInstance(typeof(SC_CASHOUT_PIX_BIND_INFO_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 8208:
				{
					msgdata = Activator.CreateInstance(typeof(SC_HUMAN_SET_PLAYER_NAME_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 8209:
				{
					msgdata = Activator.CreateInstance(typeof(SC_HUMAN_SEND_GOLD_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 8210:
				{
					msgdata = Activator.CreateInstance(typeof(SC_HUMAN_SEND_GOLD_NOTE)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 8211:
				{
					msgdata = Activator.CreateInstance(typeof(SC_DIAMOND_EXCHANGE_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 8212:
				{
					msgdata = Activator.CreateInstance(typeof(SC_DIAMOND_EXCHANGE_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 8213:
				{
					msgdata = Activator.CreateInstance(typeof(SC_CARD_EXCHANGE_GOLD_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 8230:
				{
					msgdata = Activator.CreateInstance(typeof(SC_ENTER_ROOM_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 9000:
				{
					msgdata = Activator.CreateInstance(typeof(SC_HUMAN_ENTER_GAME_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 9001:
				{
					msgdata = Activator.CreateInstance(typeof(SC_HUMAN_LEAVE_GAME_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 9002:
				{
					msgdata = Activator.CreateInstance(typeof(SC_RECHARGE_PIX_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 9003:
				{
					msgdata = Activator.CreateInstance(typeof(SC_HUMAN_ONLINE_MESSAGE_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 9004:
				{
					msgdata = Activator.CreateInstance(typeof(SC_HUMAN_RECHARGE_MONEY_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 9005:
				{
					msgdata = Activator.CreateInstance(typeof(SC_SYN_GAME_ONLINE_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 9006:
				{
					msgdata = Activator.CreateInstance(typeof(SC_HUMAN_RECHARGE_RECORD_NOTICE)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 10801:
				{
					msgdata = Activator.CreateInstance(typeof(SC_TASK_GAIN_REWARD_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 10802:
				{
					msgdata = Activator.CreateInstance(typeof(SC_TASK_LIST_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 10803:
				{
					msgdata = Activator.CreateInstance(typeof(SC_TASK_UPDATE_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 10804:
				{
					msgdata = Activator.CreateInstance(typeof(SC_TASK_EXPAND_INFO_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 10805:
				{
					msgdata = Activator.CreateInstance(typeof(SC_TASK_EXPAND_EXTRACT_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 10806:
				{
					msgdata = Activator.CreateInstance(typeof(SC_TASK_COMMRANK_CURRENT_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 10807:
				{
					msgdata = Activator.CreateInstance(typeof(SC_TASK_COMMRANK_HISTORY_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 10808:
				{
					msgdata = Activator.CreateInstance(typeof(SC_TASK_COMMARENA_CURRENT_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 10809:
				{
					msgdata = Activator.CreateInstance(typeof(SC_TASK_COMMARENA_HISTORY_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 10810:
				{
					msgdata = Activator.CreateInstance(typeof(SC_TASK_GOLDRANK_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 10811:
				{
					msgdata = Activator.CreateInstance(typeof(SC_TASK_SEND_GOLD_RECORD_INFO_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 10812:
				{
					msgdata = Activator.CreateInstance(typeof(SC_TASK_BOMBBOX_GAIN_REWARD_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 10813:
				{
					msgdata = Activator.CreateInstance(typeof(SC_TASK_BOMBBOX_UPDATE_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11001:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME1_BET_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11002:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME1_BROADCAST_JACKPOT)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11003:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME1_BROADCAST_ADD_AWARD)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11004:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME1_ROOM_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11021:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME2_BET_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11022:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME2_BROADCAST_JACKPOT)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11023:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME2_BROADCAST_ADD_AWARD)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11024:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME2_ROOM_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11041:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME3_BET_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11042:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME3_AWARD_BOX_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11043:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME3_ROOM_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11061:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME4_BET_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11062:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME4_BROADCAST_JACKPOT)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11063:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME4_BROADCAST_ADD_AWARD)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11064:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME4_ROOM_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11081:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME5_BET_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11082:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME5_BROADCAST_JACKPOT)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11083:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME5_BROADCAST_ADD_AWARD)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11084:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME5_ROOM_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11101:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME6_BET_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11102:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME6_BROADCAST_JACKPOT)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11103:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME6_BROADCAST_ADD_AWARD)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11104:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME6_ROOM_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11121:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME7_BET_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11123:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME7_BROADCAST_ADD_AWARD)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11124:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME7_ROOM_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11141:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME8_BET_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11143:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME8_BROADCAST_ADD_AWARD)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11144:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME8_ROOM_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11161:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME9_BET_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11181:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME10_BET_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11184:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME10_ROOM_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11201:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME11_BET_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11203:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME11_BROADCAST_ADD_AWARD)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11204:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME11_ROOM_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11221:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME12_ROOM_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11222:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME12_BET_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11223:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME12_BROADCAST_BET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11224:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME12_BROADCAST_ADD_PLAYER)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11225:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME12_BROADCAST_DEL_PLAYER)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11226:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME12_BROADCAST_BET_START)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11227:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME12_BROADCAST_BET_END)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11228:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME12_CALCULATE)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11241:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME13_BET_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11244:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME13_ROOM_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11261:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME14_BET_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11264:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME14_ROOM_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11281:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME15_BET_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11282:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME15_BROADCAST_JACKPOT)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11283:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME15_BROADCAST_ADD_AWARD)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11284:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME15_ROOM_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11361:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME19_BET_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11362:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME19_BROADCAST_JACKPOT)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11363:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME19_BROADCAST_ADD_AWARD)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11364:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME19_ROOM_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11381:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME20_BET_RET)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				case 11384:
				{
					msgdata = Activator.CreateInstance(typeof(SC_GAME20_ROOM_INFO)) as MsgData;
					msgdata.unpack(pDatas);
					break;
				}
				default:
					Debug.LogWarning("not find msgid " + code);
				break;
			}
			return msgdata;
		}
	}
}
