using NUnit.Framework;
using Switcheroo.Core.Matchers;

namespace Switcheroo.Core.UnitTests
{
    [TestFixture]
    public class IndividualCharactersMatcherTests
    {
        [Test]
        public void Evaluate_InputNull_ReturnsNonMatchingResult()
        {
            var result = Evaluate(null, "crm");
            Assert.That(result.Matched, Is.False);
        }

        [Test]
        public void Evaluate_InputNull_ReturnsNoStringParts()
        {
            var result = Evaluate(null, "crm");
            Assert.That(result.StringParts.Count, Is.EqualTo(0));
        }

        [Test]
        public void Evaluate_PatternNull_ReturnsNonMatchingResult()
        {
            var result = Evaluate("chrome", null);
            Assert.That(result.Matched, Is.False);
        }

        [Test]
        public void Evaluate_PatternNull_ReturnsOneNonMatchingStringPart()
        {
            var result = Evaluate("chrome", null);
            Assert.That(result.StringParts.Count, Is.EqualTo(1));
            Assert.That(result.StringParts[0].Value, Is.EqualTo("chrome"));
        }

        [Test]
        public void Evaluate_InputContainsCharacter_ReturnsMatchingResult()
        {
            var result = Evaluate("chrome", "r");
            Assert.That(result.Matched, Is.True);
        }

        [Test]
        public void Evaluate_InputContainsCharacter_ReturnsCorrectScore()
        {
            var result = Evaluate("chrome", "r");
            Assert.That(result.Score, Is.EqualTo(1));
        }

        [Test]
        public void Evaluate_InputContainsCharacter_ReturnsThreeStringParts()
        {
            var result = Evaluate("chrome", "r");
            Assert.That(result.StringParts.Count, Is.EqualTo(3));
            Assert.That(result.StringParts[0].Value, Is.EqualTo("ch"));
            Assert.That(result.StringParts[1].Value, Is.EqualTo("r"));
            Assert.That(result.StringParts[2].Value, Is.EqualTo("ome"));
        }

        [Test]
        public void Evaluate_InputContainsTwoCharacters_ReturnsFourStringParts()
        {
            var result = Evaluate("chrome", "re");
            Assert.That(result.StringParts.Count, Is.EqualTo(4));
            Assert.That(result.StringParts[0].Value, Is.EqualTo("ch"));
            Assert.That(result.StringParts[1].Value, Is.EqualTo("r"));
            Assert.That(result.StringParts[2].Value, Is.EqualTo("om"));
            Assert.That(result.StringParts[3].Value, Is.EqualTo("e"));
        }

        [Test]
        public void Evaluate_InputContainsTwoCharacters2_ReturnsFourStringParts()
        {
            var result = Evaluate("chrome", "ce");
            Assert.That(result.StringParts.Count, Is.EqualTo(3));
            Assert.That(result.StringParts[0].Value, Is.EqualTo("c"));
            Assert.That(result.StringParts[1].Value, Is.EqualTo("hrom"));
            Assert.That(result.StringParts[2].Value, Is.EqualTo("e"));
        }

        private static MatchResult Evaluate(string input, string pattern)
        {
            return new IndividualCharactersMatcher().Evaluate(input, pattern);
        }
    }
}