using System.Collections.Generic;
using Switcheroo.Core.Matchers;

namespace Switcheroo
{
    public class FilterResult
    {
        public AppWindow AppWindow { get; set; }
        public IList<MatchResult> WindowTitleMatchResults { get; set; }
        public IList<MatchResult> ProcessTitleMatchResults { get; set; }
    }
}