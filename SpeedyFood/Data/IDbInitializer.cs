using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedyFood.Data
{
    public interface IDbInitializer
    {
        Task Initialize();
        void SeedData();

    }
}
