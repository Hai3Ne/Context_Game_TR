using System.Collections.Generic;
using UnityEngine;

public enum Language
{
	CN,
	EN
}

public class StringTable
{
	static Language curLang = Language.CN;
	public static void SetLanguage(Language lang)
	{
		curLang = lang;
	}

    public static string GetString(string name)
    {
		if (FishConfig.Instance.languageConf == null || FishConfig.Instance.languageConf.Count == 0) {
			ConfigTables.Instance.LoadLanguage ();	
		}

		if (FishConfig.Instance.languageConf != null && FishConfig.Instance.languageConf.ContainsKey(name))
        {
			LanguagesVo langvo = FishConfig.Instance.languageConf.TryGet (name);
			if (curLang == Language.CN) {
				return langvo.CN;
			} else if (curLang == Language.EN)
				return langvo.EN;
			else
				return langvo.CN;
        }
        else
			return name;
    }

	public static string GetStringFormat(string name, object arg0){
		string formatStr = GetString(name);
		return string.Format (formatStr, arg0);
	}
	public static string GetStringFormat(string name, object arg0, object arg1){
		string formatStr = GetString(name);
		return string.Format (formatStr, arg0, arg1);
	}

    public static string GetString(string name, string def) {
        if (FishConfig.Instance.languageConf.ContainsKey(name)) {
            LanguagesVo langvo = FishConfig.Instance.languageConf.TryGet(name);
            if (curLang == Language.CN) {
                return langvo.CN;
            } else if (curLang == Language.EN)
                return langvo.EN;
            else
                return langvo.CN;
        } else
            return def;
    }

	public static string GetBubbleStr(uint msgID)
	{
		if (FishConfig.Instance.bubbleLanguageConf.ContainsKey(msgID))
		{
			BubbleLanguagesVo langvo = FishConfig.Instance.bubbleLanguageConf.TryGet (msgID);
			if (curLang == Language.CN) {
				return langvo.CN;
			} else if (curLang == Language.EN)
				return langvo.EN;
			else
				return langvo.CN;
		}
		else
			return msgID.ToString();
		
	}
}
