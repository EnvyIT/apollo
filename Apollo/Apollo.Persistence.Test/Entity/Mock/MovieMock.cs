using System;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Attributes.Base;

namespace Apollo.Persistence.Test.Entity.Mock
{
    [EntityTable("movie")]
    public class MovieMock : BaseEntity<MovieMock>
    {
        [EntityColumn("title")]
        public string Title { get; set; }
        [EntityColumn("description")]
        public string Description { get; set; }
        [EntityColumnRef("genre")]
        public GenreMock GenreMock { get; set; }
        [EntityColumn("genre")]
        public long GenreId { get; set; }
        [EntityColumn("duration")]
        public long Duration { get; set; }
        [EntityColumn("image")]
        public byte[] Image { get; set; }
        [EntityColumn("trailer")]
        public string Trailer { get; set; }
        [EntityColumn("rating")]
        public int Rating { get; set; }
        [EntityColumnRef("movie_actor")]
        public MovieActorMock MovieActorMock { get; set; }

        public override bool Equals(MovieMock other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Title == other.Title && Description == other.Description &&  Duration == other.Duration && Equals(Image, other.Image) && Trailer == other.Trailer && Id == other.Id && GenreId == other.GenreId && Rating == other.Rating;
        }

        public override object Clone()
        {
            var clone = (MovieMock)MemberwiseClone();
            clone.GenreMock = (GenreMock)GenreMock?.Clone();
            clone.MovieActorMock = (MovieActorMock)MovieActorMock?.Clone();
            return clone;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MovieMock)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, GenreId, Trailer, Description, Duration, Image, Title, Rating);
        }
    }
}
