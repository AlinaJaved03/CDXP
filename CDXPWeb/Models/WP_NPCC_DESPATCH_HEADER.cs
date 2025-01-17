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
    
    public partial class WP_NPCC_DESPATCH_HEADER
    {
        public decimal WP_NPCC_DESPATCH_HEADER_ID { get; set; }
        public Nullable<decimal> SETUP_SITE_ID_FK { get; set; }
        public string GENERATION_COMPANY { get; set; }
        public Nullable<decimal> VENDOR_SITE_ID { get; set; }
        public Nullable<decimal> VENDOR_ID { get; set; }
        public string SITE { get; set; }
        public Nullable<decimal> BLOCK_ID { get; set; }
        public string BLOCK { get; set; }
        public Nullable<decimal> FUEL_ID { get; set; }
        public string FUEL { get; set; }
        public Nullable<System.DateTime> INTIMATION_TIME { get; set; }
        public Nullable<bool> INTIMATION_SOURCE_TELEPHONE { get; set; }
        public Nullable<System.DateTime> INTIMATION_SOURCE_TELEPHONE_DATE_TIME { get; set; }
        public Nullable<bool> INTIMATION_SOURCE_FAX { get; set; }
        public Nullable<System.DateTime> INTIMATION_SOURCE_FAX_DATE_TIME { get; set; }
        public Nullable<bool> INTIMATION_SOURCE_PORTAL { get; set; }
        public string SENDER_NAME { get; set; }
        public string SENDER_DESIGNATION { get; set; }
        public string GC_ACK_BY { get; set; }
        public Nullable<System.DateTime> GC_ACK_TIME { get; set; }
        public string GC_ACK_DESIGNATION { get; set; }
        public string GC_ACK_REMARKS { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_ON { get; set; }
        public string MODIFIED_BY { get; set; }
        public string GC_COMP_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_ON { get; set; }
        public string GC_COMP_DESIGNATION { get; set; }
        public string GC_COMP_TYPE { get; set; }
        public Nullable<System.DateTime> GC_COMP_ACHIEVE_DATE_TIME { get; set; }
        public string GC_COMP_REMARKS { get; set; }
        public string GC_COMP_TARGET_ACHIEVED { get; set; }
        public string STATUS { get; set; }
        public string NPCC_REMARKS { get; set; }
        public string DEMAND_TYPE { get; set; }
        public Nullable<System.DateTime> SYNC_DESYNC_DATE_TIME { get; set; }
        public string EMERGENCY_TYPE { get; set; }
        public Nullable<System.DateTime> TARGET_DATE_TIME { get; set; }
        public Nullable<decimal> TARGET_DEMAND { get; set; }
        public Nullable<System.DateTime> TARGET_DATE_TIME_2 { get; set; }
        public Nullable<decimal> TARGET_DEMAND_2 { get; set; }
        public Nullable<decimal> DEMAND_NO { get; set; }
        public Nullable<decimal> FLAG { get; set; }
    }
}
