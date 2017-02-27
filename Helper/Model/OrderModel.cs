using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Helper.Model
{
    public class OrderModel
    {
        /// <summary>
        /// order bumber
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// the order create time
        /// </summary>
        public string DateOrder { get; set; }
        /// <summary>
        /// 买家支付的运费
        /// </summary>
        [DefaultValue("0")]
        public string ShippingFees { get; set; }
        /// <summary>
        /// 买家最后支付货款的金额
        /// </summary>
        [DefaultValue("0")]
        public string SettlementAmount { get; set; }
        /// <summary>
        /// 收件人英文姓名，通过软件自动翻译
        /// </summary>
        public string ERecipientName { get; set; }
        /// <summary>
        /// 收件人中文姓名
        /// </summary>
        public string CRecipientName { get; set; }
        public string Country { get; set; }
        /// <summary>
        /// 省名英文翻译，根据中英文对照表格China province_city_postcode搜索得到
        /// </summary>
        public string EProvinceAutonomousRegion { get; set; }
        /// <summary>
        /// 省名中文翻译，根据中英文对照表格China province_city_postcode搜索得到
        /// </summary>
        public string CProvinceAutonomousRegion { get; set; }
        public string ECity { get; set; }
        public string CCity { get; set; }
        public string PostCode { get; set; }
        public string ECountyDistrict { get; set; }
        public string CCountyDistrict { get; set; }
        public string AddressDetails { get; set; }
        /// <summary>
        /// 修改后的收货地址
        /// </summary>
        public string ModifiedAddressDetails { get; set; }
        public string ConsigneePhoneNumber { get; set; }
        public string CDeliveryAddress { get; set; }

    }
}
