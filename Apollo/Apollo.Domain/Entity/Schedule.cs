using System;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Attributes.Base;

namespace Apollo.Domain.Entity
{
    [EntityTable("schedule")]
    public class Schedule : BaseEntity<Schedule>
    {
        [EntityColumn("movie")]
        public long MovieId { get; set; }

        [EntityColumnRef("movie")]
        public Movie Movie { get; set; }

        [EntityColumn("cinema_hall")]
        public long CinemaHallId { get; set; }

        [EntityColumnRef("cinema_hall")]
        public CinemaHall CinemaHall { get; set; }

        [EntityColumn("price")]
        public decimal Price { get; set; }

        [EntityColumn("start_time")]
        public DateTime StartTime { get; set; }

        public override object Clone()
        {
            var clone = (Schedule)MemberwiseClone();
            clone.CinemaHall = (CinemaHall)CinemaHall?.Clone();
            clone.Movie = (Movie)Movie?.Clone();
            clone.StartTime = new DateTime(StartTime.Year, StartTime.Month, StartTime.Day, StartTime.Hour, StartTime.Minute, StartTime.Second, StartTime.Millisecond);
            return clone;
        }

        public override bool Equals(Schedule other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && MovieId == other.MovieId && CinemaHallId == other.CinemaHallId && Price == other.Price && StartTime == other.StartTime;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Schedule)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, CinemaHallId, MovieId, Price, StartTime);
        }
    }
}