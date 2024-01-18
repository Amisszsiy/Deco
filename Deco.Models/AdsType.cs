using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deco.Models
{
    public class AdsType
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int? SizeWidth { get; set; }
        public int? SizeHeight { get; set;}
    }
}
