﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModels
{
    public class User
    {
        public int Id { get; set; }
        public Guid UserID { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public List<string> Permissions { get; set; }
    }
}
