using System;

namespace Apollo.Terminal.Types
{
    public class TicketData: IEquatable<TicketData>
    {
        public long TicketId { get; }

        public long ReservationId { get; }

        public long CinemaHallId { get; }

        public long MovieId { get; }

        public DateTime StartTime { get; }

        public TicketData(long ticketId, long reservationId, long cinemaHallId, long movieId, DateTime startTime)
        {
            TicketId = ticketId;
            ReservationId = reservationId;
            CinemaHallId = cinemaHallId;
            MovieId = movieId;
            StartTime = startTime;
        }

        public bool Equals(TicketData other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return TicketId == other.TicketId && ReservationId == other.ReservationId && CinemaHallId == other.CinemaHallId && MovieId == other.MovieId && StartTime.Equals(other.StartTime);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TicketData) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TicketId, ReservationId, CinemaHallId, MovieId, StartTime);
        }
    }
}
