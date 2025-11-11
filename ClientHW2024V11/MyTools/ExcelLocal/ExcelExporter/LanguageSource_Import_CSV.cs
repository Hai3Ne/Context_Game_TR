using System;
using System.Collections.Generic;


public class LanguageSource
{
    public LanguageDataMgr _codeHelper = new LanguageDataMgr();

    public void Clear()
    {
        _codeHelper.ClearAllData();
    }

    public void InitFromCsvFile(string FileName)
    {
        var encoding = System.Text.Encoding.UTF8;
        string CSVstring = LocalizationReader.ReadCSVfile(FileName, encoding);
        Import_CSV(CSVstring);
    }

    public string Import_CSV(string CSVstring, char Separator = ',' )
	{
		List<string[]> CSV = LocalizationReader.ReadCSV (CSVstring, Separator);
		return Import_CSV(CSV);
	}

	public string Import_CSV(List<string[]> CSV)
	{
		string[] Tokens = CSV[0];

		int LanguagesStartIdx = 1;

		var ValidColumnName_Key  = new string[]{ "Key" };
		var ValidColumnName_Type = new string[]{ "Type" };
		var ValidColumnName_Desc = new string[]{ "Desc", "Description" };

		if (Tokens.Length>1 && ArrayContains(Tokens[0], ValidColumnName_Key))
		{
			if (Tokens.Length>2)
			{
				if (ArrayContains(Tokens[1], ValidColumnName_Type)) 
				{
					LanguagesStartIdx = 2;
				}
				if (ArrayContains(Tokens[1], ValidColumnName_Desc)) 
				{
					LanguagesStartIdx = 2;
				}

			}
			if (Tokens.Length>3)
			{
				if (ArrayContains(Tokens[2], ValidColumnName_Type)) 
				{
					LanguagesStartIdx = 3;
				}
				if (ArrayContains(Tokens[2], ValidColumnName_Desc)) 
				{
					LanguagesStartIdx = 3;
				}
			}
		}
		else
			return "Bad Spreadsheet Format.\nFirst columns should be 'Key', 'Type' and 'Desc'";

		int nLanguages = Math.Max (Tokens.Length-LanguagesStartIdx, 0);
        int ChineseIdx = -1;
		for (int i=0; i<nLanguages; ++i)
		{
			if (string.IsNullOrEmpty(Tokens[i+LanguagesStartIdx]))
			{
				continue;
			}

			string langToken = Tokens[i + LanguagesStartIdx];
			if (langToken.StartsWith("$"))
			{
				langToken = langToken.Substring(1);
			}
            if(langToken == "Chinese")
            {
                ChineseIdx = i + LanguagesStartIdx;
                break;
            }
		}

        for (int i = 1, imax = CSV.Count; i < imax; ++i)
        {
            Tokens = CSV[i];
            string sKey = Tokens[0];
            string sValue = Tokens[ChineseIdx];
            _codeHelper.CreateStringKey(sValue, sKey, true);
        }

		return string.Empty;
	}


    bool ArrayContains( string MainText, params string[] texts )
    {
        for (int i = 0, imax = texts.Length; i < imax; ++i)
            if (MainText.IndexOf(texts[i], StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
        return false;
    }
}