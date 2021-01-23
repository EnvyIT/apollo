using System;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Attributes.Base;

namespace Apollo.Persistence.Test.Entity.Mock
{
    [EntityTable("movie_actor")]
    public class MovieActorMock : BaseEntity<MovieActorMock>
    {
        [EntityColumn("movie_id", true)]
        public long MovieId { get; set; }
        [EntityColumnRef("movie")]
        public MovieMock MovieMock { get; set; }
        [EntityColumn("actor_id", true)]
        public long ActorId { get; set; }
        [EntityColumnRef("actor")]
        public ActorMock ActorMock { get; set; }

        public override bool Equals(MovieActorMock other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return MovieId == other.MovieId && ActorId == other.ActorId;
        }

        public override object Clone()
        {
            var clone = (MovieActorMock)MemberwiseClone();
            clone.ActorMock = (ActorMock)ActorMock?.Clone();
            return clone;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MovieActorMock)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, MovieId, ActorId);
        }
    }
}
