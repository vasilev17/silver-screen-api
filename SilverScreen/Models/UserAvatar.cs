using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilverScreen.Models
{
    public class UserAvatar
    {
        public IFormFile avatar { get; set; }
    }
}
