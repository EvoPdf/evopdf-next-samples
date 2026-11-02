using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using System.Drawing;

// Use EVO PDF Namespace
using EvoPdf;

namespace EvoHtmlToPdfDemo.Controllers.PDF_Creator
{
    public class PDF_Creator_Graphic_ElementsController : Controller
    {
        // GET: PDF_Creator_Graphic_Elements
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreatePdf(IFormCollection collection)
        {
            // Create a PDF document
            Document pdfDocument = new Document();

            // Set license key received after purchase to use the converter in licensed mode
            // Leave it not set to use the converter in demo mode
            pdfDocument.LicenseKey = "4W9+bn19bn5ue2B+bn1/YH98YHd3d3c=";

            try
            {
                // The result of adding elements to PDF document
                AddElementResult addElementResult = null;

                // The titles font used to mark various sections of the PDF document
                PdfFont titleFont = pdfDocument.AddFont(new Font("Times New Roman", 12, FontStyle.Bold, GraphicsUnit.Point));

                // The position on X anf Y axes where to add the next element
                float yLocation = 5;
                float xLocation = 5;

                // Create a PDF page in PDF document
                PdfPage pdfPage = pdfDocument.AddPage();

                // Line Elements

                // Add section title
                TextElement titleTextElement = new TextElement(xLocation, yLocation, "Line Elements", titleFont);
                titleTextElement.ForeColor = Color.Black;
                addElementResult = pdfPage.AddElement(titleTextElement);
                yLocation = addElementResult.EndPageBounds.Bottom + 10;
                xLocation += 5;
                pdfPage = addElementResult.EndPdfPage;

                // Add a line with default properties
                LineElement lineElement = new LineElement(xLocation, yLocation, xLocation + 50, yLocation);
                addElementResult = pdfPage.AddElement(lineElement);

                // Add a bold line
                LineElement boldLineElement = new LineElement(xLocation + 60, yLocation, xLocation + 110, yLocation);
                boldLineElement.LineStyle.LineWidth = 3;
                addElementResult = pdfPage.AddElement(boldLineElement);

                // Add dotted line
                LineElement dottedLineElement = new LineElement(xLocation + 120, yLocation, xLocation + 170, yLocation);
                dottedLineElement.LineStyle.LineDashStyle = LineDashStyle.Dot;
                dottedLineElement.ForeColor = Color.Green;
                addElementResult = pdfPage.AddElement(dottedLineElement);

                // Add a dashed line
                LineElement dashedLineElement = new LineElement(xLocation + 180, yLocation, xLocation + 230, yLocation);
                dashedLineElement.LineStyle.LineDashStyle = LineDashStyle.Dash;
                dashedLineElement.ForeColor = Color.Green;
                addElementResult = pdfPage.AddElement(dashedLineElement);

                // Add a dash-dot-dot line
                LineElement dashDotDotLineElement = new LineElement(xLocation + 240, yLocation, xLocation + 290, yLocation);
                dashDotDotLineElement.LineStyle.LineDashStyle = LineDashStyle.DashDotDot;
                dashDotDotLineElement.ForeColor = Color.Green;
                addElementResult = pdfPage.AddElement(dashDotDotLineElement);

                // Add a bold line with rounded cap style
                LineElement roundCapBoldLine = new LineElement(xLocation + 300, yLocation, xLocation + 350, yLocation);
                roundCapBoldLine.LineStyle.LineWidth = 5;
                roundCapBoldLine.LineStyle.LineCapStyle = LineCapStyle.RoundCap;
                roundCapBoldLine.ForeColor = Color.Blue;
                addElementResult = pdfPage.AddElement(roundCapBoldLine);

                // Add a bold line with projecting square cap style
                LineElement projectingSquareCapBoldLine = new LineElement(xLocation + 360, yLocation, xLocation + 410, yLocation);
                projectingSquareCapBoldLine.LineStyle.LineWidth = 5;
                projectingSquareCapBoldLine.LineStyle.LineCapStyle = LineCapStyle.ProjectingSquareCap;
                projectingSquareCapBoldLine.ForeColor = Color.Blue;
                addElementResult = pdfPage.AddElement(projectingSquareCapBoldLine);

                // Add a bold line with projecting butt cap style
                LineElement buttCapBoldLine = new LineElement(xLocation + 420, yLocation, xLocation + 470, yLocation);
                buttCapBoldLine.LineStyle.LineWidth = 5;
                buttCapBoldLine.LineStyle.LineCapStyle = LineCapStyle.ButtCap;
                buttCapBoldLine.ForeColor = Color.Blue;
                addElementResult = pdfPage.AddElement(buttCapBoldLine);

                yLocation = addElementResult.EndPageBounds.Bottom + 3;
                pdfPage = addElementResult.EndPdfPage;

                // Line Join Styles

                // Add section title
                xLocation -= 5;
                yLocation += 10;
                titleTextElement = new TextElement(xLocation, yLocation, "Line Join and Cap Styles", titleFont);
                titleTextElement.ForeColor = Color.Black;
                addElementResult = pdfPage.AddElement(titleTextElement);
                yLocation = addElementResult.EndPageBounds.Bottom + 10;
                xLocation += 5;
                pdfPage = addElementResult.EndPdfPage;

                // Add graphic path with miter join line style
                PathElement miterJoinPath = new PathElement(new PointF(xLocation, yLocation + 50));
                // Add path lines
                miterJoinPath.AddLineSegment(new PointF(xLocation + 25, yLocation));
                miterJoinPath.AddLineSegment(new PointF(xLocation + 50, yLocation + 50));
                // Set path style
                miterJoinPath.LineStyle.LineWidth = 5;
                miterJoinPath.LineStyle.LineCapStyle = LineCapStyle.ProjectingSquareCap;
                miterJoinPath.LineStyle.LineJoinStyle = LineJoinStyle.MiterJoin;
                miterJoinPath.ForeColor = Color.Coral;
                addElementResult = pdfPage.AddElement(miterJoinPath);

                // Add graphic path with round join line style
                PathElement roundJoinPath = new PathElement(new PointF(xLocation + 70, yLocation + 50));
                // Add path lines
                roundJoinPath.AddLineSegment(new PointF(xLocation + 95, yLocation));
                roundJoinPath.AddLineSegment(new PointF(xLocation + 120, yLocation + 50));
                // Set path style
                roundJoinPath.LineStyle.LineWidth = 5;
                roundJoinPath.LineStyle.LineCapStyle = LineCapStyle.RoundCap;
                roundJoinPath.LineStyle.LineJoinStyle = LineJoinStyle.RoundJoin;
                roundJoinPath.ForeColor = Color.Coral;
                addElementResult = pdfPage.AddElement(roundJoinPath);

                // Add graphic path with bevel join line style
                PathElement bevelJoinPath = new PathElement(new PointF(xLocation + 140, yLocation + 50));
                // Add lines to path
                bevelJoinPath.AddLineSegment(new PointF(xLocation + 165, yLocation));
                bevelJoinPath.AddLineSegment(new PointF(xLocation + 190, yLocation + 50));
                // Set path style
                bevelJoinPath.LineStyle.LineWidth = 5;
                bevelJoinPath.LineStyle.LineCapStyle = LineCapStyle.ButtCap;
                bevelJoinPath.LineStyle.LineJoinStyle = LineJoinStyle.BevelJoin;
                bevelJoinPath.ForeColor = Color.Coral;
                // Add element to document
                addElementResult = pdfPage.AddElement(bevelJoinPath);

                // Add a polygon with miter join line style
                PointF[] polygonPoints = new PointF[]{
                                new PointF(xLocation + 210, yLocation + 50),
                                new PointF(xLocation + 235, yLocation),
                                new PointF(xLocation + 260, yLocation + 50)};
                PolygonElement miterJoinPolygon = new PolygonElement(polygonPoints);
                // Set polygon style
                miterJoinPolygon.LineStyle.LineWidth = 5;
                miterJoinPolygon.LineStyle.LineJoinStyle = LineJoinStyle.MiterJoin;
                miterJoinPolygon.ForeColor = Color.Green;
                miterJoinPolygon.BackColor = Color.AliceBlue;
                addElementResult = pdfPage.AddElement(miterJoinPolygon);

                // Add a polygon with round join line style
                polygonPoints = new PointF[]{
                                new PointF(xLocation + 280, yLocation + 50),
                                new PointF(xLocation + 305, yLocation),
                                new PointF(xLocation + 330, yLocation + 50)};
                PolygonElement roundJoinPolygon = new PolygonElement(polygonPoints);
                // Set polygon style
                roundJoinPolygon.LineStyle.LineWidth = 5;
                roundJoinPolygon.LineStyle.LineJoinStyle = LineJoinStyle.RoundJoin;
                roundJoinPolygon.ForeColor = Color.Green;
                roundJoinPolygon.BackColor = Color.Blue;
                addElementResult = pdfPage.AddElement(roundJoinPolygon);

                // Add a polygon with bevel join line style
                polygonPoints = new PointF[]{
                                new PointF(xLocation + 350, yLocation + 50),
                                new PointF(xLocation + 375, yLocation),
                                new PointF(xLocation + 400, yLocation + 50)};
                PolygonElement bevelJoinPolygon = new PolygonElement(polygonPoints);
                // Set polygon style
                bevelJoinPolygon.LineStyle.LineWidth = 5;
                bevelJoinPolygon.LineStyle.LineJoinStyle = LineJoinStyle.BevelJoin;
                bevelJoinPolygon.ForeColor = Color.Green;
                bevelJoinPolygon.BackColor = Color.Blue;
                addElementResult = pdfPage.AddElement(bevelJoinPolygon);

                yLocation = addElementResult.EndPageBounds.Bottom + 3;
                pdfPage = addElementResult.EndPdfPage;

                // Add a Graphics Path Element

                // Add section title
                xLocation -= 5;
                yLocation += 10;
                titleTextElement = new TextElement(xLocation, yLocation, "Path Elements", titleFont);
                titleTextElement.ForeColor = Color.Black;
                addElementResult = pdfPage.AddElement(titleTextElement);
                yLocation = addElementResult.EndPageBounds.Bottom + 10;
                xLocation += 5;
                pdfPage = addElementResult.EndPdfPage;

                // Create the path 
                PathElement graphicsPath = new PathElement(new PointF(xLocation, yLocation));
                // Add line and Bezier curve segments
                graphicsPath.AddLineSegment(new PointF(xLocation + 50, yLocation + 50));
                graphicsPath.AddBezierCurveSegment(new PointF(xLocation + 100, yLocation), new PointF(xLocation + 200, yLocation + 100),
                                new PointF(xLocation + 250, yLocation + 50));
                graphicsPath.AddLineSegment(new PointF(xLocation + 300, yLocation));
                // Close path
                graphicsPath.ClosePath = true;
                // Set path style
                graphicsPath.LineStyle.LineWidth = 3;
                graphicsPath.LineStyle.LineJoinStyle = LineJoinStyle.MiterJoin;
                graphicsPath.LineStyle.LineCapStyle = LineCapStyle.RoundCap;
                graphicsPath.ForeColor = Color.Green;
                //graphicsPath.BackColor = Color.Green;
                graphicsPath.Gradient = new GradientColor(GradientDirection.Vertical, System.Drawing.Color.LightGreen, System.Drawing.Color.Blue);
                // Add element to document
                addElementResult = pdfPage.AddElement(graphicsPath);

                yLocation = addElementResult.EndPageBounds.Bottom + 3;
                pdfPage = addElementResult.EndPdfPage;

                // Add Circle Elements

                // Add section title
                xLocation -= 5;
                yLocation -= 10;
                titleTextElement = new TextElement(xLocation, yLocation, "Circle Elements", titleFont);
                titleTextElement.ForeColor = Color.Black;
                addElementResult = pdfPage.AddElement(titleTextElement);
                yLocation = addElementResult.EndPageBounds.Bottom + 10;
                xLocation += 5;
                pdfPage = addElementResult.EndPdfPage;

                // Add a Circle Element with default settings
                CircleElement circleElement = new CircleElement(xLocation + 30, yLocation + 30, 30);
                addElementResult = pdfPage.AddElement(circleElement);

                // Add dotted circle element
                CircleElement dottedCircleElement = new CircleElement(xLocation + 100, yLocation + 30, 30);
                dottedCircleElement.ForeColor = Color.Green;
                dottedCircleElement.LineStyle.LineDashStyle = LineDashStyle.Dot;
                addElementResult = pdfPage.AddElement(dottedCircleElement);

                // Add a disc
                CircleElement discElement = new CircleElement(xLocation + 170, yLocation + 30, 30);
                discElement.ForeColor = Color.Green;
                discElement.BackColor = Color.LightGray;
                addElementResult = pdfPage.AddElement(discElement);

                // Add disc with bold border
                CircleElement discWithBoldBorder = new CircleElement(xLocation + 240, yLocation + 30, 30);
                discWithBoldBorder.LineStyle.LineWidth = 5;
                discWithBoldBorder.BackColor = Color.Coral;
                discWithBoldBorder.ForeColor = Color.Blue;
                addElementResult = pdfPage.AddElement(discWithBoldBorder);

                // Add colored disc with bold border
                for (int i = 30; i >= 0; i = i - 3)
                {
                    CircleElement coloredDisc = new CircleElement(xLocation + 310, yLocation + 30, i == 0 ? 1 : i);
                    coloredDisc.LineStyle.LineWidth = 3;
                    switch ((i / 3) % 7)
                    {
                        case 0:
                            coloredDisc.BackColor = Color.Red;
                            break;
                        case 1:
                            coloredDisc.BackColor = Color.Orange;
                            break;
                        case 2:
                            coloredDisc.BackColor = Color.Yellow;
                            break;
                        case 3:
                            coloredDisc.BackColor = Color.Green;
                            break;
                        case 4:
                            coloredDisc.BackColor = Color.Blue;
                            break;
                        case 5:
                            coloredDisc.BackColor = Color.Indigo;
                            break;
                        case 6:
                            coloredDisc.BackColor = Color.Violet;
                            break;
                        default:
                            break;
                    }
                    addElementResult = pdfPage.AddElement(coloredDisc);
                }

                // Add a doughnut
                CircleElement exteriorNoBorderDisc = new CircleElement(xLocation + 380, yLocation + 30, 30);
                exteriorNoBorderDisc.BackColor = Color.Coral;
                addElementResult = pdfPage.AddElement(exteriorNoBorderDisc);

                CircleElement interiorNoBorderDisc = new CircleElement(xLocation + 380, yLocation + 30, 15);
                interiorNoBorderDisc.BackColor = Color.White;
                pdfPage.AddElement(interiorNoBorderDisc);

                // Add a simple disc
                CircleElement simpleDisc = new CircleElement(xLocation + 450, yLocation + 30, 30);
                simpleDisc.BackColor = Color.Green;
                addElementResult = pdfPage.AddElement(simpleDisc);

                yLocation = addElementResult.EndPageBounds.Bottom + 3;
                pdfPage = addElementResult.EndPdfPage;

                // Add Ellipse Elements

                // Add section title
                xLocation -= 5;
                yLocation += 10;
                titleTextElement = new TextElement(xLocation, yLocation, "Ellipse Elements", titleFont);
                titleTextElement.ForeColor = Color.Black;
                addElementResult = pdfPage.AddElement(titleTextElement);
                yLocation = addElementResult.EndPageBounds.Bottom + 10;
                xLocation += 5;
                pdfPage = addElementResult.EndPdfPage;

                // Add an Ellipse Element with default settings
                EllipseElement ellipseElement = new EllipseElement(xLocation + 50, yLocation + 30, 50, 30);
                addElementResult = pdfPage.AddElement(ellipseElement);

                // Add an Ellipse Element with background color and line color
                EllipseElement ellipseWithBackgroundAndBorder = new EllipseElement(xLocation + 160, yLocation + 30, 50, 30);
                ellipseWithBackgroundAndBorder.BackColor = Color.LightGray;
                ellipseWithBackgroundAndBorder.ForeColor = Color.Green;
                addElementResult = pdfPage.AddElement(ellipseWithBackgroundAndBorder);

                // Create an ellipse from multiple Ellipse Arc Elements
                EllipseArcElement ellipseArcElement1 = new EllipseArcElement(xLocation + 220, yLocation, 100, 60, 0, 100);
                ellipseArcElement1.ForeColor = Color.Coral;
                ellipseArcElement1.LineStyle.LineWidth = 3;
                addElementResult = pdfPage.AddElement(ellipseArcElement1);

                EllipseArcElement ellipseArcElement2 = new EllipseArcElement(xLocation + 220, yLocation, 100, 60, 100, 100);
                ellipseArcElement2.ForeColor = Color.Blue;
                ellipseArcElement2.LineStyle.LineWidth = 3;
                addElementResult = pdfPage.AddElement(ellipseArcElement2);

                EllipseArcElement ellipseArcElement3 = new EllipseArcElement(xLocation + 220, yLocation, 100, 60, 180, 100);
                ellipseArcElement3.ForeColor = Color.Green;
                ellipseArcElement3.LineStyle.LineWidth = 3;
                addElementResult = pdfPage.AddElement(ellipseArcElement3);

                EllipseArcElement ellipseArcElement4 = new EllipseArcElement(xLocation + 220, yLocation, 100, 60, 270, 100);
                ellipseArcElement4.ForeColor = Color.Violet;
                ellipseArcElement4.LineStyle.LineWidth = 3;
                addElementResult = pdfPage.AddElement(ellipseArcElement4);

                // Create an ellipse from multiple Ellipse Slice Elements
                EllipseSliceElement ellipseSliceElement1 = new EllipseSliceElement(xLocation + 330, yLocation, 100, 60, 0, 90);
                ellipseSliceElement1.BackColor = Color.Coral;
                addElementResult = pdfPage.AddElement(ellipseSliceElement1);

                EllipseSliceElement ellipseSliceElement2 = new EllipseSliceElement(xLocation + 330, yLocation, 100, 60, 90, 90);
                ellipseSliceElement2.BackColor = Color.Blue;
                addElementResult = pdfPage.AddElement(ellipseSliceElement2);

                EllipseSliceElement ellipseSliceElement3 = new EllipseSliceElement(xLocation + 330, yLocation, 100, 60, 180, 90);
                ellipseSliceElement3.BackColor = Color.Green;
                addElementResult = pdfPage.AddElement(ellipseSliceElement3);

                EllipseSliceElement ellipseSliceElement4 = new EllipseSliceElement(xLocation + 330, yLocation, 100, 60, 270, 90);
                ellipseSliceElement4.BackColor = Color.Violet;
                addElementResult = pdfPage.AddElement(ellipseSliceElement4);

                // Add an Ellipse Element with background
                EllipseElement ellipseWithBackground = new EllipseElement(xLocation + 490, yLocation + 30, 50, 30);
                ellipseWithBackground.BackColor = Color.Green;
                addElementResult = pdfPage.AddElement(ellipseWithBackground);

                yLocation = addElementResult.EndPageBounds.Bottom + 3;
                pdfPage = addElementResult.EndPdfPage;

                // Add Rectangle Elements

                // Add section title
                xLocation -= 5;
                yLocation += 10;
                titleTextElement = new TextElement(xLocation, yLocation, "Rectangle Elements", titleFont);
                titleTextElement.ForeColor = Color.Black;
                addElementResult = pdfPage.AddElement(titleTextElement);
                yLocation = addElementResult.EndPageBounds.Bottom + 10;
                xLocation += 5;
                pdfPage = addElementResult.EndPdfPage;

                // Add a Rectangle Element with default settings
                RectangleElement rectangleElement = new RectangleElement(xLocation, yLocation, 100, 60);
                addElementResult = pdfPage.AddElement(rectangleElement);

                // Add a Rectangle Element with background color and dotted line
                RectangleElement rectangleElementWithDottedLine = new RectangleElement(xLocation + 110, yLocation, 100, 60);
                rectangleElementWithDottedLine.BackColor = Color.LightGray;
                rectangleElementWithDottedLine.ForeColor = Color.Green;
                rectangleElementWithDottedLine.LineStyle.LineDashStyle = LineDashStyle.Dot;
                addElementResult = pdfPage.AddElement(rectangleElementWithDottedLine);

                // Add a Rectangle Element with background color without border
                RectangleElement rectangleElementWithoutBorder = new RectangleElement(xLocation + 220, yLocation, 100, 60);
                rectangleElementWithoutBorder.BackColor = Color.Green;
                addElementResult = pdfPage.AddElement(rectangleElementWithoutBorder);

                // Add a Rectangle Element with background color, bold border line and rounded corners
                RectangleElement rectangleElementWithRoundedCorners = new RectangleElement(xLocation + 330, yLocation, 100, 60);
                rectangleElementWithRoundedCorners.BackColor = Color.Coral;
                rectangleElementWithRoundedCorners.ForeColor = Color.Blue;
                rectangleElementWithRoundedCorners.LineStyle.LineWidth = 5;
                rectangleElementWithRoundedCorners.LineStyle.LineJoinStyle = LineJoinStyle.RoundJoin;
                addElementResult = pdfPage.AddElement(rectangleElementWithRoundedCorners);

                yLocation = addElementResult.EndPageBounds.Bottom + 3;
                pdfPage = addElementResult.EndPdfPage;

                // Add Polygon Elements

                // Add section title
                xLocation -= 5;
                yLocation += 10;
                titleTextElement = new TextElement(xLocation, yLocation, "Polygon Elements", titleFont);
                titleTextElement.ForeColor = Color.Black;
                addElementResult = pdfPage.AddElement(titleTextElement);
                yLocation = addElementResult.EndPageBounds.Bottom + 10;
                xLocation += 5;
                pdfPage = addElementResult.EndPdfPage;

                PointF[] polygonElementPoints = new PointF[]{
                    new PointF(xLocation, yLocation + 50),
                    new PointF(xLocation + 50, yLocation),
                    new PointF(xLocation + 100, yLocation + 50),
                    new PointF(xLocation + 50, yLocation + 100)
                };

                // Add a Polygon Element with default settings
                PolygonElement polygonElement = new PolygonElement(polygonElementPoints);
                addElementResult = pdfPage.AddElement(polygonElement);

                polygonElementPoints = new PointF[]{
                    new PointF(xLocation + 110, yLocation + 50),
                    new PointF(xLocation + 160, yLocation),
                    new PointF(xLocation + 210, yLocation + 50),
                    new PointF(xLocation + 160, yLocation + 100)
                };

                // Add a Polygon Element with background color and border
                polygonElement = new PolygonElement(polygonElementPoints);
                polygonElement.BackColor = Color.LightGray;
                polygonElement.ForeColor = Color.Green;
                polygonElement.LineStyle.LineDashStyle = LineDashStyle.Dot;
                addElementResult = pdfPage.AddElement(polygonElement);

                polygonElementPoints = new PointF[]{
                    new PointF(xLocation + 220, yLocation + 50),
                    new PointF(xLocation + 270, yLocation),
                    new PointF(xLocation + 320, yLocation + 50),
                    new PointF(xLocation + 270, yLocation + 100)
                };

                // Add a Polygon Element with background color
                polygonElement = new PolygonElement(polygonElementPoints);
                polygonElement.BackColor = Color.Green;
                addElementResult = pdfPage.AddElement(polygonElement);

                PointF[] polyFillPoints = new PointF[]{
                    new PointF(xLocation + 330, yLocation + 50),
                    new PointF(xLocation + 380, yLocation),
                    new PointF(xLocation + 430, yLocation + 50),
                    new PointF(xLocation + 380, yLocation + 100)
                };

                // Add a Polygon Element with background color and rounded line joins
                PolygonElement polygonElementWithBackgruondColorAndBorder = new PolygonElement(polyFillPoints);
                polygonElementWithBackgruondColorAndBorder.ForeColor = Color.Blue;
                polygonElementWithBackgruondColorAndBorder.BackColor = Color.Coral;
                polygonElementWithBackgruondColorAndBorder.LineStyle.LineWidth = 5;
                polygonElementWithBackgruondColorAndBorder.LineStyle.LineCapStyle = LineCapStyle.RoundCap;
                polygonElementWithBackgruondColorAndBorder.LineStyle.LineJoinStyle = LineJoinStyle.RoundJoin;
                addElementResult = pdfPage.AddElement(polygonElementWithBackgruondColorAndBorder);

                yLocation = addElementResult.EndPageBounds.Bottom + 3;
                pdfPage = addElementResult.EndPdfPage;

                // Add Bezier Curve Elements

                // Add section title
                xLocation -= 5;
                yLocation += 10;
                titleTextElement = new TextElement(xLocation, yLocation, "Bezier Curve Elements", titleFont);
                titleTextElement.ForeColor = Color.Black;
                addElementResult = pdfPage.AddElement(titleTextElement);
                yLocation = addElementResult.EndPageBounds.Bottom + 10;
                xLocation += 5;
                pdfPage = addElementResult.EndPdfPage;

                // Add a Bezier Curve Element with normal style

                BezierCurveElement bezierCurveElement = new BezierCurveElement(xLocation, yLocation + 50, xLocation + 50, yLocation,
                            xLocation + 100, yLocation + 100, xLocation + 150, yLocation + 50);
                bezierCurveElement.ForeColor = Color.Blue;
                bezierCurveElement.LineStyle.LineWidth = 3;
                addElementResult = pdfPage.AddElement(bezierCurveElement);

                // Mark the points controlling the Bezier curve
                CircleElement controlPoint1 = new CircleElement(xLocation + 200, yLocation + 50, 2);
                controlPoint1.BackColor = Color.Violet;
                pdfPage.AddElement(controlPoint1);

                CircleElement controlPoint2 = new CircleElement(xLocation + 250, yLocation, 2);
                controlPoint2.BackColor = Color.Violet;
                pdfPage.AddElement(controlPoint2);

                CircleElement controlPoint3 = new CircleElement(xLocation + 300, yLocation + 100, 2);
                controlPoint3.BackColor = Color.Violet;
                pdfPage.AddElement(controlPoint3);

                CircleElement controlPoint4 = new CircleElement(xLocation + 350, yLocation + 50, 2);
                controlPoint4.BackColor = Color.Violet;
                pdfPage.AddElement(controlPoint4);

                // Add a Bezier Curve Element with dotted line using the controlling points above

                bezierCurveElement = new BezierCurveElement(controlPoint1.X, controlPoint1.Y, controlPoint2.X, controlPoint2.Y,
                            controlPoint3.X, controlPoint3.Y, controlPoint4.X, controlPoint4.Y);
                bezierCurveElement.ForeColor = Color.Green;
                bezierCurveElement.LineStyle.LineDashStyle = LineDashStyle.Dot;
                bezierCurveElement.LineStyle.LineWidth = 1;
                addElementResult = pdfPage.AddElement(bezierCurveElement);

                // Save the PDF document in a memory buffer
                byte[] outPdfBuffer = pdfDocument.Save();
                
                // Send the PDF file to browser
                FileResult fileResult = new FileContentResult(outPdfBuffer, "application/pdf");
                fileResult.FileDownloadName = "Graphic_Elements.pdf";

                return fileResult;
            }
            finally
            {
                // Close the PDF document
                pdfDocument.Close();
            }
        }
    }
}