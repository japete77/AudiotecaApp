using System;
using System.Collections.Generic;

namespace audioteca.Models.Api
{
    public class TitleResult
    {
        public List<TitleModel> Titles { get; set; }
        public int Total { get; set; }
    }
}
