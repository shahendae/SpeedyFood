using SpeedyFood.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedyFood.Repository
{
    public interface IOrderHeaderRepository : IGenericRepository<OrderHeader>
    {
        Task<OrderHeader> GetOrderHeaderWithApplicationUser(int HeaderId, string UserId);
        Task<List<OrderHeader>> GetOrderHeadersWithApplicationUser(string id);
        Task<List<OrderHeader>> GetOrderHeadersWithReadyStatus();
        Task<List<OrderHeader>> SearchOrderHeadersByPickupNames(string searchName);
        Task<List<OrderHeader>> SearchOrderHeadersByPhone(string searchPhone);
    }
}
