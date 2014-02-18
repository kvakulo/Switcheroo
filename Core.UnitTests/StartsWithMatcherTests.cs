using System.Linq;
using NUnit.Framework;
using Switcheroo.Core.Matchers;

namespace Switcheroo.Core.UnitTests
{
    [TestFixture]
    class StartsWithMatcherTests
    {
        [Test]
        public void Evaluate_InputStartsWithPattern_ResultIsMatched()
        {
            var input = "google chrome";
            var pattern = "google";

            var result = Evaluate(input, pattern);

            Assert.That(result.Matched, Is.True);
        }

        [Test]
        public void Evaluate_InputStartsWithPattern_ScoreIsFour()
        {
            var input = "google chrome";
            var pattern = "google";

            var result = Evaluate(input, pattern);

            Assert.That(result.Score, Is.EqualTo(4));
        }

        [Test]
        public void Evaluate_InputStartsWithPattern_FirstStringPartIsMatch()
        {
            var input = "google chrome";
            var pattern = "google";

            var result = Evaluate(input, pattern);

            Assert.That(result.StringParts.First().Value, Is.EqualTo("google"));
            Assert.That(result.StringParts.First().IsMatch, Is.True);
        }

        [Test]
        public void Evaluate_InputStartsWithPattern_SecondStringPartIsNotMatch()
        {
            var input = "google chrome";
            var pattern = "google";

            var result = Evaluate(input, pattern);

            Assert.That(result.StringParts.ToList()[1].Value, Is.EqualTo(" chrome"));
            Assert.That(result.StringParts.ToList()[1].IsMatch, Is.False);
        }

        [Test]
        public void Evaluate_NullInput_ReturnsNotMatchingResult()
        {
            var result = Evaluate(null, "google");
            Assert.That(result.Matched, Is.False);
        }

        [Test]
        public void Evaluate_NullPattern_ReturnsNotMatchingResult()
        {
            var result = Evaluate("google chrome", null);
            Assert.That(result.Matched, Is.False);
        }

        [Test]
        public void Evaluate_NullPattern_ReturnsOneNonMatchingStringPart()
        {
            var result = Evaluate("google chrome", null);
            Assert.That(result.StringParts.Count(), Is.EqualTo(1));
            Assert.That(result.StringParts.First().Value, Is.EqualTo("google chrome"));
            Assert.That(result.StringParts.First().IsMatch, Is.False);
        }

        [Test]
        public void Evaluate_InputContainsPattern_ReturnsNotMatchingResult()
        {
            var result = Evaluate("google chrome", "chrome");
            Assert.That(result.Matched, Is.False);
        }

        [Test]
        public void Evaluate_InputContainsPattern_ReturnsOneNonMatchingStringPart()
        {
            var result = Evaluate("google chrome", "chrome");
            Assert.That(result.StringParts.Count(), Is.EqualTo(1));
            Assert.That(result.StringParts.First().Value, Is.EqualTo("google chrome"));
            Assert.That(result.StringParts.First().IsMatch, Is.False);
        }

        [Test]
        public void Evaluate_InputStartsWithPattern_CasingIsNotChanged()
        {
            var result = Evaluate("Google Chrome", "google");
            Assert.That(result.StringParts[0].Value, Is.EqualTo("Google"));
            Assert.That(result.StringParts[1].Value, Is.EqualTo(" Chrome"));
        }

        private static MatchResult Evaluate(string input, string pattern)
        {
            var matcher = new StartsWithMatcher();
            return matcher.Evaluate(input, pattern);
        }
    }
}
