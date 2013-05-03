using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace IdeaSparx.CoolControls.Web
{
    public static class CoolCore
    {
        public static string ToJson(string JsonString)
        {
            using (StringReader sr = new StringReader(JsonString))
            {
                StringBuilder sbKeyword = new StringBuilder();
                int iChar;
                char c;
                while ((iChar = sr.Read()) != -1)
                {
                    c = Convert.ToChar(iChar);
                    switch (c)
                    {
                        case '{' :
                            break;
                        case ':' :
                            break;
                    }
                }
            }

            return string.Empty;
        }
    }

    public class JsonValue : Dictionary<string, JsonValue>
    {
        private string _Value;
        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        public string ToJson()
        {
            return string.Empty;
        }
    }
}
