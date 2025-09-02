using Microsoft.AspNetCore.Mvc;

namespace EmployeePortalWeb.Controllers
{
    public class ViewAsPdf 
    {
        public string FileName { get; set; }
        public object PageSize { get; set; }
        public object PageOrientation { get; set; }
    }
}