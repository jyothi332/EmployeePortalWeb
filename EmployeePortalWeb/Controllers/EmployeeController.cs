
using Azure;
using EmployeeModelPackage;
using EmployeePortalWeb.Models;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Mvc;

using System.IO;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using Document = iText.Layout.Document;
using Path = System.IO.Path;

namespace EmployeePortalWeb.Controllers
{
    public class EmployeeController : Controller
    {
        
        private readonly string rootPath;
        private readonly ApiService apiService;

        public EmployeeController( IWebHostEnvironment webHost)
        {
            rootPath = webHost.ContentRootPath;
            apiService = new ApiService();
        }

        
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        


        [HttpPost]
        public async Task<IActionResult> Add(EmployeeModel viewModel)
        {
            if (viewModel != null)
            {

                
                var response = await apiService.GetScalarListObject<(int,string,bool)>(viewModel);
               
                TempData["SuccessMessage"] = response.Item2;
                
                return RedirectToAction("Add", new { id = viewModel.Id });
            }
            else
            {
                TempData["ErrorMessage"] = "Can't update the employee.";
                return RedirectToAction("Add", new { id = viewModel.Id });
            }
        }





        [HttpGet]
        public async Task<IActionResult> List(EmployeeModel viewModel)
        {
            var employees = await apiService.GetEmployeeList();
            return View(employees);
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = new EmployeeModel { Id = id };

            var result = await apiService.GetEmployeeList(); //await apiService.GetScalarListObject<List<Tuple<int, string, bool>>>(model);
            var res = result.Where(i => i.Id == id).FirstOrDefault();
            if (res == null)
            {
                TempData["ErrorMessage"] = "No records found for the given ID.";
                return RedirectToAction("List");
            }
            else
            {
                return View(res);
                
            }
        }



        [HttpPost]
        public async Task<IActionResult> UpdateEmployeeData(EmployeeModel viewModel)
        {

            Tuple<int, string, bool> response = await apiService.UpdateEmployeeData(viewModel);
            if (response != null && response.Item3)
            {
                TempData["SuccessMessage"] = response.Item2; 
            }
            
            return RedirectToAction("Edit", new { id = viewModel.Id });
        }





        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (id > 0)
            {
                var model = new EmployeeModel { Id = id };
                var response = await apiService.DeleteEmployee(id); 

                TempData["SuccessMessage"] = response;
                return RedirectToAction("List");
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid employee ID for deletion.";
                return RedirectToAction("List");
            }
        }
        
      


        public async Task<IActionResult> DownloadPdf(EmployeeModel model)
        {
            var employees = await apiService.GetEmployeeList();

            var fileName = $"EmployeeList_{DateTime.Now:yyyyMMddHHmmss}.pdf";
            var pdfFolder = Path.Combine(rootPath, "wwwroot", "pdfs");
            var filePath = Path.Combine(pdfFolder, fileName);

            
            Directory.CreateDirectory(pdfFolder);

            
            using (var writer = new PdfWriter(filePath))
            {
                var pdf = new PdfDocument(writer);
                var doc = new Document(pdf, PageSize.A4);
                doc.SetMargins(20, 20, 20, 20);

                doc.Add(new Paragraph("Employee List")
                    .SetFontSize(18)
                    .SetBold()
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                doc.Add(new Paragraph("\n"));

                Table table = new Table(4).UseAllAvailableWidth();
                table.AddHeaderCell("Name");
                table.AddHeaderCell("Position");
                table.AddHeaderCell("Salary");
                table.AddHeaderCell("Email");

                foreach (var emp in employees)
                {
                    table.AddCell(emp.Name);
                    table.AddCell(emp.Position);
                    table.AddCell(emp.Salary.ToString("C"));
                    table.AddCell(emp.Email);
                }

                doc.Add(table);
                doc.Close();
            }

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return File(stream, "application/pdf", fileName);
        }
    }
}
