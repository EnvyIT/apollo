using System;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Attributes.Base;

namespace Apollo.Domain.Entity
{
    [EntityTable("reservation")]
    public class Reservation : BaseEntity<Reservation>
    {
        [EntityColumn("schedule_id")]
        public long ScheduleId { get; set; }

        [EntityColumnRef("schedule")]
        public Schedule Schedule { get; set; }

        [EntityColumn("user_id")]
        public long UserId { get; set; }

        [EntityColumnRef("user")]
        public User User { get; set; }

        [EntityColumnRef("ticket")]
        public Ticket Ticket { get; set; }

        [EntityColumn("ticket_id")]
        public long TicketId { get; set; }

        public override object Clone()
        {
            var clone = (Reservation) MemberwiseClone();
            clone.Schedule = (Schedule) Schedule?.Clone();
            clone.Ticket = (Ticket) Ticket?.Clone();
            return clone;
        }

        public override bool Equals(Reservation other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && ScheduleId == other.ScheduleId && Equals(Schedule, other.Schedule) && UserId == other.UserId && TicketId == other.TicketId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Reservation)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, ScheduleId, UserId, TicketId);
        }
    }
}