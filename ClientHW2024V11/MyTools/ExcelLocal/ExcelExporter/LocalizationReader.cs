using System.Text;
using System.Collections.Generic;

	public class LocalizationReader 
	{
		#region CSV
		public static string ReadCSVfile( string Path, Encoding encoding )
		{
			string Text = string.Empty;
			#if (UNITY_WP8 || UNITY_METRO) && !UNITY_EDITOR
				byte[] buffer = UnityEngine.Windows.File.ReadAllBytes (Path);
				Text = Encoding.UTF8.GetString(buffer, 0, buffer.Length);
			#else
				/*using (System.IO.StreamReader reader = System.IO.File.OpenText(Path))
				{
					Text = reader.ReadToEnd();
				}*/
				using (var reader = new System.IO.StreamReader(Path, encoding ))
					Text = reader.ReadToEnd();
			#endif

			Text = Text.Replace("\r\n", "\n");
			Text = Text.Replace("\r", "\n");

			return Text;
		}

		public static List<string[]> ReadCSV( string Text, char Separator=',' )
		{
			int iStart = 0;
			List<string[]> CSV = new List<string[]>();

			while (iStart < Text.Length)
			{
				string[] list = ParseCSVline (Text, ref iStart, Separator);
				if (list==null) break;
				CSV.Add(list);
			}
			return CSV;
		}

		static string[] ParseCSVline( string Line, ref int iStart, char Separator )
		{
			List<string> list = new List<string>();
			
			//Line = "puig,\"placeres,\"\"cab\nr\nera\"\"algo\"\npuig";//\"Frank\npuig\nplaceres\",aaa,frank\nplaceres";

			int TextLength = Line.Length;
			int iWordStart = iStart;
			bool InsideQuote = false;

			while (iStart < TextLength)
			{
				char c = Line[iStart];

				if (InsideQuote)
				{
					if (c=='\"') //--[ Look for Quote End ]------------
					{
						if (iStart+1 >= TextLength || Line[iStart+1] != '\"')  //-- Single Quote:  Quotation Ends
						{
							InsideQuote = false;
						}
						else
						if (iStart+2 < TextLength && Line[iStart+2]=='\"')  //-- Tripple Quotes: Quotation ends
						{
							InsideQuote = false;
							iStart+=2;
						}
						else 
							iStart++;  // Skip Double Quotes
					}
				}

				else  //-----[ Separators ]----------------------

				if (c=='\n' || c==Separator)
				{
					AddCSVtoken(ref list, ref Line, iStart, ref iWordStart);
					if (c=='\n')  // Stop the row on line breaks
					{
						iStart++;
						break;
					}
				}

				else //--------[ Start Quote ]--------------------

				if (c=='\"')
					InsideQuote = true;

				iStart++;
			}
			if (iStart>iWordStart)
				AddCSVtoken(ref list, ref Line, iStart, ref iWordStart);

			return list.ToArray();
		}

		static void AddCSVtoken( ref List<string> list, ref string Line, int iEnd, ref int iWordStart)
		{
			string Text = Line.Substring(iWordStart, iEnd-iWordStart);
			iWordStart = iEnd+1;

			Text = Text.Replace("\"\"", "\"" );
			if (Text.Length>1 && Text[0]=='\"' && Text[Text.Length-1]=='\"')
				Text = Text.Substring(1, Text.Length-2 );

			list.Add( Text);
		}

		

		#endregion

		#region I2CSV

		public static List<string[]> ReadI2CSV( string Text )
		{
			string[] ColumnSeparator = new string[]{"[*]"};
			string[] RowSeparator = new string[]{"[ln]"};

			List<string[]> CSV = new List<string[]>();
			foreach (var line in Text.Split (RowSeparator, System.StringSplitOptions.None))
				CSV.Add (line.Split (ColumnSeparator, System.StringSplitOptions.None));

			return CSV;
		}

		#endregion

		#region Misc

		public static void ValidateFullTerm( ref string Term )
		{
			Term = Term.Replace('\\', '/');
			int First = Term.IndexOf('/');
			if (First<0)
				return;
			
			int second;
			while ( (second=Term.LastIndexOf('/')) != First )
				Term = Term.Remove( second,1);
		}

		
		// this function encodes \r\n and \n into \\n
		public static string EncodeString( string str )
		{
			if (string.IsNullOrEmpty(str))
				return string.Empty;

			return str.Replace("\r\n", "<\\n>")
				.Replace("\r", "<\\n>")
					.Replace("\n", "<\\n>");
		}
		
		public static string DecodeString( string str )
		{
			if (string.IsNullOrEmpty(str))
				return string.Empty;
			
			return str.Replace("<\\n>", "\r\n");
		}

		#endregion
	}
