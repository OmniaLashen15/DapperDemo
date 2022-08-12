using DapperDemo.Models;
using System.Collections.Generic;

namespace DapperDemo.Repository
{
    public interface IBonusRepository
    {
        List<Employee> GetEmployeeWithCompany(int id);

        Company GetCompanyWithployess(int id);

        List<Company> GetAllCompaniesWithEmployees();

        void AddTestCompaniesWithEmployees(Company objCompany);
        void AddTestCompaniesWithEmployeesWithTransaction(Company objCompany);
        void RemoveRange(int[] companyId);

        List<Company> FilterCompanyByName(string name); 

    }
}
