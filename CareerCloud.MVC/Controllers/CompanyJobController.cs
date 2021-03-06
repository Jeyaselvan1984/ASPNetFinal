﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CareerCloud.EntityFrameworkDataAccess;
using CareerCloud.Pocos;

namespace CareerCloud.MVC.Controllers
{
    public class CompanyJobController : Controller
    {
        private readonly CareerCloudContext _context;

        public CompanyJobController(CareerCloudContext context)
        {
            _context = context;
        }

        // GET: CompanyJobPocoes
        public async Task<IActionResult> Index()
        {
            var careerCloudContext = _context.CompanyJobs.Include(c => c.CompanyProfile);
            return View(await careerCloudContext.ToListAsync());
        }

        public async Task<IActionResult> DisplayJobsForGivenCompany(Guid CompanyId,string sortOrder="Id")
        {
            var companyJobData =  _context.CompanyJobs
                .Include(c => c.CompanyProfile)
                .Include(c => c.CompanyProfile.CompanyLocation)
                .Include(c => c.CompanyJobDescription)
                .Include(c => c.CompanyJobEducation)
                .Include(c => c.CompanyJobSkill)
                .Where(m => m.Company == CompanyId);
            ViewBag.IdSortParm = sortOrder == "Id" ? "Id_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            var students = from s in companyJobData
                           select s;
            switch (sortOrder)
            {
                case "Id_desc":
                    students = students.OrderByDescending(s => s.Id);   
                    break;
                case "Date":
                    students = students.OrderBy(s => s.ProfileCreated);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.ProfileCreated);
                    break;
                default:
                    students = students.OrderBy(s => s.Id);
                    break;
            }
            ViewData["CompanyId"] = CompanyId;
            return View( await students.ToListAsync());
        }
        // GET: CompanyJobPocoes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyJobPoco = await _context.CompanyJobs
                .Include(c => c.CompanyProfile)
                .Include(c=>c.CompanyProfile.CompanyLocation)
                .Include(c=>c.CompanyJobDescription)
                .Include(c=>c.CompanyJobEducation)
                .Include(c=>c.CompanyJobSkill)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (companyJobPoco == null)
            {
                return NotFound();
            }

            return View(companyJobPoco);
        }

        // GET: CompanyJobPocoes/Create
        public IActionResult Create()
        {
            ViewData["Company"] = new SelectList(_context.CompanyProfiles, "Id", "Id");
            return View();
        }

        public async Task<IActionResult> CreateJobFlowStart(Guid CompanyId)
        {
            CompanyJobPoco newjob = new CompanyJobPoco();
            newjob.Id = Guid.NewGuid();
            newjob.ProfileCreated = DateTime.Now;
            newjob.IsCompanyHidden = false;
            newjob.IsInactive = false;
            newjob.Company = CompanyId;
            if (ModelState.IsValid)
            {
                _context.Add(newjob);
                await _context.SaveChangesAsync();
                //ViewData["CompanyId"] = newjob.Company;
                //ViewData["JobId"] = newjob.Id;
                return RedirectToAction("Details", "CompanyJob", new { id = newjob.Id });

                //return RedirectToAction("ContinueUpdateJobDescription", "CompanyJobDescription", new { CompanyId = newjob.Company, JobId=newjob.Id });
            }
            return RedirectToAction("DisplayJobsForGivenCompany", "CompanyJob", new { sortOrder="Id", companyId  = CompanyId});
        }
        // POST: CompanyJobPocoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Company,ProfileCreated,IsInactive,IsCompanyHidden,TimeStamp")] CompanyJobPoco companyJobPoco)
        {
            if (ModelState.IsValid)
            {
                companyJobPoco.Id = Guid.NewGuid();
                _context.Add(companyJobPoco);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Company"] = new SelectList(_context.CompanyProfiles, "Id", "Id", companyJobPoco.Company);
            return View(companyJobPoco);
        }

        // GET: CompanyJobPocoes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyJobPoco = await _context.CompanyJobs.FindAsync(id);
            if (companyJobPoco == null)
            {
                return NotFound();
            }
            ViewData["Company"] = new SelectList(_context.CompanyProfiles, "Id", "Id", companyJobPoco.Company);
            return View(companyJobPoco);
        }

        // POST: CompanyJobPocoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Company,ProfileCreated,IsInactive,IsCompanyHidden,TimeStamp")] CompanyJobPoco companyJobPoco)
        {
            if (id != companyJobPoco.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(companyJobPoco);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyJobPocoExists(companyJobPoco.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Company"] = new SelectList(_context.CompanyProfiles, "Id", "Id", companyJobPoco.Company);
            return View(companyJobPoco);
        }

        // GET: CompanyJobPocoes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var companyJobPoco = await _context.CompanyJobs
                .Include(c => c.CompanyProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (companyJobPoco == null)
            {
                return NotFound();
            }

            return View(companyJobPoco);
        }

        // POST: CompanyJobPocoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var companyJobPoco = await _context.CompanyJobs.FindAsync(id);
            _context.CompanyJobs.Remove(companyJobPoco);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyJobPocoExists(Guid id)
        {
            return _context.CompanyJobs.Any(e => e.Id == id);
        }
    }
}
