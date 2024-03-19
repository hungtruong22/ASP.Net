using SV20T1020390.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020390.DataLayers
{
    public interface IOrderDAL
    {
        /// <summary>
        /// Tìm kiếm và lấy danh sách đơn hàng dưới dạng trang
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="status"></param>
        /// <param name="fromTime"></param>
        /// <param name="toTime"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        IList<Order> List(int page = 1, int pageSize = 0,
            int status = 0, DateTime? fromTime = null, DateTime? toTime = null,
            string searchValue = "");

        /// <summary>
        /// Đếm số lượng đơn hàng thỏa điều kiện tìm kiếm
        /// </summary>
        /// <param name="status"></param>
        /// <param name="fromTime"></param>
        /// <param name="toTime"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        int Count(int status = 0, DateTime? fromTime = null, DateTime? toTime = null,
            string searchValue = "");

        /// <summary>
        /// Lấy thông tin đơn hàng theo mã đơn hàng
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        Order? Get(int orderID);

        /// <summary>
        /// Bổ sung đơn hàng mới
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        int Add(Order data);

        /// <summary>
        /// Cập nhật thông tin của đơn hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// 
        bool Update(Order data);

        /// <summary>
        /// Xóa đơn hàng và chi tiết của đơn hàng
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        bool Delete(int orderID);

        /// <summary>
        /// ấy dánh sách hàng được bán trong đơn hàng (chi tiết đơn hàng)
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        IList<OrderDetail> ListDetails(int orderID);

        /// <summary>
        /// Lấy thông tin của 1 mặt hàng được bán trong đơn hàng (1 chi tiết trong đơn hàng)
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="productID"></param>
        /// <returns></returns>
        OrderDetail? GetDetail(int orderID, int productID);

        /// <summary>
        /// Thêm mặt hàng được bán trong đơn hàng) theo nguyên tắc:
        /// - Nếu mặt hàng chưa có trong chi tiết đơn hàng thì bổ sung
        /// - Nếu mặt hàng đã có trong chi tiết đơn hàng thì cập nhật lại số lượng và giá bán
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="productID"></param>
        /// <param name="quantity"></param>
        /// <param name="salePrice"></param>
        /// <returns></returns>
        bool SaveDetail(int orderID, int productID, int quantity, decimal salePrice);

        /// <summary>
        /// Xóa 1 mặt hàng ra khỏi đơn hàng
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="productID"></param>
        /// <returns></returns>
        bool DeleteDetail(int orderID, int productID);
    }
}
