using System.Collections.Generic;
using System.Collections.ObjectModel;
using FluentAssertions;
using NUnit.Framework;

namespace Apollo.Util.Test
{
    public class ObservableCollectionExtensionTest
    {

        [Test]
        public void AddRangeToObservableCollection_ShouldReturnCorrectAmountAfterAdding()
        {
            var numbers = new List<int> {1, 2, 3, 4, 5};
            var observableCollection = new ObservableCollection<int>();
            observableCollection.AddRange(numbers);
            observableCollection.Count.Should().Be(numbers.Count);
        } 

    }
}
