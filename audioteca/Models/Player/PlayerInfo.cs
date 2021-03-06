﻿using MediaManager.Player;
using System;
using System.Collections.Generic;
using System.Text;

namespace audioteca.Models.Player
{
    public class PlayerInfo
    {
        public MediaPlayerState Status { get; set; }
        //public string Filename { get; set; }
        public string CurrentFilename { get; set; }
        public SeekInfo Position { get; set; }
        public List<Bookmark> Bookmarks { get; set; }
    }
}
