using MDMProject.Models.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MDMProject.Models
{
    public class User : IdentityUser<int, UserLogin, UserRole, UserClaim>
    {
        // General details
        public UserTypeEnum UserType { get; set; }

        public DateTime? CreatedDate { get; set; } // TODO: fill during registration

        public DateTime? ProfileFinishedDate { get; set; }

        [StringLength(ValidationConstants.User.MAX_ADDITIONAL_COMMENT_LENGTH)]
        public string AdditionalComment { get; set; }

        // Individual details
        // TODO: Required if userType == Individual Name
        [StringLength(ValidationConstants.User.MAX_CONTACT_NAME_LENGTH)]
        public string IndividualName { get; set; }

        // Company details
        // TODO: Required if userType == Company
        [StringLength(ValidationConstants.User.MAX_COMPANY_NAME_LENGTH)]
        public string CompanyName { get; set; }

        // // TODO: NOT!!! Required if userType == Company
        [StringLength(ValidationConstants.User.MAX_CONTACT_NAME_LENGTH)]
        public string ContactPersonName { get; set; }

        // Address details
        [ForeignKey(nameof(Address))]
        public int? AddressId { get; set; }

        public virtual Address Address { get; set; }

        // Coordinator details (when user is Collection Point)
        //[ForeignKey(nameof(Coordinator))]
        public int? CoordinatorId { get; set; }

        public virtual User Coordinator { get; set; }

        [StringLength(ValidationConstants.User.MAX_COORDINATOR_DETAILS_LENGTH)]
        public string OtherCoordinatorDetails { get; set; }

        public virtual User ApprovedBy { get; set; } // Person who approved the account

        public DateTime? ApprovedDate { get; set; }

        // Coordinator details (when user is Coordinator)
        // TODO: required if role == coordinator
        [StringLength(ValidationConstants.User.MAX_COORDINATED_REGION_LENGTH)]
        public string CoordinatedRegion { get; set; }

        // Other properties - for future purposes
        public virtual ICollection<ProtectiveEquipment> OfferedEquipment { get; set; }

        public virtual ICollection<OfferedHelp> OfferedHelp { get; set; }

        [NotMapped]
        public bool IsProfileFinished => ProfileFinishedDate != null;

        [NotMapped]
        public string FullUserName => 
            UserType == UserTypeEnum.Company ? 
            $"{CompanyName}" + (ContactPersonName != null ? " - " + ContactPersonName : "") :
            $"{IndividualName}";
    }
}