﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class WP_CPPAG_PrototypeEntities : DbContext
    {
        public WP_CPPAG_PrototypeEntities()
            : base("name=WP_CPPAG_PrototypeEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ACCOUNTING_MONTH> ACCOUNTING_MONTH { get; set; }
        public virtual DbSet<AP_SUPPLIER_SITE_ALL> AP_SUPPLIER_SITE_ALL { get; set; }
        public virtual DbSet<AP_SUPPLIERS> AP_SUPPLIERS { get; set; }
        public virtual DbSet<CPPA_CDP_DETAIL> CPPA_CDP_DETAIL { get; set; }
        public virtual DbSet<CPPA_CDP_DETAILYYYYYYYYYXXXXX> CPPA_CDP_DETAILYYYYYYYYYXXXXX { get; set; }
        public virtual DbSet<CPPA_CDP_HEADERS> CPPA_CDP_HEADERS { get; set; }
        public virtual DbSet<CPPA_CDP_HEADERSYYYYYYYYYXXXX> CPPA_CDP_HEADERSYYYYYYYYYXXXX { get; set; }
        public virtual DbSet<CPPA_EMO_FC_OPN_STK_LINES> CPPA_EMO_FC_OPN_STK_LINES { get; set; }
        public virtual DbSet<CPPA_EMO_FORECAST_LINES> CPPA_EMO_FORECAST_LINES { get; set; }
        public virtual DbSet<CPPA_EMO_FUEL_RATE_LINES> CPPA_EMO_FUEL_RATE_LINES { get; set; }
        public virtual DbSet<CPPA_EMO_FUEL_TEMP_HEADER> CPPA_EMO_FUEL_TEMP_HEADER { get; set; }
        public virtual DbSet<CPPA_EMO_FUEL_TEMP_LINES> CPPA_EMO_FUEL_TEMP_LINES { get; set; }
        public virtual DbSet<CPPA_EMO_PERIODS> CPPA_EMO_PERIODS { get; set; }
        public virtual DbSet<CPPA_PPA_PLT_EMO> CPPA_PPA_PLT_EMO { get; set; }
        public virtual DbSet<CPPA_PPA_PLT_EMOXXXX> CPPA_PPA_PLT_EMOXXXX { get; set; }
        public virtual DbSet<FND_LOOKUP_VALUES> FND_LOOKUP_VALUES { get; set; }
        public virtual DbSet<PPA_APPLICABLE_INVOICES> PPA_APPLICABLE_INVOICES { get; set; }
        public virtual DbSet<PPA_BLOCKS_FUELS> PPA_BLOCKS_FUELS { get; set; }
        public virtual DbSet<PPA_COMP_DEFS> PPA_COMP_DEFS { get; set; }
        public virtual DbSet<PPA_HEADER> PPA_HEADER { get; set; }
        public virtual DbSet<tblFile> tblFiles { get; set; }
        public virtual DbSet<Testing> Testings { get; set; }
        public virtual DbSet<WP_DISCO_CDP_MONTHLY_DATA> WP_DISCO_CDP_MONTHLY_DATA { get; set; }
        public virtual DbSet<WP_FILES> WP_FILES { get; set; }
        public virtual DbSet<WP_GC_HOURLY_DATA_DETAIL> WP_GC_HOURLY_DATA_DETAIL { get; set; }
        public virtual DbSet<WP_GC_HOURLY_DATA_HEADER> WP_GC_HOURLY_DATA_HEADER { get; set; }
        public virtual DbSet<WP_GC_USER_ACCESS> WP_GC_USER_ACCESS { get; set; }
        public virtual DbSet<WP_NPCC_DESPATCH_HEADER> WP_NPCC_DESPATCH_HEADER { get; set; }
        public virtual DbSet<WP_NPCC_HOURLY_DATA_SMRY_DETAIL> WP_NPCC_HOURLY_DATA_SMRY_DETAIL { get; set; }
        public virtual DbSet<WP_NPCC_HOURLY_DATA_SMRY_HEADER> WP_NPCC_HOURLY_DATA_SMRY_HEADER { get; set; }
        public virtual DbSet<WP_NPCC_LOAD_CURTAILMENT> WP_NPCC_LOAD_CURTAILMENT { get; set; }
        public virtual DbSet<WP_NPCC_PLANT_EVENTS> WP_NPCC_PLANT_EVENTS { get; set; }
        public virtual DbSet<WP_PORTAL_USERS> WP_PORTAL_USERS { get; set; }
        public virtual DbSet<WP_SETUP_DISCO> WP_SETUP_DISCO { get; set; }
        public virtual DbSet<WP_SETUP_MENU> WP_SETUP_MENU { get; set; }
        public virtual DbSet<WP_SETUP_MENU_USER_TYPE> WP_SETUP_MENU_USER_TYPE { get; set; }
        public virtual DbSet<WP_SETUP_USER_TYPES> WP_SETUP_USER_TYPES { get; set; }
        public virtual DbSet<WP_NPCC_DESPATCH_DETAIL> WP_NPCC_DESPATCH_DETAIL { get; set; }
        public virtual DbSet<CPPA_PPA_PLT_BLK_FUEL> CPPA_PPA_PLT_BLK_FUEL { get; set; }
        public virtual DbSet<CPPA_PPA_PLT_BLK_FUEL04032019> CPPA_PPA_PLT_BLK_FUEL04032019 { get; set; }
        public virtual DbSet<WP_NPCC_FAILURE_NOTIFICATION> WP_NPCC_FAILURE_NOTIFICATION { get; set; }
        public virtual DbSet<CPPA_CDP_HEADER> CPPA_CDP_HEADER { get; set; }
        public virtual DbSet<PlantBlock> PlantBlocks { get; set; }
        public virtual DbSet<database_firewall_rules> database_firewall_rules { get; set; }
        public virtual DbSet<CPPA_EMO_FORECAST_HEADER> CPPA_EMO_FORECAST_HEADER { get; set; }
        public virtual DbSet<MDINEO_HEADER> MDINEO_HEADER { get; set; }
        public virtual DbSet<MDINEO_LINE> MDINEO_LINE { get; set; }
        public virtual DbSet<WP_NPCC_METERING> WP_NPCC_METERING { get; set; }
    
        public virtual int CrossTab(string select, string pivotCol, string summaries, string groupBy, string otherCols, string extra)
        {
            var selectParameter = select != null ?
                new ObjectParameter("Select", select) :
                new ObjectParameter("Select", typeof(string));
    
            var pivotColParameter = pivotCol != null ?
                new ObjectParameter("PivotCol", pivotCol) :
                new ObjectParameter("PivotCol", typeof(string));
    
            var summariesParameter = summaries != null ?
                new ObjectParameter("Summaries", summaries) :
                new ObjectParameter("Summaries", typeof(string));
    
            var groupByParameter = groupBy != null ?
                new ObjectParameter("GroupBy", groupBy) :
                new ObjectParameter("GroupBy", typeof(string));
    
            var otherColsParameter = otherCols != null ?
                new ObjectParameter("OtherCols", otherCols) :
                new ObjectParameter("OtherCols", typeof(string));
    
            var extraParameter = extra != null ?
                new ObjectParameter("Extra", extra) :
                new ObjectParameter("Extra", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("CrossTab", selectParameter, pivotColParameter, summariesParameter, groupByParameter, otherColsParameter, extraParameter);
        }
    
        public virtual ObjectResult<SP_CPPA_CDP_HEADER_DETAIL_BY_CDP_ID_Result> SP_CPPA_CDP_HEADER_DETAIL_BY_CDP_ID(Nullable<decimal> cDP_ID)
        {
            var cDP_IDParameter = cDP_ID.HasValue ?
                new ObjectParameter("CDP_ID", cDP_ID) :
                new ObjectParameter("CDP_ID", typeof(decimal));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_CPPA_CDP_HEADER_DETAIL_BY_CDP_ID_Result>("SP_CPPA_CDP_HEADER_DETAIL_BY_CDP_ID", cDP_IDParameter);
        }
    
        public virtual ObjectResult<SP_ddlCDPNumber_Result> SP_ddlCDPNumber(string uSER_NAME)
        {
            var uSER_NAMEParameter = uSER_NAME != null ?
                new ObjectParameter("USER_NAME", uSER_NAME) :
                new ObjectParameter("USER_NAME", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_ddlCDPNumber_Result>("SP_ddlCDPNumber", uSER_NAMEParameter);
        }
    
        public virtual ObjectResult<SP_ddlDISCOs_Result> SP_ddlDISCOs(string uSER_NAME)
        {
            var uSER_NAMEParameter = uSER_NAME != null ?
                new ObjectParameter("USER_NAME", uSER_NAME) :
                new ObjectParameter("USER_NAME", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_ddlDISCOs_Result>("SP_ddlDISCOs", uSER_NAMEParameter);
        }
    
        public virtual ObjectResult<SP_ddlMonths_Result> SP_ddlMonths()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_ddlMonths_Result>("SP_ddlMonths");
        }
    
        public virtual int SP_DEL_DRAFT_DAC_OLD()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_DEL_DRAFT_DAC_OLD");
        }
    
        public virtual ObjectResult<SP_DISCO_MONTHLY_REPORT_Result> SP_DISCO_MONTHLY_REPORT(Nullable<decimal> cUSTOMER_ID, Nullable<decimal> aCCOUNTING_MONTH_ID)
        {
            var cUSTOMER_IDParameter = cUSTOMER_ID.HasValue ?
                new ObjectParameter("CUSTOMER_ID", cUSTOMER_ID) :
                new ObjectParameter("CUSTOMER_ID", typeof(decimal));
    
            var aCCOUNTING_MONTH_IDParameter = aCCOUNTING_MONTH_ID.HasValue ?
                new ObjectParameter("ACCOUNTING_MONTH_ID", aCCOUNTING_MONTH_ID) :
                new ObjectParameter("ACCOUNTING_MONTH_ID", typeof(decimal));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_DISCO_MONTHLY_REPORT_Result>("SP_DISCO_MONTHLY_REPORT", cUSTOMER_IDParameter, aCCOUNTING_MONTH_IDParameter);
        }
    
        public virtual ObjectResult<SP_DISCO_MONTHLY_REPORT_CPPA_VERIFICATION_Result> SP_DISCO_MONTHLY_REPORT_CPPA_VERIFICATION(Nullable<decimal> cUSTOMER_ID, Nullable<decimal> aCCOUNTING_MONTH_ID, Nullable<int> selectionType, string sortSelection)
        {
            var cUSTOMER_IDParameter = cUSTOMER_ID.HasValue ?
                new ObjectParameter("CUSTOMER_ID", cUSTOMER_ID) :
                new ObjectParameter("CUSTOMER_ID", typeof(decimal));
    
            var aCCOUNTING_MONTH_IDParameter = aCCOUNTING_MONTH_ID.HasValue ?
                new ObjectParameter("ACCOUNTING_MONTH_ID", aCCOUNTING_MONTH_ID) :
                new ObjectParameter("ACCOUNTING_MONTH_ID", typeof(decimal));
    
            var selectionTypeParameter = selectionType.HasValue ?
                new ObjectParameter("SelectionType", selectionType) :
                new ObjectParameter("SelectionType", typeof(int));
    
            var sortSelectionParameter = sortSelection != null ?
                new ObjectParameter("SortSelection", sortSelection) :
                new ObjectParameter("SortSelection", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_DISCO_MONTHLY_REPORT_CPPA_VERIFICATION_Result>("SP_DISCO_MONTHLY_REPORT_CPPA_VERIFICATION", cUSTOMER_IDParameter, aCCOUNTING_MONTH_IDParameter, selectionTypeParameter, sortSelectionParameter);
        }
    
        public virtual ObjectResult<string> SP_IMPORT_EMO_FORECAST(Nullable<decimal> fORECAST_HEADER_ID)
        {
            var fORECAST_HEADER_IDParameter = fORECAST_HEADER_ID.HasValue ?
                new ObjectParameter("FORECAST_HEADER_ID", fORECAST_HEADER_ID) :
                new ObjectParameter("FORECAST_HEADER_ID", typeof(decimal));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<string>("SP_IMPORT_EMO_FORECAST", fORECAST_HEADER_IDParameter);
        }
    
        public virtual ObjectResult<SP_IMPORT_LAST_DAC_Result> SP_IMPORT_LAST_DAC(Nullable<decimal> pLT_BLK_FUEL_ID, string hOURLY_DATA_TYPE)
        {
            var pLT_BLK_FUEL_IDParameter = pLT_BLK_FUEL_ID.HasValue ?
                new ObjectParameter("PLT_BLK_FUEL_ID", pLT_BLK_FUEL_ID) :
                new ObjectParameter("PLT_BLK_FUEL_ID", typeof(decimal));
    
            var hOURLY_DATA_TYPEParameter = hOURLY_DATA_TYPE != null ?
                new ObjectParameter("HOURLY_DATA_TYPE", hOURLY_DATA_TYPE) :
                new ObjectParameter("HOURLY_DATA_TYPE", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_IMPORT_LAST_DAC_Result>("SP_IMPORT_LAST_DAC", pLT_BLK_FUEL_IDParameter, hOURLY_DATA_TYPEParameter);
        }
    
        public virtual ObjectResult<SP_LAST_METER_Reading_By_CDP_DTL_ID_Result> SP_LAST_METER_Reading_By_CDP_DTL_ID(Nullable<decimal> cDP_DTL_ID, string iS_PRIMARY_METER, Nullable<decimal> cUSTOMER_ID)
        {
            var cDP_DTL_IDParameter = cDP_DTL_ID.HasValue ?
                new ObjectParameter("CDP_DTL_ID", cDP_DTL_ID) :
                new ObjectParameter("CDP_DTL_ID", typeof(decimal));
    
            var iS_PRIMARY_METERParameter = iS_PRIMARY_METER != null ?
                new ObjectParameter("IS_PRIMARY_METER", iS_PRIMARY_METER) :
                new ObjectParameter("IS_PRIMARY_METER", typeof(string));
    
            var cUSTOMER_IDParameter = cUSTOMER_ID.HasValue ?
                new ObjectParameter("CUSTOMER_ID", cUSTOMER_ID) :
                new ObjectParameter("CUSTOMER_ID", typeof(decimal));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_LAST_METER_Reading_By_CDP_DTL_ID_Result>("SP_LAST_METER_Reading_By_CDP_DTL_ID", cDP_DTL_IDParameter, iS_PRIMARY_METERParameter, cUSTOMER_IDParameter);
        }
    
        public virtual ObjectResult<SP_LOAD_FUEL_RATES_BY_FORECAST_HEADER_ID_Result> SP_LOAD_FUEL_RATES_BY_FORECAST_HEADER_ID(Nullable<decimal> fORECAST_HEADER_ID)
        {
            var fORECAST_HEADER_IDParameter = fORECAST_HEADER_ID.HasValue ?
                new ObjectParameter("FORECAST_HEADER_ID", fORECAST_HEADER_ID) :
                new ObjectParameter("FORECAST_HEADER_ID", typeof(decimal));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_LOAD_FUEL_RATES_BY_FORECAST_HEADER_ID_Result>("SP_LOAD_FUEL_RATES_BY_FORECAST_HEADER_ID", fORECAST_HEADER_IDParameter);
        }
    
        public virtual ObjectResult<SP_MENU_BY_USER_ID_Result> SP_MENU_BY_USER_ID(string uSER_NAME)
        {
            var uSER_NAMEParameter = uSER_NAME != null ?
                new ObjectParameter("USER_NAME", uSER_NAME) :
                new ObjectParameter("USER_NAME", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_MENU_BY_USER_ID_Result>("SP_MENU_BY_USER_ID", uSER_NAMEParameter);
        }
    
        public virtual int SP_METER_READING_PERFORMA_PREVIOUS_DATA(Nullable<decimal> cDP_ID, Nullable<int> aCCOUNTING_MONTH_ID)
        {
            var cDP_IDParameter = cDP_ID.HasValue ?
                new ObjectParameter("CDP_ID", cDP_ID) :
                new ObjectParameter("CDP_ID", typeof(decimal));
    
            var aCCOUNTING_MONTH_IDParameter = aCCOUNTING_MONTH_ID.HasValue ?
                new ObjectParameter("ACCOUNTING_MONTH_ID", aCCOUNTING_MONTH_ID) :
                new ObjectParameter("ACCOUNTING_MONTH_ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_METER_READING_PERFORMA_PREVIOUS_DATA", cDP_IDParameter, aCCOUNTING_MONTH_IDParameter);
        }
    
        public virtual int SP_METER_READING_PERFORMA_PREVIOUS_DATA_Coppy(Nullable<decimal> cDP_ID, Nullable<int> aCCOUNTING_MONTH_ID)
        {
            var cDP_IDParameter = cDP_ID.HasValue ?
                new ObjectParameter("CDP_ID", cDP_ID) :
                new ObjectParameter("CDP_ID", typeof(decimal));
    
            var aCCOUNTING_MONTH_IDParameter = aCCOUNTING_MONTH_ID.HasValue ?
                new ObjectParameter("ACCOUNTING_MONTH_ID", aCCOUNTING_MONTH_ID) :
                new ObjectParameter("ACCOUNTING_MONTH_ID", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SP_METER_READING_PERFORMA_PREVIOUS_DATA_Coppy", cDP_IDParameter, aCCOUNTING_MONTH_IDParameter);
        }
    
        public virtual ObjectResult<SP_MRP_EDIT_DATA_BY_WP_DISCO_CDP_MONTHLY_DATA_ID_Result> SP_MRP_EDIT_DATA_BY_WP_DISCO_CDP_MONTHLY_DATA_ID(Nullable<decimal> wP_DISCO_CDP_MONTHLY_DATA_ID)
        {
            var wP_DISCO_CDP_MONTHLY_DATA_IDParameter = wP_DISCO_CDP_MONTHLY_DATA_ID.HasValue ?
                new ObjectParameter("WP_DISCO_CDP_MONTHLY_DATA_ID", wP_DISCO_CDP_MONTHLY_DATA_ID) :
                new ObjectParameter("WP_DISCO_CDP_MONTHLY_DATA_ID", typeof(decimal));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_MRP_EDIT_DATA_BY_WP_DISCO_CDP_MONTHLY_DATA_ID_Result>("SP_MRP_EDIT_DATA_BY_WP_DISCO_CDP_MONTHLY_DATA_ID", wP_DISCO_CDP_MONTHLY_DATA_IDParameter);
        }
    
        public virtual ObjectResult<sp_NPCC_Declaration_Avalable_Capacity_Load_Result> sp_NPCC_Declaration_Avalable_Capacity_Load(Nullable<decimal> wP_GC_HOURLY_DATA_HEADER_ID_PK)
        {
            var wP_GC_HOURLY_DATA_HEADER_ID_PKParameter = wP_GC_HOURLY_DATA_HEADER_ID_PK.HasValue ?
                new ObjectParameter("WP_GC_HOURLY_DATA_HEADER_ID_PK", wP_GC_HOURLY_DATA_HEADER_ID_PK) :
                new ObjectParameter("WP_GC_HOURLY_DATA_HEADER_ID_PK", typeof(decimal));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_NPCC_Declaration_Avalable_Capacity_Load_Result>("sp_NPCC_Declaration_Avalable_Capacity_Load", wP_GC_HOURLY_DATA_HEADER_ID_PKParameter);
        }
    
        public virtual ObjectResult<sp_NPCC_Declaration_Avalable_Capacity_Load_DAD_Result> sp_NPCC_Declaration_Avalable_Capacity_Load_DAD(Nullable<decimal> wP_GC_HOURLY_DATA_HEADER_ID_PK)
        {
            var wP_GC_HOURLY_DATA_HEADER_ID_PKParameter = wP_GC_HOURLY_DATA_HEADER_ID_PK.HasValue ?
                new ObjectParameter("WP_GC_HOURLY_DATA_HEADER_ID_PK", wP_GC_HOURLY_DATA_HEADER_ID_PK) :
                new ObjectParameter("WP_GC_HOURLY_DATA_HEADER_ID_PK", typeof(decimal));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_NPCC_Declaration_Avalable_Capacity_Load_DAD_Result>("sp_NPCC_Declaration_Avalable_Capacity_Load_DAD", wP_GC_HOURLY_DATA_HEADER_ID_PKParameter);
        }
    
        public virtual ObjectResult<sp_NPCC_Declaration_Avalable_Capacity_LoadRDAC_ADAC_View_Result> sp_NPCC_Declaration_Avalable_Capacity_LoadRDAC_ADAC_View(Nullable<decimal> wP_GC_HOURLY_DATA_HEADER_ID_PK)
        {
            var wP_GC_HOURLY_DATA_HEADER_ID_PKParameter = wP_GC_HOURLY_DATA_HEADER_ID_PK.HasValue ?
                new ObjectParameter("WP_GC_HOURLY_DATA_HEADER_ID_PK", wP_GC_HOURLY_DATA_HEADER_ID_PK) :
                new ObjectParameter("WP_GC_HOURLY_DATA_HEADER_ID_PK", typeof(decimal));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_NPCC_Declaration_Avalable_Capacity_LoadRDAC_ADAC_View_Result>("sp_NPCC_Declaration_Avalable_Capacity_LoadRDAC_ADAC_View", wP_GC_HOURLY_DATA_HEADER_ID_PKParameter);
        }
    
        public virtual ObjectResult<sp_NPCC_NEW_ADAC_RDAC_Result> sp_NPCC_NEW_ADAC_RDAC(Nullable<decimal> wP_GC_HOURLY_DATA_HEADER_ID_PK)
        {
            var wP_GC_HOURLY_DATA_HEADER_ID_PKParameter = wP_GC_HOURLY_DATA_HEADER_ID_PK.HasValue ?
                new ObjectParameter("WP_GC_HOURLY_DATA_HEADER_ID_PK", wP_GC_HOURLY_DATA_HEADER_ID_PK) :
                new ObjectParameter("WP_GC_HOURLY_DATA_HEADER_ID_PK", typeof(decimal));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_NPCC_NEW_ADAC_RDAC_Result>("sp_NPCC_NEW_ADAC_RDAC", wP_GC_HOURLY_DATA_HEADER_ID_PKParameter);
        }
    
        public virtual ObjectResult<SP_ddlMonthsEnergyDelivered_Result> SP_ddlMonthsEnergyDelivered()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_ddlMonthsEnergyDelivered_Result>("SP_ddlMonthsEnergyDelivered");
        }
    
        public virtual ObjectResult<SP_ddlMonths_DISCO_MONTHLY_DATA_Result> SP_ddlMonths_DISCO_MONTHLY_DATA()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_ddlMonths_DISCO_MONTHLY_DATA_Result>("SP_ddlMonths_DISCO_MONTHLY_DATA");
        }
    
        public virtual ObjectResult<SP_ddlMonths_TotalEnergy_Delivered_Result> SP_ddlMonths_TotalEnergy_Delivered()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<SP_ddlMonths_TotalEnergy_Delivered_Result>("SP_ddlMonths_TotalEnergy_Delivered");
        }
    }
}