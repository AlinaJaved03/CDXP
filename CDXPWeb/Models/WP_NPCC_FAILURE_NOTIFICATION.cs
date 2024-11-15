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
    
    public partial class WP_NPCC_FAILURE_NOTIFICATION
    {
        public decimal WP_NPCC_FAILURE_NOTIFICATION_ID_PK { get; set; }
        public Nullable<decimal> SETUP_SITE_ID_FK { get; set; }
        public string GENERATION_COMPANY { get; set; }
        public string SITE { get; set; }
        public string BLOCK { get; set; }
        public string FUEL { get; set; }
        public Nullable<decimal> WP_NPCC_DESPATCH_HEADER_ID_FK { get; set; }
        public string NOTIFICATION_COUNT { get; set; }
        public Nullable<System.DateTime> INTIMATION_TIME { get; set; }
        public Nullable<bool> INTIMATION_SOURCE_TELEPHONE { get; set; }
        public Nullable<System.DateTime> INTIMATION_SOURCE_TELEPHONE_TIME { get; set; }
        public Nullable<bool> INTIMATION_SOURCE_FAX { get; set; }
        public Nullable<System.DateTime> INTIMATION_SOURCE_FAX_TIME { get; set; }
        public Nullable<bool> INTIMATION_SOURCE_PORTAL { get; set; }
        public string SENDER_NAME { get; set; }
        public Nullable<bool> GC_ACK { get; set; }
        public Nullable<System.DateTime> GC_ACKNOWLEDGEMENT_TIME { get; set; }
        public string GC_AckBy { get; set; }
        public string GC_DESIGNATION { get; set; }
        public string GC_REMARKS { get; set; }
        public Nullable<bool> COMPLAINCEMET { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_ON { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_ON { get; set; }
        public string DEMAND_NO { get; set; }
        public Nullable<decimal> TARGET_DEMAND { get; set; }
        public Nullable<decimal> SUPPLY { get; set; }
        public Nullable<decimal> INSTALLED_CAPACITY { get; set; }
        public string NPCC_REMARKS { get; set; }
        public string STATUS { get; set; }
        public string SENDER_DESIGNATION { get; set; }
        public Nullable<decimal> FLAG { get; set; }
    }
}