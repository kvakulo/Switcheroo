using System.Collections.Generic;
using System.Linq;
using Switcheroo.Core.Matchers;

namespace Switcheroo.Core
{
    public class WindowFilterer
    {
        public IEnumerable<FilterResult> Filter(IEnumerable<AppWindow> windows, string filterText)
        {
            return windows
                .Select(w => new { Window = w, ResultsTitle = Score(w.Title, filterText), ResultsProcessTitle = Score(w.ProcessTitle, filterText) })
                .Where(r => r.ResultsTitle.Any(wt => wt.Matched) || r.ResultsProcessTitle.Any(pt => pt.Matched))
                .OrderByDescending(r => r.ResultsTitle.Sum(wt => wt.Score) + r.ResultsProcessTitle.Sum(pt => pt.Score))
                .Select(r => new FilterResult { AppWindow = r.Window, WindowTitleMatchResults = r.ResultsTitle, ProcessTitleMatchResults = r.ResultsProcessTitle });
        }

        private static List<MatchResult> Score(string title, string filterText)
        {
            var startsWithMatcher = new StartsWithMatcher();
            var containsMatcher = new ContainsMatcher();
            var significantCharactersMatcher = new SignificantCharactersMatcher();
            var individualCharactersMatcher = new IndividualCharactersMatcher();

            var results = new List<MatchResult>
            {
                startsWithMatcher.Evaluate(title, filterText),
                significantCharactersMatcher.Evaluate(title, filterText),
                containsMatcher.Evaluate(title, filterText),
                individualCharactersMatcher.Evaluate(title, filterText)
            };

            return results;
        }
    }
}
