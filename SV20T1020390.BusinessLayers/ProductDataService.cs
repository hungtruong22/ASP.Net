using Azure;
using SV20T1020390.DataLayers;
using SV20T1020390.DataLayers.SQLServer;
using SV20T1020390.DomainModels;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020390.BusinessLayers
{
    public class ProductDataService
    {
        private static readonly IProductDAL productDB;
        
        /// <summary>
        /// Ctor
        /// </summary>
        static ProductDataService()
        {
            productDB = new ProductDAL(Configuration.ConnectionString);
        }

        /// <summary>
        /// Tìm kiếm và lấy danh sách mặt hàng (không phân trang)
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Product> ListProducts(string searchValue = "")
        {
            return productDB.List(1, 0, searchValue, 0, 0, 0, 0).ToList();
        }

        /// <summary>
        /// Tìm kiếm và lấy danh sách mặt hàng dưới dạng phân trang
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <param name="categoryID"></param>
        /// <param name="supplierID"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <exception cref="NotImplementedException"></exception>
        public static List<Product> ListProducts(out int rowCount, int page = 1, int pageSize = 0,
            string searchValue = "", int categoryID = 0, int supplierID = 0,
            decimal minPrice = 0, decimal maxPrice = 0)
        {
            rowCount = productDB.Count(searchValue, categoryID, supplierID, minPrice, maxPrice);
            return productDB.List(page, pageSize, searchValue, categoryID, supplierID, minPrice, maxPrice).ToList();
            
        }

        /// <summary>
        /// Lấy thông tin một mặt hàng theo mã mặt hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static Product? GetProduct(int productID)
        {
            return productDB.Get(productID);
        }

        /// <summary>
        /// Thêm mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static int AddProduct(Product data) 
        {
            return productDB.Add(data);
        }

        /// <summary>
        /// Cập nhật thông tin mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static bool UpdateProduct(Product data)
        {
            return productDB.Update(data);
        }

        /// <summary>
        /// Xóa mặt hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static bool DeleteProduct(int productID)
        {
            if (productDB.IsUsed(productID))
            {
                return false;
            }
            else
            {
                return productDB.Delete(productID);
            }
        }


        /// <summary>
        /// Kiểm tra xem mặt hàng có đơn hàng liên quan hay không
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static bool IsUsedProduct(int productID)
        {
            return productDB.IsUsed(productID);
        }


        /// <summary>
        /// Hiển thị danh sách ảnh của mặt hàng
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static List<ProductPhoto> ListPhotos(int productID)
        {
            return productDB.ListPhotos(productID).ToList();
        }


        /// <summary>
        /// Lấy 1 ảnh của mặt hàng
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static ProductPhoto? GetPhoto(long photoID)
        {
            return productDB.GetPhoto(photoID);
        }

        /// <summary>
        /// Bổ sung ảnh cho mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static long AddPhoto(ProductPhoto data) 
        {
            return productDB.AddPhoto(data);
        }

        /// <summary>
        /// Cập nhật ảnh của mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static bool UpdatePhoto(ProductPhoto data)
        {
            return productDB.UpdatePhoto(data);
        }


        /// <summary>
        /// Xóa ảnh của mặt hàng
        /// </summary>
        /// <param name="photoID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static bool DeletePhoto(long photoID)
        {
            return productDB.DeletePhoto(photoID);
        }


        /// <summary>
        /// Hiển thị danh sách thuộc tính của mặt hàng
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static List<ProductAttribute> ListAttributes(int productID)
        {
            return productDB.ListAttributes(productID).ToList();
        }


        /// <summary>
        /// Lấy 1 thuộc tính của mặt hàng
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static ProductAttribute? GetAttribute(long attributeID)
        {
            return productDB.GetAttribute(attributeID);
        }

        /// <summary>
        /// Bổ sung thuộc tính cho mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static long AddAttribute(ProductAttribute data)
        {
            return productDB.AddAttribute(data);
        }

        /// <summary>
        /// Cập nhật thuộc tính của mặt hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static bool UpdateAttribute(ProductAttribute data)
        {
            return productDB.UpdateAttribute(data);
        }


        /// <summary>
        /// Xóa thuộc tính của mặt hàng
        /// </summary>
        /// <param name="attributeID"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static bool DeleteAttribute(long attributeID)
        {
            return productDB.DeleteAttribute(attributeID);
        }
    }
}
