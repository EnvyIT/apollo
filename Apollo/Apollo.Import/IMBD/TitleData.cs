using System.Collections.Generic;

namespace Apollo.Import.IMBD
{
    public class TitleData
    {
        public string Id { get; set; }
        public string Title { set; get; }
        public string FullTitle { set; get; }
        public string ReleaseDate { set; get; }
        public string RuntimeMins { set; get; }
        public string Plot { set; get; }
        public string PlotLocal { set; get; }
        public string Image { get; set; }
        public string Genres { set; get; }
        public List<KeyValueItem> GenreList { get; set; }
        public ImageData Images { get; set; }
        public TrailerData Trailer { get; set; }
        public string IMDbRating { get; set; }
        public List<ActorShort> ActorList { get; set; }
    }

    public class ActorShort
    {
        public string Id { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string AsCharacter { get; set; }
    }

    public class ImageData
    {
        public string ImbdId { get; set; }
        public string Title { get; set; }
        public string FullTitle { get; set; }
        public string Type { get; set; }
        public string Year { get; set; }
        public IEnumerable<ImageDataDetail> Items { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ImageDataDetail
    {
        public string Title { get; set; }
        public string Image { get; set; }
    }

    public class KeyValueItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
