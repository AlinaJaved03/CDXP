using System.Web;
using System.Web.Optimization;

namespace CPPA_ECMWeb
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/carbon8/jquery")
                .Include("~/Content/vendors/bower_components/jquery/dist/jquery.min.js")
                .Include("~/Content/plugins/jquery.easing-master/jquery.easing.min.js")
                .Include("~/Content/plugins/jquery.easing-master/jquery.easing.compatibility.js")
                .Include("~/Content/plugins/moment/min/moment.min.js")
                .Include("~/Content/plugins/bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js")
                .Include("~/Content/plugins/bootstrap-datetimepicker/build/js/bootstrap-datetimepicker.min.js")
                .Include("~/Content/plugins/inputMask/jquery.inputmask.js")
                .Include("~/Content/plugins/inputMask/jquery.inputmask.numeric.extensions.min.js")
                .Include("~/Content/jquery.blockUI.js")

                );

            bundles.Add(new ScriptBundle("~/carbon8/jqueryval")
                .Include("~/Content/vendors/bower_components/bootstrap/dist/js/bootstrap.min.js")
                .Include("~/Content/vendors/bower_components/datatables/media/js/jquery.dataTables.min.js")
                .Include("~/Content/plugins/tableExport.jquery.plugin-master/tableExport.js")
                .Include("~/Content/plugins/html2canvas/html2canvas.min.js")


                .Include("~/Content/vendors/bower_components/waypoints/lib/jquery.waypoints.min.js")
                .Include("~/Content/vendors/bower_components/jquery.counterup/jquery.counterup.min.js")
                .Include("~/Content/dist/js/dropdown-bootstrap-extended.js")
                .Include("~/Content/vendors/jquery.sparkline/dist/jquery.sparkline.min.js")
                .Include("~/Content/vendors/bower_components/owl.carousel/dist/owl.carousel.min.js")


                .Include("~/Content/vendors/bower_components/switchery/dist/switchery.min.js")
                .Include("~/Content/vendors/bower_components/sweetalert/dist/sweetalert.min.js")



                //.Include("~/Content/plugins/selectize/dist/js/selectize.min.js")
                .Include("~/Content/vendors/bower_components/switchery/dist/switchery.min.js")
                //.Include("~/Content/vendors/bower_components/echarts/dist/echarts-en.min.js")
                //.Include("~/Content/vendors/echarts-liquidfill.min.js")
                .Include("~/Content/vendors/vectormap/jquery-jvectormap-2.0.2.min.js")
                .Include("~/Content/vendors/vectormap/jquery-jvectormap-world-mill-en.js")
                .Include("~/Content/dist/js/vectormap-data.js")
                .Include("~/Content/vendors/bower_components/jquery-toast-plugin/dist/jquery.toast.min.js")
                .Include("~/Content/vendors/bower_components/peity/jquery.peity.min.js")
                .Include("~/Content/dist/js/peity-data.js")
                .Include("~/Content/vendors/bower_components/raphael/raphael.min.js")
                .Include("~/Content/vendors/bower_components/morris.js/morris.min.js")
                .Include("~/Content/vendors/bower_components/jquery-toast-plugin/dist/jquery.toast.min.js")
                .Include("~/Content/dist/js/init.js")
                .Include("~/Content/dist/js/dashboard-data.js")
                .Include("~/Content/CPPAGScript.js")
                .Include("~/Content/dist/js/jquery.slimscroll.js"));

            bundles.Add(new StyleBundle("~/carbon8/css")
                .Include("~/Content/plugins/jquery-ui-1.12.1.custom/jquery-ui.min.css", new CssRewriteUrlTransform())
                .Include("~/Content/plugins/bootstrap-datepicker/dist/css/bootstrap-datepicker.min.css", new CssRewriteUrlTransform())
                .Include("~/Content/plugins/bootstrap-datetimepicker/build/css/bootstrap-datetimepicker.css", new CssRewriteUrlTransform())
                .Include("~/Content/vendors/bower_components/datatables/media/css/jquery.dataTables.min.css", new CssRewriteUrlTransform())
                .Include("~/Content/vendors/bower_components/jquery-toast-plugin/dist/jquery.toast.min.css", new CssRewriteUrlTransform())
                .Include("~/Content/vendors/bower_components/morris.js/morris.css", new CssRewriteUrlTransform())
                .Include("~/Content/vendors/vectormap/jquery-jvectormap-2.0.2.css", new CssRewriteUrlTransform())
   //.Include("~/Content/plugins/selectize/dist/css/selectize.bootstrap3.css", new CssRewriteUrlTransform())


   .Include("~/Content/vendors/bower_components/bootstrap/dist/css/bootstrap.min.css", new CssRewriteUrlTransform())
   .Include("~/Content/dist/css/font-awesome.min.css", new CssRewriteUrlTransform())
   .Include("~/Content/dist/css/themify-icons.css", new CssRewriteUrlTransform())
   .Include("~/Content/dist/css/animate.css", new CssRewriteUrlTransform())
   .Include("~/Content/dist/css/simple-line-icons.css", new CssRewriteUrlTransform())
   .Include("~/Content/dist/css/linea-icon.css", new CssRewriteUrlTransform())
   .Include("~/Content/dist/css/pe-icon-7-stroke.css", new CssRewriteUrlTransform())
   .Include("~/Content/dist/css/pe-icon-7-styles.css", new CssRewriteUrlTransform())
   .Include("~/Content/dist/css/material-design-iconic-font.min.css", new CssRewriteUrlTransform())
   .Include("~/Content/dist/css/filter.css", new CssRewriteUrlTransform())
   .Include("~/Content/vendors/bower_components/switchery/dist/switchery.css", new CssRewriteUrlTransform())
   .Include("~/Content/vendors/bower_components/owl.carousel/dist/assets/owl.carousel.min.css", new CssRewriteUrlTransform())
   .Include("~/Content/vendors/bower_components/owl.carousel/dist/assets/owl.theme.default.min.css", new CssRewriteUrlTransform())
   .Include("~/Content/dist/css/lightgallery.css", new CssRewriteUrlTransform())
   .Include("~/Content/vendors/bower_components/awesome-bootstrap-checkbox/awesome-bootstrap-checkbox.css", new CssRewriteUrlTransform())


   .Include("~/Content/vendors/bower_components/sweetalert/dist/sweetalert.css", new CssRewriteUrlTransform())

                .Include("~/Content/dist/css/style.css", new CssRewriteUrlTransform())
                .Include("~/Content/CPPAGStyle.css", new CssRewriteUrlTransform()));
            BundleTable.EnableOptimizations = true;




        }
        //     public static void RegisterBundles(BundleCollection bundles)
        //     {
        //         bundles.Add(new ScriptBundle("~/carbon8/jquery")
        //             .Include("~/Content/vendors/bower_components/jquery/dist/jquery.min.js")
        //             .Include("~/Content/plugins/jquery.easing-master/jquery.easing.min.js")
        //             .Include("~/Content/plugins/jquery.easing-master/jquery.easing.compatibility.js")
        //             .Include("~/Content/plugins/moment/min/moment.min.js")
        //             .Include("~/Content/plugins/bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js")
        //             .Include("~/Content/plugins/bootstrap-datetimepicker/build/js/bootstrap-datetimepicker.min.js")
        //             .Include("~/Content/plugins/inputMask/jquery.inputmask.js")
        //             .Include("~/Content/plugins/inputMask/jquery.inputmask.numeric.extensions.min.js")
        //             );

        //         bundles.Add(new ScriptBundle("~/carbon8/jqueryval")
        //             .Include("~/Content/vendors/bower_components/bootstrap/dist/js/bootstrap.min.js")
        //             .Include("~/Content/vendors/bower_components/datatables/media/js/jquery.dataTables.min.js")
        //             .Include("~/Content/plugins/tableExport.jquery.plugin-master/tableExport.js")
        //             .Include("~/Content/plugins/html2canvas/html2canvas.min.js")


        //             .Include("~/Content/vendors/bower_components/waypoints/lib/jquery.waypoints.min.js")
        //             .Include("~/Content/vendors/bower_components/jquery.counterup/jquery.counterup.min.js")
        //             .Include("~/Content/dist/js/dropdown-bootstrap-extended.js")
        //             .Include("~/Content/vendors/jquery.sparkline/dist/jquery.sparkline.min.js")
        //             .Include("~/Content/vendors/bower_components/owl.carousel/dist/owl.carousel.min.js")


        //             .Include("~/Content/vendors/bower_components/switchery/dist/switchery.min.js")
        //             .Include("~/Content/vendors/bower_components/sweetalert/dist/sweetalert.min.js")




        //             .Include("~/Content/vendors/bower_components/switchery/dist/switchery.min.js")
        //             .Include("~/Content/vendors/bower_components/echarts/dist/echarts-en.min.js")
        //             .Include("~/Content/vendors/echarts-liquidfill.min.js")
        //             .Include("~/Content/vendors/vectormap/jquery-jvectormap-2.0.2.min.js")
        //             .Include("~/Content/vendors/vectormap/jquery-jvectormap-world-mill-en.js")
        //             .Include("~/Content/dist/js/vectormap-data.js")
        //             .Include("~/Content/vendors/bower_components/jquery-toast-plugin/dist/jquery.toast.min.js")
        //             .Include("~/Content/vendors/bower_components/peity/jquery.peity.min.js")
        //             .Include("~/Content/dist/js/peity-data.js")
        //             .Include("~/Content/vendors/bower_components/raphael/raphael.min.js")
        //             .Include("~/Content/vendors/bower_components/morris.js/morris.min.js")
        //             .Include("~/Content/vendors/bower_components/jquery-toast-plugin/dist/jquery.toast.min.js")
        //             .Include("~/Content/dist/js/init.js")
        //             .Include("~/Content/dist/js/dashboard-data.js")
        //             .Include("~/Content/CPPAGScript.js")
        //             .Include("~/Content/dist/js/jquery.slimscroll.js"));

        //         bundles.Add(new StyleBundle("~/carbon8/css")
        //             .Include("~/Content/plugins/jquery-ui-1.12.1.custom/jquery-ui.min.css", new CssRewriteUrlTransform())
        //             .Include("~/Content/plugins/bootstrap-datepicker/dist/css/bootstrap-datepicker.min.css", new CssRewriteUrlTransform())
        //             .Include("~/Content/plugins/bootstrap-datetimepicker/build/css/bootstrap-datetimepicker.css", new CssRewriteUrlTransform())
        //             .Include("~/Content/vendors/bower_components/datatables/media/css/jquery.dataTables.min.css", new CssRewriteUrlTransform())
        //             .Include("~/Content/vendors/bower_components/jquery-toast-plugin/dist/jquery.toast.min.css", new CssRewriteUrlTransform())
        //             .Include("~/Content/vendors/bower_components/morris.js/morris.css", new CssRewriteUrlTransform())
        //             .Include("~/Content/vendors/vectormap/jquery-jvectormap-2.0.2.css", new CssRewriteUrlTransform())



        //.Include("~/Content/vendors/bower_components/bootstrap/dist/css/bootstrap.min.css", new CssRewriteUrlTransform())
        //.Include("~/Content/dist/css/font-awesome.min.css", new CssRewriteUrlTransform())
        //.Include("~/Content/dist/css/themify-icons.css", new CssRewriteUrlTransform())
        //.Include("~/Content/dist/css/animate.css", new CssRewriteUrlTransform())
        //.Include("~/Content/dist/css/simple-line-icons.css", new CssRewriteUrlTransform())
        //.Include("~/Content/dist/css/linea-icon.css", new CssRewriteUrlTransform())
        //.Include("~/Content/dist/css/pe-icon-7-stroke.css", new CssRewriteUrlTransform())
        //.Include("~/Content/dist/css/pe-icon-7-styles.css", new CssRewriteUrlTransform())
        //.Include("~/Content/dist/css/material-design-iconic-font.min.css", new CssRewriteUrlTransform())
        //.Include("~/Content/dist/css/filter.css", new CssRewriteUrlTransform())
        //.Include("~/Content/vendors/bower_components/switchery/dist/switchery.css", new CssRewriteUrlTransform())
        //.Include("~/Content/vendors/bower_components/owl.carousel/dist/assets/owl.carousel.min.css", new CssRewriteUrlTransform())
        //.Include("~/Content/vendors/bower_components/owl.carousel/dist/assets/owl.theme.default.min.css", new CssRewriteUrlTransform())
        //.Include("~/Content/dist/css/lightgallery.css", new CssRewriteUrlTransform())
        //.Include("~/Content/vendors/bower_components/awesome-bootstrap-checkbox/awesome-bootstrap-checkbox.css", new CssRewriteUrlTransform())


        //.Include("~/Content/vendors/bower_components/sweetalert/dist/sweetalert.css", new CssRewriteUrlTransform())

        //             .Include("~/Content/dist/css/style.css", new CssRewriteUrlTransform())
        //             .Include("~/Content/CPPAGStyle.css", new CssRewriteUrlTransform()));
        //         BundleTable.EnableOptimizations = true;




        //     }
    }
}
