using System;
using Apollo.Domain.Entity;
using FluentAssertions;

namespace Apollo.Domain.Test.Entity
{
    public class MovieTest : EntityTest<Movie>
    {
        private readonly string _attributeTableName = "movie";
        private readonly string _attributeColumnTitle = "title";
        private readonly string _attributeColumnDescription = "description";
        private readonly string _attributeColumnGenreId = "genre";
        private readonly string _attributeColumnDuration = "duration";
        private readonly string _attributeColunmImage = "image";
        private readonly string _attributeColumnTrailer = "trailer";
        private readonly string _attributeColumnRating = "rating";
        private readonly string _attributeColumnRefGenre = "genre";
        private readonly string _attributeColumnRefMovieActor = "movie_actor";

        private readonly long _id = 1L;
        private readonly DateTime _rowVersion = DateTime.UtcNow;
        private readonly string _title = "Movie 1";
        private readonly string _description = "Demo description 1";
        private readonly long _genreId = 1L;
        private readonly byte[] _image = { 1, 2, 3, 4 };
        private readonly long _duration = 120;
        private readonly string _trailer = "Trailer 1";
        private readonly int _rating = 3;
        private readonly MovieActor _movieActor = new MovieActor { Id = 1L, MovieId = 1L, ActorId = 2L };
        private readonly Genre _genre = new Genre { Id = 1L, Name = "Horror" };

        private readonly long _cloneId = 10L;
        private readonly DateTime _cloneRowVersion = DateTime.UtcNow.AddMinutes(1);
        private readonly string _cloneTitle = "Movie 15";
        private readonly string _cloneDescription = "Demo description 15";
        private readonly long _cloneGenreId = 1L;
        private readonly byte[] _cloneImage = { 6, 7, 8, 9 };
        private readonly long _cloneDuration = 210;
        private readonly string _cloneTrailer = "Trailer 5";
        private readonly int _cloneRating = 4;
        private readonly MovieActor _cloneMovieActor = new MovieActor { Id = 10L, MovieId = 10L, ActorId = 3L };
        private readonly Genre _cloneGenre = new Genre { Id = 2L, Name = "Drama" };

        protected override void SetProperties(Movie value)
        {
            value.Id = _id;
            value.RowVersion = _rowVersion;
            value.Title = _title;
            value.Description = _description;
            value.GenreId = _genreId;
            value.Image = _image;
            value.Duration = _duration;
            value.Trailer = _trailer;
            value.Rating = _rating;
            value.MovieActor = _movieActor;
            value.Genre = _genre;
        }

        protected override void SetCloneProperties(Movie value)
        {
            value.Id = _cloneId;
            value.RowVersion = _cloneRowVersion;
            value.Title = _cloneTitle;
            value.Description = _cloneDescription;
            value.GenreId = _cloneGenreId;
            value.Image = _cloneImage;
            value.Duration = _cloneDuration;
            value.Trailer = _cloneTrailer;
            value.Rating = _cloneRating;
            value.MovieActor = _cloneMovieActor;
            value.Genre = _cloneGenre;
        }

        protected override void CheckProperties(Movie value)
        {
            value.Id.Should().Be(_id);
            value.RowVersion.Should().Be(_rowVersion);
            value.Title.Should().Be(_title);
            value.Description.Should().Be(_description);
            value.GenreId.Should().Be(_genreId);
            value.Image.Equals(_image).Should().BeTrue();
            value.Duration.Should().Be(_duration);
            value.Trailer.Should().Be(_trailer);
            value.Rating.Should().Be(_rating);
            value.MovieActor.Should().Be(_movieActor);
            value.Genre.Should().Be(_genre);
        }

        protected override void CheckClonedProperties(Movie value)
        {
            value.Id.Should().Be(_cloneId);
            value.RowVersion.Should().Be(_cloneRowVersion);
            value.Title.Should().Be(_cloneTitle);
            value.Description.Should().Be(_cloneDescription);
            value.GenreId.Should().Be(_cloneGenreId);
            value.Image.Equals(_cloneImage).Should().BeTrue();
            value.Duration.Should().Be(_cloneDuration);
            value.Trailer.Should().Be(_cloneTrailer);
            value.Rating.Should().Be(_cloneRating);
            value.MovieActor.Should().Be(_cloneMovieActor);
            value.Genre.Should().Be(_cloneGenre);
        }

        protected override int CalculateHashCode()
        {
            return HashCode.Combine(_id, _title, _description, _genreId, _image, _duration, _trailer, _rating);
        }

        protected override string GetAttributeTableName()
        {
            return _attributeTableName;
        }

        protected override void CheckAttributeColumns()
        {
            Attribute_Column_Name_Should(_ => _.Description, _attributeColumnDescription);
            Attribute_Column_Name_Should(_ => _.Duration, _attributeColumnDuration);
            Attribute_Column_Name_Should(_ => _.GenreId, _attributeColumnGenreId);
            Attribute_Column_Name_Should(_ => _.Image, _attributeColunmImage);
            Attribute_Column_Name_Should(_ => _.Title, _attributeColumnTitle);
            Attribute_Column_Name_Should(_ => _.Trailer, _attributeColumnTrailer);
            Attribute_Column_Name_Should(_ => _.Rating, _attributeColumnRating);
            Attribute_ColumnRef_Name_Should(_ => _.Genre, _attributeColumnRefGenre);
            Attribute_ColumnRef_Name_Should(_ => _.MovieActor, _attributeColumnRefMovieActor);
        }
    }
}