using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace json2snippet
{
    /// <summary>
    /// the model class for vscode snippet
    /// </summary>
    public class VSCodeSnippet
    {
        public string prefix = string.Empty;
        public List<string> body = new List<string>();
        public string description;
    }

    /// <summary>
    /// used to describe a replacable variable
    /// </summary>
    public class Var
    {
        public string id;
        public string def;
        public static Var Create(string id, string def = null)
        {
            var o = new Var();
            o.id = id;
            o.def = string.IsNullOrEmpty(def) ? id : def;
            return o;
        }
    }
}
