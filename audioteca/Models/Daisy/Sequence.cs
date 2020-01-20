namespace audioteca.Models.Daisy
{
    public class Sequence
    {
        public string Id { get; set; }
        public string Filename { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
        public float SOM { get; set; }
        public float TCIn { get; set; }
        public float TCOut { get; set; }
    }
}
