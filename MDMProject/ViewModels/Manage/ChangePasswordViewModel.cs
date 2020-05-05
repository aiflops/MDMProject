﻿using MDMProject.Models;
using MDMProject.Resources;
using System.ComponentModel.DataAnnotations;

namespace MDMProject.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.ChangePasswordViewModel_OldPassword))]
        [DataType(DataType.Password)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.FieldIsRequired))]
        public string OldPassword { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.ChangePasswordViewModel_NewPassword))]
        [DataType(DataType.Password)]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.FieldIsRequired))]
        [MinLength(ValidationConstants.User.MIN_PASSWORD_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MinFieldLength))]
        [MaxLength(ValidationConstants.User.MAX_PASSWORD_LENGTH, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxFieldLength))]
        public string NewPassword { get; set; }

        [Display(ResourceType = typeof(PropertyNames), Name = nameof(PropertyNames.ChangePasswordViewModel_ConfirmNewPassword))]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword), ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.PasswordDoesntMatch))]
        public string ConfirmPassword { get; set; }
    }
}