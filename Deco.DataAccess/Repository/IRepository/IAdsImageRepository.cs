﻿using Deco.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deco.DataAccess.Repository.IRepository
{
    public interface IAdsImageRepository : IRepository<AdsImage>
    {
        void Update(AdsImage obj);
    }
}
