using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helper.Model;

namespace Helper
{
    public class LBFDestHelper
    {

        public CitiesModel GetAddressByPostCode(OrderModel order, List<CitiesModel> cityModels)
        {
            var cityModel = new CitiesModel();
            var address = order.AddressDetails;
            if (!string.IsNullOrEmpty(order.PostCode) && order.PostCode.Length == 6 && order.PostCode != "000000")
            {
                string pchead = order.PostCode.Substring(0, ConfigHelper.GetPostCodeHeadCount());
                var citiesEntities =
                    cityModels.Where(c => c.PostCode.IndexOf(pchead, StringComparison.Ordinal) == 0).ToList();
                if (citiesEntities.Count() > 0)
                {
                    cityModel = citiesEntities.FirstOrDefault(c => address.Contains(c.Cityc.Trim()) &&
                                                                    address.Contains(c.Pc.Trim()));
                }
            }

            if (cityModel == null || string.IsNullOrEmpty(cityModel.Pc))
            {
                cityModel = cityModels.FirstOrDefault(c => address.Contains(c.Cityc.Trim()) &&
                                                                address.Contains(c.Pc.Trim()));
                order.PostCode = cityModel != null ? string.Format("{0}0000", cityModel.PostCode.Substring(0, 2)) : order.PostCode;
            }
            if (cityModel == null)
            {
                cityModel = new CitiesModel();
            }
            return cityModel;
        }

        public CitiesModel GetCityByPostCode(OrderModel order, List<CitiesModel> cityModels)
        {
            #region 检验邮编
            var cityModel = new CitiesModel();
            if (!string.IsNullOrEmpty(order.PostCode) && order.PostCode.Length == 6 && order.PostCode != "000000")
            {
                string pchead = order.PostCode.Substring(0, ConfigHelper.GetPostCodeHeadCount()); //如果第三位是0则获取前四位，否则获取前三位
                var citiesEntities =
                    cityModels.Where(c => c.PostCode.IndexOf(pchead, StringComparison.Ordinal) == 0).ToList();
                if (citiesEntities.Count() > 0)
                {
                    cityModel = citiesEntities.FirstOrDefault(c => (order.CCity.Contains(c.Cityc.Trim()) || order.CCountyDistrict.Contains(c.Cityc.Trim())) &&
                                                                  order.CProvinceAutonomousRegion.Contains(c.Pc.Trim())) ??
                                cityModels.FirstOrDefault(c => (order.CCity.Contains(c.Cityc.Trim()) ||
                                                                           order.CCountyDistrict.Contains(c.Cityc.Trim())) &&
                                                                          order.CProvinceAutonomousRegion.Contains(c.Pc.Trim()));
                    if (cityModel == null)
                    {
                        cityModel = citiesEntities.FirstOrDefault();
                    }
                }
            }
            else
            {
                cityModel =
                        cityModels.FirstOrDefault(
                            c => (order.CCity.Contains(c.Cityc.Trim()) || order.CCountyDistrict.Contains(c.Cityc.Trim())) && order.CProvinceAutonomousRegion.Contains(c.Pc.Trim()));
                order.PostCode = cityModel != null ? string.Format("{0}0000", cityModel.PostCode.Substring(0, 2)) : order.PostCode;
            }
            if (cityModel == null)
            {
                cityModel = new CitiesModel();
            }
            #endregion

            return cityModel;
        }

        public string TransNameToPin(string name)
        {
            var result = new StringBuilder();
            foreach (var c in name)
            {
                var en = TranslateHelper.JuHeZiDian(c.ToString());
                result.Append(en.ToUpper());
                result.Append(" ");
            }
            return result.Length > 0 ? result.Remove(result.Length - 1, 1).ToString() : "";
        }

        public string GetPostCode(List<CitiesModel> cityModels, OrderModel order, ref StringBuilder resultMsg)
        {
            var postcode = "";
            var cityCur = cityModels.FirstOrDefault(c =>
            {
                return order.AddressDetails != null && order.AddressDetails.IndexOf(c.Pc) == 0 &&
                       !string.IsNullOrEmpty(c.PostCode);
            });
            postcode = cityCur != null ? string.Format("{0}0000", string.IsNullOrEmpty(cityCur.PostCode) ? "" : cityCur.PostCode.Substring(0, 2)) : "";
            if (!string.IsNullOrEmpty(order.PostCode))
            {
                resultMsg.AppendLine("订单：" + order.OrderId + "使用修改后的收获地址");
            }
            if (string.IsNullOrEmpty(postcode))
            {
                resultMsg.AppendLine("订单：" + order.OrderId + "未找到邮编地址，请手动输入");
            }
            return postcode;
        }
    }
}
