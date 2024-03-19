using SV20T1020390.DomainModels;

namespace SV20T1020390.web.Models
{
    public class OrderDetailModel
    {
        public Order Order { get; set; }
        public List<OrderDetail> Details { get; set; }
    }
}
