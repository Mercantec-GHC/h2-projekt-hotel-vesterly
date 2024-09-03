﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModels
{
    public class RoomType
    {
        public int Id { get; set; }
        public string RoomTypeName { get; set; }
        public List<string> Tags { get; set; }

        public List<Room> Rooms { get; set; }

    }
}
