using System;
using System.Collections.Generic;
using System.Text;

namespace audioteca.Models.Player
{
    public class SeekInfo
    {
        public int CurrentIndex { get; set; }
        public string CurrentTitle { get; set; }
        public float CurrentTC { get; set; }
        public string AbsoluteTC { get; set; }
        public float CurrentSOM { get; set; }
        public int NavigationLevel { get; set; }
    }
}
