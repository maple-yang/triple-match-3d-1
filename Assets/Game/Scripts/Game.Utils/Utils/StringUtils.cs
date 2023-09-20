using System;
using System.Text.RegularExpressions;

namespace Game.Utils.Utils
{
    public static class StringUtils
    {
        public static string ToFormatedValue(this int _this)
        {
            var value = (long)_this;
            return value.ToFormatedValue();
        }
    
        public static string ToFormatedValue(this double _this)
        {
            var value = (long)_this;
            return value.ToFormatedValue();
        }
    
        public static string ToFormatedValue(this long _this)
        {
            if (_this / C_MILLION >= 1)
            {
                return $"{_this / C_MILLION:0.00}M";
            }
            if (_this / C_THOUSAND >= 1)
            {
                return $"{_this / C_THOUSAND:0.00}K";
            }
            return _this.ToString();
        }

        public static string ReplaceSubstr(this  string _this, int startIndex, int endIndex, string newString)
        {
            return _this.Remove(startIndex, endIndex - startIndex).Insert(startIndex, newString);
        }

        public static string RemoveSuffix(this string _this, string suffix, StringComparison comparisonType = StringComparison.Ordinal)
        {
            if (!_this.EndsWith(suffix, comparisonType))
            {
                return _this;
            }

            return _this.Substring(0, _this.Length - suffix.Length);
        }

        public static string RemovePrefix(this string _this, string prefix, StringComparison comparisonType = StringComparison.Ordinal)
        {
            if (!_this.StartsWith(prefix, comparisonType))
            {
                return _this;
            }

            return _this.Substring(prefix.Length);
        }
            
        public static string PascalToKebabCase(this string _this)
        {
            if (string.IsNullOrEmpty(_this))
                return _this;

            return Regex.Replace(
                    _this,
                    "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])",
                    "-$1",
                    RegexOptions.Compiled)
                .Trim()
                .ToLower();
        }

        private const float C_THOUSAND = 1000f;
        private const float C_MILLION = 1000000f;
    }
}