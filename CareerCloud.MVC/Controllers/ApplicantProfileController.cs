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
    public class ApplicantProfileController : Controller
    {
        private readonly CareerCloudContext _context;

        public ApplicantProfileController(CareerCloudContext context)
        {
            _context = context;
        }

    
        // GET: ApplicantProfile
        public async Task<IActionResult> Index()
        {
            var careerCloudContext = _context.ApplicantProfiles.Include(a => a.SecurityLogin).Include(a => a.SystemCountryCode);
            return View(await careerCloudContext.ToListAsync());
        }

        
        // GET: ApplicantProfile/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicantProfilePoco = await _context.ApplicantProfiles
                .Include(a => a.SecurityLogin)
                .Include(a => a.SystemCountryCode)
                .Include(a => a.ApplicantEducation)
                .Include(a => a.ApplicantResume)
                .Include(a => a.ApplicantSkill)
                .Include(a => a.ApplicantJobApplication)
                .Include(a => a.ApplicantWorkHistory)

                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicantProfilePoco == null)
            {
                return NotFound();
            }

            return View(applicantProfilePoco);
        }

        public async Task<IActionResult> ApplyViewForApplicant(Guid? id, String searchString)
        {
            if (id == null)
            {
                return NotFound();
            }
            var CompanyJobs = _context.CompanyJobDescriptions
                            .Include(c => c.CompanyJob);
           
            var careerClouddata = from s in CompanyJobs select s;
            if (!String.IsNullOrEmpty(searchString))
            {            
                careerClouddata = careerClouddata.Where(s => s.JobName.Contains(searchString));             
            }
            ViewData["ApplicantId"] = id;

            return View(await careerClouddata.AsNoTracking().ToListAsync());
        }
        //

      public IActionResult ContinueCreatingProfile(Guid loginId)
        {
            ViewData["LoginId"] = loginId;
            ViewData["Country"] = new SelectList(_context.SystemCountryCodes, "Code", "Code");
            return View();
        }
        // GET: ApplicantProfile/Create
        public IActionResult Create()
        {
            ViewData["Login"] = new SelectList(_context.SecurityLogins, "Id", "Id");
            ViewData["Country"] = new SelectList(_context.SystemCountryCodes, "Code", "Code");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateComplete([Bind("Id,Login,CurrentSalary,CurrentRate,Currency,Country,Province,Street,City,PostalCode,TimeStamp")] ApplicantProfilePoco applicantProfilePoco)
        {
            if (ModelState.IsValid)
            {
        
                applicantProfilePoco.Id = Guid.NewGuid();
                

                _context.Add(applicantProfilePoco);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("ContinueCreatingProfile", "ApplicantProfile", new { loginId = applicantProfilePoco.Login });
        }

        // POST: ApplicantProfile/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Login,CurrentSalary,CurrentRate,Currency,Country,Province,Street,City,PostalCode,TimeStamp")] ApplicantProfilePoco applicantProfilePoco)
        {
            if (ModelState.IsValid)
            {
                applicantProfilePoco.Id = Guid.NewGuid();
                _context.Add(applicantProfilePoco);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Login"] = new SelectList(_context.SecurityLogins, "Id", "Id", applicantProfilePoco.Login);
            ViewData["Country"] = new SelectList(_context.SystemCountryCodes, "Code", "Code", applicantProfilePoco.Country);
            return View(applicantProfilePoco);
        }

        // GET: ApplicantProfile/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicantProfilePoco = await _context.ApplicantProfiles.FindAsync(id);
            if (applicantProfilePoco == null)
            {
                return NotFound();
            }
            ViewData["LoginId"] = applicantProfilePoco.Login;
            ViewData["Country"] = new SelectList(_context.SystemCountryCodes, "Code", "Code", applicantProfilePoco.Country);
            return View(applicantProfilePoco);
        }

        // POST: ApplicantProfile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Login,CurrentSalary,CurrentRate,Currency,Country,Province,Street,City,PostalCode,TimeStamp")] ApplicantProfilePoco applicantProfilePoco)
        {
            if (id != applicantProfilePoco.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(applicantProfilePoco);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicantProfilePocoExists(applicantProfilePoco.Id))
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
            ViewData["Login"] = new SelectList(_context.SecurityLogins, "Id", "Id", applicantProfilePoco.Login);
            ViewData["Country"] = new SelectList(_context.SystemCountryCodes, "Code", "Code", applicantProfilePoco.Country);
            return View(applicantProfilePoco);
        }

        // GET: ApplicantProfile/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicantProfilePoco = await _context.ApplicantProfiles
                .Include(a => a.SecurityLogin)
                .Include(a => a.SystemCountryCode)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicantProfilePoco == null)
            {
                return NotFound();
            }

            return View(applicantProfilePoco);
        }

        // POST: ApplicantProfile/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var applicantProfilePoco = await _context.ApplicantProfiles.FindAsync(id);
            _context.ApplicantProfiles.Remove(applicantProfilePoco);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicantProfilePocoExists(Guid id)
        {
            return _context.ApplicantProfiles.Any(e => e.Id == id);
        }
    }
}
