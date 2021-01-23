using System;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Attributes.Base;

namespace Apollo.Domain.Entity
{
    [EntityTable("movie_actor")]
    public class MovieActor : BaseEntity<MovieActor>
    {
        [EntityColumn("movie_id")]
        public long MovieId { get; set; }
        [EntityColumnRef("movie")]
        public Movie Movie { get; set; }
        [EntityColumn("actor_id")]
        public long ActorId { get; set; }
        [EntityColumnRef("actor")]
        public Actor Actor { get; set; }

        public override bool Equals(MovieActor other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && MovieId == other.MovieId && ActorId == other.ActorId;
        }

        public override object Clone()
        {
            var clone = (MovieActor)MemberwiseClone();
            clone.Actor = (Actor)Actor?.Clone();
            return clone;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MovieActor) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, MovieId, ActorId);
        }
    }
}
