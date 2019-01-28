namespace ValidationWeb.Utility
{
    using System;
    using System.Collections.Generic;

    public class GradeLevelComparer : IComparer<string>
    {
        public int Compare(string left, string right)
        {
            // if they're equal there's nothing to do 
            if (left == right)
            {
                return 0;
            }

            var parsedLeft = Parse(left);
            var parsedRight = Parse(right);

            // both ints
            if (parsedLeft.IntValue.HasValue && parsedRight.IntValue.HasValue)
            {
                return parsedLeft.IntValue.Value.CompareTo(parsedRight.IntValue.Value);
            }

            // both strings
            if (!string.IsNullOrEmpty(parsedLeft.StringValue) && !string.IsNullOrEmpty(parsedRight.StringValue))
            {
                return string.Compare(parsedLeft.StringValue, parsedRight.StringValue, StringComparison.Ordinal);
            }

            // if we got here then one's a string and one's an int
            if (!string.IsNullOrEmpty(parsedLeft.StringValue) && parsedRight.IntValue.HasValue)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        protected ParsedValue Parse(string s)
        {
            ParsedValue parsedValue = new ParsedValue();

            int intValue;
            if (int.TryParse(s, out intValue))
            {
                parsedValue.IntValue = intValue;
            }
            else
            {
                parsedValue.StringValue = s;
            }

            return parsedValue;
        }

        protected struct ParsedValue
        {
            public int? IntValue;

            public string StringValue;
        }
    }
}