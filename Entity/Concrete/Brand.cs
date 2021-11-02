﻿using Core.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entity.Concrete
{
    public class Brand : IEntity
    {
        [Key]
        public int BrandId { get; set; }
        public string BrandName { get; set; }

    }
}
