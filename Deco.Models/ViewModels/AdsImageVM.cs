using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deco.Models.ViewModels
{
    public class AdsImageVM
    {
        public AdsImage AdsImage { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> AdsTypeList { get; set; }
    }
}
