using System;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Attributes.Base;

namespace Apollo.Persistence.Test.Entity.Mock
{
    [EntityTable("reservation")]
    public class ReservationMock : BaseEntity<ReservationMock>
    {
        [EntityColumn("schedule_id")]
            public long ScheduleId { get; set; }

            [EntityColumnRef("schedule")]
            public ScheduleMock Schedule { get; set; }

            [EntityColumn("user_id")]
            public long UserId { get; set; }

            [EntityColumnRef("user")]
            public UserMock User { get; set; }

            [EntityColumnRef("ticket")]
            public TicketMock Ticket { get; set; }

            [EntityColumn("ticket_id")]
            public long TicketId { get; set; }

            public override object Clone()
            {
                var clone = (ReservationMock)MemberwiseClone();
                clone.Schedule = (ScheduleMock)Schedule?.Clone();
                clone.Ticket = (TicketMock) Schedule?.Clone();
                return clone;
            }

            public override bool Equals(ReservationMock other)
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
                return Equals((ReservationMock)obj);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Id, ScheduleId, UserId, TicketId);
            }
        }
}
