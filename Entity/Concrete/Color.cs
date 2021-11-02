﻿using Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entity.Concrete
{
    public class Color : IEntity
    {
        [Key]
        public int ColorId { get; set; }
        public string ColorName { get; set; }

    }
}
