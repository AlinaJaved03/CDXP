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
    
    public partial class WP_GC_HOURLY_DATA_DETAIL
    {
        public decimal WP_GC_HOURLY_DATA_DETAIL_ID_PK { get; set; }
        public Nullable<decimal> WP_GC_HOURLY_DATA_HEADER_ID_FK { get; set; }
        public Nullable<decimal> BLOCK_OR_FUEL_ID_FK { get; set; }
        public string TARGET_HOUR { get; set; }
        public Nullable<decimal> AMBIENT_TEMPERATURE { get; set; }
        public Nullable<decimal> AMBIENT_AVAILABILITY { get; set; }
        public Nullable<decimal> AVAILABILITY { get; set; }
        public string REMARKS { get; set; }
        public Nullable<decimal> AVAILABILITY_AS_PER_SCH_10 { get; set; }
        public Nullable<decimal> DACLARED_AVAILABILITY { get; set; }
        public string ATTRIBUTE1 { get; set; }
        public string ATTRIBUTE2 { get; set; }
        public string ATTRIBUTE3 { get; set; }
        public string ATTRIBUTE4 { get; set; }
        public string ATTRIBUTE_5 { get; set; }
    }
}
