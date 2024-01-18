using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deco.Models
{
    public class AdsImage
    {
        public int Id { get; set; }
        [ValidateNever]
        public string ImageUrl { get; set; }
        [DisplayName("Ads link")]
        public string Url { get; set; }
        public string? Title { get; set; }
        public bool IsActive { get; set; }
        public int AdsTypeId { get; set; }
        [ForeignKey("AdsTypeId")]
        [ValidateNever]
        public AdsType AdsType { get; set; }
    }
}
