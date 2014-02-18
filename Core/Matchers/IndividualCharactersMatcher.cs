using System.Text.RegularExpressions;

namespace Switcheroo.Core.Matchers
{
    public class IndividualCharactersMatcher : IMatcher
    {
        public MatchResult Evaluate(string input, string pattern)
        {
            if (input == null || pattern == null)
            {
                return NonMatchResult(input);
            }

            var regexPattern = BuildRegexPattern(pattern);

            var match = Regex.Match(input, regexPattern, RegexOptions.IgnoreCase);

            if (!match.Success)
            {
                return NonMatchResult(input);
            }

            var matchResult = new MatchResult();
            for (var groupIndex = 1; groupIndex < match.Groups.Count; groupIndex++)
            {
                var group = match.Groups[groupIndex];
                if (group.Value.Length > 0)
                {
                    matchResult.StringParts.Add(new StringPart(group.Value, groupIndex % 2 == 0));
                }
            }

            matchResult.Matched = true;
            matchResult.Score = 1;

            return matchResult;
        }

        private static string BuildRegexPattern(string pattern)
        {
            var regexPattern = "";
            char? previousChar = null;
            foreach (var p in pattern)
            {
                if (previousChar != null)
                {
                    regexPattern += string.Format("([^{0}]*?)({1})", Regex.Escape(previousChar + ""), Regex.Escape(p + ""));
                }
                else
                {
                    regexPattern += string.Format("(.*?)({0})", Regex.Escape(p + ""));
                }
                previousChar = p;
            }
            return regexPattern + "(.*)";
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