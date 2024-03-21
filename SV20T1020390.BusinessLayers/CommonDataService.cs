using SV20T1020390.DataLayers;
using SV20T1020390.DataLayers.SQLServer;
using SV20T1020390.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV20T1020390.BusinessLayers
{
    /// <summary>
    /// Cung cấp các chức năng nghiệp vụ liên quan đến các dữ liệu "chung chung"
    /// (tỉnh/ thành, khách hàng, nhà cung cấp, loại hàng, người giao hàng, nhân viên)
    /// </summary>
    public static class CommonDataService
    {
        private static readonly ICommonDAL<Province> provinceDB;
        private static readonly ICommonDAL<Customer> customerDB;
        private static readonly ICommonDAL<Supplier> supplierDB;
        private static readonly ICommonDAL<Shipper> shipperDB;
        private static readonly ICommonDAL<Employee> employeeDB;
        private static readonly ICommonDAL<Category> categoryDB;


        /// <summary>
        /// Ctor : hàm khởi tạo
        /// Câu hỏi: static constructor hoạt động như thế nào? cách viết?
        /// </summary>
        static CommonDataService()
        {
            string connectString = Configuration.ConnectionString;

            provinceDB = new ProvinceDAL(connectString);
            customerDB = new CustomerDAL(connectString);
            supplierDB = new SupplierDAL(connectString);
            shipperDB = new ShipperDAL(connectString);
            employeeDB = new EmployeeDAL(connectString);
            categoryDB = new CategoryDAL(connectString);
        }

        

        /// <summary>
        /// Tìm kiếm và lấy danh sách nhà cung cấp
        /// </summary>
        /// <param name="rowCount">Tham số đầu ra cho biết số dòng dữ liệu tìm được</param>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng trên mỗi trang(0 nếu không phân trang)</param>
        /// <param name="searchValue">Giá trị tìm kiếm (rỗng nếu lấy toàn bộ khách hàng)</param>
        /// <returns></returns>
        public static List<Supplier> ListOfSuppliers(out int rowCount, int page = 1,
            int pageSize = 0, string searchValue = "")
        {
            rowCount = supplierDB.Count(searchValue);
            return supplierDB.List(page, pageSize, searchValue).ToList();
        }

         

        /// <summary>
        /// Lấy thông tin 1 nhà cung cấp thông qua mã nhà cung cấp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Supplier? GetSupplier(int id)
        {
            return supplierDB.Get(id);
        } 

        /// <summary>
        /// Bổ sung nhà cung cấp mới
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddSupplier(Supplier data)
        {
            return supplierDB.Add(data);
        }

        /// <summary>
        /// Cập nhật nhà cung cấp
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateSupplier(Supplier data)
        {
            return supplierDB.Update(data);
        }

        /// <summary>
        /// Xóa nhà cung cấp có mã là id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteSupplier(int id)
        {
            if(supplierDB.IsUsed(id))
            {
                return false;
            }
            else
            {
                return supplierDB.Delete(id);
            }
        }

        /// <summary>
        /// Kiểm tra xem có mã id có dữ liệu liên quan hay không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedSupplier(int id)
        {
            return supplierDB.IsUsed(id);
        }

        public static List<Province> ListOfProvinces()
        {
            return provinceDB.List().ToList();
        }

        /// <summary>
        /// Tìm kiếm và lấy danh sách khách hàng
        /// </summary>
        /// <param name="rowCount">Tham số đầu ra cho biết số dòng dữ liệu tìm được</param>
        /// <param name="page">Trang cần hiển thị</param>
        /// <param name="pageSize">Số dòng trên mỗi trang(0 nếu không phân trang)</param>
        /// <param name="searchValue">Giá trị tìm kiếm (rỗng nếu lấy toàn bộ khách hàng)</param>
        /// <returns></returns>
        public static List<Customer> ListOfCustommers(out int rowCount, int page = 1,
            int pageSize = 0, string searchValue = "")
        {
            rowCount = customerDB.Count(searchValue);
            return customerDB.List(page, pageSize, searchValue).ToList();
        }

        /// <summary>
        /// Lấy thông tin của 1 khách hàng thông qua mã khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Customer? GetCustomer(int id)
        {
            return customerDB.Get(id);
        }


        /// <summary>
        /// Bổ sung khách hàng mới.Hàm trả về mã của khách hàng mới được bổ sung
        /// (Hàm trả về -1 nếu email bị trùng, trả về giá trị 0 nếu lỗi)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCustomer(Customer data)
        {
            return customerDB.Add(data);
        }


        /// <summary>
        /// Cập nhật thông tin của một khách hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateCustomer(Customer data) 
        {
            return customerDB.Update(data);
        }

        /// <summary>
        /// Xóa 1 khách hàng (nếu khách hàng đó không có dữ liệu liên quan, (Tồn tại khách hàng trong bảng order))
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCustomer(int id)
        {
            if(customerDB.IsUsed(id))
            {
                return false;
            }
            else
            {
                return customerDB.Delete(id);
            }
        }

        /// <summary>
        /// Kiểm tra xem 1 khách hàng có dữ liệu liên quan hay không?
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedCustomer(int id)
        {
            return customerDB.IsUsed(id);
        }

        /// <summary>
        /// Tìm kiếm và lấy danh sách người giao hàng
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Shipper> ListOfShippers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = shipperDB.Count(searchValue);
            return shipperDB.List(page, pageSize,searchValue).ToList();
        }

        /// <summary>
        /// Lấy thông tin của 1 người giao hàng thông qua mã người giao hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Shipper? GetShipper(int id)
        {
            return shipperDB.Get(id);
        }

        /// <summary>
        /// Bổ sung người giao hàng mới
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddShipper(Shipper data)
        {
            return shipperDB.Add(data);
        }

        /// <summary>
        /// Cập nhật người giao hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateShipper(Shipper data)
        {
            return shipperDB.Update(data);
        }

        /// <summary>
        /// Xóa người giao hàng có mã là id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteShipper(int id)
        {
            if(shipperDB.IsUsed(id))
            {
                return false;
            }
            else
            {
                return shipperDB.Delete(id);
            }
        }

        /// <summary>
        /// Kiểm tra người giao hàng có mã id có dữ liệu liên quan hay không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedShipper(int id)
        {
            return shipperDB.IsUsed(id);
        }

        /// <summary>
        /// Tìm kiếm và lấy danh sách nhân viên
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Employee> ListOfEmployees(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = employeeDB.Count(searchValue);
            return employeeDB.List(page, pageSize, searchValue).ToList();
        }

        /// <summary>
        /// Lấy thông tin của 1 nhân viên thông qua mã nhân viên
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Employee? GetEmployee(int id)
        {
            return employeeDB.Get(id);
        }

        /// <summary>
        /// Bổ sung nhân viên mới
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddEmployee(Employee data)
        {
            return employeeDB.Add(data);
        }

        /// <summary>
        /// Cập nhật nhân viên
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateEmployee(Employee data)
        {
            return employeeDB.Update(data);
        }

        /// <summary>
        /// Xóa nhân viên có mã là id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteEmployee(int id)
        {
            if (employeeDB.IsUsed(id))
            {
                return false;
            }
            else
            {
                return employeeDB.Delete(id);
            }
        }

        /// <summary>
        /// Kiểm tra nhân viên có mã id có dữ liệu liên quan hay không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedEmployee(int id)
        {
            return employeeDB.IsUsed(id);
        }

        /// <summary>
        /// Tìm kiếm và lấy danh sách loại hàng
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Category> ListOfCategories(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = categoryDB.Count(searchValue);
            return categoryDB.List(page, pageSize, searchValue).ToList();
        }

        /*public static List<Category> ListCates()
        {
            return categoryDB.List().ToList();
        }*/

        /// <summary>
        /// Lấy thông tin của 1 loại hàng thông qua mã loại hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Category? GetCategory(int id)
        {
            return categoryDB.Get(id);
        }

        /// <summary>
        /// Bổ sung loại hàng mới
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCategory(Category data)
        {
            return categoryDB.Add(data);
        }

        /// <summary>
        /// Cập nhật loại hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateCategory(Category data)
        {
            return categoryDB.Update(data);
        }

        /// <summary>
        /// Xóa loại hàng có mã là id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCategory(int id)
        {
            if (categoryDB.IsUsed(id))
            {
                return false;
            }
            else
            {
                return categoryDB.Delete(id);
            }
        }

        /// <summary>
        /// Kiểm tra loại hàng có mã id có dữ liệu liên quan hay không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedCategory(int id)
        {
            return categoryDB.IsUsed(id);
        }

        public static List<Category> ListOfCateNames(string searchValue = "")
        {
            return categoryDB.List(1,0,searchValue).ToList();
        }

        public static List<Supplier> ListOfSupNames(string searchValue = "")
        {
            return supplierDB.List(1,0,searchValue).ToList();
        }

        public static List<Customer> ListCustomers(string searchValue = "")
        {
            return customerDB.List(1, 0, searchValue).ToList();
        }

        public static List<Shipper> ListShippers(string searchValue = "")
        {
            return shipperDB.List(1, 0, searchValue).ToList();
        }
    }
}
