using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deco.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        [DisplayName("Product Name")]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public double Price {  get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Set Price")]
        public double SetPrice { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }

        //One to many relation this way
        [ValidateNever]
        public List<ProductImage> ProductImages { get; set; }
    }
}
