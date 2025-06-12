using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Syncfusion.Pdf;

namespace JTMS.Helpers
{
    class PageSize
    {
        public static SizeF LableSize => new SizeF(PdfPageSize.A4.Width, PdfPageSize.A4.Height);
    }
}
