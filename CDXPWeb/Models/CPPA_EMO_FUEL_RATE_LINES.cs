//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CDXPWeb.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CPPA_EMO_FUEL_RATE_LINES
    {
        public decimal FUEL_RATE_LINE_ID { get; set; }
        public decimal FORECAST_HEADER_ID { get; set; }
        public string FUEL_SUPPLIER { get; set; }
        public Nullable<decimal> PRODUCT_COST { get; set; }
        public Nullable<decimal> OTHER_CHARGES { get; set; }
        public Nullable<decimal> RATE { get; set; }
        public string REMARKS { get; set; }
        public Nullable<System.DateTime> CREATION_DATE { get; set; }
        public Nullable<decimal> CREATED_BY { get; set; }
        public Nullable<System.DateTime> LAST_UPDATE_DATE { get; set; }
        public Nullable<decimal> LAST_UPDATED_BY { get; set; }
        public Nullable<decimal> LAST_UPDATE_LOGIN { get; set; }
        public string POWER_POLICY { get; set; }
        public Nullable<decimal> QUANTITY { get; set; }
        public string FINANCE_VERIFIED { get; set; }
    }
}
