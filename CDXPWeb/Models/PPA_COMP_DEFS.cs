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
    
    public partial class PPA_COMP_DEFS
    {
        public decimal COMP_DEF_ID_PK { get; set; }
        public Nullable<double> HEADER_ID_FK { get; set; }
        public Nullable<double> APP_INVOICES_ID_FK { get; set; }
        public Nullable<double> BLK_FUEL_ID_FK { get; set; }
        public string COMP_NAME { get; set; }
        public string COMP_TYPE { get; set; }
        public string COMP_ZONE { get; set; }
        public string COMP_VALUE { get; set; }
        public string SHOW_ON_INV { get; set; }
        public string INC_IN_TOT { get; set; }
        public string FORMULA { get; set; }
        public string UNIT { get; set; }
        public string COMPARE_WITH { get; set; }
        public string EXEMPT_DATE_FROM { get; set; }
        public string EXEMPT_DATE_TO { get; set; }
        public string REMARKS { get; set; }
        public Nullable<double> ORGANIZATION_ID { get; set; }
        public Nullable<System.DateTime> CREATION_DATE { get; set; }
        public Nullable<double> CREATED_BY { get; set; }
        public Nullable<System.DateTime> LAST_UPDATE_DATE { get; set; }
        public Nullable<double> LAST_UPDATED_BY { get; set; }
        public Nullable<double> LAST_UPDATE_LOGIN { get; set; }
        public string IS_DISABLE_COMP_VALUE { get; set; }
        public string IS_DISABLE_COMP_ZONE { get; set; }
        public string SHOW_ON_DIARY { get; set; }
        public string SHOW_TOT_ON_HRSINV { get; set; }
        public string PART_OF { get; set; }
        public string INCLUD_IN_CLAIM_PORTAL { get; set; }
        public string INC_IN_DIARY_TOT { get; set; }
        public Nullable<double> COMP_DEF_ORDER { get; set; }
    }
}
