using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Reporting.NETCore;
using SchoolAppCore.Models;

namespace SchoolAppCore.Pages.Reports;

public class EnrollmentReportModel : PageModel
{
    private readonly SchoolContext _db;
    private readonly IWebHostEnvironment _env;

    public EnrollmentReportModel(SchoolContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    [BindProperty] public int? DepartmentID { get; set; }
    [BindProperty] public string Format { get; set; } = "HTML5";
    public SelectList DepartmentOptions { get; set; } = null!;

    public async Task OnGetAsync()
    {
        // FIX 1: Changed "DepartmentID" to "DepartmentId" to match the entity property exactly
        DepartmentOptions = new SelectList(
            await _db.Departments.OrderBy(d => d.Name).ToListAsync(), "DepartmentId", "Name");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var rows = await (
            from e in _db.Enrollments
            join s in _db.Students on e.StudentId equals (int?)s.StudentId
            join c in _db.Courses on e.CourseId equals (int?)c.CourseId
            join d in _db.Departments on c.DepartmentId equals (int?)d.DepartmentId
            where DepartmentID == null || d.DepartmentId == DepartmentID
            orderby s.LastName, s.FirstName
            select new EnrollmentRow(s.FirstName + " " + s.LastName, c.Title, d.Name, c.Credits, e.Grade)
        ).ToListAsync();

        var fmt = Format is "EXCELOPENXML" or "WORDOPENXML" or "PDF" ? Format : "HTML5";

        // FIX 2: Added "Pages" to the path to match your actual project structure
        using var rdlc = System.IO.File.OpenRead(
            Path.Combine(_env.ContentRootPath, "Pages", "Reports", "EnrollmentReport.rdlc"));
        
        var report = new LocalReport();
        report.LoadReportDefinition(rdlc);
        report.DataSources.Add(new ReportDataSource("Enrollments", rows));
        byte[] bytes = report.Render(fmt);

        return fmt switch
        {
            "EXCELOPENXML" => File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "EnrollmentReport.xlsx"),
            "WORDOPENXML" => File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "EnrollmentReport.docx"),
            "PDF" => File(bytes, "application/pdf"),
            _ => File(bytes, "text/html; charset=utf-8")

            // Test
        };
    }
}

public record EnrollmentRow(string StudentName, string CourseTitle, string Department, int Credits, decimal? Grade);