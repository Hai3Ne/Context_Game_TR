using System;
using System.IO;
using System.Reflection;

using System.Collections.Generic;

public class NetRespCustomParse
{
	public static object Parse(Type t, BinaryReader br)
	{
		object obj = null;
		if (t == typeof(tagUserinfoExt)) {
			if (br.BaseStream.Position >= br.BaseStream.Length)
				return null;
			
			tagUserinfoExt usrextIfo = new tagUserinfoExt ();
			ITypeReader iReader = TypeReaderMapping.GetReaderFromHash (Utility.GetHash (typeof(string).ToString ()));
			while (br.BaseStream.Position < br.BaseStream.Length) {
				ushort msgl = (ushort)(br.ReadUInt16() >> 1); 
				if (br.BaseStream.Position >= br.BaseStream.Length)
					return null;
				ushort msgt = br.ReadUInt16();
                switch (msgt) {
				case 10:
					usrextIfo.UseNickName = (string)iReader.Read (br, msgl);
					break;
				case 11:
					usrextIfo.GroupName = (string)iReader.Read (br, msgl);
					break;
				case 12:
					usrextIfo.UnderWrite = (string)iReader.Read (br, msgl);
					break;
				default:
					iReader.Read (br, msgl);
					break;
				}
			}
			obj = usrextIfo;
        } else if (t == typeof(tagLoginfoExt)) {
            if (br.BaseStream.Position >= br.BaseStream.Length)
                return null;

            tagLoginfoExt usrextIfo = new tagLoginfoExt();
            ITypeReader iReader = TypeReaderMapping.GetReaderFromHash(Utility.GetHash(typeof(string).ToString()));
            while (br.BaseStream.Position < br.BaseStream.Length) {
                ushort msgl = br.ReadUInt16();
                if (br.BaseStream.Position >= br.BaseStream.Length)
                    return null;
                ushort msgt = br.ReadUInt16();
                switch (msgt) {
                    case 2:
                        usrextIfo.MemberInfo = new tagMemberInfo();
                        usrextIfo.MemberInfo.cbMemberOrder = br.ReadByte();
                        usrextIfo.MemberInfo.MemberOverDate = new SYSTEMTIME();
                        usrextIfo.MemberInfo.MemberOverDate.Year = br.ReadUInt16();
                        usrextIfo.MemberInfo.MemberOverDate.Month = br.ReadUInt16();
                        usrextIfo.MemberInfo.MemberOverDate.DayOfWeek = br.ReadUInt16();
                        usrextIfo.MemberInfo.MemberOverDate.Day = br.ReadUInt16();
                        usrextIfo.MemberInfo.MemberOverDate.Hour = br.ReadUInt16();
                        usrextIfo.MemberInfo.MemberOverDate.Minute = br.ReadUInt16();
                        usrextIfo.MemberInfo.MemberOverDate.Second = br.ReadUInt16();
                        usrextIfo.MemberInfo.MemberOverDate.Milliseconds = br.ReadUInt16();
                        break;
                    case 3:
                        usrextIfo.UnderWrite = (string)iReader.Read(br, msgl);
                        break;
                    case 4:
                        usrextIfo.NickNameInfo = new tagModifyNickNameInfo();
                        usrextIfo.NickNameInfo.wFreeModifyCount = br.ReadUInt16();
                        usrextIfo.NickNameInfo.lModifyScore = br.ReadInt64();
                        //usrextIfo.NickNameInfo = (tagModifyNickNameInfo)TypeReflector.CreateObj(TypeMapping.FindType(TypeSize<tagModifyNickNameInfo>.HASH), br);
                        break;
                    default:
                        iReader.Read(br, msgl);
                        break;
                }
            }
            obj = usrextIfo;
        } else if (t == typeof(tagUserIndividualExt)) {
            if (br.BaseStream.Position >= br.BaseStream.Length)
                return null;

            tagUserIndividualExt usrextIfo = new tagUserIndividualExt();
            ITypeReader iReader = TypeReaderMapping.GetReaderFromHash(Utility.GetHash(typeof(string).ToString()));
            while (br.BaseStream.Position < br.BaseStream.Length) {
                ushort msgl = (ushort)(br.ReadUInt16() >>1);
                if (br.BaseStream.Position >= br.BaseStream.Length)
                    return null;
                ushort msgt = br.ReadUInt16();
                switch (msgt) {
                    case 1:
                        usrextIfo.NickName = (string)iReader.Read(br, msgl);
                        break;
                    case 2:
                        usrextIfo.UserNote = (string)iReader.Read(br, msgl);
                        break;
                    case 3:
                        usrextIfo.UnderWrite = (string)iReader.Read(br, msgl);
                        break;
                    case 4:
                        usrextIfo.QQ = (string)iReader.Read(br, msgl);
                        break;
                    case 5:
                        usrextIfo.Email = (string)iReader.Read(br, msgl);
                        break;
                    case 6:
                        usrextIfo.SeatPhone = (string)iReader.Read(br, msgl);
                        break;
                    case 7:
                        usrextIfo.MobilePhone = (string)iReader.Read(br, msgl);
                        break;
                    case 8:
                        usrextIfo.Compellation = (string)iReader.Read(br, msgl);
                        break;
                    case 9:
                        usrextIfo.DwellingPlace = (string)iReader.Read(br, msgl);
                        break;
                    case 10:
                        usrextIfo.PassPortID = (string)iReader.Read(br, msgl);
                        break;
                    case 11:
                        usrextIfo.Question = (string)iReader.Read(br, msgl);
                        break;
                    case 12:
                        usrextIfo.Answer = (string)iReader.Read(br, msgl);
                        break;
                    case 13:
                        usrextIfo.NewPw = (string)iReader.Read(br, msgl);
                        break;
                    case 14:
                        usrextIfo.Accounts = (string)iReader.Read(br, msgl);
                        break;
                    case 15:
                        usrextIfo.LogonPass = (string)iReader.Read(br, msgl);
                        break;
                    case 16:
                        usrextIfo.InsurePass = (string)iReader.Read(br, msgl);
                        break;
                    default:
                        iReader.Read(br, msgl);
                        break;
                }
            }
            obj = usrextIfo;
        }else{
            //2;//DTP_GP_MEMBER_INFO

			//FieldInfo[] fields = t.GetFields ();

		}
		return obj;
	}


    public class FieldInfo {
        public string key;
        public ushort maxLen;
        public LuaNetType mType;
        public string elem_type;
    }

}


