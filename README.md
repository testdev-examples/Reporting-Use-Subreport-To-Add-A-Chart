<!-- default file list -->
*Files to look at*:

* [Form1.cs](./CS/Use-Subreport-To-Add-A-Chart/Form1.cs)
<!-- default file list end -->
# Use Subreports to Add a Chart

This project performs the following tasks:

1. Create the main report that lists products grouped by category.
   - The report's [GroupHeader](https://docs.devexpress.com/XtraReports/DevExpress.XtraReports.UI.GroupHeaderBand) band displays the category name.
     - The band's [GroupFields](https://docs.devexpress.com/XtraReports/DevExpress.XtraReports.UI.GroupHeaderBand.GroupFields) collection specifies the group field (the **CategoryID** field).
   - The report's [Detail](https://docs.devexpress.com/XtraReports/DevExpress.XtraReports.UI.DetailBand) band lists products from the group field's category.
   - The report's [GroupFooter](https://docs.devexpress.com/XtraReports/DevExpress.XtraReports.UI.GroupFooterBand) band contains an [XRSubreport](https://docs.devexpress.com/XtraReports/DevExpress.XtraReports.UI.XRSubreport) control instance.
     - The **XRSubreport**'s [ReportSource](https://docs.devexpress.com/XtraReports/DevExpress.XtraReports.UI.SubreportBase.ReportSource) property references a chart report.
     - An item is added to the **XRSubreport**'s [ParameterBindings](https://docs.devexpress.com/XtraReports/DevExpress.XtraReports.UI.XRSubreport.ParameterBindings) collection to bind the chart report's **subreportCategory** parameter to the main report's group field (the **CategoryID** field).
2. Create a chart report that visualizes a specific category's products and prices.
   - The chart report uses an [XRChart](https://docs.devexpress.com/XtraReports/DevExpress.XtraReports.UI.XRChart) control instance.
   - An item is added to the chart's [Parameters](https://docs.devexpress.com/XtraReports/DevExpress.XtraReports.UI.XRChart.Parameters) collection to bind the **chartCategory** parameter to the report's **subreportCategory** parameter.
   - The chart series' [FilterString](https://docs.devexpress.com/CoreLibraries/DevExpress.XtraCharts.SeriesBase.FilterString) property uses the **chartCategory** parameter value to filter data.

Refer to the [Merge Reports: Use Data-Driven Page Sequence](https://docs.devexpress.com/XtraReports/400691) topic for information on how to add a chart to a report at design time.
