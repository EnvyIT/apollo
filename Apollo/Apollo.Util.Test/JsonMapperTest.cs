using System;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Apollo.Util.Test
{
    public class JsonMapperTest
    {
        private class Person
        {
            public int Id { get; }
            public string FirstName { get; }
            public string LastName { get; }
            public Person(int id, string firstName, string lastName)
            {
                Id = id;
                FirstName = firstName;
                LastName = lastName;
            }
        }

        private class Movie
        {
            public int Id { get; }
            public string Name { get; }
            public Movie(int id, string name)
            {
                Id = id; 
                Name = name;
            }
        }

        [Test]
        public void JsonMapper_Deserialize_ShouldMapCorrectEntity()
        {
            var json = "{\"id\":1337, \"firstName\":\"Max\", \"lastName\":\"Mustermann\"}";
            var actualResult = JsonMapper.Map<Person>(json);
            actualResult.FirstName.Should().Be("Max");
            actualResult.LastName.Should().Be("Mustermann");
            actualResult.Id.Should().Be(1337);
        }

        [Test]
        public void JsonMapper_Deserialize_ShouldMapCorrectEntity2()
        {
            var json = "{\"id\":19, \"Name\":\"Horror\"}";
            var actualResult = JsonMapper.Map<Movie>(json);
            actualResult.Name.Should().Be("Horror");
            actualResult.Id.Should().Be(19);
        }

        [Test]
        public void JsonMapper_Deserialize_ShouldMapCorrectEntity3()
        {
            var movie = new Movie(1, "New One");
            var json = JsonConvert.SerializeObject(movie);
            var actualResult = JsonMapper.Map<Movie>(json);
            actualResult.Name.Should().Be("New One");
            actualResult.Id.Should().Be(1);
        }

        [Test]
        public void JsonMapper_Deserialize_ShouldMapCorrectEntity4()
        {
            var person = new Person(42, "Maximilian", "Muster");
            var json = JsonConvert.SerializeObject(person);
            var actualResult = JsonMapper.Map<Person>(json);
            actualResult.FirstName.Should().Be("Maximilian");
            actualResult.LastName.Should().Be("Muster");
            actualResult.Id.Should().Be(42);
        }


        [Test]
        public void JsonMapper_Deserialize_TestSameObject()
        {
            var json = "{\"id\":1337, \"firstName\":\"Max\", \"lastName\":\"Mustermann\"}";
            var actualPerson = JsonMapper.Map<Person>(json);
            var areEqual = actualPerson.Equals(actualPerson);
            areEqual.Should().BeTrue();
        }

        [Test]
        public void JsonMapper_Deserialize_TestNullObject()
        {
            var json = "{\"id\":1337, \"firstName\":\"Max\", \"lastName\":\"Mustermann\"}";
            var actualPerson = JsonMapper.Map<Person>(json);
            var areEqual = actualPerson.Equals(null);
            areEqual.Should().BeFalse();
        }

        [Test]
        public void JsonMapper_Deserialize_TestOtherType()
        {
            var json = "{\"id\":1337, \"firstName\":\"Max\", \"lastName\":\"Mustermann\"}";
            var json2 = "{\"id\":1234, \"Name\":\"Horror-Adventure\"}";
            var actualPerson = JsonMapper.Map<Person>(json);
            var actualMovie = JsonMapper.Map<Movie>(json2);
            var areEqual = actualPerson.Equals(actualMovie);
            areEqual.Should().BeFalse();
        }
        
        [Test]
        public void JsonMapper_Serialize_ShouldMapCorrectEntity1()
        {
            var actualResult = JsonMapper.Map<Person>(new Person(1337, "Max", "Mustermann"));
            actualResult.Should().Be("{\"Id\":1337,\"FirstName\":\"Max\",\"LastName\":\"Mustermann\"}");
        }

        [Test]
        public void JsonMapper_Serialize_ShouldMapCorrectEntity2()
        {
            var actualResult = JsonMapper.Map<Movie>(new Movie(19, "Horror"));
            actualResult.Should().Be("{\"Id\":19,\"Name\":\"Horror\"}");
        }

        [Test]
        public void JsonMapper_Serialize_Null_ShouldThrow()
        {
            Action callback = () => JsonMapper.Map<Movie>((string)null);
            callback.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void JsonMapper_Serialize_ShouldMapEmpty_ToNull()
        {
            var actualResult = JsonMapper.Map<Movie>("");
            actualResult.Should().BeNull();
        }
    }
}
