using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EmployeeManagementSystem.Models;
using Microsoft.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Text;
using Newtonsoft.Json;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace EmployeeManagementSystem.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly EMSDbContext _context;
        

        public EmployeesController(EMSDbContext context)
        {
            _context = context;
        }

        
        [HttpGet("GetAllEmployees")]
        public async Task<IActionResult> Index()
        {
            try
            {
                // Using the stored procedure 
                var data = _context.Employees.FromSqlRaw<Employee>("EXEC AllEmployees").ToList();
                
                return _context.Employees != null ?
                              View(data) :
                              Problem("There are no Employees");

            }
            catch(Exception ex)
            {
                var model = new ErrorViewModel
                {
                    ErrorMessage = ex.Message
                };
                return View("Error", model);
            }
            
            
        }


        // GET: Employees/Details/5
        [HttpGet("GetEmployeeById/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null || _context.Employees == null)
                {
                    return NotFound("Not Found");
                }


                var result = _context.Employees.FromSqlRaw($"exec FindEmployee @p0", id).ToList();
                var employee = result.Single();
                if (employee == null)
                {
                    return NotFound("Employee Not Found With the Id provided");
                }

                return View(employee);
            }
            catch (Exception ex)
            {
                var model = new ErrorViewModel
                {
                    ErrorMessage = ex.Message
                };
                return View("Error", model);
            }

        }


        // GET: Employees/Create
        [HttpGet("CreateEmployee")]
        public IActionResult Create()
        {
            return View();
        }


        // POST: Employees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("CreateEmployee")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,FirstName,LastName,Designation,OfficeLocation,MobileNumber,DateOfBirth,Gender,Manager")] Employee employee)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(employee);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(employee);
            }
            catch (Exception ex)
            {
                var model = new ErrorViewModel
                {
                    ErrorMessage = ex.Message
                };
                return View("Error", model);
            }
            
        }



        // GET: Employees/Edit/5
        [HttpGet("EditEmployee/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null || _context.Employees == null)
                {
                    return NotFound("Not Found");
                }

                var employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                {
                    return NotFound("No Employee Found");
                }
                return View(employee);
            }
            catch (Exception ex)
            {
                var model = new ErrorViewModel
                {
                    ErrorMessage = ex.Message
                };
                return View("Error", model);
            }
            
        }


        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("EditEmployee/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,FirstName,LastName,Designation,OfficeLocation,MobileNumber,DateOfBirth,Gender,Manager")] Employee employee)
        {
           
            if (id != employee.EmployeeId)
            {
                return NotFound("No Employee Found");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeId))
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
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null || _context.Employees == null)
                {
                    return NotFound("No Employee Found");
                }

                var employee = await _context.Employees
                    .FirstOrDefaultAsync(m => m.EmployeeId == id);
                if (employee == null)
                {
                    return NotFound("No Employee Found");
                }

                return View(employee);

            }
            catch (Exception ex)
            {
                var model = new ErrorViewModel
                {
                    ErrorMessage = ex.Message
                };
                return View("Error", model);
            }
            
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                if (_context.Employees == null)
                {
                    return Problem("Entity set 'EMSDbContext.Employees'  is null.");
                }
                var employee = await _context.Employees.FindAsync(id);
                if (employee != null)
                {
                    _context.Employees.Remove(employee);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                var model = new ErrorViewModel
                {
                    ErrorMessage = ex.Message
                };
                return View("Error", model);
            }
            
        }

        private bool EmployeeExists(int id)
        {
            return (_context.Employees?.Any(e => e.EmployeeId == id)).GetValueOrDefault();
        }


        //Implementing the search operation

        public IActionResult Search(string input)
        {
            try
            {
                int data;
                bool IsInteger = int.TryParse(input, out data);

                if (IsInteger)
                {
                    var result = _context.Employees.FirstOrDefault(x => x.EmployeeId == data);
                    var modelList = new List<Employee> { result };
                    return View("Index", modelList);
                }
                else
                {
                    var result = _context.Employees.Where(item =>
                    EF.Functions.Like(item.FirstName, $"%{input}%") ||
                    EF.Functions.Like(item.LastName, $"%{input}%") ||
                    EF.Functions.Like(item.Designation, $"%{input}%") ||
                    EF.Functions.Like(item.OfficeLocation, $"%{input}%") ||
                    EF.Functions.Like(item.MobileNumber, $"%{input}%") ||
                    EF.Functions.Like(item.Gender, $"%{input}%") ||
                    EF.Functions.Like(item.DateOfBirth.ToString(), $"%{input}%") ||
                    EF.Functions.Like(item.Manager, $"%{input}%")
                    ).ToList();
                    return View("Index", result);
                }
            }
            catch (Exception ex)
            {
                var model = new ErrorViewModel
                {
                    ErrorMessage = ex.Message
                };
                return View("Error", model);
            }
            
        }

        /* ----- Implementing the search operation with constraints ----- */

        // First search box must only return results regarding the First Name
        // Second search box must only return results regarding the Last Name
        // Both the search boxes have a search button in common
        // If both first name and last name is entered the matching result of a individual row should be displayed

        public IActionResult ModifiedSearch(string fname, string lname)
        {
            try
            {
                var fnameresult = _context.Employees.Where(item =>
                EF.Functions.Like(item.FirstName, $"%{fname}%")
                ).ToList();
                var lnameresults = _context.Employees.Where(item =>
                    EF.Functions.Like(item.LastName, $"%{lname}%")
                    ).ToList();


                if (string.IsNullOrEmpty(fname) && string.IsNullOrEmpty(lname))
                {
                    return RedirectToAction(nameof(Index));
                }
                else if (!string.IsNullOrEmpty(fname) && !string.IsNullOrEmpty(lname))
                {
                    var result = fnameresult.Where(x => x.LastName.Contains(lname));
                    return View("Index", result);
                }
                else if (!string.IsNullOrEmpty(fname))
                {
                    return View("Index", fnameresult);
                }
                else if (!string.IsNullOrEmpty(lname))
                {
                    return View("Index", lnameresults);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var model = new ErrorViewModel
                {
                    ErrorMessage = ex.Message
                };
                return View("Error", model);
            }

            
        }
        
    }
}