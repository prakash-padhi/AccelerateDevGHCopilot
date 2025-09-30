using Library.ApplicationCore.Entities;

namespace Library.ApplicationCore;

public interface ILoanRepository {
    Task<Loan?> GetLoan(int loanId);
    Task UpdateLoan(Loan loan);
    Task<IEnumerable<Book>> GetBooksByTitle(string title);
    Task<BookItem?> GetAvailableBookItem(int bookId);
    Task<Loan?> GetActiveLoanForBook(int bookId);
}