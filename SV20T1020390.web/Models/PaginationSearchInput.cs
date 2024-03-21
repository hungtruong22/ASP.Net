namespace SV20T1020390.web.Models
{
    /// <summary>
    /// Tìm kiếm dữ liệu để nhận dữ liệu dưới dạng phân trang
    /// </summary>
    public class PaginationSearchInput
    {
        public int Page {  get; set; }
        public int PageSize { get; set; }
        public string SearchValue { get; set; } = "";
    }

    public class ProductSearchInput : PaginationSearchInput
    { 
        public int CategoryID { get; set; }   
        public int SupplierID { get; set; }
        public int CustomerID { get; set; }
        public string DeliveryProvince { get; set; }
    }
}
