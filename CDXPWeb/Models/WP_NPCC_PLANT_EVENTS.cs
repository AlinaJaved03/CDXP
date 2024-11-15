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
    
    public partial class WP_NPCC_PLANT_EVENTS
    {
        public decimal WP_NPCC_PLANT_EVENTS_ID_PK { get; set; }
        public string STATUS { get; set; }
        public Nullable<decimal> SETUP_SITE_ID_FK { get; set; }
        public Nullable<decimal> VENDOR_ID { get; set; }
        public string GENERATION_COMPANY { get; set; }
        public string SITE { get; set; }
        public string BLOCK { get; set; }
        public Nullable<decimal> PLT_BLK_FUEL_ID { get; set; }
        public string FUEL { get; set; }
        public string EVENT { get; set; }
        public string TYPE_OF_OUTAGE { get; set; }
        public string REASON { get; set; }
        public Nullable<System.DateTime> EVENT_TIME { get; set; }
        public Nullable<System.DateTime> INTIMATION_TIME { get; set; }
        public Nullable<bool> INTIMATION_SOURCE_TELEPHONE { get; set; }
        public Nullable<bool> INTIMATION_SOURCE_FAX { get; set; }
        public Nullable<bool> INTIMATION_SOURCE_PORTAL { get; set; }
        public string SENDER_NAME { get; set; }
        public string SENDER_DESIGNATION { get; set; }
        public string AVAILABILITY { get; set; }
        public string REMARKS { get; set; }
        public Nullable<bool> NPCC_ACK { get; set; }
        public Nullable<System.DateTime> NPCC_ACK_DATE_TIME { get; set; }
        public string NPCC_RECEIVER_NAME { get; set; }
        public string NPCC_RECEIVER_DESIGNATION { get; set; }
        public string NPCC_REMARKS { get; set; }
        public string NPCC_REMARKS_ONLY_NPCC { get; set; }
        public string NPCC_TYPE_OF_OUTAGE { get; set; }
        public string NPCC_REASON { get; set; }
        public Nullable<bool> EVENT_VERIFIED { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_ON { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_ON { get; set; }
        public string STATUS_OF_MACHINE { get; set; }
        public Nullable<decimal> FLAG { get; set; }
    }
}
