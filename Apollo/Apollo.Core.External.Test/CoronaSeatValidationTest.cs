using System;
using System.Collections.Generic;
using System.Linq;
using Apollo.Core.Dto;
using Apollo.Core.Validation;
using FluentAssertions;
using NUnit.Framework;

namespace Apollo.Core.External.Test
{
    public class CoronaSeatValidationTest
    {
        private ISeatValidation _rule;
        private List<SeatDto> _seatData;

        [SetUp]
        public void Setup()
        {
            _rule = new CoronaRule();
            _seatData = new List<SeatDto>();
            for (int i = 0; i < 20; i++)
            {
                _seatData.Add(new SeatDto
                {
                    Id = i + 1,
                    RowId = (long) i / 10,
                    Number = i + 1,
                    State = SeatState.Free,
                    LayoutRow = 0,
                    LayoutColumn = i
                });
            }
        }

        [Test]
        public void Test_ConnectedSeatBlock_Should_BeValid()
        {
            var result = _rule.IsValid(_seatData, GetDesiredSeats(0, 1, 2));
            result.Should().BeTrue();
        }

        [Test]
        public void Test_InvalidArguments_Should_ThrowException()
        {
            Action callback = () => _rule.IsValid(Enumerable.Empty<SeatDto>(), GetDesiredSeats(0, 1));
            callback.Should().Throw<ArgumentException>();

            callback = () => _rule.IsValid(_seatData, Enumerable.Empty<SeatDto>());
            callback.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Test_MultipleSeats_BetweenFree_ShouldBe_Valid()
        {
            SetSeatOccupied(0, 1, 2);

            var result = _rule.IsValid(_seatData, GetDesiredSeats(7, 8, 9));

            result.Should().BeTrue();
        }

        [Test]
        public void Test_MultipleSeats_BetweenLocked_ShouldBe_Valid()
        {
            SetSeatOccupied(0, 1, 2);
            SetSeatLocked(3, 4, 5, 6);

            var result = _rule.IsValid(_seatData, GetDesiredSeats(7, 8, 9));
            result.Should().BeTrue();
        }

        [Test]
        public void Test_OneSeat_BetweenFree_ShouldBe_Valid()
        {
            SetSeatOccupied(0);

            var result = _rule.IsValid(_seatData, GetDesiredSeats(2));
            result.Should().BeTrue();
        }

        [Test]
        public void Test_OneSeat_BetweenLocked_ShouldBe_Valid()
        {
            SetSeatOccupied(0);
            SetSeatLocked(1);

            var result = _rule.IsValid(_seatData, GetDesiredSeats(2));
            result.Should().BeTrue();
        }

        [Test]
        public void Test_NoSeat_BetweenFree_ShouldBe_Invalid()
        {
            SetSeatOccupied(0);

            var result = _rule.IsValid(_seatData, GetDesiredSeats(1, 2));
            result.Should().BeFalse();
        }

        [Test]
        public void Test_LeftSide_OneBetweenFree_RightSiteNot_ShouldBeInvalid()
        {
            SetSeatOccupied(0, 1, 2, 7, 8);

            var result = _rule.IsValid(_seatData, GetDesiredSeats(4, 5, 6));
            result.Should().BeFalse();
        }

        [Test]
        public void Test_MultiRow_BetweenFree_ShouldBeValid()
        {
            SetSeatOccupied(0, 1, 2, 7, 8, 12, 16, 17);

            var result = _rule.IsValid(_seatData, GetDesiredSeats(4, 5, 14));
            result.Should().BeTrue();
        }

        [Test]
        public void Test_MultiRow_OneInvalid_ShouldBeInValid()
        {
            SetSeatOccupied(0, 1, 2, 7, 8, 13, 16, 17);

            var result = _rule.IsValid(_seatData, GetDesiredSeats(4, 5, 14));
            result.Should().BeFalse();
        }

        private void SetSeatOccupied(params int[] indices)
        {
            SetSeatState(SeatState.Occupied, indices);
        }

        private void SetSeatLocked(params int[] indices)
        {
            SetSeatState(SeatState.Locked, indices);
        }

        private void SetSeatState(SeatState state, params int[] indices)
        {
            foreach (var index in indices)
            {
                _seatData[index].State = state;
            }
        }

        private IEnumerable<SeatDto> GetDesiredSeats(params int[] indices)
        {
            return indices.Select(index => _seatData[index]);
        }
    }
}