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
    
    public partial class WP_PORTAL_USERS
    {
        public int WP_PORTAL_USERS_ID { get; set; }
        public string USER_NAME { get; set; }
        public string PASSWORD { get; set; }
        public string DISPLAY_NAME { get; set; }
        public Nullable<double> PPA_HEADER_ID_FK { get; set; }
        public Nullable<int> WP_SETUP_USER_TYPES_ID { get; set; }
        public string EMAIL_ADDRESSES { get; set; }
        public Nullable<bool> EMAIL_SUBSCRIPTION { get; set; }
        public string DISPLAY_DESIGNATION { get; set; }
        public string USER_NAME_OLD { get; set; }
    }
}
