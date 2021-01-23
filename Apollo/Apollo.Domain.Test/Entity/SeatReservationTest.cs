using System;
using Apollo.Domain.Entity;
using FluentAssertions;

namespace Apollo.Domain.Test.Entity
{
    public class SeatReservationTest : EntityTest<SeatReservation>
    {
        private readonly string _attributeTableName = "seat_reservation";
        private readonly string _attributeColumnReservationId = "reservation_id";
        private readonly string _attributeColumnSeatId = "seat_id";
        private readonly string _attributeColumnRefReservation = "reservation";
        private readonly string _attributeColumnRefSeat = "seat";

        private readonly long _id = 1L;
        private readonly DateTime _rowVersion = DateTime.UtcNow;
        private readonly long _reservationId = 10L;
        private readonly long _seatId = 20L;
        private readonly Reservation _reservation = new Reservation
        {
            Id = 30L,
            RowVersion = DateTime.UtcNow,
            UserId = 40L,
            ScheduleId = 50L
        };
        private readonly Seat _seat = new Seat
        {
            Id = 60L,
            RowVersion = DateTime.UtcNow,
            LayoutColumn = 4,
            LayoutRow = 5,
            Locked = true,
            Number = 20,
            RowId = 70L
        };

        private readonly long _cloneId = 10L;
        private readonly DateTime _cloneRowVersion = DateTime.UtcNow.AddMinutes(1);
        private readonly long _cloneReservationId = 11L;
        private readonly long _cloneSeatId = 21L;
        private readonly Reservation _cloneReservation = new Reservation
        {
            Id = 31L,
            RowVersion = DateTime.UtcNow,
            UserId = 41L,
            ScheduleId = 51L
        };
        private readonly Seat _cloneSeat = new Seat
        {
            Id = 61L,
            RowVersion = DateTime.UtcNow,
            LayoutColumn = 14,
            LayoutRow = 15,
            Locked = false,
            Number = 50,
            RowId = 71L
        };

        protected override void SetProperties(SeatReservation value)
        {
            value.Id = _id;
            value.RowVersion = _rowVersion;
            value.ReservationId = _reservationId;
            value.SeatId = _seatId;
            value.Reservation = _reservation;
            value.Seat = _seat;
        }

        protected override void SetCloneProperties(SeatReservation value)
        {
            value.Id = _cloneId;
            value.RowVersion = _cloneRowVersion;
            value.ReservationId = _cloneReservationId;
            value.SeatId = _cloneSeatId;
            value.Reservation = _cloneReservation;
            value.Seat = _cloneSeat;
        }

        protected override void CheckProperties(SeatReservation value)
        {
            value.Id.Should().Be(_id);
            value.RowVersion.Should().Be(_rowVersion);
            value.ReservationId.Should().Be(_reservationId);
            value.SeatId.Should().Be(_seatId);
            value.Reservation.Should().Be(_reservation);
            value.Seat.Should().Be(_seat);
        }

        protected override void CheckClonedProperties(SeatReservation value)
        {
            value.Id.Should().Be(_cloneId);
            value.RowVersion.Should().Be(_cloneRowVersion);
            value.ReservationId.Should().Be(_cloneReservationId);
            value.SeatId.Should().Be(_cloneSeatId);
            value.Reservation.Should().Be(_cloneReservation);
            value.Seat.Should().Be(_cloneSeat);
        }

        protected override int CalculateHashCode()
        {
            return HashCode.Combine(_id, _reservationId, _seatId);
        }

        protected override string GetAttributeTableName()
        {
            return _attributeTableName;
        }

        protected override void CheckAttributeColumns()
        {
            Attribute_Column_Name_Should(_ => _.ReservationId, _attributeColumnReservationId);
            Attribute_Column_Name_Should(_ => _.SeatId, _attributeColumnSeatId);
            Attribute_ColumnRef_Name_Should(_ => _.Reservation, _attributeColumnRefReservation);
            Attribute_ColumnRef_Name_Should(_ => _.Seat, _attributeColumnRefSeat);
        }
    }
}