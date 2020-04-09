using MDMProject.Models.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MDMProject.Models
{
    public class User : IdentityUser<int, UserLogin, UserRole, UserClaim>
    {
        public bool IsProfileFinished { get; set; }

        [StringLength(256)]
        public string Name { get; set; }

        [StringLength(256)]
        public string AdditionalComment { get; set; }

        [ForeignKey(nameof(Address))]
        public int? AddressId { get; set; }

        public virtual Address Address { get; set; }

        public virtual ICollection<ProtectiveEquipment> OfferedEquipment { get; set; }

        public virtual ICollection<OfferedHelp> OfferedHelp { get; set; }
    }
}