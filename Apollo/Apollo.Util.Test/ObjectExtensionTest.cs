using FluentAssertions;
using NUnit.Framework;

namespace Apollo.Util.Test
{
    public class ObjectExtensionTest
    {
        [Test]
        public void ToPrettyString_Null_ShouldReturn_EmptyString()
        {
            TestEntry entry = null;
            var result = entry.ToPrettyString();
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Test]
        public void ToPrettyString_Entry_ShouldReturn_FormattedString()
        {
            var entry = new TestEntry {Id = 1, Value = "One"};
          
            var result = entry.ToPrettyString();
            const string expected = "Id = 1 | Value = One";

            result.Should().NotBeNull();
            result.Should().Be(expected);
        }

        [Test]
        public void ToPrettyString_EntryWithNull_ShouldReturn_FormattedString()
        {
            var entry = new TestEntry {Id = 3, Value = null};
            var result = entry.ToPrettyString();
            const string expected = "Id = 3 | Value = (null)";

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
