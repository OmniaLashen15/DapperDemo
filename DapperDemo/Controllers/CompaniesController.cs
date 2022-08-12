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
    public class CompaniesController : Controller
    {
        private readonly ICompanyRepository _companyRepo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IBonusRepository _bonusRepo;
        private readonly IDapperSprocRepo _dapperSprocRepo;

        public CompaniesController(ICompanyRepository companyRepo, IEmployeeRepository employeeRepo, IBonusRepository bonusRepo, IDapperSprocRepo dapperSprocRepo)
        {
            _companyRepo = companyRepo;
            _employeeRepo = employeeRepo;
            _bonusRepo = bonusRepo;
            _dapperSprocRepo = dapperSprocRepo;
        }

        // GET: Companies
        public async Task<IActionResult> Index()
        {
            return View(_companyRepo.GetAll());

            ///Generic Repo
            //return View(_dapperSprocRepo.List<Company>("usp_GetAllCompany"));

        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = _bonusRepo.GetCompanyWithployess(id.GetValueOrDefault());
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CompanyId,Name,Address,City,State,PostalCode")] Company company)
        {
            if (ModelState.IsValid)
            {
                _companyRepo.Add(company);
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var company = _companyRepo.Find(id.GetValueOrDefault());

            ///Generic Repo
            //var company = _dapperSprocRepo.Single<Company>("usp_GetCompany",new  {CompanyId = id.GetValueOrDefault()  });

            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CompanyId,Name,Address,City,State,PostalCode")] Company company)
        {
            if (id != company.CompanyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                
                
                _companyRepo.Update(company);
                
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            _companyRepo.Remove(id.GetValueOrDefault());
            return RedirectToAction(nameof(Index));
        }

        //// POST: Companies/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var company = await _context.Companies.FindAsync(id);
        //    _context.Companies.Remove(company);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool CompanyExists(int id)
        //{
        //    return _context.Companies.Any(e => e.CompanyId == id);
        //}
    }
}
