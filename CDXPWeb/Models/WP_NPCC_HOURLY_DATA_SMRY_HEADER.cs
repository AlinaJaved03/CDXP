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
    
    public partial class WP_NPCC_HOURLY_DATA_SMRY_HEADER
    {
        public decimal HOURLY_DATA_SUMMARY_HEADER_ID_PK { get; set; }
        public Nullable<decimal> SETUP_SITE_ID_FK { get; set; }
        public string GENERATION_COMPANY { get; set; }
        public string COMPLEX_WISE { get; set; }
        public string SITE_NAME { get; set; }
        public string BLOCK { get; set; }
        public string FUEL { get; set; }
        public string CREATED_BY { get; set; }
        public Nullable<System.DateTime> CREATED_ON { get; set; }
        public string MODIFIED_BY { get; set; }
        public Nullable<System.DateTime> MODIFIED_ON { get; set; }
        public Nullable<decimal> FLAG { get; set; }
    }
}