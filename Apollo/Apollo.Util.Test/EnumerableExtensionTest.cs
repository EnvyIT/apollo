using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace Apollo.Util.Test
{
    public class EnumerableExtensionTest
    {
        [Test]
        public void JoinNull_ShouldReturnEmptyString()
        {
            IEnumerable<KeyValuePair<int, string>> keyValuePairs = null;
            var queryString = keyValuePairs.JoinKeyValuePairs();
            queryString.Should().NotBeNull();
            queryString.Should().BeEmpty();
        }

        [Test]
        public void JoinOneKeyValuePair_ShouldReturnQueryParamWithQuestionMark()
        {
            var keyValuePairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("id",
                    "12")
            };
            var queryString = keyValuePairs.JoinKeyValuePairs();
            queryString.Should().NotBeEmpty();
            queryString.Should().Be("?id=12");
        }

        [Test]
        public void JoinMultipleKeyValuePairs_ShouldReturnQueryParamWithQuestionMarkAndAmpersand()
        {
            var keyValuePairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("id",
                    "12"),
                new KeyValuePair<string, string>("title",
                    "A movie title"),
                new KeyValuePair<string, string>("genre",
                    "Adventure")
            };
            var queryString = keyValuePairs.JoinKeyValuePairs();
            queryString.Should().NotBeEmpty();
            queryString.Should().Be("?id=12&title=A movie title&genre=Adventure");
        }

        [Test]
        public void JoinMultipleValues_ShouldReturnSlashSeparatedUrl()
        {
            var keyValuePairs = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("id",
                    "12"),
                new KeyValuePair<string, string>("title",
                    "A movie title"),
                new KeyValuePair<string, string>("genre",
                    "Adventure")
            };
            var queryString = keyValuePairs.JoinValues();
            queryString.Should().NotBeEmpty();
            queryString.Should().Be("12/A movie title/Adventure");
        }

        [Test]
        public void JoinValuesNull_ShouldReturnEmptyString()
        {
            IEnumerable<KeyValuePair<string, string>> keyValuePairs = null;
            var queryString = keyValuePairs.JoinKeyValuePairs();
            queryString.Should().NotBeNull();
            queryString.Should().BeEmpty();
        }

        [Test]
        public void ToPrettyString_ListNull_ShouldReturn_EmptyString()
        {
            IEnumerable<TestEntry> list = null;
            var result = list.ToPrettyString();
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Test]
        public void ToPrettyString_EmptyList_ShouldReturn_EmptyString()
        {
            var list = new List<TestEntry>();
            var result = list.ToPrettyString();
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Test]
        public void ToPrettyString_List_ShouldReturn_FormattedString()
        {
            var list = new List<TestEntry>
            {
                new TestEntry {Id = 1, Value = "One"},
                new TestEntry {Id = 2, Value = "Two"},
                new TestEntry {Id = 3, Value = null}
            };

            var result = list.ToPrettyString();
            var expected =
                $"[0] Id = 1 | Value = One{Environment.NewLine}[1] Id = 2 | Value = Two{Environment.NewLine}[2] Id = 3 | Value = (null)";

            result.Should().NotBeNull();
            result.Should().Be(expected);
        }

        private class TestEntry
        {
            public int Id { get; set; }
            public string Value { get; set; }
        }
    }
}