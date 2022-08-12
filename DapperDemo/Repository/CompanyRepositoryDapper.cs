using Dapper;
using DapperDemo.Data;
using DapperDemo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DapperDemo.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private IDbConnection db;
        public CompanyRepository(IConfiguration configuration)
        {
            this.db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }
        public Company Add(Company company)
        {
            var sql = "INSERT INTO Companies (Name, Address, City, State, PostalCode) VALUES(@Name, @Address, @City, @State, @PostalCode);"
                        + "SELECT CAST(SCOPE_IDENTITY() as int);";  // for retrieving what was the latest record that was inserted, find out its ID and return it
        
            //var id = db.Query<int>(sql, new   //int because in the line above the result is casted into int
            //{

            //    company.Name,
            //    company.Address,
            //    company.City,
            //    company.State,
            //    company.PostalCode
            //}).Single();

            /// instead of passing each property dapper is smart enough to understand the object itself
            var id = db.Query<int>(sql,company).Single();
            company.CompanyId = id;
            return company;

        }

        public Company Find(int id)
        {
            //var sql = "SELECT * FROM Company WHERE CompanyId =" + id;  /// not recommended
            var sql = "SELECT * FROM Companies WHERE CompanyId = @Id";
            return db.Query<Company>(sql, new { @Id = id }).Single();
        }

        public List<Company> GetAll()
        {
            var sql = "SELECT * FROM Companies";
            return db.Query<Company>(sql).ToList(); 
        }

        public void Remove(int id)
        {
            var sql = "DELETE FROM Companies WHERE CompanyId = @Id";
            db.Execute(sql, new { id});

            //db.Execute(sql, new { @Id = id });

        }

        public Company Update(Company company)
        {
            var sql = "UPDATE Companies SET Name = @Name, Address = @Address," +
                " City = @City, State = @State, PostalCode = @PostalCode" +
                " WHERE CompanyId = @CompanyId";

            db.Execute(sql,company);
            return company;

        }
    }
}
