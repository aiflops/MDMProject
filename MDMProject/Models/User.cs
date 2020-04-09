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
        public string FirstName { get; set; }

        [StringLength(256)]
        public string LastName { get; set; }

        [StringLength(256)]
        public string AdditionalComment { get; set; }

        [ForeignKey(nameof(Address))]
        public int? AddressId { get; set; }

        public virtual Address Address { get; set; }

        public ICollection<ProtectiveEquipment> OfferedEquipment { get; set; }

        public ICollection<OfferedHelp> OfferedHelp { get; set; }
    }
}