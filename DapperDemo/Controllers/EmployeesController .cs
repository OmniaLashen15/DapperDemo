using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DapperDemo.Data;
using DapperDemo.Models;
using DapperDemo.Repository;

namespace DapperDemo.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IEmployeeRepository _employeeRepo;
        private readonly ICompanyRepository _companyRepo;
        private readonly IBonusRepository _bounsRepo;


        [BindProperty]
        public Employee Employee { get; set; }
        public EmployeesController(IEmployeeRepository employeeRepo,ICompanyRepository companyRepo, IBonusRepository bounsRepo)
        {
            _employeeRepo = employeeRepo;
            _companyRepo = companyRepo;
            _bounsRepo = bounsRepo;
        }

        // GET: Employees
        public async Task<IActionResult> Index(int companyId=0)
        {
            /// its logic in bounsRepo with modified and efficient code
            //List<Employee> employees = _employeeRepo.GetAll();
            //foreach (var employee in employees)
            //{
            //    employee.Company = _companyRepo.Find(employee.CompanyId);
            //}

            List<Employee> employees = _bounsRepo.GetEmployeeWithCompany(companyId);
            return View(employees);
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = _employeeRepo.Find(id.GetValueOrDefault());
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            IEnumerable<SelectListItem> companyList = _companyRepo.GetAll().Select(i=> new SelectListItem
            {
                Text = i.Name,
                Value = i.CompanyId.ToString()
            });
            ViewBag.CompanyList=companyList;
            return View();
        }

        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public async Task<IActionResult> CreatePost()
        {
            if (ModelState.IsValid)
            {
                //_employeeRepo.Add(Employee);
                await _employeeRepo.AddAsync(Employee);

                return RedirectToAction(nameof(Index));
            }
            return View(Employee);
        }

        // GET: Employees/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Employee = _employeeRepo.Find(id.GetValueOrDefault());
            IEnumerable<SelectListItem> companyList = _companyRepo.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.CompanyId.ToString()
            });
            ViewBag.CompanyList = companyList;

            if (Employee == null)
            {
                return NotFound();
            }
            return View(Employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            if (id != Employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                
                
                _employeeRepo.Update(Employee);
                
                return RedirectToAction(nameof(Index));
            }
            return View(Employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            _employeeRepo.Remove(id.GetValueOrDefault());
            return RedirectToAction(nameof(Index));
        }
    }
}
