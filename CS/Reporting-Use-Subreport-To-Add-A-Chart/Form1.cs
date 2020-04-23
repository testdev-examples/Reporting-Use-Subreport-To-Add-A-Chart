using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using DevExpress.XtraReports.UI;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.XtraPrinting;
using DevExpress.XtraCharts;
using DevExpress.XtraReports.Parameters;
using System.IO;

namespace CreateSubreportsInCode {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e) {
            XtraReport mainReport = CreateMainReport();
            ReportDesignTool designTool = new ReportDesignTool(mainReport);
            // Comment the next line to load the generated report in Report Designer.
            //designTool.ShowRibbonDesignerDialog();
            ReportPrintTool printTool = new ReportPrintTool(mainReport);
            // Comment the next line to load the generated report in Print Preview.
            printTool.ShowRibbonPreviewDialog();
            Application.Exit();
        }
        private XtraReport CreateMainReport() {
            XtraReport report = new XtraReport() {
                Bands = {
                    new GroupHeaderBand() {
                        GroupFields = {
                            new GroupField("CategoryID")
                        },
                        Controls = {
                            new XRLabel() {
                                ExpressionBindings = {
                                    new ExpressionBinding("BeforePrint", "Text", "[CategoryName]")
                                },
                                BoundsF = new RectangleF(0,0,300,25),
                                Font = new Font(new FontFamily("Arial"),12,FontStyle.Bold)
                            },
                            new XRLabel() {
                                ExpressionBindings = {
                                    new ExpressionBinding("BeforePrint", "Text", "[Description]")
                                },
                                BoundsF = new RectangleF(0,50,300,25),
                                Font = new Font(new FontFamily("Arial"),9)
                            },
                            new XRPictureBox() {
                                ExpressionBindings = {
                                    new ExpressionBinding("BeforePrint", "ImageSource", "[Picture]")
                                },
                                BoundsF = new RectangleF(500,0,150,50),
                                Sizing = ImageSizeMode.ZoomImage
                            },
                            new XRLabel() {
                                Text = "Product Name",
                                BoundsF = new RectangleF(50,100,400,25),
                                Font = new Font(new FontFamily("Arial"),9,FontStyle.Bold)
                            },
                            new XRLabel() {
                                Text = "Qty Per Unit",
                                BoundsF = new RectangleF(450,100,100,25),
                                Font = new Font(new FontFamily("Arial"),9,FontStyle.Bold)
                            },
                            new XRLabel() {
                                Text = "Unit Price",
                                BoundsF = new RectangleF(550,100,100,25),
                                TextAlignment = TextAlignment.TopRight,
                                Font = new Font(new FontFamily("Arial"),9,FontStyle.Bold)
                            },
                        }
                    },
                    new DetailBand() {
                        Controls = {
                            new XRLabel() {
                                ExpressionBindings = {
                                    new ExpressionBinding("BeforePrint", "Text", "[ProductName]")
                                },
                                BoundsF = new RectangleF(50,0,400,25),
                                Font = new Font(new FontFamily("Arial"),9)
                            },
                            new XRLabel() {
                                ExpressionBindings = {
                                    new ExpressionBinding("BeforePrint", "Text", "[QuantityPerUnit]")
                                },
                                BoundsF = new RectangleF(450,0,100,25),
                                Font = new Font(new FontFamily("Arial"),9)
                            },
                            new XRLabel() {
                                ExpressionBindings = {
                                    new ExpressionBinding("BeforePrint", "Text", "[UnitPrice]")
                                },
                                BoundsF = new RectangleF(550,0,100,25),
                                Font = new Font(new FontFamily("Arial"),9),
                                TextAlignment = TextAlignment.TopRight,
                                TextFormatString = "{0:c2}"
                            }
                        },
                        HeightF = 25
                    },
                    new GroupFooterBand() {
                        Controls = {
                            new XRSubreport(){
                                ReportSource = CreateSubReport(),
                                GenerateOwnPages = true,
                                ParameterBindings = {
                                    new ParameterBinding("subreportCategory",null,"Products.CategoryID")
                                }
                            }
                        },
                        PageBreak = PageBreak.BeforeBand
                    }
                },
                DataSource = CreateDataSource(),
                DataMember = "Products"
            };
            return report;
        }
        private XtraReport CreateSubReport() {
            XtraReport report = new XtraReport()
            {
                Bands = {
                    new DetailBand() {
                        Controls = {
                            new XRChart(){
                                BoundsF = new RectangleF(0,0,900,650),
                                DataMember = "Products",
                                Series = {
                                    new Series(){
                                        ArgumentDataMember = "ProductName"
                                    }
                                }
                            }
                        }
                    }
                },
                Parameters = {
                    new Parameter(){
                        Name = "subreportCategory",
                        Type = typeof(System.Int32)
                    }
                },
                Landscape = true,
                DataSource = CreateDataSource(),
            };
            var chart = report.Bands[0].Controls[0] as XRChart;
            chart.Parameters.Add(new XRControlParameter("chartCategory", report.Parameters[0]));
            chart.Series[0].FilterString = "CategoryID=?chartCategory";
            chart.Series[0].ValueDataMembers.AddRange(new string[] { "UnitPrice"});
            return report;
        }
        public object CreateDataSource() {
            Access97ConnectionParameters connectionParameters = new Access97ConnectionParameters(Path.Combine(Path.GetDirectoryName(typeof(Form1).Assembly.Location), "Data/nwind.mdb"), "", "");
            SqlDataSource sqlDataSource = new SqlDataSource(connectionParameters);

            CustomSqlQuery queryProducts = new CustomSqlQuery() {
                Name = "Products",
                Sql = "SELECT Products.ProductID,Products.ProductName,Products.UnitPrice,Products.QuantityPerUnit,Categories.CategoryID,Categories.CategoryName,Categories.Description,Categories.Picture FROM Products INNER JOIN Categories ON Products.CategoryID=Categories.CategoryID"
            };
            sqlDataSource.Queries.Add(queryProducts);
            sqlDataSource.Fill();

            return sqlDataSource;
        }
    }
}
