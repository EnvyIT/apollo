using System;
using Apollo.Domain.Entity;
using FluentAssertions;

namespace Apollo.Domain.Test.Entity
{
    public class ReservationTest : EntityTest<Reservation>
    {
        private readonly string _attributeTableName = "reservation";
        private readonly string _attributeColumnScheduleId = "schedule_id";
        private readonly string _attributeColumnUserId = "user_id";
        private readonly string _attributeColumnTicketId = "ticket_id";
        private readonly string _attributeColumnRefSchedule = "schedule";
        private readonly string _attributeColumnRefUser = "user";
        private readonly string _attributeColumnRefTicket= "ticket";

        private readonly long _id = 1L;
        private readonly DateTime _rowVersion = DateTime.UtcNow;
        private readonly long _scheduleId = 10L;
        private readonly long _userId = 20L;
        private readonly Schedule _schedule = new Schedule { Id = 30L, RowVersion = DateTime.UtcNow, CinemaHallId = 40L, MovieId = 50L };
        private readonly User _user = new User { Id = 60L, RowVersion = DateTime.UtcNow, FirstName = "First", LastName = "Last" };
        private readonly Ticket _ticket = new Ticket { Id = 33333L, RowVersion = DateTime.UtcNow, Printed = DateTime.UtcNow };

        private readonly long _cloneId = 10L;
        private readonly DateTime _cloneRowVersion = DateTime.UtcNow.AddMinutes(1);
        private readonly long _cloneScheduleId = 11L;
        private readonly long _cloneUserId = 21L;
        private readonly Schedule _cloneSchedule = new Schedule { Id = 31L, RowVersion = DateTime.UtcNow, CinemaHallId = 41L, MovieId = 51L };
        private readonly User _cloneUser = new User { Id = 61L, RowVersion = DateTime.UtcNow, FirstName = "First clone", LastName = "Last clone" };
        private readonly Ticket _cloneTicket = new Ticket { Id = 1234124, RowVersion = DateTime.UtcNow.AddMinutes(1), Printed = DateTime.UtcNow.AddMinutes(1) };

        protected override void SetProperties(Reservation value)
        {
            value.Id = _id;
            value.RowVersion = _rowVersion;
            value.ScheduleId = _scheduleId;
            value.UserId = _userId;
            value.Schedule = _schedule;
            value.User = _user;
            value.TicketId = _ticket.Id;
            value.Ticket = _ticket;
        }

        protected override void SetCloneProperties(Reservation value)
        {
            value.Id = _cloneId;
            value.RowVersion = _cloneRowVersion;
            value.ScheduleId = _cloneScheduleId;
            value.UserId = _cloneUserId;
            value.Schedule = _cloneSchedule;
            value.User = _cloneUser;
            value.TicketId = _cloneTicket.Id;
            value.Ticket = _cloneTicket;
        }

        protected override void CheckProperties(Reservation value)
        {
            value.Id.Should().Be(_id);
            value.RowVersion.Should().Be(_rowVersion);
            value.ScheduleId.Should().Be(_scheduleId);
            value.UserId.Should().Be(_userId);
            value.Schedule.Should().Be(_schedule);
            value.User.Should().Be(_user);
            value.Ticket.Should().Be(_ticket);
            value.TicketId.Should().Be(_ticket.Id);
        }

        protected override void CheckClonedProperties(Reservation value)
        {
            value.Id.Should().Be(_cloneId);
            value.RowVersion.Should().Be(_cloneRowVersion);
            value.ScheduleId.Should().Be(_cloneScheduleId);
            value.UserId.Should().Be(_cloneUserId);
            value.Schedule.Should().Be(_cloneSchedule);
            value.User.Should().Be(_cloneUser);
            value.Ticket.Should().Be(_cloneTicket);
            value.TicketId.Should().Be(_cloneTicket.Id);
        }

        protected override int CalculateHashCode()
        {
            return HashCode.Combine(_id, _scheduleId, _userId, _ticket.Id);
        }

        protected override string GetAttributeTableName()
        {
            return _attributeTableName;
        }

        protected override void CheckAttributeColumns()
        {
            Attribute_Column_Name_Should(_ => _.ScheduleId, _attributeColumnScheduleId);
            Attribute_Column_Name_Should(_ => _.UserId, _attributeColumnUserId);
            Attribute_Column_Name_Should(_ => _.TicketId, _attributeColumnTicketId);
            Attribute_ColumnRef_Name_Should(_ => _.Schedule, _attributeColumnRefSchedule);
            Attribute_ColumnRef_Name_Should(_ => _.User, _attributeColumnRefUser);
            Attribute_ColumnRef_Name_Should(_ => _.Ticket, _attributeColumnRefTicket);
        }
    }
}