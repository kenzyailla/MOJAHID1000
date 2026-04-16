using System;
using System.Data;

public class BuyReturnReportService
{
    private readonly BuyReturnRepository _repo;

    public BuyReturnReportService(string connectionString)
    {
        _repo = new BuyReturnRepository(connectionString);
    }

    public DataTable GetReport(DateTime from, DateTime to, int? supplierId)
    {
        return _repo.GetBuyReturnsReport(from, to, supplierId);
    }
}