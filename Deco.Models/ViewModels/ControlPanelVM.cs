﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deco.Models.ViewModels
{
    public class ControlPanelVM
    {
        public IEnumerable<AdsType> AdsType { get; set; }
        public IEnumerable<AdsImage> AdsImages { get; set; }
    }
}
