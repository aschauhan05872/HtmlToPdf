using IronPdf;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace HtmlToPdfUsingIronPdf.Controllers
{
    [ApiController]
    public class HtmlToPdfController : ControllerBase
    {
        [HttpPost]
        [Route("api/HtmlToPdf/html")]
        public HttpResponseMessage ConvertFromHtml(HtmlModel htmlModel)
        {
            var Renderer = new HtmlToPdf();
            Renderer.PrintOptions.MarginTop = 40; //millimeters
            Renderer.PrintOptions.MarginBottom = 40;
            Renderer.PrintOptions.CssMediaType = PdfPrintOptions.PdfCssMediaType.Print;
            if (!string.IsNullOrEmpty(htmlModel.CustomHeader))
                Renderer.PrintOptions.Header = new HtmlHeaderFooter()
                {
                    HtmlFragment = htmlModel.CustomHeader
                };
            if (string.IsNullOrEmpty(htmlModel.CustomFooter))
                Renderer.PrintOptions.Footer = new SimpleHeaderFooter()
                {
                    LeftText = "Report printed on {date} ",
                    RightText = "Page {page} of {total-pages}",
                    DrawDividerLine = true,
                    FontSize = 8
                };
            else
                Renderer.PrintOptions.Footer = new HtmlHeaderFooter()
                {
                    Height = 15,
                    HtmlFragment = htmlModel.CustomFooter,
                    DrawDividerLine = true
                };
            var PDF = Renderer.RenderHtmlAsPdf(htmlModel.Html);

            var OutputPath = "HtmlToPDF.pdf";
            PDF.SaveAs(OutputPath);
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(PDF.BinaryData);
            response.Content.Headers.ContentLength = PDF.BinaryData.LongLength;
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            response.Content.Headers.ContentDisposition.FileName = string.IsNullOrEmpty(htmlModel.OutputFileName) ? Guid.NewGuid().ToString() : htmlModel.OutputFileName;
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            return response;
        }
    }
}
