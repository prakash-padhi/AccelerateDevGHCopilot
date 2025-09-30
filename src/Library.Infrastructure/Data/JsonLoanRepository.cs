using Library.ApplicationCore;
using Library.ApplicationCore.Entities;

namespace Library.Infrastructure.Data;

public class JsonLoanRepository : ILoanRepository
{
    private readonly JsonData _jsonData;

    public JsonLoanRepository(JsonData jsonData)
    {
        _jsonData = jsonData;
    }

    public async Task<Loan?> GetLoan(int id)
    {
        await _jsonData.EnsureDataLoaded();

        foreach (Loan loan in _jsonData.Loans!)
        {
            if (loan.Id == id)
            {
                Loan populated = _jsonData.GetPopulatedLoan(loan);
                return populated;
            }
        }
        return null;
    }

    public async Task UpdateLoan(Loan loan)
    {
        Loan? existingLoan = null;
        foreach (Loan l in _jsonData.Loans!)
        {
            if (l.Id == loan.Id)
            {
                existingLoan = l;
                break;
            }
        }

        if (existingLoan != null)
        {
            existingLoan.BookItemId = loan.BookItemId;
            existingLoan.PatronId = loan.PatronId;
            existingLoan.LoanDate = loan.LoanDate;
            existingLoan.DueDate = loan.DueDate;
            existingLoan.ReturnDate = loan.ReturnDate;

            await _jsonData.SaveLoans(_jsonData.Loans!);

            await _jsonData.LoadData();
        }
    }

    public async Task<IEnumerable<Book>> GetBooksByTitle(string title)
    {
        await _jsonData.EnsureDataLoaded();
        return _jsonData.Books!.Where(b => b.Title.Contains(title, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<BookItem?> GetAvailableBookItem(int bookId)
    {
        await _jsonData.EnsureDataLoaded();
        var bookItems = _jsonData.BookItems!.Where(bi => bi.BookId == bookId);
        foreach (var item in bookItems)
        {
            // A book item is available if there are no active (unreturned) loans for it
            bool isOnLoan = _jsonData.Loans!.Any(l => l.BookItemId == item.Id && l.ReturnDate == null);
            if (!isOnLoan)
            {
                return _jsonData.GetPopulatedBookItem(item);
            }
        }
        return null;
    }

    public async Task<Loan?> GetActiveLoanForBook(int bookId)
    {
        await _jsonData.EnsureDataLoaded();
        var bookItems = _jsonData.BookItems!.Where(bi => bi.BookId == bookId);
        foreach (var item in bookItems)
        {
            var loan = _jsonData.Loans!.FirstOrDefault(l => l.BookItemId == item.Id && l.ReturnDate == null);
            if (loan != null)
            {
                return _jsonData.GetPopulatedLoan(loan);
            }
        }
        return null;
    }
}