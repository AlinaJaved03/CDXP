using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CDXPWeb.Models
{
    public class ScheduleTen
    {
        private string Sch_10_id; // field
        private string ambient_temperature; // field
        private string netplant_output; // field
        private string correction_factor; // field
        private string remaks; // field
        private string vendorID; // field
        private string fuelLookup; //field
        private string sch_10_type; //field

        public string Ambient_Temperature   // property
        {
            get { return ambient_temperature; }   // get method
            set { ambient_temperature = value; }  // set method
        }
        public string NetPlant_Output   // property
        {
            get { return netplant_output; }   // get method
            set { netplant_output = value; }  // set method
        }
        public string Correction_Factor   // property
        {
            get { return correction_factor; }   // get method
            set { correction_factor = value; }  // set method
        }
        public string Remaks   // property
        {
            get { return remaks; }   // get method
            set { remaks = value; }  // set method
        }
        public string Sch_10_id_value  // property
        {
            get { return Sch_10_id; }   // get method
            set { Sch_10_id = value; }  // set method
        }

        public string VendorIDvalue  // property
        {
            get { return vendorID; }   // get method
            set { vendorID = value; }  // set method
        }

        public string Fuel_Lookup_Code  // property
        {
            get { return fuelLookup; }   // get method
            set { fuelLookup = value; }  // set method
        }

        public string SCH_10_TYPE
        {
            get { return sch_10_type; }   // get method
            set { sch_10_type = value; }  // set method
        }


    }
}