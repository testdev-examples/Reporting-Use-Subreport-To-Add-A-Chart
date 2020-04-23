Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports System.Collections.Generic
Imports DevExpress.XtraReports.UI
Imports DevExpress.DataAccess.Sql
Imports DevExpress.DataAccess.ConnectionParameters
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraCharts
Imports DevExpress.XtraReports.Parameters
Imports System.IO

Namespace CreateSubreportsInCode
	Partial Public Class Form1
		Inherits Form

		Public Sub New()
			InitializeComponent()
		End Sub

		Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
			Dim mainReport As XtraReport = CreateMainReport()
			Dim designTool As New ReportDesignTool(mainReport)
			' Comment the next line to load the generated report in Report Designer.
			'designTool.ShowRibbonDesignerDialog();
			Dim printTool As New ReportPrintTool(mainReport)
			' Comment the next line to load the generated report in Print Preview.
			printTool.ShowRibbonPreviewDialog()
			Application.Exit()
		End Sub
		Private Function CreateMainReport() As XtraReport
			Dim report As New XtraReport() With {
				.Bands = {
					New GroupHeaderBand() With {
						.GroupFields = { New GroupField("CategoryID") },
						.Controls = {
							New XRLabel() With {
								.ExpressionBindings = { New ExpressionBinding("BeforePrint", "Text", "[CategoryName]") },
								.BoundsF = New RectangleF(0,0,300,25),
								.Font = New Font(New FontFamily("Arial"),12,FontStyle.Bold)
							},
							New XRLabel() With {
								.ExpressionBindings = { New ExpressionBinding("BeforePrint", "Text", "[Description]") },
								.BoundsF = New RectangleF(0,50,300,25),
								.Font = New Font(New FontFamily("Arial"),9)
							},
							New XRPictureBox() With {
								.ExpressionBindings = { New ExpressionBinding("BeforePrint", "ImageSource", "[Picture]") },
								.BoundsF = New RectangleF(500,0,150,50),
								.Sizing = ImageSizeMode.ZoomImage
							},
							New XRLabel() With {
								.Text = "Product Name",
								.BoundsF = New RectangleF(50,100,400,25),
								.Font = New Font(New FontFamily("Arial"),9,FontStyle.Bold)
							},
							New XRLabel() With {
								.Text = "Qty Per Unit",
								.BoundsF = New RectangleF(450,100,100,25),
								.Font = New Font(New FontFamily("Arial"),9,FontStyle.Bold)
							},
							New XRLabel() With {
								.Text = "Unit Price",
								.BoundsF = New RectangleF(550,100,100,25),
								.TextAlignment = TextAlignment.TopRight,
								.Font = New Font(New FontFamily("Arial"),9,FontStyle.Bold)
							}
						}
					},
					New DetailBand() With {
						.Controls = {
							New XRLabel() With {
								.ExpressionBindings = { New ExpressionBinding("BeforePrint", "Text", "[ProductName]") },
								.BoundsF = New RectangleF(50,0,400,25),
								.Font = New Font(New FontFamily("Arial"),9)
							},
							New XRLabel() With {
								.ExpressionBindings = { New ExpressionBinding("BeforePrint", "Text", "[QuantityPerUnit]") },
								.BoundsF = New RectangleF(450,0,100,25),
								.Font = New Font(New FontFamily("Arial"),9)
							},
							New XRLabel() With {
								.ExpressionBindings = { New ExpressionBinding("BeforePrint", "Text", "[UnitPrice]") },
								.BoundsF = New RectangleF(550,0,100,25),
								.Font = New Font(New FontFamily("Arial"),9),
								.TextAlignment = TextAlignment.TopRight,
								.TextFormatString = "{0:c2}"
							}
						},
						.HeightF = 25
					},
					New GroupFooterBand() With {
						.Controls = {
							New XRSubreport() With {
								.ReportSource = CreateSubReport(), .GenerateOwnPages = True, .ParameterBindings = { New ParameterBinding("subreportCategory",Nothing,"Products.CategoryID") }
							}
						},
						.PageBreak = PageBreak.BeforeBand
					}
				},
				.DataSource = CreateDataSource(),
				.DataMember = "Products"
			}
			Return report
		End Function
		Private Function CreateSubReport() As XtraReport
			Dim report As New XtraReport() With {
				.Bands = {
					New DetailBand() With {
						.Controls = {
							New XRChart() With {
								.BoundsF = New RectangleF(0,0,900,650), .DataMember = "Products", .Series = {
									New Series() With {.ArgumentDataMember = "ProductName"}
								}
							}
						}
					}
				},
				.Parameters = {
					New Parameter() With {
						.Name = "subreportCategory",
						.Type = GetType(System.Int32)
					}
				},
				.Landscape = True,
				.DataSource = CreateDataSource()
			}
			Dim chart = TryCast(report.Bands(0).Controls(0), XRChart)
			chart.Parameters.Add(New XRControlParameter("chartCategory", report.Parameters(0)))
			chart.Series(0).FilterString = "CategoryID=?chartCategory"
			chart.Series(0).ValueDataMembers.AddRange(New String() { "UnitPrice"})
			Return report
		End Function
		Public Function CreateDataSource() As Object
			Dim connectionParameters As New Access97ConnectionParameters(Path.Combine(Path.GetDirectoryName(GetType(Form1).Assembly.Location), "Data/nwind.mdb"), "", "")
			Dim sqlDataSource As New SqlDataSource(connectionParameters)

			Dim queryProducts As New CustomSqlQuery() With {
				.Name = "Products",
				.Sql = "SELECT Products.ProductID,Products.ProductName,Products.UnitPrice,Products.QuantityPerUnit,Categories.CategoryID,Categories.CategoryName,Categories.Description,Categories.Picture FROM Products INNER JOIN Categories ON Products.CategoryID=Categories.CategoryID"
			}
			sqlDataSource.Queries.Add(queryProducts)
			sqlDataSource.Fill()

			Return sqlDataSource
		End Function
	End Class
End Namespace
