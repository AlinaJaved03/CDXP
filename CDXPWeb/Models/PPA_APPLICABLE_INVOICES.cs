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
    
    public partial class PPA_APPLICABLE_INVOICES
    {
        public decimal APP_INVOICES_ID_PK { get; set; }
        public Nullable<double> HEADER_ID_FK { get; set; }
        public string INVOICE_TYPES { get; set; }
        public string IS_HOURLY { get; set; }
        public Nullable<double> ORGANIZATION_ID { get; set; }
        public Nullable<System.DateTime> CREATION_DATE { get; set; }
        public Nullable<double> CREATED_BY { get; set; }
        public Nullable<System.DateTime> LAST_UPDATE_DATE { get; set; }
        public Nullable<double> LAST_UPDATED_BY { get; set; }
        public Nullable<double> LAST_UPDATE_LOGIN { get; set; }
        public string ADVANCE_PAYMENT { get; set; }
        public string ADJUSTMENTS { get; set; }
    }
}
