using System.Collections.Generic;
using System.Text.RegularExpressions;
using UI.Presenters.Upgrades;

namespace Static
{
    public static class StaticNames
    {
        public static Dictionary<ResourceNames, string> _names = new Dictionary<ResourceNames, string>();

        public static string Get(ResourceNames t)
        {
            if (_names.TryGetValue(t, out var result))
            {
                return result;
            }

            var r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);


            string s = t.ToString();
            
            _names[t] = r.Replace(s, " ");
            if (t == ResourceNames.ShamansStick)
            {
                _names[t] = "Dream Catcher";
            }
            if (t == ResourceNames.Gum)
            {
                _names[t] = "Bubble Gum";
            }
            if (t == ResourceNames.Soft)
            {
                _names[t] = "Cats Coin";
            }
            if (t == ResourceNames.Hard)
            {
                _names[t] = "Emerald";
            }
            return _names[t];
        }
        
    }
}