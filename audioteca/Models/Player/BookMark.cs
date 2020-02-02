using System;
using System.Collections.Generic;
using System.Text;

namespace audioteca.Models.Player
{
    public class Bookmark
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public string Title { get; set; }
        public double TC { get; set; }
        public double SOM { get; set; }
        public string AbsoluteTC { get; set; }
    }
}
