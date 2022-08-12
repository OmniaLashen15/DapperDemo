using Dapper;
using DapperDemo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;

namespace DapperDemo.Repository
{
    public class BonusRepository : IBonusRepository
    {
        private IDbConnection db;

        public BonusRepository(IConfiguration configuration)
        {
            this.db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public void AddTestCompaniesWithEmployees(Company objCompany)
        {
            var sql = "INSERT INTO Companies (Name, Address, City, State, PostalCode) VALUES(@Name, @Address, @City, @State, @PostalCode);"
                                    + "SELECT CAST(SCOPE_IDENTITY() as int);";  // for retrieving what was the latest record that was inserted, find out its ID and return it

            var id = db.Query<int>(sql, objCompany).Single();
            objCompany.CompanyId = id;

            //foreach(var employee in objCompany.Employees)
            //{
            //    employee.CompanyId = objCompany.CompanyId;
            //    var sql1 = "INSERT INTO Employees (Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId);"
            //           + "SELECT CAST(SCOPE_IDENTITY() as int); ";

            //    db.Query<int>(sql1, employee).Single();
            //}

            /// bulk insert
            objCompany.Employees.Select(c => { c.CompanyId = id; return c; }).ToList();
            var sqlEmp = "INSERT INTO Employees (Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId);"
                     + "SELECT CAST(SCOPE_IDENTITY() as int); ";

            db.Execute(sqlEmp, objCompany.Employees);
        }
        public void AddTestCompaniesWithEmployeesWithTransaction(Company objCompany)
        {
            using(var transaction = new TransactionScope())
            {
                try
                {
                    var sql = "INSERT INTO Companies (Name, Address, City, State, PostalCode) VALUES(@Name, @Address, @City, @State, @PostalCode);"
                        + "SELECT CAST(SCOPE_IDENTITY() as int);";  // for retrieving what was the latest record that was inserted, find out its ID and return it

                    var id = db.Query<int>(sql, objCompany).Single();
                    objCompany.CompanyId = id;


                    /// bulk insert
                    objCompany.Employees.Select(c => { c.CompanyId = id; return c; }).ToList();
                    var sqlEmp = "INSERT INTO Employees (Name, Title, Email, Phone, CompanyId) VALUES(@Name, @Title, @Email, @Phone, @CompanyId);"
                             + "SELECT CAST(SCOPE_IDENTITY() as int); ";

                    db.Execute(sqlEmp, objCompany.Employees);

                    transaction.Complete();
                }
                catch(Exception ex)
                {

                }
            }


        }


        public List<Company> GetAllCompaniesWithEmployees()
        {
            var sql = "SELECT C.*,E.*" +
                           "FROM Employees AS E " +
                           "INNER JOIN " +
                           "Companies AS C " +
                           "ON E.CompanyId = C.CompanyId";

            var companyDic = new Dictionary<int, Company>();

            var company = db.Query<Company, Employee, Company>(sql, (c, e) =>
            {
                if (!companyDic.TryGetValue(c.CompanyId, out var currentCompany))
                {
                    currentCompany = c;
                    companyDic.Add(currentCompany.CompanyId, currentCompany);
                }
                currentCompany.Employees.Add(e);
                return currentCompany;
            }, splitOn: "EmployeeId");

            return company.Distinct().ToList();
        }

        public Company GetCompanyWithployess(int id)
        {
            var p = new
            {
                CompanyId = id
            };

            var sql = "SELECT * FROM COMPANIES WHERE CompanyId = @CompanyId;" +
                "SELECT * FROM Employees WHERE CompanyId = @CompanyId;";

            Company company;

            using (var lists = db.QueryMultiple(sql, p))
            {
                company = lists.Read<Company>().ToList().FirstOrDefault();
                company.Employees = lists.Read<Employee>().ToList();
            };

            return company;
        }

        public List<Employee> GetEmployeeWithCompany(int id)
        {
            var sql = "SELECT E.*,C.*" +
                "FROM Employees AS E " +
                "INNER JOIN " +
                "Companies AS C " +
                "ON E.CompanyId = C.CompanyId";

            if(id != 0)
            {
                sql += " WHERE E.CompanyId = @Id ";
            }

            var employee = db.Query<Employee,Company,Employee>(sql,(e,c) =>
            {
                e.Company = c;
                return e;
            }, new { id}, splitOn: "CompanyId");

            return employee.ToList();
        }

        public void RemoveRange(int[] companyId)
        {
            db.Query("DELETE FROM Companies WHERE CompanyId IN @CompanyId", new {companyId});
        }
        public List<Company> FilterCompanyByName(string name)
        {
            return db.Query<Company>("SELECT * FROM COMPANIES WHERE name like '%' + @name + '%'",new { name }).ToList();
        }
    }
}
