﻿using System;
using System.Collections.Generic;

namespace _221103018_OmerFarukBayraktutar.Models.Identity;

public partial class AspNetRole
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string NormalizedName { get; set; }

    public string ConcurrencyStamp { get; set; }

    public virtual ICollection<AspNetRoleClaim> AspNetRoleClaims { get; set; } = new List<AspNetRoleClaim>();

    public virtual ICollection<AspNetUser> Users { get; set; } = new List<AspNetUser>();
}
