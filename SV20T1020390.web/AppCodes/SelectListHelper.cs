using Microsoft.AspNetCore.Mvc.Rendering;
using SV20T1020390.BusinessLayers;
using SV20T1020390.DomainModels;

namespace SV20T1020390.web
{
    public class SelectListHelper
    {
        public static List<SelectListItem> Provinces()
        {
            List<SelectListItem > list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "",
                Text = "-- Chọn Tỉnh/Thành --"
            });
            foreach (var item in CommonDataService.ListOfProvinces()) 
            {
                list.Add(new SelectListItem()
                {
                    Value = item.ProvinceName,
                    Text = item.ProvinceName
                });
            }

            return list;
        }

        public static List<SelectListItem> Categories()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = "0",
                Text = "-- Tất cả loại hàng --"
            });
            foreach(var item in CommonDataService.ListOfCateNames())
            {
                list.Add(new SelectListItem()
                {
                    Value = item.CategoryID.ToString(),
                    Text = item.CategoryName
                });
            }
            return list;
        }

        public static List <SelectListItem> Suppliers() 
        {
            List<SelectListItem> list = new List<SelectListItem> ();
            list.Add(new SelectListItem()
            {
                Value = "0",
                Text = "-- Tất cả nhà cung cấp --",
            });
            foreach(var item in CommonDataService.ListOfSupNames())
            {
                list.Add(new SelectListItem()
                {
                    Value= item.SupplierID.ToString(),
                    Text = item.SupplierName
                });
            }
            return list;
        }
    }
}
