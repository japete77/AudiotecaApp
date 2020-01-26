using System;
using System.Collections.Generic;
using System.Text;

namespace audioteca.Models.Player
{
    public class BookMark
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public string Title { get; set; }
        public float TC { get; set; }
        public float SOM { get; set; }
        public string AbsoluteTC { get; set; }
    }
}
