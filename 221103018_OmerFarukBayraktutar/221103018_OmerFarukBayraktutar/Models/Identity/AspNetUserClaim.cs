using System;
using System.Collections.Generic;

namespace _221103018_OmerFarukBayraktutar.Models.Identity;

public partial class AspNetUserClaim
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string ClaimType { get; set; }

    public string ClaimValue { get; set; }

    public virtual AspNetUser User { get; set; }
}
