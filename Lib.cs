using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JournalProject
{
    public static class SQLQueries
    {
        public const string ALL_ARTICLES = "SELECT * FROM articles INNER JOIN journals ON articles.journal_id = journals.journal_id INNER JOIN article_authors ON articles.article_id = article_authors.article_id INNER JOIN authors ON article_authors.author_id = authors.author_id where article_authors.author_order=1";
        public const string ALL_JOURNALS = "SELECT * FROM journals";
        public const string ALL_AUTHORS = "SELECT * FROM authors";

        public const string AUTHORS_OF_ARTICLE = "SELECT authors.* FROM authors INNER JOIN article_authors ON authors.author_id = article_authors.author_id WHERE article_authors.article_id = @article_id ORDER BY article_authors.author_order";
        public const string ARTICLES_OF_AUTHOR = "SELECT * FROM articles INNER JOIN journals ON articles.journal_id = journals.journal_id WHERE article_id IN (SELECT article_id FROM article_authors WHERE author_id = @author_id)";
        public const string ARTICLES_IN_JOURNAL = "SELECT * FROM articles WHERE journal_id = @journal_id ORDER BY page_start";

        public const string INSERT_ARTICLE = "INSERT INTO articles (article_title, page_start, page_end, journal_id) VALUES (@article_title, @page_start, @page_end, @journal_id); SELECT CAST(SCOPE_IDENTITY() AS INT);";
        public const string INSERT_AUTHOR = "INSERT INTO authors (first_name, last_name) VALUES (@first_name, @last_name)";
        public const string INSERT_JOURNAL = "INSERT INTO journals (journal_number, publication_year) VALUES (@journal_number, @publication_year)";

        public const string UPDATE_ARTICLE = "UPDATE articles SET article_title=@article_title, page_start=@page_start, page_end=@page_end, journal_id=@journal_id WHERE article_id=@article_id";
        public const string UPDATE_AUTHOR = "UPDATE authors SET first_name=@first_name, last_name=@last_name WHERE author_id=@author_id";
        public const string UPDATE_JOURNAL = "UPDATE journals SET journal_number=@journal_number, publication_year=@publication_year WHERE journal_id=@journal_id";

        public const string DELETE_ARTICLE = "DELETE FROM articles WHERE article_id=@article_id";
        public const string DELETE_AUTHOR = "DELETE FROM authors WHERE author_id=@author_id";
        public const string DELETE_JOURNAL = "DELETE FROM journals WHERE journal_id=@journal_id";

        public const string SEARCH_ARTICLES = ALL_ARTICLES + " AND FREETEXT(article_title, @search_term)";
        public const string SEARCH_AUTHORS = ALL_AUTHORS + " WHERE FREETEXT((first_name, last_name), @search_term)";

        public const string ADD_AUTHOR_TO_ARTICLE = "INSERT INTO article_authors (article_id, author_id, author_order) VALUES (@article_id, @author_id, @author_order)";
        public const string REMOVE_AUTHORS_FROM_ARTICLE = "DELETE FROM article_authors WHERE article_id=@article_id";

    }

    public class Article
    {
        public int ArticleId { get; set; }
        public string ArticleTitle { get; set; }
        public int PageStart { get; set; }
        public int PageEnd { get; set; }
        public int JournalId { get; set; }
    }

    public class Journal
    {
        public int JournalId { get; set; }
        public int JournalNumber { get; set; }
        public int PublicationYear { get; set; }
    }

    public class Author
    {
        public int AuthorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class ArticleEventArgs : EventArgs
    {
        public int ArticleId { get; set; }
        public string ArticleTitle { get; set; }
        public int PageStart { get; set; }
        public int PageEnd { get; set; }
        public int JournalId { get; set; }
        public ArticleEventArgs(int articleId, string articleTitle, int pageStart, int pageEnd, int journalId)
        {
            articleId = ArticleId;
            articleTitle = ArticleTitle;
            pageStart = PageStart;
            pageEnd = PageEnd;
            journalId = JournalId;
        }
    }

    public class AuthorEventArgs : EventArgs
    {
        public int AuthorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public AuthorEventArgs(int authorId, string firstName, string lastName)
        {
            authorId = AuthorId;
            firstName = FirstName;
            lastName = LastName;
        }
    }

    public class JournalEventArgs : EventArgs
    {
        public int JournalId { get; set; }
        public int JournalNumber { get; set; }
        public int PublicationYear { get; set; }
        public JournalEventArgs(int journalId, int journalNumber, int publicationYear)
        {
            journalId = JournalId;
            journalNumber = JournalNumber;
            publicationYear = PublicationYear;
        }
    }

    public class ObservableArticle
    {
        public Article Article { get; set; }
        public Boolean Selected { get; set; }

        public event EventHandler<ArticleEventArgs> ArticleChanged;

        public void UpdateArticle(int articleId, string articleTitle, int pageStart, int pageEnd, int journalId)
        {
            Selected = true;
            Article.ArticleId = articleId;
            Article.ArticleTitle = articleTitle;
            Article.PageStart = pageStart;
            Article.PageEnd = pageEnd;
            Article.JournalId = journalId;

            OnArticleChanged(new ArticleEventArgs(Article.ArticleId, articleTitle, pageStart, pageEnd, journalId));
        }

        public void UpdateArticle(Article article)
        {
            Selected = true;
            Article = article;
            OnArticleChanged(new ArticleEventArgs(Article.ArticleId, Article.ArticleTitle, Article.PageStart, Article.PageEnd, Article.JournalId));
        }

        public void DeselectArticle()
        {
            Selected = false;
            OnArticleChanged(new ArticleEventArgs(0, String.Empty, 0, 0, 0));
        }

        protected virtual void OnArticleChanged(ArticleEventArgs e)
        {
            ArticleChanged?.Invoke(this, e);
        }
    }

    public class ObservableAuthor
    {
        public Author Author { get; set; }
        public Boolean Selected { get; set; }

        public event EventHandler<AuthorEventArgs> AuthorChanged;

        public void UpdateAuthor(int authorId, string firstName, string lastName)
        {
            Selected = true;
            Author.AuthorId = authorId;
            Author.FirstName = firstName;
            Author.LastName = lastName;

            OnAuthorChanged(new AuthorEventArgs(Author.AuthorId, firstName, lastName));
        }

        public void DeselectAuthor()
        {
            Selected = false;
            OnAuthorChanged(new AuthorEventArgs(0, String.Empty, String.Empty));
        }

        protected virtual void OnAuthorChanged(AuthorEventArgs e)
        {
            AuthorChanged?.Invoke(this, e);
        }
    }

    public class ObservableJournal
    {
        public Journal Journal { get; set; }
        public Boolean Selected { get; set; }

        public event EventHandler<JournalEventArgs> JournalChanged;

        public void UpdateJournal(int journalId, int journalNumber, int publicationYear)
        {
            Selected = true;
            Journal.JournalId = journalId;
            Journal.JournalNumber = journalNumber;
            Journal.PublicationYear = publicationYear;
            OnJournalChanged(new JournalEventArgs(Journal.JournalId, journalNumber, publicationYear));
        }

        public void DeselectJournal()
        {
            Selected = false;
            OnJournalChanged(new JournalEventArgs(0, 0, 0));
        }

        protected virtual void OnJournalChanged(JournalEventArgs e)
        {
            JournalChanged?.Invoke(this, e);
        }
    }

    public class refreshEventArgs : EventArgs
    {
        public string refreshTable { get; set; }
        public Boolean refreshNeeded { get; set; }
        public refreshEventArgs(string refreshTable, bool refreshNeeded)
        {
            this.refreshTable = refreshTable;
            this.refreshNeeded = refreshNeeded;
        }
    }

    public class refreshObservable
    {
        public Boolean refreshNeeded { get; set; }
        public string TableName { get; set; }

        public event EventHandler<refreshEventArgs> RefreshRequested;
        public void RequestRefresh(string table)
        {
            refreshNeeded = true;
            OnRefreshRequested(new refreshEventArgs(table, refreshNeeded));
        }
        public void RefreshComplete()
        {
            refreshNeeded = false;
        }
        protected virtual void OnRefreshRequested(refreshEventArgs e)
        {
            RefreshRequested?.Invoke(this, e);
        }
    }
}
