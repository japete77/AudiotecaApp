using System;
using System.Collections.Generic;

namespace audioteca.Models.Api
{
    public class AuthorsResult
    {
        public List<AuthorModel> Authors { get; set; }
        public int Total { get; set; }
    }
}
