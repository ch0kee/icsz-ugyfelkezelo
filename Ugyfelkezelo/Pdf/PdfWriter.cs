using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace Ugyfelkezelo.Pdf
{
    class PdfWriter
    {
        public void dsds()
        {
            PdfDocument document = new PdfDocument();
            document.Info.Title = "Jelentés";
        }
    }
}
