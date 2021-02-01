using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Areas.Member.Models
{
    public class AddContactInfo
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public EnumContactSalutation Salutation { get; set; }

        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public string BusinessPhone { get; set; }
        public string Fax { get; set; }

        public string Email { get; set; }
        public string Website { get; set; }
        public string BackgroundInfo { get; set; }

        public string ReferralSource { get; set; }
        public int Rating { get; set; }
        public string Company { get; set; }
        public string Industry { get; set; }
        public decimal AnnualRevenue { get; set; }
        public DateTime DateOfInitialContact { get; set; }

        public string Address { get; set; }
        public double GeoX { get; set; }
        public double GeoY { get; set; }
    }

    public class AddAttachmentInfo
    {
        public string ContactId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class EditAttachmentInfo
    {
        public string ContactAttachmentId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Token { get; set; }
    }

    public class EditContactInfo
    {
        [Required]
        public string ContactId { get; set; }
        [Required]
        public string Token { get; set; }

        public EnumContactSalutation Salutation { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public string BusinessPhone { get; set; }
        public string Fax { get; set; }

        public string Email { get; set; }
        public string Website { get; set; }

        public string Address { get; set; }
    }

    public class EditWorkInfo
    {
        [Required]
        public string ContactId { get; set; }
        [Required]
        public string Token { get; set; }

        public string Title { get; set; }
        public string Company { get; set; }
        public string Industry { get; set; }
        public decimal AnnualRevenue { get; set; }
    }
    public class EditSystemInfo
    {
        [Required]
        public string ContactId { get; set; }

        [Required]
        public string Token { get; set; }
        public EnumContactStatus Status { get; set; }
        public string ReferralSource { get; set; }

        [Required]
        public DateTime DateOfInitialContact { get; set; }

        [Required]
        public int Rating { get; set; }

        //[Required]
        //public string AssignedToId { get; set; }

    }

    public class AddFileAttachmentInfo
    {
        public string ContactId { get; set; }
        public string Token { get; set; }
        public string ImageName { get; set; }
        public string ImageLink { get; set; }
    }
}
