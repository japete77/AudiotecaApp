namespace fonoteca.Models.Player
{
    public class SeekInfo
    {
        public int CurrentIndex { get; set; }
        public string CurrentTitle { get; set; }
        public double CurrentTC { get; set; }
        public double CurrentSOM { get; set; }
        public int NavigationLevel { get; set; }
    }
}
