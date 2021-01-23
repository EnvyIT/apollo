using System;
using System.Linq;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Attributes.Base;

namespace Apollo.Domain.Entity
{
    [EntityTable("movie")]
    public class Movie : BaseEntity<Movie>
    {
        [EntityColumn("title")]
        public string Title { get; set; }
        [EntityColumn("description")]
        public string Description { get; set; }
        [EntityColumnRef("genre")]
        public Genre Genre { get; set; }
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
        public MovieActor MovieActor { get; set; }

        public override bool Equals(Movie other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && Title == other.Title && Description == other.Description && GenreId == other.GenreId && Duration == other.Duration && (Image ?? Array.Empty<byte>()).SequenceEqual(other.Image ?? Array.Empty<byte>()) && Trailer == other.Trailer && Rating == other.Rating;
        }

        public override object Clone()
        {
            var clone = (Movie)MemberwiseClone();
            clone.Genre = (Genre)Genre?.Clone();
            clone.MovieActor = (MovieActor)MovieActor?.Clone();
            return clone;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Movie)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Title, Description, GenreId, Image, Duration, Trailer, Rating);
        }
    }
}
