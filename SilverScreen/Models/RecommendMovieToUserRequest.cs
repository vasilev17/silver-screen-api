using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverScreen.Models
{
    public class RecommendMovieToUserRequest
    {
        public int friendId { get; set; }
        public int movieId { get; set; }
        public string message { get; set; }
    }
}
