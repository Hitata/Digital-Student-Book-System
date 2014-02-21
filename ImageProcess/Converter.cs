/*
 * Converter.java
 *
 * @author     Quan Nguyen
 *
 * @version    1.3, 10 July 07
 */

using System;
using System.Text;
using System.Text.RegularExpressions;

namespace ImageProcessing
{
	public abstract class Converter 
	{
		// Fonts for HTML font tags
		protected const string SERIF = "Times New Roman";
		protected const string SANS_SERIF = "Arial";
      
		public abstract string Convert(string source, bool html);
    
		/**
		 *  Changes HTML meta tag for charset to UTF-8.
		 *
		 */
		protected string PrepareMetaTag(string str) 
		{
			return Regex.Replace(
				Regex.Replace(
				// delete existing charset attribute in meta tag
				Regex.Replace(str, "(?i)charset=(?:iso-8859-1|windows-1252|windows-1258|us-ascii|x-user-defined)", ""),
				// delete the rest of the meta tag
				"(?i)<meta http-equiv=\"?Content-Type\"? content=\"text/html;\\s*\">\\n?", ""),
				// insert new meta tag with UTF-8 charset
				"(?i)<head>", "<head>\n<meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\">");
		}

		/**
		 *  Translates Character entity references to corresponding Cp1252 characters.
		 *
		 */
		protected string HtmlToAnsi(string str) 
		{
			string[] extended_ansi_html = {"&trade;", "&#8209;", "&nbsp;",
						"&iexcl;", "&cent;", "&pound;", "&curren;", "&yen;", "&brvbar;", "&sect;", "&uml;", "&copy;", "&ordf;",
						"&laquo;", "&not;", "&shy;", "&reg;", "&macr;", "&deg;", "&plusmn;", "&sup2;", "&sup3;",
						"&acute;", "&micro;", "&para;", "&middot;", "&cedil;", "&sup1;", "&ordm;", "&raquo;",
						"&frac14;", "&frac12;", "&frac34;", "&iquest;", "&Agrave;", "&Aacute;", "&Acirc;",
						"&Atilde;", "&Auml;", "&Aring;", "&AElig;", "&Ccedil;", "&Egrave;", "&Eacute;", "&Ecirc;",
						"&Euml;", "&Igrave;", "&Iacute;", "&Icirc;", "&Iuml;", "&ETH;", "&Ntilde;", "&Ograve;",
						"&Oacute;", "&Ocirc;", "&Otilde;", "&Ouml;", "&times;", "&Oslash;", "&Ugrave;", "&Uacute;",
						"&Ucirc;", "&Uuml;", "&Yacute;", "&THORN;", "&szlig;", "&agrave;", "&aacute;", "&acirc;",
						"&atilde;", "&auml;", "&aring;", "&aelig;", "&ccedil;", "&egrave;", "&eacute;", "&ecirc;",
						"&euml;", "&igrave;", "&iacute;", "&icirc;", "&iuml;", "&eth;", "&ntilde;", "&ograve;",
						"&oacute;", "&ocirc;", "&otilde;", "&ouml;", "&divide;", "&oslash;", "&ugrave;", "&uacute;",
						"&ucirc;", "&uuml;", "&yacute;", "&thorn;", "&yuml;"};
			string[] extended_ansi = {"\u0099", "\u2011", "\u00A0",
						"\u00A1", "\u00A2", "\u00A3", "\u00A4", "\u00A5", "\u00A6", "\u00A7", "\u00A8", "\u00A9",
						"\u00AA", "\u00AB", "\u00AC", "\u00AD", "\u00AE", "\u00AF", "\u00B0", "\u00B1", "\u00B2",
						"\u00B3", "\u00B4", "\u00B5", "\u00B6", "\u00B7", "\u00B8", "\u00B9", "\u00BA", "\u00BB",
						"\u00BC", "\u00BD", "\u00BE", "\u00BF", "\u00C0", "\u00C1", "\u00C2", "\u00C3", "\u00C4",
						"\u00C5", "\u00C6", "\u00C7", "\u00C8", "\u00C9", "\u00CA", "\u00CB", "\u00CC", "\u00CD",
						"\u00CE", "\u00CF", "\u00D0", "\u00D1", "\u00D2", "\u00D3", "\u00D4", "\u00D5", "\u00D6",
						"\u00D7", "\u00D8", "\u00D9", "\u00DA", "\u00DB", "\u00DC", "\u00DD", "\u00DE", "\u00DF",
						"\u00E0", "\u00E1", "\u00E2", "\u00E3", "\u00E4", "\u00E5", "\u00E6", "\u00E7", "\u00E8",
						"\u00E9", "\u00EA", "\u00EB", "\u00EC", "\u00ED", "\u00EE", "\u00EF", "\u00F0", "\u00F1",
						"\u00F2", "\u00F3", "\u00F4", "\u00F5", "\u00F6", "\u00F7", "\u00F8", "\u00F9", "\u00FA",
						"\u00FB", "\u00FC", "\u00FD", "\u00FE", "\u00FF"};
			
			StringBuilder strB = new StringBuilder(str);
			
			for (int i = 0; i < extended_ansi_html.Length; i++)
			{
				strB.Replace(extended_ansi_html[i], extended_ansi[i]);
			}

			return strB.ToString();
		}
    
		/**
		 *  Converts Numeric Character References and Unicode escape sequences to Unicode
		 */
		protected string ConvertNCR(string str) 
		{
			StringBuilder result = new StringBuilder();
			string[] NCRs = {"&#x", "&#", "\\u", "U+", "#x", "#"};
	        
			for (int i = 0; i < NCRs.Length; i++) 
			{
				int radix;
				int foundIndex;
				int startIndex = 0;        
				int STR_LENGTH = str.Length;
				string NCR = NCRs[i]; 
				int NCR_LENGTH = NCR.Length;
	            
				if (NCR == "&#" || NCR == "#")
				{
					radix = 10; 
				}
				else
				{
					radix = 16;
				}
	                                  
				while (startIndex < STR_LENGTH)
				{
					foundIndex = str.IndexOf(NCR, startIndex);

					if (foundIndex == -1)
					{
						result.Append(str.Substring(startIndex));
						break;
					}

					result.Append(str.Substring(startIndex, foundIndex - startIndex));
					
					if (NCR == "\\u" || NCR == "U+")
					{
						startIndex = foundIndex + 6;
					}
					else
					{
						startIndex = str.IndexOf(";", foundIndex);
					}

					if (startIndex == -1)
					{
						result.Append(str.Substring(foundIndex));
						break;
					}

					string tok = str.Substring(foundIndex + NCR_LENGTH, startIndex - (foundIndex + NCR_LENGTH));

					try 
					{
						result.Append((char) System.Convert.ToInt32(tok, radix));
					}
					catch (FormatException)
					{
						try 
						{
							if (NCR == "\\u" || NCR == "U+")
							{
								result.Append(NCR + tok);
							}
							else
							{
								result.Append(NCR + tok + str[startIndex]);                            
							}
						} 
						catch (IndexOutOfRangeException) 
						{
							result.Append(NCR + tok);
						}
					}

					if (NCR != "\\u" && NCR != "U+") 
					{
						startIndex++;
					}
				}
				str = result.ToString();
				result.Length = 0;
			}
			return str;
		}

		/**
		 * Converts Cp1252 characters in \u0080-\u009F range to pure hex.
		 * This method is required for VISCII and VPS because these encodings
		 * utilize characters in this range.
		 */    
		protected string Cp1252ToHex(string str)
		{
			char[] cha = {'\u20AC', '\u201A', '\u0192', '\u201E', '\u2026', '\u2020', '\u2021',
							 '\u02C6', '\u2030', '\u0160', '\u2039', '\u0152', '\u017D',
							 '\u2018', '\u2019', '\u201C', '\u201D', '\u2022', '\u2013', '\u2014',
							 '\u02DC', '\u2122', '\u0161', '\u203A', '\u0153', '\u017E', '\u0178'
						 };
			char[] hex = {'\u0080', '\u0082', '\u0083', '\u0084', '\u0085', '\u0086', '\u0087', 
							 '\u0088', '\u0089', '\u008A', '\u008B', '\u008C', '\u008E',
							 '\u0091', '\u0092', '\u0093', '\u0094', '\u0095', '\u0096', '\u0097', 
							 '\u0098', '\u0099', '\u009A', '\u009B', '\u009C', '\u009E', '\u009F'
						 };
			
			StringBuilder strB = new StringBuilder(str);
			
			for (int i = 0; i < hex.Length; i++)
			{
				strB.Replace(cha[i], hex[i]);
			}

			return strB.ToString();
		}
    
		/**
		 *  Unicode Composite-to-Unicode Precomposed conversion (NFD -> NFC)
		 */    
		protected string CompositeToPrecomposed(string str)
		{
			// Perform Unicode NFC on NFD string
			return str.Normalize(NormalizationForm.FormC);       
		}
        
		/**
		 * Converts HTML
		 */
		protected string ConvertHTML(string str)
		{
			return ReplaceFont(
				PrepareMetaTag(
				ConvertNCR(
				HtmlToAnsi(str))));
		}
        
		/**
		 * Replaces fonts, to be overridden by subclass when necessary
		 */
		protected virtual string ReplaceFont(string str)
		{
			return str;
		}
	}
}