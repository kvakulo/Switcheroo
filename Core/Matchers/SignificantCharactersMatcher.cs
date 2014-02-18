using System;
using System.Text.RegularExpressions;

namespace Switcheroo.Core.Matchers
{
    public class SignificantCharactersMatcher : IMatcher
    {
        public MatchResult Evaluate(string input, string pattern)
        {
            if (input == null || pattern == null)
            {
                return NonMatchResult(input);
            }

            var regexPattern = BuildRegexPattern(pattern);

            var match = Regex.Match(input, regexPattern);

            if (!match.Success)
            {
                return NonMatchResult(input);
            }

            var matchResult = new MatchResult();

            var beforeMatch = input.Substring(0, match.Index);
            matchResult.StringParts.Add(new StringPart(beforeMatch));

            for (var groupIndex = 1; groupIndex < match.Groups.Count; groupIndex++)
            {
                var group = match.Groups[groupIndex];
                if (group.Value.Length > 0)
                {
                    matchResult.StringParts.Add(new StringPart(group.Value, groupIndex % 2 == 0));
                }
            }

            var afterMatch = input.Substring(match.Index + match.Length);
            matchResult.StringParts.Add(new StringPart(afterMatch));

            matchResult.Matched = true;
            matchResult.Score = 2;

            return matchResult;
        }

        private static string BuildRegexPattern(string pattern)
        {
            var regexPattern = "";
            foreach (var p in pattern)
            {
                var lowerP = Char.ToLowerInvariant(p);
                var upperP = Char.ToUpperInvariant(p);
                regexPattern += string.Format(@"([^\p{{Lu}}\s]*?\s?)(\b{0}|{1})", Regex.Escape(lowerP + ""), Regex.Escape(upperP + ""));
            }
            return regexPattern;
        }

        private static MatchResult NonMatchResult(string input)
        {
            var matchResult = new MatchResult();
            if (input != null)
            {
                matchResult.StringParts.Add(new StringPart(input));
            }
            return matchResult;
        }
    }
}