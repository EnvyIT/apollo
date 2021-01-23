using System;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Attributes.Base;

namespace Apollo.Domain.Entity
{
    [EntityTable("seat_reservation")]
    public class SeatReservation : BaseEntity<SeatReservation>
    {
        [EntityColumn("reservation_id")]
        public long ReservationId { get; set; }

        [EntityColumnRef("reservation")]
        public Reservation Reservation { get; set; }

        [EntityColumn("seat_id")]
        public long SeatId { get; set; }

        [EntityColumnRef("seat")]
        public Seat Seat { get; set; }

        public override object Clone()
        {
            var clone = (SeatReservation)MemberwiseClone();
            clone.Reservation = (Reservation)Reservation?.Clone();
            clone.Seat = (Seat)Seat?.Clone();
            return clone;
        }

        public override bool Equals(SeatReservation other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && ReservationId == other.ReservationId && SeatId == other.SeatId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SeatReservation)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, ReservationId, SeatId);
        }
    }
}
